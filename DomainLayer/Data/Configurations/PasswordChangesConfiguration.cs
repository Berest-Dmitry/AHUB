using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainLayer.Data.Configurations
{
    public class PasswordChangesConfiguration : IEntityTypeConfiguration<PasswordChanges>
    {
        public void Configure(EntityTypeBuilder<PasswordChanges> builder)
        {
            builder.ToTable("PasswordChanges");
            builder.HasKey(b => b.id);
            builder.Property(b => b.id).IsRequired();
            builder.Property(b => b.recoveryKey).IsRequired();
            builder.Property(b => b.recoveryToken).IsRequired();

            builder
                .HasOne(b => b.user)
                .WithMany(u => u.PasswordChanges)
                .HasForeignKey(b => b.userId);
        }
    }
}
