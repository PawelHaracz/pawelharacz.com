using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ServiceBusExample
{
    internal sealed  class ServiceBusManager
    {
        private readonly ILogger<ServiceBusManager> _logger;
        private readonly IMessageReceiver _messageReceiver;

        public ServiceBusManager(IOptions<ServiceBusOptions> options, ILogger<ServiceBusManager> logger)
        {
            _logger = logger;
            var serviceBusOptions = options.Value;
            
            if (string.IsNullOrWhiteSpace(options.Value.ConnectionString))
            {
                throw new ArgumentNullException(nameof(options.Value.ConnectionString));
            }
            
            if (string.IsNullOrWhiteSpace(options.Value.TopicName))
            {
                throw new ArgumentNullException(nameof(options.Value.TopicName));
            }
            
            if (string.IsNullOrWhiteSpace(options.Value.Subscription))
            {
                throw new ArgumentNullException(nameof(options.Value.Subscription));
            }
            
            var connectionString = new ServiceBusConnectionStringBuilder(serviceBusOptions.ConnectionString);
            _messageReceiver = new MessageReceiver(connectionString.GetNamespaceConnectionString(),
                EntityNameHelper.FormatSubscriptionPath(serviceBusOptions.TopicName, serviceBusOptions.Subscription),
                ReceiveMode.ReceiveAndDelete, RetryPolicy.Default);
        }

        public Task<Message> ReceiveMessageAsync() => _messageReceiver.ReceiveAsync();
        public Task<Message> ReceiveMessageAsync(TimeSpan operationTimeout) =>  _messageReceiver.ReceiveAsync(operationTimeout);
        public Task<IList<Message>> ReceiveMessageAsync(int maxMessageCount) => _messageReceiver.ReceiveAsync(maxMessageCount);
        public Task<IList<Message>> ReceiveMessageAsync(int maxMessageCount, TimeSpan operationTimeout) => _messageReceiver.ReceiveAsync(maxMessageCount, operationTimeout);
        
    }
}