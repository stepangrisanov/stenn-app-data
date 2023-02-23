using Microsoft.EntityFrameworkCore;
using Stenn.TestModel.Domain.AppService.Tests;
using Stenn.TestModel.Domain.Tests;

namespace Stenn.TestModel.WebApp
{
    public partial class Program
    {
#if NET6_0
        private const string DBName = "test-appdata-service_net6";
#elif NET7_0
        protected const string DBName = "test-appdata-service_net7";
#elif NET8_0
        protected const string DBName = "test-appdata-service_net8";
#endif

        private static string GetConnectionString()
        {
            return $@"Data Source=.\SQLEXPRESS;Initial Catalog={DBName};MultipleActiveResultSets=True;Integrated Security=SSPI;Encrypt=False";
        }

        internal static DbContext GetDbContext()
        {
            var services = new ServiceCollection();
            var connString = GetConnectionString();
            AddPersistanceDbContext(services, connString);
            return services.BuildServiceProvider().GetRequiredService<TestModelDbContext>();
        }

        private static void AddPersistanceDbContext(IServiceCollection services, string connString)
        {
            services.AddDbContext<TestModelDbContext>(builder =>
            {
                builder.UseSqlServer(connString);
            });
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var connString = GetConnectionString();
            builder.Services.AddTestModelAppDataServiceServer(connString);
            AddPersistanceDbContext(builder.Services, connString);
            
            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}