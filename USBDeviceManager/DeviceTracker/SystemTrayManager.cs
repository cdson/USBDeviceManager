using System;
using System.Diagnostics;
using System.Drawing;
using System.IO.Packaging;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Resources;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Application = System.Windows.Application;
using Size = System.Drawing.Size;

namespace DeviceTracker
{
    public class SystemTrayManager
    {
        private static readonly SystemTrayManager SystemTrayManagerInstance = new SystemTrayManager();

        #region <-PrivateVars->
        private static NotifyIcon _notifyIcon;

        private static ToolStripMenuItem _loginMenuItem;
        private static ToolStripMenuItem _aboutMenuItem;
        private static ToolStripMenuItem _exitMenuItem;
        
        private static readonly Size IconSize = Size.Empty;
        
        
        #endregion
        

        #region <-Constructor->
        static SystemTrayManager()
        {   
        }
        #endregion


        #region <-PublicMethods->
        public static void Init()
        {
            try
            {
                _notifyIcon = new NotifyIcon() { Text = @"Open Device Tracker", };

                //Setting Icon
                ToggleNotifyIconStatus();

                /****ToolStripMenuItems and handling****/

                _aboutMenuItem = new ToolStripMenuItem(@"About", null, (sender, args) =>
                {
                    try
                    {
                        System.Windows.MessageBox.Show("Developed and designed by @aneesahammed", "Device Tracker", MessageBoxButton.OK);
                    }
                    catch (Exception ex)
                    {

                    }
                });



                //Exit
                _exitMenuItem = new ToolStripMenuItem(@"Exit", null, (sender, args) =>
                {
                    try
                    {
                       Exit();
                    }
                    catch (Exception ex)
                    {
                        
                    }
                });


                //ContextMenuStrip
                _notifyIcon.ContextMenuStrip = new ContextMenuStrip();
                _notifyIcon.ContextMenuStrip.Items.AddRange(new ToolStripItem[]
                    {
                      _aboutMenuItem,_exitMenuItem,//new ToolStripSeparator(),
                    });

                //Opening of ContextMenuStrip 
                _notifyIcon.ContextMenuStrip.Opening += (sender, args) =>
                {
                    try
                    {
                    }
                    catch (Exception ex)
                    {   
                    }
                };

                //Click
                _notifyIcon.Click += (sender, args) =>
                {
                    try
                    {
                        MethodInfo mi = typeof(NotifyIcon).GetMethod(@"ShowContextMenu",
                                                                      BindingFlags.Instance | BindingFlags.NonPublic);
                        mi.Invoke(_notifyIcon, null);
                    }
                    catch (Exception ex)
                    {   
                    }
                };

                //DoubleClick
                _notifyIcon.DoubleClick += (sender, args) =>
                {
                    try
                    {
                        DeviceTrackerApp.LaunchMainWindow();
                    }
                    catch (Exception ex)
                    {
                        
                    }
                };

                //setting icon visible
                _notifyIcon.Visible = true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// shows enable/disabled state based on n/w availability
        /// </summary>
        public static void ToggleNotifyIconStatus()
        {
            try
            {
                var sPack = PackUriHelper.UriSchemePack;
                var uriString =
                    String.Format("{0}://application:,,,/DeviceTracker;component/Assets/Images/appIcon.ico", sPack);

                StreamResourceInfo streamResourceInfo =Application.GetResourceStream(new Uri(uriString, UriKind.RelativeOrAbsolute));
                if (streamResourceInfo != null)
                {
                    _notifyIcon.Icon = new Icon(streamResourceInfo.Stream, IconSize);
                }
            }
            catch (Exception ex)
            {   
            }
        }

        public static async void Exit()
        {
            if (_notifyIcon != null) 
                _notifyIcon.Dispose();
            
            System.Windows.Application.Current.Shutdown();
            Environment.Exit(0);
        }

        public static void ShowBalloonTip(string notifyIconText = @"Device Tracker",
                                          System.Windows.Forms.ToolTipIcon toolTipIcon = System.Windows.Forms.ToolTipIcon.Info,
                                          string balloonTipTitle = @"Device Tracker", string balloonTipText = @"Device Tracker",
                                          int timeOut = 1000)
        {
            try
            {
                if (_notifyIcon == null)
                    _notifyIcon = new System.Windows.Forms.NotifyIcon()
                    {
                        Text = "Device Tracker",
                    };
                _notifyIcon.Text = notifyIconText;
                _notifyIcon.BalloonTipIcon = toolTipIcon;
                _notifyIcon.BalloonTipTitle = balloonTipTitle;
                _notifyIcon.BalloonTipText = balloonTipText;
                _notifyIcon.ShowBalloonTip(timeOut);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

    }
}
