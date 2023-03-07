namespace Stenn.AppData.Contracts
{
    public interface IAppDataSerializerFactory
    {
        public IAppDataSerializer GetSerializer(string? serializerName = null);
    }
}
