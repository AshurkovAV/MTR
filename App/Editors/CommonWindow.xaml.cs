using System.Windows;
using MahApps.Metro.Controls;

namespace Medical.AppLayer.Editors
{
    /// <summary>
    /// Interaction logic for CommonWindow.xaml
    /// </summary>
    public partial class CommonWindow : MetroWindow
    {
        public CommonWindow(Window owner, double width, double height, object model)
        {
            InitializeComponent();

            Owner = owner;
            Height = height;
            Width = width;
            DataContext = model;
        }
    }
}
