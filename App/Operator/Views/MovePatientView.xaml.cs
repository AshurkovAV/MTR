using System;
using System.Windows.Controls;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Operator.Views
{
    /// <summary>
    /// Interaction logic for MovePatientView.xaml
    /// </summary>
    public partial class MovePatientView : UserControl
    {
        public MovePatientView()
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
