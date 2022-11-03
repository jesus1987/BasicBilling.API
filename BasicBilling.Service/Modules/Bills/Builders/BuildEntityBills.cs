using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace BasicBilling.Service.Modules.Bills.Builders
{
    internal class BuildEntityBills : IEntityTypeConfiguration<BasicBilling.Service.Models.Bills>
    {
        public void Configure(EntityTypeBuilder<Models.Bills> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Status);
            builder.Property(x => x.Period);
            builder.Property(x => x.Description);
            builder.HasOne(x =>x.Client).WithMany(x=>x.Bills).HasForeignKey(x => x.ClientId);
            builder.ToTable("Bills");
        }
    }
}
