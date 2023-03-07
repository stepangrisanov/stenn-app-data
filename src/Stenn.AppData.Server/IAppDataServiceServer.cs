using Stenn.AppData.Contracts;

namespace Stenn.AppData.Server
{
    public interface IAppDataServiceServer<in TBaseEntity> : IAppDataServiceServer
        where TBaseEntity : IAppDataEntity
    {
    }

    public interface IAppDataServiceServer
    {
        byte[] ExecuteSerializedQuery(string bonsai, string? serializerName = null);
    }
}