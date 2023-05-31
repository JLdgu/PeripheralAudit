using FluentAssertions;
using PeripheralAudit.Application;
using PeripheralAudit.Application.Entities;
using PeripheralAudit.Report;

namespace PeripheralAudit.Tests;

public class UpgradeTests
{
    Cost _costs = new
        (
            dock: 11,
            monitor: 13,
            largeMonitor: 17,
            keyboard: 19,
            mouse: 23,
            chair: 29
        );

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    public void Upgrade_With_Empty_Desk_Returns_All_Costs_And_Counts(int value)
    {
        Location location = new()
        {
            Name = "Empty Desk",
            DeskCount = value,
        };

        Upgrade actual = new(_costs, location);

        actual.RepopulationCost.Should().Be(value * (_costs.Dock + 0 + _costs.Keyboard + _costs.Mouse));
        actual.BronzeMonitorCount.Should().Be(value);
        actual.BronzeMonitorCost.Should().Be(0);
        actual.SilverMonitorCount.Should().Be(value);
        actual.SilverMonitorCost.Should().Be(value * _costs.Monitor);
        actual.GoldMonitorCount.Should().Be(value);
        actual.GoldMonitorCost.Should().Be(value * _costs.LargeMonitor);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    public void Upgrade_With_Silver_Desk_Returns_Gold_Costs_And_Counts(int value)
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

        actual.RepopulationCost.Should().Be(0);
        actual.BronzeMonitorCount.Should().Be(0);
        actual.BronzeMonitorCost.Should().Be(0);
        actual.SilverMonitorCount.Should().Be(0);
        actual.SilverMonitorCost.Should().Be(0);
        actual.GoldMonitorCount.Should().Be(value);
        actual.GoldMonitorCost.Should().Be(value * _costs.LargeMonitor);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    public void Upgrade_With_Gold_Desk_Should_Have_Zero_Costs_And_Counts(int value)
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

        actual.RepopulationCost.Should().Be(0);
        actual.BronzeMonitorCount.Should().Be(0);
        actual.BronzeMonitorCost.Should().Be(0);
        actual.SilverMonitorCount.Should().Be(0);
        actual.SilverMonitorCost.Should().Be(0);
        actual.GoldMonitorCount.Should().Be(0);
        actual.GoldMonitorCost.Should().Be(0);
    }

    [Fact]
    public void Upgrade_With_Dock_Should_Set_Zero_Dock_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Dock",
            DeskCount = 1,
            DockCount = 1
        };

        Upgrade actual = new(_costs, location);

        actual.DockCount.Should().Be(0);
        actual.DockCost.Should().Be(0);
    }

    [Fact]
    public void Upgrade_With_PC_Should_Set_Zero_Dock_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Dock",
            DeskCount = 1,
            PcCount = 1
        };

        Upgrade actual = new(_costs, location);

        actual.DockCount.Should().Be(0);
        actual.DockCost.Should().Be(0);
    }

    [Fact]
    public void Upgrade_With_Dock_And_PC_Should_Set_Zero_Dock_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Dock",
            DeskCount = 2,
            DockCount = 1,
            PcCount = 1
        };

        Upgrade actual = new(_costs, location);

        actual.DockCount.Should().Be(0);
        actual.DockCost.Should().Be(0);
    }

    [Fact]
    public void Upgrade_With_Missing_Dock_Should_Set_Dock_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Missing Dock",
            DeskCount = 1,
            DockCount = 0
        };

        Upgrade actual = new(_costs, location);

        actual.DockCount.Should().Be(1);
        actual.DockCost.Should().Be(_costs.Dock);
    }

    [Fact]
    public void Upgrade_With_Keyboard_Should_Set_Zero_Keyboard_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Keyboard",
            DeskCount = 1,
            KeyboardCount = 1
        };

        Upgrade actual = new(_costs, location);

        actual.KeyboardCount.Should().Be(0);
        actual.KeyboardCost.Should().Be(0);
    }

    [Fact]
    public void Upgrade_With_Missing_Keyboard_Should_Set_Keyboard_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Missing Keyboard",
            DeskCount = 1,
            MouseCount = 0
        };

        Upgrade actual = new(_costs, location);

        actual.KeyboardCount.Should().Be(1);
        actual.KeyboardCost.Should().Be(_costs.Keyboard);
    }

    [Fact]
    public void Upgrade_With_Mouse_Should_Set_Zero_Mouse_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Mouse",
            DeskCount = 1,
            MouseCount = 1
        };

        Upgrade actual = new(_costs, location);

        actual.MouseCount.Should().Be(0);
        actual.MouseCost.Should().Be(0);
    }

    [Fact]
    public void Upgrade_With_Missing_Mouse_Should_Set_Mouse_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Missing Mouse",
            DeskCount = 1,
            MouseCount = 0
        };

        Upgrade actual = new(_costs, location);

        actual.MouseCount.Should().Be(1);
        actual.MouseCost.Should().Be(_costs.Mouse);
    }
    [Fact]
    public void Upgrade_With_Null_Chair_Should_Set_Null_Chair_Count_And_Zero_Chair_Cost()
    {
        Location location = new()
        {
            Name = "Null ChairCount",
            DeskCount = 1,
            ChairCount = null
        };

        Upgrade actual = new(_costs, location);

        actual.ChairCount.Should().BeNull();
        actual.ChairCost.Should().Be(0);
    }

    [Fact]
    public void Upgrade_With_Chair_Should_Set_Chair_Count_And_Cost()
    {
        Location location = new()
        {
            Name = "Chair",
            DeskCount = 1,
            ChairCount = 0
        };

        Upgrade actual = new(_costs, location);

        actual.ChairCount.Should().Be(1);
        actual.ChairCost.Should().Be(_costs.Chair);
    }

    [Fact]
    public void Upgrade_With_Multiple_Items_Should_Round_Correctly()
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

        //actual.RepopulationCost.Should().Be(4655.2m);
        actual.RepopulationCost.Should().Be((location.DeskCount - location.DockCount) * costs.Dock 
            + (location.DeskCount - location.KeyboardCount) * costs.Keyboard
            + (location.DeskCount - location.MouseCount) * costs.Mouse
            + (location.DeskCount - location.ChairCount) * costs.Chair );
    }
}