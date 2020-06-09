using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Utils;
using DataModel;
using DevExpress.Data;
using DevExpress.Data.Async.Helpers;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Register.Views
{
    /// <summary>
    /// Interaction logic for MedicalAccountView.xaml
    /// </summary>
    public partial class MedicalAccountView
    {
        public MedicalAccountView()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            MedicalOrganizationEdit.ItemsSource = new F003ItemsSource().GetValues();
        }

        List<DataModel.ZslEventShortView> _eventShortViews = new List<ZslEventShortView>(); 
        public object[] GetSelectedGridRows(GridControl gridControl)
        {
            var handles = gridControl.GetSelectedRowHandles();
            if (handles == null || !handles.Any()) return null;

            handles = handles.Where(x => x >= 0).Select(x => x).ToArray();
            var count = handles.Length;
            object[] obj = new object[count];

            for (int index = 0; index < count; index++)
            {
                int i = index;
                var handle = handles[i];
                var row = gridControl.GetRow(handle);

                if (row is DevExpress.Data.NotLoadedObject)
                {
                    gridControl.GetRowAsync(handle).ContinueWith(x =>
                    {
                        _eventShortViews.Add((DataModel.ZslEventShortView)x.Result);
                    });
                }
                else
                    _eventShortViews.Add((DataModel.ZslEventShortView)row);
            }
            return obj;
        }


        private void Test_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var rows = GetSelectedGridRows(GridControldet);
            bool isLoaded = false;
            GridControldet.IsEnabled = false;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        if (GridControldet.IsAsyncOperationInProgress == false)
                        {
                            isLoaded = true;
                        }
                    });
                    if (isLoaded) break;
                    Thread.Sleep(200);
                }

            }).ContinueWith(lr =>
            {
                foreach (var row in _eventShortViews)
                {
                    ControlResourcesLoger.LogDedug(row?.Surname + " " + row?.Name);
                }
                GridControldet.IsEnabled = true;


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
