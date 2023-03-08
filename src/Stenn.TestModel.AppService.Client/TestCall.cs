using Stenn.TestModel.AppService.Client.Models;
using System.Threading;

namespace Stenn.TestModel.AppService.Client
{
    internal class TestCall
    {
        public TestCall()
        {
            var client = new TestServiceClient<ITestServiceRequest, ITestServiceResponse>(null);
            client.CallAsync(new CountryRequest(), CancellationToken.None);
            //client.CallAsync(new CountryStateRequest(), CancellationToken.None); //should show error
        }
    }
}
