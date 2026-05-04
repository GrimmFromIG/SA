using System;
using SA1.BLL;
using SA1.Models;

namespace SA1
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var service = new DeviceService();

            // 1. Ініціалізація
            var laptop = new Laptop("Робочий Ноутбук", new Processor("Intel Core i5"), new Memory("DDR4", 16));
            var phone = new Smartphone("Особистий Смартфон", new Processor("Snapdragon 8"), new Memory("LPDDR5", 8), new TouchScreen(6.1));
            var tablet = new Tablet("Домашній Планшет", new Processor("Apple A15"), new Memory("LPDDR5", 6), new TouchScreen(10.5));

            Console.WriteLine("--- СИМУЛЯЦІЯ РОБОТИ ПРИСТРОЇВ (Варіант 5) ---\n");

            Console.WriteLine("1. РЕЄСТРАЦІЯ ПРИСТРОЇВ");
            Console.WriteLine($"[ДІЯ] Користувач купує: {laptop.Name} (Laptop, {laptop.Cpu.Model}, {laptop.Ram.CapacityGB}GB)");
            Console.WriteLine($"[ДІЯ] Користувач купує: {phone.Name} (Smartphone, {phone.Cpu.Model}, {phone.Ram.CapacityGB}GB, Екран: {phone.Screen.DiagonalSize}\")");
            Console.WriteLine($"[ДІЯ] Користувач купує: {tablet.Name} (Tablet, {tablet.Cpu.Model}, {tablet.Ram.CapacityGB}GB, Екран: {tablet.Screen.DiagonalSize}\")");

            Console.WriteLine("\n2. ПЕРЕВІРКА ЖИВЛЕННЯ (БЕЗ ЕЛЕКТРОЕНЕРГІЇ)");
            Console.WriteLine("[СТАТУС] Світло вимкнули. Пристрої переходять на акумулятори.");
            Console.WriteLine($" - {laptop.Name} працює від акумулятора (Залишок: {laptop.BatteryCapacityCurrent} мВтг).");
            Console.WriteLine($" - {phone.Name} працює від акумулятора (Залишок: {phone.BatteryCapacityCurrent} мАг).");

            Console.WriteLine("\n3. СПРОБА ЗАПУСТИТИ ГРУ БЕЗ ПЗ");
            PrintAction(laptop.Name, service.TryExecuteAction(laptop, "запустити гру", true));

            Console.WriteLine("\n4. ВСТАНОВЛЕННЯ ПЗ ТА ПІДКЛЮЧЕННЯ ДО МЕРЕЖІ");
            laptop.HasSoftware = true;
            laptop.HasNetwork = true;
            Console.WriteLine($"[ДІЯ] На {laptop.Name} встановлено ігрове ПЗ.");
            Console.WriteLine($"[ДІЯ] {laptop.Name} підключено до Wi-Fi.");

            Console.WriteLine("\n5. ЗАПУСК ГРИ ТА ВИТРАТА ЗАРЯДУ");
            PrintAction(laptop.Name, service.TryExecuteAction(laptop, "Запущено комп'ютерну гру", true));
            service.DrainBattery(laptop, 3, true); // Грали 3 години
            Console.WriteLine("[СТАТУС] Після 3 годин гри...");
            Console.WriteLine($" - Залишок заряду {laptop.Name}: {laptop.BatteryCapacityCurrent} мВтг (Вистачить ще на {service.CalculateRemainingHours(laptop, true)} год інтенсивного використання).");

            Console.WriteLine("\n6. РОБОТА СМАРТФОНА");
            phone.HasSoftware = true;
            PrintAction(phone.Name, service.TryExecuteAction(phone, "Відкрито месенджер для чату", false));
            service.DrainBattery(phone, 10, false); // Чатились 10 годин
            Console.WriteLine("[СТАТУС] Після 10 годин чату...");
            Console.WriteLine($" - Залишок заряду {phone.Name}: {phone.BatteryCapacityCurrent} мАг (Вистачить ще на {service.CalculateRemainingHours(phone, false)} год).");

            Console.WriteLine("\n7. РОЗРЯДКА ПРИСТРОЮ");
            Console.WriteLine($"[ДІЯ] Користувач грає на планшеті 5 годин підряд...");
            tablet.HasSoftware = true;
            service.DrainBattery(tablet, 5, true); // Планшет витримує 4 години важкої гри, тому через 5 годин він сяде
            PrintAction(tablet.Name, service.TryExecuteAction(tablet, "грати", true));

            Console.WriteLine("\n8. ПІДКЛЮЧЕННЯ ЗОВНІШНІХ ПРИСТРОЇВ (ПРИНТЕР)");
            PrintAction(laptop.Name, service.TryExecuteAction(laptop, "друк документа", false));
            laptop.HasPeripherals = true;
            Console.WriteLine($"[ДІЯ] Підключено принтер до {laptop.Name}.");
            PrintAction(laptop.Name, service.TryExecuteAction(laptop, "Відправляє документ на принтер", false));

            Console.ReadLine();
        }

        static void PrintAction(string name, ActionResponse response)
        {
            string prefix = response.IsSuccess ? "[УСПІХ]" : "[ВІДМОВА]";
            Console.WriteLine($"{prefix} {name} {(response.IsSuccess ? ":" : "")} {response.Message}");
        }
    }
}