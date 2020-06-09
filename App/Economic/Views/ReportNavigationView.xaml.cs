using System;
using System.Windows.Controls;
using Autofac;
using Core;
using Core.Services;
using Medical.AppLayer.Economic.ViewModels;

namespace Medical.AppLayer.Economic.Views
{
    /// <summary>
    /// Interaction logic for ReportNavigationView.xaml
    /// </summary>
    public partial class ReportNavigationView : UserControl
    {
        public ReportNavigationView()
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
                    DataContext = scope.Resolve<ReportNavigationViewModel>();
                }
            }
            catch (Exception exception)
            {
                //_messageService.ShowException(exception, "При добавлении/редактировании платежа для межтерриториального счета ID {0} произошла ошибка.".F(CurrentRow.TerritoryAccountId), typeof(TerritoryAccountViewModel));
            }
        }
    }
}
