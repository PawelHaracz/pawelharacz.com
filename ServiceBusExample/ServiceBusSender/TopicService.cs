using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ServiceBusSender.Contracts;

namespace ServiceBusSender
{
    internal class TopicService : IMessageBroker
    {
        private readonly ILogger<TopicService> _logger;
        private readonly ITopicClient _client;

        public TopicService(IOptions<TopicOptions> options, ILogger<TopicService> logger)
        {
            _logger = logger;
            var token = TokenProvider.CreateSharedAccessSignatureTokenProvider(options.Value.SharedAccessName, options.Value.SharedAccessKey, TokenScope.Entity);
            _client =  new TopicClient(options.Value.ServiceBusNamespace, options.Value.TopicName, token); 
        }

        public async Task<string> Send(object data)
        {
            var correlationId = Guid.NewGuid().ToString("N");
            var messageBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            var message = new Message(messageBody)
            {
                ContentType = $"{System.Net.Mime.MediaTypeNames.Application.Json};charset=utf-8",
                CorrelationId = correlationId
            };
            await _client.SendAsync(message);
            return correlationId;
        }
    }
}