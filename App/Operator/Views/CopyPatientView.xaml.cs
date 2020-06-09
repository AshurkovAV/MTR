using System.Windows.Controls;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Operator.Views
{
    /// <summary>
    /// Interaction logic for CopyPatientView.xaml
    /// </summary>
    public partial class CopyPatientView : UserControl
    {
        public CopyPatientView()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            TerritoryComboBox.ItemsSource = new F010TfItemsSource().GetValues();
        }
    }
}
