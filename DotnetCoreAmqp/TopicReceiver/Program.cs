using Amqp;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TopicReceiver
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
            var subscriptionName = configuration["subscriptionName"];

            var address = new Address($"{serviceBusNamespace}.servicebus.windows.net", 5671, secretUser, secretKey);
            Connection connection = await Connection.Factory.CreateAsync(address);
            Session session = new Session(connection);

            // Azure Service Bus Topic receive
            Console.WriteLine("App will terminate after 60 seconds of no messages.");
            Console.WriteLine();

            ReceiverLink topicReceiver = new ReceiverLink(session, "topic-receiver", $"{topicName}/subscriptions/{subscriptionName}");

            Message message = null;
            do
            {
                message = await topicReceiver.ReceiveAsync();
                if (message == null) continue;
                string msg4Content = string.Empty;
                try
                {
                    msg4Content = message.GetBody<string>();
                }
                catch
                {
                    msg4Content = "<unreadable>";
                }
                Console.WriteLine($"Read '{msg4Content} from '{topicName}/subscriptions/sub1'");
                topicReceiver.Accept(message);
            } while (message != null);

            await topicReceiver.CloseAsync();
            await session.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
