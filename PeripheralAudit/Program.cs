using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeripheralAudit.Application;
using PeripheralAudit.Report;

bool _dbScript = false;
string? _reportOutput = string.Empty;
string? _scriptOutput = string.Empty;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureDbServices()
    .ConfigureServices(ConfigureAppSettings)
    .Build();

PeripheralAuditDbContext dbContext = host.Services.GetRequiredService<PeripheralAuditDbContext>();
//dbContext.Database.EnsureDeleted();
dbContext.Database.EnsureCreated();
//dbContext.Database.Migrate();

if (args.Any())
{
    for (int i = 0; i < args.Length; i++)
    {
        if (args[i].ToLower() == "dbscript")
        {
            _dbScript = true;
            continue;
        }
    }
}

if (_dbScript)
{
    string sql = dbContext.Database.GenerateCreateScript();
    File.WriteAllText(Path.Combine(_reportOutput,"CreatePADb.sql"), sql);
    return;
}

GenerateReport report = new(dbContext, _reportOutput);
report.Execute();

void ConfigureAppSettings(HostBuilderContext context, IServiceCollection collection)
{
    var config = context.Configuration;
    _reportOutput = config["ReportSettings:Output"];
    _scriptOutput = config["ScriptSettings:Output"];
}