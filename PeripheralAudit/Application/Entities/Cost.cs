namespace PeripheralAudit.Application;

public class Cost
{
    public Cost () {}
    public Cost(float dock, float monitor, float largeMonitor, float keyboard, float mouse, float chair)
    {
        Dock = dock;
        Monitor = monitor;
        LargeMonitor = largeMonitor;
        Keyboard = keyboard;
        Mouse = mouse;
        Chair = chair;
    }

    public float Dock { get; set; }
    public float Monitor { get; set; }
    public float LargeMonitor { get; set; }
    public float Keyboard { get; set; }
    public float Mouse { get; set; }
    public float Chair { get; set; }
}