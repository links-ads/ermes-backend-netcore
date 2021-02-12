using Confluent.Kafka;
using Abp.BusProducer.Configuration;
using System;
using System.Threading.Tasks;

namespace Abp.BusProducer.RabbitMq
{
    public class RabbitMqManager : IBusProducer
    {
        //private readonly IBusConfigurationProvider _busConfigurationProvider;
        //IProducer<Null, string> _producer;

        public RabbitMqManager(
            //IBusConfigurationProvider busConfigurationProvider
            )
        {

        }

        public Task<bool> Publish(string topic, string message)
        {
            return Task.FromResult(false);
        }
    }
}