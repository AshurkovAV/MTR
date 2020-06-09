using System.Windows;

namespace MedicineNext.Internal.MVVM
{
    public class ModelBase : DependencyObject
    {
        public static readonly DependencyProperty NameProperty;

        static ModelBase()
        {
            NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(ModelBase), new PropertyMetadata(""));
        }
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
    }
}
