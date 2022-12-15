using Microsoft.EntityFrameworkCore;
using Stenn.TestModel.Domain.Tests.Internals;

namespace Stenn.TestModel.Domain.Tests
{
    public class TestModelDbContext : DbContext
    {
        public TestModelDbContext(DbContextOptions<TestModelDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder?.ApplyConfigurationsFromAssembly(typeof(SafeAssemblySearchAncestor).Assembly);
        }
    }
}