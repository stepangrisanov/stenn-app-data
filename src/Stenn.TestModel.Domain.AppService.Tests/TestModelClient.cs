using Stenn.AppData.Client;
using System;

namespace Stenn.TestModel.Domain.AppService.Tests
{
    internal class TestModelClient : AppDataServiceClient<ITestModelEntity>
    {
        public TestModelClient(Func<string, byte[]> func) : base(func)
        {
        }
    }
}
