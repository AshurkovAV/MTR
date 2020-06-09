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
    public class ZAnticancerDrugContainer : ViewModelBase, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;
        private readonly IZOperatorService _operatorService;
        private readonly IMedicineRepository _repository;

        private readonly int _medicalServiceOnkId;
        private readonly OperatorMode _mode;

        private ZAnticancerDrugModel _selectedAnticancerDrug;
        private Dictionary<OperatorMode, Action> _modeHandlers;
        private ObservableCollection<ZAnticancerDrugModel> _anticancerDrug;
        public ZAnticancerDrugModel AnticancerDrugModel { get; set; }

        public ObservableCollection<ZAnticancerDrugModel> AnticancerDrug => _anticancerDrug ?? (_anticancerDrug = new ObservableCollection<ZAnticancerDrugModel>());

        public ZAnticancerDrugModel SelectedAnticancerDrug
        {
            get { return _selectedAnticancerDrug; }
            set { _selectedAnticancerDrug = value; RaisePropertyChanged(()=> SelectedAnticancerDrug); }
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

        public ZAnticancerDrugContainer(IZOperatorService operatorService,
            IMedicineRepository repository,
            int medicalServiceOnkId)
        {
            _medicalServiceOnkId = medicalServiceOnkId;
            _operatorService = operatorService;
            _repository = repository;
            InitMode();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        public void InitMode()
        {
            AnticancerDrug.Clear();
            InitLocal();
        }

        public bool IsDirty
        {
            get { return AnticancerDrug.Any(p => p.IsDirty); }
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

            var result = new List<ZAnticancerDrugModel>();

            var anticancerDrugResult = _repository.GetAnticancerDrugByMedicalServiceOnkId(_medicalServiceOnkId);
            
            if (anticancerDrugResult.Success && anticancerDrugResult.Data.Any())
            {
                anticancerDrugResult.Data.ForEachAction(p =>
                {
                    AnticancerDrugModel = Map.ObjectToObject<ZAnticancerDrugModel>(p, p);
                    result.Add(AnticancerDrugModel);
                });
            }

            AnticancerDrug.AddRange(result);
        }

        public void Delete()
        {
            if (SelectedAnticancerDrug.ZAnticancerDrugId > 0)
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
            AnticancerDrug.ForEachAction(p => p.AcceptChanges());
        }
    }
}
