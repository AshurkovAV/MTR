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
    public class ZKsgKpgContainer : ViewModelBase, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;
        private readonly IMedicineRepository _repository;
        private readonly int _id;
        private Dictionary<int, ZKsgKpgModel> _cache;
        private readonly Action<int> _changePosition;
        public List<int> KsgKpgIds { get; set; }

        public ZKsgKpgModel KsgKpg { get; set; }

        private RelayCommand _nextKsgKpgCommand;
        private RelayCommand _prevKsgKpgCommand;
        private RelayCommand _firstKsgKpgCommand;
        private RelayCommand _lastKsgKpgCommand;

        private RelayCommand<object> _goToKsgKpgPositionKeyCommand;
        private RelayCommand _goToKsgKpgPositionCommand;

        private int _currentKsgKpgPosition;
        private int? _goToKsgKpgPosition;

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

        public string CurrentKsgKpgPositionText
        {
            get { return "{0} из {1}".F(_currentKsgKpgPosition + 1, KsgKpgIds.Count); }
        }

        public bool HasData
        {
            get { return KsgKpgIds.Count > 0; }
        }

        public int CurrentKsgKpgPosition
        {
            get { return _currentKsgKpgPosition; }
            set
            {
                _currentKsgKpgPosition = value;
                RaisePropertyChanged(() => CurrentKsgKpgPosition);
                RaisePropertyChanged(() => CurrentKsgKpgPositionText);
                if (KsgKpgIds.Count > 0)
                {
                    LoadKsgKpgData(KsgKpgIds[_currentKsgKpgPosition]);
                }
            }
        }

        public int? GoToKsgKpgPosition
        {
            get { return _goToKsgKpgPosition; }
            set
            {
                _goToKsgKpgPosition = value;
                RaisePropertyChanged(() => GoToKsgKpgPosition);
            }
        }

        public ZKsgKpgContainer(IMedicineRepository repository, int id, Action<int> changePosition)
        {
            _id = id;
            _repository = repository;
            _changePosition = changePosition;
            Initialize();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        private void LoadMedicalEventData(int medicalEventId, bool forced = false)
        {
            if (_cache.ContainsKey(medicalEventId) && !forced)
            {
                KsgKpg = _cache[medicalEventId];
                if (_changePosition.IsNotNull())
                {
                    _changePosition(medicalEventId);
                }
            }
            else
            {
                var medicalEventResult = _repository.GetZKsgKpgByZMeventId(medicalEventId);
                if (medicalEventResult.Success)
                {
                    var factMedicalEvent = medicalEventResult.Data;
                    KsgKpg = Map.ObjectToObject<ZKsgKpgModel>(factMedicalEvent, factMedicalEvent);

                    if (_cache.ContainsKey(medicalEventId))
                    {
                        _cache.Remove(medicalEventId);
                    }

                    _cache.Add(medicalEventId, KsgKpg);
                    if (_changePosition.IsNotNull())
                    {
                        _changePosition(medicalEventId);
                    }
                }
            }
            RaisePropertyChanged(() => KsgKpg);
        }

        private void Initialize()
        {
            _cache = new Dictionary<int, ZKsgKpgModel>();
            var result = _repository.GetZKsgKpgIdsByZslMeventId(_id); 
            if (result.Success)
            {
                KsgKpgIds = result.Data;
                CurrentKsgKpgPosition = 0;
            }
        }

        private void LoadKsgKpgData(int medicalEventId)
        {
            if (_cache.ContainsKey(medicalEventId))
            {
                KsgKpg = _cache[medicalEventId];
                if (_changePosition.IsNotNull())
                {
                    _changePosition(medicalEventId);
                }
            }
            else
            {
                var ksgKpgResult = _repository.GetZKsgKpgById(medicalEventId); //GetZKsgKpgById
                if (ksgKpgResult.Success)
                {
                    var ksgkpg = ksgKpgResult.Data;
                    KsgKpg = Map.ObjectToObject<ZKsgKpgModel>(ksgkpg, ksgkpg);
                    _cache[medicalEventId] = KsgKpg;
                    if (_changePosition.IsNotNull())
                    {
                        _changePosition(medicalEventId);
                    }
                }
            }
            RaisePropertyChanged(() => KsgKpg);
            RaisePropertyChanged(() => HasData);
        }

        #region Navigate commands
        public ICommand NextKsgKpgCommand
        {
            get { return _nextKsgKpgCommand ?? (_nextKsgKpgCommand = new RelayCommand(NextKsgKpg, CanNextKsgKpg)); }
        }

        private bool CanNextKsgKpg()
        {
            return CurrentKsgKpgPosition < KsgKpgIds.Count - 1;
        }

        private void NextKsgKpg()
        {
            CurrentKsgKpgPosition++;
        }

        public ICommand PrevKsgKpgCommand
        {
            get { return _prevKsgKpgCommand ?? (_prevKsgKpgCommand = new RelayCommand(PrevKsgKpg, CanPrevKsgKpg)); }
        }

        private bool CanPrevKsgKpg()
        {
            return CurrentKsgKpgPosition > 0;
        }

        private void PrevKsgKpg()
        {
            CurrentKsgKpgPosition--;
        }

        public ICommand FirstKsgKpgCommand
        {
            get { return _firstKsgKpgCommand ?? (_firstKsgKpgCommand = new RelayCommand(FirstKsgKpg, CanPrevKsgKpg)); }
        }

        private void FirstKsgKpg()
        {
            CurrentKsgKpgPosition = 0;
        }

        public ICommand LastMedicalServiceCommand
        {
            get { return _lastKsgKpgCommand ?? (_lastKsgKpgCommand = new RelayCommand(LastMedicalService, CanNextKsgKpg)); }
        }

        private void LastMedicalService()
        {
            CurrentKsgKpgPosition = KsgKpgIds.Count - 1;
        }

        #endregion

        #region GoToKsgKpgPosition
        public ICommand GoToKsgKPgPositionCommand
        {
            get { return _goToKsgKpgPositionCommand ?? (_goToKsgKpgPositionCommand = new RelayCommand(GoToPosition, CanGoToPosition)); }
        }

        private void GoToPosition()
        {
            if (GoToKsgKpgPosition.HasValue)
            {
                CurrentKsgKpgPosition = GoToKsgKpgPosition.Value - 1;
                GoToKsgKpgPosition = null;
            }

        }

        private bool CanGoToPosition()
        {
            return GoToKsgKpgPosition.HasValue && GoToKsgKpgPosition > 0 && GoToKsgKpgPosition <= KsgKpgIds.Count;
        }

        public ICommand GoToKsgKpgPositionKeyCommand
        {
            get { return _goToKsgKpgPositionKeyCommand ?? (_goToKsgKpgPositionKeyCommand = new RelayCommand<object>(GoToPositionByEnter, CanGoToPositionByEnter)); }
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
