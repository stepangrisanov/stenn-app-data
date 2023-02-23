using System.Reflection;

namespace Stenn.AppData.Mock
{
    internal sealed class MockDataStorage<TBaseEntity>
        where TBaseEntity : class
    {
        public MockDataStorage(IReadOnlyDictionary<TypeInfo, List<TBaseEntity>> dict)
        {
            Dictionary = dict ?? throw new ArgumentNullException(nameof(dict));
        }
        
        public IReadOnlyDictionary<TypeInfo, List<TBaseEntity>> Dictionary { get; }
    }
}