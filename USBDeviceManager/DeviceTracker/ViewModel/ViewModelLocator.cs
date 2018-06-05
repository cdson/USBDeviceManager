using DeviceTracker.Services;
using DeviceTracker.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using DeviceTracker.Model;

namespace DeviceTracker.ViewModel
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
                SimpleIoc.Default.Register<IDeviceService,DeviceService>();
                SimpleIoc.Default.Register<ISettingService,SettingService>();

                SimpleIoc.Default.Register<MainViewModel>();
                SimpleIoc.Default.Register<ConfirmationViewModel>();
            }

            SimpleIoc.Default.Register<MainWindow>();
        }

        public static void Init()
        {
            SimpleIoc.Default.Register<ConfirmationView>(true);
        }


        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public ConfirmationViewModel ConfirmationVm
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ConfirmationViewModel>();
            }
        }



        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}