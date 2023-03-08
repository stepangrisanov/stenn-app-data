using Seedwork.Network.Contracts.Rpc;
using Stenn.AppData.Contracts;

namespace Stenn.TestModel.AppService.Client.Models
{
    public class CountryRequest : IRemoteCallRequest<CountryRequest, CountryResponse>, ITestServiceRequest
    {
        public CountryRequest()
        {

        }

        public int Id { get; set; } = 0;

        /*public IAppDataRequestFilter Filter {
            get { return null; }
            set { return; }
        }*/
    }
}
