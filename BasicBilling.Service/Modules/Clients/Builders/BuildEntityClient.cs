using BasicBilling.Service.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
namespace BasicBilling.Service.Modules.Clients.Builders
{
    internal class BuildEntityClient : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(e => e.Name)
                .IsRequired();

            builder.ToTable("Clients");
        }
    }
}
