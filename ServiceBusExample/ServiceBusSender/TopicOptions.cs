namespace ServiceBusSender
{
    internal class TopicOptions
    {
        public string TopicName { get; set; }
        public string ServiceBusNamespace { get; set; }
        public string SharedAccessName { get; set; }
        public string SharedAccessKey { get; set; }
    }
}