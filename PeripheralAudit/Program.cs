using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeripheralAudit.Application;
using PeripheralAudit.Report;
using Velopack;

VelopackApp.Build().Run();

var mgr = new UpdateManager("https://github.com/JLdgu/PeripheralAudit");

var newVersion = await mgr.CheckForUpdatesAsync();
if (newVersion is not null)
{
    await mgr.DownloadUpdatesAsync(newVersion);
    mgr.ApplyUpdatesAndRestart(newVersion);
    return 0;
}

string? _reportOutput = string.Empty;
string? _scriptOutput = string.Empty;
Cost _costs = new();

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureDbServices()
    .ConfigureServices(ConfigureAppSettings)
    .Build();

PeripheralAuditDbContext dbContext = host.Services.GetRequiredService<PeripheralAuditDbContext>();
//dbContext.Database.EnsureDeleted();
dbContext.Database.EnsureCreated();
//dbContext.Database.Migrate();

RootCommand rootCommand = new("PeripheralAudit report generation");
Option<string> siteOption = new("--site", "-s")
{
    Description = "Site Filter or ALL",
    DefaultValueFactory = _ => "ALL"
};
rootCommand.Add(siteOption);

Option<string> locationOption = new("--location", "-l")
{
    Description = "Location Filter or ALL",
    DefaultValueFactory = _ => "ALL"
};
rootCommand.Add(locationOption);

rootCommand.SetAction( parseResult =>
{
    var site = parseResult.GetValue(siteOption);
    var location = parseResult.GetValue(locationOption);
    GenerateReport report = new(dbContext, _reportOutput, _costs);
    report.Execute(site!, location!);
});

Command scriptCommand = new("dbscript","Generate Database Script");
scriptCommand.SetAction( _ => 
{    
    GenerateDBScript(_scriptOutput);
});
rootCommand.Add(scriptCommand);

return rootCommand.Parse(args).Invoke();

void ConfigureAppSettings(HostBuilderContext context, IServiceCollection collection)
{
    var config = context.Configuration;
    _reportOutput = config["ReportSettings:Output"];
    _scriptOutput = config["ScriptSettings:Output"];

    string? property = config["PeripheralCosts:Dock"];
    if (property is null)
        throw new ArgumentNullException("PeripheralCosts:Dock not found in appsettings.json");
    _costs.Dock = decimal.Parse(property);

    property = config["PeripheralCosts:Monitor"];
    if (property is null)
        throw new ArgumentNullException("PeripheralCosts:Monitor not found in appsettings.json");
    _costs.Monitor = decimal.Parse(property);

    property = config["PeripheralCosts:LargeMonitor"];
    if (property is null)
        throw new ArgumentNullException("PeripheralCosts:LargeMonitor not found in appsettings.json");
    _costs.LargeMonitor = decimal.Parse(property);

    property = config["PeripheralCosts:Keyboard"];
    if (property is null)
        throw new ArgumentNullException("PeripheralCosts:Keyboard not found in appsettings.json");
    _costs.Keyboard = decimal.Parse(property);

    property = config["PeripheralCosts:Mouse"];
    if (property is null)
        throw new ArgumentNullException("PeripheralCosts:Mouse not found in appsettings.json");
    _costs.Mouse = decimal.Parse(property);

    property = config["PeripheralCosts:Chair"];
    if (property is null)
        throw new ArgumentNullException("PeripheralCosts:Chair not found in appsettings.json");
    _costs.Chair = decimal.Parse(property);
}

void GenerateDBScript(string _scriptOutput)
{
    string sql = dbContext.Database.GenerateCreateScript();
    File.WriteAllText(Path.Combine(_scriptOutput, "CreatePADb.sql"), sql);
}