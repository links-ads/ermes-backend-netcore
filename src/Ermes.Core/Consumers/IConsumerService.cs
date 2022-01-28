using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Consumers
{
    public interface IConsumerService
    {
        void ConsumeBusNotification(string message, string routingKey = "");
    }
}
