using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;


namespace Medical.AppLayer.Editors
{
    /// <summary>
    /// Interaction logic for XmlEditor.xaml
    /// </summary>
    public partial class XmlEditor : UserControl
    {
        public XmlEditor()
        {
            InitializeComponent();
            
            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object arg)
            {
                
                return null;
            }, null);
        }

        
    }
}
