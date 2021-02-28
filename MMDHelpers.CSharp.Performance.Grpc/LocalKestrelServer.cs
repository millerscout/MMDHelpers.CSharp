using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMDHelpers.CSharp.Performance.grpc
{
    public class LocalKestrelServer
    {
        public void Run()
        {
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).Build().Run();
        }
        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddGrpc();
            }

            public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }

                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapGrpcService<LogService>();
                });
            }
        }
    }
}
