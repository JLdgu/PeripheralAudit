using Microsoft.EntityFrameworkCore;
using PeripheralAudit.Application;

DbContextOptionsBuilder builder = new();
builder.UseSqlite("data source=peripheralaudit.db;");

PeripheralAuditDbContext dbContext = new(builder.Options);
//dbContext.Database.EnsureDeleted();
dbContext.Database.EnsureCreated();
//dbContext.Database.Migrate();
