using System.Threading;
using System.Windows;
using Autofac;
using Core;
using Core.Services;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Editors;
using Medical.AppLayer.Managers;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;
using MedicineNext.Internal;
using MedicineNext.Internal.MVVM;

namespace MedicineNext {
    /// <summary>
    /// 
    /// </summary>
    public partial class MainWindow
    {
        public static ResourceDictionary SharedResources { get; set; }

        private IDockLayoutManager _dockLayoutManager;
        private IAppRemoteSettings _remoteSettings;
        private IMessageService _messageService;
        private IOverlayManager _overlayManager;
        private INotifyManager _notifyManager;

        public MainWindow()
        {
            InitializeComponent();
            SharedResources = Resources;
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            DataContext = _dockLayoutManager = Di.I.Resolve<IDockLayoutManager>();
            _overlayManager = Di.I.Resolve<IOverlayManager>();
            //_remoteSettings = Di.I.Resolve<IAppRemoteSettings>();
            //Заполнение ViewModel данными
            FillViewModel(DataContext as DockLayoutManager);
            _messageService = Di.I.Resolve<IMessageService>();
            _notifyManager = Di.I.Resolve<INotifyManager>();
            
        }

        private void FillViewModel(DockLayoutManager viewModel)
        {
            //dynamic clientMo = _remoteSettings.Get(AppRemoteSettings.ClientMo);
            //bool mcod = clientMo.MO;

            var homePage = new PageModel
            {
                Id = "Main",
                Name = "Главная"
            };
            var registerHomeGroup = new PageGroupModel {Name = "Реестры"};
            var homeCategory = new CategoryModel();

            homeCategory.Pages.Add(homePage);
            homePage.Groups.Add(registerHomeGroup);

            var classifiers = new PageModel {Name = "Классификаторы"};
            homeCategory.Pages.Add(classifiers);

            var data = new PageModel {Name = "Данные"};
            homeCategory.Pages.Add(data);

            var register = new PageModel {Name = "Реестры"};
            homeCategory.Pages.Add(register);

            var economic = new PageModel {Name = "Экономисты"};
            homeCategory.Pages.Add(economic);

            var report = new PageModel {Name = "Отчеты"};
            homeCategory.Pages.Add(report);

            var stats = new PageModel {Name = "Статистика"};
            homeCategory.Pages.Add(stats);

            var admin = new PageModel {Name = "Администрирование"};
            homeCategory.Pages.Add(admin);

            var dev = new PageModel { Name = "Разработка" };
            homeCategory.Pages.Add(dev);

            var showDashboardCommand = new CommandModel
            {
                Id = "StartPage",
                Command = new RelayCommand(_dockLayoutManager.ShowDashboard),
                Caption = "Стартовая страница",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/favorites.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/favorites.png")
            };
            registerHomeGroup.Commands.Add(showDashboardCommand);

            var viewGroup = new PageGroupModel {Name = "Просмотр данных"};
            var importGroup = new PageGroupModel {Name = "Импорт данных"};

            var importFromMo = new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowImportLocal),
                Caption = "Импорт реестров МО",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/go-bottom-out.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/go-bottom-out.png")
            };
            registerHomeGroup.Commands.Add(importFromMo);
            importGroup.Commands.Add(importFromMo);

