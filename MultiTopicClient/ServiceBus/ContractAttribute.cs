using System;

namespace MultiTopicClient.ServiceBus
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)] 
    public class ContractAttribute : Attribute
    {
        public string Name { get; private set; }
        public ContractAttribute(string name)
        {
            Name = name;
        }   
    }
}