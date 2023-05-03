namespace PeripheralAudit.Application.Entities;

public class Dock
{
    public int Id { get; set; }

    public Location Location { get; set; }

    public String? SerialNumber { get; set; }

    public String? AssetTag { get; set; }
}