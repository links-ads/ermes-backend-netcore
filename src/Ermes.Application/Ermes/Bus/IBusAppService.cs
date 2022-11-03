using Ermes.Bus.Dto;
using Ermes.Dto;
using Ermes.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Bus
{
    public interface IBusAppService : IBackofficeApi
    {
        Task TestBusConsumerTopic(TestBusConsumerTopicInput input);
        Task TestGamificationNotification(TestBusConsumerTopicInput input);
        Task TestCsiPresidiService(IdInput<int> input);
        Task<bool> TestVolterTaxCodeService();
    }
}
