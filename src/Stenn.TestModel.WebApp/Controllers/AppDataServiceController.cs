using Microsoft.AspNetCore.Mvc;
using Stenn.AppData.Server;
using Stenn.AppData.Web.Controllers;
using Stenn.TestModel.Domain.AppService.Tests;

namespace Stenn.TestModel.WebApp.Controllers
{
    public class AppDataServiceController : AppServiceApiController<ITestModelEntity>
    {
        public AppDataServiceController(IAppDataServiceServer<ITestModelEntity> service)
            : base(service)
        {
        }

        [HttpGet("[action]")]
        public ActionResult Hello()
        {
            return Content("Hello world");
        }

        /// <inheritdoc />
        [HttpPost("[action]")]
        public override Task<IActionResult> Query()
        {
            return base.Query();
        }
    }
    
    
}