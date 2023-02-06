using System;
using System.ComponentModel;
using Stenn.AppData.Contracts;

namespace Stenn.AppData
{
    public sealed class AppDataServiceMockOptions<TBaseEntity>
        where TBaseEntity : IAppDataEntity
    {
        public AppDataServiceMockOptions(MockStrategy strategy)
        {
            if (!Enum.IsDefined(typeof(MockStrategy), strategy))
            {
                throw new InvalidEnumArgumentException(nameof(strategy), (int)strategy, typeof(MockStrategy));
            }
            if (strategy == MockStrategy.None)
            {
                throw new ArgumentOutOfRangeException();
            }
            Strategy = strategy;
        }

        public MockStrategy Strategy { get; }
    }
}