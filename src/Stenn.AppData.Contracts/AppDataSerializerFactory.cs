namespace Stenn.AppData.Contracts
{
    public class AppDataSerializerFactory : IAppDataSerializerFactory
    {
        private readonly IEnumerable<IAppDataSerializer> _appDataSerializers;

        public AppDataSerializerFactory(IEnumerable<IAppDataSerializer> appDataSerializers)
        {
            _appDataSerializers = appDataSerializers;
        }

        public IAppDataSerializer GetSerializer(string? serializerName = null)
        {
            if (string.IsNullOrEmpty(serializerName))
            {
                serializerName = nameof(TextJsonAppDataSerializer);
            }

            return _appDataSerializers.First(x => x.GetType().Name == serializerName);
        }
    }
}
