using Abp.Dependency;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Notifiers
{
    public interface IWebApiNotifier: ITransientDependency
    {
        Task<List<string>> SendMessage(FullNotificationData input);
    }
}
