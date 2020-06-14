using System;
using System.Collections.Generic;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MultiTopicClient.ServiceBus;
using NSubstitute;
using Qbanks.CQRS.MessageBroker.AzureServiceBus;
using Shouldly;
using Xunit;

namespace MultiTopicClient
{
    public class TopicFactoryTests
    {
        public const string EventName = "test";
        public const string NotRegisteredEventName = "test1";
        private ITopicClient Act<T>(T @event )where T: class => _factory.Get<T>();
        
        [Fact]
        public void given_event_with_contract_should_return_proper_topic_client()
        {
            _topicClientBuilder
                .Build(
                    _options.Value.ServiceBusNamespace, 
                    _options.Value.Topics[EventName].TopicName, 
                    Arg.Any<TokenProvider>())
                .Returns(_topicClient);
            
            var @event = new TestedEvent();

            var topic = Act(@event);
            
            topic.ShouldBeSameAs(_topicClient);
        }

        [Fact]
        public void given_event_without_contract_should_throw_exception()
        {
            var @event = new TestedEventWithoutContract();
            
            var exception =Record.Exception( () => Act(@event));
            
            exception.ShouldBeOfType<Exception>();
        }
        
        [Fact]
        public void given_event_with_contract_but_event_name_is_not_registered_as_options_should_throw_exception()
        {
            var @event = new TestedWithoutConfiguration();
            
            var exception =Record.Exception( () => Act(@event));
            
            exception.ShouldBeOfType<Exception>();
        }
        
        #region Arrange
        
        private readonly ITopicFactory _factory;
        private readonly ITopicClient _topicClient;
        private readonly ITopicClientBuilder _topicClientBuilder;
        private readonly IOptions<ServiceBusOptions> _options;
        public TopicFactoryTests()
        {
            var serviceBusOptions = new ServiceBusOptions
            {
                ServiceBusNamespace = $"{Guid.NewGuid():N}",
                Topics = new Dictionary<string, TopicOption>()
                {
                    [EventName] = new TopicOption
                    {
                        TopicName = $"{Guid.NewGuid():N}",
                        SharedAccessKey = $"{Guid.NewGuid():N}",
                        SharedAccessName = $"{Guid.NewGuid():N}"
                    }
                }
            };

            _topicClient = Substitute.For<ITopicClient>();
            _topicClientBuilder = Substitute.For<ITopicClientBuilder>();
            _options = Options.Create(serviceBusOptions);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddServiceBus();
            serviceCollection.Replace(ServiceDescriptor.Singleton(_topicClientBuilder));
            serviceCollection.Replace(ServiceDescriptor.Singleton(_options));
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            _factory = serviceProvider.GetService<ITopicFactory>();
        }
        
        #endregion
    }

    [Contract(TopicFactoryTests.EventName)]
    internal class TestedEvent
    {
        
    }

    internal class TestedEventWithoutContract
    {
        
    }
    
    [Contract(TopicFactoryTests.NotRegisteredEventName)]
    internal class TestedWithoutConfiguration
    {
        
    }
}