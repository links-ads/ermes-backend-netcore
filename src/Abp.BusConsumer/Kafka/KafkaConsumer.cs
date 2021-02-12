using Abp.BusConsumer.Configuration;
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
        private readonly ConsumerService _consumerService;
        private readonly ILogger Logger;

        public KafkaConsumer(
            IBusConfigurationProvider busConfigurationProvider,
            ConsumerService consumerService,
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
            new Thread(() => StartConsumerLoop(stoppingToken)).Start();

            return Task.CompletedTask;
        }

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            Logger.LogInformation("------------------------- Start Bus Consumer -----------------------------");
            kafkaConsumer.Subscribe(topicList);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = kafkaConsumer.Consume(cancellationToken);
                    _consumerService.ConsumeBusNotification(cr.Message.Value);
                }
                catch (OperationCanceledException e)
                {
                    Logger.LogError($"Bus Consumer OperationCanceledException: {e.Message}");
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    Logger.LogError($"Bus Consumer ConsumeException error: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        Logger.LogError($"Bus Consumer ConsumeException error is Fatal, BusConsumer shut down");
                        // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                        break;
                    }
                }
                catch (Exception)
                {
                    Logger.LogError($"Bus Consumer general Exception, BusConsumer shut down");
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
