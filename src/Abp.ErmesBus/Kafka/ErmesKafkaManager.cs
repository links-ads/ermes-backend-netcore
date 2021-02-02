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
    public class ErmesKafkaManager: IErmesBusManager
    {
        private readonly IErmesBusConfigurationProvider _busConfigurationProvider;
        IProducer<Null, string> _producer;

        public ErmesKafkaManager(IErmesBusConfigurationProvider busConfigurationProvider)
        {
            _busConfigurationProvider = busConfigurationProvider;
            _producer = new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = _busConfigurationProvider.GetConnectionString() }).Build();
        }

        public async Task<bool> Publish(string topic, string message)
        {
            using (CancellationTokenSource tokenSource = new CancellationTokenSource(2000))
            {
                return (await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message }, tokenSource.Token)).Status != PersistenceStatus.NotPersisted;
            }
        }        
    }
}
