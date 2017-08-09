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
            //var factory = new ConnectionFactory { HostName = "localhost" };
            var factory = new ConnectionFactory { HostName = "localhost", VirtualHost = "dev.dev", UserName = "dev.dev", Password = "dev" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                channel.QueueDeclare(queue: "SQIToGraph.Queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: new Dictionary<string, object>()
                                     {
                                         { "x-dead-letter-exchange", "RetryEx" }
                                     });

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicQos(0, 1, false);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);

                    // Simulate doing some work
                    Thread.Sleep(DateTime.Now.Millisecond);

                    Console.ForegroundColor = ConsoleColor.Green;
                    await Task.Run(() => Console.WriteLine(" [x] Received {0}", message));
                    Console.ResetColor();

                    channel.BasicAck(ea.DeliveryTag, false);
                };

                channel.BasicConsume(queue: "SQIToGraph.Queue", 
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
