using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using Core.Extensions;
using Medical.CoreLayer.Gui;
using Medical.DatabaseCore.Services.Classifiers;
using Microsoft.Windows.Controls.PropertyGrid.Attributes;

namespace Medical.AppLayer.Operator.Views
{
    /// <summary>
    /// Interaction logic for OperatorView.xaml
    /// </summary>
    public partial class OperatorView : UserControl
    {
        public OperatorView()
        {
            InitializeComponent();
            Init();
            
        }

        private void Init()
        {
            SexComboBoxEdit.ItemsSource = new V005ItemsSource().GetValues();
            F008ComboBoxEdit.ItemsSource = new F008ItemsSource().GetValues();
            F011ComboBoxEdit.ItemsSource = new F011ItemsSource().GetValues();
            F010ComboBoxEdit.ItemsSource = new F010TfItemsSource().GetValues();
            V005ReprComboBoxEdit.ItemsSource = new V005ItemsSource().GetValues();
            F010RegionComboBoxEdit.ItemsSource = new F010ItemsSource().GetValues();
            V010ComboBoxEdit.ItemsSource = new V010ItemsSource().GetValues();
            F005ComboBoxEdit.ItemsSource = F005ComboBoxEdit2.ItemsSource = new F005ItemsSource().GetValues();
            V002ServiceComboBoxEdit.ItemsSource = new V002ItemsSource().GetValues();
            V004ServiceComboBoxEdit.ItemsSource = new V004ItemsSource().GetValues();
            V014ComboBoxEdit.ItemsSource = new V014ItemsSource().GetValues();
            V015ServiceComboBoxEdit.ItemsSource = new V015ItemsSource().GetValues();
            V015MeventComboBoxEdit.ItemsSource = new V015ItemsSource().GetValues();
            HospitalizationComboBoxEdit.ItemsSource = new HospitalizationTypeItemsSource().GetValues();
            V004MeventComboBoxEdit.ItemsSource = new V004ItemsSource().GetValues();
            V002MeventComboBoxEdit.ItemsSource = new V002ItemsSource().GetValues();
            V006ComboBoxEdit.ItemsSource = new V006aItemsSource().GetValues();
            V008ComboBoxEdit.ItemsSource = new V008ItemsSource().GetValues();
            F014ComboBoxEdit.ItemsSource = new F014ItemsSource().GetValues();
            EventTypeComboBoxEdit.ItemsSource = new EventTypeItemsSource().GetValues();

            RegionalAttributeComboBoxEdit.ItemsSource = new RegionalAttributeItemsSource().GetValues();
            HealthGroupComboBoxEdit.ItemsSource = new HealthGroupItemsSource().GetValues();
            JobStatusComboBoxEdit.ItemsSource = new JobStatusItemsSource().GetValues();
        }
       
    }
}
