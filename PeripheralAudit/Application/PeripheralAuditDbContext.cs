using Microsoft.EntityFrameworkCore;

using PeripheralAudit.Application.Entities;

namespace PeripheralAudit.Application;
public class PeripheralAuditDbContext : DbContext
{
    public DbSet<Site> Sites { get; set; }  = null!;

    public DbSet<Location> Locations { get; set; }  = null!;

    public DbSet<Location> Docks { get; set; }  = null!;

    public DbSet<Location> Monitors { get; set; }  = null!;
       
    public PeripheralAuditDbContext(DbContextOptions options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(@"data source=peripheralaudit.db;");
        }
        //optionsBuilder.UseLazyLoadingProxies(); //requires entityframeworkcore.proxies package
        optionsBuilder.EnableSensitiveDataLogging();
        //optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information).EnableSensitiveDataLogging();
        //optionsBuilder.LogTo(Console.WriteLine , new[] { DbLoggerCategory.Query.Name, DbLoggerCategory.Update.Name}).EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Site>().ToTable(nameof(Site));
        modelBuilder.Entity<Location>().ToTable(nameof(Location));
        modelBuilder.Entity<Dock>().ToTable(nameof(Dock));
        modelBuilder.Entity<Entities.Monitor>().ToTable(nameof(Entities.Monitor));

        base.OnModelCreating(modelBuilder);
    }
    
}
