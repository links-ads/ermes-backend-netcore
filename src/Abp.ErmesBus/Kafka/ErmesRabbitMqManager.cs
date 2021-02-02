using Confluent.Kafka;
using Abp.Bus;
using Abp.Bus.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.Bus.Kafka
{
    public class ErmesRabbitMqManager: IErmesBusManager
    {
        private readonly IErmesBusConfigurationProvider _busConfigurationProvider;
        IProducer<Null, string> _producer;

        public ErmesRabbitMqManager(IErmesBusConfigurationProvider busConfigurationProvider)
        {
            
        }

        public Task<bool> Publish(string topic, string message)
        {
            throw new NotImplementedException();
        }
    }
}
