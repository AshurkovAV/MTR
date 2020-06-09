using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Core.Extensions;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.LayoutControl;
using Medical.AppLayer.Attributes;

namespace Medical.AppLayer.Editors
{
    /// <summary>
    /// Interaction logic for DataControl.xaml
    /// </summary>
    public partial class DataControl : UserControl
    {
        public DataControl()
        {
            InitializeComponent();
        }

        private void DataLayoutControl_AutoGeneratingItem(object sender, DevExpress.Xpf.LayoutControl.DataLayoutControlAutoGeneratingItemEventArgs e)
        {
            BaseEdit editor = e.Item.Content as BaseEdit;
            if (editor.IsNotNull())
            {
                Binding binding = editor.GetBindingExpression(BaseEdit.EditValueProperty).ParentBinding;
                if (binding.Mode == BindingMode.TwoWay)
                {
                    editor.SetBinding(BaseEdit.EditValueProperty, new Binding
                    {
                        Path = binding.Path, 
                        UpdateSourceTrigger = UpdateSourceTrigger.LostFocus,
                        ValidatesOnDataErrors = true
                    });
                }
            }
            

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

                        if (newContent is ComboBoxEdit)
                        {
                            newContent.IncrementalFiltering = true;
                            newContent.ImmediatePopup = true;
                            newContent.AutoComplete = true;
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

        private static void CopyBinding(BaseEdit oldContent, BaseEdit newContent, DependencyProperty property)
        {
            Binding binding = oldContent.GetBindingExpression(property).ParentBinding;
            newContent.SetBinding(property, binding);
        }

        private static void CopyValue(BaseEdit oldContent, BaseEdit newContent, DependencyProperty property)
        {
            newContent.SetValue(BaseEdit.IsReadOnlyProperty, oldContent.GetValue(property));
        }
    }
}
