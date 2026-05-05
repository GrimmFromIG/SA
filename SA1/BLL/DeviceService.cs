using System;
using System.Collections.Generic;
using SA1.DAL;
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
        private readonly IDeviceRepository _repository;

        public DeviceService(IDeviceRepository repository)
        {
            _repository = repository;
        }

        public void RegisterDevice(Device device)
        {
            _repository.AddDevice(device);
        }

        public IEnumerable<Device> GetAllRegisteredDevices()
        {
            return _repository.GetAllDevices();
        }

        
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

            device.SetActivity(actionName);
            return new ActionResponse(true, $"{actionName} ({(isIntensive ? "Інтенсивне" : "Неінтенсивне")} використання).");
        }

        private double GetDrainRatePerHour(Device device, bool isIntensive)
        {
            if (device.BatteryCapacityMax <= 3000)
                return isIntensive ? (device.BatteryCapacityMax / 16.0) : (device.BatteryCapacityMax / 48.0);
            else
                return isIntensive ? (device.BatteryCapacityMax / 4.0) : (device.BatteryCapacityMax / 12.0);
        }

        public void DrainBattery(Device device, int hours, bool isIntensive)
        {
            if (device.BatteryCapacityCurrent <= 0) return;
            
            double drainRatePerHour = GetDrainRatePerHour(device, isIntensive);
            device.BatteryCapacityCurrent -= drainRatePerHour * hours;
            
            if (device.BatteryCapacityCurrent < 0) 
                device.BatteryCapacityCurrent = 0;
        }

        public double CalculateRemainingHours(Device device, bool isIntensive)
        {
            if (device.BatteryCapacityCurrent <= 0) return 0;
            
            double drainRatePerHour = GetDrainRatePerHour(device, isIntensive);
            return Math.Round(device.BatteryCapacityCurrent / drainRatePerHour, 1);
        }
    }
}