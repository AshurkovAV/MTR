using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using Core;
using Core.Extensions;
using Core.Infrastructure;
using Core.Services;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Linq;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Services;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Register.ViewModels
{
    public class ExchangeViewModel : ViewModelBase, IContextCommandContainer, IHash
    {
        private readonly IMessageService _messageService;
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;
        private readonly IFileService _fileService;
        private readonly IDockLayoutManager _dockLayoutManager;

        private PLinqExchangeList _factExchangeList;
        private FactExchange _currentRow;

        #region IContextCommandContainer

        public ObservableCollection<IContextCommand> ContextCommands { get; set; }
        public string PageName { get; set; }

        #endregion

        #region IHash

        public string Hash
        {
            get { return typeof (ExchangeViewModel).FullName; }
        }

        #endregion

        public ExchangeViewModel(PLinqExchangeList list,
            IMessageService messageService,
            IMedicineRepository repository,
            INotifyManager notifyManager,
            IFileService fileService,
            IDockLayoutManager dockLayoutManager)
        {
            ExchangeList = list;
            _messageService = messageService;
            _repository = repository;
            _notifyManager = notifyManager;
            _fileService = fileService;
            _dockLayoutManager = dockLayoutManager;
            Initialize();
        }

        private void Initialize()
        {
            PageName = "Журнал информационного обмена";
            ContextCommands = new ObservableCollection<IContextCommand>
            {
                new ContextCommand
                {
                    Id = "openExchange",
                    Caption = "Открыть",
                    Command = new RelayCommand(Open, CanOpen),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/folder.png",
                    SmallGlyph = "../Resources/Icons/folder.png"
                },
                new ContextCommand
                {
                    Id = "reloadExchange",
                    Caption = "Обновить",
                    Command = new RelayCommand(Reload),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/view-refresh.png",
                    SmallGlyph = "../Resources/Icons/view-refresh.png"
                },
                new ContextCommand
                {
                    Id = "saveExchangeFile",
                    Caption = "Сохранить в файл",
                    Command = new RelayCommand(Save, CanSave),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/document-save-as.png",
                    SmallGlyph = "../Resources/Icons/document-save-as.png"
                },
                new ContextCommand
                {
                    Id = "deleteExchangeFile",
                    Caption = "Удалить",
                    Command = new RelayCommand(DeleteExchange, CanDeleteExchange),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/edit-delete.png",
                    SmallGlyph = "../Resources/Icons/edit-delete.png"
                },
                new ContextCommand
                {
                    Id = "goToAccount",
                    Caption = "Перейти на территориальный счет",
                    Command = new RelayCommand(GoToTerritoryAccount, CanGoToTerritoryAccount),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/go-jump.png",
                    SmallGlyph = "../Resources/Icons/go-jump.png"
                },
                new ContextCommand
                {
                    Id = "goToAccount",
                    Caption = "Перейти на счет МО",
                    Command = new RelayCommand(GoToMedicalAccount, CanGoToMedicalAccount),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/go-jump2.png",
                    SmallGlyph = "../Resources/Icons/go-jump2.png"
                }
            };
        }

        private void Reload()
        {
            using (ILifetimeScope scope = Di.I.BeginLifetimeScope())
            {
                if (ExchangeList != null && ExchangeList.Predicate != null)
                {
                    ExchangeList = scope.Resolve<PLinqExchangeList>(new NamedParameter("predicate", ExchangeList.Predicate));
                }
                else
                {
                    ExchangeList = scope.Resolve<PLinqExchangeList>();
                }
            }
        }

        public PLinqExchangeList ExchangeList
        {
            get { return _factExchangeList; }
            set
            {
                _factExchangeList = value;
                RaisePropertyChanged(() => ExchangeList);
            }
        }

        public FactExchange CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged(() => CurrentRow);
            }
        }

        private bool CanOpen()
        {
            return CurrentRow != null;
        }

        private void Open()
        {
            string tmpPath = GlobalConfig.TempFolder;
            string fileName = Path.Combine(tmpPath, CurrentRow.FileName);
            OperationResult result = _fileService.SaveBinaryFile(fileName, CurrentRow.Data);
            if (result.Success)
            {
                OperationResult<IEnumerable<string>> resultUnpack = _fileService.Unpack(fileName);
                if (resultUnpack.Success)
                {
                    if (resultUnpack.Data.Any())
                    {
                        string innerFile = resultUnpack.Data.FirstOrDefault();
                        OperationResult<string> loadResult = _fileService.LoadTextFile(innerFile, Encoding.GetEncoding("windows-1251"));
                        if (loadResult.Success)
                        {
                            _dockLayoutManager.ShowXmlEditor(Tuple.Create(loadResult.Data, CurrentRow.FileName));
                        }
                    }
                }
            }
        }

        private bool CanSave()
        {
            return CurrentRow != null;
        }

        private void Save()
        {
            if (CurrentRow == null)
            {
                return;
            }
            try
            {
                OperationResult result = _fileService.SaveFileWithDialog(".oms", "Файлы реестров (.oms)|*.oms|Файлы xml (.xml)|*.xml", CurrentRow.Data, FileType.Binary, CurrentRow.FileName);
                if (result.Success)
                {
                    _notifyManager.ShowNotify("Данные успешно записаны.");
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При сохранении данных информационного обмена произошло исключение", typeof (ExchangeViewModel));
            }
        }

        private bool CanDeleteExchange()
        {
            return false; //CurrentRow != null;
        }

        private void DeleteExchange()
        {
            try
            {
                if (CurrentRow == null)
                {
                    return;
                }
                if (_messageService.AskQuestion("Вы действительно хотите удалить запись?", "Внимание!"))
                {
                    TransactionResult deleteResult = _repository.DeleteExchange(CurrentRow.ExchangeId);
                    if (deleteResult.Success)
                    {
                        Reload();
                        _notifyManager.ShowNotify("Данные информационного обмена успешно удалены.");
                    }
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При удалении данных информационного обмена произошло исключение", typeof (ExchangeViewModel));
            }
        }

        private bool CanGoToTerritoryAccount()
        {
            return CurrentRow.IsNotNull() && Constants.InterTerritorialAccountTypes.Contains(CurrentRow.Type);
        }

        private void GoToTerritoryAccount()
        {
            //TODO GoToTerritoryAccount
        }

        private bool CanGoToMedicalAccount()
        {
            return CurrentRow.IsNotNull() && Constants.LocalAccountTypes.Contains(CurrentRow.Type);
        }

        private void GoToMedicalAccount()
        {
            //TODO GoToMedicalAccount
        }
    }
}