using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.WindowsUI;
using Medical.CoreLayer.Gui.Dialogs;
using MedicineNext.Internal;
using MedicineNext.UI.DXSplashScreen;
using MedicineNext.UI.Workbench;
using DispatcherHelper = GalaSoft.MvvmLight.Threading.DispatcherHelper;

namespace MedicineNext {
    /// <summary>
    /// Главная точка входа App.xaml
    /// </summary>
    public partial class App
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            args.Handled = true;
            var e = args.Exception;
            var exceptionDialog = new ExceptionDialog();
            if (e.Message != "Cross thread operation detected. To suppress this exception, set DevExpress.Xpf.Core.DXGridDataController.DisableThreadingProblemsDetection = true")
            {
                exceptionDialog.SetErrorString("Необработанное исключение!", e, typeof(App));
                exceptionDialog.ShowDialog();
            }

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Dispatcher.UnhandledException += DispatcherOnUnhandledException;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU");

            ApplicationThemeHelper.ApplicationThemeName = Theme.VS2010Name;

            var window = Start(() => base.OnStartup(e));
            if ( window != null )
            {
                window.Show();
                Current.MainWindow.Closing += MainWindowClosingHandler;
            }else
            {
                Current.Shutdown();
            }
            
        }

        private void MainWindowClosingHandler(object sender, CancelEventArgs e)
        {
            e.Cancel = WinUIMessageBox.Show(
                "Выйти из программы?",
                "Внимание",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No, MessageBoxOptions.None
                ) == MessageBoxResult.No;
        }

        public static Window Start(Action baseStartup)
        {
            MedicineSplashScreen.Show<AppSplashScreen>();

            MedicineSplashScreen.Progress("Инициализация загрузчика");
            if( !Bootstrapper.Instance.Init())
            {
                MedicineSplashScreen.Close();
                var exceptionDialog = new ExceptionDialog();
                exceptionDialog.SetErrorString("Ошибка при запуске программы", Bootstrapper.Instance.LastError, typeof(App));
                exceptionDialog.ShowDialog();
                return null;
            }

            //ThemeManager.ApplicationThemeName = Theme.VS2010Name;
            var view = new MainWindow();
            WpfWorkbench.DockManager = view.DockManager;
            MedicineSplashScreen.Close();
            return view;
        }

        private void OnAppStartup_UpdateThemeName(object sender, StartupEventArgs e)
        {

            //DevExpress.Xpf.Core.ApplicationThemeHelper.UpdateApplicationThemeName();
        }
    }
}
