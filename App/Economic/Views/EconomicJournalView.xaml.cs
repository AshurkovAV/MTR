using System;
using System.Linq;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using Medical.DatabaseCore.Services.Classifiers;
using Microsoft.Win32;

namespace Medical.AppLayer.Economic.Views
{
    /// <summary>
    /// Interaction logic for SearchResultView.xaml
    /// </summary>
    public partial class EconomicJournalView
    {
        public EconomicJournalView()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            TerritoryComboBox.ItemsSource = new F010ItemsSource().GetValues();
        }

        private void BPrint2_OnItemClick(object sender, ItemClickEventArgs e)
        {
            tableView.BestFitColumns();
            tableView.PrintAutoWidth = false;

            GridColumn grouped = null; 
            foreach (GridColumn column in Grid.Columns.Where(column => column.GroupIndex != -1))
            {
                column.GroupIndex = -1;
                grouped  = column;
            }

            var link = new PrintableControlLink(tableView);
            var dlg = new SaveFileDialog { FileName = "journal_" + DateTime.Now.ToShortDateString(), DefaultExt = ".xls", Filter = "Excel документы (.xls)|*.xls" };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                link.ExportToXls(dlg.FileName);
            }

            if (grouped != null)
            {
                grouped.GroupIndex = 0;
            }
            
            //link.ShowPrintPreviewDialog(null);
        }

        private void gridControl1_GroupRowCollapsing(object sender, RowAllowEventArgs e)
        {
            e.Allow = false;
        }
    }
}
