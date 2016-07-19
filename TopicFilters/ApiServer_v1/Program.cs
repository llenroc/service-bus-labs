using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;

namespace ApiServer_v1
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceBusNamesapce = ConfigurationManager.AppSettings["serviceBusNamespace"];
            var serviceBusTopicName = ConfigurationManager.AppSettings["serviceBusTopicName"];
            var serviceBusKeyName   = ConfigurationManager.AppSettings["serviceBusKeyName"];
            var serviceBusAccessKey = ConfigurationManager.AppSettings["serviceBusAccessKey"];

            Console.WriteLine("API Server Version 1");

            InitializeEnvironment(serviceBusNamesapce, serviceBusKeyName, serviceBusAccessKey, serviceBusTopicName);

            var subscriptionClient = SubscriptionClient.CreateFromConnectionString(
                $"Endpoint=sb://{serviceBusNamesapce}.servicebus.windows.net/;SharedAccessKeyName={serviceBusKeyName};SharedAccessKey={serviceBusAccessKey}",
                serviceBusTopicName, 
                "version1");

            subscriptionClient.OnMessage((msg) =>
            {
                Console.WriteLine($"Received message with version number {msg.Properties["version"]}.");
            });

            Console.ReadLine();
        }

        static void InitializeEnvironment(string sbNamespace, string keyName, string accessKey, string topicName)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(
                $"Endpoint=sb://{sbNamespace}.servicebus.windows.net/;SharedAccessKeyName={keyName};SharedAccessKey={accessKey}");

            if (!namespaceManager.SubscriptionExists(topicName, "version1"))
            {
                var subscriptionDescription = new SubscriptionDescription(topicName, "version1");
                var version1Filter = new SqlFilter("version < 2");
                var rd = new RuleDescription();
                namespaceManager.CreateSubscription(subscriptionDescription, version1Filter);
            }
        }
    }
}
