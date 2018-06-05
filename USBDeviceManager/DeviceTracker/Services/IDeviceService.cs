using System.Collections.Generic;
using System.Threading.Tasks;
using DeviceTracker.Model;

namespace DeviceTracker.Services
{
    /// <summary>
    /// interface
    /// </summary>
    public interface IDeviceService
    {
        Task<int> AddDevice(Device @device);
        Task<int> RemoveDevice(Device @device);
        Task<int> UpdateDevice(Device @device);
        Task<List<Device>> GetAllDevices();
        Task<int> GetDeviceCount();
        Task<Device> GetDeviceById(string id);
    }
}