namespace ServiceBusExample.Contracts
{
    public interface IDeserializer<out T>
    {
        T Deserialize(byte[] body);
    }
}