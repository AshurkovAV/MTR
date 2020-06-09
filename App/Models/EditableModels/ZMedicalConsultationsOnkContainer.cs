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
    public class ZMedicalConsultationsOnkContainer : ViewModelBase, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;
        private readonly IMedicineRepository _repository;
        
        private readonly int _id;
        private Dictionary<int, ZMedicalConsultationsOnkModel> _cache;
        public List<int> MedicalConsultationsOnkIds { get; set; }

        public ZMedicalConsultationsOnkModel MedicalConsultationsOnk { get; set; }

        private RelayCommand _nextMedicalConsultationsCommand;
        private RelayCommand _prevMedicalConsultationsCommand;
        private RelayCommand _firstMedicalConsultationsCommand;
        private RelayCommand _lastMedicalConsultationsCommand;

        private RelayCommand<object> _goToMedicalConsultationsPositionKeyCommand;
        private RelayCommand _goToMedicalConsultationsPositionCommand;

        private int _currentMedicalConsultationsPosition;
        private int? _goToMedicalConsultationsPosition;

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

        public string CurrentMedicalConsultationsPositionText
        {
            get { return "{0} из {1}".F(_currentMedicalConsultationsPosition + 1, MedicalConsultationsOnkIds.Count); }
        }

        public bool HasData
        {
            get { return MedicalConsultationsOnkIds.Count > 0; }
        }

        public int CurrentMedicalConsultationsPosition
        {
            get { return _currentMedicalConsultationsPosition; }
            set
            {
                _currentMedicalConsultationsPosition = value;
                RaisePropertyChanged(() => CurrentMedicalConsultationsPosition);
                RaisePropertyChanged(() => CurrentMedicalConsultationsPositionText);
                if (MedicalConsultationsOnkIds.Count > 0)
                {
                    LoadMedicalConsultationsData(MedicalConsultationsOnkIds[_currentMedicalConsultationsPosition]);
                }
            }
        }

        public ZMedicalConsultationsOnkContainer(IMedicineRepository repository, int id)
        {
            _id = id;
            _repository = repository;
            
            Initialize();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        private void Initialize()
        {
            _cache = new Dictionary<int, ZMedicalConsultationsOnkModel>();
            var result = _repository.GetZMedicalConsultationsOnkIdsByMedicalEventId(_id);
            if (result.Success)
            {
                MedicalConsultationsOnkIds = result.Data;
                CurrentMedicalConsultationsPosition = 0;
            }
        }

        private void LoadMedicalConsultationsData(int medicalConsultationsOnkId)
        {
            if (_cache.ContainsKey(medicalConsultationsOnkId))
            {
                MedicalConsultationsOnk = _cache[medicalConsultationsOnkId];
            }
            else
            {
                var medicalConsultationsResult = _repository.GetZMedicalConsultationsOnkById(medicalConsultationsOnkId);
                if (medicalConsultationsResult.Success)
                {
                    var medicalConsultations = medicalConsultationsResult.Data;
                    MedicalConsultationsOnk = Map.ObjectToObject<ZMedicalConsultationsOnkModel>(medicalConsultations, medicalConsultations);
                    _cache[medicalConsultationsOnkId] = MedicalConsultationsOnk;
                }
            }
            RaisePropertyChanged(() => MedicalConsultationsOnk);
            RaisePropertyChanged(() => HasData);
        }

        public int? GoToMedicalConsultationsPosition
        {
            get { return _goToMedicalConsultationsPosition; }
            set
            {
                _goToMedicalConsultationsPosition = value;
                RaisePropertyChanged(() => GoToMedicalConsultationsPosition);
            }
        }

        #region Navigate commands
        public ICommand NextMedicalConsultationsCommand
        {
            get { return _nextMedicalConsultationsCommand ?? (_nextMedicalConsultationsCommand = new RelayCommand(NextMedicalConsultations, CanNextMedicalConsultations)); }
        }

        private bool CanNextMedicalConsultations()
        {
            return CurrentMedicalConsultationsPosition < MedicalConsultationsOnkIds.Count - 1;
        }

        private void NextMedicalConsultations()
        {
            CurrentMedicalConsultationsPosition++;
        }

        public ICommand PrevMedicalConsultationsCommand
        {
            get { return _prevMedicalConsultationsCommand ?? (_prevMedicalConsultationsCommand = new RelayCommand(PrevMedicalConsultations, CanPrevMedicalConsultations)); }
        }

        private bool CanPrevMedicalConsultations()
        {
            return CurrentMedicalConsultationsPosition > 0;
        }

        private void PrevMedicalConsultations()
        {
            CurrentMedicalConsultationsPosition--;
        }

        public ICommand FirstMedicalConsultationsCommand
        {
            get { return _firstMedicalConsultationsCommand ?? (_firstMedicalConsultationsCommand = new RelayCommand(FirstMedicalConsultations, CanPrevMedicalConsultations)); }
        }

        private void FirstMedicalConsultations()
        {
            CurrentMedicalConsultationsPosition = 0;
        }

        public ICommand LastMedicalConsultationsCommand
        {
            get { return _lastMedicalConsultationsCommand ?? (_lastMedicalConsultationsCommand = new RelayCommand(LastMedicalConsultations, CanNextMedicalConsultations)); }
        }

        private void LastMedicalConsultations()
        {
            CurrentMedicalConsultationsPosition = MedicalConsultationsOnkIds.Count - 1;
        }

        #endregion

        #region GoToMedicalConsultationsPosition
        public ICommand GoToMedicalConsultationsPositionCommand
        {
            get { return _goToMedicalConsultationsPositionCommand ?? (_goToMedicalConsultationsPositionCommand = new RelayCommand(GoToPosition, CanGoToPosition)); }
        }

        private void GoToPosition()
        {
            if (GoToMedicalConsultationsPosition.HasValue)
            {
                CurrentMedicalConsultationsPosition = GoToMedicalConsultationsPosition.Value - 1;
                GoToMedicalConsultationsPosition = null;
            }

        }

        private bool CanGoToPosition()
        {
            return GoToMedicalConsultationsPosition.HasValue && GoToMedicalConsultationsPosition > 0 && GoToMedicalConsultationsPosition <= MedicalConsultationsOnkIds.Count;
        }

        public ICommand GoToMedicalConsultationsPositionKeyCommand
        {
            get { return _goToMedicalConsultationsPositionKeyCommand ?? (_goToMedicalConsultationsPositionKeyCommand = new RelayCommand<object>(GoToPositionByEnter, CanGoToPositionByEnter)); }
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
