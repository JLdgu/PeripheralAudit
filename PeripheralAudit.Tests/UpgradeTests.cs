using PeripheralAudit.Application;
using PeripheralAudit.Application.Entities;
using PeripheralAudit.Report;

namespace PeripheralAudit.Tests;

internal sealed class UpgradeTests
{
    readonly Cost _costs = new
        (
            dock: 11,
            monitor: 13,
            largeMonitor: 17,
            keyboard: 19,
            mouse: 23,
            chair: 29
        );

    [Test]
    [Arguments(1)]
    [Arguments(3)]
    internal async Task Upgrade_With_Empty_Desk_Returns_All_Costs_And_Counts(int value)
    {
        Location location = new()
        {
            Name = "Empty Desk",
            DeskCount = value,
        };

        Upgrade actual = new(_costs, location);

        await Assert.That(actual.RepopulationCost).IsEqualTo(value * (_costs.Dock + 0 + _costs.Keyboard + _costs.Mouse));
        await Assert.That(actual.BronzeMonitorCount).IsEqualTo(value);
        await Assert.That(actual.BronzeMonitorCost).IsEqualTo(0);
        await Assert.That(actual.SilverMonitorCount).IsEqualTo(value);
        await Assert.That(actual.SilverMonitorCost).IsEqualTo(value * _costs.Monitor);
        await Assert.That(actual.GoldMonitorCount).IsEqualTo(value);
        await Assert.That(actual.GoldMonitorCost).IsEqualTo(value * _costs.LargeMonitor);
    }

    [Test]
    [Arguments(1)]
    [Arguments(3)]
    internal async Task Upgrade_With_Silver_Desk_Returns_Gold_Costs_And_Counts(int value)
    {

        Location location = new()
        {
            Name = "Silver Desk",
            DeskCount = value,
            MonitorGradeSilverCount = value,
            DockCount = value,
            KeyboardCount = value,
            MouseCount = value
        };

        Upgrade actual = new(_costs, location);

        await Assert.That(actual.RepopulationCost).IsEqualTo(0);
        await Assert.That(actual.BronzeMonitorCount).IsEqualTo(0);
        await Assert.That(actual.BronzeMonitorCost).IsEqualTo(0);
        await Assert.That(actual.SilverMonitorCount).IsEqualTo(0);
        await Assert.That(actual.SilverMonitorCost).IsEqualTo(0);
        await Assert.That(actual.GoldMonitorCount).IsEqualTo(value);
        await Assert.That(actual.GoldMonitorCost).IsEqualTo(value * _costs.LargeMonitor);
    }

    [Test]
    [Arguments(1)]
    [Arguments(3)]
    internal async Task Upgrade_With_Gold_Desk_Should_Have_Zero_Costs_And_Counts(int value)
    {
        Location location = new()
        {
            Name = "Gold Desk",
            DeskCount = value,
            MonitorGradeGoldCount = value,
            DockCount = value,
            KeyboardCount = value,
            MouseCount = value
        };

        Upgrade actual = new(_costs, location);

        await Assert.That(actual.RepopulationCost).IsEqualTo(0);
        await Assert.That(actual.BronzeMonitorCount).IsEqualTo(0);
        await Assert.That(actual.BronzeMonitorCost).IsEqualTo(0);
        await Assert.That(actual.SilverMonitorCount).IsEqualTo(0);
        await Assert.That(actual.SilverMonitorCost).IsEqualTo(0);
        await Assert.That(actual.GoldMonitorCount).IsEqualTo(0);
        await Assert.That(actual.GoldMonitorCost).IsEqualTo(0);
    }

