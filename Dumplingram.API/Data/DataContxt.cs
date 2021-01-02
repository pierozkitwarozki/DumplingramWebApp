using Dumplingram.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Dumplingram.API.Data
{
    public class DataContxt : DbContext
    {
        public DataContxt(DbContextOptions<DataContxt> options) : base(options) {}
        public DbSet<User> Users { get; set; }   
        public DbSet<Follow> Follow { get; set; }
        public DbSet<Photo> Photo {get; set;}
        public DbSet<PhotoLike> PhotoLikes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<PhotoComment> PhotoComment { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Group> Groups { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) 
        {
            builder.Entity<Follow>()
                .HasKey(k => new { k.FollowerId, k.FolloweeId });

            builder.Entity<Follow>()
                .HasOne(u => u.Followee)
                .WithMany(u => u.Followers)
                .HasForeignKey(u => u.FolloweeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Follow>()
                .HasOne(u => u.Follower)
                .WithMany(u => u.Followees)
                .HasForeignKey(u => u.FollowerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PhotoLike>()
                .HasKey(k => new {k.UserId, k.PhotoId});

            builder.Entity<PhotoLike>()
                .HasOne(u => u.Liker)
                .WithMany(p => p.SendLikes)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PhotoLike>()
                .HasOne(p => p.Photo)
                .WithMany(u => u.GottenLikes)
                .HasForeignKey(p => p.PhotoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PhotoComment>()
                .HasOne(u => u.Commenter)
                .WithMany(p => p.Comments)
                .HasForeignKey(u => u.CommenterId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PhotoComment>()
                .HasOne(p => p.Photo)
                .WithMany(u => u.Comments)
                .HasForeignKey(p => p.PhotoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(s => s.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(r => r.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}