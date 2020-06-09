using GalaSoft.MvvmLight;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class SrzErrorViewModel : ViewModelBase
    {
        public ZP1errors Data { get; set; }

        public SrzErrorViewModel(ZP1errors _data)
        {
            Data = _data;
        }

        public string iso_err_code { get { return Data.iso_err_code; } }
        public string iso_err_descr { get { return Data.iso_err_descr; } }
        public string app_err_code { get { return Data.app_err_code; } }
        public string app_err_descr { get { return Data.app_err_descr; } }

        public string error_full { get { return string.Format("{0} {1} {2} {3}", Data.iso_err_code, Data.iso_err_descr, Data.app_err_code, Data.app_err_descr); } }
    }

}
