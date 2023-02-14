using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stenn.TestModel.Domain.AppService.Tests;
using Stenn.TestModel.Domain.AppService.Tests.Entities;
using Stenn.TestModel.Domain.Tests;

#nullable disable

namespace Stenn.PerformanceTests
{
    [MemoryDiagnoser]
    public class SerializeVsSerializeAsyncTests
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
        private ServiceProvider ServiceProvider { get; set; }

        //private List<TestModelCountry> Data { get; set; }

        [Params(1, 10, 100)]
        public int NumberOfEntities { get; set; }

        [GlobalSetup]
        public async Task OneTimeSetUp()
        {
            var serviceCollection = InitServices();

            ServiceProvider = serviceCollection.BuildServiceProvider();
            AppDataService = ServiceProvider.GetRequiredService<ITestModelDataService>();

            var dbContext = ServiceProvider.GetRequiredService<TestModelDbContext>();
            await dbContext.Database.EnsureCreatedAsync(TestCancellationToken);
        }

        [GlobalCleanup]
        public async Task OneTimeTearDown()
        {
            var dbContext = ServiceProvider.GetRequiredService<TestModelDbContext>();
            await dbContext.Database.EnsureDeletedAsync(TestCancellationToken);
        }

        /*[IterationSetup]
        public void EveryTestSetup()
        {
            var list = AppDataService.Query<TestModelCountry>().Take(NumberOfEntities).ToList();
            while (list.Count < NumberOfEntities)
            {
                list.AddRange(list);
            }
            Data = list.Take(NumberOfEntities).ToList();
        }*/

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

        [Benchmark]
        public List<TestModelCountry> SerializeJson()
        {
            var data = AppDataService.Query<TestModelCountry>().Take(NumberOfEntities).ToList();
            var serialized = System.Text.Json.JsonSerializer.Serialize(data);
            var result = System.Text.Json.JsonSerializer.Deserialize<List<TestModelCountry>>(serialized);

            return result;
        }

        [Benchmark]
        public List<TestModelCountry> SerializeUtf8Bytes()
        {
            var data = AppDataService.Query<TestModelCountry>().Take(NumberOfEntities).ToList();
            var serialized = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(data);
            var result = System.Text.Json.JsonSerializer.Deserialize<List<TestModelCountry>>(serialized);

            return result;
        }

        [Benchmark]
        public async Task<List<TestModelCountry>> SerializeJsonAsync()
        {
            var query = AppDataService.Query<TestModelCountry>().Take(NumberOfEntities);
            using MemoryStream dataStream = new MemoryStream();
            await System.Text.Json.JsonSerializer.SerializeAsync(dataStream, query);
            dataStream.Position = 0;
            var result = await System.Text.Json.JsonSerializer.DeserializeAsync<List<TestModelCountry>>(dataStream);

            return result;
        }

        public static void Run()
        {
            BenchmarkRunner.Run<SerializeVsSerializeAsyncTests>();
        }
    }
}
