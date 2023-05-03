namespace PeripheralAudit.Application.Entities;

public class Location
{
    public int Id { get; set; }

    public string Name { get; set; }

    public Site Site  { get; set; }

    public int DeskCount { get; set; }

    public int DockCount { get; set; }

    public int DockPsuCount { get; set; }

    public int PcCount { get; set; }

    public int KeyboardCount { get; set; }

    public int MouseCount { get; set; }

    public int ChairCount { get; set; }

    public string? Note { get; set;}
}
