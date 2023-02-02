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
    public class CompareSerializatorsTests
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

        private List<TestModelCountry> Data { get; set; }

        [Params(100, 1000, 10000)]
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

        [IterationSetup]
        public void EveryTestSetup()
        {
            var list = AppDataService.Query<TestModelCountry>().Take(NumberOfEntities).ToList();
            while (list.Count < NumberOfEntities)
            {
                list.AddRange(list);
            }
            Data = list.Take(NumberOfEntities).ToList();
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

        [Benchmark]
        public List<TestModelCountry> SerializeTextJson()
        {
            var serialized = System.Text.Json.JsonSerializer.Serialize(Data);
            var result = System.Text.Json.JsonSerializer.Deserialize<List<TestModelCountry>>(serialized);

            return result;
        }

        [Benchmark]
        public List<TestModelCountry> SerializeTextJsonUtf8Bytes()
        {
            var serialized = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(Data);
            var result = System.Text.Json.JsonSerializer.Deserialize<List<TestModelCountry>>(serialized);

            return result;
        }

        [Benchmark]
        public List<TestModelCountry> SerializeNewtonsoftJson()
        {
            var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(Data);
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TestModelCountry>>(serialized);

            return result;
        }

        [Benchmark]
        public List<TestModelCountry> SerializeMemoryPack()
        {
            var serialized = MemoryPack.MemoryPackSerializer.Serialize(Data);
            var result = MemoryPack.MemoryPackSerializer.Deserialize<List<TestModelCountry>>(serialized);

            return result;
        }

        [Benchmark]
        public List<TestModelCountry> SerializeMessagePackTypelessBinary()
        {
            var serialized = MessagePack.MessagePackSerializer.Typeless.Serialize(Data);
            var result = MessagePack.MessagePackSerializer.Typeless.Deserialize(serialized);

            return (List<TestModelCountry>)result;
        }

        public static void Run()
        {
            BenchmarkRunner.Run<CompareSerializatorsTests>();
        }
    }
}
