using System.Windows.Controls;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Operator.Views
{
    /// <summary>
    /// Interaction logic for SearchDiagnosis.xaml
    /// </summary>
    public partial class SearchInsuranceView : UserControl
    {
        public SearchInsuranceView()
        {
            InitializeComponent();

            Init();
            
        }

        private void Init()
        {
            InsuranceList.ItemsSource = new F002ItemsSource().GetValues();
        }

        private void InsuranceList_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
            InsuranceList.ScrollIntoView(InsuranceList.SelectedItem);
        }
    }
}
