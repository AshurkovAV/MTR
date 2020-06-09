using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BLToolkit.Mapping;
using Core;
using Core.Extensions;
using Core.Infrastructure;
using Core.Validation;
using DataModel;
using GalaSoft.MvvmLight;
using Medical.AppLayer.Services;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Models.EditableModels
{
    public class ZSlKoefContainer : ViewModelBase, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;
        private readonly IZOperatorService _operatorService;
        private readonly IMedicineRepository _repository;

        private readonly int _ksgkpgId;
        private readonly OperatorMode _mode;

        private ZSlkoefModel _selectedRefusal;
        private Dictionary<OperatorMode, Action> _modeHandlers;
        private ObservableCollection<ZSlkoefModel> _slkoefs;
        public ZSlkoefModel SlkoefModel { get; set; }

        public ObservableCollection<ZSlkoefModel> MedicalSlkoef => _slkoefs ?? (_slkoefs = new ObservableCollection<ZSlkoefModel>());

        public ZSlkoefModel SelectedSlkoef {
            get { return _selectedRefusal; }
            set { _selectedRefusal = value; RaisePropertyChanged(()=> SelectedSlkoef); }
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

        public ZSlKoefContainer(IZOperatorService operatorService,
            IMedicineRepository repository,
            int ksgkpgId)
        {
            _ksgkpgId = ksgkpgId;
            _operatorService = operatorService;
            _repository = repository;
            InitMode();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        public void InitMode()
        {
            MedicalSlkoef.Clear();
            InitLocal();
        }

        public bool IsDirty
        {
            get { return MedicalSlkoef.Any(p => p.IsDirty); }
        }

        public OperationResult Save()
        {
            var result = new OperationResult();
            MedicalSlkoef.Where(p => p.IsDirty).ForEachAction(p =>
            {
                p.Slkoef.NumberDifficultyTreatment = p.NumberDifficultyTreatment;
                p.Slkoef.ValueDifficultyTreatment = p.ValueDifficultyTreatment;
                var updatedResult = _repository.InsertOrUpdateSlKOef(p.Update());
                if (updatedResult.HasError)
                {
                    result.AddError(updatedResult.LastError);
                }
            });
            
            return result;

        }

        private void InitLocal()
        {

            var result = new List<ZSlkoefModel>();

            var slkoefResult = _repository.GetSlKoefByKsgKpgId(_ksgkpgId);
            
            if (slkoefResult.Success && slkoefResult.Data.Any())
            {
                slkoefResult.Data.ForEachAction(p =>
                {
                    SlkoefModel = Map.ObjectToObject<ZSlkoefModel>(p, p);

                    SlkoefModel.ZslKoefId = p.ZslKoefId;
                    SlkoefModel.NumberDifficultyTreatment = p.NumberDifficultyTreatment;
                    SlkoefModel.ValueDifficultyTreatment = p.ValueDifficultyTreatment;
                    SlkoefModel.ZksgKpgId = p.ZksgKpgId;
                    result.Add(SlkoefModel);
                });
            }

            MedicalSlkoef.AddRange(result);
            //var slk = _operatorService.LoadSlkoef(_ksgkpgId);
            //MedicalSlkoef.AddRange(slk);
        }

        public void Delete()
        {
            if (SelectedSlkoef.ZslKoefId > 0)
            {
                var deleteSlkoef = _repository.DeleteSlkoef(SelectedSlkoef.ZslKoefId);
                if (deleteSlkoef.Success)
                {
                    ReloadSlKoef();
                }
            }
        }

        public void ReloadSlKoef()
        {
            InitMode();
        }
        public void AcceptChanges()
        {
            MedicalSlkoef.ForEachAction(p => p.AcceptChanges());
        }
    }
}
