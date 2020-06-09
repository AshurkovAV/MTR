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
    public class ZDiagBlokContainer : ViewModelBase, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;
        private readonly IZOperatorService _operatorService;
        private readonly IMedicineRepository _repository;

        private readonly int _medicalEventOnkId;
        private readonly OperatorMode _mode;

        private ZDiagBlokModel _selectedDiagBlok;
        private Dictionary<OperatorMode, Action> _modeHandlers;
        private ObservableCollection<ZDiagBlokModel> _diagBloks;
        public ZDiagBlokModel DiagBlokModel { get; set; }

        public ObservableCollection<ZDiagBlokModel> DiagBlok => _diagBloks ?? (_diagBloks = new ObservableCollection<ZDiagBlokModel>());

        public ZDiagBlokModel SelectedDiagBlok
        {
            get { return _selectedDiagBlok; }
            set { _selectedDiagBlok = value; RaisePropertyChanged(()=> SelectedDiagBlok); }
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

        public ZDiagBlokContainer(IZOperatorService operatorService,
            IMedicineRepository repository,
            int medicalEventOnkId)
        {
            _medicalEventOnkId = medicalEventOnkId;
            _operatorService = operatorService;
            _repository = repository;
            InitMode();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        public void InitMode()
        {
            DiagBlok.Clear();
            InitLocal();
        }

        public bool IsDirty
        {
            get { return DiagBlok.Any(p => p.IsDirty); }
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

            var result = new List<ZDiagBlokModel>();

            var diagBlokResult = _repository.GetDiafBlokByMedicalEventOnkId(_medicalEventOnkId);
            
            if (diagBlokResult.Success && diagBlokResult.Data.Any())
            {
                diagBlokResult.Data.ForEachAction(p =>
                {
                    DiagBlokModel = Map.ObjectToObject<ZDiagBlokModel>(p, p);
                    result.Add(DiagBlokModel);
                });
            }

            DiagBlok.AddRange(result);
        }

        public void Delete()
        {
            if (SelectedDiagBlok.ZDiagBlokId > 0)
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
            DiagBlok.ForEachAction(p => p.AcceptChanges());
        }
    }
}
