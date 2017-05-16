using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client.MessagePatterns;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //var sub = new Subscription(channel, "test.qu");
                //foreach (BasicDeliverEventArgs e in sub)
                //{
                //    Console.ForegroundColor = ConsoleColor.Green;
                //    Console.WriteLine(" [x] Received {0}", Encoding.UTF8.GetString(e.Body));
                //    Console.ResetColor();

                //    sub.Ack(e);
                //}

                channel.QueueDeclare(queue: "test.qu",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicQos(0, 1, false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    Thread.Sleep(DateTime.Now.Millisecond);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" [x] Received {0}", message);
                    Console.ResetColor();

                    channel.BasicAck(ea.DeliveryTag, false);
                };

                channel.BasicConsume(queue: "test.qu",
                                     noAck: false,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
