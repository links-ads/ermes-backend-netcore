namespace Abp.BusConsumer
{
    public class BusConsumerSettings
    {
        public string ConnectionString { get; set; }
        public string GroupId { get; set; }
        public string Topics { get; set; }
        public string[] TopicList
        {
            get
            {
                return Topics.Split(',');
            }
        }
        public bool IsEnabled { get; set; }
    }
}
