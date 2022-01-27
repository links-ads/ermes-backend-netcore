namespace Abp.BusConsumer
{
    public class BusConsumerSettings
    {
        #region KAFKA
        public string ConnectionString { get; set; }
        public string GroupId { get; set; }
        public string Topics { get; set; }
        public string[] TopicList
        {
            get
            {
                return Topics != null && Topics.Length > 0 ? Topics.Split(',') : null;
            }
        }
        public bool IsEnabled { get; set; }
        #endregion

        #region RabbitMq
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string Exchange { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Queue { get; set; }
        #endregion
    }
}
