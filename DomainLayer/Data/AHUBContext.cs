using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DomainLayer.Data
{
    /// <summary>
    /// контекст базы данных
    /// </summary>
    public class AHUBContext : DbContext
    {
        public AHUBContext(DbContextOptions<AHUBContext> options)
            : base(options)
        {
            Database.Migrate();
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PasswordChanges> PasswordChanges { get; set; }

        public DbSet<Post> Posts { get; set; }
        public DbSet<PostFile> PostFiles { get; set; }
        public DbSet<FileData> Files { get; set; }

        public DbSet<HashTag> HashTags { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Appeal> Appeals { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRoles> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AHUBContext).Assembly);
        }
    }
}
