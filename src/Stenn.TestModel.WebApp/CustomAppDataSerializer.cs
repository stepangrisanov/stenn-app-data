using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Stenn.AppData.Contracts;

namespace Stenn.TestModel.WebApp
{
    public class CustomAppDataSerializer : IAppDataSerializer
    {
        public T? Deserialize<T>(byte[] bytes)
        {
            MemoryStream ms = new MemoryStream(bytes);
            using (BsonDataReader reader = new BsonDataReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }

        public byte[] Serialize<T>(T obj)
        {
            MemoryStream ms = new MemoryStream();
            using (BsonDataWriter writer = new BsonDataWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, obj);
            }

            return ms.ToArray();
        }
    }
}
