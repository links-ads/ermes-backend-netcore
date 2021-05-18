using Confluent.Kafka;
using Abp.BusProducer.Configuration;
using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text;

namespace Abp.BusProducer.RabbitMq
{
    public class RabbitMqManager : IBusProducer
    {
        private readonly IBusConfigurationProvider _busConfigurationProvider;
        private ConnectionFactory factory;

        public RabbitMqManager(
            IBusConfigurationProvider busConfigurationProvider
            )
        {
            _busConfigurationProvider = busConfigurationProvider;
            factory = new ConnectionFactory()
            {
                HostName = _busConfigurationProvider.GetHostname(),
                Port = _busConfigurationProvider.GetPort(),
                VirtualHost = _busConfigurationProvider.GetVirtualHost(),
                UserName = _busConfigurationProvider.GetUsername(),
                Password = _busConfigurationProvider.GetPassword(),
                Ssl = new SslOption(_busConfigurationProvider.GetHostname(), enabled: true)
            };
        }

        public async Task<bool> Publish(string routingKey, string message)
        {
            if(string.IsNullOrWhiteSpace(routingKey))
                routingKey = "Chatbot";
            using (var connection = factory.CreateConnection())
            {
                using var channel = connection.CreateModel();
                string exchange = _busConfigurationProvider.GetExchange();
                channel.ExchangeDeclare(exchange, "topic", true, false);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: exchange,
                                     routingKey: routingKey,
                                     body: body);
            }

            return true;
        }
    }
}