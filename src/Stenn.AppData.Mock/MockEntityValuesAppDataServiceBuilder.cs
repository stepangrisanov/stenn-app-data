using Microsoft.EntityFrameworkCore;

namespace Stenn.AppData.Mock
{
    internal sealed class MockEntityValuesAppDataServiceBuilder<TBaseEntity> : MockAppDataServiceBuilder<TBaseEntity>
        where TBaseEntity : class
    {
        private readonly ModelBuilder _modelBuilder;

        internal MockEntityValuesAppDataServiceBuilder(ModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public override T Add<T>(T entity)
        {
            if (_modelBuilder.Model.FindEntityType(typeof(T)) != null)
            {
                _modelBuilder.Entity<T>().HasData(entity);
            }

            return entity;
        }
    }
}