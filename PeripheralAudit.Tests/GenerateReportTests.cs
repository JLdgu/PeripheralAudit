using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Moq;
using PeripheralAudit.Application;
using PeripheralAudit.Application.Entities;
using PeripheralAudit.Report;

namespace PeripheralAudit.Tests;

public class GenerateReportTests
{
    private Cost _costs = new
        (
            dock: 11,
            monitor: 13,
            largeMonitor: 17,
            keyboard: 19,
            mouse: 23,
            chair: 29
        );
        
    [Fact]
    public void UpgradeCosts_With_Empty_Desk_Returns_All_Upgrade_Costs_And_Counts()
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
        };

        GenerateReport report = new(context.Object, "report output", _costs);

        HtmlNode? actual = report.UpgradeCosts(location, _costs);
        if (actual is null)
            Assert.Fail("Unexpectd Null returned in actual");

        actual.Attributes["colspan"].Value.Should().Be("12");
        actual.Attributes["class"].Value.Should().Be("tal");

        HtmlNode bronze = actual.Descendants("bronze").First();
        HtmlNode dock = actual.Descendants("dock").First();
        HtmlNode monitor = actual.Descendants("monitor").First();
        HtmlNode keyboard = actual.Descendants("keyboard").First();
        HtmlNode mouse = actual.Descendants("mouse").First();
        HtmlNode silver = actual.Descendants("silver").First();
        HtmlNode gold = actual.Descendants("gold").First();

        bronze.InnerText.Should().Be($"Repopultion Costs &#163;53 - ");
        dock.InnerText.Should().Be($"1 dock(s) @ &#163;{_costs.Dock}, ");
        monitor.InnerText.Should().Be("1 monitor(s) @ &#163;0, ");
        keyboard.InnerText.Should().Be($"1 keyboard(s) @ &#163;{_costs.Keyboard}, ");
        mouse.InnerText.Should().Be($"1 mice @ &#163;{_costs.Mouse}");
        silver.InnerText.Should().Be($"Silver Upgrade Costs &#163;{_costs.Monitor} - 1 monitor(s) @ &#163;{_costs.Monitor}");
        gold.InnerText.Should().Be($"Gold Upgrade Costs &#163;{_costs.LargeMonitor} - 1 monitor(s) @ &#163;{_costs.LargeMonitor}");
    }

    [Fact]
    public void UpgradeCosts_With_Complete_Bronze_Desk_Returns_Silver_And_Gold_Upgrade_Cost_And_Count()
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
        if (actual is null)
            Assert.Fail("Unexpectd Null returned in actual");
        var silver = actual.Descendants("silver").First();
        var gold = actual.Descendants("gold").First();

        silver.InnerText.Should().Be($"Silver Upgrade Costs &#163;{_costs.Monitor} - 1 monitor(s) @ &#163;{_costs.Monitor}");
        gold.InnerText.Should().Be($"Gold Upgrade Costs &#163;{_costs.LargeMonitor} - 1 monitor(s) @ &#163;{_costs.LargeMonitor}");
    }

    [Fact]
    public void UpgradeCosts_With_Silver_Desk_Returns_Only_Gold_Upgrade_Cost_And_Count()
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
        if (actual is null)
            Assert.Fail("Unexpectd Null returned in actual");
        var silver = actual.Descendants("silver").FirstOrDefault();
        var gold = actual.Descendants("gold").First();

        silver.Should().BeNull();
        gold.InnerText.Should().Be($"Gold Upgrade Costs &#163;{_costs.LargeMonitor} - 1 monitor(s) @ &#163;{_costs.LargeMonitor}");
    }

    [Fact]
    public void UpgradeCosts_With_All_Gold_Desks_Returns_Null()
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

        actual.Should().BeNull();
    }
}