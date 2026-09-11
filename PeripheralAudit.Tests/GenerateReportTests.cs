using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Moq;
using PeripheralAudit.Application;
using PeripheralAudit.Application.Entities;
using PeripheralAudit.Report;

namespace PeripheralAudit.Tests;

internal sealed class GenerateReportTests
{
    private readonly Cost _costs = new
        (
            dock: 11,
            monitor: 13,
            largeMonitor: 17,
            keyboard: 19,
            mouse: 23,
            chair: 29
        );

    [Test]
    internal async Task UpgradeCosts_With_Empty_Desk_And_Null_Chair_Returns_Upgrade_Costs_And_Counts()
    {
        var options = new DbContextOptionsBuilder<PeripheralAuditDbContext>().UseSqlite("DataSource=:memory:;").Options;
        var context = new Mock<PeripheralAuditDbContext>(options);
        Location location = new()
        {
            Name = "Empty Desk",
            DeskCount = 1,
            DockCount = 0,
            MonitorSingleCount = 0,
            MonitorDualCount = 0,
            KeyboardCount = 0,
            MouseCount = 0,
            ChairCount = null
        };

        GenerateReport report = new(context.Object, "report output", _costs);

        HtmlNode? actual = report.UpgradeCosts(location, _costs);
        await Assert.That(actual).IsNotNull();

        await Assert.That(actual!.Attributes["colspan"]!.Value).IsEqualTo("12");
        await Assert.That(actual!.Attributes["class"]!.Value).IsEqualTo("tal");

        HtmlNode? bronze = actual!.Descendants("bronze").First();
        HtmlNode? dock = actual!.Descendants("dock").First();
        HtmlNode? monitor = actual!.Descendants("monitor").First();
        HtmlNode? keyboard = actual!.Descendants("keyboard").First();
        HtmlNode? mouse = actual!.Descendants("mouse").First();
        HtmlNode? chair = actual!.Descendants("chair").FirstOrDefault();
        HtmlNode? silver = actual!.Descendants("silver").First();
        HtmlNode? gold = actual!.Descendants("gold").First();

        await Assert.That(bronze.InnerText).IsEqualTo($"Repopultion Costs &#163;53 - ");
        await Assert.That(dock.InnerText).IsEqualTo($"1 dock @ &#163;{_costs.Dock}, ");
        await Assert.That(monitor.InnerText).IsEqualTo("1 monitor @ &#163;0, ");
        await Assert.That(keyboard.InnerText).IsEqualTo($"1 keyboard @ &#163;{_costs.Keyboard}, ");
        await Assert.That(mouse.InnerText).IsEqualTo($"1 mouse @ &#163;{_costs.Mouse}, ");
        await Assert.That(chair).IsNull();
        await Assert.That(silver.InnerText).IsEqualTo($"Additional costs for upgrading to silver grade monitors &#163;{_costs.Monitor} - 1 monitor @ &#163;{_costs.Monitor}");
        await Assert.That(gold.InnerText).IsEqualTo($"Additional costs for upgrading to gold grade monitors &#163;{_costs.LargeMonitor} - 1 monitor @ &#163;{_costs.LargeMonitor}");
    }

