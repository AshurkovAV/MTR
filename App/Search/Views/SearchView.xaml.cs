using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Core.Extensions;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.LayoutControl;
using Medical.AppLayer.Attributes;

namespace Medical.AppLayer.Search.Views
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        public SearchView()
        {
            InitializeComponent();
        }

        private static void CopyBinding(BaseEdit oldContent, BaseEdit newContent, DependencyProperty property)
        {
            Binding binding = oldContent.GetBindingExpression(property).ParentBinding;
            newContent.SetBinding(property, binding);
        }

        private static void CopyValue(BaseEdit oldContent, BaseEdit newContent, DependencyProperty property)
        {
            newContent.SetValue(BaseEdit.IsReadOnlyProperty, oldContent.GetValue(property));
        }

        private void AutoGeneratingItemHandler(object sender, DataLayoutControlAutoGeneratingItemEventArgs e)
        {
            if (sender is DataLayoutControl)
            {
                var t = (sender as DataLayoutControl).CurrentItem.GetType();
                var p = t.GetProperty(e.PropertyName);

                var attributes = p.GetAttributes<CustomEditorAttribute>();
                foreach (var attribute in attributes)
                {
                    var oldContent = e.Item.Content as BaseEdit;
                    dynamic newContent = null;
                    if (attribute.EditorType != null)
                    {
                        newContent = Activator.CreateInstance(attribute.EditorType);
                    }

                    if (newContent != null)
                    {
                        if (attribute.ItemSourceType != null)
                        {
                            dynamic source = Activator.CreateInstance(attribute.ItemSourceType);
                            newContent.ItemsSource = source.GetValues();
                        }
                        if (attribute.DisplayName.IsNotNullOrWhiteSpace())
                        {
                            newContent.DisplayMember = attribute.DisplayName;
                        }
                        if (attribute.Value.IsNotNullOrWhiteSpace())
                        {
                            newContent.ValueMember = attribute.Value;
                        }

                        e.Item.Content = newContent;
                        try
                        {
                            CopyBinding(oldContent, newContent, BaseEdit.EditValueProperty);
                            CopyValue(oldContent, newContent, BaseEdit.IsReadOnlyProperty);
                        }
                        catch (Exception)
                        {

                        }

                    }
                }
            }
        }
    }


}
