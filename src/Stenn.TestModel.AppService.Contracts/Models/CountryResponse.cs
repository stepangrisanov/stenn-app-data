using Seedwork.Network.Contracts.Rpc;
using Stenn.TestModel.Domain.Tests.Entities;

namespace Stenn.TestModel.AppService.Contracts.Models
{
    public class CountryResponse : IRemoteCallResponse<CountryRequest, CountryResponse>, ITestServiceResponse
    {
        public Country Country { get; set; }

        //public int CountryId { get; set; }
    }
}
