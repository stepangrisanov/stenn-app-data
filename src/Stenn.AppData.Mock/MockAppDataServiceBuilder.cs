namespace Stenn.AppData.Mock
{
    public abstract class MockAppDataServiceBuilder<TBaseEntity>
    {
        public abstract T Add<T>(T entity)
            where T : class, TBaseEntity;
    }
}