using System.ComponentModel;
using GalaSoft.MvvmLight;

namespace Medical.AppLayer.Models.PolicySearch
{
    public class PolicyCheckTortillaResultModel : ViewModelBase
    {
        private bool _isApply;

        public int? PatientId { get; set; }
        public string INP { get; set; }
        public int? TerritoryOkato { get; set; }
        public string Comments { get; set; }
        public bool IsApply
        {
            get { return _isApply; }
            set
            {
                _isApply = value;
                RaisePropertyChanged(() => IsApply);
            }
        }

        public string Data { get; set; }

        [DefaultValue(true)]
        public bool IsExpanded { get; set; }
    }
}
