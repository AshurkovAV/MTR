using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Autofac;
using Autofac.Core;
using BLToolkit.Data;
using Core;
using Core.Extensions;
using Core.Infrastructure;
using Core.Infrastructure.Compiler;
using Core.Linq;
using Core.Services;
using DataModel;
using DevExpress.Xpf.Docking;
using System.Collections.ObjectModel;
using System.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Medical.AppLayer.Admin.ViewModels;
using Medical.AppLayer.Admin.Views;
using Medical.AppLayer.Auth.ViewModels;
using Medical.AppLayer.Auth.Views;
using Medical.AppLayer.Classifiers.ViewModels;
using Medical.AppLayer.Classifiers.Views;
using Medical.AppLayer.Economic.ViewModels;
using Medical.AppLayer.Economic.Views;
using Medical.AppLayer.Editors;
using Medical.AppLayer.Examination.ViewModels;
using Medical.AppLayer.Examination.Views;
using Medical.AppLayer.Linq;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Migration.ViewModels;
using Medical.AppLayer.Models;
using Medical.AppLayer.Models.OperatorModels;
using Medical.AppLayer.Operator.ViewModel;
using Medical.AppLayer.Operator.Views;
using Medical.AppLayer.Processing.ViewModels;
using Medical.AppLayer.Processing.Views;
using Medical.AppLayer.Register.ViewModels;
using Medical.AppLayer.Register.Views;
using Medical.AppLayer.Report.ViewModels;
using Medical.AppLayer.Report.Views;
using Medical.AppLayer.Search.ViewModels;
using Medical.AppLayer.Search.Views;
using Medical.AppLayer.Services;
using Medical.AppLayer.Settings.ViewModels;
using Medical.AppLayer.StartPage.ViewModels;
using Medical.AppLayer.StartPage.Views;
using Medical.CoreLayer.Helpers;
using Medical.CoreLayer.Models.Config;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;
using MedicineNext.Internal;
using MedicineNext.Internal.MVVM;
using DataException = BLToolkit.Data.DataException;
using DispatcherHelper = GalaSoft.MvvmLight.Threading.DispatcherHelper;
using MedicalAccountView = Medical.AppLayer.Register.Views.MedicalAccountView;
using TerritoryAccountView = Medical.AppLayer.Register.Views.TerritoryAccountView;

namespace MedicineNext {

    /// <summary>
    /// Менеджер управления окнами приложения
    /// </summary>
    public class DockLayoutManager : ViewModelBase, IDockLayoutManager
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        private readonly IOverlayManager _overlayManager;
        private IReportService _reportService;
        private ICommonService _commonService;

        private ObservableCollection<WorkspaceViewModel> _workspaces;
        private ObservableCollection<CategoryModel> _categories;
        private int _selectedTabIndex;
        private bool _inProgress;
        private WaitIndicatorModel _overlayModel;

        private RelayCommand _exitCommand;
        private RelayCommand _testCommand;
        private RelayCommand _saveCommand;
        private RelayCommand<object> _selectedItemCommand;

        public DatabaseConfigModel DatabaseConfig { get; set; }
        public MigrationsViewModel Migrations { get; set; }
        public SettingsViewModel Settings { get; set; }
        
        public ObservableCollection<CategoryModel> Categories
        {
            get { return _categories ?? (_categories = new ObservableCollection<CategoryModel>()); }
            set
            {
                _categories = value;
                RaisePropertyChanged(() => Categories);
            }
        }

