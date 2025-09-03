using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainLayer.Data.Configurations
{
    public class UserRolesConfiguration : IEntityTypeConfiguration<UserRoles>
    {
        public void Configure(EntityTypeBuilder<UserRoles> builder)
        {
            builder.ToTable("UserRoles");
            builder.HasKey(b => b.id);
            builder.Property(b => b.id).IsRequired();

            builder.HasOne(ur => ur.User).WithMany(u => u.Roles).HasForeignKey(ur => ur.userId);

            builder.HasOne(ur => ur.Role).WithMany(r => r.Users).HasForeignKey(ur => ur.roleId);
        }
    }
}
