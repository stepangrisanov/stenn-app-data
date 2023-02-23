using Microsoft.AspNetCore.Mvc.Testing;

namespace Stenn.TestModel.IntegrationTests.HttpClientFactory
{
    internal sealed class WebApplicationFactoryHttpClientActivator<TEntrypoint> : IHttpClientActivator where TEntrypoint : class
    {
        private readonly WebApplicationFactory<TEntrypoint> _webApplicationFactory;

        public WebApplicationFactoryHttpClientActivator(WebApplicationFactory<TEntrypoint> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        public HttpClient CreateClient(HttpMessageHandler handler, bool disposeHandler) => _webApplicationFactory.CreateClient();
    }
}