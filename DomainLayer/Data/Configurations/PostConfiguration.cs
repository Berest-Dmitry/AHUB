using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainLayer.Data.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");
            builder.HasKey(b => b.id);
            builder.Property(b => b.id).IsRequired();

            builder.Property(b => b.title).IsRequired();
            builder.Property(b => b.content).IsRequired();

            builder.HasOne(b => b.User).WithMany(u => u.Posts).HasForeignKey(b => b.userId);
        }
    }
}
