using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Stenn.AppData.Contracts;

namespace Stenn.AppData
{
    public class AppDataService<TDbContext, TBaseEntity> : AppDataService<TBaseEntity>
        where TDbContext : DbContext
        where TBaseEntity : class, IAppDataEntity
    {
        private TDbContext DBContext { get; }

        protected AppDataService(TDbContext dbContext, IEnumerable<IAppDataProjection<TBaseEntity>> projections)
        :base(projections)
        {
            DBContext = dbContext;
        }

        protected override IQueryable<T> Set<T>()
        {
            return DBContext.Set<T>().AsNoTracking();
        }

        protected override IQueryable<T>? GetProjectionQuery<T>()
        {
            return base.GetProjectionQuery<T>()?.AsNoTracking();
        }


    }
}