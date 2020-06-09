using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using BLToolkit.Mapping;
using Core.Extensions;
using Core.Infrastructure;
using Core.Validation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Models.EditableModels
{
    public class MedicalServiceContainer : ViewModelBase, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;
        private readonly IMedicineRepository _repository;
        private readonly int _id;
        private Dictionary<int, MedicalServiceModel> _cache;
        public List<int> MedicalServiceIds { get; set; }

        public MedicalServiceModel MedicalService { get; set; }

        private RelayCommand _nextMedicalServiceCommand;
        private RelayCommand _prevMedicalServiceCommand;
        private RelayCommand _firstMedicalServiceCommand;
        private RelayCommand _lastMedicalServiceCommand;

        private RelayCommand<object> _goToMedicalServicePositionKeyCommand;
        private RelayCommand _goToMedicalServicePositionCommand;

        private int _currentMedicalServicePosition;
        private int? _goToMedicalServicePosition;

        private RelayCommand _selectMedicalOrganizationCodeCommand;

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

        public string CurrentMedicalServicePositionText
        {
            get { return "{0} из {1}".F(_currentMedicalServicePosition + 1, MedicalServiceIds.Count); }
        }

        public bool HasData
        {
            get { return MedicalServiceIds.Count > 0; }
        }

        public int CurrentMedicalServicePosition
        {
            get { return _currentMedicalServicePosition; }
            set
            {
                _currentMedicalServicePosition = value;
                RaisePropertyChanged(() => CurrentMedicalServicePosition);
                RaisePropertyChanged(() => CurrentMedicalServicePositionText);
                if (MedicalServiceIds.Count > 0)
                {
                    LoadMedicalServiceData(MedicalServiceIds[_currentMedicalServicePosition]);
                }
            }
        }

        public int? GoToMedicalServicePosition
        {
            get { return _goToMedicalServicePosition; }
            set
            {
                _goToMedicalServicePosition = value;
                RaisePropertyChanged(() => GoToMedicalServicePosition);
            }
        }

        public MedicalServiceContainer(IMedicineRepository repository, int id)
        {
            _id = id;
            _repository = repository;

            Initialize();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        private void Initialize()
        {
            _cache = new Dictionary<int, MedicalServiceModel>();
            var result = _repository.GetMedicalServiceIdsByMedicalEventId(_id);
            if (result.Success)
            {
                MedicalServiceIds = result.Data;
                CurrentMedicalServicePosition = 0;
            }
        }

        private void LoadMedicalServiceData(int medicalServiceId)
        {
            if (_cache.ContainsKey(medicalServiceId))
            {
                MedicalService = _cache[medicalServiceId];
            }
            else
            {
                var medicalServiceResult = _repository.GetMedicalServiceById(medicalServiceId);
                if (medicalServiceResult.Success)
                {
                    var medicalService = medicalServiceResult.Data;
                    MedicalService = Map.ObjectToObject<MedicalServiceModel>(medicalService, medicalService);
                    _cache[medicalServiceId] = MedicalService;
                }
            }
            RaisePropertyChanged(() => MedicalService);
            RaisePropertyChanged(() => HasData);
        }

        #region Navigate commands
        public ICommand NextMedicalServiceCommand {
            get { return _nextMedicalServiceCommand ?? (_nextMedicalServiceCommand = new RelayCommand(NextMedicalService, CanNextMedicalService)); }
        }

        private bool CanNextMedicalService()
        {
            return CurrentMedicalServicePosition < MedicalServiceIds.Count - 1;
        }

        private void NextMedicalService()
        {
            CurrentMedicalServicePosition++;
        }

        public ICommand PrevMedicalServiceCommand
        {
            get { return _prevMedicalServiceCommand ?? (_prevMedicalServiceCommand = new RelayCommand(PrevMedicalService, CanPrevMedicalService)); }
        }

        private bool CanPrevMedicalService()
        {
            return CurrentMedicalServicePosition > 0;
        }

        private void PrevMedicalService()
        {
            CurrentMedicalServicePosition--;
        }

        public ICommand FirstMedicalServiceCommand
        {
            get { return _firstMedicalServiceCommand ?? (_firstMedicalServiceCommand = new RelayCommand(FirstMedicalService, CanPrevMedicalService)); }
        }

        private void FirstMedicalService()
        {
            CurrentMedicalServicePosition = 0;
        }

        public ICommand LastMedicalServiceCommand
        {
            get { return _lastMedicalServiceCommand ?? (_lastMedicalServiceCommand = new RelayCommand(LastMedicalService, CanNextMedicalService)); }
        }

        private void LastMedicalService()
        {
            CurrentMedicalServicePosition = MedicalServiceIds.Count - 1;
        }

        #endregion

        #region GoToMedicalServicePosition
        public ICommand GoToMedicalServicePositionCommand
        {
            get { return _goToMedicalServicePositionCommand ?? (_goToMedicalServicePositionCommand = new RelayCommand(GoToPosition, CanGoToPosition)); }
        }

        private void GoToPosition()
        {
            if (GoToMedicalServicePosition.HasValue)
            {
                CurrentMedicalServicePosition = GoToMedicalServicePosition.Value - 1;
                GoToMedicalServicePosition = null;
            }

        }

        private bool CanGoToPosition()
        {
            return GoToMedicalServicePosition.HasValue && GoToMedicalServicePosition > 0 && GoToMedicalServicePosition <= MedicalServiceIds.Count;
        }

        public ICommand GoToMedicalServicePositionKeyCommand
        {
            get { return _goToMedicalServicePositionKeyCommand ?? (_goToMedicalServicePositionKeyCommand = new RelayCommand<object>(GoToPositionByEnter, CanGoToPositionByEnter)); }
        }

        private bool CanGoToPositionByEnter(object obj)
        {
            if (obj is KeyEventArgs)
            {
                var key = obj as KeyEventArgs;
                if (key.IsDown && key.Key == Key.Enter)
                {
                    return CanGoToPosition();
                }
            }

            return false;
        }

        private void GoToPositionByEnter(object obj)
        {
            if (obj is KeyEventArgs)
            {
                var key = obj as KeyEventArgs;
                if (key.IsDown && key.Key == Key.Enter)
                {
                    GoToPosition();
                }
            }
        }
        #endregion

        public bool IsDirty {
            get { return _cache.Any(p=>p.Value.IsDirty); }
        }

        public void RejectChanges()
        {
            _cache.ForEachAction(p=>p.Value.RejectChanges());
        }

        public OperationResult Save()
        {
            var result = new OperationResult();
            _cache.Where(p => p.Value.IsDirty).Select(p => p.Value).ForEachAction(p =>
            {
                var updatedResult = _repository.Update(p.Update());
                if (updatedResult.HasError)
                {
                    result.AddError(updatedResult.LastError);
                }
            });
            return result;

        }

        public void AcceptChanges()
        {
            _cache.ForEachAction(p => p.Value.AcceptChanges());
        }
    }
}
