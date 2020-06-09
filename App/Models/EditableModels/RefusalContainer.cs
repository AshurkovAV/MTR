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
    public class RefusalContainer : ViewModelBase, IDataErrorInfo
    {
        private readonly DataErrorInfoSupport _dataErrorInfoSupport;
        private readonly IOperatorService _operatorService;

        private readonly int _medicalEventId;
        private readonly OperatorMode _mode;

        private RefusalModel _selectedRefusal;
        private Dictionary<OperatorMode, Action> _modeHandlers;
        private ObservableCollection<RefusalModel> _refusals;

        public ObservableCollection<RefusalModel> MedicalRefusals => _refusals ?? (_refusals = new ObservableCollection<RefusalModel>());

        public RefusalModel SelectedRefusal {
            get { return _selectedRefusal; }
            set { _selectedRefusal = value; RaisePropertyChanged(()=>SelectedRefusal); }
        }

        public decimal? RefusalPrice {
            get { return MedicalRefusals.Sum(p => p.Amount); }
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

        public RefusalContainer(IOperatorService operatorService, 
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
                {OperatorMode.InterTerritorialError, InitInterTerritorial},
                {OperatorMode.InterTerritorialSrzQuery, InitInterTerritorial},
                {OperatorMode.Local, InitLocal},
                {OperatorMode.LocalError, InitLocal},
                {OperatorMode.LocalSrzQuery, InitLocal},
                {OperatorMode.Patient, InitPatient}
            };
        }

        public void InitMode()
        {
            MedicalRefusals.Clear();

            if (_modeHandlers.ContainsKey(_mode))
            {
                _modeHandlers[_mode]();
            }
        }

        private void InitPatient()
        {
            //TODO
        }

        private void InitLocal()
        {
            MedicalRefusals.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEC, (int)RefusalSource.Local));
            MedicalRefusals.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEE, (int)RefusalSource.Local));
            MedicalRefusals.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.EQMA, (int)RefusalSource.Local));

            //compatibility mode
            //MedicalRefusals.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEC));
            //MedicalRefusals.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEE));
            //MedicalRefusals.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.EQMA));
        }

        private void InitInterTerritorial()
        {
            var tmp = new ObservableCollection<RefusalModel>();
            tmp.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.External));

            //tmp.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEC, (int)RefusalSource.InterTerritorial));
            //tmp.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEE, (int)RefusalSource.InterTerritorial));
            //tmp.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.EQMA, (int)RefusalSource.InterTerritorial));

            //compatibility mode
            tmp.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEC));
            tmp.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.MEE));
            tmp.AddRange(_operatorService.LoadRefusal(_medicalEventId, RefusalType.EQMA));

            MedicalRefusals.AddRange(new ObservableCollection<RefusalModel>(tmp.OrderBy(p => p.ReasonCode)));
        }

        public void ReloadRefusals()
        {
            InitMode();
        }
    }
}
