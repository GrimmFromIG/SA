using SA1.Models;

namespace SA1.Factories
{
    public abstract class DeviceFactory
    {
        public abstract Device CreateDevice(string name);
    }

    public class LaptopFactory : DeviceFactory
    {
        public override Device CreateDevice(string name) => 
            new Laptop(name, new Processor("Intel i7"), new Memory(16), 
                new Screen(15.6, false), new Speaker("Stereo Dolby Atmos", 2.5));
    }

    public class SmartphoneFactory : DeviceFactory
    {
        public override Device CreateDevice(string name) => 
            new Smartphone(name, new Processor("A16"), new Memory(6), 
                new Screen(6.1, true), new Speaker("Internal Mono Mono", 1.0));
    }

    public class TabletFactory : DeviceFactory
    {
        public override Device CreateDevice(string name) => 
            new Tablet(name, new Processor("Apple M2"), new Memory(8), 
                new Screen(11.0, true), new Speaker("Quad Surround", 4.0));
    }
}