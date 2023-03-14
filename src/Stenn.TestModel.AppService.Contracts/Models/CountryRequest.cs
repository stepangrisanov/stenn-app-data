﻿using Seedwork.Network.Contracts.Rpc;
using Stenn.AppData.Contracts;

namespace Stenn.TestModel.AppService.Contracts.Models
{
    public class CountryRequest : IRemoteCallRequest<CountryRequest, CountryResponse>
    {
        public IAppDataRequestOptions? RequestOptions { get; set; }
    }
}
