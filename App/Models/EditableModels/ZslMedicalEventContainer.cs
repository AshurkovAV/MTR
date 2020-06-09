using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using BLToolkit.Mapping;
using Core.Extensions;
using Core.Infrastructure;
using Core.Validation;
using DevExpress.Mvvm.Native;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Medical.DatabaseCore.Services.Database;


namespace Medical.AppLayer.Models.EditableModels
{
    public class ZslMedicalEventContainer : ViewModelBase, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;

        private readonly IMedicineRepository _repository;

        private readonly int _id;
        private readonly Action<int> _changePosition;

        private Dictionary<int, ZslMedicalEventModel> _cache;
        
        public List<int> MedicalEventIds { get; set; }

        public ZslMedicalEventModel ZslMedicalEvent { get; set; }

        private RelayCommand _nextMedicalEventCommand;
        private RelayCommand _prevMedicalEventCommand;
        private RelayCommand _firstMedicalEventCommand;
        private RelayCommand _lastMedicalEventCommand;

        private RelayCommand<object> _goToMedicalEventPositionKeyCommand;
        private RelayCommand _goToMedicalEventPositionCommand;

        private int _currentMedicalEventPosition;
        private int? _goToMedicalEventPosition;
        

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

        public string CurrentMedicalEventPositionText
        {
            get { return "{0} из {1}".F(_currentMedicalEventPosition + 1, MedicalEventIds.Count); }
        }

        public int CurrentMedicalEventPosition
        {
            get { return _currentMedicalEventPosition; }
            set
            {
                _currentMedicalEventPosition = value;
                RaisePropertyChanged(() => CurrentMedicalEventPosition);
                RaisePropertyChanged(() => CurrentMedicalEventPositionText);
                LoadMedicalEventData(MedicalEventIds[_currentMedicalEventPosition]);
            }
        }

        public int? GoToMedicalEventPosition
        {
            get { return _goToMedicalEventPosition; }
            set
            {
                _goToMedicalEventPosition = value;
                RaisePropertyChanged(() => GoToMedicalEventPosition);
            }
        }

        public ZslMedicalEventContainer(IMedicineRepository repository, int id, Action<int> changePosition)
        {
            _id = id;
            _changePosition = changePosition;
            _repository = repository;
            
            Initialize();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        private void Initialize()
        {
            _cache = new Dictionary<int, ZslMedicalEventModel>();
            var result = _repository.GetZslMedicalEventIdsByPatientId(_id);
            if (result.Success)
            {
                MedicalEventIds = result.Data;
                CurrentMedicalEventPosition = 0;
            }
        }

        private void LoadMedicalEventData(int medicalEventId, bool forced = false)
        {
            if (_cache.ContainsKey(medicalEventId) && !forced)
            {
                ZslMedicalEvent = _cache[medicalEventId];
                if (_changePosition.IsNotNull())
                {
                    _changePosition(medicalEventId);
                }
            }
            else
            {
                var medicalEventResult = _repository.GetZslMedicalEventById(medicalEventId);
                if (medicalEventResult.Success)
                {
                    var factMedicalEvent = medicalEventResult.Data;
                    ZslMedicalEvent = Map.ObjectToObject<ZslMedicalEventModel>(factMedicalEvent, factMedicalEvent);

                    if (_cache.ContainsKey(medicalEventId))
                    {
                        _cache.Remove(medicalEventId);
                    }

                    _cache.Add(medicalEventId, ZslMedicalEvent);
                    if (_changePosition.IsNotNull())
                    {
                        _changePosition(medicalEventId);
                    }
                }
            }
            RaisePropertyChanged(() => ZslMedicalEvent);
        }

        #region Navigate commands
        public ICommand NextMedicalEventCommand {
            get { return _nextMedicalEventCommand ?? (_nextMedicalEventCommand = new RelayCommand(NextMedicalEvent, CanNextMedicalEvent)); }
        }

        private bool CanNextMedicalEvent()
        {
            return CurrentMedicalEventPosition < MedicalEventIds.Count - 1;
        }

        private void NextMedicalEvent()
        {
            CurrentMedicalEventPosition++;
        }

        public ICommand PrevMedicalEventCommand
        {
            get { return _prevMedicalEventCommand ?? (_prevMedicalEventCommand = new RelayCommand(PrevMedicalEvent, CanPrevMedicalEvent)); }
        }

        private bool CanPrevMedicalEvent()
        {
            return CurrentMedicalEventPosition > 0;
        }

        private void PrevMedicalEvent()
        {
            CurrentMedicalEventPosition--;
        }

        public ICommand FirstMedicalEventCommand
        {
            get { return _firstMedicalEventCommand ?? (_firstMedicalEventCommand = new RelayCommand(FirstMedicalEvent, CanPrevMedicalEvent)); }
        }

        private void FirstMedicalEvent()
        {
            CurrentMedicalEventPosition = 0;
        }

        public ICommand LastMedicalEventCommand
        {
            get { return _lastMedicalEventCommand ?? (_lastMedicalEventCommand = new RelayCommand(LastMedicalEvent, CanNextMedicalEvent)); }
        }

        private void LastMedicalEvent()
        {
            CurrentMedicalEventPosition = MedicalEventIds.Count - 1;
        }

        #endregion

        #region GoToMedicalEventPosition
        public ICommand GoToMedicalEventPositionCommand
        {
            get { return _goToMedicalEventPositionCommand ?? (_goToMedicalEventPositionCommand = new RelayCommand(GoToPosition, CanGoToPosition)); }
        }

        private void GoToPosition()
        {
            if (GoToMedicalEventPosition.HasValue)
            {
                CurrentMedicalEventPosition = GoToMedicalEventPosition.Value - 1;
                GoToMedicalEventPosition = null;
            }

        }

        private bool CanGoToPosition()
        {
            return GoToMedicalEventPosition.HasValue && GoToMedicalEventPosition > 0 && GoToMedicalEventPosition <= MedicalEventIds.Count;
        }

        public ICommand GoToMedicalEventPositionKeyCommand
        {
            get { return _goToMedicalEventPositionKeyCommand ?? (_goToMedicalEventPositionKeyCommand = new RelayCommand<object>(GoToPositionByEnter, CanGoToPositionByEnter)); }
        }

        public bool IsDirty {
            get { return _cache.Any(p => p.Value.IsDirty); }
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

        public void RejectChanges()
        {
            _cache.ForEachAction(p => p.Value.RejectChanges());
        }

        public OperationResult Save()
        {
            var result = new OperationResult();
            _cache.Where(p=>p.Value.IsDirty).Select(p=>p.Value).ForEachAction(p =>
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
            _cache.ForEach(p => p.Value.AcceptChanges());
        }

        public void ReloadCurrentMedicalEvent()
        {
            LoadMedicalEventData(MedicalEventIds[_currentMedicalEventPosition], true);
        }
    }
}
