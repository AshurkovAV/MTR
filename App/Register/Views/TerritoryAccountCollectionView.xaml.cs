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
    /// Interaction logic for TerritoryAccountCollectionView.xaml
    /// </summary>
    public partial class TerritoryAccountCollectionView
    {
        private FiltrHelpers _filtr;
        public TerritoryAccountCollectionView()
        {
            InitializeComponent();
            _filtr = new FiltrHelpers();
            Initialize();
        }

        private void Initialize()
        {
            _filtr.LoadFilter(GridEvent,TypeFiler.Event);
        }

        private void SaveFiltr_OnItemClick(object sender, ItemClickEventArgs e)
        {
            using (var stream = new MemoryStream())
            {
                _filtr.SaveFilter(stream, TypeFiler.Account);
            }
            using (var stream = new MemoryStream())
            {
                GridEvent.SaveLayoutToStream(stream);
                _filtr.SaveFilter(stream, TypeFiler.Event);
            }
        }

        private void EventCheck_OnItemClick(object sender, ItemClickEventArgs e)
        {
            CommonGridControl<ZslEventShortView>.GridEvent = GridEvent;
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
    }
}
