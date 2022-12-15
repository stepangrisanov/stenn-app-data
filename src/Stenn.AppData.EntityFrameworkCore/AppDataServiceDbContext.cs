using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stenn.AppData.Contracts;

namespace Stenn.AppData
{
    // ReSharper disable once UnusedTypeParameter
    public abstract class AppDataServiceDbContext<TBaseEntity> : AppDataServiceDbContext
        where TBaseEntity : class, IAppDataEntity
    {
        /// <inheritdoc />
        protected AppDataServiceDbContext()
        {
        }

        /// <inheritdoc />
        protected AppDataServiceDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }

    public abstract class AppDataServiceDbContext : DbContext
    {
        /// <inheritdoc />
        protected AppDataServiceDbContext()
        {
        }

        /// <inheritdoc />
        protected AppDataServiceDbContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <inheritdoc />
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw ThrowReadOnlyException();
        }

        /// <inheritdoc />
        public override int SaveChanges()
        {
            throw ThrowReadOnlyException();
        }

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw ThrowReadOnlyException();
        }

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            throw ThrowReadOnlyException();
        }

        private Exception ThrowReadOnlyException()
        {
            return new ApplicationException($"DbContext '{GetType().Name}' is read only");
        }
    }
}