/*
 * @file:SingleInstanceManager
 * @brief: for single instance of application.
 * @author:AA
 * @Credits:http://www.switchonthecode.com/tutorials/wpf-writing-a-single-instance-application                
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using DeviceTracker;
using Microsoft.VisualBasic.ApplicationServices;


using MessageBox = System.Windows.Forms.MessageBox;
using StartupEventArgs = Microsoft.VisualBasic.ApplicationServices.StartupEventArgs;

namespace DeviceTracker
{
    public sealed class SingleInstanceManager:WindowsFormsApplicationBase
    {
        #region <-Properties->
        public DeviceTrackerApp App { get; private set; }
        #endregion

        #region <-Constructor->
        private SingleInstanceManager()
        {
            IsSingleInstance = true;
        }
        #endregion

        #region <-Main->

        /// <summary>
        /// Main method which creates a new SingleInstanceManager and passes in arguments.
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {   
                new SingleInstanceManager().Run(args);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(@"Error on launch {0} \n {1}", ex.Message, ex.StackTrace),
                                "DeviceTracker", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region <-ProtectedMethods->
        /// <summary>
        /// Overrides the OnStartup Method from Base.
        /// On first startup, creates a new Application and runs it. 
        /// </summary>
        /// <returns>A System.Boolean that indicates if the application should continue starting up.</returns>
        protected override bool OnStartup(StartupEventArgs eventArgs)
        {
            try
            {
                //to use async/await
                InitializeStartup();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return false;
        }

        /// <summary>
        /// Overrides the OnStartupNextInstance Method from Base.
        /// On subsequent startup, restore the current application instance and passes any command line arguments to it
        /// </summary>
        /// <param name="eventArgs"></param>
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
                base.OnStartupNextInstance(eventArgs);
                App.MainWindow.Activate();
                if (App.MainWindow.Visibility == Visibility.Collapsed || App.MainWindow.Visibility == Visibility.Hidden)
                {
                    App.MainWindow.Visibility = Visibility.Visible;
                    App.MainWindow.Topmost = true;
                    App.MainWindow.Topmost = false;
                }
        }
        #endregion

        #region <-PrivateMethods->
        private async void InitializeStartup()
        {
            InitializeLogger();

            App = new DeviceTrackerApp();
            //DeviceTrackerApp.Current.Resources.Add("Locator", new DeviceTracker.ViewModel.ViewModelLocator());
            App.Run();
        }

        private void InitializeLogger()
        {
            Debug.WriteLine(@"______________________________________STARTING DEVICETRACKER______________________________________");
        }
        #endregion

    }
}
