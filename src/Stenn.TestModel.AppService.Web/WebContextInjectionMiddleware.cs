using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Seedwork.DependencyInjection;
using Seedwork.Http.Contracts;
using Seedwork.Logging;
using Seedwork.Network.Core.Abstractions;
using Seedwork.Web.Middleware;
using Seedwork.Web;
using Seedwork;

namespace Stenn.TestModel.AppService.Web
{
    public class WebRequestOperationContextInjectorMiddleware : RequestContextFillerBase<HttpContext>
    {
        private readonly RequestDelegate _next;

        public WebRequestOperationContextInjectorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        protected override string GetDisplayUrl(HttpContext context)
        {
            Guard.DebugAssertArgumentNotNull(context, nameof(context));

            return context.Request.GetDisplayUrl();
        }

        protected override string GetActionName(HttpContext context)
        {
            Guard.DebugAssertArgumentNotNull(context, nameof(context));

#if NETSTANDARD2_0
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var metadata = endpoint?.Metadata.GetMetadata<RouteValuesAddressMetadata>();
            return metadata?.RequiredValues?.ContainsKey("action") == true ? (string)metadata.RequiredValues["action"] : "";
#elif NET5_0_OR_GREATER
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var endpointName = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>()?.ActionName;
            return endpointName;
#endif
        }

        protected override string GetIpAddress(HttpContext context)
        {
            Guard.DebugAssertArgumentNotNull(context, nameof(context));

            return context.Connection.RemoteIpAddress?.ToString();
        }

        protected override string GetLang(HttpContext context)
        {
            Guard.DebugAssertArgumentNotNull(context, nameof(context));

            return context.Request.Headers[StennHeaders.Lang];
        }

        protected override string GetStennApplication(HttpContext context)
        {
            Guard.DebugAssertArgumentNotNull(context, nameof(context));

            var stennApplication = context.Request.Headers[StennHeaders.StennApplication];

            if (!string.IsNullOrEmpty(stennApplication))
                return stennApplication;

            var referer = context.Request.GetTypedHeaders().Referer;
            return referer?.Host;
        }

        protected override string GetDistributedOperationId(HttpContext context)
        {
            Guard.DebugAssertArgumentNotNull(context, nameof(context));

            return context.Request.Headers[StennHeaders.DistributedOperationId];
        }

        protected override string GetCallerOperationId(HttpContext context)
        {
            Guard.DebugAssertArgumentNotNull(context, nameof(context));

            return context.Request.Headers[StennHeaders.CallerOperationId];
        }

        public async Task Invoke(HttpContext context, IDiScopeBuilderFactory factory)
        {
            Guard.DebugAssertArgumentNotNull(context, nameof(context));
            Guard.DebugAssertArgumentNotNull(factory, nameof(factory));

            var operationContext = new WebRequestOperationContext();
            FillOperationContext(operationContext, context);
            using var scopedLogContext = new ScopedLogContext();
            await factory.CreateBuilder()
                .AddInstance<IOperationContext>(operationContext)
                .AddInstance<ILogContext>(scopedLogContext)
                .BuildAndExecuteInScopeAsync((scope) =>
                {
                    var logContext = scope.GetRequiredService<ILogContext>();
                    (operationContext as ICanEnrichLogContext)?.Enrich(logContext);
                    return _next(context);
                });
        }
    }
}
