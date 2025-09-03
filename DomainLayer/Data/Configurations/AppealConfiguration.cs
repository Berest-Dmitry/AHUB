using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainLayer.Data.Configurations
{
    public class AppealConfiguration : IEntityTypeConfiguration<Appeal>
    {
        public void Configure(EntityTypeBuilder<Appeal> builder)
        {
            builder.ToTable("Appeals");
            builder.HasKey(b => b.id);
            builder.Property(b => b.id).IsRequired();

            builder.Property(b => b.reason).IsRequired();
            builder.Property(b => b.appealEntityId).IsRequired();

            builder.HasOne(a => a.User).WithMany(u => u.Appeals).HasForeignKey(a => a.userId);
        }
    }
}
