using System;
using System.Diagnostics;
using System.Windows.Input;
using DeviceTracker.Common;
using DeviceTracker.Services;
using GalaSoft.MvvmLight;
using DeviceTracker.Model;
using DeviceTrackerSupport;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.CommandWpf;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utility = DeviceTracker.Common.Utility;

namespace DeviceTracker.ViewModel
{
   
    public class MainViewModel : ViewModelBase
    {
        #region <-PrivateVars->
        private readonly IDataService _dataService;
        private readonly ISettingService _settingService;
        private readonly IDeviceService _deviceService;
        private DriveDetector _driveDetector = null;
        private PortableDeviceCollection _connectedDeviceCollection;
        private Queue<Device> _deviceProcessingQueue;
        private readonly object _locker = new object();
        private readonly object _locker2 = new object();
        private static bool _progressFlag = false;
        private static bool _copyInProgress = false;
        private Device _activeDevice = null;
        #endregion

        #region <-Properties->
        private bool _isDeviceBackupEnabled = false;
        public bool IsDeviceBackupEnabled
        {
            get { return _isDeviceBackupEnabled; }
            set
            {
                _isDeviceBackupEnabled = value;
                RaisePropertyChanged();

                _settingService.DeviceBackupEnabled = value;

                if (_isDeviceBackupEnabled)
                {
                    RegisterDriveDetector();
                }
                else
                {
                    UnRegisterDriveDetector();
                }

                RefreshStoredDevices();
            }
        }

        private bool _isDefaultMediaBackupPathEnabled = false;
        public bool IsDefaultMediaBackupPathEnabled
        {
            get { return _isDefaultMediaBackupPathEnabled; }
            set
            {
                _isDefaultMediaBackupPathEnabled = value;
                RaisePropertyChanged();

                _settingService.DefaultMediaBackupPathEnabled = value;
            }
        }

        private string _defaultMediaBackupPath;
        public string DefaultMediaBackupPath
        {
            get { return _defaultMediaBackupPath; }
            set
            {
                _defaultMediaBackupPath = value;
                RaisePropertyChanged();

                _settingService.MediaBackupPath = value;
            }
        }

        private ObservableCollection<Device> _storedDevices=new ObservableCollection<Device>();
        public ObservableCollection<Device> StoredDevices
        {
            get { return _storedDevices; }
            set { _storedDevices = value;
            RaisePropertyChanged();}
        }

        private ObservableCollection<string> _testData;
        public ObservableCollection<string> TestData
        {
            get { return _testData; }
            set
            {
                _testData = value;
                RaisePropertyChanged();
            }
        }

        private string _infoText;
        public string InfoText
        {
            get
            {
                if (!IsDeviceBackupEnabled)
                    _infoText = "Check the option to detect connected devices!";
                else
                    _infoText = @"No devices are connected!";
                
                return _infoText;
            }
            set { _infoText = value;
            RaisePropertyChanged();}
        }

        private bool _defaultView;
        public bool DefaultView
        {
            get { return _defaultView= IsDeviceBackupEnabled && StoredDevices.Count > 0; }
            set { _defaultView = value;
            RaisePropertyChanged();}
        }
        #endregion

        #region <-Commands->
        public ICommand BrowseMediaBackupPathCommand
        {
            get { return new RelayCommand(BrowseMediaBackupPathCommandExecute, CanBrowseMediaBackupPathCommandExecute); }
        }

        public ICommand ImportOptionToggleCommand
        {
            get { return new RelayCommand<Device>(ImportOptionToggleCommandExecute); }
        }

        public ICommand ClearDeviceCommand
        {
            get { return new RelayCommand<Device>(ClearDeviceCommandExecute); }
        }

        public ICommand TestCommand
        {
            get { return new GalaSoft.MvvmLight.CommandWpf.RelayCommand(OnTestCommandExecute); }
        }

        public ICommand RefreshCommand
        {
            get { return new RelayCommand(RefreshCommandExecute); }
        }

        #endregion

        #region <-Constructor->
        public MainViewModel(IDataService dataService,ISettingService settingService, IDeviceService deviceService)
        {
            _dataService = dataService;
            _settingService = settingService;
            _deviceService = deviceService;
        }
        #endregion

