using System;
using System.Diagnostics;
using System.Management;

namespace DeviceTrackerSupport
{
	public class WmiTracker
	{
		private static ManagementEventWatcher _watcherDevices;

		public WmiTracker()
		{
		}

		private static void DeviceChangeEventReceived(object sender, EventArrivedEventArgs e)
		{
			try
			{
				ManagementBaseObject newEvent = e.NewEvent;
				if (newEvent.ClassPath.ClassName.Equals("__InstanceCreationEvent"))
				{
					StatusTracker.AddMessage("A drive was connected");
				}
				else if (newEvent.ClassPath.ClassName.Equals("__InstanceDeletionEvent"))
				{
					StatusTracker.AddMessage("A drive was removed");
				}
				var item = (ManagementBaseObject)e.NewEvent["TargetInstance"];
				StatusTracker.AddMessage(string.Concat("Drive type is ", item.Properties["DriveType"].Value));
			}
			catch (Exception exception)
			{
				Debug.WriteLine(string.Concat("Error:DeviceChangeEventReceived=", exception.Message));
			}
		}

		public static void Initialize()
		{
			try
			{
				_watcherDevices = new ManagementEventWatcher("SELECT * FROM __InstanceOperationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'");
				_watcherDevices.EventArrived += DeviceChangeEventReceived;
				_watcherDevices.Start();
			}
			catch (Exception exception)
			{
				Debug.WriteLine(string.Concat("Error:Initialize=", exception.Message));
			}
		}

		public static void Stop()
		{
			if (_watcherDevices != null)
			{
				_watcherDevices.Stop();
				_watcherDevices.Dispose();
			}
		}
	}
}