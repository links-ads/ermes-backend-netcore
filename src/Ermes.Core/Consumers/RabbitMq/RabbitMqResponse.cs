using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ermes.Consumers.RabbitMq
{
    public class RabbitMqResponse
    {
        public string message { get; set; }
        [JsonIgnore]
        public string request_code { get; set; }
        public string status_code { get; set; }
        public int datatype_id { get; set; }
        public string type { get; set; }
    }
}
