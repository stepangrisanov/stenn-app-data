using Stenn.AppData;

namespace Stenn.TestModel.Domain.AppService.Tests.Projections
{
    public interface ITestModelDataProjection<out T> : IAppDataProjection<T, ITestModelEntity>
        where T : class, ITestModelEntity
    {
    }
}