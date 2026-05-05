using System;
using System.Collections.Generic;
using System.Linq;
using SA1.Models;

namespace SA1.DAL
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly List<Device> _devices = new List<Device>();

        public void AddDevice(Device device) => _devices.Add(device);
        
        public Device GetDeviceById(Guid id) => _devices.FirstOrDefault(d => d.Id == id);
        
        public IEnumerable<Device> GetAllDevices() => _devices;
    }
}