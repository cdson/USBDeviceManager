using System;
using System.Windows.Input;
using DeviceTrackerSupport;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.CommandWpf;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using OnePhotoBackup.Common;
using OnePhotoBackup.Common.Services;
using OnePhotoBackup.Common.Utilities;
using OnePhotoBackup.Common.Utilities.Extensions;
using OnePhotoBackup.Model;


namespace OnePhotoBackup.ViewModel
{
    
    //public class DeviceTabViewModel : ViewModelBase
    public partial class PreferenceViewModel
    {
        #region <-PrivateVars->
        private readonly IDataService _dataService;
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
            set { 
                _isDeviceBackupEnabled = value;
                RaisePropertyChanged();

                if (_isDeviceBackupEnabled)
                {
                    RegisterDriveDetector();
                }
                else
                {
                    UnRegisterDriveDetector();
                }
            }
        }

        private bool _isDefaultMediaBackupPathEnabled = false;
        public bool IsDefaultMediaBackupPathEnabled
        {
            get { return _isDefaultMediaBackupPathEnabled; }
            set
            {
                _isDefaultMediaBackupPathEnabled=value;
                RaisePropertyChanged();
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
            }
        }
        
        public ObservableCollection<Device> StoredDevices{ get; set; }

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
        #endregion

        #region <-Commands->
        public ICommand BrowseMediaBackupPathCommand
        {
            get { return new RelayCommand(BrowseMediaBackupPathCommandExecute, CanBrowseMediaBackupPathCommandExecute); }
        }

        public ICommand ImportOptionToggleCommand
        {
            get {return new RelayCommand<Device>(ImportOptionToggleCommandExecute) ;}
        }

        public ICommand ClearDeviceCommand
        {
            get { return new RelayCommand<Device>(ClearDeviceCommandExecute); }
        }
        #endregion

        #region <-Constructor->
        #endregion        

        #region <-Init->
        private async void InitDevice()
        {               
            _connectedDeviceCollection = new PortableDeviceCollection();
            _deviceProcessingQueue = new Queue<Device>();           

            //retrieve settings from db and
            _isDeviceBackupEnabled = true;

            if (_isDeviceBackupEnabled)
            {
                //default folder,get from db, if null init with default path.
                _defaultMediaBackupPath = System.IO.Path.Combine(Environment.GetFolderPath
                                                    (Environment.SpecialFolder.MyPictures), @"OnePhotoBackup");
                if (!Directory.Exists(_defaultMediaBackupPath))
                {
                    try
                    {
                        Directory.CreateDirectory(_defaultMediaBackupPath);
                    }
                    catch {/*parse*/}
                }

                RegisterDriveDetector();

                //get all devices from db.if empty,initialize StoredDevices
                var devices = await _deviceService.GetAllDevices();

                if (devices == null)
                    StoredDevices = new ObservableCollection<Device>();
                else
                    StoredDevices = new ObservableCollection<Device>(devices);
                
                CheckDeviceStatusChange();
            }
            
        }
        #endregion        

        #region <-CommandMethods->
        private bool CanBrowseMediaBackupPathCommandExecute()
        {
            return IsDefaultMediaBackupPathEnabled;
        }
        private void BrowseMediaBackupPathCommandExecute()
        {
            try
            {
                Messenger.Default.Send(new Message.SelectFolderPathMessage((path) =>
                {
                    OnMediaFolderSelected(path);
                }));
            }
            catch (Exception ex)
            {
                _logService.LogException(ex);
            }
           
        }

