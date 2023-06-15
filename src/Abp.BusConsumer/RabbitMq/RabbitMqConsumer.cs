using Abp.BusConsumer.Configuration;
using Ermes.Consumers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.BusConsumer.RabbitMq
{
    public class RabbitMqConsumer: BackgroundService
    {
        private readonly IBusConfigurationProvider _busConfigurationProvider;
        private readonly IModel _channel;
        private readonly ILogger Logger;
        private readonly IConnection _connection;
        private EventingBasicConsumer _consumer;
        private readonly IConsumerService _consumerService;
        public RabbitMqConsumer(
                IBusConfigurationProvider busConfigurationProvider,
                ILogger<RabbitMqConsumer> logger,
                IConsumerService consumerService
            )
        {
            _busConfigurationProvider = busConfigurationProvider;
            Logger = logger;
            _consumerService = consumerService;
            var connectionFactory = new ConnectionFactory
            {
                VirtualHost = _busConfigurationProvider.GetVirtualHost(),
                Port = _busConfigurationProvider.GetPort(),
                UserName = _busConfigurationProvider.GetUsername(),
                Password = _busConfigurationProvider.GetPassword(),
                HostName = _busConfigurationProvider.GetHostname(),
                Ssl = new SslOption(
                    _busConfigurationProvider.GetHostname(),
                    enabled: true
                ),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(30)
            };
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            //Queues and exchanges can be declared "passively". A passive declare simply checks that the entity
            //with the provided name exists.
            _channel.ExchangeDeclarePassive(_busConfigurationProvider.GetExchange());
            var q = _channel.QueueDeclarePassive(_busConfigurationProvider.GetQueue());

            //Create the binding if not present
            _channel.QueueBind(q.QueueName, _busConfigurationProvider.GetExchange(), "status.brn.*.links.*");
            _channel.QueueBind(q.QueueName, _busConfigurationProvider.GetExchange(), "status.propagator.*.links.#");
            _channel.QueueBind(q.QueueName, _busConfigurationProvider.GetExchange(), "status.pwm.*.links.#");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Thread(() => Consume())
            {
                Name = "RabbitMqConsumerThread"
            }.Start();

            return Task.CompletedTask;
        }

        public void Consume()
        {
            if (_busConfigurationProvider == null)
                throw new ApplicationException("RabbitMQ configuration is missing.");


            Logger.LogDebug("---------------- RabbitMQ Consumer starting.....");

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += (model, result) =>
            {
                //Check on routing key should not be necessary
                //The queue should be binded to a single routing key
                if (result.RoutingKey.Contains("status."))
                {
                    Logger.LogDebug("---------------- RabbitMQ Consumer: new message received");
                    var body = result.Body.ToArray();
                    var bodyString = Encoding.UTF8.GetString(body);
                    Logger.LogDebug("RoutingKey: " + result.RoutingKey);
                    _consumerService.ConsumeBusNotification(bodyString, result.RoutingKey);
                    
                }

                _channel.BasicAck(result.DeliveryTag, false);
            };

            _channel.BasicConsume(_busConfigurationProvider.GetQueue(), false, _consumer);
        }

        public override void Dispose()
        {
            if (_channel?.IsOpen == true && _consumer != null)
            {
                _channel.BasicCancel(_consumer.ConsumerTags[0]);
            }
            Logger.LogDebug("---------------- RabbitMQ Consumer disposing.....");
            _channel?.Dispose();
            _connection?.Dispose();
        }

    }
}
