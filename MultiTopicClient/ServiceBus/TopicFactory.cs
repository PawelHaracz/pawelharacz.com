using System;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Qbanks.CQRS.MessageBroker.AzureServiceBus;

namespace MultiTopicClient.ServiceBus
{
    internal sealed class TopicFactory<T> : ITopicFactory<T> where T : class
    {
        private readonly ITopicClient _client;
        public TopicFactory(IOptions<ServiceBusOptions> options, ITopicClientBuilder topicClientBuilder)
        {
            var option = options.Value;
            var contract = typeof(T).GetCustomAttribute<ContractAttribute>();
            if (contract is null)
            {
                throw new Exception(nameof(T));
            }

            var topicOption = option.Topics.SingleOrDefault(kv =>
                kv.Key.Equals(contract.Name, StringComparison.OrdinalIgnoreCase)).Value;
            
            if (topicOption is null)
            {
                throw new Exception(contract.Name);
            }
            
            var token = TokenProvider.CreateSharedAccessSignatureTokenProvider(
                topicOption.SharedAccessName,
                topicOption.SharedAccessKey, 
                TokenScope.Entity);

            _client = topicClientBuilder.Build(option.ServiceBusNamespace, topicOption.TopicName, token);
        }

        public ITopicClient Get() => _client;
    }

    internal sealed class TopicFactory : ITopicFactory
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IOptions<ServiceBusOptions> _options;

        public TopicFactory(IServiceScopeFactory serviceScopeFactory, IOptions<ServiceBusOptions> options)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _options = options;
        }
        public ITopicClient Get<T>() where T : class
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITopicFactory<T>>();
            if (service is null)
            {
                throw new Exception(nameof(T));
            }

            return service.Get();
        }
    }
}