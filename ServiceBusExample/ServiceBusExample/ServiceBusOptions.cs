namespace ServiceBusExample
{
    internal class ServiceBusOptions
    {
        public string ConnectionString { get; set; }
        public string TopicName { get; set; }
        public string Subscription { get; set; }
    }
}