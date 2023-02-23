using System.Reflection;
using Stenn.AppData.Contracts;

namespace Stenn.AppData.Mock
{
    public sealed class MockAppDataServiceBuilder<TBaseEntity>
        where TBaseEntity : class, IAppDataEntity
    {
        public Dictionary<TypeInfo, List<TBaseEntity>> Dictionary { get; } = new();

        public T Add<T>(T entity)
            where T : class, TBaseEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var type = typeof(T).GetTypeInfo();
            if (!Dictionary.TryGetValue(type, out var list))
            {
                list = new List<TBaseEntity>();
                if (!Dictionary.TryAdd(type, list))
                {
                    list = Dictionary[type];
                }
            }
            list.Add(entity);
            return entity;
        }
    }
}