using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;

namespace MultiTopicClient.ServiceBus
{
    public interface ITopicClientBuilder
    {
        ITopicClient Build(string serviceBusName, string topicName, TokenProvider tokenProvider);
    }
}