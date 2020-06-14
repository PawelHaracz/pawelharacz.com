using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;

namespace MultiTopicClient.ServiceBus
{
    internal sealed class TopicClientBuilder : ITopicClientBuilder
    {
        public ITopicClient Build(string serviceBusName, string topicName, TokenProvider tokenProvider)
            => new TopicClient(
                serviceBusName,
                topicName,
                tokenProvider,
                TransportType.AmqpWebSockets,
                RetryPolicy.Default);
        }
}