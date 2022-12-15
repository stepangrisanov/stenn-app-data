using Microsoft.EntityFrameworkCore;

namespace Stenn.AppData.Mock
{
    public sealed class MockAppDataServiceBuilder<TBaseEntity>
    {
        private readonly ModelBuilder _modelBuilder;

        public MockAppDataServiceBuilder(ModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public T Add<T>(T entity)
            where T : class, TBaseEntity
        {
            _modelBuilder.Entity<T>().HasData(entity);
            return entity;
        }
    }
}