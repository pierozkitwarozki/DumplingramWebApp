using Dumplingram.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dumplingram.API.Data
{
    public class DataContxt : DbContext
    {
        public DataContxt(DbContextOptions<DataContxt> options) : base(options) {}
        public DbSet<User> Users { get; set; }   
        public DbSet<Follow> Follow { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) 
        {
            builder.Entity<Follow>()
                .HasKey(k => new { k.FollowerId, k.FolloweeId });

            builder.Entity<Follow>()
                .HasOne(u => u.Followee)
                .WithMany(u => u.Followers)
                .HasForeignKey(u => u.FolloweeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Follow>()
                .HasOne(u => u.Follower)
                .WithMany(u => u.Followees)
                .HasForeignKey(u => u.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}