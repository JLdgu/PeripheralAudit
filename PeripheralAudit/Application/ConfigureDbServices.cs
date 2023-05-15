using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PeripheralAudit.Application;

public static class ConfigureDbServicesExtensions
{
    public static IHostBuilder ConfigureDbServices(this IHostBuilder host)
    {
        host.ConfigureServices((context, services) =>
        {
            string? connectionString = context.Configuration.GetConnectionString("Default");

            services.AddDbContext<PeripheralAuditDbContext>(
                        options => options.UseSqlite(connectionString));
        });

        return host;
    }
}