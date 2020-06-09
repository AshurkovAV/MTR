using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using Medical.DatabaseCore.Services.Classifiers;
using DataModel;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting;
using Medical.AppLayer.Filters;
using Medical.CoreLayer.Models.Common;
using Microsoft.Win32;


namespace Medical.AppLayer.Register.Views
{
    /// <summary>
    /// Interaction logic for SearchResultView.xaml
    /// </summary>
    public partial class TerritoryAccountView
    {
        private FiltrHelpers _filtr;
        public TerritoryAccountView()
        {
            InitializeComponent();
            _filtr = new FiltrHelpers();
            Initialize();
        }

        private void Initialize()
        {
            TerritoriesEdit.ItemsSource = new F010ItemsSource().GetValues();

            _filtr.LoadFilter(Grid,TypeFiler.Account);
            _filtr.LoadFilter(GridEvent,TypeFiler.Event);
        }

        private void gridControl1_GroupRowCollapsing(object sender, RowAllowEventArgs e)
        {
            if (e.Row == null ||

                colAccountNumber.GroupIndex != -1)
                e.Allow = false;

            e.Allow = false;
        }

        private void SaveFiltr_OnItemClick(object sender, ItemClickEventArgs e)
        {
            using (var stream = new MemoryStream())
            {
                Grid.SaveLayoutToStream(stream);
                _filtr.SaveFilter(stream, TypeFiler.Account);
            }
            using (var stream = new MemoryStream())
            {
                GridEvent.SaveLayoutToStream(stream);
                _filtr.SaveFilter(stream, TypeFiler.Event);
            }
        }
        
        private void Test_OnItemClick(object sender, ItemClickEventArgs e)
        {
            CommonGridControl<FactTerritoryAccount> commonGridControl = new CommonGridControl<FactTerritoryAccount>();
            commonGridControl.GridRowsTerAccount(Grid);
            bool isLoaded = false;
            Grid.IsEnabled = false;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        if (Grid.IsAsyncOperationInProgress == false)
                        {
                            isLoaded = true;
                        }
                    });
                    if (isLoaded) break;
                    Thread.Sleep(200);
                }

            }).ContinueWith(lr =>
            {
               Grid.IsEnabled = true;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void EventCheck_OnItemClick(object sender, ItemClickEventArgs e)
        {
            //CommonGridControl<FactTerritoryAccount> commonGrid = new CommonGridControl<FactTerritoryAccount>();
            //commonGrid.GridRowsTerAccount(Grid);
            //var version = CommonGridControl<FactTerritoryAccount>._selectTerritoryAccount;
            //if (Constants.Zversion.Contains(version))
            //{
            //    CommonGridControl<ZslEventShortView> commonGridControl = new CommonGridControl<ZslEventShortView>();
            //    commonGridControl.GridRowsEvent(GridEvent);
            //}
            //else
            //{
            //    CommonGridControl<EventExtendedView> commonGridControl = new CommonGridControl<EventExtendedView>();
            //    commonGridControl.GridRowsEvent(GridEvent);
            //}

            CommonGridControl<ZslEventShortView>.GridEvent = GridEvent;

            //SaveFileDialog dialogService = new SaveFileDialog
            //{
            //    Filter = "Excel File (*.xlsx)|*.xlsx"
            //};
            //bool? result = dialogService.ShowDialog();

            //tableView2.ExportToXlsx(@"d://grid_export.xlsx");
            //GridEvent.View.ExportToXls("D://121212.xls", new DevExpress.XtraPrinting.XlsExportOptions(DevExpress.XtraPrinting.TextExportMode.Value));
            //if (result == true)
            //{
                
            //}

            CommonGridControl<ZslEventShortView> commonGridControl = new CommonGridControl<ZslEventShortView>();
            commonGridControl.GridRowsEvent(GridEvent);

            bool isLoaded = false;
            GridEvent.IsEnabled = false;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        if (GridEvent.IsAsyncOperationInProgress == false)
                        {
                            isLoaded = true;
                        }
                    });
                    if (isLoaded) break;
                    Thread.Sleep(200);
                }

            }).ContinueWith(lr =>
            {
                GridEvent.IsEnabled = true;

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ExportExcel_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var link = new PrintableControlLink(tableView2)
            {
                Margins = new Margins(3, 3, 3, 3),
                Landscape = true,
            };
            link.ShowPrintPreview(this, "Предварительный просмотр");
        }

        private void ExportExcelAccount_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var link = new PrintableControlLink(tableView)
            {
                Margins = new Margins(3, 3, 3, 3),
                Landscape = true,
            };
            link.ShowPrintPreview(this, "Предварительный просмотр");
        }
    }
}