    [Test]
    internal async Task Upgrade_With_Dock_Should_Set_Zero_Dock_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Dock",
            DeskCount = 1,
            DockCount = 1
        };

        Upgrade actual = new(_costs, location);

        await Assert.That(actual.DockCount).IsEqualTo(0);
        await Assert.That(actual.DockCost).IsEqualTo(0);
    }

    [Test]
    internal async Task Upgrade_With_PC_Should_Set_Zero_Dock_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Dock",
            DeskCount = 1,
            PcCount = 1
        };

        Upgrade actual = new(_costs, location);

        await Assert.That(actual.DockCount).IsEqualTo(0);
        await Assert.That(actual.DockCost).IsEqualTo(0);
    }

    [Test]
    internal async Task Upgrade_With_Dock_And_PC_Should_Set_Zero_Dock_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Dock",
            DeskCount = 2,
            DockCount = 1,
            PcCount = 1
        };

        Upgrade actual = new(_costs, location);

        await Assert.That(actual.DockCount).IsEqualTo(0);
        await Assert.That(actual.DockCost).IsEqualTo(0);
    }

    [Test]
    internal async Task Upgrade_With_Missing_Dock_Should_Set_Dock_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Missing Dock",
            DeskCount = 1,
            DockCount = 0
        };

        Upgrade actual = new(_costs, location);

        await Assert.That(actual.DockCount).IsEqualTo(1);
        await Assert.That(actual.DockCost).IsEqualTo(_costs.Dock);
    }

    [Test]
    internal async Task Upgrade_With_Keyboard_Should_Set_Zero_Keyboard_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Keyboard",
            DeskCount = 1,
            KeyboardCount = 1
        };

        Upgrade actual = new(_costs, location);

        await Assert.That(actual.KeyboardCount).IsEqualTo(0);
        await Assert.That(actual.KeyboardCost).IsEqualTo(0);
    }

    [Test]
    internal async Task Upgrade_With_Missing_Keyboard_Should_Set_Keyboard_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Missing Keyboard",
            DeskCount = 1,
            MouseCount = 0
        };

        Upgrade actual = new(_costs, location);

        await Assert.That(actual.KeyboardCount).IsEqualTo(1);
        await Assert.That(actual.KeyboardCost).IsEqualTo(_costs.Keyboard);
    }

    [Test]
    internal async Task Upgrade_With_Mouse_Should_Set_Zero_Mouse_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Mouse",
            DeskCount = 1,
            MouseCount = 1
        };

        Upgrade actual = new(_costs, location);

        await Assert.That(actual.MouseCount).IsEqualTo(0);
        await Assert.That(actual.MouseCost).IsEqualTo(0);
    }

    [Test]
    internal async Task Upgrade_With_Missing_Mouse_Should_Set_Mouse_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Missing Mouse",
            DeskCount = 1,
            MouseCount = 0
        };

        Upgrade actual = new(_costs, location);

        await Assert.That(actual.MouseCount).IsEqualTo(1);
        await Assert.That(actual.MouseCost).IsEqualTo(_costs.Mouse);
    }

    [Test]
    internal async Task Upgrade_With_Null_Chair_Should_Set_Null_Chair_Count_And_Zero_Chair_Cost()
    {
        Location location = new()
        {
            Name = "Null ChairCount",
            DeskCount = 1,
            ChairCount = null
        };

        Upgrade actual = new(_costs, location);

        await Assert.That(actual.ChairCount).IsNull();
        await Assert.That(actual.ChairCost).IsEqualTo(0);
    }

    [Test]
    internal async Task Upgrade_With_Chair_Should_Set_Chair_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Chair",
            DeskCount = 1,
            ChairCount = 0
        };

        Upgrade actual = new(_costs, location);

        await Assert.That(actual.ChairCount).IsEqualTo(1);
        await Assert.That(actual.ChairCost).IsEqualTo(_costs.Chair);
    }

    [Test]
    internal async Task Upgrade_With_Multiple_Items_Should_Round_Correctly()
    {
        Cost costs = new
        (
            dock: 225,
            monitor: 115,
            largeMonitor: 120,
            keyboard: 8.87m,
            mouse: 8.24m,
            chair: 209
        );

        Location location = new()
        {
            Name = "Multiple Desks",
            DeskCount = 107,
            DockCount = 89,
            KeyboardCount = 75,
            MouseCount = 68,
            ChairCount = 100
        };

        Upgrade actual = new(costs, location);

        await Assert.That(actual.RepopulationCost).IsEqualTo((location.DeskCount - location.DockCount) * costs.Dock 
            + (location.DeskCount - location.KeyboardCount) * costs.Keyboard
            + (location.DeskCount - location.MouseCount) * costs.Mouse
            + (location.DeskCount - location.ChairCount) * costs.Chair );
    }
}