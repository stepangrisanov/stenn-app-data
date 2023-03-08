using Seedwork.Network.Contracts.Rpc;
using Stenn.TestModel.Domain.Tests.Entities;

namespace Stenn.TestModel.AppService.Client.Models
{
    public class CountryResponse : IRemoteCallResponse<CountryRequest, CountryResponse>, ITestServiceResponse
    {
        public CountryResponse()
        {

        }

        //public Country Country { get; set; }

        public int CountryId { get; set; }
    }
}
