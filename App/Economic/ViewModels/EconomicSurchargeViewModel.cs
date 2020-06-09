using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using Autofac;
using Core.Extensions;
using Core.Infrastructure;
using Core.Services;
using Core.Validation;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Economic.Models;
using Medical.AppLayer.Linq;
using Medical.AppLayer.Managers;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Economic.ViewModels
{
    public class EconomicSurchargeViewModel : ViewModelBase, IContextCommandContainer, IHash, IDataErrorInfo
    {
        private readonly IMessageService _messageService;
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;

        private readonly DataErrorInfoSupport _dataErrorInfoSupport;

        private PLinqEconomicSurchargeList _accountListSource;
        private EconomicSurchargeCustomModel _currentRow;

        private AddSurchargeViewModel _addSurchargeModel;
        private bool _isAddSurchargeOpen;

        #region IContextCommandContainer
        public ObservableCollection<IContextCommand> ContextCommands { get; set; }
        public string PageName { get; set; }
        #endregion

        #region IHash
        public string Hash
        {
            get { return typeof(EconomicSurchargeViewModel).FullName; }
        }
        #endregion

        private Expression<Func<FactEconomicSurcharge, bool>> Predicate { get; set; }

        public EconomicSurchargeViewModel(PLinqEconomicSurchargeList list, IMessageService messageService, IMedicineRepository repository, INotifyManager notifyManager)
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
            PageName = "Доплаты";
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
                }
            };
        }

        public EconomicSurchargeCustomModel CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged("CurrentRow");
            }
        }

        public PLinqEconomicSurchargeList AccountListSource
        {
            get { return _accountListSource; }
            set
            {
                if (_accountListSource == value) return;
                _accountListSource = value;
                RaisePropertyChanged("AccountListSource");
            }
        }

        public AddSurchargeViewModel AddSurchargeModel
        {
            get { return _addSurchargeModel; }
            set { _addSurchargeModel = value; RaisePropertyChanged(() => AddSurchargeModel); }
        }

        public bool IsAddSurchargeOpen
        {
            get { return _isAddSurchargeOpen; }
            set
            {
                _isAddSurchargeOpen = value;
                RaisePropertyChanged(() => IsAddSurchargeOpen);
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

        private void RefreshAccount()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                if (AccountListSource.Predicate != null)
                {
                    AccountListSource = scope.Resolve<PLinqEconomicSurchargeList>(
                        new NamedParameter("predicate", AccountListSource.Predicate));
                }
                else
                {
                    AccountListSource = scope.Resolve<PLinqEconomicSurchargeList>();
                }

            }
        }

        private bool CanDeleteAcount()
        {
            return CurrentRow != null;
        }

        private void DeleteAcount()
        {
            try
            {
                if (!_messageService.AskQuestion("Вы действительно хотите удалить данные о доплатах?", "Внимание!"))
                {
                    return;
                }

                var deleteResult = _repository.DeleteEconomicSurcharge(CurrentRow.Surcharge.EconomicSurchargeId);
                if (deleteResult.Success)
                {
                    _notifyManager.ShowNotify("Данные о доплатах успешно удалены.");
                    RefreshAccount();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При удалении данных о доплатах ID {0} произошла ошибка.".F(CurrentRow.Surcharge.EconomicSurchargeId), typeof(EconomicSurchargeViewModel));
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
                IsAddSurchargeOpen = true;
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    AddSurchargeModel = scope.Resolve<AddSurchargeViewModel>(
                        new NamedParameter("accountId", CurrentRow.Surcharge.AccountId),
                        new NamedParameter("economicSurchargeId", CurrentRow.Surcharge.EconomicSurchargeId));
                    AddSurchargeModel.OkCallback = () =>
                    {
                        _notifyManager.ShowNotify("Данные о доплатах успешно отредактированы.");
                        //TODO replace by current row update
                        RefreshAccount();
                        IsAddSurchargeOpen = false;
                    };
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При редактировании данных о доплатах ID {0} произошла ошибка.".F(CurrentRow.Surcharge.EconomicSurchargeId), typeof(EconomicSurchargeViewModel));
            }
        }

    }
}