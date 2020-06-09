using GalaSoft.MvvmLight;

namespace Medical.AppLayer.Operator.ViewModel
{
    public class DepartmentViewModel : ViewModelBase
    {
        /*private List<MedicalDepartmentViewModel> _current;

        public DepartmentViewModel(int departmentCode, int moCode)
        {
            using (var db = new DatabaseIns())
            {
                Current = db.GetTableQuery<shareMedicalDepartment>().Where(p => p.DepartmentCode == departmentCode).Select(p=>new MedicalDepartmentViewModel(p)).ToList();
                
            }
        }

        public List<MedicalDepartmentViewModel> Current
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