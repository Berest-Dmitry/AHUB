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
    internal class FileDataConfiguration : IEntityTypeConfiguration<FileData>
    {
        public void Configure(EntityTypeBuilder<FileData> builder)
        {
            builder.ToTable("FileData");
            builder.HasKey(b => b.id);
            builder.Property(b => b.id).IsRequired();
            builder.Property(b => b.fileName).IsRequired();
            builder.Property(b => b.mediaType).IsRequired();
            builder.Property(b => b.bucketName).IsRequired();
        }
    }
}
