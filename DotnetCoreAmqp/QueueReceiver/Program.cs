using Amqp;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace QueueReceiver
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

            // Azure Service Bus Queue receive
            Console.WriteLine("App will terminate after 60 seconds of no messages.");
            Console.WriteLine();

            ReceiverLink receiver = new ReceiverLink(session, "queue-receiver", queueName);
            Message message = null;
            do {
                message = await receiver.ReceiveAsync();
                if (message == null) continue;
                string msg2Content = string.Empty;
                try
                {
                    msg2Content = message.GetBody<string>();
                }
                catch
                {
                    msg2Content = "<unreadable>";
                }
                Console.WriteLine($"Read '{msg2Content} from '{queueName}'");
                receiver.Accept(message);
            } while (message != null);

            await receiver.CloseAsync();
            await session.CloseAsync();
            await connection.CloseAsync();
        }
    }
}
