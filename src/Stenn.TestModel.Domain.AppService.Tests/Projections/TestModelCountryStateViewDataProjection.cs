using System.Linq;
using Stenn.TestModel.Domain.AppService.Tests.Entities;

namespace Stenn.TestModel.Domain.AppService.Tests.Projections
{
    internal sealed class TestModelCountryStateViewDataProjection : TestModelDataProjection<TestModelCountryStateView>
    {
        /// <inheritdoc />
        public TestModelCountryStateViewDataProjection(TestModelAppDataServiceDbContext dbContext)
            : base(dbContext)
        {
        }

        /// <inheritdoc />
        public override IQueryable<TestModelCountryStateView> Query()
        {
            return DBContext.Set<TestModelCountryState>().Select(s => new TestModelCountryStateView
            {
                StateId = s.Id,
                Description = s.Description,
                CountryName = s.Country.Name,
                CountryAlpha3Code = s.Country.Alpha3Code,
                CountryNumeric3Code = s.Country.Numeric3Code,
            });
        }
    }
}