        #region <-Init->
        public async void InitDevice()
        {
            _connectedDeviceCollection = new PortableDeviceCollection();
            _deviceProcessingQueue = new Queue<Device>();


            //retrieve settings from db and
            IsDeviceBackupEnabled = _settingService.DeviceBackupEnabled;
            IsDefaultMediaBackupPathEnabled = _settingService.DefaultMediaBackupPathEnabled;
            DefaultMediaBackupPath = _settingService.MediaBackupPath;

            //create Default-MediaBackupPath
            if (!Directory.Exists(DefaultMediaBackupPath))
            {
                try
                {
                    Directory.CreateDirectory(DefaultMediaBackupPath);
                }
                catch {/*parse*/}
            }

            if (IsDeviceBackupEnabled)
            {
                CheckDeviceStatusChange();
            }
        }
        #endregion

        #region <-CommandMethods->
        private void OnTestCommandExecute()
        {
            Messenger.Default.Send(new ConfirmationMessage("string", (response) =>
            {   
                Utility.LogMessage(response);
            }));
        }

        private bool CanBrowseMediaBackupPathCommandExecute()
        {
            return IsDefaultMediaBackupPathEnabled;
        }
        private void BrowseMediaBackupPathCommandExecute()
        {
            try
            {
                Messenger.Default.Send(new SelectFolderPathMessage((path) =>
                {
                    OnMediaFolderSelected(path);
                }));
            }
            catch (Exception ex)
            {
                Utility.LogException(ex);
            }
        }

        private async void ImportOptionToggleCommandExecute(Device device)
        {
            try
            {
                //get the device by id from data-store
                var storedDevice = await _deviceService.GetDeviceById(device.Id);

                //update the check-field
                storedDevice.ImportOption = (device.ImportOption == DeviceImportOption.Always ?
                    DeviceImportOption.Never : DeviceImportOption.Always);

                //update same in db.
                await _deviceService.UpdateDevice(storedDevice);
                //_deviceService.SetImportOption(storedDevice.Id, storedDevice.ImportOption);


                await RefreshStoredDevices();

                //if importoption toggled to active for any device,check status change
                if (storedDevice.ImportOption == DeviceImportOption.Always)
                {
                    CheckDeviceStatusChange();
                }
                else if (_activeDevice != null && _activeDevice.Id.Equals(storedDevice.Id))
                {
                    _activeDevice.ImportOption = storedDevice.ImportOption;
                }
            }
            catch (Exception ex)
            {
                Utility.LogException(ex);
            }
        }

        //remove device from db
        private async void ClearDeviceCommandExecute(Device device)
        {
            try
            {
                await _deviceService.RemoveDevice(device);
                await RefreshStoredDevices();
            }
            catch (Exception ex)
            {
                Utility.LogException(ex);
            }
        }

        private void RefreshCommandExecute()
        {
            CheckDeviceStatusChange();
        }
        #endregion

        #region <-Events->
        void _driveDetector_DeviceStatusChanged(object sender, DriveDetectorEventArgs e)
        {
            try
            {
                CheckDeviceStatusChange();
            }
            catch (Exception ex)
            {
                Utility.LogException(ex);
            }
            finally
            {
                _progressFlag = false;
            }
        }

        //[Not used]
        void _driveDetector_DeviceArrived(object sender, DriveDetectorEventArgs e)
        {
            try
            {
                Utility.LogMessage("Device arrived");
            }
            catch (Exception ex)
            {
                Utility.LogException(ex);
            }
        }

        //[Not used]
        void _driveDetector_DeviceRemoved(object sender, DriveDetectorEventArgs e)
        {
            try
            {
                Utility.LogMessage("Device removed");
            }
            catch (Exception ex)
            {
                Utility.LogException(ex);
            }
        }

        //[Not used]
        void _driveDetector_QueryRemove(object sender, DriveDetectorEventArgs e)
        {
            try
            {
                Utility.LogMessage("QueryRemove");
            }
            catch (Exception ex)
            {
                Utility.LogException(ex);
            }

        }
        #endregion

        #region <-PrivateMethods->

