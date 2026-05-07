namespace SA1.Models
{
    public class Processor 
    { 
        public string Model { get; set; } 
        public Processor() {} 
        public Processor(string m) => Model = m; 
    }
    
    public class Memory 
    { 
        public int CapacityGB { get; set; } 
        public Memory() {} 
        public Memory(int c) => CapacityGB = c; 
    }
    
    public class Screen 
    { 
        public double Size { get; set; } 
        public bool IsTouch { get; set; } 
        public Screen() {} 
        public Screen(double s, bool t) { Size = s; IsTouch = t; } 
    }

    public class Speaker
    {
        public string Model { get; set; }
        public double PowerWatts { get; set; }
        public Speaker() {} 
        public Speaker(string model, double power) { Model = model; PowerWatts = power; }
    }
}