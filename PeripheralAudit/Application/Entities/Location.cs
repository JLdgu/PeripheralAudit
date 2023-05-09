namespace PeripheralAudit.Application.Entities;

public sealed class Location
{
    public int Id { get; set; }

    public string Name { get; set; }

    public Site Site  { get; set; }

    public int DeskCount { get; set; }

    public int MonitorSingleCount { get; set; }

    public int MonitorDualCount { get; set; }

    public int MonitorGradeBronzeCount { get; set; }

    public int MonitorGradeSilverCount { get; set; }

    public int MonitorGradeGoldCount { get; set; }

    public int DockCount { get; set; }

    public int DockPsuCount { get; set; }

    public int PcCount { get; set; }

    public int KeyboardCount { get; set; }

    public int MouseCount { get; set; }

    public int ChairCount { get; set; }

    public string? Note { get; set;}
}
