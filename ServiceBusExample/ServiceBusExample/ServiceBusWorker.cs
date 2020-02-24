using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceBusExample.Contracts;

namespace ServiceBusExample
{
    internal sealed class ServiceBusWorker<T>
    {
        private readonly ServiceBusManager _serviceBusManager;
        private readonly IDeserializerFactory<T> _deserializerFactory;
        private readonly ILogger<ServiceBusWorker<T>> _logger;
        private readonly IList<Exception> _exceptions = new List<Exception>();
        
        public ServiceBusWorker(ServiceBusManager serviceBusManager, IDeserializerFactory<T> deserializerFactory, ILogger<ServiceBusWorker<T>> logger)
        {
            _serviceBusManager = serviceBusManager;
            _deserializerFactory = deserializerFactory;
            _logger = logger;
        }
        public async Task ExecuteAsync(CancellationToken stoppingToken, Action<T> callback = null)
        {
            if (callback == null)
            {
                callback = (obj) => _logger.LogInformation(JsonConvert.SerializeObject(obj));
            } 
            
            stoppingToken.Register(() =>
                _logger.LogInformation($"{nameof(ServiceBusWorker<T>)} background task is stopping."));
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var message = await _serviceBusManager.ReceiveMessageAsync();
                    var body = message.Body;
                    
                    callback(_deserializerFactory.Deserialize(message.ContentType, body));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "A problem happened while invoking a callback method");
                    _exceptions.Add(ex);
                }
            }
            _logger.LogInformation(stoppingToken.IsCancellationRequested.ToString());
            _logger.LogInformation($"{nameof(ServiceBusWorker<T>)} background task is stopping.");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        
        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_exceptions.Any())
            {
                _logger.LogCritical(new AggregateException(_exceptions), "The host threw exceptions unexpectedly");
            }
            return  Task.CompletedTask;
        }
    }
}