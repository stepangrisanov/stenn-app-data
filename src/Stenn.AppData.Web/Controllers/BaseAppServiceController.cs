using Microsoft.AspNetCore.Mvc;
using Stenn.AppData.Contracts;
using Stenn.AppData.Server;
using System.Text.RegularExpressions;

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
            string? serializerName = null;

            try
            {
                var acceptHeader = HttpContext.Request.Headers.Accept;
                if (!string.IsNullOrEmpty(acceptHeader) )
                {
                    var regex = new Regex("serializer=(?'name'[^;]+)");
                    serializerName = regex.Match(acceptHeader!).Groups["name"].Value;
                }
                
                var reader = new StreamReader(Request.Body);
                var serializedQuery = await reader.ReadToEndAsync();
                var bytes = Service.ExecuteSerializedQuery(serializedQuery, serializerName);
                return File(bytes, "application/octet-stream");
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
