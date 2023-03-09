using Microsoft.EntityFrameworkCore;
using Seedwork.Network.Rpc;
using Stenn.TestModel.AppService.Contracts.Models;
using Stenn.TestModel.Domain.Tests;
using Stenn.TestModel.Domain.Tests.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Stenn.TestModel.AppService.Server.Handlers
{
    internal class CountryRequestHandler : IRemoteCallAsyncHandler<CountryRequest, CountryResponse>
    {
        private readonly TestModelDbContext _dbContext;

        public CountryRequestHandler(TestModelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CountryResponse> HandleAsync(CountryRequest request, IRemoteCallContext<CountryRequest, CountryResponse> context, CancellationToken token)
        {
            var country = await _dbContext.Set<Country>().FirstAsync();
            return new CountryResponse { Country = country }; // be carefull showing entities. use DTOs with appropriate fields
            //return new CountryResponse { CountryId = 5 };
        }
    }
}
