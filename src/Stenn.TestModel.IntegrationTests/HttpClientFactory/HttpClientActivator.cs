namespace Stenn.TestModel.IntegrationTests.HttpClientFactory
{
    public interface IHttpClientActivator
    {
        HttpClient CreateClient(HttpMessageHandler handler, bool disposeHandler);
    }
}