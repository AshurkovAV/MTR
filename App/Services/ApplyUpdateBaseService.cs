using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BLToolkit.Data;
using Core.Helpers;
using Core.Infrastructure;
using Core.Services;
using DataModel;
using Ionic.Zip;
using Medical.AppLayer.Managers;
using Medical.CoreLayer.Helpers;
using Medical.CoreLayer.Service;

namespace Medical.AppLayer.Services
{
    public class ApplyUpdateBaseService: IApplyUpdateBaseService, IErrorMessage, ITestable
    {
        private readonly IAppShareSettings _settings;
        private readonly IMessageService _messageService;
        private readonly IOverlayManager _overlayManager;
        public ApplyUpdateBaseService(
            IAppShareSettings settings,
            IOverlayManager overlayManager,
            IMessageService messageService)
        {
            _settings = settings;
            _overlayManager = overlayManager;
            _messageService = messageService;
        }

        public bool Run(string sqlScript)
        {
            try
            {
                dynamic config = _settings.Get("database");
                using (var db = new DbManager(((string)config.Provider).ToDataProvider(), (string)config.ConnectionString))
                {
                    if (ConnectionState.Open == db.Connection.State)
                    {
                        db.SetCommand(sqlScript).ExecuteNonQuery();
                        return true;
                        //if (CryptoHelpers.ConfirmPassword(password, Convert.FromBase64String(user.Pass),
                        //    Convert.FromBase64String(user.Salt)))
                        //{
                        //    UserInfo.SetUser(user);
                        //    Messenger.Default.Send(UserInfo);
                        //    return true;
                        //}
                    }
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Ошибка при попытке выполнить обновление базы", typeof(ApplyUpdateBaseService));
            }

            return false;
        }

        public string UnZipFileToSqlScript(byte[] data)
        {
            var ms1 = new MemoryStream();
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(data))
                {
                    using (ZipFile zip = ZipFile.Read(memoryStream))
                    {
                        foreach (ZipEntry zipEntry in zip)
                        {
                            if (zipEntry.FileName.Contains("sql"))
                            {
                                zipEntry.Extract(ms1);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _overlayManager.SetOverlayTitle(ex.Message);
                Thread.Sleep(50);
            }

            string result = Encoding.Default.GetString(ms1.ToArray());
            return result;
        }

        public OperationResult<List<string>> UnZipFileToFile(byte[] data)
        {
            var result = new OperationResult<List<string>>();
            
            try
            {
                using (MemoryStream memoryStream = new MemoryStream(data))
                {
                    using (ZipFile zip = ZipFile.Read(memoryStream))
                    {
                        result.Data = new List<string>();
                        foreach (ZipEntry zipEntry in zip)
                        {
                            if (zipEntry.FileName.Contains(".dll")|| zipEntry.FileName.Contains(".exe"))
                            {
                                var ms1 = new MemoryStream();
                                zipEntry.Extract(ms1);
                                
                                string path = Directory.GetCurrentDirectory();
                                if (!Directory.Exists(path + @"\Update"))
                                {
                                    Directory.CreateDirectory(path + @"\Update");
                                }
                                using (FileStream file = new FileStream(path + @"\Update\" + zipEntry.FileName, FileMode.Create, System.IO.FileAccess.Write))
                                {
                                    result.Data.Add(zipEntry.FileName);
                                    ms1.WriteTo(file);
                                    ms1.Close();
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _overlayManager.SetOverlayTitle(ex.Message);
                Thread.Sleep(50);
                result.AddError(ex.Message);
            }
            
            return result;
        }

        public string ErrorMessage { get; }
        public bool Test()
        {
            //TODO self testing
            return true;
        }

        
    }
}
