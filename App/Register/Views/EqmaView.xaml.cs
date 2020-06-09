using Medical.DatabaseCore.Services.Classifiers;
namespace Medical.AppLayer.Register.Views
{
    /// <summary>
    /// 
    /// </summary>
    public partial class EqmaView
    {
        public EqmaView()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            ExpertCodeComboBox.ItemsSource = new F004ItemsSource().GetValues();
            OutcomeComboBox.ItemsSource = new GlobalEqmaOutcomesItemsSource().GetValues();
        }
    }
}
