using Seedwork.Network.Contracts.Rpc;
using Stenn.AppData.Contracts;

namespace Stenn.TestModel.AppService.Contracts.Models
{
    public class CountryRequest : IRemoteCallRequest<CountryRequest, CountryResponse>, ITestServiceRequest
    {
        public IAppDataRequestFilter Filter {
            get { return null; }
            set { return; }
        }
    }
}
