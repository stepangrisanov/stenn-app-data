namespace Stenn.TestModel.IntegrationTests.HttpClientFactory
{
    public static class DelegateHttpClientFactoryExtensions
    {
        public static IServiceCollection AddDelegateHttpClientFactory(this IServiceCollection services, IHttpClientActivator clientActivator)
        {
            services.AddSingleton(clientActivator);
            services.AddSingleton<DelegateHttpClientFactory>();
            services.AddSingleton<IHttpClientFactory>(p => p.GetRequiredService<DelegateHttpClientFactory>());
            services.AddSingleton<IHttpMessageHandlerFactory>(p => p.GetRequiredService<DelegateHttpClientFactory>());

            return services;
        }
    }
}