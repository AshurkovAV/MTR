using System.Collections.ObjectModel;
using System.ComponentModel;
using Core.Extensions;
using GalaSoft.MvvmLight;

namespace Medical.AppLayer.Models.PolicySearch
{
    public class PolicyCheckPatientModel : ViewModelBase
    {
        private bool _isApply;

        public int? PatientId { get; set; }
        public string Data { get; set; }
        public ObservableCollection<PolicyCheckMedicalEventModel> Mevents { get; set; }

        public bool IsApply
        {
            get { return _isApply; }
            set
            {
                _isApply = value;
                RaisePropertyChanged(() => IsApply);
            }
        }

        [DefaultValue(true)]
        public bool IsExpanded { get; set; }

        public PolicyCheckPatientModel()
        {
            Init();
        }

        private void Init()
        {
            this.ApplyDefaultValues();
        }
    }

    public class PolicyCheckMedicalEventModel : ViewModelBase
    {
        private bool _isApply;
        public int? MedicalEventId { get; set; }
        public int? ZslMedicalEventId { get; set; }
        public int Reason { get; set; }
        public string Comments { get; set; }
        public string Data { get; set; }
        public decimal? Amount { get; set; }
        public decimal? ZslAmount { get; set; }

        public bool IsApply
        {
            get { return _isApply; }
            set
            {
                _isApply = value;
                RaisePropertyChanged(() => IsApply);
            }
        }

        [DefaultValue(true)]
        public bool IsExpanded { get; set; }

        public PolicyCheckMedicalEventModel()
        {
            Init();
        }

        private void Init()
        {
            this.ApplyDefaultValues();
        }
    }
}
