using Microsoft.AspNetCore.Mvc;
using Stenn.TestModel.Domain.AppService.Tests;
using Stenn.TestModel.Domain.AppService.Tests.Entities;

namespace Stenn.TestModel.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestServiceController : ControllerBase
    {
        private readonly ITestModelDataService _service;

        public TestServiceController(ITestModelDataService service)
        {
            _service = service;
        }

        [HttpGet("[action]")]
        public ActionResult Hello()
        {
            var query = _service.Query<TestModelConstantView>().Select(i => new { data = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")) });
            var res = query.FirstOrDefault()!.data;
            return Content(res);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ExecuteSerializedExpression()
        {
            var reader = new StreamReader(Request.Body);
            var serializedQuery = await reader.ReadToEndAsync();
            var bytes = _service.ExecuteSerializedQuery(serializedQuery);
            return File(bytes, "application/octet-stream");
        }
    }
}
