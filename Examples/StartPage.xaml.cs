using System.Windows.Controls;

namespace Examples
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();

            DataContext = new StartPageViewModel();
        }
    }
}
