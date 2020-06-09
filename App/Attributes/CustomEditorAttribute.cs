using System;
using System.Windows;

namespace Medical.AppLayer.Attributes
{
    /// <summary>
    /// Атрибут пользовательского редактора данных для использования с DataLayoutControl и PropertyGridControl
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class CustomEditorAttribute : Attribute
    {
        public Type EditorType { get; set; }
        public Type ItemSourceType { get; set; }
        public DependencyProperty Property { get; set; }
        public string Value { get; set; }
        public string DisplayName { get; set; }
    }
}
