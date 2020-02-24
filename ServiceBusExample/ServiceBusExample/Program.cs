using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceBusExample.Contracts;

namespace ServiceBusExample
{
    internal static class Program
    {
        static Task Main(string[] args)
        {
            using var cts = new CancellationTokenSource();
            return Host.CreateDefaultBuilder()
                .ConfigureServices(collection =>
                    {
                        collection.AddOptions<ServiceBusOptions>().Configure<IConfiguration>((options, configuration) =>
                        {
                            var sectionName = "ServiceBus";
                            var section = configuration.GetSection(sectionName);
                            if (section.Exists() is false)
                            {
                                throw new ArgumentException($"Section {sectionName} does not exist"); 
                            }
                            
                            section.Bind(options);
                        });
                        collection.AddSingleton<IDeserializerFactory<object>, DeserializerFactory<object>>();
                        collection.AddSingleton<ServiceBusManager>();
                        collection.AddSingleton<ServiceBusWorker<object>>();
                        collection.AddScoped<IDeserializer<object>, JsonUtf8Deserializer<object>>();
                        collection.AddHostedService<ServiceBusWorkerService>();
                        
                    })
                .ConfigureAppConfiguration((context, config) =>
                {
                    var azureServiceTokenProvider = new AzureServiceTokenProvider();
                    var keyVaultClient = new KeyVaultClient(
                        new KeyVaultClient.AuthenticationCallback(
                            azureServiceTokenProvider.KeyVaultTokenCallback));
                    
                    config.AddAzureKeyVault(
                        $"https://{context.Configuration.GetValue<string>("keyVault:name")}.vault.azure.net/",
                        keyVaultClient,
                        new DefaultKeyVaultSecretManager());
                }).Build().RunAsync(cts.Token);
        }
    }
}