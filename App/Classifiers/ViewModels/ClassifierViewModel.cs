using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using Autofac;
using BLToolkit.Mapping;
using Core;
using Core.Extensions;
using Core.Infrastructure;
using Core.Services;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Linq;
using Medical.AppLayer.Linq.Classifiers;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.CoreLayer.PropertyGrid;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Classifiers.ViewModels
{
    public class ClassifierViewModel : ViewModelBase, IContextCommandContainer, IHash
    {
        private readonly IMedicineRepository _repository;
        private readonly IMessageService _messageService;
        private readonly INotifyManager _notifyManager;
        private readonly IElmedicineRepository _elmedicine;
        private readonly IDockLayoutManager _dockManager;

        #region IContextCommandContainer

        public ObservableCollection<IContextCommand> ContextCommands { get; set; }
        public string PageName { get; set; }

        #endregion

        #region IHash

        public string Hash
        {
            get { return typeof(ClassifierViewModel).FullName; }
        }

        #endregion

        private object _selectedClassifier;
        private IListSource _dataListSource;
        private object _currentRow;
        private Dictionary<string, Action<ILifetimeScope>> _dataHandlers;
        private Dictionary<string, Action> _importHandlers;

        private bool _isEditDataOpen;
        private object _dataModel;

        public ObservableCollection<ClassifierModel> ClassifiersList { get; set; }
        public IListSource DataListSource {
            get { return _dataListSource; }
            set { _dataListSource = value;RaisePropertyChanged(()=>DataListSource); }
        }

        public object SelectedClassifier
        {
            get { return _selectedClassifier; }
            set { 
                _selectedClassifier = value; 
                RaisePropertyChanged(()=>SelectedClassifier);
                Load();
            }
        }
        
        public ClassifierModel Classifier
        {
            get { return _selectedClassifier as ClassifierModel; }
        }

        public object CurrentRow
        {
            get { return _currentRow; }
            set {
                _currentRow = value;
                RaisePropertyChanged(() => CurrentRow);
                RaisePropertyChanged(() => IsDetailsExist);
            }
        }

        public bool IsDetailsExist
        {
            get { return _currentRow != null && _currentRow.HasProperty("Details"); }
        }

        

        public bool IsEditDataOpen
        {
            get { return _isEditDataOpen; }
            set { _isEditDataOpen = value; RaisePropertyChanged(()=>IsEditDataOpen); }
        }

        public object DataModel
        {
            get { return _dataModel; }
            set
            {
                _dataModel = value;
                RaisePropertyChanged(() => DataModel);
            }
        }
        

        public ClassifierViewModel(
            INotifyManager notifyManager,
            IMedicineRepository repository,
            IElmedicineRepository elmedicine,
            IDockLayoutManager dockManager,
            IMessageService messageService)
        {
            _repository = repository;
            _elmedicine = elmedicine;
            _messageService = messageService;
            _notifyManager = notifyManager;
            _dockManager = dockManager;
            Init();
        }

        private void Init()
        {
            PageName = "Управление справочниками";
            ContextCommands = new ObservableCollection<IContextCommand>
            {
                new ContextCommand
                {
                    Id = "add",
                    Caption = "Добавить запись",
                    Command = new RelayCommand(Add, CanAdd),
                    Page = "действия",
                    LargeGlyph = "../Resources/Icons/plus.png",
                    SmallGlyph = "../Resources/Icons/plus.png"
                },
                new ContextCommand
                {
                    Id = "edit",
                    Caption = "Редактирование",
                    Command = new RelayCommand(Edit, CanEdit),
                    Page = "действия",
                    LargeGlyph = "../Resources/Icons/edit.png",
                    SmallGlyph = "../Resources/Icons/edit.png"
                },
                new ContextCommand
                {
                    Id = "reload",
                    Caption = "Обновить",
                    Command = new RelayCommand(Load),
                    Page = "действия",
                    LargeGlyph = "../Resources/Icons/view-refresh.png",
                    SmallGlyph = "../Resources/Icons/view-refresh.png"
                },
                new ContextCommand
                {
                    Id = "delete",
                    Caption = "Удалить запись",
                    Command = new RelayCommand(Delete, CanDelete),
                    Page = "действия",
                    LargeGlyph = "../Resources/Icons/edit-delete.png",
                    SmallGlyph = "../Resources/Icons/edit-delete.png"
                },
                new ContextCommand
                {
                    Id = "importExportData",
                    Caption = "Импорт/экспорт данных",
                    Page = "Импорт/экспорт",
                    LargeGlyph = "../Resources/Icons/import-export.png",
                    SmallGlyph = "../Resources/Icons/import-export.png",
                    IsParent = true
                },
                new ContextCommand
                {
                    Id = "importXml",
                    Caption = "Импорт из XML",
                    LargeGlyph = "../Resources/Icons/document-import.png",
                    SmallGlyph = "../Resources/Icons/document-import.png",
                    Parent = "importExportData",
                    Command = new RelayCommand(ImportXml, CanImportXml),
                },
                new ContextCommand
                {
                    Id = "exportXml",
                    Caption = "Экспорт в XML",
                    LargeGlyph = "../Resources/Icons/document-export.png",
                    SmallGlyph = "../Resources/Icons/document-export.png",
                    Parent = "importExportData",
                    Command = new RelayCommand(ExportXml, CanExportXml),
                },
                new ContextCommand
                {
                    Id = "importDatabase",
                    Caption = "Импорт из БД",
                    LargeGlyph = "../Resources/Icons/db_in.png",
                    SmallGlyph = "../Resources/Icons/db_in.png",
                    Parent = "importExportData",
                    Command = new RelayCommand(ImportDatabase, CanImportDatabase),
                }
            };

            _dataHandlers = new Dictionary<string, Action<ILifetimeScope>>
            {
                {"F001", scope => { DataListSource = scope.Resolve<PLinqF001List>();}},
                {"F014", scope => { DataListSource = scope.Resolve<PLinqF014List>();}},
                {"F015", scope => { DataListSource = scope.Resolve<PLinqF015List>();}},
                {"V017", scope => { DataListSource = scope.Resolve<PLinqV017List>();}},
                {"localF001", scope => { DataListSource = scope.Resolve<PLinqLocalF001List>();}},
                {"shareDoctor", scope => { DataListSource = scope.Resolve<PLinqDoctorList>();}},
                {"EconomicPartner", scope => { DataListSource = scope.Resolve<PLinqEconomicPartnerList>();}},
                {"globalRefusalPenalty", scope => { DataListSource = scope.Resolve<PLinqRefusalPenaltyList>();}},
                {"globalMedicalEventType", scope => { DataListSource = scope.Resolve<PLinqGlobalMedicalEventTypeList>();}},
                {"globalRegionalAttribute", scope => { DataListSource = scope.Resolve<PLinqGlobalRegionalAttributeList>();}},
                {"globalObsoleteData", scope => { DataListSource = scope.Resolve<PLinqObsoleteDataList>();}},
                {"globalV015EqV002", scope => { DataListSource = scope.Resolve<PLinqV015EqV002List>();}},
                {"globalClinicalExamination", scope => { DataListSource = scope.Resolve<PLinqClinicalExaminationList>();}},
                {"globalLicense", scope => { DataListSource = scope.Resolve<PLinqLicenseList>();}},
                {"globalIDCToEventType", scope => { DataListSource = scope.Resolve<PLinqIDCToEventTypeList>();}},
                {"localF003", scope => { DataListSource = scope.Resolve<PLinqLocalF003List>();}},
                {"F004", scope => { DataListSource = scope.Resolve<PLinqF004List>();}},
            };

            _importHandlers = new Dictionary<string, Action>
            {
                {
                    "globalRefusalPenalty", () => {
                        if (Classifier.ImportTable.IsNotNullOrWhiteSpace())
                        {
                            var result = _elmedicine.GetDataSetByTableName("SANK_AUTO");
                            if (result.Success)
                            {
                                var data = new List<globalRefusalPenalty>();
                                _repository.TrancateTable("globalRefusalPenalty");
                                var table = result.Data.Tables[0];
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    DataRow row = table.Rows[i];
                                    var name = row["S_NAME"] as string;
                                    if (name == null)
                                    {
                                        continue;
                                    }

                                    var splited = name.Split(' ');
                                    if (splited[0].Replace(".","").ToInt32Nullable().HasValue)
                                    {
                                        var model = new globalRefusalPenalty
                                        {
                                            Reason = splited[0],
                                            Percent = row["S_PROCENT"].ToInt32(),
                                            Penalty = row["S_STRAF"].ToDecimal(),
                                            Decrease = row["S_UMENS"].ToDecimal(),
                                            Comments = row["S_ZAKL"] as string
                                        };
                                        data.Add(model);
                                    }

                                    _dockManager.SetOverlayProgress(table.Rows.Count, i);
                                }

                                var insertResult = _repository.InsertBatch(data);
                                if (insertResult.Success)
                                {
                                    Load();
                                    _notifyManager.ShowNotify("Данные успешно импортированы.");
                                }
                            }
                        }
                    }
                },
                {
                    "globalMedicalEventType", () => {
                        if (Classifier.ImportTable.IsNotNullOrWhiteSpace())
                        {
                            var result = _elmedicine.GetDataSetByTableName("TypeSluchDb");
                            if (result.Success)
                            {
                                var data = new List<globalMedicalEventType>();
                                _repository.TrancateTable("globalMedicalEventType");

                                var table = result.Data.Tables[0];
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    DataRow row = table.Rows[i];
                                    
                                    var model = new globalMedicalEventType
                                    {
                                        ID = row["Id"].ToInt32(),
                                        Name = row["Name"] as string
                                    };
                                    data.Add(model);

                                    _dockManager.SetOverlayProgress(table.Rows.Count, i);
                                }

                                var insertResult = _repository.InsertBatch(data);
                                if (insertResult.Success)
                                {
                                    Load();
                                    _notifyManager.ShowNotify("Данные успешно импортированы.");
                                }
                            }
                        }
                    }
                },
                {
                    "globalRegionalAttribute", () => {
                        if (Classifier.ImportTable.IsNotNullOrWhiteSpace())
                        {
                            var result = _elmedicine.GetDataSetByTableName("OsobSluchDb");
                            if (result.Success)
                            {
                                var data = new List<globalRegionalAttribute>();
                                _repository.TrancateTable("globalRegionalAttribute");

                                var table = result.Data.Tables[0];
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    DataRow row = table.Rows[i];

                                    var model = new globalRegionalAttribute
                                    {
                                        ID = row["Id"].ToInt32(),
                                        Name = row["Name"] as string
                                    };
                                    data.Add(model);

                                    _dockManager.SetOverlayProgress(table.Rows.Count, i);
                                }

                                var insertResult = _repository.InsertBatch(data);
                                if (insertResult.Success)
                                {
                                    Load();
                                    _notifyManager.ShowNotify("Данные успешно импортированы.");
                                }
                            }
                        }
                    }
                },
                {
                    "globalV015EqV002", () => {
                        if (Classifier.ImportTable.IsNotNullOrWhiteSpace())
                        {
                            var result = _elmedicine.GetDataSetByTableName("V015_V002");
                            if (result.Success)
                            {
                                var data = new List<globalV015EqV002>();
                                _repository.TrancateTable("globalV015EqV002");

                                var table = result.Data.Tables[0];
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    DataRow row = table.Rows[i];

                                    var model = new globalV015EqV002
                                    {
                                        V015EqV002Id = row["ID"].ToInt32(),
                                        Profile_Id = row["CodProf"].ToInt32(),
                                        Speciality_Id = row["CodSpec"].ToInt32()
                                    };
                                    data.Add(model);

                                    _dockManager.SetOverlayProgress(table.Rows.Count, i);
                                }

                                var insertResult = _repository.InsertBatch(data);
                                if (insertResult.Success)
                                {
                                    Load();
                                    _notifyManager.ShowNotify("Данные успешно импортированы.");
                                }
                            }
                        }
                    }
                },
                {
                    "globalClinicalExamination", () => {
                        if (Classifier.ImportTable.IsNotNullOrWhiteSpace())
                        {
                            var result = _elmedicine.GetDataSetByTableName("Shablon_Filter");
                            if (result.Success)
                            {
                                var data = new List<globalClinicalExamination>();
                                _repository.TrancateTable("globalClinicalExamination");

                                var table = result.Data.Tables[0];
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    DataRow row = table.Rows[i];

                                    var model = new globalClinicalExamination
                                    {
                                        ClinicalExaminationId = row["id"].ToInt32(),
                                        ServiceCode = row["CodUslugi"] as string,
                                        ProfileId = row["Profil"].ToInt32(),
                                        SpecialityId = row["SpecialityDoctor"].ToInt32(),
                                        AgeMonths = row["Vozrm"].ToInt32(),
                                        Age = row["Vozr"].ToInt32(),
                                        Sex = row["W"].ToInt32(),
                                        Type = row["ShablonType"].ToInt32(),
                                        IsChildren = row["DetProfil"].ToBool(),
                                        Quantity = row["UslCount"].ToInt32()
                                    };
                                    data.Add(model);

                                    _dockManager.SetOverlayProgress(table.Rows.Count, i);
                                }

                                var insertResult = _repository.InsertBatch(data);
                                if (insertResult.Success)
                                {
                                    Load();
                                    _notifyManager.ShowNotify("Данные успешно импортированы.");
                                }
                            }
                        }
                    }
                },
                {
                    "globalLicense", () => {
                        if (Classifier.ImportTable.IsNotNullOrWhiteSpace())
                        {
                            var result = _elmedicine.GetDataSetByTableName("LIC_NUM_TBL");
                            if (result.Success)
                            {
                                var data = new List<globalLicense>();
                                _repository.DeleteTable("globalLicenseEntry");
                                _repository.DeleteTable("globalLicense");

                                var table = result.Data.Tables[0];
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    DataRow row = table.Rows[i];

                                    var model = new globalLicense
                                    {
                                        LicenseId = row["ID"].ToInt32(),
                                        LicenseNumber = row["LICN"] as string,
                                        MedicalOrganization = row["LPU"] as string,
                                        DateBegin = row["DATE_1"] as DateTime?,
                                        DateEnd = row["DATE_2"] as DateTime?,
                                        DateStop = row["DATE_3"] as DateTime?,
                                    };
                                    data.Add(model);

                                    _dockManager.SetOverlayProgress(table.Rows.Count, i);
                                }

                                var insertResult = _repository.InsertBatch(data);
                                if (insertResult.Success)
                                {
                                    var resultEntry = _elmedicine.GetDataSetByTableName("LIC_TBL");
                                    if (resultEntry.Success)
                                    {
                                        var dataEntry = new List<globalLicenseEntry>();
                                        var tableEntry = resultEntry.Data.Tables[0];
                                        for (int i = 0; i < tableEntry.Rows.Count; i++)
                                        {
                                            DataRow row = tableEntry.Rows[i];

                                            var model = new globalLicenseEntry
                                            {
                                                LicenseEntryId = row["ID"].ToInt32(),
                                                License_LicenseId = row["LIC_NUM_ID"].ToInt32(),
                                                AssistanceCondition_id = row["USL_MP"].ToInt32(),
                                                AssistanceType_Id = row["VID_MP"].ToInt32(),
                                                Profile_Id = row["PROFIL"].ToInt32()
                                            };
                                            dataEntry.Add(model);

                                            _dockManager.SetOverlayProgress(table.Rows.Count, i);
                                        }
                                        var insertEntryResult = _repository.InsertBatch(dataEntry);
                                        if (insertResult.Success)
                                        {
                                            Load();
                                             _notifyManager.ShowNotify("Данные успешно импортированы.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                {
                    "globalIDCToEventType", () => {
                        if (Classifier.ImportTable.IsNotNullOrWhiteSpace())
                        {
                            var result = _elmedicine.GetDataSetByTableName("AmbulanceMKB");
                            if (result.Success)
                            {
                                var data = new List<globalIDCToEventType>();
                                _repository.TrancateTable("globalIDCToEventType");

                                var table = result.Data.Tables[0];
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    DataRow row = table.Rows[i];

                                    var model = new globalIDCToEventType
                                    {
                                        Id = row["id"].ToInt32(),
                                        IDC = row["DS"] as string,
                                        EventType_ID = row["SluchType"].ToInt32()
                                    };
                                    data.Add(model);

                                    _dockManager.SetOverlayProgress(table.Rows.Count, i);
                                }

                                var insertResult = _repository.InsertBatch(data);
                                if (insertResult.Success)
                                {
                                    Load();
                                    _notifyManager.ShowNotify("Данные успешно импортированы.");
                                }
                            }
                        }
                    }
                },
                {
                    "localF003", () => {
                        if (Classifier.ImportTable.IsNotNullOrWhiteSpace())
                        {
                            var result = _elmedicine.GetDataSetByTableName("F003Local");
                            if (result.Success)
                            {
                                var data = new List<localF003>();
                                _repository.TrancateTable("localF003");

                                var table = result.Data.Tables[0];
                                for (int i = 0; i < table.Rows.Count; i++)
                                {
                                    DataRow row = table.Rows[i];

                                    var model = new localF003
                                    {
                                        Code = row["mcod"] as string,
                                        ShortName = row["nam_mok"] as string,
                                        Surname = row["fam_ruk"] as string,
                                        Name = row["im_ruk"] as string,
                                        Patronymic = row["ot_ruk"] as string,
                                        Phone = row["phone"] as string,
                                        FullAddress = row["addr_j"] as string
                                    };
                                    data.Add(model);

                                    _dockManager.SetOverlayProgress(table.Rows.Count, i);
                                }

                                var insertResult = _repository.InsertBatch(data);
                                if (insertResult.Success)
                                {
                                    Load();
                                    _notifyManager.ShowNotify("Данные успешно импортированы.");
                                }
                            }
                        }
                    }
                }

                
            };

            ClassifiersList = new ObservableCollection<ClassifierModel>
            {
                new ClassifierModel
                {
                    Id = 1,
                    GroupName = "Федеральные",
                    Name = "Штрафные санкции F014",
                    Table = "F014",
                    Editable = false
                },
                new ClassifierModel
                {
                    Id = 2,
                    GroupName = "Федеральные",
                    Name = "Федеральные округа F015",
                    Table = "F015",
                    Editable = false
                },
                new ClassifierModel
                {
                    Id = 3,
                    GroupName = "Федеральные",
                    Name = "Группы здоровья V017",
                    Table = "V017",
                    Editable = false
                },
                new ClassifierModel
                {
                    Id = 100,
                    GroupName = "Локальные",
                    Name = "Территориальные фонды",
                    Table = "localF001",
                    Editable = true
                },
                new ClassifierModel
                {
                    Id = 101,
                    GroupName = "Локальные",
                    Name = "Устаревшие профили МП",
                    Table = "globalObsoleteData",
                    Editable = true
                },
                new ClassifierModel
                {
                    Id = 102,
                    GroupName = "Локальные",
                    Name = "Эксперты",
                    Table = "F004",
                    Editable = true,
                    IsManualIdentity = true
                },
                new ClassifierModel
                {
                    Id = 103,
                    GroupName = "Локальные",
                    Name = "Экономические партнеры",
                    Table = "EconomicPartner",
                    Editable = true,
                    IsManualIdentity = false
                },
                new ClassifierModel
                {
                    Id = 1000,
                    GroupName = "Elmedicine",
                    Name = "Персонал МО",
                    Table = "shareDoctor",
                    Editable = false
                },
                new ClassifierModel
                {
                    Id = 1001,
                    GroupName = "Elmedicine",
                    Name = "Штрафные санкции",
                    Table = "globalRefusalPenalty",
                    ImportTable = "SANK_AUTO",
                    Editable = false
                },
                new ClassifierModel
                {
                    Id = 1002,
                    GroupName = "Elmedicine",
                    Name = "Тип случая МП",
                    Table = "globalMedicalEventType",
                    ImportTable = "TypeSluchDb",
                    Editable = false
                },
                new ClassifierModel
                {
                    Id = 1003,
                    GroupName = "Elmedicine",
                    Name = "Региональный признак",
                    Table = "globalRegionalAttribute",
                    ImportTable = "OsobSluchDb",
                    Editable = false
                },
                new ClassifierModel
                {
                    Id = 1004,
                    GroupName = "Elmedicine",
                    Name = "Лицензии",
                    Table = "globalLicense",
                    ImportTable = "LIC_NUM_TBL",
                    Editable = false
                },
                new ClassifierModel
                {
                    Id = 1005,
                    GroupName = "Elmedicine",
                    Name = "Соответствие профиля МП специльности врача",
                    Table = "globalV015EqV002",
                    ImportTable = "V015_V002",
                    Editable = false
                },
                new ClassifierModel
                {
                    Id = 1006,
                    GroupName = "Elmedicine",
                    Name = "Диспансеризация",
                    Table = "globalClinicalExamination",
                    ImportTable = "Shablon_Filter",
                    Editable = false
                },
                new ClassifierModel
                {
                    Id = 1007,
                    GroupName = "Elmedicine",
                    Name = "Код МКБ10 \u2192 Тип случая",
                    Table = "globalIDCToEventType",
                    ImportTable = "AmbulanceMKB",
                    Editable = false
                },
                new ClassifierModel
                {
                    Id = 1008,
                    GroupName = "Elmedicine",
                    Name = "Медицинские организации курской области",
                    Table = "localF003",
                    ImportTable = "F003Local",
                    Editable = false
                }
                
            };

        }

        private void Load()
        {
            if (_selectedClassifier.IsNotNull() && _selectedClassifier is ClassifierModel)
            {
                var classifier = _selectedClassifier as ClassifierModel;

                using (var scope = Di.I.BeginLifetimeScope())
                {
                    if (_dataHandlers.ContainsKey(classifier.Table))
                    {
                        _dataHandlers[classifier.Table](scope);
                    }
                }
            }
        }

        private bool CanImportDatabase()
        {
            return Classifier.IsNotNull() && Classifier.ImportTable.IsNotNullOrWhiteSpace();
        }

        private void ImportDatabase()
        {
            if (_messageService.AskQuestion("При иимпорте все данные будут перезаписаны, Вы уверены?", "Внимание!"))
            {

                Task.Factory.StartNew(() => _dockManager.ShowOverlay(Constants.LoadDataMsg, Constants.PleaseWaitMsg))
                    .ContinueWith(action =>
                    {
                        if (_importHandlers.ContainsKey(Classifier.Table))
                        {
                            _importHandlers[Classifier.Table]();
                        }

                    })
                    .ContinueWith(action => _dockManager.HideOverlay());
            }
        }

        private bool CanExportXml()
        {
            return SelectedClassifier.IsNotNull();
        }

        private void ExportXml()
        {
        }

        private bool CanImportXml()
        {
            return SelectedClassifier.IsNotNull();
        }

        private void ImportXml()
        {
        }

        private bool CanDelete()
        {
            return CurrentRow.IsNotNull() && Classifier.IsNotNull() && Classifier.Editable;
        }

        private void Delete()
        {
            if (CurrentRow.IsNull())
            {
                return;
            }

            if (_messageService.AskQuestion("Вы действительно хотите удалить эту запись?", "Внимание!"))
            {
                var deleteResult = _repository.Delete((CurrentRow as dynamic).Classifier);
                if (deleteResult.Success)
                {
                    Load();
                    _notifyManager.ShowNotify("Запись успешно удалена.");
                }
            }
        }

        private bool CanEdit()
        {
            return CurrentRow.IsNotNull() && Classifier.IsNotNull() && Classifier.Editable;
        }

        private void Edit()
        {
            try
            {
                if (CurrentRow.IsNull() || (_selectedClassifier as ClassifierModel) == null)
                {
                    return;
                }

                IsEditDataOpen = true;
                var copy = Map.ObjectToObject(CurrentRow, CurrentRow.GetType());
                object model = null;
                var classifier = _selectedClassifier as ClassifierModel;
                switch (classifier.Table)
                {
                    case "localF001":
                        model = new PropertyGridViewModel<EditLocalF001>(copy as EditLocalF001);
                        break;
                    case "F004":
                        model = new PropertyGridViewModel<EditF004>(copy as EditF004);
                        break;
                    case "EconomicPartner":
                        model = new PropertyGridViewModel<EditEconomicPartner>(copy as EditEconomicPartner);
                        break;
                    /*case "shareDoctor":
                        model = new PropertyGridViewModel<EditShareDoctor>(copy as EditShareDoctor);
                        break;
                    case "globalRefusalPenalty":
                        model = new PropertyGridViewModel<EditRefusalPenalty>(copy as EditRefusalPenalty);
                        break;
                    case "globalMedicalEventType":
                        model = new PropertyGridViewModel<EditMedicalEventType>(copy as EditMedicalEventType);
                        break;
                    case "globalRegionalAttribute":
                        model = new PropertyGridViewModel<EditRegionalAttribute>(copy as EditRegionalAttribute);
                        break;*/
                    case "globalObsoleteData":
                        model = new PropertyGridViewModel<EditObsoleteData>(copy as EditObsoleteData);
                        break;
                        
                }

                if (model == null)
                {
                    return;
                }

                (model as ICallbackable).OkCallback = () =>
                {
                    var result = _repository.Update((model as dynamic).SelectedObject.Classifier);
                    if (result.Success)
                    {
                        Load();
                        _notifyManager.ShowNotify("Данные успешно обновлены.");
                        IsEditDataOpen = false;
                    }
                };

                DataModel = model;
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при обновлении данных", typeof(ClassifierViewModel));
            }
        }

        private bool CanAdd()
        {
            return SelectedClassifier.IsNotNull() && Classifier.IsNotNull() && Classifier.Editable;
        }

        private void Add()
        {
            try
            {
                if ((_selectedClassifier as ClassifierModel) == null)
                {
                    return;
                }

                IsEditDataOpen = true;
                object model = null;
                var classifier = _selectedClassifier as ClassifierModel;
                switch (classifier.Table)
                {
                    case "localF001":
                        model = new PropertyGridViewModel<EditLocalF001>(new EditLocalF001());
                        break;
                    case "F004":
                        var f004 = new EditF004();
                        f004.Id = _repository.GetMax("F004", "Id");
                        model = new PropertyGridViewModel<EditF004>(f004);
                        break;
                    case "EconomicPartner":
                        var economicPartner = new EditEconomicPartner();
                        //economicPartner.EconomicPartnerId = _repository.GetMax("EconomicPartner", "EconomicPartnerId");
                        model = new PropertyGridViewModel<EditEconomicPartner>(economicPartner);
                        break;
                    /*case "shareDoctor":
                        model = new PropertyGridViewModel<EditShareDoctor>(new EditShareDoctor());
                        break;
                    case "globalRefusalPenalty":
                        model = new PropertyGridViewModel<EditRefusalPenalty>(new EditRefusalPenalty());
                        break;
                    case "globalMedicalEventType":
                        model = new PropertyGridViewModel<EditMedicalEventType>(new EditMedicalEventType());
                        break;
                    case "globalRegionalAttribute":
                        model = new PropertyGridViewModel<EditRegionalAttribute>(new EditRegionalAttribute());
                        break;*/
                    case "globalObsoleteData":
                        model = new PropertyGridViewModel<EditObsoleteData>(new EditObsoleteData());
                        break;
                }

                if (model == null)
                {
                    return;
                }

                (model as ICallbackable).OkCallback = () =>
                {
                    TransactionResult result = null;
                    if (Classifier != null && Classifier.IsManualIdentity)
                    {
                        result = _repository.InsertOrUpdate((model as dynamic).SelectedObject.Classifier);
                    }
                    else
                    {
                        result = _repository.InsertWithIdentity((model as dynamic).SelectedObject.Classifier);
                    }

                    if (result != null && result.Success)
                    {
                        Load();
                        _notifyManager.ShowNotify("Данные успешно обновлены.");
                        IsEditDataOpen = false;
                    }
                };

                DataModel = model;
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "Исключение при добавлении данных", typeof(ClassifierViewModel));
            }
        }
    }
}
