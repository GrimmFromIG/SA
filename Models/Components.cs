namespace SA.Models
{
    public class Processor
    {
        public string Model { get; private set; }
        public Processor(string model) => Model = model;
    }

    public class Memory
    {
        public string Type { get; private set; }
        public int CapacityGB { get; private set; }
        public Memory(string type, int capacity)
        {
            Type = type;
            CapacityGB = capacity;
        }
    }

    public class TouchScreen
    {
        public double DiagonalSize { get; private set; }
        public TouchScreen(double diagonalSize) => DiagonalSize = diagonalSize;
    }
}