using System;
using System.Collections.Generic;
using SA1.Models; // Або APPZ_Lab1.Models

namespace SA1.DAL
{
    public interface IDeviceRepository
    {
        void AddDevice(Device device);
        Device GetDeviceById(Guid id);
        IEnumerable<Device> GetAllDevices();
    }
}