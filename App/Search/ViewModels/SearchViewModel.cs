using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Core;
using Core.Extensions;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Models;
using Medical.AppLayer.Models.OperatorModels;
using Medical.AppLayer.Models.PatientSearch;
using Medical.AppLayer.Services;
using System;

namespace Medical.AppLayer.Search.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private readonly ISearchService _searchService;
        private readonly INotifyManager _notifyManager;
        private readonly IDockLayoutManager _dockLayoutManager;
        private readonly ICommonService _commonService;

        private ObservableCollection<SearchFormModel> _formList;
        private ObservableCollection<SearchResultModel> _resultList;
        private ObservableCollection<object> _detailsCollection;

        private RelayCommand _openAccountCommand;
        private RelayCommand _openMedicalAccountCommand;
        private RelayCommand _openPatientCommand;
        private RelayCommand _resetCommand;
        private RelayCommand _searchCommand;

        private SearchFormModel _selectedForm;
        private SearchResultModel _selectedPatient;

        public SearchViewModel(ISearchService searchService,
            ICommonService commonService,
            INotifyManager notifyManager,
            IDockLayoutManager dockLayoutManager)
        {
            _searchService = searchService;
            _commonService = commonService;
            _notifyManager = notifyManager;
            _dockLayoutManager = dockLayoutManager;
            Init();
        }

        public ObservableCollection<SearchFormModel> FormList
        {
            get { return _formList; }
            set
            {
                _formList = value;
                RaisePropertyChanged(() => FormList);
            }
        }

        public ObservableCollection<SearchResultModel> ResultList
        {
            get { return _resultList; }
            set
            {
                _resultList = value;
                RaisePropertyChanged(() => ResultList);
            }
        }

        public ObservableCollection<object> DetailsCollection
        {
            get { return _detailsCollection; }
            set
            {
                _detailsCollection = value;
                RaisePropertyChanged(() => DetailsCollection);
            }
        }


        public SearchFormModel SelectedForm
        {
            get { return _selectedForm; }
            set
            {
                _selectedForm = value;
                RaisePropertyChanged(() => SelectedForm);
            }
        }

        public SearchResultModel SelectedPatient
        {
            get { return _selectedPatient; }
            set
            {
                _selectedPatient = value;
                RaisePropertyChanged(() => SelectedPatient);
                if (_selectedPatient.IsNotNull())
                {
                    //TODO load details
                }
            }
        }

        public ICommand SearchCommand
        {
            get { return _searchCommand ?? (_searchCommand = new RelayCommand(SearchPatient, CanSearchPatient)); }
        }

        public ICommand ResetCommand
        {
            get { return _resetCommand ?? (_resetCommand = new RelayCommand(Reset, CanReset)); }
        }

        public ICommand OpenPatientCommand
        {
            get { return _openPatientCommand ?? (_openPatientCommand = new RelayCommand(OpenPatient, CanOpenPatient)); }
        }

        public ICommand OpenMedicalAccountCommand
        {
            get
            {
                return _openMedicalAccountCommand ??
                       (_openMedicalAccountCommand = new RelayCommand(OpenMedicalAccount, CanOpenMedicalAccount));
            }
        }

        public ICommand OpenAccountCommand
        {
            get { return _openAccountCommand ?? (_openAccountCommand = new RelayCommand(OpenAccount, CanOpenAccount)); }
        }

        public void Init()
        {
            FormList = new ObservableCollection<SearchFormModel>
            {
                new SearchFormModel
                {
                    Name = "Поиск пациентов",
                    SelectedObject = new PatientSearchFormViewModel(),
                    Handler = ProcessPatientForm
                },
                new SearchFormModel
                {
                    Name = "Поиск случаев",
                    SelectedObject = new EventSearchFormViewModel(),
                    Handler = ProcessEventForm
                }
            };

            ResultList = new ObservableCollection<SearchResultModel>();
        }

        private void ProcessEventForm(SearchFormModel obj)
        {
            Task.Factory.StartNew(() => _dockLayoutManager.ShowOverlay(Constants.SearchDataTitleMsg, Constants.PleaseWaitMsg))
                .ContinueWith(action =>
                {
                    if (obj.SelectedObject is EventSearchFormViewModel)
                    {
                        var model = obj.SelectedObject as EventSearchFormViewModel;

                        var parameters = new Dictionary<SearchParameters, object>
                        {
                            {SearchParameters.EventBeginDate, model.EventBeginDate},
                            {SearchParameters.EventEndDate, model.EventEndDate},
                            {SearchParameters.EndEventBeginDate, model.EndEventBeginDate},
                            {SearchParameters.EndEventEndDate, model.EndEventEndDate},
                            {SearchParameters.IsUnderpayment, model.IsUnderpayment}
                        };

                        //IEnumerable<EventShortView> result =
                        //    _searchService.SearchByParameters(
                        //        parameters.Where(p => p.Value.IsNotNull()).ToDictionary(p => p.Key, p => p.Value));

                        IEnumerable<GeneralEventShortView> result =
                            _searchService.SearchGeneralByParameters(
                                parameters.Where(p => p.Value.IsNotNull()).ToDictionary(p => p.Key, p => p.Value));
                        if (result.IsNotNull())
                        {
                            ResultList =
                                new ObservableCollection<SearchResultModel>(result.Select(p => new SearchResultModel
                                {
                                    PatientId = p.PatientId,
                                    PatientData = p.PatientData,
                                    MeventData = p.MeventDataShort,
                                    AccountId = p.AccountId,
                                    MedicalAccountId = p.MedicalAccountId,
                                    EventId = p.EventId,
                                    Version = p.Version,
                                    AccountData = _commonService.GenerateAccountData(p)
                                }));
                        }

                        if (!ResultList.Any())
                        {
                            _notifyManager.ShowNotify("Данные не найдены. Смягчите условия поиска.");
                        }
                    }
                }).ContinueWith(action => _dockLayoutManager.HideOverlay());
        }

        private void ProcessPatientForm(SearchFormModel obj)
        {
            Task.Factory.StartNew(() => _dockLayoutManager.ShowOverlay(Constants.SearchDataTitleMsg, Constants.PleaseWaitMsg))
                .ContinueWith(action =>
                {
                    if (obj.SelectedObject is PatientSearchFormViewModel)
                    {
                        var model = obj.SelectedObject as PatientSearchFormViewModel;
                        var parameters = new Dictionary<SearchParameters, object>
                        {
                            {SearchParameters.InsuranceNumber, model.InsuranceNumber},
                            {SearchParameters.Id, model.Id},
                            {SearchParameters.Name, model.Name},
                            {SearchParameters.Surname, model.Surname},
                            {SearchParameters.Patronymic, model.Patronymic},
                            {SearchParameters.BirthDate, model.BirthDate},
                            {SearchParameters.Sex, model.Sex},
                            {SearchParameters.IsUnderpayment, model.IsUnderpayment}
                        };

                        //IEnumerable<EventShortView> result =
                        //    _searchService.SearchByParameters(
                        //        parameters.Where(p => p.Value.IsNotNull()).ToDictionary(p => p.Key, p => p.Value));

                        IEnumerable<GeneralEventShortView> result =
                           _searchService.SearchGeneralByParameters(
                               parameters.Where(p => p.Value.IsNotNull()).ToDictionary(p => p.Key, p => p.Value));
                        if (result.IsNotNull())
                        {
                            ResultList =
                                new ObservableCollection<SearchResultModel>(result.Select(p => new SearchResultModel
                                {
                                    PatientId = p.PatientId,
                                    PatientData = p.PatientData,
                                    MeventData = p.MeventDataShort,
                                    AccountId = p.AccountId,
                                    MedicalAccountId = p.MedicalAccountId,
                                    EventId = p.EventId,
                                    Version = p.Version,
                                    AccountData = _commonService.GenerateAccountData(p)
                                }));
                        }

                        if (!ResultList.Any())
                        {
                            _notifyManager.ShowNotify("Данные не найдены. Смягчите условия поиска.");
                        }
                    }
                }).ContinueWith(actions => _dockLayoutManager.HideOverlay());
        }

        private bool CanSearchPatient()
        {
            return SelectedForm.IsNotNull() && (SelectedForm.SelectedObject as dynamic).IsNotEmpty;
        }

        private void SearchPatient()
        {
            _dockLayoutManager.ShowOverlay(Constants.SearchDataTitleMsg, Constants.PleaseWaitMsg);
            if (SelectedForm.IsNotNull())
            {
                SelectedForm.Handler(SelectedForm);
            }
            _dockLayoutManager.HideOverlay();
        }

        private bool CanReset()
        {
            return SelectedForm.IsNotNull() && (SelectedForm.SelectedObject as dynamic).IsNotEmpty;
        }

        private void Reset()
        {
            SelectedForm.SelectedObject.ResetDefaultValues();
            (SelectedForm.SelectedObject as dynamic).UpdateAll();
            ResultList = new ObservableCollection<SearchResultModel>();
        }

        private bool CanOpenPatient()
        {
            return SelectedPatient.IsNotNull() && SelectedPatient.PatientId.HasValue;
        }

        private void OpenPatient()
        {
            if (SelectedPatient.PatientId.HasValue)
            {
                _dockLayoutManager.ShowOperator(SelectedPatient.PatientId.Value, OperatorMode.Patient);
            }
        }

        private bool CanOpenMedicalAccount()
        {
            return SelectedPatient.IsNotNull() && SelectedPatient.MedicalAccountId.HasValue &&
                   SelectedPatient.PatientId.HasValue && SelectedPatient.EventId.HasValue;
        }

        private void OpenMedicalAccount()
        {
            if (SelectedPatient.MedicalAccountId.HasValue && SelectedPatient.PatientId.HasValue &&
                SelectedPatient.EventId.HasValue)
            {
                if (Constants.Zversion.Contains(SelectedPatient.Version))
                {
                    _dockLayoutManager.ShowOperator(SelectedPatient.MedicalAccountId.Value, OperatorMode.Zlocal,
                        new List<OperatorAction>
                        {
                            new OperatorAction
                            {
                                Id = SelectedPatient.PatientId.Value,
                                ActionType = OperatorActionType.GoToPatient
                            },
                            new OperatorAction
                            {
                                Id = SelectedPatient.EventId.Value,
                                ActionType = OperatorActionType.GoToMedicalEvent
                            }
                        });
                }
                else
                {
                    _dockLayoutManager.ShowOperator(SelectedPatient.MedicalAccountId.Value, OperatorMode.Local,
                        new List<OperatorAction>
                        {
                            new OperatorAction
                            {
                                Id = SelectedPatient.PatientId.Value,
                                ActionType = OperatorActionType.GoToPatient
                            },
                            new OperatorAction
                            {
                                Id = SelectedPatient.EventId.Value,
                                ActionType = OperatorActionType.GoToMedicalEvent
                            }
                        });
                }
            }
        }

        private bool CanOpenAccount()
        {
            return SelectedPatient.IsNotNull() && SelectedPatient.AccountId.HasValue &&
                   SelectedPatient.PatientId.HasValue && SelectedPatient.EventId.HasValue;
        }

        private void OpenAccount()
        {
            if (SelectedPatient.AccountId.HasValue && SelectedPatient.PatientId.HasValue &&
                SelectedPatient.EventId.HasValue)
            {
                
                if (Constants.Zversion.Contains(SelectedPatient.Version))
                {
                    _dockLayoutManager.ShowOperator(SelectedPatient.AccountId.Value, OperatorMode.ZInterTerritorial,
                        new List<OperatorAction>
                        {
                            new OperatorAction
                            {
                                Id = SelectedPatient.PatientId.Value,
                                ActionType = OperatorActionType.GoToPatient
                            },
                            new OperatorAction
                            {
                                Id = SelectedPatient.EventId.Value,
                                ActionType = OperatorActionType.GoToMedicalEvent
                            }
                        });
                }
                else
                {
                    _dockLayoutManager.ShowOperator(SelectedPatient.AccountId.Value, OperatorMode.InterTerritorial,
                        new List<OperatorAction>
                        {
                            new OperatorAction
                            {
                                Id = SelectedPatient.PatientId.Value,
                                ActionType = OperatorActionType.GoToPatient
                            },
                            new OperatorAction
                            {
                                Id = SelectedPatient.EventId.Value,
                                ActionType = OperatorActionType.GoToMedicalEvent
                            }
                        });
                }
            }
        }
    }
}