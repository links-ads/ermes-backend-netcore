using Confluent.Kafka;
using Abp.BusProducer.Configuration;
using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Abp.BusProducer.RabbitMq
{
    public class RabbitMqProducer : IBusProducer
    {
        private readonly IBusConfigurationProvider _busConfigurationProvider;
        private readonly ConnectionFactory _factory;

        public RabbitMqProducer(
            IBusConfigurationProvider busConfigurationProvider
            )
        {
            _busConfigurationProvider = busConfigurationProvider;
            _factory = new ConnectionFactory()
            {
                HostName = _busConfigurationProvider.GetHostname(),
                Port = _busConfigurationProvider.GetPort(),
                VirtualHost = _busConfigurationProvider.GetVirtualHost(),
                UserName = _busConfigurationProvider.GetUsername(),
                Password = _busConfigurationProvider.GetPassword(),
                Ssl = new SslOption(
                    _busConfigurationProvider.GetHostname(),
                    enabled: true
                )
            };
        }

        public async Task<bool> Publish(string routingKey, string message)
        {
            if (string.IsNullOrWhiteSpace(routingKey))
                return false;
            using (var connection = _factory.CreateConnection())
            {
                using var channel = connection.CreateModel();
                string exchange = _busConfigurationProvider.GetExchange();
                var body = Encoding.UTF8.GetBytes(message);
                var props = channel.CreateBasicProperties();
                try
                {
                    props.MessageId = routingKey.Split('.')[^1];
                }
                catch
                {
                    return true;
                }
                props.UserId = _busConfigurationProvider.GetUsername();

                // Dashboard service
                props.AppId = "dsh";

                channel.BasicPublish(exchange: exchange,
                                     routingKey: routingKey,
                                     basicProperties: props,
                                     body: body);
            }

            return true;
        }
    }
}