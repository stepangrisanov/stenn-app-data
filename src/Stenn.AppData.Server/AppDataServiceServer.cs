using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Stenn.AppData.Contracts;
using Stenn.AppData.Expressions;

namespace Stenn.AppData.Server
{
    public sealed class AppDataServiceServer<TBaseEntity, TAppDataService> : IAppDataServiceServer<TBaseEntity>
        where TBaseEntity : class, IAppDataEntity
    {
        private readonly TAppDataService _service;
        private readonly ExpressionTreeValidator<TBaseEntity> _expressionValidator;

        public AppDataServiceServer(TAppDataService service,
            ExpressionTreeValidator<TBaseEntity>? expressionValidator = null)
        {
            _service = service;
            _expressionValidator = expressionValidator ?? new ExpressionTreeValidator<TBaseEntity>();
        }

        // выполняет Expression полученный в сериализованном виде и возвращает сериализованный результат.
        // предполагается что expression имеет вид (AppDataService s) => s.Query<T>.OtherOperations()
        public byte[] ExecuteSerializedQuery(string bonsai)
        {
            var serializer = new ExpressionSerializer();

            var deserializedExpression = (LambdaExpression)serializer.Reduce(serializer.Deserialize(bonsai));

            deserializedExpression = (LambdaExpression)ValidateExpression(deserializedExpression);

            var resultType = deserializedExpression.ReturnType;

            var func = deserializedExpression.Compile();
            var res = func.DynamicInvoke(_service);

            if (IsSubclassOfRawGeneric(typeof(IQueryable<>), resultType))
            {
                var innerType = resultType.GetGenericArguments()[0];
                resultType = typeof(IEnumerable<>).MakeGenericType(innerType);

                var castMethod = GetType().GetMethod(nameof(WriteGenericList), BindingFlags.Static | BindingFlags.NonPublic)!.MakeGenericMethod(innerType);
                res = castMethod.Invoke(this, new[] { res! });
            }

            var serializedResponse = GetType()
                .GetMethod(nameof(Serialize), BindingFlags.Static | BindingFlags.NonPublic)!
                .MakeGenericMethod(resultType)
                .Invoke(this, new[] { res! });

            return (byte[])serializedResponse!;
        }

        private Expression ValidateExpression(Expression expression)
        {
            return _expressionValidator.Visit(expression);
        }

        private static IEnumerable<T> WriteGenericList<T>(object obj)
        {
            return ((IEnumerable<T>)obj).ToList();
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type? toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType!;
            }
            return false;
        }

        private static byte[] Serialize<T>(T obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }
    }
}