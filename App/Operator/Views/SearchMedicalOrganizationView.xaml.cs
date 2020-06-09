using System.Windows.Controls;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Operator.Views
{
    /// <summary>
    /// Interaction logic for SearchDiagnosis.xaml
    /// </summary>
    public partial class SearchMedicalOrganizationView : UserControl
    {
        public SearchMedicalOrganizationView()
        {
            InitializeComponent();

            Init();
            
        }

        private void Init()
        {
            MedicalOrganizationList.ItemsSource = new F003CacheFullItemsSource().GetValues();
        }

        private void MedicalOrganizationList_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            MedicalOrganizationList.ScrollIntoView(MedicalOrganizationList.SelectedItem);
        }
    }
}
