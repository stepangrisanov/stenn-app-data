using Stenn.AppData.Contracts;
using Stenn.TestModel.Domain.AppService.Tests.Entities;

namespace Stenn.TestModel.Domain.AppService.Tests
{
    public interface ITestModelDataService : IAppDataService<ITestModelEntity>
    {
    }
}