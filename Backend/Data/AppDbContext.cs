using Backend.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<GameHistory> GameHistory { get; set; }
    public DbSet<UserGames> UserGames { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UserGames>()
            .HasKey(ug => new { ug.UserId, ug.GameId });

        base.OnModelCreating(builder);
    }
}
