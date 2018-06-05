using System.Collections.Generic;
using System.Linq;

namespace DeviceTrackerSupport
{
	internal class StatusTracker
	{
		private static readonly Dictionary<string, string> DeviceMap;
		private static readonly List<string> DeviceStatusList;
		private static readonly object ObjLock;

        #region <-Constructor->
        static StatusTracker()
		{
			DeviceMap = new Dictionary<string, string>();
			DeviceStatusList = null;
			ObjLock = null;
			DeviceStatusList = new List<string>();
			ObjLock = new object();
		}

		public StatusTracker()
		{
		}
        #endregion

        public static void AddDevices(string deviceId, string displayName)
		{
			try
			{
				if (!DeviceMap.Keys.Contains<string>(deviceId))
				{
					DeviceMap.Add(deviceId, displayName);
				}
			}
			catch
			{
			}
		}

        public static void RemoveDevice(string deviceId)
        {
            DeviceMap.Remove(deviceId);
        }

		public static void ClearDeviceList()
		{
			DeviceMap.Clear();
		}


		public static Dictionary<string, string> GetDeviceList()
		{
			return DeviceMap;
		}


		public static List<string> GetMessages()
		{
			var strs = new List<string>();

			lock (ObjLock)
			{
				strs.AddRange(DeviceStatusList);
				DeviceStatusList.Clear();
			}
			return strs;
		}

        public static void AddMessage(string msg)
        {
            DeviceStatusList.Add(msg);
        }
	}
}