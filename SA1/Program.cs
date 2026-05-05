using System;
using SA1.BLL;
using SA1.DAL;
using SA1.Models;

namespace SA1
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            IDeviceRepository repository = new DeviceRepository(); 
            DeviceService service = new DeviceService(repository); 

            var laptop = new Laptop("Робочий Ноутбук", new Processor("Intel Core i7"), new Memory("DDR5", 32));
            var phone = new Smartphone("Особистий Смартфон", new Processor("Apple A16 Bionic"), new Memory("LPDDR5", 6), new TouchScreen(6.1));
            var tablet = new Tablet("Домашній Планшет", new Processor("Snapdragon 8 Gen 2"), new Memory("LPDDR5X", 8), new TouchScreen(11.0));

   
            service.RegisterDevice(laptop);
            service.RegisterDevice(phone);
            service.RegisterDevice(tablet);

            Console.WriteLine("=================================================");
            Console.WriteLine("  СИМУЛЯЦІЯ РОБОТИ ПРИСТРОЇВ  ");
            Console.WriteLine("=================================================\n");

            Console.WriteLine("1. ДОСТУПНІ ПРИСТРОЇ У БАЗІ ДАНИХ:");
            foreach (var device in service.GetAllRegisteredDevices())
            {
                Console.WriteLine($" - {device.Name} (Батарея: {device.BatteryCapacityMax} умовних одиниць)");
            }

            Console.WriteLine("\n2. ЖИВЛЕННЯ ВІДКЛЮЧЕНО (ПЕРЕХІД НА АКУМУЛЯТОРИ)");
            Console.WriteLine("[СТАТУС] Всі пристрої працюють від власних батарей.");

            Console.WriteLine("\n3. СПРОБА ГРАТИ НА НОУТБУЦІ БЕЗ ІГОР");
            PrintAction(laptop.Name, service.TryExecuteAction(laptop, "запустити гру Cyberpunk", true));
            
            Console.WriteLine("[ДІЯ] Встановлюємо ігрове ПЗ на Ноутбук...");
            laptop.HasSoftware = true;
            PrintAction(laptop.Name, service.TryExecuteAction(laptop, "запустити гру Cyberpunk", true));

            Console.WriteLine("\n[ЧАС] Граємо 3 години підряд (Інтенсивне використання)...");
            service.DrainBattery(laptop, 3, true);
            Console.WriteLine($"[БАТАРЕЯ] Залишок на {laptop.Name}: {laptop.BatteryCapacityCurrent} мВтг. Вистачить ще на {service.CalculateRemainingHours(laptop, true)} год активної гри.");

            Console.WriteLine("\n4. ПЕРЕВІРКА МЕРЕЖІ НА СМАРТФОНІ");
            phone.HasSoftware = true; 
            PrintAction(phone.Name, service.TryExecuteAction(phone, "Мережа: Зайти в Telegram", false));
            
            Console.WriteLine("[ДІЯ] Підключаємо Смартфон до 4G-мережі...");
            phone.HasNetwork = true;
            PrintAction(phone.Name, service.TryExecuteAction(phone, "Мережа: Зайти в Telegram", false));

            Console.WriteLine("\n[ЧАС] Сидимо в месенджерах 10 годин (Неінтенсивне використання)...");
            service.DrainBattery(phone, 10, false);
            Console.WriteLine($"[БАТАРЕЯ] Залишок на {phone.Name}: {phone.BatteryCapacityCurrent} мАг. Вистачить ще на {service.CalculateRemainingHours(phone, false)} год чату.");

            Console.WriteLine("\n5. ЖОРСТКИЙ ТЕСТ ПЛАНШЕТА (ПОВНА РОЗРЯДКА)");
            tablet.HasSoftware = true;
            PrintAction(tablet.Name, service.TryExecuteAction(tablet, "Дивитися серіали", false));
            
            Console.WriteLine("[ЧАС] Користувач заснув і планшет програвав відео 15 годин підряд...");
            service.DrainBattery(tablet, 15, false);
            
            Console.WriteLine($"[БАТАРЕЯ] Поточний заряд {tablet.Name}: {tablet.BatteryCapacityCurrent}");
            
            Console.WriteLine("[ДІЯ] Спроба відкрити браузер на планшеті...");
            PrintAction(tablet.Name, service.TryExecuteAction(tablet, "Відкрити браузер", false));

            Console.WriteLine("\n6. ПІДКЛЮЧЕННЯ ПЕРИФЕРІЇ ДО НОУТБУКА");
            PrintAction(laptop.Name, service.TryExecuteAction(laptop, "друк курсової роботи", false));
            
            Console.WriteLine("[ДІЯ] Підключаємо принтер через USB...");
            laptop.HasPeripherals = true;
            PrintAction(laptop.Name, service.TryExecuteAction(laptop, "друк курсової роботи", false));

            Console.WriteLine("\n=================================================");
            Console.WriteLine("СИМУЛЯЦІЯ УСПІШНО ЗАВЕРШЕНА! Натисніть Enter...");
            Console.ReadLine();
        }
        static void PrintAction(string name, ActionResponse response)
        {
            if (response.IsSuccess)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[УСПІХ] {name}: {response.Message}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ВІДМОВА] {name} {response.Message}");
            }
            Console.ResetColor(); 
        }
    }
}