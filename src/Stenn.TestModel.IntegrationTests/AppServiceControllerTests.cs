using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using Stenn.TestModel.WebApp;
using Stenn.TestModel.Domain.AppService.Tests;
using Stenn.TestModel.Domain.AppService.Tests.Entities;
using FluentAssertions;
using Stenn.TestModel.Domain.Tests;
using Microsoft.EntityFrameworkCore;

namespace Stenn.TestModel.IntegrationTests
{
    [TestFixture]
    public class AppServiceControllerTests
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

        private readonly WebApplicationFactory<Program> _applicationFactory = new WebApplicationFactory<Program>();

        public HttpClient GetClient() => _applicationFactory.CreateClient();

        private ServiceProvider? ServiceProvider { get; set; }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var serviceCollection = InitServices();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var dbContext = ServiceProvider.GetRequiredService<TestModelDbContext>();
            await dbContext.Database.EnsureCreatedAsync(TestCancellationToken);
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            var dbContext = ServiceProvider!.GetRequiredService<TestModelDbContext>();
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
        public async Task GetIndexTest()
        {
            var hpptClient = GetClient();
            var response = await hpptClient.GetAsync("/TestService/Hello");
            response.EnsureSuccessStatusCode();
        }

        [Test]
        public void PostSerializedExpressionTest()
        {
            var httpClient = GetClient();

            Func<string, byte[]> func = (string s) =>
            {
                var result = httpClient.PostAsync("/TestService/ExecuteSerializedExpression", new StringContent(s)).Result;
                result.EnsureSuccessStatusCode();
                return result.Content.ReadAsByteArrayAsync().Result;
            };

            var appDataClient = new TestModelClient(func);

            var data = appDataClient.Query<TestModelCountry>().Take(5).ToList();

            data.Should().HaveCount(5);
        }
    }
}
