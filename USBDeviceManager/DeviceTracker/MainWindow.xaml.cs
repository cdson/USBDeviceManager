using System.Windows;
using DeviceTracker.Common;
using DeviceTracker.ViewModel;
using GalaSoft.MvvmLight.Messaging;


namespace DeviceTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;

            Messenger.Default.Register<SelectFolderPathMessage>(this, OnSelectFolderPathMessage);
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            this.Topmost = false;

            var mainVm = DataContext as MainViewModel;
            if(mainVm!=null)
                mainVm.InitDevice();
        }


        private void OnSelectFolderPathMessage(SelectFolderPathMessage selectFolderPathMsgNotification)
        {
            using (var fbDialog = new System.Windows.Forms.FolderBrowserDialog() { Description=@"Select a folder to backup media files!!"})
            {
                if (fbDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    selectFolderPathMsgNotification.Callback(fbDialog.SelectedPath);
                }
            }
        }
    }
}