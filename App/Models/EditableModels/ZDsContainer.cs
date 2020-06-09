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
    public class ZDsContainer : ViewModelBase, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;
        private readonly IZOperatorService _operatorService;
        private readonly IMedicineRepository _repository;

        private readonly int _medicalEventId;
        private readonly OperatorMode _mode;

        private ZDsModel _selectedDs;
        private Dictionary<OperatorMode, Action> _modeHandlers;
        private ObservableCollection<ZDsModel> _ds;
        public ZDsModel DsModel { get; set; }

        public ObservableCollection<ZDsModel> Ds => _ds ?? (_ds = new ObservableCollection<ZDsModel>());

        public ZDsModel SelectedDs
        {
            get { return _selectedDs; }
            set { _selectedDs = value; RaisePropertyChanged(()=> SelectedDs); }
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

        public ZDsContainer(IZOperatorService operatorService,
            IMedicineRepository repository,
            int medicalEventId)
        {
            _medicalEventId = medicalEventId;
            _operatorService = operatorService;
            _repository = repository;
            InitMode();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        public void InitMode()
        {
            Ds.Clear();
            InitLocal();
        }

        public bool IsDirty
        {
            get { return Ds.Any(p => p.IsDirty); }
        }

        public OperationResult Save()
        {
            var result = new OperationResult();
            //MedicalSlkoef.Where(p => p.IsDirty).ForEachAction(p =>
            //{
            //    p.Slkoef.NumberDifficultyTreatment = p.NumberDifficultyTreatment;
            //    p.Slkoef.ValueDifficultyTreatment = p.ValueDifficultyTreatment;
            //    var updatedResult = _repository.InsertOrUpdateSlKOef(p.Update());
            //    if (updatedResult.HasError)
            //    {
            //        result.AddError(updatedResult.LastError);
            //    }
            //});
            
            return result;

        }

        private void InitLocal()
        {

            var result = new List<ZDsModel>();

            var diagBlokResult = _repository.GetDsByMedicalEventId(_medicalEventId);
            
            if (diagBlokResult.Success && diagBlokResult.Data.Any())
            {
                diagBlokResult.Data.ForEachAction(p =>
                {
                    DsModel = Map.ObjectToObject<ZDsModel>(p, p);
                    result.Add(DsModel);
                });
            }

            Ds.AddRange(result);
        }

        public void Delete()
        {
            if (SelectedDs.ZFactDsId > 0)
            {
                //var deleteSlkoef = _repository.DeleteSlkoef(SelectedDiagBlok.ZDiagBlokId);
                //if (deleteSlkoef.Success)
                //{
                //    ReloadSlKoef();
                //}
            }
        }

        public void ReloadDiagBlok()
        {
            InitMode();
        }
        public void AcceptChanges()
        {
            Ds.ForEachAction(p => p.AcceptChanges());
        }
    }
}
