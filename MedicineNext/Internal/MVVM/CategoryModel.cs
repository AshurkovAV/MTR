using System;
using System.Collections.ObjectModel;
using System.Windows;
using Core.Perfomance;
using GalaSoft.MvvmLight;

namespace MedicineNext.Internal.MVVM
{
    public class CategoryModel : ViewModelBase
    {
        private string _color;
        private bool _isVisible;
        public ObservableCollection<PageModel> Pages { get; set; }

        public string Color
        {
            get { return _color; }
            set { _color = value; RaisePropertyChanged(() => Color); }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; RaisePropertyChanged(() => IsVisible); }
        }

        public bool IsDefault { get; set; }

        public bool IsContext { get; set; }
        public Type Typo { get; set; }

        public CategoryModel()
        {
            Pages = new ObservableCollection<PageModel>();
            Color = "Orange";
            IsDefault = true;
        }
        public void Clear()
        {
            foreach (PageModel cat in Pages)
            {
                cat.Clear();
            }
            Pages.Clear();
        }
    }

    public class ContextCategoryModel : CategoryModel
    {
    }
}

/*
 * public class CategoryModel : ModelBase
    {
        public static readonly DependencyProperty PagesProperty =
            DependencyProperty.Register("Pages", typeof(ObservableCollection<PageModel>), typeof(CategoryModel), new PropertyMetadata(null));
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(String), typeof(CategoryModel), new PropertyMetadata(null));
        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisible", typeof(bool), typeof(CategoryModel), new PropertyMetadata(null));

        public ObservableCollection<PageModel> Pages
        {
            get { return (ObservableCollection<PageModel>)GetValue(PagesProperty); }
            set { SetValue(PagesProperty, value); }
        }

        public String Color
        {
            get { return ((String)GetValue(ColorProperty)); }
            set { SetValue(ColorProperty, value); }
        }

        public bool IsVisible
        {
            get { return ((bool)GetValue(IsVisibleProperty)); }
            set { SetValue(IsVisibleProperty, BooleanBoxes.Box(value)); }
        }

        public Boolean IsDefault { get; set; }

        public Boolean IsContext { get; set; }
        public Type Typo { get; set; }

        public CategoryModel()
        {
            Pages = new ObservableCollection<PageModel>();
            Color = "Orange";
            IsDefault = true;
        }
        public void Clear()
        {
            foreach (PageModel cat in Pages)
            {
                cat.Clear();
            }
            Pages.Clear();
        }
    }

    public class ContextCategoryModel : CategoryModel
    {
    }
}
 * */
