using System;
using System.Drawing.Printing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DataModel;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Printing;
using Medical.AppLayer.Filters;
using Medical.CoreLayer.Models.Common;

namespace Medical.AppLayer.Economic.Views
{
    /// <summary>
    /// Interaction logic for EconomicSankCollectionView.xaml
    /// </summary>
    public partial class EconomicSankCollectionView
    {
        private FiltrHelpers _filtr;
        public EconomicSankCollectionView()
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
            //CommonGridControl<ZslEventShortView>.GridEvent = GridEvent;
            //CommonGridControl<ZslEventShortView> commonGridControl = new CommonGridControl<ZslEventShortView>();
            //commonGridControl.GridRowsEvent(GridEvent);

            //bool isLoaded = false;
            //GridEvent.IsEnabled = false;

            //Task.Factory.StartNew(() =>
            //{
            //    while (true)
            //    {
            //        Dispatcher.BeginInvoke((Action)delegate ()
            //        {
            //            if (GridEvent.IsAsyncOperationInProgress == false)
            //            {
            //                isLoaded = true;
            //            }
            //        });
            //        if (isLoaded) break;
            //        Thread.Sleep(200);
            //    }

            //}).ContinueWith(lr =>
            //{
            //    GridEvent.IsEnabled = true;

            //}, TaskScheduler.FromCurrentSynchronizationContext());
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
