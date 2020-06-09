using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using BLToolkit.Mapping;
using Core;
using Core.Extensions;
using Core.Infrastructure;
using Core.Validation;
using DataModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.AppLayer.Services;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Models.EditableModels
{
    public class ZDirectionContainer : ViewModelBase, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;
        private readonly IMedicineRepository _repository;
        private readonly int _id;
        private Dictionary<int, ZDirectionModel> _cache;
        public List<int> MedicalDirectionIds { get; set; }

        public ZDirectionModel MedicalDirection { get; set; }

        private RelayCommand _nextMedicalDirectionCommand;
        private RelayCommand _prevMedicalDirectionCommand;
        private RelayCommand _firstMedicalDirectionCommand;
        private RelayCommand _lastMedicalDirectionCommand;

        private RelayCommand<object> _goToMedicalDirectionPositionKeyCommand;
        private RelayCommand _goToMedicalDirectionPositionCommand;

        private int _currentMedicalDirectionPosition;
        private int? _goToMedicalDirectionPosition;

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

        public string CurrentMedicalDirectionPositionText
        {
            get { return "{0} из {1}".F(_currentMedicalDirectionPosition + 1, MedicalDirectionIds.Count); }
        }

        public bool HasData
        {
            get { return MedicalDirectionIds.Count > 0; }
        }

        public int CurrentMedicalDirectionPosition
        {
            get { return _currentMedicalDirectionPosition; }
            set
            {
                _currentMedicalDirectionPosition = value;
                RaisePropertyChanged(() => CurrentMedicalDirectionPosition);
                RaisePropertyChanged(() => CurrentMedicalDirectionPositionText);
                if (MedicalDirectionIds.Count > 0)
                {
                    LoadMedicalDirectionData(MedicalDirectionIds[_currentMedicalDirectionPosition]);
                }
            }
        }

        public int? GoToMedicalDirectionPosition
        {
            get { return _goToMedicalDirectionPosition; }
            set
            {
                _goToMedicalDirectionPosition = value;
                RaisePropertyChanged(() => GoToMedicalDirectionPosition);
            }
        }

        public ZDirectionContainer(IMedicineRepository repository, int id)
        {
            _id = id;
            _repository = repository;
            Initialize();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        private void Initialize()
        {
            _cache = new Dictionary<int, ZDirectionModel>();
            var result = _repository.GetZDirectionIdsByMedicalEventId(_id);
            if (result.Success)
            {
                MedicalDirectionIds = result.Data;
                CurrentMedicalDirectionPosition = 0;
            }
        }

        private void LoadMedicalDirectionData(int medicalDirectionId)
        {
            if (_cache.ContainsKey(medicalDirectionId))
            {
                MedicalDirection = _cache[medicalDirectionId];
            }
            else
            {
                var medicalDirectionResult = _repository.GetZDirectionById(medicalDirectionId);
                if (medicalDirectionResult.Success)
                {
                    var medicalDirection = medicalDirectionResult.Data;
                    MedicalDirection = Map.ObjectToObject<ZDirectionModel>(medicalDirection, medicalDirection);
                    _cache[medicalDirectionId] = MedicalDirection;
                }
            }
            RaisePropertyChanged(() => MedicalDirection);
            RaisePropertyChanged(() => HasData);
        }

        #region Navigate commands
        public ICommand NextMedicalDirectionCommand
        {
            get { return _nextMedicalDirectionCommand ?? (_nextMedicalDirectionCommand = new RelayCommand(NextMedicalDirection, CanNextMedicalDirection)); }
        }

        private bool CanNextMedicalDirection()
        {
            return CurrentMedicalDirectionPosition < MedicalDirectionIds.Count - 1;
        }

        private void NextMedicalDirection()
        {
            CurrentMedicalDirectionPosition++;
        }

        public ICommand PrevMedicalDirectionCommand
        {
            get { return _prevMedicalDirectionCommand ?? (_prevMedicalDirectionCommand = new RelayCommand(PrevMedicalDirection, CanPrevMedicalDirection)); }
        }

        private bool CanPrevMedicalDirection()
        {
            return CurrentMedicalDirectionPosition > 0;
        }

        private void PrevMedicalDirection()
        {
            CurrentMedicalDirectionPosition--;
        }

        public ICommand FirstMedicalDirectionCommand
        {
            get { return _firstMedicalDirectionCommand ?? (_firstMedicalDirectionCommand = new RelayCommand(FirstMedicalDirection, CanPrevMedicalDirection)); }
        }

        private void FirstMedicalDirection()
        {
            CurrentMedicalDirectionPosition = 0;
        }

        public ICommand LastMedicalDirectionCommand
        {
            get { return _lastMedicalDirectionCommand ?? (_lastMedicalDirectionCommand = new RelayCommand(LastMedicalDirection, CanNextMedicalDirection)); }
        }

        private void LastMedicalDirection()
        {
            CurrentMedicalDirectionPosition = MedicalDirectionIds.Count - 1;
        }

        #endregion

        #region GoToMedicalDirectionPosition
        public ICommand GoToMedicalDirectionPositionCommand
        {
            get { return _goToMedicalDirectionPositionCommand ?? (_goToMedicalDirectionPositionCommand = new RelayCommand(GoToPosition, CanGoToPosition)); }
        }

        private void GoToPosition()
        {
            if (GoToMedicalDirectionPosition.HasValue)
            {
                CurrentMedicalDirectionPosition = GoToMedicalDirectionPosition.Value - 1;
                GoToMedicalDirectionPosition = null;
            }

        }

        private bool CanGoToPosition()
        {
            return GoToMedicalDirectionPosition.HasValue && GoToMedicalDirectionPosition > 0 && GoToMedicalDirectionPosition <= MedicalDirectionIds.Count;
        }

        public ICommand GoToMedicalDirectionPositionKeyCommand
        {
            get { return _goToMedicalDirectionPositionKeyCommand ?? (_goToMedicalDirectionPositionKeyCommand = new RelayCommand<object>(GoToPositionByEnter, CanGoToPositionByEnter)); }
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

        public bool IsDirty
        {
            get { return _cache.Any(p => p.Value.IsDirty); }
        }

        public void RejectChanges()
        {
            _cache.ForEachAction(p => p.Value.RejectChanges());
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
