using System;
using Domain;
using Microsoft.EntityFrameworkCore;


namespace Persistence
{
  public class DataContext: DbContext
  {
    public DataContext(DbContextOptions options): base(options) { }

    public DbSet<Value> Values { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
      
      builder.Entity<Value>()
        .HasData(
          new Value { ID = 1, Name = "Value 101" },
          new Value { ID = 2, Name = "Value 102" },
          new Value { ID = 3, Name = "Value 103" }
        );
    }
  }
}
