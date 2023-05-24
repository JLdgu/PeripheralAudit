using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using PeripheralAudit.Application;
using PeripheralAudit.Application.Entities;
using PeripheralAudit.Report;

namespace PeripheralAudit.Tests;

public class UpgradeTests
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
    public void Upgrade_With_Empty_Desk_Returns_All_Costs_And_Counts()
    {
        Location location = new()
        {
            Name = "Empty Desk",
            DeskCount = 1,
        };

        Upgrade actual = new(_costs, location);

        actual.RepopulationCost.Should().Be(_costs.Dock + 0 + _costs.Keyboard + _costs.Mouse);
        actual.BronzeMonitorCount.Should().Be(1);
        actual.BronzeMonitorCost.Should().Be(0);
        actual.SilverMonitorCount.Should().Be(1);
        actual.SilverMonitorCost.Should().Be(_costs.Monitor);
        actual.GoldMonitorCount.Should().Be(1);
        actual.GoldMonitorCost.Should().Be(_costs.LargeMonitor);
    }

    [Fact]
    public void Upgrade_With_Silver_Desk_Returns_Gold_Costs_And_Counts()
    {

        Location location = new()
        {
            Name = "Silver Desk",
            DeskCount = 1,
            MonitorGradeSilverCount = 1,
            DockCount = 1,
            KeyboardCount = 1,
            MouseCount = 1
        };

        Upgrade actual = new(_costs, location);

        actual.RepopulationCost.Should().Be(0);
        actual.BronzeMonitorCount.Should().Be(0);
        actual.BronzeMonitorCost.Should().Be(0);
        actual.SilverMonitorCount.Should().Be(0);
        actual.SilverMonitorCost.Should().Be(0);
        actual.GoldMonitorCount.Should().Be(1);
        actual.GoldMonitorCost.Should().Be(_costs.LargeMonitor);
    }

    [Fact]
    public void Upgrade_With_Gold_Desk_Should_Have_Zero_Costs_And_Counts()
    {
        Location location = new()
        {
            Name = "Gold Desk",
            DeskCount = 1,
            MonitorGradeGoldCount = 1,
            DockCount = 1,
            KeyboardCount = 1,
            MouseCount = 1
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
}