using Confluent.Kafka;
using Abp.BusProducer.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.BusProducer.Kafka
{
    public class KafkaProducer: IBusProducer
    {
        private readonly IBusConfigurationProvider _busConfigurationProvider;
        readonly IProducer<Null, string> _producer;

        public KafkaProducer(
                IBusConfigurationProvider busConfigurationProvider
            )
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
