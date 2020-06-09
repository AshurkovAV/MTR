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
using Core.Linq;
using DataModel;
using Medical.AppLayer.Economic.Views;
using Medical.AppLayer.Register.ViewModels;
using Medical.AppLayer.Register.Views;
using CommonWindow = Medical.AppLayer.Editors.CommonWindow;
using CommonWindowControl = Medical.AppLayer.Editors.CommonWindowControl;

namespace Medical.AppLayer.Economic.ViewModels
{
    public class EconomicAccountViewModel : ViewModelBase, IContextCommandContainer, IHash, IDataErrorInfo
    {
        private readonly IMessageService _messageService;
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;

        private readonly DataErrorInfoSupport _dataErrorInfoSupport;

        private PLinqEconomicAccountList _accountListSource;
        private List<EconomicAccountCustomModel> _dataListSource;
        private List<EconomicAccountCustomModel> _filteredDataListSource;
        private EconomicAccountCustomModel _currentRow;

        private AddPaymentViewModel _addPaymentModel;
        private bool _isAddPaymentOpen;

        private RelayCommand _editCommand;
        private RelayCommand _refreshCommand;
        private RelayCommand _deleteCommand;
        private RelayCommand _exportAccountCommand;
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
            get { return typeof(EconomicAccountViewModel).FullName; }
        }
        #endregion
        public EconomicAccountViewModel(PLinqEconomicAccountList list, IMessageService messageService, IMedicineRepository repository, INotifyManager notifyManager)
        {
            AccountListSource = list;
            _messageService = messageService;
            _repository = repository;
            _notifyManager = notifyManager;
            Initialize();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        private void Initialize()
        {
            YearItemsSource = new ObservableCollection<int>(new List<int> {2013,2014,2015,2016,2017,2018,2019,2020,2021} );
            SelectedYear = DateTime.Now.Year.ToString(CultureInfo.InvariantCulture);
            

            PageName = "Платежные поручения";
            ContextCommands = new ObservableCollection<IContextCommand>
            {
                new ContextCommand
                {
                    Id = "viewAccountFilter",
                    Caption = "Редактировать",
                    Command = new RelayCommand(EditAccount, CanEditAccount),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/edit.png",
                    SmallGlyph = "../Resources/Icons/edit.png",
                },
                new ContextCommand
                {
                    Id = "viewAccountFilter",
                    Caption = "Обновить",
                    Command = new RelayCommand(RefreshAccount),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/view-refresh.png",
                    SmallGlyph = "../Resources/Icons/view-refresh.png",
                },
                new ContextCommand
                {
                    Id = "viewAccountFilter",
                    Caption = "Удалить",
                    Command = new RelayCommand(DeleteAcount, CanDeleteAcount),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/edit-delete.png",
                    SmallGlyph = "../Resources/Icons/edit-delete.png",
                },
                new ContextCommand
                {
                    Id = "changeStatus",
                    Caption = "Статус оплаты",
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/view-refresh-2.png",
                    SmallGlyph = "../Resources/Icons/view-refresh-2.png",
                    IsParent = true
                },
                new ContextCommand
                {
                    Id = "viewOnlyError",
                    Caption = "Не оплачено",
                    LargeGlyph = "../Resources/Icons/dialog-error.png",
                    SmallGlyph = "../Resources/Icons/dialog-error.png",
                    Parent = "changeStatus",
                    Command = new RelayCommand<object>(ChangeStatus, CanChangeStatus),
                    CommandParameter = 1
                },
                new ContextCommand
                {
                    Id = "viewOnlyError",
                    Caption = "Деньги перечислены",
                    LargeGlyph = "../Resources/Icons/dialog-clean.png",
                    SmallGlyph = "../Resources/Icons/dialog-clean.png",
                    Parent = "changeStatus",
                    Command = new RelayCommand<object>(ChangeStatus, CanChangeStatus),
                    CommandParameter = 2
                },
                new ContextCommand
                {
                    Id = "viewOnlyError",
                    Caption = "Платежное поручение выгруженно",
                    LargeGlyph = "../Resources/Icons/dialog-clean.png",
                    SmallGlyph = "../Resources/Icons/dialog-clean.png",
                    Parent = "changeStatus",
                    Command = new RelayCommand<object>(ChangeStatus, CanChangeStatus),
                    CommandParameter = 3
                },
            };
        }

        public ICommand EditCommand => _editCommand ??
                                              (_editCommand = new RelayCommand(EditAccount, CanEditAccount));
        public ICommand RefreshCommand => _refreshCommand ??
                                              (_refreshCommand = new RelayCommand(RefreshAccount));
        public ICommand DeleteCommand => _deleteCommand ??
                                              (_deleteCommand = new RelayCommand(DeleteAcount, CanDeleteAcount));
        public ICommand ChangeStatusCommand => _changeStatusCommand ??
                                              (_changeStatusCommand = new RelayCommand<object>(ChangeStatus, CanChangeStatus));
        public ICommand ExportAccountCommand => _exportAccountCommand ??
                                             (_exportAccountCommand = new RelayCommand(ExportXml, CanExportXml));
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
            Expression<Func<FactEconomicAccount, bool>> predicate = PredicateBuilder.True<FactEconomicAccount>();
            int yearResult;
            if (int.TryParse(_selectedYear, out yearResult))
            {
                predicate = predicate.And(p => p.PaymentOrderDate.Value.Year == yearResult);
            }
            if (SelectedDirection.HasValue && SelectedDirection > 0)
            {
                var direction = _selectedDirection - 1;
                predicate = predicate.And(p => p.FACTTERRACCACCID.Direction == direction);
            }
            if (!string.IsNullOrWhiteSpace(_selectedTerritory))
            {
                predicate = predicate.And(p => p.FACTTERRACCACCID.Source == _selectedTerritory || p.FACTTERRACCACCID.Destination == _selectedTerritory);
            }

            AccountListSource = Di.I.Resolve<PLinqEconomicAccountList>(new NamedParameter("predicate", predicate));
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

        public EconomicAccountCustomModel CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged("CurrentRow");
            }
        }

        public List<EconomicAccountCustomModel> ListSource
        {
            get { return _dataListSource; }
            set
            {
                if (_dataListSource == value) return;
                _dataListSource = value;
                RaisePropertyChanged("ListSource");
            }
        }

        public List<EconomicAccountCustomModel> FilteredListSource
        {
            get { return _filteredDataListSource; }
            set
            {
                if (_filteredDataListSource == value) return;
                _filteredDataListSource = value;
                RaisePropertyChanged("FilteredListSource");
            }
        }

        public PLinqEconomicAccountList AccountListSource
        {
            get { return _accountListSource; }
            set
            {
                if (_accountListSource == value) return;
                _accountListSource = value;
                RaisePropertyChanged("AccountListSource");
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
                var statusResult = _repository.ChangeEconomicAccountStatus(CurrentRow.Account.EconomicAccountId, CurrentRow.Account.AccountId, status);
                if (statusResult.Success)
                {
                    CurrentRow.Account.PaymentStatus = status;
                    CurrentRow.UpdateProperty("Account.PaymentStatus");
                    _notifyManager.ShowNotify("Статус платежа успешно изменен.");
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При изменении статуса платежа ID {0} произошла ошибка.".F(CurrentRow.Account.EconomicAccountId), typeof(EconomicAccountViewModel));
            }
        }

        private void RefreshAccount()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                if (AccountListSource.Predicate != null)
                {
                    AccountListSource = scope.Resolve<PLinqEconomicAccountList>(
                        new NamedParameter("predicate", AccountListSource.Predicate));
                }
                else
                {
                    AccountListSource = scope.Resolve<PLinqEconomicAccountList>();
                }
            }
        }

        private bool CanDeleteAcount()
        {
            return CurrentRow != null && CurrentRow.Account.PaymentStatus != 2;
        }

        private void DeleteAcount()
        {
            try
            {
                if (!_messageService.AskQuestion("Вы действительно хотите удалить данные об оплате?", "Внимание!"))
                {
                    return;
                }

                var deleteResult = _repository.DeleteEconomicAccount(CurrentRow.Account.EconomicAccountId, CurrentRow.Account.AccountId);
                if (deleteResult.Success)
                {
                    _notifyManager.ShowNotify("Данные об оплате успешно удалены.");
                    RefreshAccount();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При удалении платежа ID {0} произошла ошибка.".F(CurrentRow.Account.EconomicAccountId), typeof(EconomicAccountViewModel));
            }
        }

        private bool CanEditAccount()
        {
            return CurrentRow != null;
        }

        private void EditAccount()
        {
            try
            {
                IsAddPaymentOpen = true;
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    AddPaymentModel = scope.Resolve<AddPaymentViewModel>(
                        new NamedParameter("accountId", CurrentRow.Account.AccountId),
                        new NamedParameter("economicAccountId", CurrentRow.Account.EconomicAccountId));
                    AddPaymentModel.OkCallback = () =>
                    {
                        _notifyManager.ShowNotify("Данные платежа успешно отредактированы.");
                        //TODO replace by current row update
                        RefreshAccount();
                        IsAddPaymentOpen = false;
                    };
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При редактировании платежа ID {0} произошла ошибка.".F(CurrentRow.Account.EconomicAccountId), typeof(EconomicAccountViewModel));
            }
        }

        private void ExportXml()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    var model = scope.Resolve<EcExportXmlViewModel>(
                        new NamedParameter("id", CurrentRow.Account.AccountId),
                        new NamedParameter("economicAccountId", CurrentRow.Account.EconomicAccountId));

                    var view = new CommonWindowControl(Application.Current.MainWindow, 800, 600, model, new EcExportXmlView());
                    view.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception,
                    "При выгрузке файла OMS для межтерриториального счета ID {0} произошла ошибка.".F(
                        CurrentRow.Account.EconomicAccountId), 
                    typeof (EconomicAccountViewModel));
            }
        }

        

    }
}