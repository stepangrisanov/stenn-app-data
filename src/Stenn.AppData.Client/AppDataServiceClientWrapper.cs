#nullable disable
using Stenn.AppData.Contracts;

namespace Stenn.AppData.Client
{
    internal sealed class AppDataServiceClientWrapper<TBaseEntity> : IAppDataService<TBaseEntity> where TBaseEntity : IAppDataEntity
    {
        private readonly IAppDataServiceClient<TBaseEntity> _client;

        public AppDataServiceClientWrapper(IAppDataServiceClient<TBaseEntity> client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public IQueryable<T> Query<T>()
            where T : class, TBaseEntity
        {
            return new Query<T>(_client);
        }
    }
}