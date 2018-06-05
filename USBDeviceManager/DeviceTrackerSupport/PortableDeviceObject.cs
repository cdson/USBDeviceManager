using System;
using System.Runtime.CompilerServices;

namespace DeviceTrackerSupport
{
	public abstract class PortableDeviceObject
	{
		public string Id
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		protected PortableDeviceObject(string id, string name)
		{
			Id = id;
			Name = name;
		}
	}
}