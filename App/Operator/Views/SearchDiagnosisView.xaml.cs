using System.Windows.Controls;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Operator.Views
{
    /// <summary>
    /// Interaction logic for SearchDiagnosis.xaml
    /// </summary>
    public partial class SearchDiagnosisView : UserControl
    {
        public SearchDiagnosisView()
        {
            InitializeComponent();

            Init();
            
        }

        private void Init()
        {
            DiagnosisList.ItemsSource = new M001CacheItemsSource().GetValues();
        }

        private void DiagnosisList_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            DiagnosisList.ScrollIntoView(DiagnosisList.SelectedItem);
        }
    }
}
