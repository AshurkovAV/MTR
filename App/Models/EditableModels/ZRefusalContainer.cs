using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Core;
using Core.Extensions;
using Core.Validation;
using GalaSoft.MvvmLight;
using Medical.AppLayer.Services;

namespace Medical.AppLayer.Models.EditableModels
{
    public class ZRefusalContainer : ViewModelBase, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;
        private readonly IZOperatorService _operatorService;

        private readonly int _medicalEventId;
        private readonly OperatorMode _mode;

        private ZRefusalModel _selectedRefusal;
        private Dictionary<OperatorMode, Action> _modeHandlers;
        private ObservableCollection<ZRefusalModel> _refusals;

        public ObservableCollection<ZRefusalModel> MedicalRefusals => _refusals ?? (_refusals = new ObservableCollection<ZRefusalModel>());

        public ZRefusalModel SelectedRefusal {
            get { return _selectedRefusal; }
            set { _selectedRefusal = value; RaisePropertyChanged(()=>SelectedRefusal); }
        }

        public decimal? RefusalPrice {
            get { return MedicalRefusals.Where(p=>((p.IsAgree == null || p.IsAgree == true) && p.TypeSank == TypeSank.FactExternalRefuse) || (p.IsAgree == null && p.TypeSank == TypeSank.ZFactSank)).Sum(p => p.Amount); }
        }

        public decimal? ZslRefusalPrice
        {
            get { return MedicalRefusals.Sum(p => p.ZslAmount); }
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

        public ZRefusalContainer(IZOperatorService operatorService, 
            int medicalEventId, 
            int mode)
        {
            _medicalEventId = medicalEventId;
            _mode = (OperatorMode)mode;

            _operatorService = operatorService;

            Initialize();
            InitMode();

            _dataErrorInfoSupport = new DataErrorInfoSupport(this);
        }

        private void Initialize()
        {
            _modeHandlers = new Dictionary<OperatorMode, Action>
            {
                {OperatorMode.InterTerritorial, InitInterTerritorial},
                {OperatorMode.ZInterTerritorial, InitInterTerritorial},
                {OperatorMode.InterTerritorialError, InitInterTerritorial},
                {OperatorMode.ZInterTerritorialError, InitInterTerritorial},
                {OperatorMode.InterTerritorialSrzQuery, InitInterTerritorial},
                {OperatorMode.Local, InitLocal},
                {OperatorMode.Zlocal, InitLocal },
                {OperatorMode.LocalError, InitLocal},
                {OperatorMode.ZLocalError, InitLocal},
                {OperatorMode.LocalSrzQuery, InitLocal},
                {OperatorMode.Patient, InitPatient}
            };
        }

        public void InitMode()
        {
            try
            {
                MedicalRefusals.Clear();

                if (_modeHandlers.ContainsKey(_mode))
                {
                    _modeHandlers[_mode]();
                }

            }
            catch (Exception ex)
            {
            }
            
        }

        private void InitPatient()
        {
            //TODO
        }

        private void InitLocal()
        {
            MedicalRefusals.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEC, TypeSank.ZFactSank, (int)RefusalSource.Local));
            MedicalRefusals.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEE, TypeSank.ZFactSank, (int)RefusalSource.Local));
            MedicalRefusals.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.EQMA, TypeSank.ZFactSank, (int)RefusalSource.Local));

            //compatibility mode
            //MedicalRefusals.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEC));
            //MedicalRefusals.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEE));
            //MedicalRefusals.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.EQMA));
        }

        private void InitInterTerritorial()
        {
            var tmp = new ObservableCollection<ZRefusalModel>();
            tmp.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.External, TypeSank.FactExternalRefuse));

            //tmp.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEC, (int)RefusalSource.InterTerritorial));
            //tmp.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEE, (int)RefusalSource.InterTerritorial));
            //tmp.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.EQMA, (int)RefusalSource.InterTerritorial));

            //compatibility mode
            tmp.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEC, TypeSank.ZFactSank));
            tmp.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEE, TypeSank.ZFactSank));
            tmp.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.EQMA, TypeSank.ZFactSank));

            MedicalRefusals.AddRange(new ObservableCollection<ZRefusalModel>(tmp.OrderBy(p => p.ReasonCode)));
        }

        public void ReloadRefusals()
        {
            InitMode();
        }
    }
}
