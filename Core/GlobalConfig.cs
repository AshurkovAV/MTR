using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Core.Extensions;

namespace Core
{
    /// <summary>
    /// Глобальные настройки приложения
    /// </summary>
    public class GlobalConfig
    {
        private static string _appInstanceId;
        private static string _settingsFolder;
        private static string _tempFolder;
        private static int _processorCount;
        private static bool _multiProcessor;

        public static readonly string TempFolderBase = Path.Combine(Path.GetTempPath(), "Medicine");
        private static string _baseDirectory;
        private static Assembly _assembly;
        private static string _exePath;

        public static string UserDataFolder = Path.GetDirectoryName(typeof(GlobalConfig).Assembly.Location);

        public static bool AllowOneToOne { get; set; }

        public static readonly string AppName = "Medicine";
        public static readonly bool Is64Bit = Environment.Is64BitProcess;
        public static readonly string UserName = Environment.UserName;

        public static int ProcessorCount { get { return _processorCount; } }
        public static bool MultiProcessor { get { return _multiProcessor; } }

        static GlobalConfig()
        {
            _processorCount = Environment.ProcessorCount;

            if (_processorCount > 1)
            {
                _multiProcessor = true;
            }
        }

        public static string AppInstanceId
        {
            get
            {
                if (string.IsNullOrEmpty(_appInstanceId))
                {
                    _appInstanceId = (AppDomain.CurrentDomain.GetData("MedicineInstanceId") as string);
                    if (string.IsNullOrEmpty(_appInstanceId))
                    {
                        _appInstanceId = "{0}_{1}_{2}".F(AppName, UserName, DateTime.Now.ToString("yyyyMMddHHmmss"));
                    }
                }
                return _appInstanceId;
            }
        }

        public static string SettingsFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_settingsFolder))
                {
                    _settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
                    if (!Directory.Exists(_settingsFolder))
                    {
                        try
                        {
                            Directory.CreateDirectory(_settingsFolder);
                        }
                        catch
                        {
                            Thread.Sleep(100);
                            if (!Directory.Exists(_settingsFolder))
                            {
                                Directory.CreateDirectory(_settingsFolder);
                            }
                        }
                    }
                }
                return _settingsFolder;
            }
        }

        public static string TempFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_tempFolder))
                {
                    _tempFolder = Path.Combine(TempFolderBase, AppInstanceId);
                    if (!Directory.Exists(_tempFolder))
                    {
                        try
                        {
                            Directory.CreateDirectory(_tempFolder);
                        }
                        catch
                        {
                            Thread.Sleep(100);
                            if (!Directory.Exists(_tempFolder))
                            {
                                Directory.CreateDirectory(_tempFolder);
                            }
                        }
                    }
                }
                return _tempFolder;
            }
        }

        public static Version Version { get { return EntryAssembly.GetName().Version; } }

        public static Assembly EntryAssembly
        {
            get
            {
                return _assembly ?? (_assembly = Assembly.GetEntryAssembly());
            }
        }
        public static string ExePath
        {
            get
            {
                if (_exePath == null)
                {
                    _exePath = EntryAssembly.Location;
                }

                return _exePath;
            }
        }

        public static string BaseDirectory
        {
            get
            {
                if (_baseDirectory == null)
                {
                    try
                    {
                        _baseDirectory = ExePath;

                        if (_baseDirectory.Length > 0)
                            _baseDirectory = Path.GetDirectoryName(_baseDirectory);
                    }
                    catch
                    {
                        _baseDirectory = "";
                    }
                }

                return _baseDirectory;
            }
        }
    }
}