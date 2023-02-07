using Stenn.AppData.Contracts;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text.Json;

#nullable disable

namespace Stenn.AppData.Client
{
    public class AppDataServiceClient<TBaseEntity> : IAppDataServiceClient, IAppDataService<TBaseEntity> where TBaseEntity : class, IAppDataEntity
    {
        private HttpClient _httpClient;

        public AppDataServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IQueryable<T> Query<T>() where T : class, TBaseEntity
        {
            return new Query<T>(this);
        }

        public T Deserialize<T>(byte[] bytes)
        {
            return JsonSerializer.Deserialize<T>(bytes);
        }

        public byte[] ExecuteRemote(string serializedExpression)
        {
            // no requestUri passed. httpClient's base address should already be configured to use correct uri
            var response = _httpClient.PostAsync(string.Empty, new StringContent(serializedExpression)).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadAsByteArrayAsync().Result;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            // заменяем в выражении обращение к типу Query<T> на вызов метода Query<T> на сервисе
            //var param = Expression.Parameter(_service.GetType(), "service");
            var param = Expression.Parameter(typeof(IAppDataService<TBaseEntity>), "service");
            var visitor = new ExpressionTreeModifier(param);
            var newExpression = visitor.Visit(expression);

            // строим лямбду которая будет принимать экземпляр сервиса
            var lambdaExpression = Expression.Lambda(newExpression, param);

            var serializer = new ExpressionSerializer();
            var slimExpression = serializer.Lift(lambdaExpression);
            var bonsai = serializer.Serialize(slimExpression);

            // передаем сериализованное выражение в метод сервиса который умеет его исполнять и получаем массив байт сериализованных данных
            var bytes = ExecuteRemote(bonsai);
            var result = Deserialize<TResult>(bytes);

            return result!;
        }

        public byte[] ExecuteSerializedQuery(string bonsai)
        {
            throw new NotImplementedException();
        }
    }
}
