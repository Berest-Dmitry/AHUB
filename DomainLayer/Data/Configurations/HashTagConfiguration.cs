using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainLayer.Data.Configurations
{
    public class HashTagConfiguration : IEntityTypeConfiguration<HashTag>
    {
        public void Configure(EntityTypeBuilder<HashTag> builder)
        {
            builder.ToTable("HashTags");
            builder.HasKey(b => b.id);
            builder.Property(b => b.id).IsRequired();
            builder.Property(b => b.content).IsRequired();
        }
    }
}
