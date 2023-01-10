using System.Collections.Generic;
using System.Linq;
using Stenn.EntityFrameworkCore;
using Stenn.TestModel.Domain.AppService.Tests.Entities;

namespace Stenn.TestModel.Domain.AppService.Tests.Projections
{
    internal sealed class TestModelConstantViewDataProjection : ITestModelDataProjection<TestModelConstantView>
    {
        private readonly IQueryable<TestModelConstantView> _query;

        public static readonly string ConstantValue = "RR";

        public TestModelConstantViewDataProjection()
        {

            _query = new List<TestModelConstantView>
            {
                new()
                {
                    ConstId = ConstantValue
                }
            }.AsQueryableFixed();
        }

        public IQueryable<TestModelConstantView> Query()
        {
            return _query;
        }
    }
}