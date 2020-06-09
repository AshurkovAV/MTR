using System;

namespace Medical.CoreLayer.Attributes
{
    /// <summary>
    /// Атрибут для комманд стартовой страницы
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class StartPageCommandAttribute : Attribute
    {
        /// <summary>
        /// Тип класса
        /// </summary>
        public Type Class { get; set; }
        /// <summary>
        /// Категорий пункта меню
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Название пункта меню
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        /// Изображение для пункты меню (берется относительно CoreLayer/Resource/IResourceCatalog)
        /// </summary>
        public string ImageSource { get; set; }
        /// <summary>
        /// Размер плитки
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// Цвет плитки в формате #XXXXXXXX (ARGB hex #FF00ABDC)
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// Порядок расположения плитки (строка)
        /// </summary>
        public int Order { get; set; }

        public StartPageCommandAttribute()
        {
            Class = null;
            Category = string.Empty;
            Caption = string.Empty;
            ImageSource = "/Images/64/link.png";
            Size = 0;
            Color = "#FF00ABDC";
            Order = 0;
        }

        public StartPageCommandAttribute(string category, string caption, string imageSource, int size, string color, int order)
        {
            Category = category;
            Caption = caption;
            ImageSource = imageSource;
            Size = size;
            Color = color;
            Order = order;
        }

        public StartPageCommandAttribute(Type classtype, string category, string caption, string imageSource, int size, string color)
        {
            Class = classtype;
            Category = category;
            Caption = caption;
            ImageSource = imageSource;
            Size = size;
            Color = color;
            Order = 0;
        }

        public StartPageCommandAttribute(Type classtype, string category, string caption, string imageSource, int size, string color, int order)
        {
            Class = classtype;
            Category = category;
            Caption = caption;
            ImageSource = imageSource;
            Size = size;
            Color = color;
            Order = order;
        }
    }

}
