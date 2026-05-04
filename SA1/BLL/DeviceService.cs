using System;
using SA1.Models;

namespace SA1.BLL
{
    public class ActionResponse
    {
        public bool IsSuccess { get; }
        public string Message { get; }
        public ActionResponse(bool success, string message) { IsSuccess = success; Message = message; }
    }

    public class DeviceService
    {
        // Метод для виконання дій з урахуванням інтенсивності (для розряду батареї)
        public ActionResponse TryExecuteAction(Device device, string actionName, bool isIntensive)
        {
            if (device.BatteryCapacityCurrent <= 0)
                return new ActionResponse(false, "вимкнувся: Акумулятор повністю розряджений!");

            if (!device.HasSoftware)
                return new ActionResponse(false, $"не може {actionName}: Відсутнє необхідне ПЗ.");

            if (actionName.Contains("Мережа") && !device.HasNetwork)
                return new ActionResponse(false, $"не може {actionName}: Немає підключення до мережі.");

            if (actionName.Contains("друк") && !device.HasPeripherals)
                return new ActionResponse(false, "відмовляється друкувати: Не підключено принтер.");

            // Виконання дії
            device.SetActivity(actionName);
            return new ActionResponse(true, $"{actionName} ({(isIntensive ? "Інтенсивне" : "Неінтенсивне")} використання).");
        }

        // Логіка розряду батареї
        public void DrainBattery(Device device, int hours, bool isIntensive)
        {
            if (device.BatteryCapacityCurrent <= 0) return;

            double drainRatePerHour;
            if (device.BatteryCapacityMax <= 3000) // Смартфон (до 48г легке, 16г важке)
                drainRatePerHour = isIntensive ? (3000.0 / 16) : (3000.0 / 48);
            else // Ноутбук/Планшет (до 12г легке, 4г важке)
                drainRatePerHour = isIntensive ? (device.BatteryCapacityMax / 4) : (device.BatteryCapacityMax / 12);

            device.BatteryCapacityCurrent -= drainRatePerHour * hours;
            if (device.BatteryCapacityCurrent < 0) device.BatteryCapacityCurrent = 0;
        }

        public double CalculateRemainingHours(Device device, bool isIntensive)
        {
            if (device.BatteryCapacityCurrent <= 0) return 0;
            
            double drainRatePerHour;
            if (device.BatteryCapacityMax <= 3000)
                drainRatePerHour = isIntensive ? (3000.0 / 16) : (3000.0 / 48);
            else
                drainRatePerHour = isIntensive ? (device.BatteryCapacityMax / 4) : (device.BatteryCapacityMax / 12);

            return Math.Round(device.BatteryCapacityCurrent / drainRatePerHour, 1);
        }
    }
}