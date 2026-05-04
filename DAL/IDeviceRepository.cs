using System;
using System.Collections.Generic;
using SA.Models;

namespace SA.DAL
{
    public interface IDeviceRepository
    {
        void AddDevice(Device device);
        Device GetDeviceById(Guid id);
        IEnumerable<Device> GetAllDevices();
    }
}