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
    public class ZContraindicationsContainer : ViewModelBase, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;
        private readonly IZOperatorService _operatorService;
        private readonly IMedicineRepository _repository;

        private readonly int _medicalEventOnkId;
        private readonly OperatorMode _mode;

        private ZContraindicationsModel _selectedСontraindications;
        private Dictionary<OperatorMode, Action> _modeHandlers;
        private ObservableCollection<ZContraindicationsModel> _сontraindications;
        public ZContraindicationsModel СontraindicationsModel { get; set; }

        public ObservableCollection<ZContraindicationsModel> Сontraindications => _сontraindications ?? (_сontraindications = new ObservableCollection<ZContraindicationsModel>());

        public ZContraindicationsModel SelectedСontraindications
        {
            get { return _selectedСontraindications; }
            set { _selectedСontraindications = value; RaisePropertyChanged(()=> SelectedСontraindications); }
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

        public ZContraindicationsContainer(IZOperatorService operatorService,
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
            Сontraindications.Clear();
            InitLocal();
        }

        public bool IsDirty
        {
            get { return Сontraindications.Any(p => p.IsDirty); }
        }

        private void InitLocal()
        {

            var result = new List<ZContraindicationsModel>();

            var contraindicationsResult = _repository.GetContraindicationsByMedicalEventOnkId(_medicalEventOnkId);
            
            if (contraindicationsResult.Success && contraindicationsResult.Data.Any())
            {
                contraindicationsResult.Data.ForEachAction(p =>
                {
                    СontraindicationsModel = Map.ObjectToObject<ZContraindicationsModel>(p, p);
                    result.Add(СontraindicationsModel);
                });
            }

            Сontraindications.AddRange(result);
        }
        

        public void ReloadDiagBlok()
        {
            InitMode();
        }
        public void AcceptChanges()
        {
            Сontraindications.ForEachAction(p => p.AcceptChanges());
        }
    }
}
