using System.Windows;
using DeviceTracker.Common;
using GalaSoft.MvvmLight.Messaging;

namespace DeviceTracker.Views
{
    /// <summary>
    /// Description for Confirmation.
    /// </summary>
    public partial class ConfirmationView : Window
    {
        private DeviceImportOption _deviceImportOption=DeviceImportOption.JustOnce;
       
        public ConfirmationView()
        {
            InitializeComponent();

            Messenger.Default.Register<ConfirmationMessage>(this, OnConfirmationMessageResponseNotification);
            this.Closing += ConfirmationView_Closing;
            this.Loaded += ConfirmationView_Loaded;
        }

        void ConfirmationView_Loaded(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            this.Topmost = false;
        }

        private static bool isBusy = false;
        private void OnConfirmationMessageResponseNotification(ConfirmationMessage confirmationMessage)
        {
            if (!isBusy)
            {
                isBusy = true;

                deviceName.Text = confirmationMessage.DeviceName;

                BringIntoView();

                ShowDialog();

                confirmationMessage.Callback(_deviceImportOption);
                isBusy = false;
            }
        }


        private void BtnNever_OnClick(object sender, RoutedEventArgs e)
        {
            _deviceImportOption = DeviceImportOption.Never;
            this.Close();
        }

        private void BtnAlways_OnClick(object sender, RoutedEventArgs e)
        {
            _deviceImportOption = DeviceImportOption.Always;
            this.Close();
        }

        private void BtnJustOnce_OnClick(object sender, RoutedEventArgs e)
        {
            _deviceImportOption = DeviceImportOption.JustOnce;
            this.Close();
        }

        void ConfirmationView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}