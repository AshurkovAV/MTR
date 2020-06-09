using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Core.Helpers
{
    public static class PathHelper
    {
        private static Dictionary<string, string> _tokensToFolders;
        private static string _programFiles;
        private static string _programFilesX86;
        private static Regex _folderDecoder;

        static PathHelper()
        {
            _tokensToFolders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _folderDecoder = new Regex("^<.+>", RegexOptions.IgnoreCase);
            AddSpecialFolder(Environment.SpecialFolder.ApplicationData);
            AddSpecialFolder(Environment.SpecialFolder.CommonApplicationData);
            AddSpecialFolder(Environment.SpecialFolder.LocalApplicationData);
            AddSpecialFolder(Environment.SpecialFolder.CommonProgramFiles);
            AddSpecialFolder(Environment.SpecialFolder.ProgramFiles);
            AddSpecialFolder(Environment.SpecialFolder.ProgramFilesX86);
            AddSpecialFolder(Environment.SpecialFolder.Personal);
            AddSpecialFolder("RuntimeDirectory", RuntimeEnvironment.GetRuntimeDirectory());
            AddSpecialFolder("TempFiles", Path.GetTempPath());
        }

        public static string ProgramFiles
        {
            get
            {
                if (_programFiles == null)
                {
                    _programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                }
                return _programFiles;
            }
        }

        public static string ProgramFilesX86
        {
            get
            {
                if (_programFilesX86 == null)
                {
                    _programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                }
                return _programFilesX86;
            }
        }

        private static void AddSpecialFolder(Environment.SpecialFolder sf)
        {
            try
            {
                AddSpecialFolder(sf.ToString(), Environment.GetFolderPath(sf));
            }
            catch
            {
            }
        }

        private static void AddSpecialFolder(string token, string location)
        {
            if (location.EndsWith("\\"))
            {
                location = location.Substring(0, location.Length - 1);
            }
            token = "<" + token + ">";
            _tokensToFolders[token] = location;
        }

        public static bool PathHasInvalidChars(string path)
        {
            bool result;
            for (int i = 0; i < path.Length; i++)
            {
                int num = path[i];
                if (num == 34 || num == 60 || (num == 62 || num == 124) || num < 32)
                {
                    result = true;
                    return result;
                }
            }
            result = false;
            return result;
        }

        public static string DecodeFolder(string s)
        {
            string result;
            if (string.IsNullOrEmpty(s))
            {
                result = s;
            }
            else
            {
                if (Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).EndsWith(" (x86)",
                                                                                               StringComparison.
                                                                                                   OrdinalIgnoreCase))
                {
                    s = s.Replace("<ProgramFiles> (x86)", "<ProgramFiles>");
                }
                Match match = _folderDecoder.Match(s);
                if (!match.Success)
                {
                    result = s;
                }
                else
                {
                    string text = "";
                    _tokensToFolders.TryGetValue(match.Value, out text);
                    if (!string.IsNullOrEmpty(text))
                    {
                        text += "\\";
                    }
                    string text2 = _folderDecoder.Replace(s, text ?? "");
                    if (text2.Length > 2)
                    {
                        text2 = text2[0] + text2.Substring(1).Replace("\\\\", "\\");
                    }
                    result = FixReference(text2);
                }
            }
            return result;
        }

        public static string ResolveReference(string referencePath)
        {
            if (!referencePath.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
            {
                referencePath += ".dll";
            }
            string result;
            if (File.Exists(referencePath))
            {
                result = FixReference(referencePath);
            }
            else
            {
                string text =
                    FixReference(Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), Path.GetFileName(referencePath)));
                if (File.Exists(text))
                {
                    result = text;
                }
                else
                {
                    result = referencePath;
                }
            }
            return result;
        }

        private static string FixReference(string filePath)
        {
            string runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
            string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                                       "Reference Assemblies\\Microsoft\\Framework\\");
            string text2 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            if (text2.EndsWith("\\"))
            {
                text2 = text2.Substring(0, text2.Length - 1);
            }
            if (!text2.EndsWith("(x86)", StringComparison.InvariantCultureIgnoreCase))
            {
                text2 += " (x86)";
            }
            string text3 = text2 + "\\Reference Assemblies\\Microsoft\\Framework\\";
            bool flag = filePath.StartsWith(runtimeDirectory, StringComparison.InvariantCultureIgnoreCase);
            bool flag2 = filePath.StartsWith(text, StringComparison.InvariantCultureIgnoreCase);
            bool flag3 = filePath.StartsWith(text3, StringComparison.InvariantCultureIgnoreCase);
            string result;
            if (flag || flag2 || flag3)
            {
                if (File.Exists(filePath))
                {
                    if (flag)
                    {
                        result = filePath;
                        return result;
                    }
                    if (Environment.Version.Major == 2 && filePath.IndexOf("\\.NETFramework\\v4.0\\") == -1)
                    {
                        result = filePath;
                        return result;
                    }
                    if (Environment.Version.Major == 4 && filePath.IndexOf("\\Framework\\v3.0\\") == -1 &&
                        filePath.IndexOf("\\Framework\\v3.5\\") == -1)
                    {
                        result = filePath;
                        return result;
                    }
                }
                string fileName = Path.GetFileName(filePath);
                string text4 = Path.Combine(runtimeDirectory, fileName);
                if (File.Exists(text4))
                {
                    result = text4;
                    return result;
                }
                text4 = Path.Combine(runtimeDirectory, "wpf\\" + fileName);
                if (File.Exists(text4))
                {
                    result = text4;
                    return result;
                }
                if (Environment.Version.Major == 2)
                {
                    string text5 = Path.Combine(text, "v3.5\\" + fileName);
                    if (File.Exists(text5))
                    {
                        result = text5;
                        return result;
                    }
                    text5 = Path.Combine(text, "v3.0\\" + fileName);
                    if (File.Exists(text5))
                    {
                        result = text5;
                        return result;
                    }
                }
                else
                {
                    if (Environment.Version.Major == 4)
                    {
                        string text5 = Path.Combine(text, ".NETFramework\\v4.0\\" + fileName);
                        if (File.Exists(text5))
                        {
                            result = text5;
                            return result;
                        }
                        text5 = Path.Combine(text3, ".NETFramework\\v4.0\\" + fileName);
                        if (File.Exists(text5))
                        {
                            result = text5;
                            return result;
                        }
                    }
                }
            }
            result = filePath;
            return result;
        }

        public static string EncodeFolder(string p)
        {
            string result;
            if (string.IsNullOrEmpty(p))
            {
                result = p;
            }
            else
            {
                foreach (var current in _tokensToFolders)
                {
                    if (string.Equals(p, current.Value, StringComparison.OrdinalIgnoreCase) ||
                        p.StartsWith(current.Value + "\\", StringComparison.OrdinalIgnoreCase))
                    {
                        result = current.Key + p.Substring(current.Value.Length);
                        return result;
                    }
                }
                result = p;
            }
            return result;
        }
    }
}