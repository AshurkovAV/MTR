using DataModel;
using GalaSoft.MvvmLight;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class SrzQueryViewModel : ViewModelBase
    {
        public FactSrzQuery Data {get;set;}

        public SrzQueryViewModel(FactSrzQuery _data)
        {
            Data = _data;
        }

        public string Details
        {
            get { return string.Format("{0} - {1}", Data.DateQuery.Value.ToString("dd MMMM yyyy"), TypeString); }
        }

        public string TypeString
        {
            get {
                switch (Data.Type)
                {
                    case 1:
                        return "СНИЛС";
                    case 2:
                        return "УДЛ";
                    case 3:
                        return "ЕНП";
                    default:
                        return "Нет";
                }
           }
        }

        public int SrzQueryId
        {
            get { return Data.SrzQueryId; }
        }

    }
}
