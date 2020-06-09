using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace Medical.AppLayer.Economic.ViewModels
{
    public class EconomicRefuseViewModel : ViewModelBase, IContextCommandContainer, IHash, IDataErrorInfo
    {
        private readonly IMessageService _messageService;
        private readonly IMedicineRepository _repository;
        private readonly INotifyManager _notifyManager;

        private readonly DataErrorInfoSupport _dataErrorInfoSupport;

        private PLinqEconomicRefuseList _accountListSource;
        private EconomicRefuseCustomModel _currentRow;

        private AddRefuseViewModel _addRefuseModel;
        private bool _isAddRefuseOpen;

        #region IContextCommandContainer
        public ObservableCollection<IContextCommand> ContextCommands { get; set; }
        public string PageName { get; set; }
        #endregion

        #region IHash
        public string Hash
        {
            get { return typeof(EconomicRefuseViewModel).FullName; }
        }
        #endregion

        public EconomicRefuseViewModel(PLinqEconomicRefuseList list, IMessageService messageService, IMedicineRepository repository, INotifyManager notifyManager)
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
            PageName = "Отказы";
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
        

        public EconomicRefuseCustomModel CurrentRow
        {
            get { return _currentRow; }
            set
            {
                _currentRow = value;
                RaisePropertyChanged("CurrentRow");
            }
        }

        public PLinqEconomicRefuseList AccountListSource
        {
            get { return _accountListSource; }
            set
            {
                if (_accountListSource == value) return;
                _accountListSource = value;
                RaisePropertyChanged("AccountListSource");
            }
        }

        public AddRefuseViewModel AddRefuseModel
        {
            get { return _addRefuseModel; }
            set { _addRefuseModel = value; RaisePropertyChanged(() => AddRefuseModel); }
        }

        public bool IsAddRefuseOpen
        {
            get { return _isAddRefuseOpen; }
            set
            {
                _isAddRefuseOpen = value;
                RaisePropertyChanged(() => IsAddRefuseOpen);
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
                    AccountListSource = scope.Resolve<PLinqEconomicRefuseList>(
                        new NamedParameter("predicate", AccountListSource.Predicate));
                }
                else
                {
                    AccountListSource = scope.Resolve<PLinqEconomicRefuseList>();
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
                if (!_messageService.AskQuestion("Вы действительно хотите удалить данные об отказах?", "Внимание!"))
                {
                    return;
                }

                var deleteResult = _repository.DeleteEconomicRefuse(CurrentRow.Refuse.EconomicRefuseId);
                if (deleteResult.Success)
                {
                    _notifyManager.ShowNotify("Данные об оплате успешно удалены.");
                    RefreshAccount();
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При удалении данных об отказах ID {0} произошла ошибка.".F(CurrentRow.Refuse.EconomicRefuseId), typeof(EconomicRefuseViewModel));
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
                IsAddRefuseOpen = true;
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    AddRefuseModel = scope.Resolve<AddRefuseViewModel>(
                        new NamedParameter("accountId", CurrentRow.Refuse.AccountId),
                        new NamedParameter("version", null),   
                        new NamedParameter("economicRefuseId", CurrentRow.Refuse.EconomicRefuseId));
                    AddRefuseModel.OkCallback = () =>
                    {
                        _notifyManager.ShowNotify("Данные об отказах успешно отредактированы.");
                        //TODO replace by current row update
                        RefreshAccount();
                        IsAddRefuseOpen = false;
                    };
                }
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При редактировании данных об отказах ID {0} произошла ошибка.".F(CurrentRow.Refuse.EconomicRefuseId), typeof(EconomicRefuseViewModel));
            }
        }
    }
}