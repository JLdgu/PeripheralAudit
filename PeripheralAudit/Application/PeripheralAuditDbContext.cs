using Microsoft.EntityFrameworkCore;

using PeripheralAudit.Application.Entities;

namespace PeripheralAudit.Application;
public sealed class PeripheralAuditDbContext : DbContext
{
    public DbSet<Site> Sites { get; set; }

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


}
