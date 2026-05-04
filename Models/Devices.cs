using System;

namespace SA.Models
{
    public abstract class Device
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; protected set; }
        public Processor Cpu { get; protected set; }
        public Memory Ram { get; protected set; }
        
        // Стан пристрою змінюється ТІЛЬКИ через методи, без виводу в консоль
        public string CurrentActivity { get; private set; } = "Вимкнено / Очікування";

        protected Device(string name, Processor cpu, Memory ram)
        {
            Name = name;
            Cpu = cpu;
            Ram = ram;
        }

        public void Work() => CurrentActivity = "Виконує роботу з документами";
        public void Play() => CurrentActivity = "Запущено комп'ютерну гру";
        public void Chat() => CurrentActivity = "Відкрито месенджер для чату";
        public void ListenMusic() => CurrentActivity = "Відтворює музичний трек";
        public void WatchVideo() => CurrentActivity = "Відтворює відеоролик";
        public void PrintDocument() => CurrentActivity = "Відправляє документ на принтер";
    }

    public class Laptop : Device
    {
        public Laptop(string name, Processor cpu, Memory ram) 
            : base(name, cpu, ram) { }
    }

    public class Smartphone : Device
    {
        public TouchScreen Screen { get; private set; }

        public Smartphone(string name, Processor cpu, Memory ram, TouchScreen screen) 
            : base(name, cpu, ram)
        {
            Screen = screen;
        }
    }

    public class Tablet : Device
    {
        public TouchScreen Screen { get; private set; }

        public Tablet(string name, Processor cpu, Memory ram, TouchScreen screen) 
            : base(name, cpu, ram)
        {
            Screen = screen;
        }
    }
}