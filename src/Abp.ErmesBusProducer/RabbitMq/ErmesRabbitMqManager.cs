using Confluent.Kafka;
using Abp.ErmesBusProducer.Configuration;
using System;
using System.Threading.Tasks;

namespace Abp.ErmesBusProducer.RabbitMq
{
    public class ErmesRabbitMqManager : IErmesBusProducer
    {
        //private readonly IErmesBusConfigurationProvider _busConfigurationProvider;
        //IProducer<Null, string> _producer;

        public ErmesRabbitMqManager(
            //IErmesBusConfigurationProvider busConfigurationProvider
            )
        {

        }

        public Task<bool> Publish(string topic, string message)
        {
            return Task.FromResult(false);
        }
    }
}