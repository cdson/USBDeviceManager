using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace DeviceTracker.Common
{
    public class Utility
    {
        private static readonly object _locker = new object();

        private static readonly string LogPath = "";
            //Path.Combine(Directory.GetParent(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath)).ToString(),
            //             @"DTLogs");

        public static readonly List<string> ListValidImageExtensions = new List<string>() { ".jpg", ".jpeg", ".webp", ".gif", ".crw", ".cr2", ".nef", ".dng", ".orf", ".raf", ".arw", ".pef", ".srw", ".rw2", ".png" };
        public static readonly List<string> ListValidVideoExensions = new List<string>() { ".mpg", ".wmv", ".asf", ".avi", ".divx", ".mov", ".m4v", ".3gp", ".3g2", ".mp4", ".mkv" };

        public class DeepCopy
        {

            /// <summary>
            /// Makes a deep copy of the specified Object.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="objectToCopy">Object to make a deep copy of.</param>
            /// <returns>Deep copy of the Object</returns>
            public static T Make<T>(T objectToCopy) where T : class
            {

                using (var ms = new MemoryStream())
                {

                    var bf = new BinaryFormatter();
                    bf.Serialize(ms, objectToCopy);
                    ms.Position = 0;
                    return (T)bf.Deserialize(ms);
                }
            }
        }

        private static System.Windows.Forms.NotifyIcon _notifyIcon;
        

        public static void WriteToFile(string contents)
        {
            using (var sw = new StreamWriter(@"fileStorage.json"))
            {
                sw.WriteLine(contents);
            }
        }


        //Debug
        public static void LogMessage(string message)
        {
            try
            {
                lock (_locker)
                {
                    using (var sw = new StreamWriter(Path.Combine(LogPath, "DT.log"), true, Encoding.ASCII))
                    {
                        var strFormattedMessage = String.Format("[{0}][{1}][{2};]\r\n",
                            DateTime.UtcNow, LogLevel.Debug, message);

                        sw.WriteLine(strFormattedMessage);
                        System.Diagnostics.Debug.WriteLine(message);
                    }
                }
            }
            catch { }
        }

        //With LogLevel
        public static void LogMessage(object message, LogLevel logLevel=LogLevel.Debug)
        {
            lock (_locker)
            {
                try
                {
                    using (var sw = new StreamWriter(Path.Combine(LogPath, "DT.log"), true, Encoding.ASCII))
                    {
                        var strFormattedMessage = String.Format("[{0}][{1}][{2};]\r\n",
                            DateTime.UtcNow, logLevel, message);

                        sw.WriteLine(strFormattedMessage);
                        System.Diagnostics.Debug.WriteLine(message);
                    }
                }
                catch { }
            }
        }


        public static void LogMessage(string format, Object arg0, Object arg1)
        {
            LogMessage(String.Format(format,arg0,arg1));
        }

        public static void LogMessage(string format, params Object[] arg)
        {
            if (arg == null)                       // avoid ArgumentNullException from String.Format
                LogMessage(format, null, null);

            LogMessage(String.Format(format, arg));
        }

        public static void LogException(Exception exception)
        {
            try
            {
                using (var sw = new StreamWriter(Path.Combine(LogPath, "DT.log"), true, Encoding.ASCII))
                {
                    var strFormattedMessage = String.Format("[{0}][{1}][{2};{3}]\r\n",
                        DateTime.UtcNow, LogLevel.Error, exception.Message, exception.StackTrace);

                    sw.WriteLine(strFormattedMessage);
                    System.Diagnostics.Debug.WriteLine(exception);
                }
            }
            catch { }
        }
    }
}
