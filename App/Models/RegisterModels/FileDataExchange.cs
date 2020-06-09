using System;
using System.Collections.Generic;
using System.IO;
using Core;
using Core.Extensions;
using Core.Helpers;
using DevExpress.XtraEditors.DXErrorProvider;
using GalaSoft.MvvmLight;
using Medical.AppLayer.Services;
using Medical.DataCore.partial;

namespace Medical.AppLayer.Models.RegisterModels
{
    public class FileDataExchange : ViewModelBase, IDXDataErrorInfo 
    {
        private readonly IFileService _fileService;
        private readonly IDataService _dataService;

        private Exception _error;

        public string FilePath { get; set; }
        public List<string> FilePathXml { get; set; }
        public string FileNameOms { get; set; }
        public List<string> FileNameXml { get; set; }

        public DateTime? Date { get; set; }
        public DateTime? AccountDate { get; set; }
        public int? TerritorySrc { get; set; }
        public int? TerritoryDest { get; set; }
        public string AccountNumber { get; set; }
        public int? Type { get; set; }
        public Exception Error
        {
            get { return _error; }
            set { _error = value; RaisePropertyChanged(()=>Error); }
        }

        public FileDataExchange(string path, 
            IFileService fileService,
            IDataService dataService)
        {
            FilePath = path;
            _fileService = fileService;
            _dataService = dataService;
            Init();
        }

        private void Init()
        {
            try
            {
                FileNameXml = new List<string>();
                FileNameOms = Path.GetFileName(FilePath);
                var unpackResult = _fileService.Unpack(FilePath, GlobalConfig.TempFolder);
                if (unpackResult.Success)
                {
                    FilePathXml = new List<string>(unpackResult.Data);
                    foreach (var file in FilePathXml)
                    {
                        var fileNameXml = Path.GetFileName(file);
                        FileNameXml.Add(fileNameXml);
                        var info = new RegisterEInfo();
                        info.SetInfo(fileNameXml);
                        TerritorySrc = info.SourceTerritory;
                        TerritoryDest = info.DestinationTerritory;

                        switch (info.Type)
                        {
                            case "R":
                                Type = (int)AccountType.GeneralPart;
                                break;
                            case "D":
                                Type = (int)AccountType.CorrectedPart;
                                break;
                            case "A":
                                Type = (int)AccountType.LogPart;
                                break;
                        }

                        using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            var registerResult = _dataService.Deserialize<DataCore.partial.Register>(stream);
                            if (registerResult.Success)
                            {
                                AccountNumber = registerResult.Data.Account.AccountNumber;
                                AccountDate = registerResult.Data.Account.AccountDate;
                                if (registerResult.Data.Account.Year.HasValue && registerResult.Data.Account.Month.HasValue)
                                {
                                    Date = new DateTime(registerResult.Data.Account.Year.Value, registerResult.Data.Account.Month.Value, 01);
                                }
                            }
                        }
                    }
                }
                else
                {
                    _error = unpackResult.LastError;
                }
            }
            catch (Exception exception)
            {
                _error = exception;
            }
            
            
        }

        #region IDXDataErrorInfo Members
        void IDXDataErrorInfo.GetPropertyError(string propertyName, ErrorInfo info)
        {
            
        }
        void IDXDataErrorInfo.GetError(ErrorInfo info)
        {
            if (_error.IsNotNull())
            {
                SetErrorInfo(info, "Ошибка обработки файла.\r\n{0}".F(_error), ErrorType.Warning);
            }
        }
        #endregion


        void SetErrorInfo(ErrorInfo info, string errorText, ErrorType errorType)
        {
            info.ErrorText = errorText;
            info.ErrorType = errorType;
        }
    }
}
