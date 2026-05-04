using System;
using SA.BLL;
using SA.DAL;
using SA.Models;

namespace SA
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Налаштування Dependency Injection "вручну"
            IDeviceRepository repository = new DeviceRepository();
            DeviceService deviceService = new DeviceService(repository);

            // 1. Створення даних
            Processor p1 = new Processor("Intel Core i5");
            Memory m1 = new Memory("DDR4", 16);
            Laptop laptop = new Laptop("Робочий Ноутбук", p1, m1);

            Processor p2 = new Processor("Apple A15");
            Memory m2 = new Memory("LPDDR5", 6);
            TouchScreen screen = new TouchScreen(6.1);
            Smartphone smartphone = new Smartphone("Особистий Смартфон", p2, m2, screen);

            // 2. Реєстрація через бізнес-шар
            deviceService.RegisterNewDevice(laptop);
            deviceService.RegisterNewDevice(smartphone);

            Console.WriteLine("--- СИМУЛЯЦІЯ: ІДЕАЛЬНИЙ СВІТ (3-Layer Architecture) ---\n");

            // 3. Виконання дій та відображення
            foreach (var device in deviceService.GetAllDevices())
            {
                Console.WriteLine($"[ПРИСТРІЙ]: {device.Name} ({device.GetType().Name})");
                Console.WriteLine($"Початковий стан: {device.CurrentActivity}");
                
                // Викликаємо логіку через сервіс (BLL)
                deviceService.MakeDeviceWork(device.Id);
                Console.WriteLine($"Зміна стану 1: {device.CurrentActivity}");

                if (device is Smartphone)
                {
                    deviceService.MakeDeviceWatchVideo(device.Id);
                    Console.WriteLine($"Зміна стану 2: {device.CurrentActivity}");
                }
                else
                {
                    deviceService.MakeDevicePlay(device.Id);
                    Console.WriteLine($"Зміна стану 2: {device.CurrentActivity}");
                }

                Console.WriteLine(new string('-', 30));
            }

            Console.ReadLine();
        }
    }
}