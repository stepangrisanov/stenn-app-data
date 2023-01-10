using Stenn.AppData;
using Stenn.TestModel.Domain.AppService.Tests.Entities;

namespace Stenn.TestModel.Domain.AppService.Tests.Projections
{
    internal static class InitProjections
    {
        /// <summary>
        /// Projections registration
        /// </summary>
        /// <param name="builder"></param>
        internal static void Init(AppDataServiceBuilder<ITestModelEntity> builder)
        {
            //NOTE: Register projections here (e.g)
            builder.AddProjection<TestModelConstantView, TestModelConstantViewDataProjection>();
            builder.AddProjection<TestModelCountryStateView, TestModelCountryStateViewDataProjection>();
        }
    }
}