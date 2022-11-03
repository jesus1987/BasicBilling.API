using BasicBilling.API.Extensions;
using BasicBilling.API.Filter;
using BasicBilling.API.Middleware;
using BasicBilling.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Text.Json.Serialization;

namespace BasicBilling.API
{
    public class Startup
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        public Startup(Microsoft.Extensions.Configuration.IConfiguration configuration, IWebHostEnvironment env)
        {
            var userName = Environment.GetEnvironmentVariable("USERNAME");
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddJsonFile($"appsettings.{userName}.json", true)
                .AddEnvironmentVariables();
            this.configuration = builder.Build();

            this.environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opt =>
            {
                opt.Filters.Add(typeof(ExceptionHandler));
                opt.Filters.Add(typeof(ValidateModelStateAttribute));
            })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddHttpContextAccessor();

            services.UseBasicBillingService(option =>
            {
                option.UseDatabaseConnectionString(configuration.GetConnectionString("DefaultConnection"));

                if (environment.IsMockServer())
                    option.UseInMemoryDatabase(true);
            });

            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<HttpLoggingMiddleware>();
            app.UseSerilogRequestLogging(opts =>
            {
                opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
                opts.MessageTemplate = "HTTP {RequestMethod} {Path} responded {StatusCode} in {Elapsed:0.00} ms";
            });

            //if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
