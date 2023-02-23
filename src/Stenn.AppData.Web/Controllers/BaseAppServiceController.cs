using Microsoft.AspNetCore.Mvc;
using Stenn.AppData.Contracts;
using Stenn.AppData.Server;

namespace Stenn.AppData.Web.Controllers
{
    public abstract class BaseAppServiceController<TBaseEntity> : Controller 
        where TBaseEntity : IAppDataEntity
    {
        protected IAppDataServiceServer<TBaseEntity> Service { get; }

        protected BaseAppServiceController(IAppDataServiceServer<TBaseEntity> service)
        {
            Service = service;
        }

        protected virtual async Task<IActionResult> DoQuery()
        {
            try
            {
                var reader = new StreamReader(Request.Body);
                var serializedQuery = await reader.ReadToEndAsync();
                var bytes = Service.ExecuteSerializedQuery(serializedQuery);
                return File(bytes, "application/octet-stream");
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
