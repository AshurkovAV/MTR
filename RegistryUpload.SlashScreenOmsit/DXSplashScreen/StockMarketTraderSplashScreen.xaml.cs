using System;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using Omsit.UpdateSlashScreen.DXSplashScreen.Internal;

namespace Omsit.UpdateSlashScreen.DXSplashScreen
{

    public partial class StockMarketTraderSplashScreen : Window, ISplashScreenEx
    {
        public StockMarketTraderSplashScreen() {
            InitializeComponent();
            Footer_Text.Text = String.Format("Omsit © - {0}", DateTime.Now.Year);
            this.board.Completed += OnAnimationCompleted;
        }

        #region ISplashScreen
        public void Progress(double value) {
            progressBar.Value = value;
        }
        public void CloseSplashScreen() {
            this.board.Begin(this);
        }
        public void SetProgressState(bool isIndeterminate) {
            progressBar.StyleSettings = new ProgressBarMarqueeStyleSettings();
#if DEBUGTEST
            progressBar.SetValue(ThemeManager.ThemeNameProperty, Theme.MetropolisDarkName);
#else
            Theme metropolisDarkTheme = new Theme("MetropolisDark") { IsStandard = true };
            progressBar.SetValue(ThemeManager.ThemeNameProperty, metropolisDarkTheme.Name);
#endif
        }
        #endregion

        #region Event Handlers
        void OnAnimationCompleted(object sender, EventArgs e) {
            this.board.Completed -= OnAnimationCompleted;
            this.Close();
        }

        public void Progress(string value)
        {
            Info.Text = value;
        }
        #endregion
    }
}
