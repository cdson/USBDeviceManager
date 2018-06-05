using System.IO;
using System.Linq;

namespace DeviceTrackerSupport
{
	public static class Utility
	{
		private static readonly string[] MediaExt;

		static Utility()
		{
			var strArrays = new[] { ".jpg", ".png", ".m4a", ".mpg" };

			MediaExt = strArrays;
		}

		public static string GetIPhoneDescription(string deviceType)
		{
			string str = deviceType;
			string str1 = str;
			if (str != null)
			{
				switch (str1)
				{
					case "iPhone1,1":
					{
						return "iPhone 2G";
					}
					case "iPhone1,2":
					{
						return "iPhone 3G";
					}
					case "iPhone2,1":
					{
						return "iPhone 3G[S]";
					}
					case "iPhone3,1":
					{
						return "iPhone 4 [GSM]";
					}
					case "iPhone3,3":
					{
						return "iPhone 4 [CDMA]";
					}
					case "iPhone4,1":
					{
						return "iPhone 4S";
					}
					case "iPhone5,1":
					{
						return "iPhone 5";
					}
					case "iPod1,1":
					{
						return "iPod Touch 1G";
					}
					case "iPod2,1":
					{
						return "iPod Touch 2G";
					}
					case "iPod3,1":
					{
						return "iPod Touch 3G";
					}
					case "iPod4,1":
					{
						return "iPod Touch 4";
					}
					case "iPad1,1":
					{
						return "iPad 1G";
					}
					case "iPad2,1":
					{
						return "iPad 2 [WiFi]";
					}
					case "iPad2,2":
					{
						return "iPad 2 [3G-GSM]";
					}
					case "iPad2,3":
					{
						return "iPad 2 [3G-CDMA]";
					}
				}
			}
			return string.Empty;
		}

		public static bool IsMediaFiles(string path)
		{
			string extension = Path.GetExtension(path);
			if (string.IsNullOrEmpty(extension))
			{
				return false;
			}
			if (MediaExt.Any(p => string.Compare(p, extension, true) == 0))
			{
				return true;
			}
			return false;
		}
	}
}