using System;
using System.Linq;
using SA1.Models;
using SA1.Factories;
using SA1.Services;
using SA1.Repository;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        IDeviceRepository repository = new DeviceRepository();
        DeviceService service = new DeviceService();
        Device currentDevice = repository.GetAllDevices().FirstOrDefault();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== СИСТЕМА УПРАВЛІННЯ ПРИСТРОЯМИ ===");
            
            if (currentDevice != null)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"АКТИВНИЙ ПРИСТРІЙ: {currentDevice.Name} ({currentDevice.GetType().Name})");
                Console.WriteLine($"Заряд: {currentDevice.BatteryCapacityCurrent:F0} / {currentDevice.BatteryCapacityMax} мАг | Стан: {currentDevice.CurrentActivity}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("Пристрій не обрано. Створіть новий або виберіть зі списку.");
            }

            Console.WriteLine("\n1. Створити новий пристрій");
            Console.WriteLine("2. Переключити активний пристрій");
            Console.WriteLine("3. Налаштування обладнання (ПЗ, Мережа, Живлення...)");
            Console.WriteLine("4. Виконати дію (Робота, Гра, Зарядка...)");
            Console.WriteLine("5. Історія дій (Останні 10)");
            Console.WriteLine("6. Характеристики пристрою"); // НОВИЙ ПУНКТ
            Console.WriteLine("0. Вихід");
            Console.Write("\nВибір: ");
            string input = Console.ReadLine();

            if (input == "1")
            {
                Console.WriteLine("\n1. Ноутбук | 2. Смартфон | 3. Планшет | 0. Назад");
                string type = Console.ReadLine();
                Device newDevice = null;

                if (type == "1") newDevice = new LaptopFactory().CreateDevice($"Laptop #{repository.GetAllDevices().Count() + 1}");
                if (type == "2") newDevice = new SmartphoneFactory().CreateDevice($"Phone #{repository.GetAllDevices().Count() + 1}");
                if (type == "3") newDevice = new TabletFactory().CreateDevice($"Tablet #{repository.GetAllDevices().Count() + 1}");

                if (newDevice != null)
                {
                    repository.AddDevice(newDevice);
                    currentDevice = newDevice;
                    Console.WriteLine("Пристрій створено та встановлено як активний!");
                    Console.ReadKey();
                }
            }

            if (input == "2")
            {
                var all = repository.GetAllDevices().ToList();
                if (!all.Any())
                {
                    Console.WriteLine("\nСписок порожній!");
                    Console.ReadKey();
                    continue;
                }

                Console.WriteLine("\nОберіть пристрій:");
                for (int i = 0; i < all.Count; i++)
                    Console.WriteLine($"{i + 1}. {all[i].Name} [{all[i].GetType().Name}]");
                Console.WriteLine("0. Назад");

                if (int.TryParse(Console.ReadLine(), out int idx) && idx > 0 && idx <= all.Count)
                {
                    currentDevice = all[idx - 1];
                }
            }

            if (input == "3" && currentDevice != null)
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"--- НАЛАШТУВАННЯ: {currentDevice.Name} ---");
                    Console.WriteLine($"1. ПЗ: {(currentDevice.Settings.HasSoftware ? "Так" : "Ні")}");
                    Console.WriteLine($"2. Мережа: {(currentDevice.Settings.HasNetwork ? "Так" : "Ні")}");
                    Console.WriteLine($"3. Принтер: {(currentDevice.Settings.HasPeripherals ? "Так" : "Ні")}");
                    Console.WriteLine($"4. Гарнітура: {(currentDevice.Settings.HasExternalHeadset ? "Так" : "Ні")}");
                    Console.WriteLine($"5. Миша: {(currentDevice.Settings.HasMouse ? "Так" : "Ні")}");
                    Console.ForegroundColor = currentDevice.Settings.IsPluggedIn ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine($"6. ЖИВЛЕННЯ ВІД МЕРЕЖІ: {(currentDevice.Settings.IsPluggedIn ? "ПІДКЛЮЧЕНО" : "ВІДКЛЮЧЕНО")}");
                    Console.ResetColor();
                    Console.WriteLine("0. Назад");
                    Console.Write("Вибір пункту: ");
                    string cfg = Console.ReadLine();

                    if (cfg == "0") break;

                    Console.Write("Змінити стан? (1 - Увімк/Вст, 2 - Вимк/Вид): ");
                    bool state = Console.ReadLine() == "1";

                    if (cfg == "1") currentDevice.Settings.HasSoftware = state;
                    if (cfg == "2") currentDevice.Settings.HasNetwork = state;
                    if (cfg == "3") currentDevice.Settings.HasPeripherals = state;
                    if (cfg == "4") currentDevice.Settings.HasExternalHeadset = state;
                    if (cfg == "5") currentDevice.Settings.HasMouse = state;
                    if (cfg == "6") currentDevice.Settings.IsPluggedIn = state;

                    repository.SaveData(); 
                }
            }

            if (input == "4" && currentDevice != null)
            {
                Console.WriteLine("\n--- ВИБІР ДІЇ ---");
                Console.WriteLine("1. Робота | 2. Відео | 3. Музика | 4. Друк | 5. Зарядка | 6. Гра | 0. Назад");
                string actChoice = Console.ReadLine();
                if (actChoice == "0") continue;

                Console.Write("Скільки годин триватиме дія? (наприклад, 1): ");
                if (!int.TryParse(Console.ReadLine(), out int hours)) hours = 1;

                string actName = "";
                bool isIntensive = false;

                if (actChoice == "5") 
                {
                    actName = "Зарядка";
                }
                else 
                {
                    isIntensive = (actChoice == "1" || actChoice == "2" || actChoice == "6"); 
                    actName = actChoice switch { "1"=>"Робота", "2"=>"Відео", "3"=>"Музика", "4"=>"Друк", "6"=>"Гра", _ => "" };
                }

                if (service.ExecuteAction(currentDevice, actName, isIntensive, hours, out string resultMessage))
                {
                    if (resultMessage.Contains("[УВАГА]"))
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    else
                        Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine($"\n{resultMessage}");
                    Console.ResetColor();
                    
                    repository.SaveData();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n{resultMessage}");
                    Console.ResetColor();
                }
                
                Console.ReadKey();
            }

            if (input == "5" && currentDevice != null)
            {
                Console.Clear();
                Console.WriteLine($"--- ІСТОРІЯ ДІЙ: {currentDevice.Name} ---");
                
                if (currentDevice.ActionHistory.Count == 0)
                {
                    Console.WriteLine("Пристрій ще не виконував жодних дій.");
                }
                else
                {
                    foreach (var record in currentDevice.ActionHistory)
                    {
                        Console.WriteLine(record);
                    }
                }
                
                Console.WriteLine("\nНатисніть будь-яку клавішу для повернення...");
                Console.ReadKey();
            }

            if (input == "6" && currentDevice != null)
            {
                Console.Clear();
                Console.WriteLine($"=== ХАРАКТЕРИСТИКИ: {currentDevice.Name} ===");
                Console.WriteLine($"[Тип пристрою]: {currentDevice.GetType().Name}");
                Console.WriteLine($"[Процесор]:     {currentDevice.Cpu.Model}");
                Console.WriteLine($"[ОЗП]:          {currentDevice.Ram.CapacityGB} ГБ");
                Console.WriteLine($"[Екран]:        {currentDevice.DeviceScreen.Size} дюймів (Сенсор: {(currentDevice.DeviceScreen.IsTouch ? "Так" : "Ні")})");
                Console.WriteLine($"[Динаміки]:     {currentDevice.InternalSpeaker.Model} ({currentDevice.InternalSpeaker.PowerWatts} Вт)");
                Console.WriteLine($"[Батарея]:      {currentDevice.BatteryCapacityMax} мАг");
                Console.WriteLine($"[ID]:           {currentDevice.Id}");
                
                Console.WriteLine("\nНатисніть будь-яку клавішу для повернення...");
                Console.ReadKey();
            }

            if ((input == "3" || input == "4" || input == "5" || input == "6") && currentDevice == null)
            {
                Console.WriteLine("\n[ПОМИЛКА] Спочатку створіть або виберіть пристрій!");
                Console.ReadKey();
            }

            if (input == "0") break;
        }
    }
}