using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Stenn.TestModel.Domain.Tests;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Stenn.TestModel.Domain.AppService.Tests;
using Stenn.TestModel.Domain.AppService.Tests.Entities;
using Stenn.TestModel.Domain.Tests.Entities.Declarations;
using Stenn.EntityFrameworkCore.Testing;

namespace Stenn.AppData.Tests
{
    /// <summary>
    /// Tests sample for producer app service mock
    /// </summary>
    [TestFixture]
    public class AppDataMockStrategyTests
    {
        protected readonly CancellationToken TestCancellationToken = CancellationToken.None;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var serviceCollection = InitServices();

            ServiceProvider = serviceCollection.BuildServiceProvider();
            AppDataService = ServiceProvider.GetRequiredService<ITestModelDataService>();
        }

        

        internal static IServiceCollection InitServices(MockStrategy mockStrategy)
        {
            var services = new ServiceCollection();

            services.AddTestModelAppDataService(connString);

            return services;
        }

        [Test]
        public async Task TestEntities()
        {
            var countries = await AppDataService.Query<TestModelCountry>().ToListAsync(TestCancellationToken);
            var countriesActual = CountryDeclaration.GetActual().ToList();
            countries.Count.Should().Be(countriesActual.Count);

            var states = await AppDataService.Query<TestModelCountryState>().Take(5).Include(s => s.Country)
                .ToListAsync(TestCancellationToken);

            states.Count.Should().Be(5);
        }

        [Test]
        public async Task TestProjections()
        {
            // data service should return one CountryState entity - the on filled inside InitEntities()
            var countries = await AppDataService.Query<TestModelCountryStateView>().Take(5).ToListAsync(TestCancellationToken);
            countries.Count.Should().Be(5);

            // data service should return one CountryState entity - the on filled inside InitEntities()
            var constant = await AppDataService.Query<TestModelConstantView>().ToListAsync(TestCancellationToken);
            constant.Count.Should().Be(1);
        }
        
        [Test]
        public void CheckMapping()
        {
            var dbContext = ServiceProvider.GetRequiredService<TestModelAppDataServiceDbContext>();
            dbContext.CheckEntities();
        }
    }
}