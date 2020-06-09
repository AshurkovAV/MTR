
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using GalaSoft.MvvmLight;

namespace Medical.AppLayer.Examination.ViewModels
{
    public class ExamResultModel : ViewModelBase
    {
        private string _comments;
        private bool _isApply;

        public bool IsApply {
            get { return _isApply; }
            set { _isApply = value;RaisePropertyChanged(()=>IsApply); }
        }
        public int? ErrorScope { get; set; }
        public int? Id { get; set; }
        public int? Reason { get; set; }
        public decimal? RefusalPercent { get; set; }
        public decimal? RefusalAmount { get; set; }
        public string Comments {
            get { return _comments; }
            set { _comments = value;RaisePropertyChanged(()=>Comments); } 
        }

        public int UserId { get; set; }

        public int Scope { get; set; }
    }
}
