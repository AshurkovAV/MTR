using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Autofac;
using Core.Extensions;
using Core.Infrastructure;
using Core.Services;
using Core.Validation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Economic.Models;
using Medical.AppLayer.Linq;
using Medical.AppLayer.Managers;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;
using System.Windows.Input;
using System.Windows;
using BLToolkit.Mapping;
using Core;
using Core.Linq;
using DataModel;
using Medical.AppLayer.Editors;
using Medical.AppLayer.Register.ViewModels;
using Medical.CoreLayer.PropertyGrid;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.AppLayer.Economic.ViewModels
{
    public class EconomicActExpertiseViewModel : ViewModelBase, IContextCommandContainer, IHash, IDataErrorInfo
    {
        private readonly IMessageService _messageService;
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;
        private readonly IDockLayoutManager _dockManager;
        public static IAppRemoteSettings _remoteSettings;

        private readonly DataErrorInfoSupport _dataErrorInfoSupport;

        private PLinqEconomicActExpertiseList _actExpertiseListSource;
        private List<ActExpertiseShortView> _dataListSource;
        private List<ActExpertiseShortView> _filteredDataListSource;
        private ActExpertiseShortView _currentRow;

        private AddPaymentViewModel _addPaymentModel;
        private bool _isAddPaymentOpen;

        private RelayCommand _viewActCommand;
        private RelayCommand _editCommand;
        private RelayCommand _createCommand;
        private RelayCommand _refreshCommand;
        private RelayCommand _deleteCommand;
        private RelayCommand _resetDirectionCommand;
        private string _selectedTerritory;
        private int? _selectedDirection;
        private string _selectedYear;
        private RelayCommand<object> _changeStatusCommand;

        #region IContextCommandContainer
        public ObservableCollection<IContextCommand> ContextCommands { get; set; }
        public string PageName { get; set; }
        #endregion

        #region IHash
        public string Hash
        {
            get { return typeof(EconomicActExpertiseViewModel).FullName; }
        }
        #endregion
        public EconomicActExpertiseViewModel(PLinqEconomicActExpertiseList list, IMessageService messageService, IMedicineRepository repository, INotifyManager notifyManager)
        {
            ActExpertiseListSource = list;
            _messageService = messageService;
            _repository = repository;
            _notifyManager = notifyManager;
            _remoteSettings = Di.I.Resolve<IAppRemoteSettings>();
            _dockManager = Di.I.Resolve<IDockLayoutManager>();
            Initialize();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        private void Initialize()
        {
            YearItemsSource = new ObservableCollection<int>(new List<int> {2013,2014,2015,2016,2017,2018,2019} );
            SelectedYear = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
            

            PageName = "Акты экспертиз";
            ContextCommands = new ObservableCollection<IContextCommand>
            {
                new ContextCommand
                {
                    Id = "viewAccountFilter",
                    Caption = "Редактировать",
                    Command = new RelayCommand(EditAct, CanEditAct),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/edit.png",
                    SmallGlyph = "../Resources/Icons/edit.png",
                },
                new ContextCommand
                {
                    Id = "viewAccountFilter",
                    Caption = "Обновить",
                    Command = new RelayCommand(RefreshAct),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/view-refresh.png",
                    SmallGlyph = "../Resources/Icons/view-refresh.png",
                },
                new ContextCommand
                {
                    Id = "viewAccountFilter",
                    Caption = "Удалить",
                    Command = new RelayCommand(DeleteAct, CanDeleteAct),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/edit-delete.png",
                    SmallGlyph = "../Resources/Icons/edit-delete.png",
                },
                new ContextCommand
                {
                    Id = "changeStatus",
                    Caption = "Статус акта",
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/view-refresh-2.png",
                    SmallGlyph = "../Resources/Icons/view-refresh-2.png",
                    IsParent = true
                },
                new ContextCommand
                {
                    Id = "viewOnlyError",
                    Caption = "Новый",
                    LargeGlyph = "../Resources/Icons/dialog-error.png",
                    SmallGlyph = "../Resources/Icons/dialog-error.png",
                    Parent = "changeStatus",
                    Command = new RelayCommand<object>(ChangeStatus, CanChangeStatus),
                    CommandParameter = 1
                },
                new ContextCommand
                {
                    Id = "viewOnlyError",
                    Caption = "Утвержден",
                    LargeGlyph = "../Resources/Icons/dialog-clean.png",
                    SmallGlyph = "../Resources/Icons/dialog-clean.png",
                    Parent = "changeStatus",
                    Command = new RelayCommand<object>(ChangeStatus, CanChangeStatus),
                    CommandParameter = 2
                },
            };
        }
        public ICommand ViewAccountCommand => _viewActCommand ??
                                              (_viewActCommand = new RelayCommand(ViewAct, CanViewAct));
        public ICommand CreateCommand => _createCommand ??
                                       (_createCommand = new RelayCommand(CreateAct, CanCreateAct));
        public ICommand EditCommand => _editCommand ??
                                              (_editCommand = new RelayCommand(EditAct, CanEditAct));
        public ICommand RefreshCommand => _refreshCommand ??
                                              (_refreshCommand = new RelayCommand(RefreshAct));
        public ICommand DeleteCommand => _deleteCommand ??
                                              (_deleteCommand = new RelayCommand(DeleteAct, CanDeleteAct));
        public ICommand ChangeStatusCommand => _changeStatusCommand ??
                                              (_changeStatusCommand = new RelayCommand<object>(ChangeStatus, CanChangeStatus));
        public ICommand ResetDirectionCommand => _resetDirectionCommand ??
                                                 (_resetDirectionCommand = new RelayCommand(ResetDirection, CanResetDirection));

        public ObservableCollection<int> YearItemsSource { get; set; }

        private void ResetDirection()
        {
            SelectedDirection = 0;
        }

        private bool CanResetDirection()
        {
            return _selectedDirection.HasValue;
        }

        public int? SelectedDirection
        {
            get { return _selectedDirection; }
            set
            {
                _selectedDirection = value;
                RaisePropertyChanged();
                UpdateFilter();
            }
        }

        public string SelectedTerritory
        {
            get { return _selectedTerritory; }
            set
            {
                _selectedTerritory = value;
                RaisePropertyChanged();
                UpdateFilter();
            }
        }

        private void UpdateFilter()
        {
            Expression<Func<FactActExpertise, bool>> predicate = PredicateBuilder.True<FactActExpertise>();
            int yearResult;
            if (int.TryParse(_selectedYear, out yearResult))
            {
                predicate = predicate.And(p => p.DateAct.Value.Year == yearResult);
            }

           // ActExpertiseListSource = Di.I.Resolve<PLinqEconomicActExpertiseList>(new NamedParameter("predicate", predicate));
        }

        public string SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                _selectedYear = value;
                RaisePropertyChanged();
                UpdateFilter();
            }
        }
        private bool CanExportXml()
        {
            return CurrentRow != null;
        }

        public ActExpertiseShortView CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged("CurrentRow");
            }
        }

        public List<ActExpertiseShortView> ListSource
        {
            get { return _dataListSource; }
            set
            {
                if (_dataListSource == value) return;
                _dataListSource = value;
                RaisePropertyChanged("ListSource");
            }
        }

        public List<ActExpertiseShortView> FilteredListSource
        {
            get { return _filteredDataListSource; }
            set
            {
                if (_filteredDataListSource == value) return;
                _filteredDataListSource = value;
                RaisePropertyChanged("FilteredListSource");
            }
        }

        public PLinqEconomicActExpertiseList ActExpertiseListSource
        {
            get { return _actExpertiseListSource; }
            set
            {
                if (_actExpertiseListSource == value) return;
                _actExpertiseListSource = value;
                RaisePropertyChanged("ActExpertiseListSource");
            }
        }

        public AddPaymentViewModel AddPaymentModel
        {
            get { return _addPaymentModel; }
            set { _addPaymentModel = value; RaisePropertyChanged(() => AddPaymentModel); }
        }

        public bool IsAddPaymentOpen
        {
            get { return _isAddPaymentOpen; }
            set
            {
                _isAddPaymentOpen = value;
                RaisePropertyChanged(() => IsAddPaymentOpen);
            }
        }

        public string Error
        {
            get
            {
                RaisePropertyChanged("Visibility");
                return _dataErrorInfoSupport.Error;
            }
        }
        private bool CanViewAct()
        {
            return CurrentRow != null && CurrentRow.GetType() != typeof(DevExpress.Data.NotLoadedObject);
        }

        private void ViewAct()
        {
            try
            {
                if (CurrentRow != null)
                {
                    VidControls vidControls;
                    switch (CurrentRow.VidControlId)
                    {
                        case 1: vidControls = VidControls.MEC;
                            break;
                        case 2: vidControls = VidControls.MEE;
                            break;
                        case 3: vidControls = VidControls.EQMA;
                            break;
                        case 4: vidControls = VidControls.ExternalMEC;
                            break;
                        case 5: vidControls = VidControls.ExternalMEE;
                            break;
                        case 6: vidControls = VidControls.ExternalEQMA;
                            break;
                        default: vidControls = VidControls.Unknown;
                            break;
                    }

                    _dockManager.ShowActExpertiseCollection(CurrentRow.ActExpertiseId, vidControls);
                }
                else
                {
                    _notifyManager.ShowNotify("Нет данных для просмотра.");
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception,
                    "Исключение при открытии позиций счета МО ID {0}".F(CurrentRow.MedicalAccountId),
                    typeof(MedicalAccountViewModel));
            }
        }


        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get { return _dataErrorInfoSupport.Error; }
        }

        string IDataErrorInfo.this[string memberName]
        {
            get { return _dataErrorInfoSupport[memberName]; }
        }

        #endregion

        private bool CanChangeStatus(object sender)
        {
            return CurrentRow != null;
        }

        private void ChangeStatus(object sender)
        {
            try
            {
                var status = sender.ToInt32Nullable();
                var statusResult = _repository.ChangeActExperrtiseStatus(CurrentRow.ActExpertiseId, status);
                if (statusResult.Success)
                {
                    CurrentRow.ActExspertiStatusId = status;
                    RefreshAct();
                    CurrentRow.UpdateProperty("ActExspertiStatusId");
                    _notifyManager.ShowNotify("Статус акта экспертизы успешно изменен.");
                    
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При изменении статуса акта экспертизы ID {0} произошла ошибка.".F(CurrentRow.ActExpertiseId), typeof(EconomicActExpertiseViewModel));
            }
        }

        private void RefreshAct()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                if (ActExpertiseListSource.Predicate != null)
                {
                    ActExpertiseListSource = scope.Resolve<PLinqEconomicActExpertiseList>(
                        new NamedParameter("predicate", ActExpertiseListSource.Predicate));
                }
                else
                {
                    ActExpertiseListSource = scope.Resolve<PLinqEconomicActExpertiseList>();
                }
            }
        }

        private bool CanDeleteAct()
        {
            return CurrentRow != null && CurrentRow.ActExspertiStatusId != 2;
        }

        private void DeleteAct()
        {
            try
            {
                if (!_messageService.AskQuestion("Вы действительно хотите удалить данные акта экспертизы?", "Внимание!"))
                {
                    return;
                }

                var deleteResult = _repository.DeleteActAxpertise(CurrentRow.ActExpertiseId);
                if (deleteResult.Success)
                {
                    _notifyManager.ShowNotify("Данные об акте экспертизы удалены.");
                    RefreshAct();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При удалении акта экспертизы ID {0} произошла ошибка.".F(CurrentRow.ActExpertiseId), typeof(EconomicActExpertiseViewModel));
            }
        }
        private bool CanCreateAct()
        {
            return true;
        }

        private void CreateAct()
        {
            try
            {
                dynamic clientMo = _remoteSettings.Get(AppRemoteSettings.ClientMo);
                string mcod = clientMo.Mcod;

                var model =
                    new PropertyGridViewModel<EditActExpertiseViewModel>(
                        new EditActExpertiseViewModel(new FactActExpertise
                        {
                            Mo = mcod,
                            DateEdit= DateTime.Now,
                            DateAct = DateTime.Now,
                            DateExpertiseBegin = DateTime.Now,
                            DateExpertiseEnd = DateTime.Now,
                            ActExspertiStatusId = (int)StatusActExpertise.New,

                        }));
                var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
                model.OkCallback = () =>
                {
                    var tmp = Map.ObjectToObject<FactActExpertise>(model.SelectedObject.Classifier);

                    var result = _repository.InsertOrUpdateActExpertise(tmp);
                    if (result.Success)
                    {
                        RefreshAct();
                        _notifyManager.ShowNotify("Данные акта успешно добавлены.");
                        view.Close();
                    }
                    else
                    {
                        _notifyManager.ShowNotify($"Ошибка при сохранении.{result.LastError.Message}");
                    }

                };
                view.ShowDialog();
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При редактировании акта ID {0} произошла ошибка.".F(CurrentRow.ActExpertiseId), typeof(EconomicActExpertiseViewModel));
            }
        }


        private bool CanEditAct()
        {
            return CurrentRow != null;
        }

        private void EditAct()
        {
            try
            {
                var copy = Map.ObjectToObject<FactActExpertise>(CurrentRow);
                var model = new PropertyGridViewModel<EditActExpertiseViewModel>(new EditActExpertiseViewModel(copy));
                var view = new CommonWindow(Application.Current.MainWindow, 800, 600, model);
                model.OkCallback = () =>
                {
                    var tmp = Map.ObjectToObject<FactActExpertise>(model.SelectedObject.Classifier);

                    var result = _repository.Update(tmp);
                    if (result.Success)
                    {
                        var map = model.SelectedObject.MapObject<ActExpertiseShortView>(CurrentRow);
                        CurrentRow.Update(map.Affected);
                        RefreshAct();
                        _notifyManager.ShowNotify("Данные акта успешно обновлены.");
                        view.Close();
                    }
                    else
                    {
                        _notifyManager.ShowNotify($"Ошибка {result.LastError.Message}");
                    }

                };

                view.ShowDialog();
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При редактировании акта ID {0} произошла ошибка.".F(CurrentRow.ActExpertiseId), typeof(EconomicActExpertiseViewModel));
            }
        }
    }
}