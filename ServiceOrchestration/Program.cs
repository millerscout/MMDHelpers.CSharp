using Microsoft.Extensions.DependencyInjection;
using MMDHelpers.CSharp.DependencyInjection;
using MMDHelpers.CSharp.DependencyInjection.Serilog;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using System;

namespace ServiceOrchestration
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Information()
           .WriteTo.MMDSink()
           .CreateLogger();


            Log.Information("Processed {@Position} in {Elapsed} ms.", "23132", 123);

            Log.CloseAndFlush();

            IServiceCollection services = new ServiceCollection();
            services.AddOrchestrationControl(new config());
        }

    }
   
}
