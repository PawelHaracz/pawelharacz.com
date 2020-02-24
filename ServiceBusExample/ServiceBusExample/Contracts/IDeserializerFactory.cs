namespace ServiceBusExample.Contracts
{
    public interface IDeserializerFactory<out T>
    {
        T Deserialize(string contentType, byte[] body);
    }
}