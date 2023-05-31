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

    public decimal RepopulationCost { get => BronzeMonitorCost + DockCost + KeyboardCost + MouseCost + ChairCost; }

    public int DeskCount { get => _location.DeskCount; }

    public int BronzeMonitorCount
    {
        get => DeskCount - _location.MonitorGradeBronzeCount - _location.MonitorGradeSilverCount - _location.MonitorGradeGoldCount;
    }

    public decimal BronzeMonitorCost { get => 0; }

    public int SilverMonitorCount { get => DeskCount - _location.MonitorGradeSilverCount - _location.MonitorGradeGoldCount; }

    public decimal SilverMonitorCost { get => SilverMonitorCount * _cost.Monitor; }

    public int GoldMonitorCount { get => DeskCount - _location.MonitorGradeGoldCount; }

    public decimal GoldMonitorCost { get => GoldMonitorCount * _cost.LargeMonitor; }

    public int DockCount { get => DeskCount - _location.DockCount - _location.PcCount; }

    public decimal DockCost { get => DockCount * _cost.Dock; }

    public int KeyboardCount { get => DeskCount - _location.KeyboardCount; }

    public decimal KeyboardCost { get => KeyboardCount * _cost.Keyboard; }

    public int MouseCount { get => DeskCount - _location.MouseCount; }

    public decimal MouseCost { get => MouseCount * _cost.Mouse; }

    public int? ChairCount
    {
        get
        {
            if (_location.ChairCount is null)
                return null;
            return DeskCount - _location.ChairCount;
        }
    }

    public decimal ChairCost
    { 
        get
        {
            if (ChairCount is null)
                return 0;
            return (decimal)ChairCount * _cost.Chair;
        } 
    }
}