        public WorkspaceViewModel SelectedItem
        {
            get { return Workspaces.FirstOrDefault(p=>p.IsActive); }
        }

        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                _selectedTabIndex = value;
                RaisePropertyChanged(() => SelectedTabIndex);
            }
        }

        //Имя пользователя в статусе приложения
        public string UserName
        {
            get { return _userService != null ? _userService.UserName : string.Empty; }
        }

        /// <summary>
        /// Строка подключения в статусе приложения
        /// </summary>
        public string DatabaseConnection
        {
            get { return DatabaseConfig != null ? "Адрес сервера: {0} База данных: _{1}".F(DatabaseConfig.DataSource, DatabaseConfig.Database) : string.Empty; }
            
        }

        /// <summary>
        /// Название подлючения
        /// </summary>
        public string Name
        {
            get { return DatabaseConfig != null ? DatabaseConfig.Name : string.Empty; }
            set { DatabaseConfig._data.Name = value; RaisePropertyChanged(() => Name); }
        }

        public string DataSource
        {
            get { return DatabaseConfig != null ? DatabaseConfig._data.DataSource : string.Empty; }
            set { DatabaseConfig._data.DataSource = value; RaisePropertyChanged(() => DataSource); }
        }
        public string Database
        {
            get { return DatabaseConfig != null ? DatabaseConfig._data.Database : string.Empty; }
            set { DatabaseConfig._data.Database = value; RaisePropertyChanged(() => Database); }
        }
        public string Username
        {
            get { return DatabaseConfig != null ? DatabaseConfig._data.Username : string.Empty; }
            set { DatabaseConfig._data.Username = value; RaisePropertyChanged(() => Username); }
        }
        public string Password
        {
            get { return DatabaseConfig != null ? DatabaseConfig._data.Password : string.Empty; }
            set { DatabaseConfig._data.Password = value; RaisePropertyChanged(() => Password); }
        }
        public string Provider
        {
            get { return DatabaseConfig != null ? DatabaseConfig._data.Provider : string.Empty; }
            set { DatabaseConfig._data.Provider = value; RaisePropertyChanged(() => Provider); }
        }
        public bool IsWindowsAuth
        {
            get { return DatabaseConfig != null ? DatabaseConfig._data.IsWindowsAuth : false; }
            set { DatabaseConfig._data.IsWindowsAuth = value; RaisePropertyChanged(() => IsWindowsAuth); }
        }
        public int Timeout
        {
            get { return DatabaseConfig != null ? DatabaseConfig._data.Timeout : 0; }
            set { DatabaseConfig._data.Timeout = value; RaisePropertyChanged(() => Timeout); }
        }

        //public ObservableCollection<object> Sections
        //{
        //    get { return Settings != null ? Settings.Sections : null; }
        //    set { Settings.Sections = value; RaisePropertyChanged(() => Sections); }
        //}

        /// <summary>
        /// Видимость главного меню
        /// </summary>
        public string IsRibbonVisible
        {
            get
            {
                if (_userService == null)
                {
                    return "Collapsed";
                }
                return _userService.IsUserLogged() ? "Visible" : "Collapsed";
            }
        }

        public bool InProgress
        {
            get { return _inProgress; }
            set
            {
                _inProgress = value;
                RaisePropertyChanged(() => InProgress);
            }
        }
        public WaitIndicatorModel OverlayModel
        {
            get { return _overlayModel ?? (_overlayModel = new WaitIndicatorModel()); }
        }


        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                if (_workspaces == null)
                {
                    _workspaces = new ObservableCollection<WorkspaceViewModel>();
                    _workspaces.CollectionChanged += OnWorkspacesChanged;
                }
                return _workspaces;
            }
        }

        void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                    workspace.RequestClose += OnWorkspaceRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.OldItems)
                    workspace.RequestClose -= OnWorkspaceRequestClose;
        }

        void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkspaceRequestClose(sender as WorkspaceViewModel);
        }

        public void WorkspaceRequestClose(WorkspaceViewModel workspace)
        {
            if (workspace is PanelWorkspaceViewModel)
            {
                if (workspace.AskClose)
                {
                    if (!_messageService.AskQuestion("Вы действительно хотите закрыть {0}?".F(workspace.DisplayName), "Внимание!"))
                    {
                        return;
                    }
                }

                workspace.IsClosed = true;
                if (workspace is DocumentViewModel)
                {
                    workspace.Dispose();
                    Workspaces.Remove(workspace);
                }
            }
        }

        public ICommand SelectedItemCommand
        {
            get { return _selectedItemCommand ?? (_selectedItemCommand = new RelayCommand<object>(SelectedItemUpdate)); }
        }

        private void SelectedItemUpdate(object data)
        {
            var content = Workspaces.FirstOrDefault(p => p.IsActive);
            if (content != null)
            {
                SetContextMenu(content.IsClosed ? null : content);
            }
            else
            {
                SetContextMenu(null);
            }
        }


        public ICommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new RelayCommand(Exit)); }
        }

        public ICommand TestCommand => _testCommand ?? (_testCommand = new RelayCommand(Test));

        private void InProgressSwitch()
        {
            InProgress = !InProgress;
        }
        private void Test()
        {
            Task.Factory.StartNew(InProgressSwitch).ContinueWith(p =>
            {
                try
                {
                    using (var db = new DbManager(DatabaseConfig.Provider.ToDataProvider(), DatabaseConfig.ConnectionString))
                    {
                        if (ConnectionState.Open == db.Connection.State)
                        {
                            _messageService.ShowMessage(CoreMessages.ConnectionSuccess);
                        }
                    }
                }
                catch (DataException exception)
                {
                    _messageService.ShowException(exception, "Ошибка", typeof(DatabaseConfigModel));
                }
            }).ContinueWith(p => InProgressSwitch());
            //DatabaseConfig.TestCommand;
        }

        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(Save));

        private void Save()
        {
            DatabaseConfig._shareSettings.Put("database", DatabaseConfig._data);
        }



        private void Exit()
        {
            Application.Current.MainWindow.Close();
        }

        public DockLayoutManager(
            IUserService userService, 
            IMessageService messageService,
            IOverlayManager overlayManager)
        {
            _userService = userService;
            _messageService = messageService;
            _overlayManager = overlayManager;

            Initialize();
        }

        private void Initialize()
        {
            //Регистрация вызова HandleLoginInfo с помощью Messenger (вызов в IUserService в LoginUser(string login, string password))
            Messenger.Default.Register<UserInfoModel>(this, HandleLoginInfo);
            //Обновляем видимость меню
            
            RaisePropertyChanged(() => IsRibbonVisible);
        }

        private void HandleLoginInfo(UserInfoModel info)
        {
            
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var parameters = Environment.GetCommandLineArgs();
                //Регистрация сервисов зависимых от базы данных
                var builder = new ContainerBuilder();

                builder.RegisterType<MedicineRepository>().As<IMedicineRepository>().SingleInstance();
                builder.RegisterType<MigrationService>().As<IMigrationService>().SingleInstance();
                builder.RegisterType<AppRemoteSettings>().As<IAppRemoteSettings>().SingleInstance();
                builder.RegisterType<UserRemoteSettings>().As<IUserRemoteSettings>().SingleInstance();
                builder.RegisterType<ElmedicineRepository>().As<IElmedicineRepository>().SingleInstance();
                builder.RegisterType<TortillaRepository>().As<ITortillaRepository>().SingleInstance();
                builder.RegisterType<MedicineLogRepository>().As<IMedicineLogRepository>().SingleInstance();
                builder.RegisterType<FfomsReportRepository>().As<IFfomsReportRepository>().SingleInstance();

                Di.Update(builder);

                using (var scope = Di.I.BeginLifetimeScope())
                {
                    DatabaseConfig = scope.Resolve<DatabaseConfigModel>();
                    Migrations = scope.Resolve<MigrationsViewModel>();
                    
                    //Параметр запуска программы для генерации скриптов в случае необходимости
                    if (parameters.Contains("/generate"))
                    {
                        //TODO cache
                        //Проверка по дате генерации не работает, т.е. скрипты пересоздаются. Нужно считать хеш по содержимому.
                        var scriptGenerator = scope.Resolve<IScriptCodeGenerator>();
                        scriptGenerator.GenerateSettingsScriptsCode();
                    }

                    var compiler = scope.Resolve<IScriptCompiler>();
                    while (!compiler.Compile(true, true))
                    {
                        throw new Exception("Scripts: One or more scripts failed to compile or no script files were found.");
                    }

                    compiler.Invoke("Configure");
                    compiler.Invoke("Initialize");

                    Settings = scope.Resolve<SettingsViewModel>();
                    _reportService = scope.Resolve<IReportService>();
                    _commonService = scope.Resolve<ICommonService>();

                    var remoteSettings = scope.Resolve<IAppRemoteSettings>();
                    var remoteSettingsAll = remoteSettings.GetAll();
                    if (remoteSettingsAll == null)
                    {
                        throw new Exception("Настройки в БД отсутствуют!");
                    }
                    else
                    {
                        foreach (dynamic settings in remoteSettingsAll)
                        {
                            if (_commonService.IsDatabaseConnectionString(settings.Value))
                            {
                                var connectionString = _commonService.ConstructDatabaseConnectionString(settings.Value);
                                DbManager.AddConnectionString((string)settings.Value.Provider, settings.Key, connectionString);
                            }
                        }
                    }
                    TerritoryService._remoteSettings = remoteSettings;

                    //TODO User settings
                    //Хранение индивидуальных настроек пользователя
                }

                RaisePropertyChanged(() => IsRibbonVisible);
                RaisePropertyChanged(() => UserName);
                RaisePropertyChanged(() => DatabaseConnection);
                RaisePropertyChanged(() => Name);
                RaisePropertyChanged(() => DataSource);
                RaisePropertyChanged(() => Database);
                RaisePropertyChanged(() => Username);
                RaisePropertyChanged(() => Provider);
                RaisePropertyChanged(() => Password);
                RaisePropertyChanged(() => IsWindowsAuth);
                RaisePropertyChanged(() => Timeout);
                RaisePropertyChanged(() => Settings);
                WorkspaceRequestClose(Workspaces.FirstOrDefault(p => p.IsActive = true));
                
                ShowDashboard();
            });
        }

        public void AddWorkspace(object workspace)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (workspace is PanelWorkspaceViewModel)
                {
                    var workspaceTmp = workspace as PanelWorkspaceViewModel;
                    WorkspaceViewModel tmp;
                    var exist = Workspaces.FirstOrDefault(p => p.Content.GetType() == workspaceTmp.Content.GetType());
                    if (workspaceTmp.IsSingle && exist != null)
                    {
                        SetActiveWorkspace(exist);
                        tmp = exist;
                    }
                    else
                    {
                        var hash = workspaceTmp.Hash();
                        var hashed = Workspaces.FirstOrDefault(p => p.Hash() == hash);
                        if (hashed == null)
                        {
                            Workspaces.Add(workspaceTmp);
                            workspaceTmp.IsClosed = false;
                            tmp = workspaceTmp;
                        }
                        else
                        {
                            tmp = hashed;
                        }
                    }
                    SetActiveWorkspace(tmp);
                    SetContextMenu(tmp);
                }
            });
        }


        public void ShowOperator(int id, OperatorMode mode, IEnumerable<OperatorAction> operatorActions = null)
        {
            var titleResult = _commonService.GetTitleByIdForOperatorMode(id, mode);
            if (mode == OperatorMode.Zlocal || mode == OperatorMode.ZInterTerritorial || mode == OperatorMode.ZInterTerritorialError || mode == OperatorMode.ZLocalError)
            {
                var document = new ZslOperatorViewContentModel(id, mode, titleResult, operatorActions) { DisplayName = titleResult, InnerName = titleResult, AskClose = true };
                AddWorkspace(document);
            }
            else
            {
                var document = new OperatorViewContentModel(id, mode, titleResult, operatorActions) { DisplayName = titleResult, InnerName = titleResult, AskClose = true };
                AddWorkspace(document);
            }
            
        }



        public void ShowReport()
        {
            var document = new ReportViewContentModel{ DisplayName = "Отчеты" };
            AddWorkspace(document);
        }

        public void ShowPreparedReport()
        {
            var document = new PreparedReportViewContentModel { DisplayName = "Готовые отчеты" };
            AddWorkspace(document);
        }

        public void ShowCriterion()
        {
            var document = new CriterionViewContentModel { DisplayName = "Критерии экспертизы" };
            AddWorkspace(document);
        }

        public void ShowEconomicReport()
        {
            var document = new EconomicReportViewContentModel { DisplayName = "Отчеты экономистов" };
            AddWorkspace(document);
        }

        private void SetContextMenu(WorkspaceViewModel workspace)
        {
            var category = Categories.FirstOrDefault(p => p.IsContext);
            if (category != null && workspace != null && workspace.DataContext != null && category.GetType() == workspace.DataContext.GetType())
            {
                return;
            }

            Categories.Remove(category);

            if (workspace?.DataContext is IContextCommandContainer)
            {
                var contextCommandContainer = (IContextCommandContainer) workspace.DataContext;
                
                
                var newCategory = new ContextCategoryModel
                {
                    IsDefault = false,
                    IsVisible = true,
                    IsContext = true,
                    Typo = workspace.Content.GetType()
                };

                Categories.Add(newCategory);
                
                var contextPage = new PageModel { Name = contextCommandContainer.PageName, IsSelected = true };

                contextCommandContainer.ContextCommands.ThrowIfArgumentIsNull("СontextCommandContainer не может быть пустым!");

                contextCommandContainer.ContextCommands
                    .GroupBy(p => p.Page)
                    .Select(r => new {r.Key, Commands = r.Select(p => p)})
                    .ForEachAction(s =>
                    {
                        if (s.Key.IsNotNullOrEmpty())
                        {
                            var groupModel = new PageGroupModel {Name = s.Key};
                            contextPage.Groups.Add(groupModel);
                            foreach (var command in s.Commands)
                            {
                                var commandCopy = command;
                                if (!commandCopy.IsParent)
                                {
                                    var commandModel = new CommandModel
                                    {
                                        Caption = commandCopy.Caption,
                                        Command = commandCopy.Command,
                                        LargeGlyph = Utils.GlyphHelper.GetGlyph(commandCopy.LargeGlyph),
                                        SmallGlyph = Utils.GlyphHelper.GetGlyph(commandCopy.SmallGlyph),
                                        CommandParameter = commandCopy.CommandParameter
                                    };
                                    groupModel.Commands.Add(commandModel);
                                }
                                else
                                {
                                    var groupCommandModel = new MyGroupCommand
                                    {
                                        Caption = commandCopy.Caption,
                                        LargeGlyph = Utils.GlyphHelper.GetGlyph(commandCopy.LargeGlyph),
                                        SmallGlyph = Utils.GlyphHelper.GetGlyph(commandCopy.SmallGlyph)
                                    };

                                    contextCommandContainer.ContextCommands.Where(p => p.Parent == commandCopy.Id).ForEachAction(
                                        p =>
                                        {
                                            var parentCommand = new MyParentCommand
                                            {
                                                Caption = p.Caption,
                                                Command = p.Command,
                                                LargeGlyph = Utils.GlyphHelper.GetGlyph(p.LargeGlyph),
                                                SmallGlyph = Utils.GlyphHelper.GetGlyph(p.SmallGlyph),
                                                CommandParameter = p.CommandParameter
                                            };
                                            groupCommandModel.Commands.Add(parentCommand);
                                        });
                                    groupModel.Commands.Add(groupCommandModel);
                                }
                            }
                        }
                    });

                newCategory.Pages.Add(contextPage);
            }
        }

        void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            Debug.Assert(Workspaces.Contains(workspace));
            workspace.IsActive = true;
        }

        public object GetActiveWorkspaceDataContext()
        {
            var active = Workspaces.FirstOrDefault(p => p.IsActive);
            if (active.IsNotNull())
            {
                return active.DataContext;
            }
            return null;
        }

        public void Clear()
        {
            foreach (CategoryModel cat in Categories)
            {
                cat.Clear();
            }
            Categories.Clear();
        }


        #region Overlay

        /// <summary>
        /// Показать оверлей
        /// </summary>
        public void ShowOverlay()
        {
            ShowOverlay(Constants.OperationBeginTitleMsg, Constants.PleaseWaitMsg);
        }

        /// <summary>
        /// Показать оверлей с настройками
        /// </summary>
        /// <param name="title">заголовок</param>
        /// <param name="message">сообщение</param>
        /// <param name="progress">прогресс (если указан, то прогресс бар будет со значением)</param>
        public void ShowOverlay(string title, string message, double progress = Double.NaN)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                InProgress = true;
                OverlayModel.Title = title;
                OverlayModel.Text = message;
                OverlayModel.Progress = progress;
            });
        }

        /// <summary>
        /// Изменить сообщение оверлея
        /// </summary>
        /// <param name="message">сообщение</param>
        public void SetOverlayMessage(string message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                OverlayModel.Text = message;
            });
        }

        /// <summary>
        /// Изменить прогресс в %
        /// </summary>
        /// <param name="percent">прогресс в %</param>

        public void SetOverlayProgress(double percent)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                OverlayModel.Percent = percent;
            });
        }

        /// <summary>
        /// Изменить прогресс (будет показано примерное время завершения)
        /// </summary>
        /// <param name="total">всего тиков</param>
        /// <param name="progress">выполнено</param>
        public void SetOverlayProgress(double total, double progress)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                OverlayModel.Total = total;
                OverlayModel.Progress = progress;
            });
        }

        /// <summary>
        /// Спрятать оверлей
        /// </summary>
        public void HideOverlay()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                InProgress = false;
            });
        }
        #endregion

        #region Actions
        public void ShowLogin()
        {
            var document = new LoginViewContentModel { DisplayName = "Вход пользователя" };
            AddWorkspace(document);
        }

        public void ShowEconomicAccount(int? id = null)
        {
            var document = new EconomicAccountViewContentModel(id) { DisplayName = "Платежные поручения" };
            AddWorkspace(document);
        }

        public void ShowEconomicRefuse(int? id = null)
        {
            var document = new EconomicRefuseViewContentModel(id) { DisplayName = "Отказы" };
            AddWorkspace(document);
        }

        public void ShowEconomicSurcharge(int? id = null)
        {
            var document = new EconomicSurchargeViewContentModel(id) { DisplayName = "Доплаты" };
            AddWorkspace(document);
        }

        public void ShowEconomicJournal()
        {
            var document = new EconomicJournalViewContentModel { DisplayName = "Журнал платежей" };
            AddWorkspace(document);
        }

        public void ShowDashboard()
        {
            var document = new StartPageViewContentModel { DisplayName = "Стартовая страница" };
            AddWorkspace(document);
        }

        public void ShowImportInterTerritorial()
        {
            ShowImportInterTerritorial(null);
        }

        public void ShowTerritoryAccount()
        {
            var t = _overlayManager.ShowOverlay(Constants.OperationBeginTitleMsg, Constants.PleaseWaitMsg);
            t.ContinueWith(p =>
            {
                AddWorkspace(new TerritoryAccountViewContentModel { DisplayName = "Счета с территорий" });
            })
            .ContinueWith(p => _overlayManager.HideOverlay());
        }

        public void ShowTerritoryAccountCollection(List<int> idAccount)
        {
            var t = _overlayManager.ShowOverlay(Constants.OperationBeginTitleMsg, Constants.PleaseWaitMsg);
            t.ContinueWith(p =>
                {
                    AddWorkspace(new TerritoryAccountCollectionViewContentModel(idAccount) { DisplayName = "Реестр счета" });
                })
                .ContinueWith(p => _overlayManager.HideOverlay());
        }

        public void ShowActExpertiseCollection(int idAct, VidControls vidExpertise)
        {
            var t = _overlayManager.ShowOverlay(Constants.OperationBeginTitleMsg, Constants.PleaseWaitMsg);
            t.ContinueWith(p =>
                {
                    AddWorkspace(new EconomicActExpertiseCollectionViewContentModel(idAct) { DisplayName = $"Список {vidExpertise}" });
                })
                .ContinueWith(p => _overlayManager.HideOverlay());
        }

        public void ShowImportLocal()
        {
            var document = new ImportLocalXmlContentModel { DisplayName = "Загрузка счетов МО" };
            AddWorkspace(document);
        }

        public void ShowImportInterTerritorial(string fileName)
        {
            var document = new ImportXmlContentModel(fileName) { DisplayName = "Загрузка счетов с территорий" };
            AddWorkspace(document);
        }

        public void ShowMedicalAccount()
        {

            var t = _overlayManager.ShowOverlay(Constants.OperationBeginTitleMsg, Constants.PleaseWaitMsg);
            t.ContinueWith(p =>
            {
                AddWorkspace(new MedicalAccountViewContentModel { DisplayName = "Счета МО" });
            })
            .ContinueWith(p => _overlayManager.HideOverlay());

            //Task.Factory.StartNew(ShowOverlay)
            //    .ContinueWith(p => AddWorkspace(new MedicalAccountViewContentModel { DisplayName = "Счета МО" }))
            //    .ContinueWith(p => HideOverlay());
        }

        public void ShowExchange()
        {
            ShowExchange(null);
        }

        public void ShowExchange(int? id)
        {
            Task.Factory.StartNew(ShowOverlay)
                .ContinueWith(p => AddWorkspace(new ExchangeViewContentModel(id) { DisplayName = "Журнал информационного обмена" }))
                .ContinueWith(p => HideOverlay());
        }

        public void ShowExchangeTerritory(int? id)
        {
            Task.Factory.StartNew(ShowOverlay)
                .ContinueWith(p => AddWorkspace(new ExchangeViewContentModel(id, AccountSourceType.InterTerritorial) { DisplayName = "Журнал информационного обмена" }))
                .ContinueWith(p => HideOverlay());
        }

        public void ShowExchangeLocal(int? id)
        {
            Task.Factory.StartNew(ShowOverlay)
                .ContinueWith(p => AddWorkspace(new ExchangeViewContentModel(id, AccountSourceType.Local) { DisplayName = "Журнал информационного обмена" }))
                .ContinueWith(p => HideOverlay());
        }

        public void ExportVolume()
        {
            var document = new ExportMedicalAssistanceVolumeContentModel { DisplayName = "Выгрузка объемов МП в ФФОМС" };
            AddWorkspace(document);
        }

        public void ShowSearch()
        {
            var document = new SearchContentModel { DisplayName = "Поиск данных" };
            AddWorkspace(document);
        }

        public void ShowReportDesigner()
        {
            _reportService.RunReportDesigner();
        }

        public void ShowReportView()
        {
            _reportService.RunReportViewer();
        }

        public void ShowXmlEditor(Tuple<string,string> data)
        {
            string value = string.Empty;
            string innerName = string.Empty;
            if (data.IsNotNull())
            {
                value = data.Item1;
                innerName = data.Item2;
            }
           
            var document = new XmlEditorContentModel(value, innerName) { DisplayName = "Редактор XML - {0}".F(innerName) };
            AddWorkspace(document);
        }

        public void ShowUsersManagement()
        {
            var document = new UsersManagementContentModel { DisplayName = "Управление пользователями" };
            AddWorkspace(document);
        }

        public void ShowFileView()
        {
            var document = new FileViewContentModel { DisplayName = "Файлы реестров" };
            AddWorkspace(document);
        }

        public void ShowProcessing()
        {
            var document = new ProcessingViewContentModel { DisplayName = "Обработка данных" };
            AddWorkspace(document);
        }

        public void ShowClassifier()
        {
            var document = new ClassifierViewContentModel { DisplayName = "Справочники" };
            AddWorkspace(document);
        }

        public void ShowActsMo()
        {
            var document = new ActsViewContentModel(Constants.ScopeLocalAccount) { DisplayName = "Акты экспертиз МО" };
            AddWorkspace(document);
        }

        //Отобажения актов экспертиз для версии МО
        public void ShowActsExpertiseMo(int? id = null)
        {
            var document = new ActsExpertiseViewContentModel(id) { DisplayName = "Акты экспертиз МО" };
            AddWorkspace(document);
        }

        public void ShowActsTerritory()
        {
            var document = new ActsViewContentModel(Constants.ScopeInterTerritorialAccount) { DisplayName = "Акты экспертиз для территорий" };
            AddWorkspace(document);
        }

        public void ShowActs(int scope, int? id)
        {
            var document = new ActsViewContentModel(scope,  id) { DisplayName = "Акты экспертиз" };
            AddWorkspace(document);
        }
        
        public void ShowTerritoryAccountZ()
        {
            throw new NotImplementedException();
        }

        public void ShowMedicalAccountZ()
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public abstract class WorkspaceViewModel : ViewModelBase
    {
        private bool _isActive;
        private bool _isOpened;
        private bool _isClosed = true;
        private bool _isSelectedItem = true;
        private bool _askClose;
        private string _displayName;
        protected WorkspaceViewModel()
        {
            this.ApplyDefaultValues();
        }
        public bool IsClosed
        {
            get { return _isClosed; }
            set
            {
                _isClosed = value; RaisePropertyChanged(()=>IsClosed);
            }
        }
        public bool IsOpened
        {
            get { return _isOpened; }
            set
            {
                _isOpened = value; RaisePropertyChanged(() => IsOpened); 
            }
        }

        public bool AskClose
        {
            get { return _askClose; }
            set
            {
                _askClose = value; RaisePropertyChanged(() => AskClose);
            }
        }

        [DefaultValue(true)]
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; RaisePropertyChanged(() => IsActive); }
        }

        public bool IsSelectedItem
        {
            get { return _isSelectedItem; }
            set { _isSelectedItem = value; RaisePropertyChanged(() => IsSelectedItem); }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; RaisePropertyChanged(() => DisplayName); }
        }

        public string InnerName
        {
            get { return _innerName; }
            set { _innerName = value; RaisePropertyChanged(() => InnerName); }
        }

        public virtual ImageSource Glyph { get; set; }
        public object Content { get; set; }
        public object DataContext { get; set; }
        
        public bool IsSingle { get; set; }

        RelayCommand<object> _closeCommand;
        private string _innerName;

        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand<object>(OnRequestClose)); }
        }
        public event EventHandler RequestClose;
        void OnRequestClose(object param)
        {
            EventHandler handler = RequestClose;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        protected virtual void OnIsClosedChanged(bool newValue)
        {
            IsOpened = !newValue;
        }

        public virtual void Dispose()
        {
            DataContext = null;
            Content = null;
#if DEBUG
            string msg = string.Format("{0} ({1}) ({2}) Dispose", GetType().Name, DisplayName, GetHashCode());
            Debug.WriteLine(msg);
#endif
        }
    }

    public abstract class PanelWorkspaceViewModel : WorkspaceViewModel, IMVVMDockingProperties
    {
        protected abstract string WorkspaceName { get; }

        protected PanelWorkspaceViewModel()
        {
            Init();   
        }

        protected void Init()
        {
            ((IMVVMDockingProperties)this).TargetName = WorkspaceName;
        }
        string IMVVMDockingProperties.TargetName { get; set; }

        

    }

    public class DocumentViewModel : PanelWorkspaceViewModel
    {
        protected override string WorkspaceName { get { return "DocumentHost"; } }
        
        public DocumentViewModel(string displayName)
        {
            DisplayName = displayName;
        }
        public DocumentViewModel()
        {
        }
        
        public string Footer { get; protected set; }
        public string Description { get; protected set; }
    }

    public sealed class LoginViewContentModel : DocumentViewModel
    {
        public LoginViewContentModel()
        {
            Glyph = new BitmapImage(new Uri("/MedicineNext;component/Resources/Icons/16/key.png", UriKind.Relative));
            IsSingle = true;
            var control = new LoginView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<LoginViewModel>();
                control.DataContext = model;
                DataContext = model;
            }
            Content = control;
        }
    }

    public class ExchangeViewContentModel : DocumentViewModel
    {
        public ExchangeViewContentModel(int? id = null, AccountSourceType? type = null)
        {
            IsSingle = true;
            using (var scope = Di.I.BeginLifetimeScope())
            {
                ExchangeViewModel model;
                if (id.HasValue)
                {
                    Expression<Func<FactExchange, bool>> predicate = PredicateBuilder.True<FactExchange>();
                    predicate = predicate.And(exchange => exchange.AccountId == id);
                    if (type.HasValue)
                    {
                        switch (type)
                        {
                            case AccountSourceType.InterTerritorial:
                                predicate = predicate.And(exchange => Constants.InterTerritorialAccountTypes.Contains(exchange.Type));
                                break;
                            case AccountSourceType.Local:
                                predicate = predicate.And(exchange => Constants.LocalAccountTypes.Contains(exchange.Type));
                                break;
                        }
                    }
                    model = scope.Resolve<ExchangeViewModel>(new ResolvedParameter(
                      (pi, ctx) => pi.ParameterType == typeof(PLinqExchangeList) && pi.Name == "list",
                      (pi, ctx) => ctx.Resolve<PLinqExchangeList>(new NamedParameter("predicate", predicate))));
                }
                else
                {
                    model = scope.Resolve<ExchangeViewModel>();
                }
                
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Content = new ExchangeView { DataContext = model };
                    DataContext = model;
                });
            }
        }
    }

    public class TerritoryAccountViewContentModel : DocumentViewModel
    {
        private readonly TerritoryAccountViewModel _model;
       
        public TerritoryAccountViewContentModel()
        {
            IsSingle = true;

            _model = Di.I.Resolve<TerritoryAccountViewModel>();
            
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Content = new TerritoryAccountView {DataContext = _model};
                DataContext = _model;
            });

        }
    }

    public class TerritoryAccountCollectionViewContentModel : DocumentViewModel
    {
        private readonly TerritoryAccountCollectionViewModel _model;

        public TerritoryAccountCollectionViewContentModel(List<int> idAccount)
        {
            IsSingle = true;

            _model = Di.I.Resolve<TerritoryAccountCollectionViewModel>(new NamedParameter("idAccounts", idAccount));

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Content = new TerritoryAccountCollectionView { DataContext = _model };
                DataContext = _model;
            });

        }
    }

    public class EconomicActExpertiseCollectionViewContentModel : DocumentViewModel
    {
        private readonly EconomicSankCollectionViewModel _model;

        public EconomicActExpertiseCollectionViewContentModel(int idAct)
        {
            IsSingle = true;

            _model = Di.I.Resolve<EconomicSankCollectionViewModel>(new NamedParameter("idAct", idAct));

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Content = new EconomicSankCollectionView { DataContext = _model };
                DataContext = _model;
            });

        }
    }

    public class MedicalAccountViewContentModel : DocumentViewModel
    {
        public MedicalAccountViewContentModel()
        {
            IsSingle = true;
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<MedicalAccountViewModel>();

                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    Content = new MedicalAccountView { DataContext = model };
                    DataContext = model;
                });
            }
        }
    }

    public class OperatorViewContentModel : DocumentViewModel
    {
        public OperatorViewContentModel(int id, OperatorMode mode, string innerName, IEnumerable<OperatorAction> operatorActions = null)
        {
            var control = new OperatorView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<OperatorViewModel>(new NamedParameter("id", id), 
                    new NamedParameter("mode", mode), 
                    new NamedParameter("operatorActions", operatorActions));
                model.InnerName = innerName;
                control.DataContext = model;
                DataContext = model;
            }

            Content = control;
        }
    }

    public class ZslOperatorViewContentModel : DocumentViewModel
    {
        public ZslOperatorViewContentModel(int id, OperatorMode mode, string innerName, IEnumerable<OperatorAction> operatorActions = null)
        {
            var control = new ZslOperatorView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<ZslOperatorViewModel>(new NamedParameter("id", id),
                    new NamedParameter("mode", mode),
                    new NamedParameter("operatorActions", operatorActions));
                model.InnerName = innerName;
                control.DataContext = model;
                    DataContext = model;
            }

            Content = control;
        }
    }

    public class ReportViewContentModel : DocumentViewModel
    {
        public ReportViewContentModel()
        {
            IsSingle = true;
            var control = new ReportView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<ReportViewModel>();
                control.DataContext = model;
                DataContext = model;
            }

            Content = control;
        }
    }

    public class PreparedReportViewContentModel : DocumentViewModel
    {
        public PreparedReportViewContentModel()
        {
            IsSingle = true;
            var control = new PreparedReportView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<PreparedReportViewModel>();
                control.DataContext = model;
                DataContext = model;
            }

            Content = control;
        }
    }

    public class CriterionViewContentModel : DocumentViewModel
    {
        public CriterionViewContentModel()
        {
            IsSingle = true;
            var control = new CriterionView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<CriterionViewModel>();
                control.DataContext = model;
                DataContext = model;
            }

            Content = control;
        }
    }

    public class EconomicReportViewContentModel : DocumentViewModel
    {
        public EconomicReportViewContentModel()
        {
            IsSingle = true;
            var control = new EconomicReportView();
            Content = control;
        }
    }

    public class EconomicAccountViewContentModel : DocumentViewModel
    {
        public EconomicAccountViewContentModel(int? id)
        {
            var control = new EconomicAccountView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                EconomicAccountViewModel model;
                if (id.HasValue)
                {
                    Expression<Func<FactEconomicAccount, bool>> predicate = account => account.AccountId == id;
                    model = scope.Resolve<EconomicAccountViewModel>(new ResolvedParameter(
                      (pi, ctx) => pi.ParameterType == typeof(PLinqEconomicAccountList) && pi.Name == "list",
                      (pi, ctx) => ctx.Resolve<PLinqEconomicAccountList>(new NamedParameter("predicate", predicate))));
                }
                else
                {
                    model = scope.Resolve<EconomicAccountViewModel>();
                }
                
                control.DataContext = model;
                DataContext = model;
            }

            Content = control;
        }
    }

    public class EconomicRefuseViewContentModel : DocumentViewModel
    {
        public EconomicRefuseViewContentModel(int? id)
        {
            var control = new EconomicRefuseView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                EconomicRefuseViewModel model;
                if (id.HasValue)
                {
                    Expression<Func<FactEconomicRefuse, bool>> predicate = account => account.AccountId == id;
                    model = scope.Resolve<EconomicRefuseViewModel>(new ResolvedParameter(
                      (pi, ctx) => pi.ParameterType == typeof(PLinqEconomicRefuseList) && pi.Name == "list",
                      (pi, ctx) => ctx.Resolve<PLinqEconomicRefuseList>(new NamedParameter("predicate", predicate))));
                }
                else
                {
                    model = scope.Resolve<EconomicRefuseViewModel>();
                }

                control.DataContext = model;
                DataContext = model;
            }

            Content = control;
        }
    }

    public class EconomicSurchargeViewContentModel : DocumentViewModel
    {
        public EconomicSurchargeViewContentModel(int? id)
        {
            var control = new EconomicSurchargeView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                EconomicSurchargeViewModel model;
                if (id.HasValue)
                {
                    Expression<Func<FactEconomicSurcharge, bool>> predicate = account => account.AccountId == id;
                    model = scope.Resolve<EconomicSurchargeViewModel>(new ResolvedParameter(
                      (pi, ctx) => pi.ParameterType == typeof(PLinqEconomicSurchargeList) && pi.Name == "list",
                      (pi, ctx) => ctx.Resolve<PLinqEconomicSurchargeList>(new NamedParameter("predicate", predicate))));
                }
                else
                {
                    model = scope.Resolve<EconomicSurchargeViewModel>();
                }

                control.DataContext = model;
                DataContext = model;
            }

            Content = control;
        }
    }

    public class EconomicJournalViewContentModel : DocumentViewModel
    {
        public EconomicJournalViewContentModel()
        {
            IsSingle = true;
            var control = new EconomicJournalView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<EconomicJournalViewModel>();
                control.DataContext = model;
                DataContext = model;
            }

            Content = control;
        }
    }

    public class StartPageViewContentModel : DocumentViewModel
    {
        public StartPageViewContentModel()
        {
            IsSingle = true;
            var control = new StartPageView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<StartPageViewModel>();
                control.DataContext = model;
                DataContext = model;
            }

            Content = control;
        }
    }

    public class ExportMedicalAssistanceVolumeContentModel : DocumentViewModel
    {
        public ExportMedicalAssistanceVolumeContentModel()
        {
            IsSingle = true;
            var control = new ExportMedicalAssistanceView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<ExportMedicalAssistanceViewModel>();
                control.DataContext = model;
                DataContext = model;
            }

            Content = control;
        }
    }

    public class XmlEditorContentModel : DocumentViewModel
    {
        public XmlEditorContentModel(string value, string innerName)
        {
            var control = new XmlEditor();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<XmlEditViewModel>(new NamedParameter("value",value));
                model.InnerName = innerName;
                control.DataContext = model;
                DataContext = model;
            }
            Content = control;
        }
    }

    public class ImportXmlContentModel : DocumentViewModel
    {
        public ImportXmlContentModel(string fileName = null)
        {
            IsSingle = true;
            var control = new ImportXmlView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<ImportXmlViewModel>(new NamedParameter("fileName", fileName));
                control.DataContext = model;
                DataContext = model;
            }
            Content = control;
        }
    }

    public class ImportLocalXmlContentModel : DocumentViewModel
    {
        public ImportLocalXmlContentModel()
        {
            IsSingle = true;
            var control = new ImportLocalXmlView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<ImportLocalXmlViewModel>();
                control.DataContext = model;
                DataContext = model;
            }
            Content = control;
        }
    }

    public class SearchContentModel : DocumentViewModel
    {
        public SearchContentModel()
        {
            IsSingle = true;
            var control = new SearchView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<SearchViewModel>();
                control.DataContext = model;
                DataContext = model;
            }
            Content = control;
        }
    }

    public class UsersManagementContentModel : DocumentViewModel
    {
        public UsersManagementContentModel()
        {
            IsSingle = true;
            var control = new UsersManagementView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<UsersManagementViewModel>();
                control.DataContext = model;
                DataContext = model;
            }
            Content = control;
        }
    }

    public class FileViewContentModel : DocumentViewModel
    {
        public FileViewContentModel()
        {
            IsSingle = true;
            var control = new FilesView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<FilesViewModel>();
                control.DataContext = model;
                DataContext = model;
            }
            Content = control;
        }
    }

    public class ProcessingViewContentModel : DocumentViewModel
    {
        public ProcessingViewContentModel()
        {
            IsSingle = true;
            var control = new ProcessingView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<ProcessingViewModel>();
                control.DataContext = model;
                DataContext = model;
            }
            Content = control;
        }
    }

    public class ClassifierViewContentModel : DocumentViewModel
    {
        public ClassifierViewContentModel()
        {
            IsSingle = true;
            var control = new ClassifierView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<ClassifierViewModel>();
                control.DataContext = model;
                DataContext = model;
            }
            Content = control;
        }
    }

    public class ActsExpertiseViewContentModel : DocumentViewModel
    {
        public ActsExpertiseViewContentModel(int? id)
        {
            var control = new EconomicActExpertiseView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                EconomicActExpertiseViewModel model;
                if (id.HasValue)
                {
                    Expression<Func<FactActExpertise, bool>> predicate = act => act.ActExpertiseId == id;
                    model = scope.Resolve<EconomicActExpertiseViewModel>(new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(PLinqEconomicActExpertiseList) && pi.Name == "list",
                        (pi, ctx) => ctx.Resolve<PLinqEconomicActExpertiseList>(new NamedParameter("predicate", predicate))));
                }
                else
                {
                    model = scope.Resolve<EconomicActExpertiseViewModel>();
                }

                control.DataContext = model;
                DataContext = model;
            }

            Content = control;
        }
    }

    public class ActsViewContentModel : DocumentViewModel
    {
        public ActsViewContentModel(int? actScope, int? id = null)
        {
            var control = new ActsView();
            using (var scope = Di.I.BeginLifetimeScope())
            {
                var model = scope.Resolve<ActsViewModel>(new NamedParameter("accountId", id), new NamedParameter("scope", actScope));
                control.DataContext = model;
                DataContext = model;
            }
            Content = control;
        }
    }
    
}
