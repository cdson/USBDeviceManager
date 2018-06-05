using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceTracker.Common;
using DeviceTracker.Model;

namespace DeviceTracker.Services
{
    public class DeviceService : IDeviceService
    {
        private List<Device> _devices;
        public DeviceService()
        {
            _devices = new List<Device>();   
        }

        public async Task<int> AddDevice(Device @device)
        {
            try
            {
                if (@device == null)
                    throw new ArgumentException(Constant.STRING_DEVICE);

                var localDevice = await GetLocalCopyById(@device.Id);

                if (localDevice != null)
                    await RemoveDevice(localDevice);

                _devices.Add(@device);

                return Constant.INT32_SUCCESS;
            }
            finally
            {
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(_devices);
                Utility.WriteToFile(data);
            }            
        }

        public async Task<Int32> RemoveDevice(Device @device)
        {
            try
            {
                if (@device == null)
                    throw new ArgumentException(Constant.STRING_DEVICE);

                var localDevice = await GetLocalCopyById(@device.Id);
                if (localDevice != null && _devices.Remove(localDevice))
                {
                    return Constant.INT32_SUCCESS;
                }

                return Constant.INT32_NOTFOUND;
            }
            finally
            {
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(_devices);
                Utility.WriteToFile(data);
            }
        }

        public async Task<Int32> UpdateDevice(Device @device)
        {
            try
            {
                if (@device == null)
                    throw new ArgumentException(Constant.STRING_DEVICE);

                var localDevice = await GetLocalCopyById(@device.Id);

                if (localDevice != null)
                    await RemoveDevice(localDevice);

                _devices.Add(@device);
            }
            finally
            {
                var data = Newtonsoft.Json.JsonConvert.SerializeObject(_devices);
                Utility.WriteToFile(data);
            }            

            return Constant.INT32_SUCCESS;
        }

        public async Task<List<Device>> GetAllDevices()
        {
            try
            {
                //testCode
                //await Task.Delay(2000);

                const string fileName = "fileStorage.json";
                if (_devices == null || _devices.Count == 0)
                {
                    if (System.IO.File.Exists(fileName))
                    {
                        var data = System.IO.File.ReadAllText(fileName);
                        if (!String.IsNullOrEmpty(data))
                            return _devices = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Device>>(data);//=DataStore.GetStoredDevices();
                    }
                }
                return _devices;                
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }

        public async Task<int> GetDeviceCount()
        {
            return _devices.Count;
        }
        

        /// <summary>
        /// returns deep-copy object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Device> GetDeviceById(string id)
        {
            var @device = _devices.SingleOrDefault(p => p.Id.Equals(id));

            if (@device == null)
                @device = null;

            //@device= Utility.DeepCopy.Make(@device);
            
            return @device;
        }
        public async Task<Device> GetLocalCopyById(string id)
        {
            return _devices.SingleOrDefault(p => p.Id.Equals(id));
        }
    }
}