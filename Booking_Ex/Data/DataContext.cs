using ASP_.Net_Core_Class_Home_Work.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ASP_.Net_Core_Class_Home_Work.Data;

public class DataContext : DbContext
{
    public DbSet<Data.Entities.User> users { set; get; }
    public DbSet<Data.Entities.Category> categories { set; get; }
    public DbSet<Data.Entities.Location> locations { set; get; }
    public DbSet<Data.Entities.Room> rooms { set; get; }
    public DbSet<Data.Entities.Reservation> Reservations { set; get; }
    public DbSet<Data.Entities.Token> Token { get; set; }
     public DataContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasIndex(c => c.Slug).IsUnique();
        modelBuilder.Entity<Location>().HasIndex(l => l.Slug).IsUnique();
        modelBuilder.Entity<Room>().HasIndex(r => r.Slug).IsUnique();
        modelBuilder.Entity<Entities.Reservation>().HasOne(r => r.User).WithMany(u=>u.Reservations).HasForeignKey(r => r.UserId);
        modelBuilder.Entity<Entities.Reservation>().HasOne(r => r.Room).WithMany(r=>r.Reservations).HasForeignKey(r => r.RoomId);
        modelBuilder.Entity<Entities.Token>().HasOne(t => t.User).WithMany();
       
    }
    
}