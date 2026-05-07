using System;
using System.Linq;
using SA1.Models;
using SA1.Factories;
using SA1.Services;
using SA1.Repository;
using SA1;

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
            Console.WriteLine("3. Налаштування обладнання");
            Console.WriteLine("4. Виконати дію");
            Console.WriteLine("5. Історія дій (Останні 10)");
            Console.WriteLine("6. Характеристики пристрою");
            Console.WriteLine("0. Вихід");
            Console.Write("\nВибір: ");
            
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.WriteLine("\n1. Ноутбук | 2. Смартфон | 3. Планшет | 0. Назад");
                    string type = Console.ReadLine();
                    Device newDevice = null;

                    switch (type)
                    {
                        case "1": newDevice = new LaptopFactory().CreateDevice($"Laptop #{repository.GetAllDevices().Count() + 1}"); break;
                        case "2": newDevice = new SmartphoneFactory().CreateDevice($"Phone #{repository.GetAllDevices().Count() + 1}"); break;
                        case "3": newDevice = new TabletFactory().CreateDevice($"Tablet #{repository.GetAllDevices().Count() + 1}"); break;
                    }

                    if (newDevice != null)
                    {
                        repository.AddDevice(newDevice);
                        currentDevice = newDevice;
                        Console.WriteLine("Пристрій створено!");
                        Console.ReadKey();
                    }
                    break;

                case "2":
                    var all = repository.GetAllDevices().ToList();
                    if (!all.Any())
                    {
                        Console.WriteLine("\nСписок порожній!");
                        Console.ReadKey();
                        break;
                    }

                    Console.WriteLine("\nОберіть пристрій:");
                    for (int i = 0; i < all.Count; i++)
                        Console.WriteLine($"{i + 1}. {all[i].Name} [{all[i].GetType().Name}]");
                    
                    if (int.TryParse(Console.ReadLine(), out int idx) && idx > 0 && idx <= all.Count)
                        currentDevice = all[idx - 1];
                    break;

                case "3":
                    if (currentDevice == null) { ConsoleMenu.ShowNoDeviceError(); break; }
                    ConsoleMenu.RunSettingsMenu(currentDevice, repository);
                    break;

                case "4":
                    if (currentDevice == null) { ConsoleMenu.ShowNoDeviceError(); break; }
                    ConsoleMenu.RunActionMenu(currentDevice, service, repository);
                    break;

                case "5":
                    if (currentDevice == null) { ConsoleMenu.ShowNoDeviceError(); break; }
                    ConsoleMenu.ShowHistory(currentDevice);
                    break;

                case "6":
                    if (currentDevice == null) { ConsoleMenu.ShowNoDeviceError(); break; }
                    ConsoleMenu.ShowSpecs(currentDevice);
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Некоректний вибір. Спробуйте ще раз.");
                    Console.ReadKey();
                    break;
            }
        }
    }
}