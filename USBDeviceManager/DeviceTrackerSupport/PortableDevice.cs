using PortableDeviceApiLib;
using PortableDeviceTypesLib;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DeviceTrackerSupport
{
	public class PortableDevice
	{
		private bool _isConnected;
		private readonly PortableDeviceClass _device;

        #region <-Properties->
        public string DeviceId
		{
			get;
			set;
		}

		public string FriendlyName
		{
			get
			{
				IPortableDeviceContent portableDeviceContent;
				IPortableDeviceProperties portableDeviceProperty;
				PortableDeviceApiLib.IPortableDeviceValues portableDeviceValue;
				string str;

				if (!_isConnected)
				{
					throw new InvalidOperationException("Not connected to device.");
				}

				_device.Content(out portableDeviceContent);
				portableDeviceContent.Properties(out portableDeviceProperty);
				portableDeviceProperty.GetValues("DEVICE", null, out portableDeviceValue);

				PortableDeviceApiLib._tagpropertykey __tagpropertykey = new PortableDeviceApiLib._tagpropertykey()
				{
					fmtid = new Guid(651466650, 58947, 17958, 158, 43, 115, 109, 192, 201, 47, 220),
					pid = 12
				};

				portableDeviceValue.GetStringValue(ref __tagpropertykey, out str);
				return str;
			}
		}

        #endregion

        public PortableDevice(string deviceId)
		{
			_device = new PortableDeviceClass();
			DeviceId = deviceId;
		}

		public void Connect()
		{
			if (this._isConnected)
			{
				return;
			}

			PortableDeviceApiLib.IPortableDeviceValues portableDeviceValuesClass = (PortableDeviceApiLib.IPortableDeviceValues)(new PortableDeviceValuesClass());

            //added by AA
            if (_device != null && DeviceId!=null)
            {
                this._device.Open(this.DeviceId, portableDeviceValuesClass);
                this._isConnected = true;
            }
		}

		public void CopyFilefromDevice(PortableDeviceFile file, string saveToPath)
		{
			unsafe
			{
				IPortableDeviceContent portableDeviceContent;
				IPortableDeviceResources portableDeviceResource;
				PortableDeviceApiLib.IStream stream;
				int num = 0;

				_device.Content(out portableDeviceContent);
				portableDeviceContent.Transfer(out portableDeviceResource);

				uint num1 = 0;
				PortableDeviceApiLib._tagpropertykey __tagpropertykey = new PortableDeviceApiLib._tagpropertykey()
				{
					fmtid = new Guid(-400655938, 13552, 16831, 181, 63, 241, 160, 106, 232, 120, 66),
					pid = 0
				};

				portableDeviceResource.GetStream(file.Id, ref __tagpropertykey, 0, ref num1, out stream);
				System.Runtime.InteropServices.ComTypes.IStream stream1 = (System.Runtime.InteropServices.ComTypes.IStream)stream;

				string str = (string.IsNullOrEmpty(file.Name) ? Path.GetFileName(file.Id) : Path.GetFileName(file.Name));

				var fileStream = new FileStream(Path.Combine(saveToPath, str), FileMode.Create, FileAccess.Write);

				byte[] numArray = new byte[1024];
				do
				{
					//stream1.Read(numArray, 1024, new IntPtr(ref num));
                    stream1.Read(numArray, 1024, new IntPtr(& num));
					fileStream.Write(numArray, 0, 1024);
				}
				while (num > 0);

				fileStream.Close();

				Marshal.ReleaseComObject(stream1);
				Marshal.ReleaseComObject(stream);
			}
		}

		public void Disconnect()
		{
			if (!_isConnected)
			{
				return;
			}

			_device.Close();
			_isConnected = false;
		}

		private static void EnumerateContents(ref IPortableDeviceContent content, PortableDeviceFolder parent)
		{
			IPortableDeviceProperties portableDeviceProperty;
			IEnumPortableDeviceObjectIDs enumPortableDeviceObjectId;
			string str;

			content.Properties(out portableDeviceProperty);
			content.EnumObjects(0, parent.Id, null, out enumPortableDeviceObjectId);
			uint num = 0;

			do
			{
				enumPortableDeviceObjectId.Next(1, out str, ref num);
				if (num <= 0)
				{
					continue;
				}
				PortableDeviceObject portableDeviceObject = WrapObject(portableDeviceProperty, str);

				parent.Files.Add(portableDeviceObject);

				if (!(portableDeviceObject is PortableDeviceFolder))
				{
					continue;
				}

				EnumerateContents(ref content, (PortableDeviceFolder)portableDeviceObject);
			}
			while (num > 0);
		}

		public PortableDeviceFolder GetContents()
		{
			IPortableDeviceContent portableDeviceContent;

			PortableDeviceFolder portableDeviceFolder = new PortableDeviceFolder("DEVICE", "DEVICE");

			this._device.Content(out portableDeviceContent);
			EnumerateContents(ref portableDeviceContent, portableDeviceFolder);

			return portableDeviceFolder;
		}

		private static PortableDeviceObject WrapObject(IPortableDeviceProperties properties, string objectId)
		{
			PortableDeviceApiLib.IPortableDeviceKeyCollection portableDeviceKeyCollection;
			PortableDeviceApiLib.IPortableDeviceValues portableDeviceValue;
			Guid guid;

			properties.GetSupportedProperties(objectId, out portableDeviceKeyCollection);
			properties.GetValues(objectId, portableDeviceKeyCollection, out portableDeviceValue);

			string empty = string.Empty;
			PortableDeviceApiLib._tagpropertykey __tagpropertykey = new PortableDeviceApiLib._tagpropertykey()
			{
				fmtid = new Guid(-278181619, 23768, 17274, 175, 252, 218, 139, 96, 238, 74, 60)
			};

			__tagpropertykey = new PortableDeviceApiLib._tagpropertykey()
			{
				fmtid = new Guid(-278181619, 23768, 17274, 175, 252, 218, 139, 96, 238, 74, 60),
				pid = 7
			};

			PortableDeviceApiLib._tagpropertykey __tagpropertykey1 = __tagpropertykey;
			portableDeviceValue.GetGuidValue(ref __tagpropertykey1, out guid);

			Guid guid1 = new Guid(669180818, 41233, 18656, 171, 12, 225, 119, 5, 160, 95, 133);
			Guid guid2 = new Guid(-1712520864, 6143, 19524, 157, 152, 29, 122, 111, 148, 25, 33);

			try
			{
				__tagpropertykey.pid = 12;
				PortableDeviceApiLib._tagpropertykey __tagpropertykey2 = __tagpropertykey;
				portableDeviceValue.GetStringValue(ref __tagpropertykey2, out empty);
			}
			catch
			{
			}

			if (!(guid == guid1) && !(guid == guid2))
			{
				return new PortableDeviceFile(objectId, empty);
			}

			return new PortableDeviceFolder(objectId, empty);
		}
	}
}