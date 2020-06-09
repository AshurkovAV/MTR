using System;
using System.Collections.Generic;
using System.Windows;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.PivotGrid;
using DevExpress.Xpf.Printing;
using Medical.Classifiers.Managers;
using Medical.DatabaseCore.DataModel;
using MedicineCore.Services;
using Microsoft.Win32;
using Microsoft.Practices.Unity;

namespace Medical.Economic.Views
{
    /// <summary>
    /// Interaction logic for SearchResultView.xaml
    /// </summary>
    public partial class EconomicAccountStatisticsView
    {
        private readonly Dictionary<object, string> _assistance = new Dictionary<object, string>()
        {
            {1,"Стационар"},
            {2,"Дневной стационар"},
            {3,"Поликлиника"},
            {4,"Скорая помощь"}
        };

        private Dictionary<object, string> _territory = new Dictionary<object, string>();
       

        public EconomicAccountStatisticsView()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            var manager = UnityService.Instance.Resolve<ClassifierManager>();
            _territoryComboBox.ItemsSource = manager.GetClassifier<F010>();
            var ter = manager.GetClassifier<F010>();
            foreach (F010 f010 in ter)
            {
                _territory.Add(f010.KOD_OKATO,f010.SUBNAME);
            }
           
            Grid.UseAsyncMode = true;
            Grid.AsyncOperationStarting += Grid_AsyncOperationStarting;
            Grid.AsyncOperationCompleted += Grid_AsyncOperationCompleted;
        }

        void Grid_AsyncOperationCompleted(object sender, RoutedEventArgs e)
        {

        }

        void Grid_AsyncOperationStarting(object sender, RoutedEventArgs e)
        {
            
        }

        private void Grid_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Grid.BestFit();
        }

        private void BPrint_OnItemClick(object sender, ItemClickEventArgs e)
        {
            Grid.BestFit();

            var dlg = new SaveFileDialog { FileName = "statistics_" + DateTime.Now.ToShortDateString(), DefaultExt = ".xls", Filter = "Excel документы (.xls)|*.xls" };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                var link = new PrintableControlLink(Grid);
                link.ExportToXls(dlg.FileName);
            }

            //link.ShowPrintPreviewDialog(null);
        }

        private void Grid_OnFieldValueDisplayText(object sender, PivotFieldDisplayTextEventArgs e)
        {
            if (e.Field != null && e.Field.FieldName == "Details.AssistanceConditionsId")
            {
                e.DisplayText = _assistance[e.Value];
            }
            if (e.Field != null && e.Field.FieldName == "TerritoryAccount.Source")
            {
                e.DisplayText = _territory[e.Value];
            }
            if (e.Field != null && e.Field.FieldName == "TerritoryAccount.Destination")
            {
                e.DisplayText = _territory[e.Value];
            } 
        }

        private void _bRefresh_OnItemClick(object sender, ItemClickEventArgs e)
        {
            Grid.ReloadDataAsync();
            Grid.BestFit();
        }

        private void LookUpEditBase_OnSelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            switch (_directionComboBoxEdit.SelectedIndex)
            {
                case 0:
                    TerritoryS.FieldName = "TerritoryAccount.Destination";
                    break;
                case 1:
                    TerritoryS.FieldName = "TerritoryAccount.Source";
                    break;
                    
            }
            
        }
    }
}
