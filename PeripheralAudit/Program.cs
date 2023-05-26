using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeripheralAudit.Application;
using PeripheralAudit.Report;

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

var rootCommand = new RootCommand("PeripheralAudit report generation");

var scriptCommand = new Command("dbscript","Generate Database Script");
scriptCommand.SetHandler( () => 
{    
    GenerateDBScript(_scriptOutput);
});
rootCommand.AddCommand(scriptCommand);

var reportCommand = new Command("report", "Generate reports for location(s)");
var locationOption = new Option<string>(
     name: "--location",      
     description: "Location Filter or ALL",
     getDefaultValue: () => "ALL");
locationOption.AddAlias("-l");
reportCommand.AddOption(locationOption);
reportCommand.SetHandler( (location) =>
{
    GenerateReport report = new(dbContext, _reportOutput, _costs);
    report.Execute(location);
},
locationOption);
rootCommand.AddCommand(reportCommand);

return rootCommand.Invoke(args);

void ConfigureAppSettings(HostBuilderContext context, IServiceCollection collection)
{
    var config = context.Configuration;
    _reportOutput = config["ReportSettings:Output"];
    _scriptOutput = config["ScriptSettings:Output"];

    string? property = config["PeripheralCosts:Dock"];
    if (property is null)
        throw new ArgumentNullException("PeripheralCosts:Dock not found in appsettings.json");
    _costs.Dock = float.Parse(property);

    property = config["PeripheralCosts:Monitor"];
    if (property is null)
        throw new ArgumentNullException("PeripheralCosts:Monitor not found in appsettings.json");
    _costs.Monitor = float.Parse(property);

    property = config["PeripheralCosts:LargeMonitor"];
    if (property is null)
        throw new ArgumentNullException("PeripheralCosts:LargeMonitor not found in appsettings.json");
    _costs.LargeMonitor = float.Parse(property);

    property = config["PeripheralCosts:Keyboard"];
    if (property is null)
        throw new ArgumentNullException("PeripheralCosts:Keyboard not found in appsettings.json");
    _costs.Keyboard = float.Parse(property);

    property = config["PeripheralCosts:Mouse"];
    if (property is null)
        throw new ArgumentNullException("PeripheralCosts:Mouse not found in appsettings.json");
    _costs.Mouse = float.Parse(property);

    property = config["PeripheralCosts:Chair"];
    if (property is null)
        throw new ArgumentNullException("PeripheralCosts:Chair not found in appsettings.json");
    _costs.Chair = float.Parse(property);
}

void GenerateDBScript(string _scriptOutput)
{
    string sql = dbContext.Database.GenerateCreateScript();
    File.WriteAllText(Path.Combine(_scriptOutput, "CreatePADb.sql"), sql);
}