using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;
using Serilog.Filters;
using BasicBilling.API.Extensions;
using Destructurama;
using BasicBilling.API.SerilogEnrichers;

namespace BasicBilling.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var host = CreateWebHostBuilder(args)
                    .Build();

                Log.Information("Application is starting up");

                await host.MigrateDatabase();

                await host.RunAsync();
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "The application failed to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
               .UseSerilog()
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
                   webBuilder.UseSerilog((context, configureLogger) =>
                   {
                       configureLogger.ReadFrom.Configuration(context.Configuration)
                           .Filter.ByExcluding(
                               Matching.WithProperty<string>("RequestPath", v =>
                               {
                                   return v.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) ||
                                          v.StartsWith("/api/activitylog", StringComparison.OrdinalIgnoreCase);
                               })
                           )
                           .Destructure.UsingAttributes()
                           .Destructure.JsonNetTypes()
                           .Enrich.FromLogContext()
                           .Enrich.With<RemovePropertiesEnricher>()
                           ;
                   });
               });
        }
    }
}
