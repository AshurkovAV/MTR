using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Core;
using Core.DataStructure;
using Core.Extensions;
using Core.Infrastructure;
using Core.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Models.RegisterModels;
using Medical.AppLayer.Services;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Register.ViewModels
{
    public class FilesViewModel : ViewModelBase, IContextCommandContainer, IHash
    {
        private readonly IAppRemoteSettings _remoteSettings;
        private readonly IFileService _fileService;
        private readonly INotifyManager _notifyManager;
        private readonly IDockLayoutManager _dockLayoutManager;

        private int _selectedSection;
        private FileDataExchange _currentRow;
        private ObservableCollection<FileDataExchange> _fileList;

        #region IContextCommandContainer
        public ObservableCollection<IContextCommand> ContextCommands { get; set; }
        public string PageName { get; set; }
        #endregion

        #region IHash
        public string Hash
        {
            get { return typeof(FilesViewModel).FullName; }
        }
        #endregion

        public ObservableCollection<CommonTuple> Sections { get; set; }
        public ObservableCollection<FileDataExchange> FileList {
            get { return _fileList; }
            set { _fileList = value; RaisePropertyChanged(()=>FileList); }
        }

        private Dictionary<int, Action<int>> _fileLoadHandlers;
        private Dictionary<int, string> _statusConverter;
        private int? _selectedTerritory;

        public FileDataExchange CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged(() => CurrentRow);
            }
        }

        public int SelectedSection
        {
            get { return _selectedSection; }
            set
            {
                _selectedSection = value; RaisePropertyChanged(() => SelectedSection);
                RunLoadFilesHandler();
            }
        }

        public int? SelectedTerritory
        {
            get { return _selectedTerritory; }
            set
            {
                _selectedTerritory = value;
                RaisePropertyChanged("SelectedTerritory");
                RunLoadFilesHandler();
            }
        }

        public FilesViewModel(IAppRemoteSettings remoteSettings,
            IFileService fileService,
            INotifyManager notifyManager,
            IDockLayoutManager dockLayoutManager)
        {
            _remoteSettings = remoteSettings;
            _fileService = fileService;
            _notifyManager = notifyManager;
            _dockLayoutManager = dockLayoutManager;

            Init();
        }

        private void RunLoadFilesHandler()
        {
            if (SelectedSection.IsNotNull() && _fileLoadHandlers.ContainsKey(SelectedSection))
            {
                Task.Factory.StartNew(() => _dockLayoutManager.ShowOverlay(Constants.FileProcessingTitleMsg, Constants.PleaseWaitMsg, 0d))
                    .ContinueWith(action => _fileLoadHandlers[SelectedSection](SelectedSection))
                    .ContinueWith(action => _dockLayoutManager.HideOverlay());
            }
        }

        private void ProcessUnloadFiles(int status)
        {
            var result = new List<FileDataExchange>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                dynamic settings = _remoteSettings.Get(AppRemoteSettings.RegisterPath);
                var configName = "UnprocessedPath{0}".F(_statusConverter[status]);
                string unprocessedPath = settings[configName];
                var mask = _statusConverter[status] + _selectedTerritory.ToStringNullable("D2","");
                var files = new List<string>(Directory.GetFiles(unprocessedPath, mask + "*.oms", SearchOption.TopDirectoryOnly));
                files = files.Where(p => p.Contains(mask, StringComparison.OrdinalIgnoreCase)).ToList();
                if (files.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        result.Add(scope.Resolve<FileDataExchange>(new NamedParameter("path", files[i])));
                        _dockLayoutManager.SetOverlayProgress(files.Count, i);
                    }
                }
            }
            FileList = new ObservableCollection<FileDataExchange>(result);
        }

        private void ProcessLoadedFiles(int status)
        {
            var result = new List<FileDataExchange>();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                dynamic settings = _remoteSettings.Get(AppRemoteSettings.RegisterPath);
                var configName = "UnprocessedPath{0}".F(_statusConverter[status]);
                string unprocessedPath = settings[configName];
                string processedPattern = settings["ProcessedPattern"];
                var dirs = new List<string>(Directory.GetDirectories(unprocessedPath, processedPattern.F("*")));
                foreach (var dir in dirs)
                {
                    var mask = _statusConverter[status] + _selectedTerritory.ToStringNullable("D2", "");
                    var files = new List<string>(Directory.GetFiles(dir, mask + "*.oms", SearchOption.TopDirectoryOnly));
                    files = files.Where(p => p.Contains(mask, StringComparison.OrdinalIgnoreCase)).ToList();
                    if (files.Count > 0)
                    {
                        foreach (var file in files)
                        {
                            result.Add(scope.Resolve<FileDataExchange>(new NamedParameter("path", file)));
                        }
                    }
                }
            }
            FileList = new ObservableCollection<FileDataExchange>(result);
        }
        private void Init()
        {
            PageName = "Файлы реестров";
            ContextCommands = new ObservableCollection<IContextCommand>
            {
                new ContextCommand
                {
                    Id = "showFile",
                    Caption = "Показать в проводнике",
                    Command = new RelayCommand(ShowFile, CanShowFile),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/folder.png",
                    SmallGlyph = "../Resources/Icons/folder.png"
                },
                new ContextCommand
                {
                    Id = "viewFile",
                    Caption = "Просмотр",
                    Command = new RelayCommand(ViewFile, CanViewFile),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/eye.png",
                    SmallGlyph = "../Resources/Icons/eye.png"
                },
                new ContextCommand
                {
                    Id = "importFile",
                    Caption = "Импорт",
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/go-bottom-in.png",
                    SmallGlyph = "../Resources/Icons/go-bottom-in.png",
                    Command = new RelayCommand(ImportFile, CanImportFile),
                },
                new ContextCommand
                {
                    Id = "reloadFiles",
                    Caption = "Обновить",
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/view-refresh.png",
                    SmallGlyph = "../Resources/Icons/view-refresh.png",
                    Command = new RelayCommand(RefreshFiles),
                },
                new ContextCommand
                {
                    Id = "deleteFile",
                    Caption = "Удалить",
                    Page = "Редактирование",
                    LargeGlyph = "../Resources/Icons/edit-delete.png",
                    SmallGlyph = "../Resources/Icons/edit-delete.png",
                    Command = new RelayCommand(DeleteFile, CanDeleteFile),
                },
                new ContextCommand
                {
                    Id = "changeStatus",
                    Caption = "Статус",
                    Page = "Редактирование",
                    LargeGlyph = "../Resources/Icons/view-refresh-2.png",
                    SmallGlyph = "../Resources/Icons/view-refresh-2.png",
                    IsParent = true
                },
                new ContextCommand
                {
                    Id = "unprocessed",
                    Caption = "Не загружен",
                    LargeGlyph = "../Resources/Icons/emblem-important-red.png",
                    SmallGlyph = "../Resources/Icons/emblem-important-red.png",
                    Parent = "changeStatus",
                    Command = new RelayCommand<object>(ChangeStatus, CanChangeStatus),
                    CommandParameter = 1
                },
                new ContextCommand
                {
                    Id = "processed",
                    Caption = "Загружен",
                    LargeGlyph = "../Resources/Icons/task-complete.png",
                    SmallGlyph = "../Resources/Icons/task-complete.png",
                    Parent = "changeStatus",
                    Command = new RelayCommand<object>(ChangeStatus, CanChangeStatus),
                    CommandParameter = 2
                }
            };

            _fileLoadHandlers = new Dictionary<int, Action<int>>
            {
                { FileDataExchangeStatus.UnloadR.ToInt32(), ProcessUnloadFiles},
                { FileDataExchangeStatus.UnloadD.ToInt32(), ProcessUnloadFiles},
                { FileDataExchangeStatus.UnloadA.ToInt32(), ProcessUnloadFiles},
                { FileDataExchangeStatus.LoadedR.ToInt32(), ProcessLoadedFiles},
                { FileDataExchangeStatus.LoadedD.ToInt32(), ProcessLoadedFiles},
                { FileDataExchangeStatus.LoadedA.ToInt32(), ProcessLoadedFiles}
            };

            _statusConverter = new Dictionary<int, string>
            {
                { FileDataExchangeStatus.UnloadR.ToInt32(), "R"},
                { FileDataExchangeStatus.UnloadD.ToInt32(), "D"},
                { FileDataExchangeStatus.UnloadA.ToInt32(), "A"},
                { FileDataExchangeStatus.LoadedR.ToInt32(), "R"},
                { FileDataExchangeStatus.LoadedD.ToInt32(), "D"},
                { FileDataExchangeStatus.LoadedA.ToInt32(), "A"}
            };

            Sections = new ObservableCollection<CommonTuple>(FlagExtension.ToList<FileDataExchangeStatus>());
            SelectedSection = FileDataExchangeStatus.UnloadR.ToInt32();
            SelectedTerritory = 1;
        }

        private bool CanShowFile()
        {
            return CurrentRow.IsNotNull();
        }

        private void ShowFile()
        {
            _fileService.ShowFileInExplorer(CurrentRow.FilePath);
        }

        private bool CanChangeStatus(object arg)
        {
            return CurrentRow.IsNotNull() && CurrentRow.Error.IsNull() && SelectedSection.IsNotSimilarOddOrEven(arg.ToInt32()); 
        }

        private void ChangeStatus(object status)
        {
            dynamic settings = _remoteSettings.Get(AppRemoteSettings.RegisterPath);
            var configName = "UnprocessedPath{0}".F(_statusConverter[SelectedSection]);
            string unprocessedPath = settings[configName];
            string processedPattern = settings["ProcessedPattern"];

            var src = CurrentRow.FilePath;
            var dest = string.Empty;
            var statusFlag = (int) status;

            switch (statusFlag)
            {
                case  1:
                    dest = Path.Combine(unprocessedPath, CurrentRow.FileNameOms);
                    break;
                case 2:
                    var path = Path.GetDirectoryName(CurrentRow.FilePath);
                    if (CurrentRow.Date.HasValue)
                    {
                        var dir = Path.Combine(path, processedPattern.F(DateTime.Now.Year));
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                            
                        }
                        dest = Path.Combine(dir, CurrentRow.FileNameOms);
                    }
                    break;
            }

            if (src.IsNotNullOrWhiteSpace() && dest.IsNotNullOrWhiteSpace())
            {
                var moveResult = _fileService.MoveFile(src, dest);
                if (moveResult.Success)
                {
                    _notifyManager.ShowNotify("Статус изменен, файл перенесен");
                    RunLoadFilesHandler();
                }
            }
        }

        private bool CanDeleteFile()
        {
            return CurrentRow.IsNotNull();
        }

        private void DeleteFile()
        {
            var deleteResult = _fileService.DeleteFileWithAsk(CurrentRow.FilePath);
            if (deleteResult.Success)
            {
                _notifyManager.ShowNotify("Файл {0} успешно удален".F(CurrentRow.FileNameOms));
                RunLoadFilesHandler();
            }
        }

        private void RefreshFiles()
        {
            RunLoadFilesHandler();
        }

        private bool CanImportFile()
        {
            return CurrentRow.IsNotNull();
        }

        private void ImportFile()
        {
            _dockLayoutManager.ShowImportInterTerritorial(CurrentRow.FilePath);
        }

        private bool CanViewFile()
        {
            return CurrentRow.IsNotNull();
        }

        private void ViewFile()
        {
            var resultUnpack = _fileService.Unpack(CurrentRow.FilePath, GlobalConfig.TempFolder);
            if (resultUnpack.Success)
            {
                if (resultUnpack.Data.Any())
                {
                    foreach (var file in resultUnpack.Data)
                    {
                        var loadResult = _fileService.LoadTextFile(file, Encoding.GetEncoding("windows-1251"));
                        if (loadResult.Success)
                        {
                            _dockLayoutManager.ShowXmlEditor(Tuple.Create(loadResult.Data, Path.GetFileName(file)));
                        }
                    }
                }
            }
        }
    }
}
