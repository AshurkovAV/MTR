using System;
using System.Windows.Controls;
using Autofac;
using Core.Services;
using Medical.AppLayer.Economic.ViewModels;

namespace Medical.AppLayer.Economic.Views
{
    /// <summary>
    /// Interaction logic for EconomicAccountEditView.xaml
    /// </summary>
    public partial class EcoReportForm2PrefilterView : UserControl
    {
        public EcoReportForm2PrefilterView()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            try
            {
                using (var scope = Di.I.BeginLifetimeScope())
                {
                    DataContext = scope.Resolve<EcoReportForm2PrefilterViewModel>();
                }
            }
            catch (Exception exception)
            {
                //_messageService.ShowException(exception, "При добавлении/редактировании платежа для межтерриториального счета ID {0} произошла ошибка.".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }

    }
}
