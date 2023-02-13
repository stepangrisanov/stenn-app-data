using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Stenn.AppData.Contracts;

namespace Stenn.AppData
{
    public abstract class AppDataService<TBaseEntity> : IAppDataService<TBaseEntity>
        where TBaseEntity : class, IAppDataEntity
    {
        private readonly IEnumerable<IAppDataProjection<TBaseEntity>> _projections;

        private readonly ExpressionTreeValidator<TBaseEntity> _expressionValidator;

        protected AppDataService(IEnumerable<IAppDataProjection<TBaseEntity>> projections, ExpressionTreeValidator<TBaseEntity>? expressionValidator = null)
        {
            _projections = projections.ToList();
            _expressionValidator = expressionValidator ?? new ExpressionTreeValidator<TBaseEntity>();
        }

        /// <inheritdoc />
        public IQueryable<T> Query<T>()
            where T : class, TBaseEntity
        {
            return GetProjectionQuery<T>() ?? Set<T>();
        }

        public abstract byte[] ExecuteSerializedQuery(string bonsai);

        protected abstract IQueryable<T> Set<T>()
            where T : class;

        protected virtual IQueryable<T>? GetProjectionQuery<T>()
            where T : class, TBaseEntity
        {
            return GetProjection<T>()?.Query();
        }

        protected IAppDataProjection<T, TBaseEntity>? GetProjection<T>()
            where T : class, TBaseEntity
        {
            return (IAppDataProjection<T, TBaseEntity>?)_projections.SingleOrDefault(p => p.GetEntityType() == typeof(T));
        }

        protected Expression ValidateExpression(Expression expression)
        {
            return _expressionValidator.Visit(expression);
        }
    }
}