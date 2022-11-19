using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using logcat_monitor.configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace logcat_monitor.extenstions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection ConfigureAppConfig(this IServiceCollection services, IConfiguration configuration, AppConfig config = null)
        {
            var appConfigSection = configuration.GetSection(AppConfig.SECTION_NAME);

            if (config != null)
            {
                appConfigSection.Bind(config);
            }
            
            return services.Configure<AppConfig>(appConfigSection);
        }

        public static IServiceCollection ConfigureAdbConfig(this IServiceCollection services, IConfiguration configuration, AdbConfig config = null)
        {
            var appConfigSection = configuration.GetSection(AdbConfig.SECTION_NAME);

            if (config != null)
            {
                appConfigSection.Bind(config);
            }

            return services.Configure<AdbConfig>(appConfigSection);
        }

        public static IServiceCollection ConfigureWebSocketServerConfig(this IServiceCollection services, IConfiguration configuration, WebSocketServerConfig config = null)
        {
            var appConfigSection = configuration.GetSection(WebSocketServerConfig.SECTION_NAME);

            if (config != null)
            {
                appConfigSection.Bind(config);
            }

            return services.Configure<WebSocketServerConfig>(appConfigSection);
        }
    }
}
