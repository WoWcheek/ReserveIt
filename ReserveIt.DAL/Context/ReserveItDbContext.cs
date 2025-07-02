using Microsoft.EntityFrameworkCore;
using ReserveIt.DAL.Models;

namespace ReserveIt.DAL.Context;

public class ReserveItDbContext : DbContext
{
    public ReserveItDbContext(DbContextOptions<ReserveItDbContext> opt) : base(opt)
    { }

    public DbSet<Room> Rooms { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId);
    }
}