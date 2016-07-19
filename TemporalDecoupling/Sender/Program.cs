using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

namespace OrderTaker
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceBusNamesapce = ConfigurationManager.AppSettings["serviceBusNamespace"];
            var serviceBusQueueName = ConfigurationManager.AppSettings["serviceBusQueueName"];
            var serviceBusKeyName   = ConfigurationManager.AppSettings["serviceBusKeyName"];
            var serviceBusAccessKey = ConfigurationManager.AppSettings["serviceBusAccessKey"];

            // Use the "Write" SharedAccessKey from the portal
            var connectionString = 
                $"Endpoint=sb://{serviceBusNamesapce}.servicebus.windows.net/;SharedAccessKeyName={serviceBusKeyName};SharedAccessKey={serviceBusAccessKey}";

            var connectionStringBuilder = new ServiceBusConnectionStringBuilder(connectionString);
            connectionStringBuilder.TransportType = TransportType.Amqp;

            var queueClient = QueueClient.CreateFromConnectionString(
                connectionStringBuilder.ToString(), 
                serviceBusQueueName);

            var lastCount = 0;
            // Place some orders for products
            for (var i = 0; i < 60; i++)
            {
                // Sleep for 1 second and then enqueue messages in a batch
                Thread.Sleep(1000);
                var orders = new Random().Next(100);
                var messages = new List<BrokeredMessage>();
                for (var j = 0; j < orders; j++)
                {
                    messages.Add(new BrokeredMessage($"Order number {++lastCount}"));
                }
                queueClient.SendBatch(messages);
                Console.WriteLine($"I just submitted {messages.Count} more orders to the queue. {{Total: {lastCount}}}");
            }

            Console.WriteLine("Orders complete");
            Console.ReadLine();
        }
    }
}
