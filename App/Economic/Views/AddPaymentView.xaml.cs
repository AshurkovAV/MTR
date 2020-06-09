using System.Windows.Controls;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Economic.Views
{
    /// <summary>
    /// Interaction logic for EconomicAccountEditView.xaml
    /// </summary>
    public partial class AddPaymentView : UserControl
    {
        public AddPaymentView()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            PaymentStatusComboBoxEdit.ItemsSource = new PaymentStatusItemsSource().GetValues();
        }

    }
}
