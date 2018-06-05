using System;

namespace DeviceTracker.Common
{
    public class Message
    {
        public const string ShowConfirmationDialog = @"ShowConfirmationDialog";
    }

    public class SelectFolderPathMessage
    {
        //Callback property with a param
        public Action<string> Callback { get; set; }

        //ctor accepts callback.
        public SelectFolderPathMessage(Action<string> callback)
        {
            Callback = callback;
        }
    }

    public class ConfirmationMessage
    {
        public string DeviceName { get; set; }
        public Action<DeviceImportOption> Callback { get; set; }
        public ConfirmationMessage(string deviceName, Action<DeviceImportOption> callback)
        {
            DeviceName = deviceName;
            Callback = callback;
        }

    }
}
