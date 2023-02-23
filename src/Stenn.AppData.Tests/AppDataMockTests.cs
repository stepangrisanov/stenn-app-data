using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.AppData.Mock;
using Stenn.TestModel.Domain.AppService.Tests;
using Stenn.TestModel.Domain.AppService.Tests.Entities;

namespace Stenn.AppData.Tests
{
    /// <summary>
    /// Tests sample for consumer app service
    /// </summary>
    [TestFixture]
    internal class AppDataMockTests
    {
        private ITestModelDataService AppDataService { get; set; }
        private IServiceProvider ServiceProvider { get; set; }

        private static readonly TestModelCountry CountryNarnia = new("NN", "Narnia", "NRN", "555");
        private static readonly TestModelCountryState CountryState = new("CL", "Calormen", true, "NN");

        private static readonly TestModelCountryStateView CountryStateView = new()
            { StateId = "SV", CountryName = "Narnia", CountryAlpha2Code = "NN", CountryAlpha3Code = "NRN", CountryNumeric3Code = "555" };
        
        private static readonly TestModelConstantView ConstantView = new()
            { ConstId =  "CNST" };
        
        private static void InitEntities(MockAppDataServiceBuilder<ITestModelEntity> mockBuilder)
        {
            mockBuilder.Add(CountryNarnia);
            mockBuilder.Add(CountryState);
            mockBuilder.Add(CountryStateView);
            mockBuilder.Add(ConstantView);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var services = AppDataTests.InitServices();
            services.AddMockTestModelAppDataService(InitEntities, QueryTrackingBehavior.NoTracking);

            ServiceProvider = services.BuildServiceProvider();
            AppDataService = ServiceProvider.GetRequiredService<ITestModelDataService>();
        }

        [Test]
        public async Task MockedDataServiceShouldReturnTestData()
        {
            // data service should return one Country entity - the on filled inside InitEntities()
            var countries = await AppDataService.Query<TestModelCountry>().Where(i => i.Id == "NN").ToListAsync();
            countries.Count.Should().Be(1);

            // data service should return one CountryState entity - the on filled inside InitEntities()
            var states = await AppDataService
                .Query<TestModelCountryState>()
                .Where(i => i.CountryId == "NN")
                .Include(countryState => countryState.Country)
                .ToListAsync();
            states.Count.Should().Be(1);

            // related entity should be filled
            states.First().Country.Should().NotBeNull();
        }
        
        [Test]
        public async Task MockedDataServiceShouldReturnProjectionTestData()
        {
            // data service should return one CountryState entity - the on filled inside InitEntities()
            var countries = await AppDataService.Query<TestModelCountryStateView>().Where(i => i.StateId == "SV").ToListAsync();
            countries.Count.Should().Be(1);

            // data service should return one CountryState entity - the on filled inside InitEntities()
            var constant = await AppDataService.Query<TestModelConstantView>().ToListAsync();
            constant.Count.Should().Be(1);
        }

        [Test]
        public void RegistrationOfMockShouldClearPreviousDbContextRegistration()
        {
            var serviceCollection = AppDataTests.InitServices();
            serviceCollection.AddMockTestModelAppDataService(InitEntities, QueryTrackingBehavior.NoTracking);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var overridenContext = serviceProvider.GetRequiredService<TestModelAppDataServiceDbContext>();

            overridenContext.Database.ProviderName.Should().Be("Microsoft.EntityFrameworkCore.InMemory");

            overridenContext.Set<TestModelCountry>().Count().Should().Be(1);
            overridenContext.Set<TestModelCountry>().Single().Should().BeEquivalentTo(CountryNarnia);

            overridenContext.Set<TestModelCountryState>().Count().Should().Be(1);
            overridenContext.Set<TestModelCountryState>().Single().Should().BeEquivalentTo(CountryState);

            var overridenAppDataService = serviceProvider.GetRequiredService<ITestModelDataService>();
            overridenAppDataService.Query<TestModelCountry>().Count().Should().Be(1);
            overridenAppDataService.Query<TestModelCountry>().Single().Should().BeEquivalentTo(CountryNarnia);

            overridenAppDataService.Query<TestModelCountryState>().Count().Should().Be(1);
            overridenAppDataService.Query<TestModelCountryState>().Single().Should().BeEquivalentTo(CountryState);
        }
    }
}