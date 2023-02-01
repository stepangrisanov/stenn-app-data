using Stenn.AppData.Contracts;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#nullable disable

namespace Stenn.AppData.Client
{
    public class QueryProvider : IQueryProvider
    {
        private readonly IAppDataServiceClient _client;

        public QueryProvider(IAppDataServiceClient client)
        {
            _client = client;
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException!;
            }
        }

        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return new Query<T>(this, expression);
        }

        public object Execute(Expression expression)
        {
            //TODO: проверить откуда это может вызываться. найти тип который возвращает expression и передать его вместо object?
            return Execute<object>(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            /*// заменяем в выражении обращение к типу Query<T> на вызов метода Query<T> на сервисе
            //var param = Expression.Parameter(_service.GetType(), "service");
            var param = Expression.Parameter(typeof(IAppDataService<>), "service");
            var visitor = new ExpressionTreeModifier(param);
            var newExpression = visitor.Visit(expression);

            // строим лямбду которая будет принимать экземпляр сервиса
            var lambdaExpression = Expression.Lambda(newExpression, new ParameterExpression[] { param });

            var serializer = new ExpressionSerializer();
            var slimExpression = serializer.Lift(lambdaExpression);
            var bonsai = serializer.Serialize(slimExpression);

            // передаем сериализованное выражение в метод сервиса который умеет его исполнять и получаем массив байт сериализованных данных
            //var bytes = _service.ExecuteSerializedQuery(bonsai);

            var bytes = _client.ExecuteRemote(bonsai);

            var result = _client.Deserialize<TResult>(bytes);

            return result!;*/

            return _client.Execute<TResult>(expression);
        }
    }
}
