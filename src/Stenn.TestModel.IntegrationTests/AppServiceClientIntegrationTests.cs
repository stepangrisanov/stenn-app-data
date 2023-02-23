using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Stenn.TestModel.Domain.AppService.Tests;
using Stenn.TestModel.Domain.AppService.Tests.Entities;
using Stenn.TestModel.WebApp;
using System.Linq.Expressions;
using Stenn.AppData.Contracts;
using Stenn.AppData.Expressions;
using Stenn.TestModel.IntegrationTests.HttpClientFactory;

namespace Stenn.TestModel.IntegrationTests
{
    [TestFixture]
    public class AppServiceClientIntegrationTests
    {
        private static DbContext GetDbContext()
        {
            return Program.GetDbContext();
        }

        private readonly CancellationToken TestCancellationToken = CancellationToken.None;

        private readonly WebApplicationFactory<Program> _applicationFactory = new();

        private HttpClient GetHttpClient(string? uri = null)
        {
            var client = _applicationFactory.CreateClient();
            SetBaseAddress(client, uri);
            return client;
        }

        private static void SetBaseAddress(HttpClient client, string? uri)
        {
            if (!string.IsNullOrEmpty(uri))
            {
                client.BaseAddress = new Uri(client.BaseAddress!, uri);
            }
        }

        private IServiceProvider ServiceProviderClient { get; set; } = null!;
        private IAppDataService<ITestModelEntity> AppDataServiceClient { get; set; } = null!;
        private IAppDataService<ITestModelEntity> AppDataServiceServer { get; set; } = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var serviceCollection = InitServicesClient();

            var serviceProviderClient = serviceCollection.BuildServiceProvider().CreateScope().ServiceProvider;
            AppDataServiceClient = serviceProviderClient.GetRequiredService<IAppDataService<ITestModelEntity>>();

            AppDataServiceServer = _applicationFactory.Services.CreateScope().ServiceProvider.GetRequiredService<IAppDataService<ITestModelEntity>>();
            
            var dbContext = GetDbContext();
            await dbContext.Database.EnsureCreatedAsync(TestCancellationToken);
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            var dbContext = GetDbContext();
            await dbContext.Database.EnsureDeletedAsync(TestCancellationToken);
        }

        private IServiceCollection InitServicesClient()
        {
            var services = new ServiceCollection();

            services.AddDelegateHttpClientFactory(_applicationFactory.GetHttpClientActivator());

            services.AddTestModelAppDataService(client => SetBaseAddress(client, "/AppDataService/Query"));

            return services;
        }

        /// <summary>
        /// Checking if webApp can process responses
        /// </summary>
        [Test]
        public async Task GetIndexTest()
        {
            var httpClient = GetHttpClient("/AppDataService/Hello");
            var response = await httpClient.GetAsync((string?)null, TestCancellationToken);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// AppDataClient builds expression and sends it to webApp. webApp executes expression on TestModelService to get data, serializes data and sends it back to client.
        /// </summary>
        [Test]
        public void ClientQueryTest()
        {
            var expected = AppDataServiceServer.Query<TestModelCountry>().Take(5).ToList();
            var actual = AppDataServiceClient.Query<TestModelCountry>().Take(5).ToList();

            actual.Should().BeEquivalentTo(expected);
        }
        
        /// <summary>
        /// AppDataClient builds expression and sends it to webApp. webApp executes expression on TestModelService to get data, serializes data and sends it back to client.
        /// </summary>
        [Test]
        public void ClientQueryWithAnonymousTypeTest()
        {
            var expected = AppDataServiceServer.Query<TestModelCountry>().Select(i => new { i.Name, Code = i.Alpha3Code }).ToList();
            var actual = AppDataServiceClient.Query<TestModelCountry>().Select(i => new { i.Name, Code = i.Alpha3Code }).ToList();

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ClientQueryIncludeTest()
        {
            var expected = AppDataServiceServer.Query<TestModelCountryState>().Take(5).Include(s => s.Country).ToList();
            var actual = AppDataServiceClient.Query<TestModelCountryState>().Take(5).Include(s => s.Country).ToList();

            actual.Should().BeEquivalentTo(expected);
            actual.First().Country.Should().NotBeNull();
        }

        [Test]
        public async Task ClientQueryJoinTest()
        {
            var expected = await AppDataServiceServer.Query<TestModelCountry>()
                .Where(i => i.Id == "US")
                .Join(AppDataServiceServer.Query<TestModelCountryState>(), c => c.Id, s => s.CountryId, (c, s) => new { Text = c.Name, Code = s.Description })
                .ToListAsync(cancellationToken: TestCancellationToken);

            var appDataClient = AppDataServiceClient;

            var actual = await appDataClient.Query<TestModelCountry>()
                .Where(i => i.Id == "US")
                .Join(appDataClient.Query<TestModelCountryState>(), c => c.Id, s => s.CountryId, (c, s) => new { Text = c.Name, Code = s.Description })
                .ToListAsync(cancellationToken: TestCancellationToken);

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ClientQueryOrderTest()
        {
            var expected = AppDataServiceServer.Query<TestModelCountryState>().OrderBy(i => i.Description).Take(5).ToList();
            var actual = AppDataServiceClient.Query<TestModelCountryState>().OrderBy(i => i.Description).Take(5).ToList();

            actual.Should().BeEquivalentTo(expected);
        }

        /// <summary>
        /// We can manually craft Expression to read remote service config. Remote service should validate expression before execution
        /// </summary>
        [Test]
        public void ReadRemoteConfigTest()
        {
            var httpClient = GetHttpClient();

            var pathExpression = Expression.Constant(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));
            var ex = Expression.Call(typeof(File), "ReadAllText", null, pathExpression);
            var param = Expression.Parameter(typeof(object), "service");
            var le = Expression.Lambda<Func<object, string>>(ex, param);

            var response = httpClient.PostAsync("/AppDataService/Query", new StringContent(SerializeExpression(le)), TestCancellationToken).Result;
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        private static string SerializeExpression(Expression expression)
        {
            var serializer = new ExpressionSerializer();
            var slimExpression = serializer.Lift(expression);
            return serializer.Serialize(slimExpression);
        }
    }
}