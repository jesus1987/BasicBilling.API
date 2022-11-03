using BasicBilling.Service.DataAccess;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BasicBilling.Service.Repositories;
using MediatR;
using System.Reflection;
using FluentValidation;

namespace BasicBilling.Service
{

    public static class BuildService
    {
        public static IServiceCollection UseBasicBillingService(this IServiceCollection services, 
            Action<BasicBillingServiceOption> value)
        {
            var option = new BasicBillingServiceOption();
            value(option);
            if (option.InMemoryDatabase)
            {
                var inMemoryDatabaseRoot = new InMemoryDatabaseRoot();
                services.AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<DatabaseContext>(
                    (serviceProvider, options) =>
                        options.UseInMemoryDatabase("BasicBilling.API", inMemoryDatabaseRoot)
                            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                            .UseInternalServiceProvider(serviceProvider)
                            .ConfigureWarnings(warnings =>
                                warnings.Default(WarningBehavior.Ignore)
                                    .Ignore(InMemoryEventId.TransactionIgnoredWarning)));
            }
            else
            {
                services.AddDbContext<DatabaseContext>(options =>
                    options.UseSqlite(option.ConnectionString));
            }

            services.AddFeatureManagement();

            //Initialize repositories
            services.TryAddScoped(typeof(IRepository<>), typeof(Repository<>));

            var assembly = Assembly.GetExecutingAssembly();
            services.AddMediatR(assembly);

            //Ensure all of the validation types are registered.
            var validatorTypes = assembly.GetTypes();
            var scanner = new AssemblyScanner(validatorTypes);
            scanner.ForEach(pair =>
            {
                services.Add(ServiceDescriptor.Transient(pair.InterfaceType, pair.ValidatorType));
            });

            return services;
        }
    }
}
