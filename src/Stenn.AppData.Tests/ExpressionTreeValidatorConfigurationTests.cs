using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Stenn.TestModel.Domain.AppService.Tests;
using Stenn.TestModel.Domain.AppService.Tests.Entities;
using Stenn.TestModel.Domain.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

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

            var result = BuildAndExecuteExpression();
            result.Should().BeNull();
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
