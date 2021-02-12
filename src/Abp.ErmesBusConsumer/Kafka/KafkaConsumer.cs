using Abp.ErmesBusConsumer.Configuration;
using Confluent.Kafka;
using Ermes.Consumers;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.ErmesBusConsumer.Kafka
{
    public class KafkaConsumer: BackgroundService
    {
        private readonly string[] topicList;
        private readonly IErmesBusConfigurationProvider _busConfigurationProvider;
        private readonly IConsumer<Null, string> kafkaConsumer;
        private readonly ConsumerService _consumerService;

        public KafkaConsumer(
            IErmesBusConfigurationProvider busConfigurationProvider,
            ConsumerService consumerService)
        {
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
            kafkaConsumer.Subscribe(topicList);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = kafkaConsumer.Consume(cancellationToken);

                    _consumerService.ConsumeBusNotification(cr.Message.Value);
                    
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    Console.WriteLine($"Consume error: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e}");
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
