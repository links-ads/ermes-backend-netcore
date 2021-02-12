using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.BusConsumer.RabbitMq
{
    public class RabbitMqManager: BackgroundService
    {
        //private readonly IBusConfigurationProvider _busConfigurationProvider;
        //IProducer<Null, string> _producer;

        public RabbitMqManager(
                //IBusConfigurationProvider busConfigurationProvider
            )
        {
            
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
