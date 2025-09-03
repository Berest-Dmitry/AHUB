using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainLayer.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comments");
            builder.HasKey(b => b.id);
            builder.Property(b => b.id).IsRequired();

            builder.Property(b => b.content).IsRequired().HasMaxLength(500);

            builder.HasOne(c => c.User).WithMany(u => u.Comments).HasForeignKey(c => c.userId);
            builder.HasOne(c => c.Post).WithMany(p => p.Comments).HasForeignKey(c => c.postId);
        }
    }
}
