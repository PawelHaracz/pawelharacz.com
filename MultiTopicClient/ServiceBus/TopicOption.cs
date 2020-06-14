using System.ComponentModel.DataAnnotations;

namespace MultiTopicClient.ServiceBus
{
    internal sealed class TopicOption
    {
        [Required]
        public string TopicName { get; set; }
        
        [Required]
        public string SharedAccessName { get; set; }
        
        [Required]
        public string SharedAccessKey { get; set; }
    }
}