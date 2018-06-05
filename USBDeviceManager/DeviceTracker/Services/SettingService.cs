using System;

namespace DeviceTracker.Services
{
    public class SettingService:ISettingService
    {
        private readonly object _locker = new object();

        private bool _isInitialRun = true;
        public bool IsInitialRun
        {
            get { return _isInitialRun; }
            set
            {
                if (_isInitialRun != value)
                {
                    _isInitialRun = value;

                    Properties.Settings.Default.IsInitialRun = value;
                    Save();
                }
            }
        }

        private bool _deviceBackupEnabled=true;
        public bool DeviceBackupEnabled
        {
            get { return _deviceBackupEnabled; }
            set
            {
                if (_deviceBackupEnabled != value)
                {
                    _deviceBackupEnabled = value;

                    Properties.Settings.Default.DeviceBackupEnabled = value;
                    Save();
                }
            }
        }

        private string _mediaBackupPath;
        public string MediaBackupPath
        {
            get { return _mediaBackupPath; }
            set
            {
                if (_mediaBackupPath != value)
                {
                    _mediaBackupPath = value;

                    Properties.Settings.Default.MediaBackupPath = value;
                    Save();
                }
            }
        }

        private bool _defaultMediaBackupPathEnabled=true;
        public bool DefaultMediaBackupPathEnabled
        {
            get { return _defaultMediaBackupPathEnabled; }
            set
            {
                if (_defaultMediaBackupPathEnabled != value)
                {
                    _defaultMediaBackupPathEnabled = value;

                    Properties.Settings.Default.DefaultMediaBackupPathEnabled = value;
                    Save();
                }
               
            }
        }

        public SettingService()
        {
            _isInitialRun = Properties.Settings.Default.IsInitialRun;

            if (_isInitialRun)
            {
                _deviceBackupEnabled = true;
                _defaultMediaBackupPathEnabled = true;

                //set flag to false
                IsInitialRun = false;
            }
            else
            {
                _deviceBackupEnabled = Properties.Settings.Default.DeviceBackupEnabled;
                _mediaBackupPath = Properties.Settings.Default.MediaBackupPath;
                _defaultMediaBackupPathEnabled = Properties.Settings.Default.DefaultMediaBackupPathEnabled;    
            }

            if (String.IsNullOrEmpty(_mediaBackupPath))
            {
                _mediaBackupPath =
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                                           @"DeviceTracker");
            }

        }

        private void Save()
        {
            lock (_locker)
            {
                Properties.Settings.Default.Save();
            }
        }
    }
}