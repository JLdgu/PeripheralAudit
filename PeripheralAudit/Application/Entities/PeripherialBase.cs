namespace PeripheralAudit.Application.Entities;

public abstract class PeripheralBase
{
    public int Id { get; set; }

    public Location Location { get; set; }

    public String? SerialNumber { get; set; }

    public String? AssetTag { get; set; }

    public String Manufacturer { get; set; }

    public String Model { get; set; }
}