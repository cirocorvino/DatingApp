using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.Data;

public class DatingAppDBContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) 
    {
        base.OnModelCreating(builder);

        #region Likes (UserLike) mapping relation
        builder.Entity<UserLike>()
            .HasKey(k => new { k.SourceUserId, k.TargetUserId });

        builder.Entity<UserLike>()
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLike>()
            .HasOne(s => s.TargetUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(s => s.TargetUserId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion UserLike mapping relation


        #region Messages mapping relation
        builder.Entity<Message>()
            .HasOne(m => m.Recipient)
            .WithMany(usr => usr.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(usr => usr.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);        
        #endregion Messages mapping relation

    }
}
