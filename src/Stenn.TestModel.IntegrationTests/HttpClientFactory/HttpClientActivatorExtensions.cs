using Microsoft.AspNetCore.Mvc.Testing;

namespace Stenn.TestModel.IntegrationTests.HttpClientFactory
{
    public static class HttpClientActivatorExtensions
    {
        public static IHttpClientActivator GetHttpClientActivator<TEntrypoint>(this WebApplicationFactory<TEntrypoint> factory)
            where TEntrypoint : class
        {
            return new WebApplicationFactoryHttpClientActivator<TEntrypoint>(factory);
        }
    }
}