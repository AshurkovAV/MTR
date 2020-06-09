using GalaSoft.MvvmLight;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class SrzAnswerViewModel : ViewModelBase
    {
        public SrzAnswer Data {get;set;}

        public SrzAnswerViewModel(SrzAnswer _data)
        {
            Data = _data;
        }

        public int Status
        {
            get { 
                return 0;
            }
        }

        public string enp { get { return Data.enp; } }
        public string main_enp { get { return Data.main_enp; } }
        public string date_beg { get { return Data.date_beg; } }
        public string date_end { get { return Data.date_end; } }
        public string inssernum { get { return Data.inssernum; } }
        public string terr_code { get { return Data.terr_code; } }
    }
}
