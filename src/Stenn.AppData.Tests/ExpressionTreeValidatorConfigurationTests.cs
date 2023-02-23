using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.TestModel.Domain.AppService.Tests;
using Stenn.TestModel.Domain.AppService.Tests.Entities;
using System.Linq.Expressions;
using Stenn.AppData.Contracts;
using Stenn.AppData.Expressions;
using Stenn.AppData.Server;

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

        protected static string GetConnectionString()
        {
            return AppDataTests.GetConnectionString();
        }

        [Test]
        public void Server_WithConfiguration()
        {
            var services = new ServiceCollection();
            var connString = GetConnectionString();

            // provide additional expression validation function to allow additional methods
            services.AddTestModelAppDataServiceServer(connString, x => x.Name == nameof(Directory.GetCurrentDirectory));

            var serviceProvider = services.BuildServiceProvider();
            var serviceServer = serviceProvider.GetRequiredService<IAppDataServiceServer<ITestModelEntity>>();

            var result = BuildAndExecuteExpression(serviceServer);
            result.Should().NotBeNull();
        }

        [Test]
        public void Server_WithoutConfiguration()
        {
            var services = new ServiceCollection();
            var connString = GetConnectionString();

            // no additional expression validation function provided
            services.AddTestModelAppDataServiceServer(connString);

            var serviceProvider = services.BuildServiceProvider();
            var serviceServer = serviceProvider.GetRequiredService<IAppDataServiceServer<ITestModelEntity>>();

            Action act = () => BuildAndExecuteExpression(serviceServer);
            act.Should().Throw<InvalidOperationException>().Where(e => e.Message.StartsWith("Expression contains not allowed method"));
        }

        [Test]
        public void Client_WithConfiguration()
        {
            var services = new ServiceCollection();

            // provide additional expression validation function to allow additional methods
            services.AddTestModelAppDataService(string.Empty, x => x.Name == nameof(Directory.GetCurrentDirectory));

            var serviceProvider = services.BuildServiceProvider();
            var appDataServiceClient = serviceProvider.GetRequiredService<IAppDataService<ITestModelEntity>>();

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Action act = () => appDataServiceClient.Query<TestModelCountry>().Select(x => new { Text = Directory.GetCurrentDirectory() }).First();
            
            // this exception happens when sending data, which means validation passed
            act.Should().Throw<InvalidOperationException>().Where(e => e.Message.StartsWith("An invalid request URI was provided"));
        }

        [Test]
        public void Client_WithoutConfiguration()
        {
            var services = new ServiceCollection();

            // no additional expression validation function provided
            services.AddTestModelAppDataService(string.Empty);

            var serviceProvider = services.BuildServiceProvider();
            var appDataServiceClient = serviceProvider.GetRequiredService<IAppDataService<ITestModelEntity>>();

            Action act = () => appDataServiceClient.Query<TestModelCountry>().Select(x => new { Text = Directory.GetCurrentDirectory() }).First();
            act.Should().Throw<InvalidOperationException>().Where(e => e.Message.StartsWith("Expression contains not allowed method"));
        }

        private static string BuildAndExecuteExpression(IAppDataServiceServer serviceServer)
        {
            var ex = Expression.Call(typeof(Directory), "GetCurrentDirectory", null, null);
            var param = Expression.Parameter(typeof(object), "service");
            var le = Expression.Lambda<Func<object, string>>(ex, param);

            var serializer = new ExpressionSerializer();
            var slimExpression = serializer.Lift(le);
            var bonsai = serializer.Serialize(slimExpression);
            var jsonResult = serviceServer.ExecuteSerializedQuery(bonsai);
            var result = System.Text.Json.JsonSerializer.Deserialize<string>(jsonResult);
            return result;
        }
    }
}
