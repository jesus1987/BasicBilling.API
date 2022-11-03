
using BasicBilling.API.Controllers;
using BasicBilling.Service.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BasicBilling.Service.Test
{
    public class BaseTest
    { 
        protected IServiceProvider ServiceProvider;

        [OneTimeSetUp]
        public void BaseSetup()
        {
            ServiceProvider = CreateServiceProvider();
        }

        private  IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();

            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            services.UseBasicBillingService(option =>
            {
                option.UseDatabaseConnectionString(configuration.GetConnectionString("DefaultConnection"));

                option.UseInMemoryDatabase(true);
            });


            var controllerTypes = Assembly.GetAssembly(typeof(Base))?.GetTypes().Where(type => typeof(Base).IsAssignableFrom(type)).ToList();

            foreach (var controllerType in (controllerTypes ?? new List<Type>()).Where(controllerType => !controllerType.IsAbstract))
                services.AddScoped(controllerType);

            

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        protected virtual DatabaseContext ReplaceObject() 
        {
            return null;
        }
    }
}