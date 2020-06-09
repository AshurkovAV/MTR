using System.Windows.Controls;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Operator.Views
{
    /// <summary>
    /// Interaction logic for SearchKsgKpgView.xaml
    /// </summary>
    public partial class SearchKsgKpgView : UserControl
    {
        public SearchKsgKpgView()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            V006ComboBoxEdit.ItemsSource = new V006aItemsSource().GetValues();
            ColumnNumberDifficultyTreatment.DataContext = new KslpItemsSource().GetValues();
            CritColumn.DataContext = new V024CacheItemsSource().GetValues();
        }

        private void DiagnosisList_EditValueChanged(object sender, DevExpress.Xpf.Editors.EditValueChangedEventArgs e)
        {
           // DKsgKpgsList.ScrollIntoView(DKsgKpgsList.SelectedItem);
        }
    }
}
