using System.Windows.Controls;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Operator.Views
{
    /// <summary>
    /// Interaction logic for SearchDiagnosis.xaml
    /// </summary>
    public partial class SearchDoctorView : UserControl
    {
        public SearchDoctorView()
        {
            InitializeComponent();

            Init();
            
        }

        private void Init()
        {
            DoctorList.ItemsSource = new ShareDoctorItemsSource().GetValues();
        }

        private void DoctorList_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            DoctorList.ScrollIntoView(DoctorList.SelectedItem);
        }
    }
}
