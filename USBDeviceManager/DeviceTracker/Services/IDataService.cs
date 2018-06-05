using System;
using DeviceTracker.Model;

namespace DeviceTracker.Services
{
    public interface IDataService
    {
        void GetData(Action<DataItem, Exception> callback);
    }
}
