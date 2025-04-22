using Microsoft.EntityFrameworkCore;
using Frontend.Models.Entities;

namespace Frontend.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions) { }
    public DbSet<UserAccount> UserAccounts { get; set; }
}
