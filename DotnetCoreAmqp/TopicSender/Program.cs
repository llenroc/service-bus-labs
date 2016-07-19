using Amqp;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TopicSender
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
            var topicName = configuration["serviceBusTopicName"];
            var secretUser = configuration["serviceBusKeyName"];
            var secretKey = configuration["serviceBusAccessKey"];

            var address = new Address($"{serviceBusNamespace}.servicebus.windows.net", 5671, secretUser, secretKey);
            Connection connection = await Connection.Factory.CreateAsync(address);
            Session session = new Session(connection);

            // Azure Service Bus Topic send and receive
            SenderLink topicSender = new SenderLink(session, "topic-sender", topicName);
            Console.WriteLine("Enter Message: ");
            string userMessage = Console.ReadLine();
            while (!string.IsNullOrWhiteSpace(userMessage))
            {
                var message = new Message(userMessage);
                await topicSender.SendAsync(message);
                userMessage = Console.ReadLine();
            }

            await topicSender.CloseAsync();
            await session.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
