using Ermes.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Notifiers
{
    public class BaseNotificationData
    {
        public string Body { get; set; }
        public string Title { get; set; }
        public List<string> Receivers { get; set; }
    }
}
