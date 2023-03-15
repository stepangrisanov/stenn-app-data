using Stenn.AppData.Contracts;
using System;

namespace Stenn.TestModel.Domain.AppService.Tests
{
    [Obsolete]
    public interface ITestModelDataService : IAppDataService<ITestModelEntity>
    {
    }
}