    [Test]
    [Arguments(1, "", "mouse")]
    [Arguments(2, "s", "mice")]
    internal async Task UpgradeCosts_With_Empty_Desk_Returns_All_Upgrade_Costs_And_Counts(int count, string plural, string mousePlural)
    {
        var options = new DbContextOptionsBuilder<PeripheralAuditDbContext>().UseSqlite("DataSource=:memory:;").Options;
        var context = new Mock<PeripheralAuditDbContext>(options);
        Location location = new()
        {
            Name = "Empty Desk",
            DeskCount = count,
            DockCount = 0,
            MonitorSingleCount = 0,
            MonitorDualCount = 0,
            KeyboardCount = 0,
            MouseCount = 0,
            ChairCount = 0
        };

        GenerateReport report = new(context.Object, "report output", _costs);

        HtmlNode? actual = report.UpgradeCosts(location, _costs);
        await Assert.That(actual).IsNotNull();

        await Assert.That(actual!.Attributes["colspan"]!.Value).IsEqualTo("12");
        await Assert.That(actual!.Attributes["class"]!.Value).IsEqualTo("tal");

        HtmlNode? bronze = actual!.Descendants("bronze").First();
        HtmlNode? dock = actual!.Descendants("dock").First();
        HtmlNode? monitor = actual!.Descendants("monitor").First();
        HtmlNode? keyboard = actual!.Descendants("keyboard").First();
        HtmlNode? mouse = actual!.Descendants("mouse").First();
        HtmlNode? chair = actual!.Descendants("chair").First();
        HtmlNode? silver = actual!.Descendants("silver").First();
        HtmlNode? gold = actual!.Descendants("gold").First();

        await Assert.That(bronze.InnerText).IsEqualTo($"Repopultion Costs &#163;{82 * count} - ");
        await Assert.That(dock.InnerText).IsEqualTo($"{count} dock{plural} @ &#163;{_costs.Dock}, ");
        await Assert.That(monitor.InnerText).IsEqualTo($"{count} monitor{plural} @ &#163;0, ");
        await Assert.That(keyboard.InnerText).IsEqualTo($"{count} keyboard{plural} @ &#163;{_costs.Keyboard}, ");
        await Assert.That(mouse.InnerText).IsEqualTo($"{count} {mousePlural} @ &#163;{_costs.Mouse}, ");
        await Assert.That(chair.InnerText).IsEqualTo($"{count} chair{plural} @ &#163;{_costs.Chair}");
        await Assert.That(silver.InnerText).IsEqualTo($"Additional costs for upgrading to silver grade monitors &#163;{_costs.Monitor * count} - {count} monitor{plural} @ &#163;{_costs.Monitor}");
        await Assert.That(gold.InnerText).IsEqualTo($"Additional costs for upgrading to gold grade monitors &#163;{_costs.LargeMonitor * count} - {count} monitor{plural} @ &#163;{_costs.LargeMonitor}");
    }

    [Test]
    internal async Task UpgradeCosts_With_Complete_Bronze_Desk_Returns_Silver_And_Gold_Upgrade_Cost_And_Count()
    {
        var options = new DbContextOptionsBuilder<PeripheralAuditDbContext>().UseSqlite("DataSource=:memory:;").Options;
        var context = new Mock<PeripheralAuditDbContext>(options);
        Location location = new()
        {
            Name = "Bronze Desk",
            DeskCount = 1,
            DockCount = 1,
            MonitorGradeBronzeCount = 1,
            KeyboardCount = 1,
            MouseCount = 1
        };

        GenerateReport report = new(context.Object, "report output", _costs);

        HtmlNode? actual = report.UpgradeCosts(location, _costs);
        await Assert.That(actual).IsNotNull();

        var silver = actual.Descendants("silver").First();
        var gold = actual.Descendants("gold").First();

        await Assert.That(silver.InnerText).IsEqualTo($"Additional costs for upgrading to silver grade monitors &#163;{_costs.Monitor} - 1 monitor @ &#163;{_costs.Monitor}");
        await Assert.That(gold.InnerText).IsEqualTo($"Additional costs for upgrading to gold grade monitors &#163;{_costs.LargeMonitor} - 1 monitor @ &#163;{_costs.LargeMonitor}");
    }

    [Test]
    internal async Task UpgradeCosts_With_Silver_Desk_Returns_Only_Gold_Upgrade_Cost_And_Count()
    {
        var options = new DbContextOptionsBuilder<PeripheralAuditDbContext>().UseSqlite("DataSource=:memory:;").Options;
        var context = new Mock<PeripheralAuditDbContext>(options);
        Location location = new()
        {
            Name = "Silver Desk",
            DeskCount = 1,
            MonitorGradeSilverCount = 1
        };

        GenerateReport report = new(context.Object, "report output", _costs);

        HtmlNode? actual = report.UpgradeCosts(location, _costs);
        await Assert.That(actual).IsNotNull();

        var silver = actual.Descendants("silver").FirstOrDefault();
        var gold = actual.Descendants("gold").First();

        await Assert.That(silver).IsNull();
        await Assert.That(gold.InnerText).IsEqualTo($"Additional costs for upgrading to gold grade monitors &#163;{_costs.LargeMonitor} - 1 monitor @ &#163;{_costs.LargeMonitor}");
    }

    [Test]
    internal async Task UpgradeCosts_With_All_Gold_Desks_Returns_Null()
    {
        var options = new DbContextOptionsBuilder<PeripheralAuditDbContext>().UseSqlite("DataSource=:memory:;").Options;
        var context = new Mock<PeripheralAuditDbContext>(options);
        Location location = new()
        {
            Name = "3 Gold Desks",
            DeskCount = 3,
            MonitorGradeGoldCount = 3,
            DockCount = 3,
            KeyboardCount = 3,
            MouseCount = 3
        };

        GenerateReport report = new(context.Object, "report output", _costs);

        HtmlNode? actual = report.UpgradeCosts(location, _costs);
        await Assert.That(actual).IsNull();
    }
}