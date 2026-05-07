using System;
using System.Linq;
using SA1.Models;
using SA1.Services;
using SA1.Repository;

namespace SA1
{
    public static class ConsoleMenu
    {
        public static void ShowNoDeviceError()
        {
            Console.WriteLine("\n[ПОМИЛКА] Спочатку створіть або виберіть пристрій!");
            Console.ReadKey();
        }

        public static void RunSettingsMenu(Device device, IDeviceRepository repo)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"--- НАЛАШТУВАННЯ: {device.Name} ---");
                Console.WriteLine($"1. ПЗ: {(device.Settings.HasSoftware ? "Так" : "Ні")}");
                Console.WriteLine($"2. Мережа: {(device.Settings.HasNetwork ? "Так" : "Ні")}");
                Console.WriteLine($"3. Принтер: {(device.Settings.HasPeripherals ? "Так" : "Ні")}");
                Console.WriteLine($"4. Гарнітура: {(device.Settings.HasExternalHeadset ? "Так" : "Ні")}");
                Console.WriteLine($"5. Миша: {(device.Settings.HasMouse ? "Так" : "Ні")}");
                
                Console.ForegroundColor = device.Settings.IsPluggedIn ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"6. ЖИВЛЕННЯ: {(device.Settings.IsPluggedIn ? "ПІДКЛЮЧЕНО" : "ВІДКЛЮЧЕНО")}");
                Console.ResetColor();
                
                Console.WriteLine("0. Назад");
                Console.Write("Вибір пункту: ");
                string cfg = Console.ReadLine();
                if (cfg == "0") break;

                Console.Write("Змінити стан? (1 - Увімк, 2 - Вимк): ");
                bool state = Console.ReadLine() == "1";

                switch (cfg)
                {
                    case "1": device.Settings.HasSoftware = state; break;
                    case "2": device.Settings.HasNetwork = state; break;
                    case "3": device.Settings.HasPeripherals = state; break;
                    case "4": device.Settings.HasExternalHeadset = state; break;
                    case "5": device.Settings.HasMouse = state; break;
                    case "6": device.Settings.IsPluggedIn = state; break;
                }
                repo.SaveData();
            }
        }

        public static void RunActionMenu(Device device, DeviceService service, IDeviceRepository repo)
        {
            Console.WriteLine("\n--- ВИБІР ДІЇ ---");
            Console.WriteLine("1. Робота | 2. Відео | 3. Музика | 4. Друк | 5. Зарядка | 6. Гра | 0. Назад");
            string choice = Console.ReadLine();
            if (choice == "0") return;

            Console.Write("Скільки годин? ");
            if (!int.TryParse(Console.ReadLine(), out int hours)) hours = 1;

            string actName = "";
            bool isIntensive = false;

            switch (choice)
            {
                case "1": actName = "Робота"; isIntensive = true; break;
                case "2": actName = "Відео"; isIntensive = true; break;
                case "3": actName = "Музика"; break;
                case "4": actName = "Друк"; break;
                case "5": actName = "Зарядка"; break;
                case "6": actName = "Гра"; isIntensive = true; break;
                default: return;
            }

            if (service.ExecuteAction(device, actName, isIntensive, hours, out string msg))
            {
                Console.ForegroundColor = msg.Contains("[УВАГА]") ? ConsoleColor.Yellow : ConsoleColor.Green;
                Console.WriteLine($"\n{msg}");
                Console.ResetColor();
                repo.SaveData();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n{msg}");
                Console.ResetColor();
            }
            Console.ReadKey();
        }

        public static void ShowHistory(Device device)
        {
            Console.Clear();
            Console.WriteLine($"--- ІСТОРІЯ ДІЙ: {device.Name} ---");
            if (!device.ActionHistory.Any()) Console.WriteLine("Історія порожня.");
            else foreach (var record in device.ActionHistory) Console.WriteLine(record);
            Console.ReadKey();
        }

        public static void ShowSpecs(Device device)
        {
            Console.Clear();
            Console.WriteLine($"=== ХАРАКТЕРИСТИКИ: {device.Name} ===");
            Console.WriteLine($"[Тип]:      {device.GetType().Name}");
            Console.WriteLine($"[Процесор]: {device.Cpu.Model}");
            Console.WriteLine($"[ОЗП]:      {device.Ram.CapacityGB} ГБ");
            Console.WriteLine($"[Екран]:    {device.DeviceScreen.Size}\" (Touch: {device.DeviceScreen.IsTouch})");
            Console.WriteLine($"[Динаміки]: {device.InternalSpeaker.Model} ({device.InternalSpeaker.PowerWatts}Вт)");
            Console.WriteLine($"[Батарея]:  {device.BatteryCapacityMax} мАг");
            Console.ReadKey();
        }
    }
}