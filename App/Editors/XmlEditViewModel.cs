using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Core.Extensions;
using Core.Infrastructure;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Service;

namespace Medical.AppLayer.Editors
{
    public class XmlEditViewModel : ViewModelBase, IHash
    {
        private readonly IFileService _fileService;
        private readonly INotifyManager _notifyManager;
        private readonly IMessageService _messageService;
        private readonly ITextService _textService;

        private string _editValue;

        private RelayCommand _saveAsCommand;
        private RelayCommand _saveAsOmsCommand;
        private RelayCommand _formatCommand;

        public string EditValue
        {
            get { return _editValue; }
            set
            {
                _editValue = value;
                RaisePropertyChanged(()=>EditValue);
            }
        }

        #region IHash
        public string Hash
        {
            get { return typeof(XmlEditViewModel).FullName + InnerName; }
        }

        public string InnerName { get; set; }

        #endregion
        
        public XmlEditViewModel(IFileService fileService, 
            IMessageService messageService, 
            INotifyManager notifyManager,
            ITextService textService,
            string value)
        {
            _fileService = fileService;
            _messageService = messageService;
            _notifyManager = notifyManager;
            _textService = textService;
            EditValue = value;
        }


        public ICommand SaveAsOmsCommand => _saveAsOmsCommand ??
                                              (_saveAsOmsCommand = new RelayCommand(SaveAsOms, CanSaveAsOms));

        private bool CanSaveAsOms()
        {
            return EditValue.IsNotNullOrWhiteSpace();
        }

        private void SaveAsOms()
        {
            try
            {
                var selectFileResult = _fileService.ShowSaveFileDialog(".oms", "Файлы oms (.oms)|*.oms", InnerName.Replace(".xml",""));
                if (selectFileResult.Success)
                {
                    var xmlFileName = selectFileResult.Data.Replace("oms", "xml");
                    
                    var saveResult = _fileService.SaveTextFileWithEncoding(xmlFileName, EditValue, Encoding.GetEncoding("windows-1251"));
                    if (saveResult.Success)
                    {
                        var packResult = _fileService.Pack(selectFileResult.Data, new List<string> { xmlFileName }, true);
                        if (packResult.Success)
                        {
                            _notifyManager.ShowNotify("Данные успешно записаны.");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При сохранении данных XML произошло исключение", typeof(XmlEditViewModel));
            }
        }

        public ICommand SaveAsCommand => _saveAsCommand ??
                                              (_saveAsCommand = new RelayCommand(SaveAs, CanSaveAs));

        private bool CanSaveAs()
        {
            return EditValue.IsNotNullOrWhiteSpace();
        }

        private void SaveAs()
        {
            try
            {
                var selectFileResult = _fileService.ShowSaveFileDialog(".xml", "Файлы xml (.xml)|*.xml", InnerName);
                if (selectFileResult.Success)
                {
                    var saveResult = _fileService.SaveTextFileWithEncoding(selectFileResult.Data, EditValue, Encoding.GetEncoding("windows-1251"));
                    if (saveResult.Success)
                    {
                        _notifyManager.ShowNotify("Данные успешно записаны.");
                    }
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При сохранении данных XML произошло исключение", typeof(XmlEditViewModel));
            }
        }

        public ICommand FormatCommand => _formatCommand ??

                                              (_formatCommand = new RelayCommand(Format, CanFormat));
        private bool CanFormat()
        {
            return _textService.IsValidXml(EditValue).Success;
        }

        private void Format()
        {
            var formatResult = _textService.FormatXml(EditValue);
            if (formatResult.Success)
            {
                EditValue = formatResult.Data;
            }
        }
    }
}
