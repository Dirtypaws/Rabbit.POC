using CommandLine;
using CommandLine.Text;
using RabbitMQ.Client;
using System;
using System.Data;
using System.Text;

namespace Publisher
{
    class Program
    {
        private class Options
        {
            [Option('f', "fill", Required = false, HelpText = "Fills the queue with the specified amount of messages", DefaultValue = 0)]
            public int Fill { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        static void Main(string[] args)
        {
            var opts = new Options();
            if(Parser.Default.ParseArguments(args, opts))
            {
                if (opts.Fill == 0)
                    Listen();

                for (int i = 0; i < opts.Fill; i++)
                {
                    var now = DateTime.Now.ToString("ddMMMyyyy hh:mm:ss");
                    Publish($"{i} - {now}");
                }
            }
            else
            {
                opts.GetUsage();
            }
        }

        private static void Listen()
        {
            while (true)
            {
                Console.Write("> ");
                var msg = Console.ReadLine();
                Publish(msg);
            }
        }

        private static void Publish(string msg)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare("test.qu",
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                
                var body = Encoding.UTF8.GetBytes(msg);

                channel.BasicPublish(exchange: "",
                                        routingKey: "test.qu",
                                        basicProperties: properties,
                                        body: body);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" Publisher Sent {0}", msg);
                Console.ResetColor();
            }
        }

        
    }
}
