using System;
using DeviceTracker.Common;
using GalaSoft.MvvmLight;

namespace DeviceTracker.Model
{
    /// <summary>
    /// Model
    /// </summary>
    [Serializable]
    public class Device : ICloneable//ViewModelBase
    {
        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                //RaisePropertyChanged();
            }
        }

        private string _displayName = string.Empty;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                //RaisePropertyChanged();
            }
        }

        private DeviceImportOption _importOption;
        public DeviceImportOption ImportOption
        {
            get { return _importOption; }
            set
            {
                _importOption = value;
                //RaisePropertyChanged();
            }
        }

        public object Clone()
        {
            return new Device()
            {
                Id = Id,
                DisplayName = DisplayName,
                ImportOption = ImportOption,
            };
        }
    }
}
