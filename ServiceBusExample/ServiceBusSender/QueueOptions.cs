namespace ServiceBusSender
{
    internal class QueueOptions
    {
        public string QueueName { get; set; }
        public string ServiceBusNamespace { get; set; }
        public string SharedAccessName { get; set; }
        public string SharedAccessKey { get; set; }
    }
}