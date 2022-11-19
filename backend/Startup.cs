using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using logcat_monitor.configuration;
using logcat_monitor.extenstions;
using logcat_monitor.services;
using logcat_monitor.services.internals;
using logcat_monitor.services.internals.events;

namespace logcat_monitor
{
    public class Startup
    {


        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            var wsConfig = new WebSocketServerConfig();
            var adbConfig = new AdbConfig();

            services.ConfigureAppConfig(Configuration);
            services.ConfigureAdbConfig(Configuration, adbConfig);
            services.ConfigureWebSocketServerConfig(Configuration, wsConfig);

            services.AddSingleton<IEventDispatch, EventDispatch>();

            if (adbConfig.Enabled)
            {
                services.AddHostedService<AdbService>();
            }

            if (wsConfig.Enabled)
            {
                services.AddHostedService<WebSocketService>();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }



    }
}

