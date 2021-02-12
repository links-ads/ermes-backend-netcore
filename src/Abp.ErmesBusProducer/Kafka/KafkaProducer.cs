using Confluent.Kafka;
using Abp.ErmesBusProducer;
using Abp.ErmesBusProducer.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Abp.ErmesBusProducer.Kafka
{
    public class KafkaProducer: IErmesBusProducer
    {
        private readonly IErmesBusConfigurationProvider _busConfigurationProvider;
        IProducer<Null, string> _producer;

        public KafkaProducer(IErmesBusConfigurationProvider busConfigurationProvider)
        {
            _busConfigurationProvider = busConfigurationProvider;
            _producer = new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = _busConfigurationProvider.GetConnectionString() }).Build();
        }

        public async Task<bool> Publish(string topic, string message)
        {
            using (CancellationTokenSource tokenSource = new CancellationTokenSource(4000))
            {
                return (await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message }, tokenSource.Token)).Status != PersistenceStatus.NotPersisted;
            }
        }   
    }
}
