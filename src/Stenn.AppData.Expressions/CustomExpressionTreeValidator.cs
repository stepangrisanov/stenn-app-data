using System.Reflection;
using Stenn.AppData.Contracts;
using Stenn.AppData.Expressions;

namespace Stenn.AppData.Server
{
    internal sealed class CustomExpressionTreeValidator<TBaseEntity> : ExpressionTreeValidator<TBaseEntity>
        where TBaseEntity : IAppDataEntity
    {
        private readonly Func<MethodInfo, bool> _additionalValidationFunc;

        public CustomExpressionTreeValidator(Func<MethodInfo, bool> additionalValidationFunc) : base()
        {
            _additionalValidationFunc = additionalValidationFunc;
        }

        /// <inheritdoc />
        protected override bool CheckAllowed(MethodInfo method)
        {
            return base.CheckAllowed(method) || _additionalValidationFunc(method);
        }
    }
}