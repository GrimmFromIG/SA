using System;

namespace SA1.Models
{
    public abstract class Device
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; protected set; }
        public Processor Cpu { get; protected set; }
        public Memory Ram { get; protected set; }
        public double BatteryCapacityCurrent { get; set; }
        public double BatteryCapacityMax { get; protected set; }
        public bool HasSoftware { get; set; } = false;
        public bool HasNetwork { get; set; } = false;
        public bool HasPeripherals { get; set; } = false;

        public string CurrentActivity { get; private set; } = "Очікування";

        protected Device(string name, Processor cpu, Memory ram, double batteryCapacity)
        {
            Name = name;
            Cpu = cpu;
            Ram = ram;
            BatteryCapacityMax = batteryCapacity;
            BatteryCapacityCurrent = batteryCapacity;
        }

        public void SetActivity(string activity) => CurrentActivity = activity;
    }

    public class Laptop : Device
    {
        public Laptop(string name, Processor cpu, Memory ram) 
            : base(name, cpu, ram, 35000) { } // 35000 мВтг
    }

    public class Smartphone : Device
    {
        public TouchScreen Screen { get; private set; }
        public Smartphone(string name, Processor cpu, Memory ram, TouchScreen screen) 
            : base(name, cpu, ram, 3000) // 3000 мАг
        {
            Screen = screen;
        }
    }

    public class Tablet : Device
    {
        public TouchScreen Screen { get; private set; }
        public Tablet(string name, Processor cpu, Memory ram, TouchScreen screen) 
            : base(name, cpu, ram, 5000) // 5000 мАг
        {
            Screen = screen;
        }
    }
}