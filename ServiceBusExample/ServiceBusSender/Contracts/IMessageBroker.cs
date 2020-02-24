using System.Threading.Tasks;

namespace ServiceBusSender.Contracts
{
    internal interface IMessageBroker
    {
        Task<string> Send(object data);
    }
}