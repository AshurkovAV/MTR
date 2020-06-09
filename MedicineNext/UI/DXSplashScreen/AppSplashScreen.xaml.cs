using System;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using MedicineNext.Internal;

namespace MedicineNext.UI.DXSplashScreen
{
    /// <summary>
    /// Interaction logic for AppSplashScreen.xaml
    /// </summary>
    public partial class AppSplashScreen : Window, ISplashScreenEx
    {
        private static volatile AppSplashScreen _instance;

        public static AppSplashScreen Instance => _instance;

        public AppSplashScreen()
        {
            _instance = this;         
            InitializeComponent();
            this.board.Completed += OnAnimationCompleted;
        }

        #region ISplashScreen
        public void Progress(double value)
        {
            ProgressBar.Value = value;
        }

        public void Progress(string value)
        {
            Info.Text = value;
        }

        public void CloseSplashScreen()
        {
            this.board.Begin(this);
        }
        public void SetProgressState(bool isIndeterminate)
        {
            ProgressBar.StyleSettings = new ProgressBarMarqueeStyleSettings();
            var metropolisDarkTheme = new Theme("Office2013") { IsStandard = true };
            ProgressBar.SetValue(ThemeManager.ThemeNameProperty, metropolisDarkTheme.Name);
        }
        #endregion

        #region Event Handlers
        void OnAnimationCompleted(object sender, EventArgs e)
        {
            this.board.Completed -= OnAnimationCompleted;
            this.Close();
        }
        #endregion
    }
}
