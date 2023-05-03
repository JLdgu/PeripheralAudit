using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeripheralAudit.Application;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(ConfigureDatabase)
.Build();

PeripheralAuditDbContext dbContext = host.Services.GetRequiredService<PeripheralAuditDbContext>();

//dbContext.Database.EnsureDeleted();
dbContext.Database.EnsureCreated();
//dbContext.Database.Migrate();

void ConfigureDatabase(HostBuilderContext context, IServiceCollection services)
{
    string? connectionString = context.Configuration.GetConnectionString("Default");

    services.AddDbContext<PeripheralAuditDbContext>(
                options => options.UseSqlite(connectionString));
}