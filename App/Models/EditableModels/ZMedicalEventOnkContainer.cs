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
    public class ZMedicalEventOnkContainer : ViewModelBase, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;
        private readonly IMedicineRepository _repository;
        private readonly int _id;
        private Dictionary<int, ZMedicalEventOnkModel> _cache;
        private readonly Action<int> _changePosition;
        public List<int> MedicalEventOnkIds { get; set; }

        public ZMedicalEventOnkModel MedicalEventOnk { get; set; }

        private RelayCommand<object> _goToMedicalEventOnkPositionKeyCommand;
        private RelayCommand _goToMedicalEventOnkPositionCommand;

        private int _currentMedicalEventOnkPosition;
        private int? _goToMedicalEventOnkPosition;

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

        public string CurrentMedicalEventOnkPositionText
        {
            get { return "{0} из {1}".F(_currentMedicalEventOnkPosition + 1, MedicalEventOnkIds.Count); }
        }

        public bool HasData
        {
            get { return MedicalEventOnkIds.Count > 0; }
        }

        public int CurrentMedicalEventOnkPosition
        {
            get { return _currentMedicalEventOnkPosition; }
            set
            {
                _currentMedicalEventOnkPosition = value;
                RaisePropertyChanged(() => CurrentMedicalEventOnkPosition);
                RaisePropertyChanged(() => CurrentMedicalEventOnkPositionText);
                if (MedicalEventOnkIds.Count > 0)
                {
                    LoadMedicalEventOnkData(MedicalEventOnkIds[_currentMedicalEventOnkPosition]);
                }
            }
        }

        public int? GoToMedicalEventOnkPosition
        {
            get { return _goToMedicalEventOnkPosition; }
            set
            {
                _goToMedicalEventOnkPosition = value;
                RaisePropertyChanged(() => GoToMedicalEventOnkPosition);
            }
        }

        public ZMedicalEventOnkContainer(IMedicineRepository repository, int id, Action<int> changePosition)
        {
            _id = id;
            _repository = repository;
            _changePosition = changePosition;
            Initialize();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        private void LoadMedicalEventOnkData(int medicalEventId, bool forced = false)
        {
            if (_cache.ContainsKey(medicalEventId) && !forced)
            {
                MedicalEventOnk = _cache[medicalEventId];
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
                    MedicalEventOnk = Map.ObjectToObject<ZMedicalEventOnkModel>(factMedicalEvent, factMedicalEvent);

                    if (_cache.ContainsKey(medicalEventId))
                    {
                        _cache.Remove(medicalEventId);
                    }

                    _cache.Add(medicalEventId, MedicalEventOnk);
                    if (_changePosition.IsNotNull())
                    {
                        _changePosition(medicalEventId);
                    }
                }
            }
            RaisePropertyChanged(() => MedicalEventOnk);
        }

        private void Initialize()
        {
            _cache = new Dictionary<int, ZMedicalEventOnkModel>();
            var result = _repository.GetZMedicalEventOnkIdsByZslMeventId(_id); 
            if (result.Success)
            {
                MedicalEventOnkIds = result.Data;
                CurrentMedicalEventOnkPosition = 0;
            }
        }

        private void LoadMedicalEventOnkData(int medicalEventId)
        {
            if (_cache.ContainsKey(medicalEventId))
            {
                MedicalEventOnk = _cache[medicalEventId];
                if (_changePosition.IsNotNull())
                {
                    _changePosition(medicalEventId);
                }
            }
            else
            {
                var medicalEventOnkResult = _repository.GetZMedicalEventOnkById(medicalEventId); 
                if (medicalEventOnkResult.Success)
                {
                    var medicalEventOnk = medicalEventOnkResult.Data;
                    MedicalEventOnk = Map.ObjectToObject<ZMedicalEventOnkModel>(medicalEventOnk, medicalEventOnk);
                    _cache[medicalEventId] = MedicalEventOnk;
                    if (_changePosition.IsNotNull())
                    {
                        _changePosition(medicalEventId);
                    }
                }
            }
            RaisePropertyChanged(() => MedicalEventOnk);
            RaisePropertyChanged(() => HasData);
        }

        #region Navigate commands
        //public ICommand NextKsgKpgCommand
        //{
        //    get { return _nextMedicalEventOnkCommand ?? (_nextKsgKpgCommand = new RelayCommand(NextKsgKpg, CanNextKsgKpg)); }
        //}

        //private bool CanNextKsgKpg()
        //{
        //    return CurrentKsgKpgPosition < KsgKpgIds.Count - 1;
        //}

        //private void NextKsgKpg()
        //{
        //    CurrentKsgKpgPosition++;
        //}

        //public ICommand PrevKsgKpgCommand
        //{
        //    get { return _prevKsgKpgCommand ?? (_prevKsgKpgCommand = new RelayCommand(PrevKsgKpg, CanPrevKsgKpg)); }
        //}

        //private bool CanPrevKsgKpg()
        //{
        //    return CurrentKsgKpgPosition > 0;
        //}

        //private void PrevKsgKpg()
        //{
        //    CurrentKsgKpgPosition--;
        //}

        //public ICommand FirstKsgKpgCommand
        //{
        //    get { return _firstKsgKpgCommand ?? (_firstKsgKpgCommand = new RelayCommand(FirstKsgKpg, CanPrevKsgKpg)); }
        //}

        //private void FirstKsgKpg()
        //{
        //    CurrentKsgKpgPosition = 0;
        //}

        //public ICommand LastMedicalServiceCommand
        //{
        //    get { return _lastKsgKpgCommand ?? (_lastKsgKpgCommand = new RelayCommand(LastMedicalService, CanNextKsgKpg)); }
        //}

        //private void LastMedicalService()
        //{
        //    CurrentKsgKpgPosition = KsgKpgIds.Count - 1;
        //}

        #endregion

        #region GoToKsgKpgPosition
        //public ICommand GoToKsgKPgPositionCommand
        //{
        //    get { return _goToKsgKpgPositionCommand ?? (_goToKsgKpgPositionCommand = new RelayCommand(GoToPosition, CanGoToPosition)); }
        //}

        //private void GoToPosition()
        //{
        //    if (GoToKsgKpgPosition.HasValue)
        //    {
        //        CurrentKsgKpgPosition = GoToKsgKpgPosition.Value - 1;
        //        GoToKsgKpgPosition = null;
        //    }

        //}

        //private bool CanGoToPosition()
        //{
        //    return GoToKsgKpgPosition.HasValue && GoToKsgKpgPosition > 0 && GoToKsgKpgPosition <= KsgKpgIds.Count;
        //}

        //public ICommand GoToKsgKpgPositionKeyCommand
        //{
        //    get { return _goToKsgKpgPositionKeyCommand ?? (_goToKsgKpgPositionKeyCommand = new RelayCommand<object>(GoToPositionByEnter, CanGoToPositionByEnter)); }
        //}

        //private bool CanGoToPositionByEnter(object obj)
        //{
        //    if (obj is KeyEventArgs)
        //    {
        //        var key = obj as KeyEventArgs;
        //        if (key.IsDown && key.Key == Key.Enter)
        //        {
        //            return CanGoToPosition();
        //        }
        //    }

        //    return false;
        //}

        //private void GoToPositionByEnter(object obj)
        //{
        //    if (obj is KeyEventArgs)
        //    {
        //        var key = obj as KeyEventArgs;
        //        if (key.IsDown && key.Key == Key.Enter)
        //        {
        //            GoToPosition();
        //        }
        //    }
        //}
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
