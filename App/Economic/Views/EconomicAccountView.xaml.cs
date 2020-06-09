
using System;
using System.Threading;
using System.Threading.Tasks;
using DataModel;
using DevExpress.Xpf.Bars;
using Medical.AppLayer.Economic.Models;
using Medical.CoreLayer.Models.Common;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Economic.Views
{
    /// <summary>
    /// Interaction logic for SearchResultView.xaml
    /// </summary>
    public partial class EconomicAccountView
    {
        public EconomicAccountView()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            TerritoriesEdit.ItemsSource = new F010ItemsSource().GetValues();
        }

        private void EconomicCheck_OnItemClick(object sender, ItemClickEventArgs e)
        {
            CommonGridControl<EconomicAccountCustomModel> commonGridControl = new CommonGridControl<EconomicAccountCustomModel>();
            commonGridControl.GridRowsEconomicAccount(Grid);

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
    }
}
