using System.Text.Json;

namespace Stenn.AppData.Contracts
{
    public class TextJsonAppDataSerializer : IAppDataSerializer
    {
        public T? Deserialize<T>(byte[] bytes)
        {
            return JsonSerializer.Deserialize<T>(bytes);
        }

        public byte[] Serialize<T>(T obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }
    }
}
