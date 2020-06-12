using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using Omsit.UpdateSlashScreen.Common.Utils;


namespace Omsit.UpdateSlashScreen.DXSplashScreen.Internal
{
    /// <summary>
    /// Загрузчик приложения
    /// </summary>
    public class Bootstrapper
    {
        //Имплементация паттерна одиночка (Singlton)
        #region Singlton
        private static volatile Bootstrapper _instance;
        private static readonly object SyncRoot = new Object();

        public static Bootstrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new Bootstrapper();
                    }
                }
                return _instance;
            }
        }
        #endregion

        public Exception LastError { get; protected set; }

        //Инициализация загрузчика
        public bool Init(StartupEventArgs args)
        {
            // LoadCommonAssemblies();
            Thread.Sleep(2000);
            InitBltoolkit(args);

            MedicineSplashScreen.Progress("Обновление завершено");

            return LastError == null;
        }
        private void InitBltoolkit(StartupEventArgs args)
        {
            try
            {
                var commandLine = new CommandLineArguments(args.Args);
                var profileName = commandLine["profileName"];
                var up = commandLine["up"];

                try
                {

                    Process[] ps1 = System.Diagnostics.Process.GetProcessesByName("MedicineNext"); //Имя процесса
                    foreach (Process p1 in ps1)
                    {
                        p1.Kill();
                    }
                    Thread.Sleep(1000);

                    string path = Directory.GetCurrentDirectory();
                    if (!Directory.Exists(path + @"\Update"))
                    {
                        MedicineSplashScreen.Progress("Каталог Update не найден"); //(path + @"\Update");
                        Thread.Sleep(1000);
                        return;
                    }
                    
                    var files =
                        new List<string>(Directory.GetFiles(path + @"\Update"));
                    if (files.Count > 0)
                    {
                        foreach (string file in files)
                        {
                            FileInfo fi = new FileInfo(file);
                            fi.CopyTo(Path.Combine(path, fi.Name), true);
                            MedicineSplashScreen.Progress($"{fi.Name}");
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        MedicineSplashScreen.Progress("Файлы не найдены."); 
                        Thread.Sleep(1000); 
                        return;
                    }

                    //HttpClient client = new HttpClient();
                    //var connect = client.GetConnect();
                    //if (connect.Success)
                    //{
                    //    MedicineSplashScreen.Progress("Соединение установлено.");
                    //    var vers = _versionService.Version();

                    //    if (vers)
                    //    {
                    //        var s = client.GetVersion((int)_versionService.VersionId);
                    //        if (s.Status == 0)
                    //        {
                    //            IApplyUpdateBaseService applyUpdateBaseService =
                    //                new ApplyUpdateBaseService(_shareSettings, _overlayManager, _messageService);

                    //            foreach (var versionMtr in s.Data)
                    //            {
                    //                if (versionMtr.Type == TypeProductUpdate.DB_TFOMS)
                    //                {
                    //                    MedicineSplashScreen.Progress(
                    //                        versionMtr.NumberVersion +
                    //                        " " + versionMtr.NameVersion.Replace("<p>", "").Replace("</p>", "")
                    //                            .Replace("<strong>", "").Replace("</strong>", ""));
                    //                    string resultSqlScript =
                    //                        applyUpdateBaseService.UnZipFileToSqlScript(versionMtr.Data);
                    //                    if (applyUpdateBaseService.Run(resultSqlScript))
                    //                    {
                    //                        MedicineSplashScreen.Progress($"Обновление {versionMtr.NumberVersion} выполнено успешно");
                    //                        Thread.Sleep(4000);
                    //                    }
                    //                    else
                    //                    {
                    //                        MedicineSplashScreen.Progress("Произошла ошибка при обновлении базы данных");
                    //                        throw new Exception("Произошла ошибка при обновлении базы данных");
                    //                    }
                    //                }

                    //                if (versionMtr.Type == TypeProductUpdate.APP_TFOMS)
                    //                {
                    //                    MedicineSplashScreen.Progress($"Есть обновление клиента");

                    //                    var resultfile =
                    //                        applyUpdateBaseService.UnZipFileToFile(versionMtr.Data);
                    //                    if (resultfile.Success)
                    //                    {
                    //                        foreach (var file in resultfile.Data)
                    //                        {
                    //                            MedicineSplashScreen.Progress($"{file}");
                    //                            Thread.Sleep(1000);
                    //                        }
                    //                    }
                    //                    else
                    //                    {
                    //                        MedicineSplashScreen.Progress($"{resultfile.LastError}");
                    //                        Thread.Sleep(5000);
                    //                    }
                    //                    //string path = Directory.GetCurrentDirectory();
                    //                    //Process.Start(path + @"\Omsit.UpdateSlashScreen.exe");
                    //                }

                    //            }
                    //            MedicineSplashScreen.Progress($"База данных обновлена до последней версии");
                    //            Thread.Sleep(5000);

                    //            //var ss = client.GetVersionJson(500);

                    //            // Console.WriteLine(ss);
                    //        }
                    //        else
                    //        {
                    //            if (s.Message == "Нет данных для обновления")
                    //            {
                    //                MedicineSplashScreen.Progress($"Нет данных для обновления. Вы используете актуальную версию базы данных.");
                    //                Thread.Sleep(5000);
                    //            }

                    //        }
                    //    }

                    //}
                    //else
                    //{
                    //    MedicineSplashScreen.Progress("Сервис не доступен");
                    //    Thread.Sleep(3000);
                    //}

                    string pathMedicineNext = Directory.GetCurrentDirectory();
                    Process.Start(pathMedicineNext + @"\MedicineNext.exe");

                }
                catch (Exception exception)
                {
                    MedicineSplashScreen.Progress($"Ошибка {exception.Message}");
                    Thread.Sleep(2000);
                }

            }
            catch (Exception ex)
            {
                MedicineSplashScreen.Progress($"Ошибка {ex.Message}");
                Thread.Sleep(2000);
            }
        }

    }
}