        #region <Register/UnRegister>
        private void RegisterDriveDetector()
        {
            try
            {
                _driveDetector = new DriveDetector();
                _driveDetector.DeviceArrived += _driveDetector_DeviceArrived;
                //_driveDetector.QueryRemove += _driveDetector_QueryRemove;
                _driveDetector.DeviceRemoved += _driveDetector_DeviceRemoved;
                _driveDetector.DeviceStatusChanged += _driveDetector_DeviceStatusChanged;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void UnRegisterDriveDetector()
        {
            try
            {
                if (_driveDetector != null)
                {
                    _driveDetector.DeviceArrived -= _driveDetector_DeviceArrived;
                    //_driveDetector.QueryRemove -= _driveDetector_QueryRemove;
                    _driveDetector.DeviceRemoved -= _driveDetector_DeviceRemoved;
                    _driveDetector.DeviceStatusChanged -= _driveDetector_DeviceStatusChanged;
                    _driveDetector.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        private void CheckDeviceStatusChange()
        {
            try
            {
                lock (_locker)
                {
                    var connectedDevices = GetConnectedDevices();
                    foreach (var device in connectedDevices)
                    {
                        if (_deviceProcessingQueue.SingleOrDefault(p => p.Id.Equals(device.Id)) == null)
                        {
                            Utility.LogMessage(@"::[CheckDeviceStatusChange] [Queueing the device {0}]::", device.DisplayName);
                            _deviceProcessingQueue.Enqueue(device);

                            OnEqueue();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //Utility.LogMessage(@"[CheckDeviceStatusChange][Finally]");
            }
        }

        private void OnEqueue()
        {
            if(System.Threading.Thread.CurrentThread.IsBackground)
                Utility.LogMessage("[OnEqueue] [IsBackground]");

            try
            {
                if (!_progressFlag)
                {
                    if (_deviceProcessingQueue.Count > 0)
                    {
                        _progressFlag = true;

                        var device = _deviceProcessingQueue.Peek();
                        if (device != null)
                        {
                            //start processing device
                            OnDeviceStatusChange(device);

                            //after processing,dequeue the element
                            var dequeuedDevice = _deviceProcessingQueue.Dequeue();
                            Utility.LogMessage(@"[OnEqueue] [Finally dequeuing device {0} from processing queue]",
                                              dequeuedDevice.DisplayName);

                            _progressFlag = false;

                            Utility.LogMessage(@"___________________________________________________________");

                            //System.Threading.Thread.Sleep(1000);
                            OnEqueue();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                
            }
        }

        private async void OnDeviceStatusChange(Device connectedDevice)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
                Utility.LogMessage("[OnDeviceStatusChange] [IsBackground]");

            try
            {
                //peek queue/process/dequeue
                Utility.LogMessage(@"[OnDeviceStatusChange] [Started processing the device {0}]", connectedDevice.DisplayName);

                var storedDevices = await _deviceService.GetAllDevices();

                var deviceStoredInDb = storedDevices.SingleOrDefault(p => p.Id.Equals(connectedDevice.Id));

                if (deviceStoredInDb == null)
                {
                    Utility.LogMessage(@"[OnDeviceStatusChange] [Device {0} is not present in the database.]", connectedDevice.DisplayName);
                    ManageNewDevices(connectedDevice);
                }
                else
                {
                    //same devices is stored in db
                    Utility.LogMessage(@"[OnDeviceStatusChange] [Device {0} is already present in database with import option {1}]",
                        deviceStoredInDb.DisplayName, deviceStoredInDb.ImportOption);

                    ManageStoredDevices(deviceStoredInDb);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ManageNewDevices(Device connectedDevice)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
                Utility.LogMessage("[ManageNewDevices] [IsBackground]");

            //call prompt
            Messenger.Default.Send(new ConfirmationMessage(connectedDevice.DisplayName, async (response) =>
                {
                    connectedDevice.ImportOption = response;
                    Utility.LogMessage("[ManageNewDevices] [Import option selected for device {0} is {1}]",
                                      connectedDevice.DisplayName, connectedDevice.ImportOption);

                    switch (response)
                    {
                        case DeviceImportOption.Always:
                            
                            //add-device in db
                            Utility.LogMessage("[ManageNewDevices] [Adding device {0} to database][STATUS=ALWAYS]", connectedDevice.DisplayName);
                            AddDevice(connectedDevice);

                            Utility.LogMessage("[ManageNewDevices] [Call to show notification for device:{0}!!!]", connectedDevice.DisplayName);

                            Task.Run(() =>
                                {
                                    ShowFileCountNotification(connectedDevice);

                                    //copy files -> default-media-path
                                    CopyDeviceContents(connectedDevice);
                                });
                            break;

                        case DeviceImportOption.Never:
                            //add-device in db
                            Utility.LogMessage("[ManageNewDevices] [Adding device {0} to database][STATUS=NEVER]", connectedDevice.DisplayName);
                            AddDevice(connectedDevice);
                            break;

                        case DeviceImportOption.JustOnce:
                            //no need to store in db.

                            //copy files->default-media-path
                            Utility.LogMessage("[ManageNewDevices] [Copy contents from device:{0}!!!][STATUS=JUSTONCE]", connectedDevice.DisplayName);
                            Task.Run(() =>
                                {
                                    CopyDeviceContents(connectedDevice);
                                });
                            
                            break;
                    }
                }));
        }

        private void ManageStoredDevices(Device storedDevice)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
                Utility.LogMessage("[ManageStoredDevices] [IsBackground]");

            try
            {
                switch (storedDevice.ImportOption)
                {
                   
                    case DeviceImportOption.Always:
                        Task.Run(() =>
                            {
                                Utility.LogMessage("[ManageStoredDevices] [Call to show notification for device:{0}]",
                                                  storedDevice.DisplayName);
                                ShowFileCountNotification(storedDevice);

                                //copy files -> default-media-path
                                Utility.LogMessage("[ManageStoredDevices] [Copy contents from device:{0}]",
                                                  storedDevice.DisplayName);
                                CopyDeviceContents(storedDevice);
                            });
                       
                        break;
                    case DeviceImportOption.Never:
                        //ignore
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void OnMediaFolderSelected(string selectedPath)
        {
            try
            {
                if (!String.IsNullOrEmpty(selectedPath))
                {
                    DefaultMediaBackupPath = selectedPath;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// get all devices connected to computer
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Device> GetConnectedDevices()
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
                Utility.LogMessage("[GetConnectedDevices] [IsBackground]");

            var devices = new List<Device>();

            try
            {
                _connectedDeviceCollection.Clear();
                _connectedDeviceCollection.Refresh();
                foreach (var connectedDevice in _connectedDeviceCollection)
                {
                    try
                    {
                        connectedDevice.Connect();

                        var deviceObj = new Device()
                        {
                            Id = connectedDevice.DeviceId,
                            DisplayName = connectedDevice.FriendlyName,
                        };

                        if (!devices.Contains(deviceObj))
                        {
                            devices.Add(deviceObj);
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        connectedDevice.Disconnect();
                    }
                }
            }
            catch
            {
                throw;
            }

            return devices;
        }

        private async void AddDevice(Device connectedDevice)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
                Utility.LogMessage("[AddDevice] [IsBackground]");

            try
            {
                var result = await _deviceService.AddDevice(connectedDevice);
                if (result != Constant.INT32_NOTFOUND)
                    await RefreshStoredDevices();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task RefreshStoredDevices()
        {
            try
            {
                if (System.Threading.Thread.CurrentThread.IsBackground)
                    Utility.LogMessage("[RefreshStoredDevices] [IsBackground]");

                var devices = await _deviceService.GetAllDevices();
                lock (_locker2)
                {

                    StoredDevices = new ObservableCollection<Device>(devices);
                    StoredDevices.SortDescending(p => p.DisplayName);
                    RaisePropertyChanged("StoredDevices");

                    RaisePropertyChanged("InfoText");
                    RaisePropertyChanged("DefaultView");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ShowFileCountNotification(Device connectedDevice)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
                Utility.LogMessage("[ShowFileCountNotification] [IsBackground]");
            else
                Utility.LogMessage("[ShowFileCountNotification] [Foreground]");

            PortableDevice portableDevice = null;
            try
            {
                if (_connectedDeviceCollection != null &&
                                _connectedDeviceCollection.Count > 0)
                {
                    //get the connect device of same Id
                    portableDevice = _connectedDeviceCollection.
                        SingleOrDefault(p => p.DeviceId.Equals(connectedDevice.Id));

                    if (portableDevice != null)
                    {
                        portableDevice.Connect();

                        //set active device to curren-device
                        Utility.LogMessage(@"[ShowFileCountNotification] [Setting_ActiveDevice] [{0}]",connectedDevice.DisplayName);
                        _activeDevice = connectedDevice;

                        PortableDeviceFolder deviceFolder = portableDevice.GetContents();

                        var fileCount = EnumerateContents(portableDevice, deviceFolder);
                        Utility.LogMessage("[ShowFileCountNotification] [Total number of files found -> {0}][Device::{1}]", fileCount,connectedDevice.DisplayName);

                        if (_validFileCounter > 0)
                        {
                            SystemTrayManager.ShowBalloonTip(balloonTipText: string.Format("{0} files to be copied, found in - {1} "
                                , _validFileCounter, connectedDevice.DisplayName));

                            Utility.LogMessage(string.Format("{0} files to be copied, found in - {1} "
                                , _validFileCounter, connectedDevice.DisplayName));
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (portableDevice != null)
                    portableDevice.Disconnect();

                Utility.LogMessage(@"[ShowFileCountNotification] [Finally] [Setting_ActiveDevice][NULL]");
                _activeDevice = null;

                _validFileCounter = 0;
                _totalFileCounter = 0;
            }
        }
        
        #region <-Enumerate->
        private static long _validFileCounter = 0;
        long _totalFileCounter;
        private long EnumerateContents(PortableDevice portableDevice, PortableDeviceFolder deviceFolder)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
                Utility.LogMessage("[EnumerateContents] [IsBackground]");
            else
                Utility.LogMessage("[EnumerateContents] [Foreground]");
            
            try
            {
                foreach (var fileObject in deviceFolder.Files)
                {
                    if (_activeDevice != null && _activeDevice.ImportOption == DeviceImportOption.Never)
                        break;

                    if (fileObject is PortableDeviceFile)
                    {
                        //if (Common.Utilities.Utility.FileExtensions.IsValidFileExtension(Path.GetExtension(fileObject.Name.ToLower())))
                        if (Utility.ListValidImageExtensions.Contains(Path.GetExtension(fileObject.Name).ToLower()))
                        {
                            _totalFileCounter++;

                            string str = (string.IsNullOrEmpty(fileObject.Name) ? Path.GetFileName(fileObject.Id)
                                : Path.GetFileName(fileObject.Name));

                            if (!File.Exists(Path.Combine(_defaultMediaBackupPath, str)))
                            {
                                 //Utility.LogMessage("{0} {1}", fileObject.Id, fileObject.Name);
                                _validFileCounter++;
                            }
                        }
                    }
                    else
                    {
                        //recursive call to enumerate contents
                        EnumerateContents(portableDevice, (PortableDeviceFolder)fileObject);
                    }
                }
                return _totalFileCounter;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region <-Copy->
        private void CopyDeviceContents(Device device)
        {
            //if (System.Threading.Thread.CurrentThread.IsBackground)
            //    Utility.LogMessage("[CopyDeviceContents] [Task] [IsBackground][Device::{0}]",device.DisplayName);

            PortableDevice portableDevice = null;
            try
            {
                if (_connectedDeviceCollection != null &&
                    _connectedDeviceCollection.Count > 0)
                {
                    //get the connect device of same Id
                    portableDevice = _connectedDeviceCollection.
                        SingleOrDefault(p => p.DeviceId.Equals(device.Id));

                    if (portableDevice != null)
                    {
                        portableDevice.Connect();

                        //set active device to current-device
                        Utility.LogMessage(@"[CopyDeviceContents] [Setting_ActiveDevice] [{0}]", device.DisplayName);
                        _activeDevice = device;

                        PortableDeviceFolder deviceFolder = portableDevice.GetContents();
                        FetchContents(portableDevice, deviceFolder);
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LogException(ex);
            }
            finally
            {
                if (portableDevice != null)
                    portableDevice.Disconnect();

                Utility.LogMessage(@"[CopyDeviceContents] [Finally] [Setting_ActiveDevice][NULL]");
                _activeDevice = null;
            }
        }

        private void FetchContents(PortableDevice portableDevice, PortableDeviceFolder deviceFolder)
        {
            try
            {
                foreach (var fileObject in deviceFolder.Files)
                {
                    if (_activeDevice != null && _activeDevice.ImportOption == DeviceImportOption.Never)
                        break;

                    if (_activeDevice != null && StoredDevices.SingleOrDefault(p => p.Id.Equals(_activeDevice.Id)) == null)
                        break;

                    if (fileObject is PortableDeviceFile)
                    {
                        //if (Common.Utilities.Utility.FileExtensions.IsValidFileExtension(Path.GetExtension(fileObject.Name.ToLower())))
                        if (Utility.ListValidImageExtensions.Contains(Path.GetExtension(fileObject.Name).ToLower()))
                        {
                            //validFile
                            //Utility.LogMessage("{0} {1}", fileObject.Id, fileObject.Name);

                            string str = (string.IsNullOrEmpty(fileObject.Name) ? Path.GetFileName(fileObject.Id) : Path.GetFileName(fileObject.Name));
                            if (!File.Exists(Path.Combine(_defaultMediaBackupPath, str)))
                            {
                                //copy the file to device
                                Utility.LogMessage("[FetchContents] [Copy] [{0} {1}]", fileObject.Id, fileObject.Name);
                                portableDevice.CopyFilefromDevice((PortableDeviceFile)fileObject, DefaultMediaBackupPath);
                                //System.Threading.Thread.Sleep(2000);
                            }
                        }
                    }
                    else
                    {
                        FetchContents(portableDevice, (PortableDeviceFolder)fileObject);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #endregion
        
    }
}