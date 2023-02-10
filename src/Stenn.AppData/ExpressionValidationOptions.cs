using System;
using System.Reflection;

namespace Stenn.AppData
{
    public class ExpressionValidationOptions<TBaseEntity>
    {
        public Func<MethodInfo, bool>? validationFunc;
    }
}
