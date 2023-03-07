namespace Stenn.AppData.Contracts
{
    public interface IAppDataSerializer
    {
        public byte[] Serialize<T>(T obj);

        public T? Deserialize<T>(byte[] bytes);
    }
}