        private async void ImportOptionToggleCommandExecute(Device device)
        {
            try
            {
                //get the devic by id from data-store
                var storedDevice = await _deviceService.GetDeviceById(device.Id);

                //update the check-field
                storedDevice.ImportOption = (device.ImportOption == DeviceImportOption.Always ?
                    DeviceImportOption.Never : DeviceImportOption.Always);

                //update same in db.
                //await _deviceService.UpdateDevice(storedDevice);
                _deviceService.SetImportOption(storedDevice.Id, storedDevice.ImportOption);


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
                _logService.LogException(ex);
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
                _logService.LogException(ex);
            }
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
                _logService.LogException(ex);
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
                System.Diagnostics.Debug.WriteLine("Device arrived");
            }
            catch (Exception ex)
            {
                _logService.LogException(ex);
            }
        }

        //[Not used]
        void _driveDetector_DeviceRemoved(object sender, DriveDetectorEventArgs e)
        {
            try
            {
                Console.WriteLine("Device removed");
            }
            catch (Exception ex)
            {
                //log exception
            }
        }

        //[Not used]
        void _driveDetector_QueryRemove(object sender, DriveDetectorEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("QueryRemove");
            }
            catch (Exception ex)
            {
                _logService.LogException(ex);
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
                _driveDetector.DeviceArrived -= _driveDetector_DeviceArrived;
                //_driveDetector.QueryRemove -= _driveDetector_QueryRemove;
                _driveDetector.DeviceRemoved -= _driveDetector_DeviceRemoved;
                _driveDetector.DeviceStatusChanged -= _driveDetector_DeviceStatusChanged;
                _driveDetector.Dispose();
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
                            Console.WriteLine(@"::[CheckDeviceStatusChange] [Queueing the device {0}]::", device.DisplayName);
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
        }

        private void OnEqueue()
        {
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
                            Console.WriteLine(@"[OnEqueue] [Finally dequeuing device {0} from processing queue]",
                                              dequeuedDevice.DisplayName);

                            _progressFlag = false;

                            Console.WriteLine("___________________________________________________________");

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
        }

        private async void OnDeviceStatusChange(Device connectedDevice)
        {
            try
            {
                //peek queue/process/dequeue
                Console.WriteLine(@"[OnDeviceStatusChange] [Started processing the device {0}]", connectedDevice.DisplayName);

                var storedDevices = await _deviceService.GetAllDevices();

                var deviceStoredInDb = storedDevices.SingleOrDefault(p => p.Id.Equals(connectedDevice.Id));

                if (deviceStoredInDb == null)
                {
                    
                }
                else
                {
                    //same devices is stored in db
                    Console.WriteLine("::Device {0} is already present in database with import option {1}",
                        deviceStoredInDb.DisplayName, deviceStoredInDb.ImportOption);

                  
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ManageNewDevices(Device connectedDevice)
        {
            Console.WriteLine("::Device {0} is not stored in database.", connectedDevice.DisplayName);

            //call prompt
            Messenger.Default.Send(new Message.ConfirmationMessage(connectedDevice.DisplayName, async (response) =>
            {
                connectedDevice.ImportOption = response;
                Console.WriteLine("Import option selected for device {0} is {1}",
                                        connectedDevice.DisplayName, connectedDevice.ImportOption);

                switch (response)
                {
                    case DeviceImportOption.Always:
                        Console.WriteLine("Adding device {0} to database", connectedDevice.DisplayName);
                        AddDevice(connectedDevice);

                        ShowFileCountNotification(connectedDevice);

                        //copy files -> default-media-path
                        await CopyDeviceContents(connectedDevice);
                        break;

                    case DeviceImportOption.Never:
                        Console.WriteLine("Adding device {0} to database", connectedDevice.DisplayName);

                        AddDevice(connectedDevice);
                        break;

                    case DeviceImportOption.JustOnce:
                        //dont store in db.

                        //copy files->default-media-path
                        await CopyDeviceContents(connectedDevice);
                        break;
                }
            }));
        }

        private void ManageStoredDevices(Device storedDevice)
        {
            if (System.Threading.Thread.CurrentThread.IsBackground)
                Console.WriteLine("[ManageStoredDevices] [IsBackground]");

            try
            {
                switch (storedDevice.ImportOption)
                {
                    case DeviceImportOption.Always:
                        ShowFileCountNotification(storedDevice);

                        //copy files->default-media-path
                        CopyDeviceContents(storedDevice);
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
            try
            {
                var result = await _deviceService.AddDevice(connectedDevice);
                if (result!= Constants.INT32_NOTFOUND)
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
                var devices = await _deviceService.GetAllDevices();
                lock (_locker2)
                {

                    StoredDevices = new ObservableCollection<Device>(devices);
                    StoredDevices.SortDescending(p=>p.DisplayName);
                    RaisePropertyChanged("StoredDevices");
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void ShowFileCountNotification(Device connectedDevice)
        {
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
                        _activeDevice = connectedDevice;

                        PortableDeviceFolder deviceFolder = portableDevice.GetContents();
                        
                        var fileCount = EnumerateContents(portableDevice, deviceFolder);                
                        Console.WriteLine("Total number of files found {0}",fileCount);

                        if(_fileCounter>0)
                            SystemTrayManager.ShowBalloonTip(balloonTipText:string.Format("{0} files found in - {1} "
                                ,_fileCounter,connectedDevice.DisplayName));
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
                
                _activeDevice = null;
                _fileCounter = 0;
            }
        }


        private static long _fileCounter = 0;
        private long EnumerateContents(PortableDevice portableDevice, PortableDeviceFolder deviceFolder)
        {
            try
            {   
                foreach (var fileObject in deviceFolder.Files)
                {
                    if (_activeDevice != null && _activeDevice.ImportOption == DeviceImportOption.Never)
                        break;

                    if (fileObject is PortableDeviceFile)
                    {
                        if (Common.Utilities.Utility.FileExtensions.IsValidFileExtension(Path.GetExtension(fileObject.Name.ToLower())))
                        {

                            string str = (string.IsNullOrEmpty(fileObject.Name)? Path.GetFileName(fileObject.Id)
                                : Path.GetFileName(fileObject.Name));
                            
                            if (!File.Exists(Path.Combine(_defaultMediaBackupPath, str)))
                            {
                                Console.WriteLine("{0} {1}", fileObject.Id, fileObject.Name);
                                _fileCounter++;
                            }
                        }
                    }
                    else
                    {
                        EnumerateContents(portableDevice, (PortableDeviceFolder)fileObject);
                    }
                }
                return _fileCounter;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region --Copy--
        private async Task CopyDeviceContents(Device device)
        {
            Task.Run(() =>
                {
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

                                //set active device to curren-device
                                _activeDevice = device;
                                PortableDeviceFolder deviceFolder = portableDevice.GetContents();
                                FetchContents(portableDevice, deviceFolder);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //log exception
                    }
                    finally
                    {
                        if (portableDevice != null)
                            portableDevice.Disconnect();
                        _activeDevice = null;
                    }
                });
        }

        private void FetchContents(PortableDevice portableDevice,PortableDeviceFolder deviceFolder)
        {
            try
            {  
                foreach (var fileObject in deviceFolder.Files)
                {
                    if (_activeDevice != null && _activeDevice.ImportOption == DeviceImportOption.Never)
                        break;
                    
                    if (_activeDevice!=null && StoredDevices.SingleOrDefault(p => p.Id.Equals(_activeDevice.Id))==null)
                        break;
                    
                    if (fileObject is PortableDeviceFile)
                    {
                        if (Common.Utilities.Utility.FileExtensions.IsValidFileExtension(Path.GetExtension(fileObject.Name.ToLower())))
                        {
                            Console.WriteLine("{0} {1}", fileObject.Id, fileObject.Name);

                            string str = (string.IsNullOrEmpty(fileObject.Name) ? Path.GetFileName(fileObject.Id) : Path.GetFileName(fileObject.Name));
                            if (!File.Exists(Path.Combine(_defaultMediaBackupPath, str)))
                            {
                                Console.WriteLine("{0} {1}", fileObject.Id, fileObject.Name);
                                portableDevice.CopyFilefromDevice((PortableDeviceFile)fileObject, DefaultMediaBackupPath);    
                            }
                        }
                    }
                    else
                    {
                        FetchContents(portableDevice,(PortableDeviceFolder)fileObject);
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