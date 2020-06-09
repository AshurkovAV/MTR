using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using Autofac;
using Core.Infrastructure;
using Core.Linq;
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
    public class EconomicJournalViewModel : ViewModelBase, IContextCommandContainer, IHash, IDataErrorInfo
    {
        private IMessageService _messageService;
        private IMedicineRepository _repository;
        private INotifyManager _notifyManager;

        private readonly DataErrorInfoSupport _dataErrorInfoSupport;

        private PLinqEconomicJournalList _accountListSource;
        private List<EconomicAccountCustomModel> _dataListSource;
        private EconomicAccountCustomModel _currentRow;

        private RelayCommand _resetStatusCommand;
        private RelayCommand _resetTerritoryCommand;
        private RelayCommand _resetDateBeginCommand;
        private RelayCommand _resetDateEndCommand;
        private RelayCommand _resetDirectionCommand;

        private int? _selectedStatus;
        private string _selectedTerritory;
        private DateTime? _selectedDateBegin;
        private DateTime? _selectedDateEnd;
        private int? _selectedDirection;
        

        #region IContextCommandContainer
        public ObservableCollection<IContextCommand> ContextCommands { get; set; }
        public string PageName { get; set; }
        #endregion

        #region IHash
        public string Hash
        {
            get { return typeof(EconomicJournalViewModel).FullName; }
        }
        #endregion

        public EconomicJournalViewModel(PLinqEconomicJournalList list, IMessageService messageService, IMedicineRepository repository, INotifyManager notifyManager)
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
            PageName = "Журнал платежей";
            ContextCommands = new ObservableCollection<IContextCommand>
            {
                new ContextCommand
                {
                    Id = "viewAccountFilter",
                    Caption = "Печать",
                    Command = new RelayCommand(PrintJournal, CanPrintJournal),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/document-print.png",
                    SmallGlyph = "../Resources/Icons/document-print.png",
                },
                new ContextCommand
                {
                    Id = "viewAccountFilter",
                    Caption = "Обновить",
                    Command = new RelayCommand(RefreshAccount),
                    Page = "Действия",
                    LargeGlyph = "../Resources/Icons/view-refresh.png",
                    SmallGlyph = "../Resources/Icons/view-refresh.png",
                }
            };
            UpdateFilter();
        }

        private bool CanPrintJournal()
        {
            return AccountListSource != null && !AccountListSource.IsEmpty();
        }

        private void PrintJournal()
        {
            
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

        public string SelectedTerritory
        {
            get { return _selectedTerritory; }
            set
            {
                _selectedTerritory = value;
                RaisePropertyChanged("SelectedTerritory");
                UpdateFilter();
            }
        }

        public int? SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                //globalAccountStatus begin from 1
                _selectedStatus = value + 1;
                RaisePropertyChanged("SelectedStatus");
                UpdateFilter();
            }
        }

        public DateTime? SelectedDateBegin
        {
            get { return _selectedDateBegin; }
            set
            {
                _selectedDateBegin = value;
                RaisePropertyChanged("SelectedDateBegin");
                UpdateFilter();
            }
        }

        public DateTime? SelectedDateEnd
        {
            get { return _selectedDateEnd; }
            set
            {
                _selectedDateEnd = value;
                RaisePropertyChanged("SelectedDateEnd");
                UpdateFilter();
            }
        }

        public int? SelectedDirection
        {
            get { return _selectedDirection; }
            set
            {
                _selectedDirection = value;
                RaisePropertyChanged("SelectedDirection");
                UpdateFilter();
            }
        }

        public PLinqEconomicJournalList AccountListSource
        {
            get { return _accountListSource; }
            set
            {
                _accountListSource = value;
                RaisePropertyChanged("AccountListSource");
            }
        }

        public ICommand ResetTerritoryCommand
        {
            get
            {
                return _resetTerritoryCommand ??
                       (_resetTerritoryCommand = new RelayCommand(ResetTerritory, CanResetTerritory));
            }
        }

        public ICommand ResetStatusCommand
        {
            get
            {
                return _resetStatusCommand ??
                       (_resetStatusCommand = new RelayCommand(ResetStatus, CanResetStatus));
            }
        }

        public ICommand ResetDateBeginCommand
        {
            get
            {
                return _resetDateBeginCommand ??
                       (_resetDateBeginCommand = new RelayCommand(ResetDateBegin, CanResetDateBegin));
            }
        }

        public ICommand ResetDateEndCommand
        {
            get
            {
                return _resetDateEndCommand ??
                       (_resetDateEndCommand = new RelayCommand(ResetDateEnd, CanResetDateEnd));
            }
        }

        public ICommand ResetDirectionCommand
        {
            get
            {
                return _resetDirectionCommand ??
                       (_resetDirectionCommand = new RelayCommand(ResetDirection, CanResetDirection));
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

        private bool CanResetStatus()
        {
            return _selectedStatus.HasValue;
        }

        private void ResetStatus()
        {
            SelectedStatus = null;
        }


        private bool CanResetTerritory()
        {
            return !string.IsNullOrWhiteSpace(_selectedTerritory);
        }

        private void ResetTerritory()
        {
            SelectedTerritory = null;
        }

        private bool CanResetDateBegin()
        {
            return _selectedDateBegin.HasValue;
        }

        private void ResetDateBegin()
        {
            SelectedDateBegin = null;
        }

        private bool CanResetDateEnd()
        {
            return _selectedDateEnd.HasValue;
        }

        private void ResetDateEnd()
        {
            SelectedDateEnd = null;
        }

        private bool CanResetDirection()
        {
            return _selectedDirection.HasValue;
        }

        private void ResetDirection()
        {
            SelectedDirection = null;
        }

        private void UpdateFilter()
        {
            Expression<Func<FactEconomicAccount, bool>> predicate = PredicateBuilder.True<FactEconomicAccount>();

            if (SelectedStatus.HasValue && SelectedStatus != -1)
            {
                predicate = predicate.And(p => p.FACTTERRACCACCID.Status == _selectedStatus);

            }
            if (!string.IsNullOrWhiteSpace(_selectedTerritory))
            {
                predicate = predicate.And(p => p.FACTTERRACCACCID.Source == _selectedTerritory || p.FACTTERRACCACCID.Destination == _selectedTerritory);
            }

            if ( SelectedDateBegin.HasValue)
            {
                predicate = predicate.And(p => p.FACTTERRACCACCID.Date.Value >= SelectedDateBegin.Value);
            }

            if (SelectedDateEnd.HasValue)
            {
                predicate = predicate.And(p => p.FACTTERRACCACCID.Date.Value <= SelectedDateEnd.Value);
            }

            if (SelectedDirection.HasValue && SelectedDirection != -1)
            {
                predicate = predicate.And(p => p.FACTTERRACCACCID.Direction == _selectedDirection);
            }

            
            using (var scope = Di.I.BeginLifetimeScope())
            {
                AccountListSource = scope.Resolve<PLinqEconomicJournalList>(new NamedParameter("predicate", predicate));
            }
        }

        private void RefreshAccount()
        {
            UpdateFilter();
        } 
    }
}