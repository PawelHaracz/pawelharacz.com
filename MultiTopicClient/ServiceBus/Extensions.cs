using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Qbanks.CQRS.MessageBroker.AzureServiceBus;

namespace MultiTopicClient.ServiceBus
{
    public static class Extensions
    {
        private const string SectionName = "ServiceBus";
        public static IServiceCollection AddServiceBus(this IServiceCollection services)
        {
            services.AddOptions<ServiceBusOptions>()
                .Configure<IConfiguration>(
                    (options, configuration) 
                        =>
                    {
                        var section = configuration.GetSection(SectionName);
                        if (section.Exists() is false)
                        {
                            throw new ConfigurationErrorsException($"Missing section: {SectionName}");
                        }

                        options.ServiceBusNamespace = section["ServiceBusNamespace"];
                        var topicSection = section.GetSection("Topics");
                        if (topicSection.Exists() is false)
                        {
                            options.Topics = new Dictionary<string, TopicOption>();
                        }
                        else
                        {
                            try
                            {
                                var topics = new Dictionary<string, TopicOption>();
                                foreach (var kv in  topicSection.AsEnumerable())
                                {
                                    var option = new TopicOption();
                                    var oneTopicSection = topicSection.GetSection(kv.Key);
                                    oneTopicSection.Bind(option);
                                    topics.Add(kv.Key, option);
                                }

                                options.Topics = topics;
                            }
                            catch (OptionsValidationException ex)
                            {
                                throw new Exception(string.Join(", ", ex.Failures));
                            }
                        }

                    })
                .ValidateDataAnnotations();
            
            services.AddSingleton(typeof(ITopicFactory), typeof(TopicFactory));
            services.AddSingleton(typeof(ITopicFactory<>), typeof(TopicFactory<>));
            services.AddSingleton<ITopicClientBuilder, TopicClientBuilder>();
            
            
            return services;
        }
    }
}