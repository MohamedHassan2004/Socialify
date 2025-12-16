using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Socialify.Domain.Entities;
using System.Runtime.InteropServices;

namespace Socialify.Infrastructure.Data.Context
{
    public class SocialifyDbContext : IdentityDbContext<ApplicationUser>
    {
        public SocialifyDbContext(DbContextOptions<SocialifyDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<SavedPost> SavedPosts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<SharedPost> SharedPosts { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.FullName)
                .HasComputedColumnSql("[FirstName] + ' ' + [LastName]", stored: true);

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.FullName)
                .HasDatabaseName("IX_Users_FullName");

            // Add indexes for better query performance
            modelBuilder.Entity<Post>()
                .HasIndex(p => p.UserId)
                .HasDatabaseName("IX_Posts_UserId");

            modelBuilder.Entity<Post>()
                .HasIndex(p => p.CreatedAt)
                .HasDatabaseName("IX_Posts_CreatedAt");

            modelBuilder.Entity<Like>()
                .HasIndex(l => new { l.PostId, l.UserId })
                .HasDatabaseName("IX_Likes_PostId_UserId")
                .IsUnique();

            modelBuilder.Entity<SavedPost>()
                .HasIndex(p => p.SavedAt)
                .HasDatabaseName("IX_SavedPosts_SavedAt");

            modelBuilder.Entity<SavedPost>()
                .HasIndex(sp => new { sp.PostId, sp.UserId })
                .HasDatabaseName("IX_SavedPosts_PostId_UserId")
                .IsUnique();

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SavedPost>()
                .HasOne(sp => sp.Post)
                .WithMany(p => p.SavedPosts)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.User)
                .WithMany(u => u.Likes)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SavedPost>()
                .HasOne(sp => sp.User)
                .WithMany(u => u.SavedPosts)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .OnDelete(DeleteBehavior.Restrict);

            // ----------- SharedPost ----------- //
            modelBuilder.Entity<SharedPost>(entity =>
            {
                entity.HasKey(sp => sp.Id);

                entity.HasOne(sp => sp.OriginalPost)
                    .WithMany(p => p.SharedPosts)
                    .HasForeignKey(sp => sp.OriginalPostId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(sp => sp.Post)
                    .WithMany()
                    .HasForeignKey(sp => sp.SharedPostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(sp => sp.SharedByUser)
                    .WithMany()
                    .HasForeignKey(sp => sp.SharedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(sp => new { sp.OriginalPostId, sp.SharedByUserId })
                    .IsUnique();
            });


            // Post self-referencing relationship for shared posts
            modelBuilder.Entity<Post>()
                .HasOne(p => p.OriginalPost)
                .WithMany()
                .HasForeignKey(p => p.OriginalPostId)
                .OnDelete(DeleteBehavior.Restrict);


            // FriendRequest configuration
            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Sender)
                .WithMany(u => u.SentFriendRequests)
                .HasForeignKey(fr => fr.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Receiver)
                .WithMany(u => u.ReceivedFriendRequests)
                .HasForeignKey(fr => fr.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Friend)
                .WithMany(f => f.Friendships)
                .HasForeignKey(f => f.FriendId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Friendship>(entity =>
            {
                entity.HasIndex(f => new { f.UserId, f.FriendId })
                    .HasDatabaseName("IX_Friendships_UserId_FriendId");
            });

            // Notification configuration
            modelBuilder.Entity<Notification>()
                .HasIndex(n => n.ReceiverUserId)
                .HasDatabaseName("IX_Notifications_ReceiverUserId");

            modelBuilder.Entity<Notification>()
                .HasIndex(n => n.PostId)
                .HasDatabaseName("IX_Notifications_PostId");

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.ReceiverUser)
                .WithMany()
                .HasForeignKey(n => n.ReceiverUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Post)
                .WithMany(p => p.Notifications)
                .HasForeignKey(n => n.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
