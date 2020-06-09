using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Core.Utils
{
    public static class CommonUtils
    {
        public static string GetVersionInformationString(Type type)
        {
            string str = "";
            if (type != null)
            {
                object[] attr = type.Assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                if (attr.Length == 1)
                {
                    var aiva = (AssemblyInformationalVersionAttribute)attr[0];
                    str += type.Assembly.GetName().Name + " Version : " + aiva.InformationalVersion + Environment.NewLine;
                }
            }
            
            str += ".NET Version         : " + Environment.Version.ToString() + Environment.NewLine;
            str += "OS Version           : " + Environment.OSVersion.ToString() + Environment.NewLine;
            try
            {
                string cultureName = CultureInfo.CurrentCulture.Name;
                str += "Current culture      : " + CultureInfo.CurrentCulture.EnglishName + " (" + cultureName + ")" + Environment.NewLine;
            }
            catch { }

            try
            {
                if (IntPtr.Size != 4)
                {
                    str += "Running as " + (IntPtr.Size * 8) + " bit process" + Environment.NewLine;
                }
                string PROCESSOR_ARCHITEW6432 = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432");
                if (!string.IsNullOrEmpty(PROCESSOR_ARCHITEW6432))
                {
                    if (PROCESSOR_ARCHITEW6432 == "AMD64")
                        PROCESSOR_ARCHITEW6432 = "x86-64";
                    str += "Running under WOW6432, processor architecture: " + PROCESSOR_ARCHITEW6432 + Environment.NewLine;
                }
            }
            catch { }
            try
            {
                if (SystemInformation.TerminalServerSession)
                {
                    str += "Terminal Server Session" + Environment.NewLine;
                }
                if (SystemInformation.BootMode != BootMode.Normal)
                {
                    str += "Boot Mode            : " + SystemInformation.BootMode + Environment.NewLine;
                }
            }
            catch { }
            str += "Working Set Memory   : " + (Environment.WorkingSet / 1024) + "kb" + Environment.NewLine;
            str += "GC Heap Memory       : " + (GC.GetTotalMemory(false) / 1024) + "kb" + Environment.NewLine;
            return str;
        }

        private static Stack<ConsoleColor> _consoleColors = new Stack<ConsoleColor>();
        public static void PushColor(ConsoleColor color)
        {
            try
            {
                _consoleColors.Push(Console.ForegroundColor);
                Console.ForegroundColor = color;
            }
            catch
            {
            }
        }

        public static void PopColor()
        {
            try
            {
                Console.ForegroundColor = _consoleColors.Pop();
            }
            catch
            {
            }
        }
    }
}
