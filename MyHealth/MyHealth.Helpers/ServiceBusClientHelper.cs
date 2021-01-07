using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHealth.Helpers
{
    public class ServiceBusClientHelper
    {
        private readonly ServiceBusClient _serviceBusClient;

        public ServiceBusClientHelper(string connectionString)
        {
            _serviceBusClient = new ServiceBusClient(connectionString);
        }

        /// <summary>
        /// Sends a ServiceBusMessage to the provided topic name.
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual async Task SendMessageToTopic(string topicName, ServiceBusMessage message)
        {
            ServiceBusSender sender = _serviceBusClient.CreateSender(topicName);
            await sender.SendMessageAsync(message);
        }

        /// <summary>
        /// Sends multiple messages as a batch to a defined topic.
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public virtual async Task SendMessagesToTopic(string topicName, Queue<ServiceBusMessage> messages)
        {
            ServiceBusSender sender = _serviceBusClient.CreateSender(topicName);

            while (messages.Count > 0)
            {
                using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

                while (messages.Count > 0 && messageBatch.TryAddMessage(messages.Peek()))
                {
                    messages.Dequeue();
                }

                await sender.SendMessagesAsync(messageBatch);
            }
        }
    }
}
