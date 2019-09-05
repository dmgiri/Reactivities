using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Persistence
{
  public class DataContext: IdentityDbContext<AppUser>
  {
    public DataContext(DbContextOptions options): base(options) { }

    public DbSet<Value> Values { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<UserActivity> UserActivities { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<UserFollowing> Followings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
      
      base.OnModelCreating(builder);

      builder.Entity<Value>().HasData(
          new Value { ID = 1, Name = "Value 101" },
          new Value { ID = 2, Name = "Value 102" },
          new Value { ID = 3, Name = "Value 103" }
      );

      builder.Entity<UserActivity>(x => x.HasKey(ua => new {ua.AppUserID, ua.ActivityID}));

      builder.Entity<UserActivity>()
        .HasOne(u => u.AppUser)
        .WithMany(a => a.UserActivities)
        .HasForeignKey(u => u.AppUserID);
      
      builder.Entity<UserActivity>()
        .HasOne(a => a.Activity)
        .WithMany(u => u.UserActivities)
        .HasForeignKey(a => a.ActivityID);

      builder.Entity<UserFollowing>(b => 
      {
        b.HasKey(k => new {k.ObserverID, k.TargetID});

        b.HasOne(o => o.Observer)
          .WithMany(f => f.Followings)
          .HasForeignKey(o => o.ObserverID)
          .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(o => o.Target)
          .WithMany(f => f.Followers)
          .HasForeignKey(o => o.TargetID)
          .OnDelete(DeleteBehavior.Restrict);
      });
    }
  }
}
