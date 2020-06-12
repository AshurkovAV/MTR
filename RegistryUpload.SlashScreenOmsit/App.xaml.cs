using System.Windows;
using DevExpress.Xpf.Core;
using Omsit.UpdateSlashScreen.DXSplashScreen;
using Omsit.UpdateSlashScreen.DXSplashScreen.Internal;

namespace Omsit.UpdateSlashScreen
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnAppStartup_UpdateThemeName(object sender, StartupEventArgs e)
        {
            DevExpress.Xpf.Core.ApplicationThemeHelper.UpdateApplicationThemeName();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");

            ThemeManager.ApplicationThemeName = Theme.HybridApp.Name;
            MedicineSplashScreen.Show<StockMarketTraderSplashScreen>();


            if (!Bootstrapper.Instance.Init(e))
            {
                MedicineSplashScreen.Progress("Ошибка при запуске программы");
                MedicineSplashScreen.Close();
            }
            MedicineSplashScreen.Close();
            Application.Current.Shutdown();
        }
    }
}
