using System;
using System.Windows.Controls;
using Autofac;
using Core.Services;
using Medical.AppLayer.Economic.ViewModels;
using Medical.DatabaseCore.Services.Classifiers;

namespace Medical.AppLayer.Economic.Views
{
    /// <summary>
    /// Interaction logic for EconomicAccountEditView.xaml
    /// </summary>
    public partial class EcoReportRevisePrefilterView : UserControl
    {
        public EcoReportRevisePrefilterView()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            try
            {
                _territoryComboBox.ItemsSource = new F010ItemsSource().GetValues();
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    DataContext = scope.Resolve<EcoReportRevisePrefilterViewModel>();
                }
            }
            catch (Exception exception)
            {
                //_messageService.ShowException(exception, "При добавлении/редактировании платежа для межтерриториального счета ID {0} произошла ошибка.".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

    }
}
