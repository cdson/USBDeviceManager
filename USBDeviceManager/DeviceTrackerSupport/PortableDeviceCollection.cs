using PortableDeviceApiLib;
using System;
using System.Collections.ObjectModel;

namespace DeviceTrackerSupport
{
    public class PortableDeviceCollection : Collection<PortableDevice>
	{
		private readonly PortableDeviceManager _deviceManager;

		public PortableDeviceCollection()
		{
			_deviceManager = new PortableDeviceManagerClass();
		}

		public void Refresh()
		{
			unsafe
			{
				_deviceManager.RefreshDeviceList();
				string[] strArrays = null;
				uint num = 0;
				_deviceManager.GetDevices(strArrays, ref num);
				strArrays = new string[num];
				_deviceManager.GetDevices(strArrays, ref num);
				string[] strArrays1 = strArrays;
				for (int i = 0; i < (int)strArrays1.Length; i++)
				{
                    base.Add(new PortableDevice(strArrays1[i]));
				}
			}
		}
	}
}