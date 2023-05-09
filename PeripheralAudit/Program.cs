using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PeripheralAudit.Application;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureDbServices()
    .Build();

PeripheralAuditDbContext dbContext = host.Services.GetRequiredService<PeripheralAuditDbContext>();

dbContext.Database.EnsureDeleted();
dbContext.Database.EnsureCreated();
//dbContext.Database.Migrate();

string sql = dbContext.Database.GenerateCreateScript();
//File.WriteAllText("c:/dev/temp/CreatePADb.sql", sql);
File.WriteAllText("d:/temp/CreatePADb.sql", sql);

GenerateReport report = new();
report.Execute();