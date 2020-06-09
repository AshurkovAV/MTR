using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Core.Infrastructure;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Models;
using Medical.CoreLayer.Service;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Migration.ViewModels
{
    
    public class MigrationsViewModel : ViewModelBase
    {
        private readonly IMigrationService _migrationService;
        private readonly IMessageService _messageService;
        public ObservableCollection<MigrationModel> Migrations { get; set; }

        private MigrationModel _selectedMigration;
        private RelayCommand _applySelectedMigrationCommand;
        private RelayCommand _applyLastMigrationCommand;

        public MigrationModel SelectedMigration
        {
            get { return _selectedMigration; }
            set { _selectedMigration = value; RaisePropertyChanged(() => SelectedMigration); RaisePropertyChanged(() => CanApplySelected); }
        }

        public bool CanApplySelected
        {
            get { return CanApplySelectedMigration(); }
        }

        public bool CanApplyLast
        {
            get { return CanApplyLastMigration(); }
        }

        public ICommand ApplySelectedMigrationCommand
        {
            get { return _applySelectedMigrationCommand ?? (_applySelectedMigrationCommand = new RelayCommand(ApplySelectedMigration, CanApplySelectedMigration)); }
        }

        private bool CanApplySelectedMigration()
        {
            return _selectedMigration != null;
        }

        private void ApplySelectedMigration()
        {
            if (_messageService.AskQuestionFormatted("Применить миграцию {0}?", "Внимание", _selectedMigration.Name))
            {
                _migrationService.Update(_selectedMigration.Name);
                if (_migrationService is IError)
                {
                    var operation = _migrationService as IError;
                    if (operation.Success)
                    {
                        _messageService.ShowMessage("Миграция успешно применена", "Сообщение");
                    }
                    else
                    {
                        _messageService.ShowException(operation.LastError, "При применении миграции произошло исключение", typeof(MigrationsViewModel));
                    }
                }
                
                LoadMigrations();
            }
        }

        public ICommand ApplyLastMigrationCommand
        {
            get { return _applyLastMigrationCommand ?? (_applyLastMigrationCommand = new RelayCommand(ApplyLastMigration, CanApplyLastMigration)); }
        }

        private bool CanApplyLastMigration()
        {
            return Migrations.Any(p => !p.Applied);
        }

        private void ApplyLastMigration()
        {
            if (_messageService.AskQuestion("Применить все доступные миграции?", "Внимание"))
            {
                var lastMigration = Migrations.Where(p => !p.Applied).OrderBy(p => p.Date).LastOrDefault();
                if (lastMigration == null)
                {
                    _messageService.ShowWarning("Не найдена миграция");
                    return;
                }
                _migrationService.Update(lastMigration.Name);
                if (_migrationService is IError)
                {
                    var operation = _migrationService as IError;
                    if (operation.Success)
                    {
                        _messageService.ShowMessage("Миграции успешно применены", "Сообщение");
                    }
                    else
                    {
                        _messageService.ShowException(operation.LastError, "При применении миграции произошло исключение", typeof(MigrationsViewModel));
                    }
                }
                LoadMigrations();
            }
        }

        public MigrationsViewModel()
        {
            
        }

        public MigrationsViewModel(IMigrationService migrationService, IMessageService messageService)
        {
            _messageService = messageService;
            _migrationService = migrationService;
            LoadMigrations();
        }

        private void LoadMigrations()
        {
            Migrations = new ObservableCollection<MigrationModel>(_migrationService.GetLocalMigrations().Select(p => new MigrationModel(p)));
            var pending = _migrationService.GetPendingMigrations().ToList();
            foreach (var migration in Migrations.Where(migration => !pending.Contains(migration.RawName)))
            {
                migration.Applied = true;
            }
            RaisePropertyChanged(() => Migrations);
            RaisePropertyChanged(() => CanApplyLast);
            RaisePropertyChanged(() => CanApplySelected);
        }
    }
}
