using Abp.ErmesBusConsumer;
using Abp.ErmesBusConsumer.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.ErmesBusConsumer.RabbitMq
{
    public class ErmesRabbitMqManager: BackgroundService
    {
        //private readonly IErmesBusConfigurationProvider _busConfigurationProvider;
        //IProducer<Null, string> _producer;

        public ErmesRabbitMqManager(
                //IErmesBusConfigurationProvider busConfigurationProvider
            )
        {
            
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
