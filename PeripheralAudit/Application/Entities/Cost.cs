namespace PeripheralAudit.Application;

public class Cost
{
    public Cost () {}
    public Cost(decimal dock, decimal monitor, decimal largeMonitor, decimal keyboard, decimal mouse, decimal chair)
    {
        Dock = dock;
        Monitor = monitor;
        LargeMonitor = largeMonitor;
        Keyboard = keyboard;
        Mouse = mouse;
        Chair = chair;
    }

    public decimal Dock { get; set; }
    public decimal Monitor { get; set; }
    public decimal LargeMonitor { get; set; }
    public decimal Keyboard { get; set; }
    public decimal Mouse { get; set; }
    public decimal Chair { get; set; }
}