using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(b => b.id);
            builder.Property(b => b.id).IsRequired();

            builder.Property(b => b.userName).IsRequired().HasMaxLength(50);
            builder.Property(b => b.hashedPassword).IsRequired();

            builder.Property(b => b.firstName).IsRequired().HasMaxLength(50);
            builder.Property(b => b.lastName).IsRequired().HasMaxLength(50);
        }
    }
}
