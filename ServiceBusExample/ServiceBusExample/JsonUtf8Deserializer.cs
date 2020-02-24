using System.Text;
using Newtonsoft.Json;
using ServiceBusExample.Contracts;

namespace ServiceBusExample
{
    internal sealed class JsonUtf8Deserializer<T> : IDeserializer<T>
    {
        public T Deserialize(byte[] body) => JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(body));
    }
}