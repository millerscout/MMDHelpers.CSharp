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

            var teste = MMDHelpers.CSharp.LocalDB.SQLLocalDB.CreateInstance("LogLarvaErrors", "teste",
                $@"create table controller (id integer identity(1,1),last integer,name varchar(500));
create table ExceptionDetails (Id integer identity(1,1), IdLog int, File int, Type int, Method int, Line int, StackOrder int);");


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
