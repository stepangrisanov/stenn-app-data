using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Seedwork.Configuration.Contracts;
using Seedwork.Network.Core.Abstractions;
using Seedwork.Network.Rpc.Http;
using Seedwork.Network.Rpc.Http.AspNet;
using Stenn.AppData.Contracts;
using Stenn.TestModel.AppService.Client;
using Stenn.TestModel.AppService.Contracts;
using Stenn.TestModel.AppService.Contracts.Models;
using Stenn.TestModel.AppService.Web;
using Stenn.TestModel.Domain.Tests.Entities;

namespace Stenn.TestModel.AppService.IntegrationTests
{
    [TestFixture]
    public class AppServiceOverRPCIntegrationTests
    {
        private static DbContext GetDbContext()
        {
            return Program.GetDbContext();
        }

        private readonly CancellationToken TestCancellationToken = CancellationToken.None;

        private readonly WebApplicationFactory<Program> _applicationFactory = new();

        protected IAppDataServiceClient<ITestServiceRequest, ITestServiceResponse> _testServiceClient { get; set; } = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var serviceCollection = InitServicesClient();

            var serviceProviderClient = serviceCollection.BuildServiceProvider().CreateScope().ServiceProvider;

            _testServiceClient = serviceProviderClient.GetRequiredService<IAppDataServiceClient<ITestServiceRequest, ITestServiceResponse>>();

            var dbContext = GetDbContext();
            await dbContext.Database.EnsureCreatedAsync(TestCancellationToken);
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            var dbContext = GetDbContext();
            await dbContext.Database.EnsureDeletedAsync(TestCancellationToken);
        }

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

        private IServiceCollection InitServicesClient()
        {
            var services = new ServiceCollection();

            services.AddSingleton(new EnvironmentConfig("RpcTests", "Test")); // add environment config

            services.AddScoped(c => GetHttpClient());
            services.AddSingleton<IHttpEndpointDiscoverer, TestHttpEndpointDiscoverer>();
            services.AddAspNetRpcClient();

            services.AddTestModelAppDataService();

            services.AddSingleton<IOperationContext>(new TestInitialOperationContext());

            return services;
        }

        /// <summary>
        /// Checking if webApp can process responses
        /// </summary>
        [Test]
        public async Task GetIndexTest()
        {
            var httpClient = GetHttpClient("/Hello");
            var response = await httpClient.GetAsync((string?)null, TestCancellationToken);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Request country over RPC
        /// </summary>
        [Test]
        public async Task ClientQueryTest()
        {
            var expected = GetDbContext().Set<Country>().First();
            var actual = await _testServiceClient.CallAsync(new CountryRequest(), TestCancellationToken); // intellisense will show error if request does not implement ITestServiceRequest
            actual.Country.Should().BeEquivalentTo(expected);
        }
    }
}
