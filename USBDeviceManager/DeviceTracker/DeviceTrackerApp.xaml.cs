using System;
using System.Windows;
using DeviceTracker.ViewModel;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;

namespace DeviceTracker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class DeviceTrackerApp : Application
    {
        static DeviceTrackerApp()
        {
            DispatcherHelper.Initialize();
        }


         /// <summary>
        /// needed for resource initialization
        /// </summary>
        public DeviceTrackerApp()
        {
            InitializeComponent();
            ViewModelLocator.Init();
        }

        #region <-ProtectedMethods->
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);

                AppInit();
            }
            catch (Exception ex)
            {
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                base.OnExit(e);
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        private async void AppInit()
        {
            SystemTrayManager.Init();
            ViewModelLocator.Init();
            LaunchMainWindow();
        }

        public static void LaunchMainWindow()
        {
            var appWindow = ServiceLocator.Current.GetInstance<MainWindow>();
            appWindow.Activate();
            appWindow.WindowState = WindowState.Normal;
            appWindow.Show();
            appWindow.Topmost = true;
            appWindow.Topmost = false;
        }
    }
}
