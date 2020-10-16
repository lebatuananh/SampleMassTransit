using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SendEmailMassTransit.Common.Configs;

namespace SendEmailMassTransit.DataProcessor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBusControl _busControl;
        private readonly IServiceProvider _serviceProvider;
        private readonly QueueSettings _queueSettings;

        public Worker(ILogger<Worker> logger, IBusControl busControl, IServiceProvider serviceProvider,
            QueueSettings queueSettings)
        {
            _logger = logger;
            _busControl = busControl;
            _serviceProvider = serviceProvider;
            _queueSettings = queueSettings;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var mailChangeHandler = _busControl.ConnectReceiveEndpoint(_queueSettings.QueueName,
                    endpoint => { endpoint.Consumer(() => new MailChangedConsumer(_serviceProvider)); });

                await mailChangeHandler.Ready;
            }
            catch (Exception ex)
            {
                _logger.LogError("DataProcessor cannot be started.", ex);
            }
        }
    }
}