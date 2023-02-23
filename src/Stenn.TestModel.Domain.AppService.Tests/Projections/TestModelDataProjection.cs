namespace Stenn.TestModel.Domain.AppService.Tests.Projections
{
    internal abstract class TestModelDataProjection<T> : ITestModelDataProjection<T> where T : class, ITestModelEntity
    {
        protected TestModelDataProjection(TestModelAppDataServiceDbContext dbContext)
        {
            DBContext = dbContext;
        }

        protected TestModelAppDataServiceDbContext DBContext { get; }


        /// <inheritdoc />
        public abstract IQueryable<T> Query();
    }
}