using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SA1.Models;

namespace SA1.Repository
{
    public class DeviceRepository : IDeviceRepository
    {
        private List<Device> _devices = new List<Device>();
        private readonly string _filePath = "devices.json"; 

        public DeviceRepository()
        {
            LoadData(); 
        }

        public void AddDevice(Device device)
        {
            _devices.Add(device);
            SaveData(); 
        }
        
        public Device GetDeviceById(Guid id) => _devices.FirstOrDefault(d => d.Id == id);
        
        public IEnumerable<Device> GetAllDevices() => _devices;

        public void SaveData()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_devices, options);
            File.WriteAllText(_filePath, json);
        }

        private void LoadData()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    _devices = JsonSerializer.Deserialize<List<Device>>(json) ?? new List<Device>();
                }
            }
        }
    }
}