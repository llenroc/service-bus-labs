using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;

namespace ReplayDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceBusNamesapce = ConfigurationManager.AppSettings["serviceBusNamespace"];
            var serviceBusTopicName = ConfigurationManager.AppSettings["serviceBusTopicName"];
            var serviceBusKeyName   = ConfigurationManager.AppSettings["serviceBusKeyName"];
            var serviceBusAccessKey = ConfigurationManager.AppSettings["serviceBusAccessKey"];

            Console.WriteLine("Replay Demo Client");

            InitializeEnvironment(serviceBusNamesapce, serviceBusKeyName, serviceBusAccessKey, serviceBusTopicName);
            
            var subscriptionClient = SubscriptionClient.CreateFromConnectionString(
                $"Endpoint=sb://{serviceBusNamesapce}.servicebus.windows.net/;SharedAccessKeyName={serviceBusKeyName};SharedAccessKey={serviceBusAccessKey}",
                serviceBusTopicName, 
                "replay-demo");

            var lastMessageTime = DateTimeOffset.Now;
            subscriptionClient.OnMessage((msg) =>
            {
                Console.WriteLine($"Received message Id: {msg.MessageId}");
                Console.WriteLine("System died, message not completed.");
            }, new OnMessageOptions()
            {
                AutoComplete = false
            });

            Console.ReadLine();
        }

        static void InitializeEnvironment(string sbNamespace, string keyName, string accessKey, string topicName)
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(
                $"Endpoint=sb://{sbNamespace}.servicebus.windows.net/;SharedAccessKeyName={keyName};SharedAccessKey={accessKey}");
            
            // Delete and recreate the subscription to make sure it is empty
            if (namespaceManager.SubscriptionExists(topicName, "replay-demo"))
            {
                namespaceManager.DeleteSubscription(topicName, "replay-demo");
            }

            namespaceManager.CreateSubscription(topicName, "replay-demo");

            var topicClient = TopicClient.CreateFromConnectionString(
                $"Endpoint=sb://{sbNamespace}.servicebus.windows.net/;SharedAccessKeyName={keyName};SharedAccessKey={accessKey};EntityPath={topicName}");
            topicClient.Send(new BrokeredMessage());
        }
    }
}
