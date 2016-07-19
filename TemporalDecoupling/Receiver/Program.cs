using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;

namespace OrderProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceBusNamesapce = ConfigurationManager.AppSettings["serviceBusNamespace"];
            var serviceBusQueueName = ConfigurationManager.AppSettings["serviceBusQueueName"];
            var serviceBusKeyName   = ConfigurationManager.AppSettings["serviceBusKeyName"];
            var serviceBusAccessKey = ConfigurationManager.AppSettings["serviceBusAccessKey"];

            // Use the "Read" SharedAccessKey from the portal
            var connectionString = 
                $"Endpoint=sb://{serviceBusNamesapce}.servicebus.windows.net/;SharedAccessKeyName={serviceBusKeyName};SharedAccessKey={serviceBusAccessKey}";

            var connectionStringBuilder = new ServiceBusConnectionStringBuilder(connectionString);
            connectionStringBuilder.TransportType = TransportType.Amqp;

            var queueClient = QueueClient.CreateFromConnectionString(
                connectionStringBuilder.ToString(), 
                serviceBusQueueName);

            // Process orders
            queueClient.OnMessage((msg) =>
            {
                var msgBody = msg.GetBody<string>();
                Console.WriteLine($"Processed {msgBody}.");
                
            }, new OnMessageOptions()
            {
                AutoComplete = true // default behavior shown
            });

            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }
    }
}
