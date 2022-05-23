using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //fluent api
           base.OnModelCreating(builder); // if not errors in migrations

           builder.Entity<UserLike>().HasKey(k=> new {k.SourceUserId , k.LikedUserId});

           builder.Entity<UserLike>().HasOne(s => s.SourceUser).WithMany(k=> k.LikedUsers)
           .HasForeignKey(s=>s.SourceUserId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>().HasOne(s => s.LikedUser).WithMany(k=> k.LikedByUsers)
           .HasForeignKey(s=>s.LikedUserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}