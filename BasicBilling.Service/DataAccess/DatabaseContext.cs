using BasicBilling.Service.DataAccess.Extensions;
using Microsoft.EntityFrameworkCore;
namespace BasicBilling.Service.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Build(GetType().Assembly);

            base.OnModelCreating(builder);
        }
    }
}
