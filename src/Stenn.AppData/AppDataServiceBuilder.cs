namespace Stenn.AppData
{
    public abstract class AppDataServiceBuilder<TBaseEntity> 
        where TBaseEntity : class
    {
        public abstract void AddProjection<T, TProjection>()
            where T : class, TBaseEntity
            where TProjection : class, IAppDataProjection<T, TBaseEntity>;
    }
}