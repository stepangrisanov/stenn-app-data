using Microsoft.AspNetCore.Mvc;
using Stenn.AppData.Web.Controllers;
using Stenn.TestModel.Domain.AppService.Tests;

namespace Stenn.TestModel.WebApp.Controllers
{
    public class AppDataServiceController : BaseAppServiceController<ITestModelDataService, ITestModelEntity>
    {
        private readonly ITestModelDataService _service;

        public AppDataServiceController(ITestModelDataService service) : base(service)
        {
            _service = service;
        }

        [HttpGet("[action]")]
        public ActionResult Hello()
        {
            return Content("Hello world");
        }

        [HttpPost("[action]")]
        public override async Task<IActionResult> ExecuteSerializedExpression()
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
