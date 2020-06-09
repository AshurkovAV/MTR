using System.Windows;
using System.Windows.Data;
using Core.Extensions;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using Medical.AppLayer.Converters;
using System.Windows.Markup;

namespace Medical.AppLayer.Classifiers.Views
{
    /// <summary>
    /// Interaction logic for AccountConfigView.xaml
    /// </summary>
    public partial class ClassifierView
    {
        string cellTemplate = @"
            <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                            xmlns:dxe=""http://schemas.devexpress.com/winfx/2008/xaml/editors"">
                <dxe:TextEdit x:Name=""PART_Editor""/>
            </DataTemplate>";

        public ClassifierView()
        {
            InitializeComponent();
        }

        private void GridControl_AutoGeneratingColumn(object sender, DevExpress.Xpf.Grid.AutoGeneratingColumnEventArgs e)
        {
            if (sender is GridControl)
            {
                if ((sender as GridControl).CurrentItem.IsNull())
                {
                    return;
                }
                

                switch (e.Column.FieldName)
                {
                    case "MedicalOrganizationCode":
                        e.Column.Binding = new Binding(e.Column.FieldName) { Converter = new MedicalOrganizationConverter() };
                        break;
                    case "Profile_Id":
                    case "ProfileId":
                        e.Column.CellTemplate = XamlReader.Parse(cellTemplate) as DataTemplate;
                        e.Column.Binding = new Binding(e.Column.FieldName) { Converter = new ProfileConverter() };
                        break;
                    case "Speciality_Id":
                        e.Column.CellTemplate = XamlReader.Parse(cellTemplate) as DataTemplate;
                        e.Column.Binding = new Binding(e.Column.FieldName) { Converter = new SpecialityV015Converter() };
                        break;
                    case "SpecialityId":
                        e.Column.CellTemplate = XamlReader.Parse(cellTemplate) as DataTemplate;
                        e.Column.Binding = new Binding(e.Column.FieldName) { Converter = new SpecialityV004Converter() };
                        break;
                    case "AssistanceCondition_id":
                        e.Column.CellTemplate = XamlReader.Parse(cellTemplate) as DataTemplate;
                        e.Column.Binding = new Binding(e.Column.FieldName) { Converter = new AssistanceConditionsConverter() };
                        break;
                    case "AssistanceType_Id":
                        e.Column.CellTemplate = XamlReader.Parse(cellTemplate) as DataTemplate;
                        e.Column.Binding = new Binding(e.Column.FieldName) { Converter = new AssistanceTypeConverter() };
                        break;
                }
            }
        }
    }
}
