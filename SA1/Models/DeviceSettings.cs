namespace SA1.Models
{
    public class DeviceSettings
    {
        public bool HasSoftware { get; set; } = false;
        public bool HasNetwork { get; set; } = false;
        public bool HasPeripherals { get; set; } = false; 
        public bool HasExternalHeadset { get; set; } = false;
        public bool HasMouse { get; set; } = false;
        public bool IsPluggedIn { get; set; } = false;
    }
}