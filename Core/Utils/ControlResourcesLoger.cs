using System;
using System.IO;
using System.Configuration;
using System.Text;

namespace Core.Utils
{
    public static class ControlResourcesLoger
    {
        private static bool LogWriteFile()
        {
            if (ConfigurationManager.AppSettings["LogWriteFile"] == null)
                return false;
            return Convert.ToBoolean(ConfigurationManager.AppSettings["LogWriteFile"]);
        }

        private static bool LogQueryWriteFile()
        {
            if (ConfigurationManager.AppSettings["LogQueryWriteFile"] == null)
                return false;
            return Convert.ToBoolean(ConfigurationManager.AppSettings["LogQueryWriteFile"]);
        }

        public static void LogDedug(string str)
        {
            try
            {
                if (LogWriteFile())
                {
                    string path = Directory.GetCurrentDirectory();
                    if (!Directory.Exists(path + @"\Log"))
                    {
                        Directory.CreateDirectory(path + @"\Log");
                    }
                    var aFileStream = new FileStream($@"{path}\Log\Loger.log", FileMode.Append);
                    var sw = new StreamWriter(aFileStream, Encoding.UTF8);
                    string currentTimeStr = DateTime.Now + " - ";
                    sw.Write(currentTimeStr);
                    sw.WriteLine(str);
                    sw.Dispose();
                    sw.Close();
                    aFileStream.Dispose();
                    aFileStream.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        public static void LogDedugD(string str)
        {
            try
            {
                if (LogWriteFile())
                {
                    string path = @"D:";
                    if (!Directory.Exists(path + @"\Log"))
                    {
                        Directory.CreateDirectory(path + @"\Log");
                    }
                    var aFileStream = new FileStream($@"{path}\Log\Loger.log", FileMode.Append);
                    var sw = new StreamWriter(aFileStream, Encoding.UTF8);
                    string currentTimeStr = DateTime.Now + " - ";
                    sw.Write(currentTimeStr);
                    sw.WriteLine(str);
                    sw.Dispose();
                    sw.Close();
                    aFileStream.Dispose();
                    aFileStream.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        public static void LogDedugQueryTrue(string str)
        {
            try
            {

                string path = Directory.GetCurrentDirectory();
                if (!Directory.Exists(path + @"\Log"))
                {
                    Directory.CreateDirectory(path + @"\Log");
                }
                var aFileStream = new FileStream($@"{path}\Log\Loger.log", FileMode.Append);
                var sw = new StreamWriter(aFileStream, Encoding.UTF8);
                string currentTimeStr = DateTime.Now + " - ";
                sw.Write(currentTimeStr);
                sw.WriteLine(str);
                sw.Dispose();
                sw.Close();
                aFileStream.Dispose();
                aFileStream.Close();

            }
            catch (Exception)
            {
            }
        }

        public static void LogDedugQuery(string str)
        {
            try
            {
                if (LogQueryWriteFile())
                {
                    string path = Directory.GetCurrentDirectory();
                    if (!Directory.Exists(path + @"\Log"))
                    {
                        Directory.CreateDirectory(path + @"\Log");
                    }
                    var aFileStream = new FileStream($@"{path}\Log\Loger.log", FileMode.Append);
                    var sw = new StreamWriter(aFileStream, Encoding.UTF8);
                    string currentTimeStr = DateTime.Now + " - ";
                    sw.Write(currentTimeStr);
                    sw.WriteLine(str);
                    sw.Dispose();
                    sw.Close();
                    aFileStream.Dispose();
                    aFileStream.Close();
                }
            }
            catch (Exception)
            {
            }
        }
     
    }
}
