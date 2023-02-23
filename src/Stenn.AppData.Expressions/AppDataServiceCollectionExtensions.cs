using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Stenn.AppData.Contracts;
using Stenn.AppData.Server;

namespace Stenn.AppData.Expressions
{
    public static class AppDataServiceCollectionExtensions
    {
        /// <summary>
        /// Add <see cref="ExpressionTreeValidator{TBaseEntity}"/> for validate query expressions before run it
        /// </summary>
        /// <param name="services"></param>
        /// <param name="expressionValidationFunc"></param>
        /// <typeparam name="TBaseEntity"></typeparam>
        public static void AddAppDataServiceExpressionValidator<TBaseEntity>(this IServiceCollection services, Func<MethodInfo, bool>? expressionValidationFunc)
            where TBaseEntity : class, IAppDataEntity
        {
            if (expressionValidationFunc == null)
            {
                services.TryAddSingleton<ExpressionTreeValidator<TBaseEntity>>();
            }
            else
            {
                services.TryAddSingleton<ExpressionTreeValidator<TBaseEntity>>(new CustomExpressionTreeValidator<TBaseEntity>(expressionValidationFunc));
            }
        }
    }
}