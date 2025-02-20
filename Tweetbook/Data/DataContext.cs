using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Tweetbook.Domain;

namespace Tweetbook.Data
{
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }

        public DbSet<PostTag> PostTags { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Post has many PostTags
            builder.Entity<Post>()
                .HasMany(e => e.Tags)
                .WithOne(e => e.Post)
                .HasForeignKey(e => e.PostId)
                .IsRequired(false);

            // PostTag has one Tag
            builder.Entity<Tag>()
                .HasOne(e => e.PostTag)
                .WithOne(e => e.Tag)
                .HasForeignKey<PostTag>(e => e.TagId)
                .IsRequired();
        }
    }
}
