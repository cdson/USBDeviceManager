using System.Collections.Generic;

namespace DeviceTrackerSupport
{
	public class PortableDeviceFolder : PortableDeviceObject
	{
		public IList<PortableDeviceObject> Files
		{
			get;
			set;
		}

		public PortableDeviceFolder(string id, string name) : base(id, name)
		{
			Files = new List<PortableDeviceObject>();
		}
	}
}