﻿using Abp.BusConsumer.Configuration;
using Confluent.Kafka;
using Ermes.Consumers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.BusConsumer.Kafka
{
    public class KafkaConsumer: BackgroundService
    {
        private readonly string[] topicList;
        private readonly IBusConfigurationProvider _busConfigurationProvider;
        private readonly IConsumer<Null, string> kafkaConsumer;
        private readonly IConsumerService _consumerService;
        private readonly ILogger Logger;

        public KafkaConsumer(
            IBusConfigurationProvider busConfigurationProvider,
            IConsumerService consumerService,
            ILogger<KafkaConsumer> logger)
        {
            Logger = logger;
            _consumerService = consumerService;
            _busConfigurationProvider = busConfigurationProvider;
            var consumerConfig = new ConsumerConfig { BootstrapServers = _busConfigurationProvider.GetConnectionString(), GroupId = _busConfigurationProvider.GetGroupId(), AutoOffsetReset = AutoOffsetReset.Earliest };
            topicList = _busConfigurationProvider.GetTopicList();
            kafkaConsumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Thread(() => StartConsumerLoop(stoppingToken)) { 
                Name = "KafkaConsumerThread"
            }.Start();

            return Task.CompletedTask;
        }

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            if (!_busConfigurationProvider.IsEnabled())
            {
                Logger.LogInformation("------------------------- Bus Consumer Disabled-----------------------------");
                return;
            }

            Logger.LogInformation("------------------------- Start Bus Consumer -----------------------------");
            kafkaConsumer.Subscribe(topicList);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = kafkaConsumer.Consume(cancellationToken);
                    Logger.LogInformation("------------------------- Bus Consumer Message Received-----------------------------");
                    Logger.LogInformation("Message: {0}", cr.Message.Value);
                    _consumerService.ConsumeBusNotification(cr.Message.Value);
                }
                catch (OperationCanceledException e)
                {
                    Logger.LogError($"-----------------Bus Consumer OperationCanceledException: {e.Message}");
                    break;
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    Logger.LogError($"-----------------Bus Consumer ConsumeException error: {e.Error.Reason}");
                    break;
                }
                catch (Exception e)
                {
                    Logger.LogError($"-----------------Bus Consumer general Exception: {e.Message}");
                    break;
                }
            }
        }

        public override void Dispose()
        {
            kafkaConsumer.Close(); // Commit offsets and leave the group cleanly.
            kafkaConsumer.Dispose();

            base.Dispose();
        }
    }
}
