using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Wishmaster.Mvc
{
    public class UrlHelperAccessor
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public UrlHelperAccessor(
            IActionContextAccessor actionContextAccessor,
            IUrlHelperFactory urlHelperFactory
        )
        {
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }

        public IUrlHelper GetHelper()
        {
            return _actionContextAccessor.ActionContext != null
                ? _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext)
                : throw new System.Exception("Service currently created not by controller action");
        }
    }

    public static class UrlHelperDependencyExtensions
    {
        public static IServiceCollection AddUrlHelperAccessor(this IServiceCollection services)
        {
            services.AddScoped<UrlHelperAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            return services;
        }
    }
}
