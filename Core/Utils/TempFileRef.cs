using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Utils
{
    public class TempFileRef
    {
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
        public readonly string FileName;
        public readonly string FullPath;
        private int _deleteAttempt;

        public TempFileRef(string fileOrPath)
        {
            FullPath = Path.IsPathRooted(fileOrPath) ? fileOrPath : Path.Combine(GlobalConfig.TempFolder, fileOrPath);
            FileName = Path.GetFileName(FullPath);
        }

        public static TempFileRef GetRandom(string prefix, string suffix)
        {
            string fileOrPath = prefix + "_" + GetRandomName(6) + suffix;
            return new TempFileRef(fileOrPath);
        }

        public static string GetRandomName(int length)
        {
            var array = new byte[length];
            lock (Rng)
            {
                Rng.GetBytes(array);
            }
            var array2 = new char[array.Length];
            int num = 0;
            byte[] array3 = array;
            foreach (byte b in array3)
            {
                array2[num++] = (char) (97 + b%26);
            }
            return new string(array2);
        }

        public static void DeleteAll()
        {
            Task.Factory.StartNew(() =>
                                      {
                                          Thread.Sleep(100);
                                          try
                                          {
                                              Directory.Delete(GlobalConfig.TempFolder, true);
                                          }
                                          catch
                                          {
                                              Thread.Sleep(300);
                                              try
                                              {
                                                  Directory.Delete(GlobalConfig.TempFolder, true);
                                              }
                                              catch
                                              {
                                              }
                                          }
                                          string[] files = Directory.GetFiles(GlobalConfig.TempFolderBase, "query_*.*");
                                          foreach (string path in files)
                                          {
                                              try
                                              {
                                                  File.Delete(path);
                                              }
                                              catch
                                              {
                                              }
                                          }
                                          files = Directory.GetFiles(GlobalConfig.TempFolderBase, "TypedDataContext_*.*");
                                          foreach (string path in files)
                                          {
                                              try
                                              {
                                                  File.Delete(path);
                                              }
                                              catch
                                              {
                                              }
                                          }
                                      });
        }

        ~TempFileRef()
        {
            try
            {
                if (File.Exists(FullPath))
                {
                    try
                    {
                        File.Delete(FullPath);
                    }
                    catch
                    {
                        if (_deleteAttempt++ < 5)
                        {
                            GC.ReRegisterForFinalize(this);
                        }
                        else
                        {
                            //TODO Log
                            //Log.Write("Unable to delete file " + this.FullPath + " in finalizer");
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}