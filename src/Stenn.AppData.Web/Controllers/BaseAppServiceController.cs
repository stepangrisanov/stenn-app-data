using Microsoft.AspNetCore.Mvc;
using Stenn.AppData.Contracts;
using System.IO;
using System.Threading.Tasks;

namespace Stenn.AppData.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class BaseAppServiceController<TService, TBaseEntity> : Controller where TService : IAppDataService<TBaseEntity> where TBaseEntity : IAppDataEntity
    {
        private readonly TService _service;

        public BaseAppServiceController(TService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public virtual async Task<IActionResult> ExecuteSerializedExpression()
        {
            try
            {
                var reader = new StreamReader(Request.Body);
                var serializedQuery = await reader.ReadToEndAsync();
                var bytes = _service.ExecuteSerializedQuery(serializedQuery);
                return File(bytes, "application/octet-stream");
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
