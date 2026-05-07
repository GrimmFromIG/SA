using SA1.Models;

namespace SA1.Services
{
    public class DeviceService
    {
        public bool ExecuteAction(Device device, string action, bool isIntensive, int hours, out string resultMessage)
        {
            if (action == "Зарядка" && !device.Settings.IsPluggedIn)
            {
                resultMessage = "[ПОМИЛКА] Кабель живлення не підключено! Зайдіть у Налаштування (пункт 3) і увімкніть його.";
                return false;
            }

            if (device.BatteryCapacityCurrent <= 0 && !device.Settings.IsPluggedIn)
            {
                resultMessage = "[ПОМИЛКА] Пристрій повністю розряджений! Підключіть його до мережі.";
                return false;
            }

            if (!device.Settings.HasSoftware && action != "Зарядка")
            {
                resultMessage = "[ПОМИЛКА] Не встановлено ПЗ!";
                return false;
            }
            if (action == "Друк" && !device.Settings.HasPeripherals)
            {
                resultMessage = "[ПОМИЛКА] Не підключено принтер у налаштуваннях!";
                return false;
            }

            if (device.Settings.IsPluggedIn)
            {
                double chargeRate = action == "Зарядка" ? (device.BatteryCapacityMax / 2.0) :
                                   isIntensive ? (device.BatteryCapacityMax / 5.0) : (device.BatteryCapacityMax / 3.0);

                device.BatteryCapacityCurrent += chargeRate * hours;
                if (device.BatteryCapacityCurrent > device.BatteryCapacityMax)
                    device.BatteryCapacityCurrent = device.BatteryCapacityMax;
                
                device.SetActivity(action);
                resultMessage = $"[УСПІХ] '{action}' ({hours} год). Пристрій підключено до мережі. Заряд: {device.BatteryCapacityCurrent:F0} мАг";
                device.LogAction(resultMessage);
                return true;
            }
            else
            {
                double drainPerHour = CalculateDrain(device, isIntensive);
                double maxPossibleHours = device.BatteryCapacityCurrent / drainPerHour;

                if (hours <= maxPossibleHours)
                {
                    device.BatteryCapacityCurrent -= drainPerHour * hours;
                    device.SetActivity(action);
                    
                    resultMessage = $"[УСПІХ] Дію '{action}' виконано повністю ({hours} год). Залишок заряду: {device.BatteryCapacityCurrent:F0} мАг";
                    device.LogAction(resultMessage);
                    return true;
                }
                else
                {
                    device.BatteryCapacityCurrent = 0;
                    device.SetActivity("Вимкнено (розряджений)");
                    
                    resultMessage = $"[УВАГА] Пристрій був у режимі '{action}', але розрядився через {maxPossibleHours:F1} год.";
                    device.LogAction(resultMessage);
                    return true; 
                }
            }
        }

        private double CalculateDrain(Device d, bool intensive)
        { 
            if (d.BatteryCapacityMax <= 3000)
                return intensive ? (d.BatteryCapacityMax / 16.0) : (d.BatteryCapacityMax / 48.0);
            
            return intensive ? (d.BatteryCapacityMax / 4.0) : (d.BatteryCapacityMax / 12.0);
        }
    }
}