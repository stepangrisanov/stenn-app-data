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
using System;
using Stenn.AppData.Client;

namespace Stenn.AppData.Tests
{
    /// <summary>
    /// Tests sample for producer app service
    /// </summary>
    [TestFixture]
    public class AppDataSerializationTests
    {
#if NET6_0
        protected const string DBName = "test-appdata-service_net6";
#elif NET7_0
        protected const string DBName = "test-appdata-service_net7";
#elif NET8_0
        protected const string DBName = "test-appdata-service_net8";
#endif

        protected static string GetConnectionString(string dbName)
        {
            return $@"Data Source=.\SQLEXPRESS;Initial Catalog={dbName};MultipleActiveResultSets=True;Integrated Security=SSPI;Encrypt=False";
        }

        protected readonly CancellationToken TestCancellationToken = CancellationToken.None;

        private ITestModelDataService AppDataService { get; set; }
        private TestModelClient AppDataClient { get; set; }
        private ServiceProvider ServiceProvider { get; set; }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var serviceCollection = InitServices();

            ServiceProvider = serviceCollection.BuildServiceProvider();
            AppDataService = ServiceProvider.GetRequiredService<ITestModelDataService>();
            AppDataClient = new TestModelClient(AppDataService.ExecuteSerializedQuery);

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

            services.AddTestModelAppDataService(connString);

            return services;
        }

        [Test]
        public async Task TestClientCountry()
        {
            var countries = await AppDataService.Query<TestModelCountry>().ToListAsync(TestCancellationToken);
            var countriesActual = AppDataClient.Query<TestModelCountry>().ToList();

            countries.Count.Should().Be(countriesActual.Count);
        }

        [Test]
        public async Task TestClientCountryState()
        {
            var countries = await AppDataService.Query<TestModelCountryState>().ToListAsync(TestCancellationToken);
            var countriesActual = AppDataClient.Query<TestModelCountryState>().ToList();

            countries.Count.Should().Be(countriesActual.Count);
        }

        [Test]
        public async Task TestClientAnonymous()
        {
            var countries = await AppDataService.Query<TestModelCountry>().Select(i => new { Name = i.Name, Code = i.Alpha3Code }).ToListAsync(TestCancellationToken);
            var countriesActual = AppDataClient.Query<TestModelCountry>().Select(i => new { Name = i.Name, Code = i.Alpha3Code }).ToList();

            countries.Count.Should().Be(countriesActual.Count);
        }

        [Test]
        public async Task TestClientInclude()
        {
            var states = await AppDataService.Query<TestModelCountryState>().Take(5).Include(s => s.Country).FirstOrDefaultAsync(TestCancellationToken);

            var actualStates = AppDataClient.Query<TestModelCountryState>().Take(5).Include(s => s.Country).FirstOrDefault();

            actualStates.Country.Should().NotBeNull();
        }

        [Test]
        public void TestClientJoin()
        {
            var expectedResult = AppDataService.Query<TestModelCountry>()
                .Where(i=>i.Id == "US")
                .Join(AppDataService.Query<TestModelCountryState>(), c => c.Id, s => s.CountryId, (c, s) => new { Text = c.Name, Code = s.Description })
                .ToList();

            var result = AppDataClient.Query<TestModelCountry>()
                .Where(i => i.Id == "US")
                .Join(AppDataClient.Query<TestModelCountryState>(), c => c.Id, s => s.CountryId, (c, s) => new { Text = c.Name, Code = s.Description })
                .ToList();

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}