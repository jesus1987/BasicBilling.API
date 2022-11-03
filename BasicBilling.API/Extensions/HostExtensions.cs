using BasicBilling.Service.DataAccess;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BasicBilling.API.Extensions
{
    public static class HostExtensions
    {
        public static async Task MigrateDatabase(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var contexts = new List<DbContext>
            {
                serviceProvider.GetRequiredService<DatabaseContext>()
            };

            foreach (var dbContext in contexts)
            {
                await dbContext.Database.MigrateAsync();
            }
        }
    }
}
