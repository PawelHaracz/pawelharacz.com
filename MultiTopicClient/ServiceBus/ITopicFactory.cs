using Microsoft.Azure.ServiceBus;

namespace MultiTopicClient.ServiceBus
{
    public interface ITopicFactory
    {
        ITopicClient Get<T>() where T : class;
    }

    public interface ITopicFactory<T> where T : class
    {
        ITopicClient Get();
    }
}