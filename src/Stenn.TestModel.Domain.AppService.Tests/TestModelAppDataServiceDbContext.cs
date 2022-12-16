using Microsoft.EntityFrameworkCore;
using Stenn.AppData;
using Stenn.TestModel.Domain.AppService.Tests.Internals;

namespace Stenn.TestModel.Domain.AppService.Tests
{
    internal sealed class TestModelAppDataServiceDbContext : AppDataServiceDbContext<ITestModelEntity>
    {
        public TestModelAppDataServiceDbContext(DbContextOptions<TestModelAppDataServiceDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SafeAssemblySearchAncestor).Assembly);
        }
    }
}