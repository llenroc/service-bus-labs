using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;

namespace ApiClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceBusNamesapce = ConfigurationManager.AppSettings["serviceBusNamespace"];
            var serviceBusTopicName = ConfigurationManager.AppSettings["serviceBusTopicName"];
            var serviceBusKeyName   = ConfigurationManager.AppSettings["serviceBusKeyName"];
            var serviceBusAccessKey = ConfigurationManager.AppSettings["serviceBusAccessKey"];

            var topicClient = TopicClient.CreateFromConnectionString(
                $"Endpoint=sb://{serviceBusNamesapce}.servicebus.windows.net/;SharedAccessKeyName={serviceBusKeyName};SharedAccessKey={serviceBusAccessKey}",
                serviceBusTopicName);

            string versionNumber;
            do
            {
                Console.Write("Enter version number: ");
                versionNumber = Console.ReadLine();
                double version;

                if(double.TryParse(versionNumber, out version))
                {
                    var msg = new BrokeredMessage("A message!");
                    msg.Properties["version"] = version;
                    topicClient.Send(msg);
                }
                else
                {
                    Console.WriteLine("Invalid version number. Try again.");
                }

            } while (!string.IsNullOrEmpty(versionNumber));
        }
    }
}
