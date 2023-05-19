using AG.LoggerViewer.UI.Application.Common;
using AG.LoggerViewer.UI.Application.Services;
using AG.LoggerViewer.UI.Application.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AG.LoggerViewer.UI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAgLogger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRazorPages();

            var loggerUtility = configuration.GetSection("AgLoggerViewer").Get<LoggerUtility>();

            if (loggerUtility == null)
                throw new AgLoggerExceptions("Cannot read logger utility, Please add {LoggerUtility} section");

            services.AddSingleton(loggerUtility);
            services.AddSingleton<DateTimeService>();
            services.AddScoped<ILoggerReadService, LoggerReadService>();
            return services;
        }

        public static IApplicationBuilder UseAgLogger(this IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });

            return app;
        }
    }
}