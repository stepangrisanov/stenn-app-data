using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using Stenn.TestModel.Domain.Tests;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Stenn.TestModel.Domain.AppService.Tests;
using Stenn.TestModel.Domain.AppService.Tests.Entities;
using Stenn.TestModel.Domain.Tests.Entities.Declarations;
using Stenn.EntityFrameworkCore.Testing;

namespace Stenn.AppData.Tests
{
    /// <summary>
    /// Tests sample for producer app service
    /// </summary>
    [TestFixture]
    public class AppDataTests
    {
#if NET6_0
        protected const string DBName = "test-appdata-service_net6";
#elif NET7_0
        protected const string DBName = "test-appdata-service_net7";
#elif NET8_0
        protected const string DBName = "test-appdata-service_net8";
#endif

        internal static string GetConnectionString(string dbName = DBName)
        {
            return $@"Data Source=.\SQLEXPRESS;Initial Catalog={dbName};MultipleActiveResultSets=True;Integrated Security=SSPI;Encrypt=False";
        }

        protected readonly CancellationToken TestCancellationToken = CancellationToken.None;

        private ITestModelDataService AppDataService { get; set; }
        private ServiceProvider ServiceProvider { get; set; }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var serviceCollection = InitServices();

            ServiceProvider = serviceCollection.BuildServiceProvider();
            AppDataService = ServiceProvider.GetRequiredService<ITestModelDataService>();

            var dbContext = ServiceProvider.GetRequiredService<TestModelDbContext>();
            await dbContext.Database.EnsureCreatedAsync(TestCancellationToken);
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            var dbContext = ServiceProvider.GetRequiredService<TestModelDbContext>();
            await dbContext.Database.EnsureDeletedAsync(TestCancellationToken);
        }

        internal static IServiceCollection InitServices(string dbName = DBName)
        {
            var services = new ServiceCollection();

            var connString = GetConnectionString(dbName);
            services.AddDbContext<TestModelDbContext>(builder =>
            {
                builder.UseSqlServer(connString);
            });

            services.AddTestModelAppDataServiceV1(connString);

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