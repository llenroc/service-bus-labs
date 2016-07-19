using Amqp;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace QueueSender
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Run().Wait();
        }

        static async Task Run()
        {
            var builder = new ConfigurationBuilder()
                     .AddJsonFile("appsettings.json");
            var configuration = builder.Build();


            var serviceBusNamespace = configuration["serviceBusNamespace"];
            var queueName = configuration["serviceBusQueueName"];
            var secretUser = configuration["serviceBusKeyName"];
            var secretKey = configuration["serviceBusAccessKey"];

            var address = new Address($"{serviceBusNamespace}.servicebus.windows.net", 5671, secretUser, secretKey);
            Connection connection = await Connection.Factory.CreateAsync(address);
            Session session = new Session(connection);

            // Azure Service Bus Queue send and receive
            SenderLink sender = new SenderLink(session, "queue-sender", queueName);
            Console.WriteLine("Enter Message: ");
            string userMessage = Console.ReadLine();
            while(!string.IsNullOrWhiteSpace(userMessage))
            {
                var message = new Message(userMessage);
                await sender.SendAsync(message);
                userMessage = Console.ReadLine();
            }

            await sender.CloseAsync();
            await session.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
