using Microsoft.AspNetCore.Mvc;
using Stenn.AppData.Contracts;
using Stenn.AppData.Server;

namespace Stenn.AppData.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class AppServiceApiController<TBaseEntity> : BaseAppServiceController<TBaseEntity> 
        where TBaseEntity : IAppDataEntity
    {
        /// <inheritdoc />
        protected AppServiceApiController(IAppDataServiceServer<TBaseEntity> service) 
            : base(service)
        {
        }

        [HttpPost("[action]")]
        public virtual Task<IActionResult> Query()
        {
            return DoQuery();
        }
    }
}