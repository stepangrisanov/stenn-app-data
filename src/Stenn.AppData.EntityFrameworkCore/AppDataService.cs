using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
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

        // выполняет Expression полученный в сериализованном виде и возвращает сериализованный результат.
        // предполагается что expression имеет вид (AppDataService s) => s.Query<T>.OtherOperations()
        public override byte[] ExecuteSerializedQuery(string bonsai)
        {
            var serializer = new ExpressionSerializer();

            var deserializedExpression = (LambdaExpression)serializer.Reduce(serializer.Deserialize(bonsai));

            var expressionValidator = new ExpressionTreeValidator();
            deserializedExpression = (LambdaExpression)expressionValidator.Visit(deserializedExpression);

            var resultType = deserializedExpression.ReturnType;

            var func = deserializedExpression.Compile();
            var res = func.DynamicInvoke(this);

            if (IsSubclassOfRawGeneric(typeof(IQueryable<>), resultType))
            {
                var innerType = resultType.GetGenericArguments()[0];
                resultType = typeof(IEnumerable<>).MakeGenericType(innerType);

                var castMethod = GetType().GetMethod("WriteGenericList")!.MakeGenericMethod(innerType);
                res = castMethod.Invoke(this, new object[] { res! });
            }

            var serializedResponse = GetType()
                .GetMethod("Serialize")!
                .MakeGenericMethod(resultType)
                .Invoke(this, new object[] { res! });

            return (byte[])serializedResponse!;
        }

        public IEnumerable<T> WriteGenericList<T>(object obj)
        {
            return ((IEnumerable<T>)obj).ToList();
        }

        static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
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

        public byte[] Serialize<T>(T obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }
    }
}