            var importFromTerritory = new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowImportInterTerritorial),
                Caption = "Импорт реестров с территорий",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/go-bottom-in.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/go-bottom-in.png")
            };
            registerHomeGroup.Commands.Add(importFromTerritory);
            importGroup.Commands.Add(importFromTerritory);

            //var importFromTerritory2018 = new CommandModel
            //{
            //    Command = new RelayCommand(_dockLayoutManager.ShowImportInterTerritorial),
            //    Caption = "Импорт реестров с территорий 2018",
            //    LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/go-bottom-in.png"),
            //    SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/go-bottom-in.png")
            //};
            //registerHomeGroup.Commands.Add(importFromTerritory2018);
            //importGroup.Commands.Add(importFromTerritory2018);

            var showTerritoryAccountCommand = new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowTerritoryAccount),
                Caption = "Счета с территорий",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/text-x-changelog.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/text-x-changelog.png")
            };
            registerHomeGroup.Commands.Add(showTerritoryAccountCommand);
            viewGroup.Commands.Add(showTerritoryAccountCommand);

            var showMoAccountCommand = new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowMedicalAccount),
                Caption = "Счета МО",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/text-x-changelog.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/text-x-changelog.png")
            };
            registerHomeGroup.Commands.Add(showMoAccountCommand);
            viewGroup.Commands.Add(showMoAccountCommand);

            var journalGroup = new PageGroupModel {Name = "Журналы"};

            register.Groups.Add(viewGroup);
            register.Groups.Add(importGroup);
            register.Groups.Add(journalGroup);

            journalGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowExchange),
                Caption = "Журнал информационного обмена",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/text-x-log.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/text-x-log.png")
            });

            var fileGroup = new PageGroupModel { Name = "Файлы реестров" };

            register.Groups.Add(fileGroup);

            fileGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowFileView),
                Caption = "Файлы реестров",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/drive-harddisk.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/drive-harddisk.png")
            });

            var statGroup = new PageGroupModel {Name = "Федеральный фонд"};
            stats.Groups.Add(statGroup);
            statGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ExportVolume),
                Caption = "Выгрузка объемов МП",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/document-save-2.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/document-save-2.png")
            });


            var economicGroup = new PageGroupModel {Name = "Журналы"};
            economic.Groups.Add(economicGroup);
            var economicReportGroup = new PageGroupModel {Name = "Отчеты"};
            economic.Groups.Add(economicReportGroup);
            
            economicGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand<int?>(_dockLayoutManager.ShowEconomicAccount),
                Caption = "Информация об оплате",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/bookmark.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/bookmark.png")
            });

            economicGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand<int?>(_dockLayoutManager.ShowEconomicSurcharge),
                Caption = "Информация о доплатах",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/bookmark-new.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/bookmark-new.png")
            });

            economicGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand<int?>(_dockLayoutManager.ShowEconomicRefuse),
                Caption = "Информация об отказах",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/bookmark-remove.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/bookmark-remove.png")
            });

            //if (mcod)
            //{
            economicGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand<int?>(_dockLayoutManager.ShowActsExpertiseMo),
                Caption = "Акты экспертиз",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/bookmark.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/bookmark.png")
            });
            //}


            economicGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowEconomicJournal),
                Caption = "Журнал учёта счетов",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/bookmarks-organize.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/bookmarks-organize.png")
            });

            economicReportGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowEconomicReport),
                Caption = "Список отчетов",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/report.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/report.png")
            });

            var searchGroup = new PageGroupModel {Name = "Поиск"};
            homePage.Groups.Add(searchGroup);
            searchGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowSearch),
                Caption = "Поиск данных",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/user-search.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/user-search.png")
            });

            var examinationGroup = new PageGroupModel {Name = "Экспертизы"};
            data.Groups.Add(examinationGroup);
            examinationGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowCriterion),
                Caption = "Критерии экспертизы МП",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/run-build-clean.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/run-build-clean.png")
            });

            var processingGroup = new PageGroupModel { Name = "Обработка данных" };
            data.Groups.Add(processingGroup);
            processingGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowProcessing),
                Caption = "Функции обработки данных",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/processing.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/processing.png")
            });

            var reportGroup = new PageGroupModel {Name = "Отчеты"};
            report.Groups.Add(reportGroup);
            reportGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowReport),
                Caption = "Список отчетов",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/text-x-script.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/text-x-script.png")
            });

            reportGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowPreparedReport),
                Caption = "Список готовых отчетов",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/x-office-document.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/x-office-document.png")
            });

            reportGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowReportDesigner),
                Caption = "Дизайнер отчетов",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/application-x-designer.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/application-x-designer.png")
            });

            reportGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowReportView),
                Caption = "Средство просмотра отчетов",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/application-x-viewer.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/application-x-viewer.png")
            });

            var classifierGroup = new PageGroupModel { Name = "Справочники" };
            classifiers.Groups.Add(classifierGroup);
            classifierGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowClassifier),
                Caption = "Список справочников",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/applications-office.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/applications-office.png")
            });

            var adminGroup = new PageGroupModel { Name = "Пользователи" };
            admin.Groups.Add(adminGroup);
            adminGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand(_dockLayoutManager.ShowUsersManagement),
                Caption = "Управление пользователями",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/system-users.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/system-users.png")
            });

            var devGroup = new PageGroupModel { Name = "Разработка" };
            dev.Groups.Add(devGroup);
            //devGroup.Commands.Add(new CommandModel
            //{
            //    Command = new RelayCommand(DeleteAllData),
            //    Caption = "Удалить все данные из БД",
            //    LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/text-x-script.png"),
            //    SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/text-x-script.png")
            //});

            devGroup.Commands.Add(new CommandModel
            {
                Command = new RelayCommand(ShowMyOverlay),
                Caption = "Оверлей",
                LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/text-x-script.png"),
                SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/text-x-script.png")
            });

            //devGroup.Commands.Add(new CommandModel
            //{
            //    Command = new RelayCommand(ShowMyNotify),
            //    Caption = "Нотификация",
            //    LargeGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/text-x-script.png"),
            //    SmallGlyph = Utils.GlyphHelper.GetGlyph("../Resources/Icons/text-x-script.png")
            //});

            viewModel.Categories.Add(homeCategory);
            _dockLayoutManager.ShowLogin();
        }

        private void DXRibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
            Activate();
        }

        #region Dev methods
        private void ShowMyOverlay()
        {
            var view = new CommonWindowControl(this, 800, 600, null, new DataControl());
            view.OkCallback = () =>
            {
                var task = _overlayManager.ShowOverlay(Constants.AccountBreakUpTitleMsg, Constants.PleaseWaitMsg);
                task.ContinueWith(t => Thread.Sleep(5000))
                    .ContinueWith(t => _overlayManager.SetOverlayTitle(Constants.SearchDataTitleMsg))
                    .ContinueWith(t => Thread.Sleep(5000))
                    .ContinueWith(t =>
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            _overlayManager.SetOverlayProgress(1000, i);
                            Thread.Sleep(10);
                        }
                    })
                    .ContinueWith(t => _overlayManager.HideOverlay());
            };
            view.ShowDialog();
        }

        private void ShowMyNotify()
        {
            _notifyManager.ShowNotify("ПРоверка", "Строка 2", "Строка 3");
        }

        private void DeleteAllData()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                if (_messageService.AskQuestion("Полное удаление данных. Использовать только на тестовых БД!!!","Внимание!")) {
                    var repository = scope.Resolve<IMedicineRepository>();
                    var result = repository.TrancateAllData();
                    if (result.Success)
                    {
                        _messageService.ShowMessage("База данных успешно очищена");
                    }
                }
            }
        }

       
        #endregion
    }
}
