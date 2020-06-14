using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MultiTopicClient.ServiceBus;

namespace Qbanks.CQRS.MessageBroker.AzureServiceBus
{
    internal sealed class ServiceBusOptions
    {
        [Required]
        public string ServiceBusNamespace { get; set; }
        
        [Required]
        public IDictionary<string, TopicOption> Topics { get; set; }
    }
}