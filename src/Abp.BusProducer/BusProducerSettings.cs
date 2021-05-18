namespace Abp.BusProducer
{
    public class BusProducerSettings
    {
        public string ConnectionString { get; set; }
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string VirtualHost { get; set; }
        public string Exchange { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Queue { get; set; }
    }
}
