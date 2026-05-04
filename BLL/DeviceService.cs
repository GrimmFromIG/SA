using System;
using System.Collections.Generic;
using SA.DAL;
using SA.Models;

namespace SA.BLL
{
    public class DeviceService
    {
        private readonly IDeviceRepository _repository;

        public DeviceService(IDeviceRepository repository)
        {
            _repository = repository;
        }

        public void RegisterNewDevice(Device device)
        {
            _repository.AddDevice(device);
        }

        public IEnumerable<Device> GetAllDevices()
        {
            return _repository.GetAllDevices();
        }

        // Бізнес-методи для взаємодії з пристроями
        public void MakeDeviceWork(Guid deviceId)
        {
            var device = _repository.GetDeviceById(deviceId);
            if (device != null)
            {
                // В ідеальному світі немає перевірок на заряд чи ПЗ
                device.Work();
            }
        }

        public void MakeDevicePlay(Guid deviceId)
        {
            var device = _repository.GetDeviceById(deviceId);
            device?.Play();
        }

        public void MakeDeviceWatchVideo(Guid deviceId)
        {
            var device = _repository.GetDeviceById(deviceId);
            device?.WatchVideo();
        }
    }
}