using GalaSoft.MvvmLight;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class DoctorMoViewModel : ViewModelBase
    {
        /*private List<DoctorViewModel> _current;

        public DoctorMoViewModel(string doctorCode)
        {
            using (var db = new DatabaseIns())
            {
                Current = db.GetTableQuery<shareDoctor>().Where(p => p.Code == doctorCode).Select(p => new DoctorViewModel(p)).ToList();
                
            }
        }

        public List<DoctorViewModel> Current
        {
            get { return _current; }
            set
            {
                _current = value;
                RaisePropertyChanged("Current");
            }
        }*/
    }


}