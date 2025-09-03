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
    public class PostFileConfiguration : IEntityTypeConfiguration<PostFile>
    {
        public void Configure(EntityTypeBuilder<PostFile> builder)
        {
            builder.HasKey(b => b.id);
            builder.Property(b => b.id).IsRequired();

            builder.HasOne(b => b.Post).WithMany(p => p.PostFiles).HasForeignKey(b => b.postId);
        }
    }
}
