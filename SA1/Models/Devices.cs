using System;
using System.Collections.Generic; // Важливо для List<>
using System.Text.Json.Serialization;

namespace SA1.Models
{
    [JsonDerivedType(typeof(Laptop), typeDiscriminator: "Laptop")]
    [JsonDerivedType(typeof(Smartphone), typeDiscriminator: "Smartphone")]
    [JsonDerivedType(typeof(Tablet), typeDiscriminator: "Tablet")]
    public abstract class Device
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public Processor Cpu { get; set; }
        public Memory Ram { get; set; }
        public Screen DeviceScreen { get; set; }
        public Speaker InternalSpeaker { get; set; }
        public DeviceSettings Settings { get; set; } = new DeviceSettings();

        public double BatteryCapacityMax { get; set; }
        public double BatteryCapacityCurrent { get; set; }
        public string CurrentActivity { get; set; } = "Очікування";

        public List<string> ActionHistory { get; set; } = new List<string>();

        public void SetActivity(string activity) => CurrentActivity = activity;

        public void LogAction(string message)
        {
            ActionHistory.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            if (ActionHistory.Count > 10)
            {
                ActionHistory.RemoveAt(0); 
            }
        }

        public Device() { }

        protected Device(string name, Processor cpu, Memory ram, Screen screen, Speaker speaker, double battery)
        {
            Id = Guid.NewGuid();
            Name = name; Cpu = cpu; Ram = ram; DeviceScreen = screen; InternalSpeaker = speaker;
            BatteryCapacityMax = battery; BatteryCapacityCurrent = battery;
        }
    }

    public class Laptop : Device 
    { 
        public Laptop() { } 
        public Laptop(string n, Processor c, Memory r, Screen s, Speaker sp) : base(n, c, r, s, sp, 6000) { } 
    }

    public class Smartphone : Device 
    { 
        public Smartphone() { } 
        public Smartphone(string n, Processor c, Memory r, Screen s, Speaker sp) : base(n, c, r, s, sp, 3000) { } 
    }

    public class Tablet : Device 
    { 
        public Tablet() { } 
        public Tablet(string n, Processor c, Memory r, Screen s, Speaker sp) : base(n, c, r, s, sp, 5000) { } 
    }
}