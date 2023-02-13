using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
//using Nuqleon.Json.Expressions;
using Stenn.AppData.Client;
using Stenn.TestModel.Domain.AppService.Tests;
using Stenn.TestModel.Domain.AppService.Tests.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;

namespace Stenn.AppData.Tests
{
    [TestFixture]
    public class ExpressionTreeValidatorConfigurationTests
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

        private ITestModelDataService AppDataService { get; set; }
        private TestModelClient AppDataServiceClient { get; set; }
        private ServiceProvider ServiceProvider { get; set; }

        [Test]
        public void ExpressionTreeValidatorWithConfiguration()
        {
            var services = new ServiceCollection();
            var connString = GetConnectionString(DBName);

            // provide additional expression validation function to allow additional methods
            services.AddTestModelAppDataService(connString, (x) => x.Name == nameof(Directory.GetCurrentDirectory));

            ServiceProvider = services.BuildServiceProvider();
            AppDataService = ServiceProvider.GetRequiredService<ITestModelDataService>();

            var result = BuildAndExecuteExpression();
            result.Should().NotBeNull();
        }

        [Test]
        public void ExpressionTreeValidatorWithoutConfiguration()
        {
            var services = new ServiceCollection();
            var connString = GetConnectionString(DBName);

            // no additional expression validation function provided
            services.AddTestModelAppDataService(connString);

            ServiceProvider = services.BuildServiceProvider();
            AppDataService = ServiceProvider.GetRequiredService<ITestModelDataService>();

            Action act = () => BuildAndExecuteExpression();
            act.Should().Throw<InvalidOperationException>().Where(e => e.Message.StartsWith("Expression contains not allowed method"));
        }

        [Test]
        public void ExpressionTreeClientValidatorWithConfiguration()
        {
            var services = new ServiceCollection();
            var connString = GetConnectionString(DBName);

            // provide additional expression validation function to allow additional methods
            services.AddTestModelAppDataServiceClient((x) => x.Name == nameof(Directory.GetCurrentDirectory));

            services.AddHttpClient<TestModelClient>();

            ServiceProvider = services.BuildServiceProvider();
            AppDataServiceClient = ServiceProvider.GetRequiredService<TestModelClient>();

            Action act = () => AppDataServiceClient.Query<TestModelCountry>().Select(x => new { Text = Directory.GetCurrentDirectory() }).First();
            // this exception happens when sending data, which means validation passed
            act.Should().Throw<InvalidOperationException>().Where(e => e.Message.StartsWith("An invalid request URI was provided"));
        }

        [Test]
        public void ExpressionTreeClientValidatorWithoutConfiguration()
        {
            var services = new ServiceCollection();
            var connString = GetConnectionString(DBName);

            // no additional expression validation function provided
            services.AddTestModelAppDataServiceClient();

            services.AddHttpClient<TestModelClient>();

            ServiceProvider = services.BuildServiceProvider();
            AppDataServiceClient = ServiceProvider.GetRequiredService<TestModelClient>();

            Action act = () => AppDataServiceClient.Query<TestModelCountry>().Select(x => new { Text = Directory.GetCurrentDirectory() }).First();
            act.Should().Throw<InvalidOperationException>().Where(e => e.Message.StartsWith("Expression contains not allowed method"));
        }

        private string BuildAndExecuteExpression()
        {
            var ex = Expression.Call(typeof(Directory), "GetCurrentDirectory", null, null);
            var param = Expression.Parameter(typeof(object), "service");
            Expression<Func<object, string>> le = Expression.Lambda<Func<object, string>>(ex, param);

            var serializer = new ExpressionSerializer();
            var slimExpression = serializer.Lift(le);
            var bonsai = serializer.Serialize(slimExpression);
            var jsonResult = AppDataService.ExecuteSerializedQuery(bonsai);
            var result = System.Text.Json.JsonSerializer.Deserialize<string>(jsonResult);
            return result;
        }
    }
}
