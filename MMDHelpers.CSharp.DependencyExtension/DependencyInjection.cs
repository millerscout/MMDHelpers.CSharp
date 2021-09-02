using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;

namespace MMDHelpers.CSharp.DependencyInjection.Serilog
{

    public static class DependencyInjection
    {
        /// <summary>
        /// Service to send/receive commands to configured application.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IServiceCollection AddOrchestrationControl(this IServiceCollection service, config t)
        {
            service.AddSingleton<MyClass>();

            return service;
        }
    }
    public static class MMDSerilogOrchestration
    {
        public static LoggerConfiguration MMDSink(this LoggerSinkConfiguration sinkConfiguration)
        {
            return sinkConfiguration.Sink(new MMDSink());
        }

    }

    public class MMDSink : ILogEventSink
    {
        public MMDSink()
        {

        }

        public void Emit(LogEvent logEvent)
        {
            System.Console.WriteLine(logEvent.RenderMessage());
        }
    }
    public class MyClass

    {
        public MyClass(config s)
        {

        }
       
    }
    public class config
    {

    }

}
