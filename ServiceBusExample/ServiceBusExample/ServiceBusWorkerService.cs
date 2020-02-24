using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ServiceBusExample
{
    internal sealed class ServiceBusWorkerService : BackgroundService
    {
        private readonly ServiceBusWorker<object> _worker;
        private readonly ILogger<ServiceBusWorkerService> _logger;
        public ServiceBusWorkerService(ServiceBusWorker<object> worker, ILogger<ServiceBusWorkerService> logger)
        {
            _worker = worker;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
            _worker.ExecuteAsync(stoppingToken, (obj) => _logger.LogInformation((JsonConvert.SerializeObject(obj))));

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _worker.StartAsync(cancellationToken).ConfigureAwait(false);
            await base.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public override Task StopAsync(CancellationToken cancellationToken) => _worker.StopAsync(cancellationToken);
    }
}