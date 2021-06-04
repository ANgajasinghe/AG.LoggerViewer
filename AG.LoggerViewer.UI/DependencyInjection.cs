using AG.LoggerViewer.Application.Commin;
using AG.LoggerViewer.Application.Services;
using AG.LoggerViewer.Application.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AG.LoggerViewer.UI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAGLogger(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddRazorPages();

            var loggerUtility = configuration.GetSection("AGLoggerViewer").Get<LoggerUtitlity>();

            if (loggerUtility == null) throw new AGLoggerExceptions("Cannot read logger utility, Please add {LoggerUtitlity} section");

            services.AddSingleton(loggerUtility);
            services.AddSingleton<DateTimeService>();
            services.AddScoped<ILoggerReadService,LoggerReadService>();



            return services;

        }

        public static IApplicationBuilder UserAGLogger(this IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            return app;
        }
    }
}
