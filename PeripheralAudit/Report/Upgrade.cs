using PeripheralAudit.Application;
using PeripheralAudit.Application.Entities;

namespace PeripheralAudit.Report;

public sealed class Upgrade
{
    private Cost _cost;
    private Location _location;

    public Upgrade(Cost cost, Location location)
    {
        _cost = cost;
        _location = location;
    }

    public float RepopulationCost { get => BronzeMonitorCost + DockCost + KeyboardCost + MouseCost; }

    public int DeskCount { get => _location.DeskCount; }

    public int BronzeMonitorCount 
    { 
        get => DeskCount - _location.MonitorGradeBronzeCount - _location.MonitorGradeSilverCount - _location.MonitorGradeGoldCount; 
    }

    public float BronzeMonitorCost { get => 0; }

    public int SilverMonitorCount 
    { 
        get => DeskCount - _location.MonitorGradeSilverCount - _location.MonitorGradeGoldCount; 
    }

    public float SilverMonitorCost { get => SilverMonitorCount * _cost.Monitor; }

    public int GoldMonitorCount { get => DeskCount - _location.MonitorGradeGoldCount; }

    public float GoldMonitorCost { get => GoldMonitorCount * _cost.LargeMonitor; }

    public int DockCount { get => DeskCount - _location.DockCount - _location.PcCount; }

    public float DockCost { get => DockCount * _cost.Dock; }

    public int KeyboardCount { get => DeskCount - _location.KeyboardCount; }

    public float KeyboardCost { get => KeyboardCount * _cost.Keyboard; }

    public int MouseCount { get => DeskCount - _location.MouseCount; }

    public float MouseCost { get => MouseCount * _cost.Mouse; }

    // public int ChairCount { get; set; }
    // public float Chair { get; set; }    
}
