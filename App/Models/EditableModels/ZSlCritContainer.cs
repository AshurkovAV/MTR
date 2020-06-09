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
using GalaSoft.MvvmLight;
using Medical.AppLayer.Services;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Models.EditableModels
{
    public class ZSlCritContainer : ViewModelBase, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;
        private readonly IZOperatorService _operatorService;
        private readonly IMedicineRepository _repository;

        private readonly int _ksgkpgId;
        private readonly OperatorMode _mode;

        private ZSlCritModel _selected;
        private Dictionary<OperatorMode, Action> _modeHandlers;
        private ObservableCollection<ZSlCritModel> _crit;
        public ZSlCritModel CritModel { get; set; }

        public ObservableCollection<ZSlCritModel> MedicalCrit => _crit ?? (_crit = new ObservableCollection<ZSlCritModel>());

        public ZSlCritModel SelectedCrit {
            get { return _selected; }
            set { _selected = value; RaisePropertyChanged(()=> SelectedCrit); }
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

        public ZSlCritContainer(IZOperatorService operatorService,
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
            MedicalCrit.Clear();
            InitLocal();
        }

        public bool IsDirty
        {
            get { return MedicalCrit.Any(p => p.IsDirty); }
        }

        public OperationResult Save()
        {
            var result = new OperationResult();
            MedicalCrit.Where(p => p.IsDirty).ForEachAction(p =>
            {
                p.Crit.IdDkk= p.IdDkk;
                p.Crit.Value = p.Value;
                p.Crit.ZksgKpgId = _ksgkpgId;
                var updatedResult = _repository.InsertOrUpdateCrit(p.Update());
                if (updatedResult.HasError)
                {
                    result.AddError(updatedResult.LastError);
                }
            });

            return result;
        }

        private void InitLocal()
        {
            var result = new List<ZSlCritModel>();

            var critResult = _repository.GetCritByksgKpgId(_ksgkpgId);

            if (critResult.Success && critResult.Data.Any())
            {
                critResult.Data.ForEachAction(p =>
                {
                    CritModel = Map.ObjectToObject<ZSlCritModel>(p, p);

                    CritModel.ZCritId = p.ZCritId;
                    CritModel.Value = p.Value;
                    CritModel.ZksgKpgId = p.ZksgKpgId;
                    result.Add(CritModel);
                });
            }

            MedicalCrit.AddRange(result);
            //var slk = _operatorService.LoadSlkoef(_ksgkpgId);
            //MedicalSlkoef.AddRange(slk);
        }

        public void Delete()
        {
            if (SelectedCrit.ZCritId > 0)
            {
                var deleteCrit = _repository.DeleteCrit(SelectedCrit.ZCritId);
                if (deleteCrit.Success)
                {
                    ReloadSlCrit();
                }
            }
        }

        public void ReloadSlCrit()
        {
            InitMode();
        }
        public void AcceptChanges()
        {
            MedicalCrit.ForEachAction(p => p.AcceptChanges());
        }
    }
}
