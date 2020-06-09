using System.Collections.ObjectModel;
using System.Windows;
using Core.Perfomance;
using GalaSoft.MvvmLight;

namespace MedicineNext.Internal.MVVM
{
    public class PageModel : ViewModelBase
    {
        private bool _isSelected;
        public ObservableCollection<PageGroupModel> Groups { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }

        public PageModel()
        {
            Groups = new ObservableCollection<PageGroupModel>();
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; RaisePropertyChanged(() => IsSelected); }
        }

        public void Clear()
        {
            foreach (PageGroupModel cat in Groups)
            {
                cat.Clear();
            }
            Groups.Clear();
        }
    }

    /*public class PageModel : ModelBase
    {
        public static readonly DependencyProperty GroupsProperty;
        public static readonly DependencyProperty IsSelectedProperty;

        public string Id { get; set; }

        static PageModel()
        {
            GroupsProperty = DependencyProperty.Register("Groups", typeof(ObservableCollection<PageGroupModel>), typeof(PageModel), new PropertyMetadata(null));
            IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(PageModel), new PropertyMetadata(null));
        }
        public PageModel()
        {
            Groups = new ObservableCollection<PageGroupModel>();
        }
        public ObservableCollection<PageGroupModel> Groups
        {
            get { return ((ObservableCollection<PageGroupModel>)GetValue(GroupsProperty)); }
            set { SetValue(GroupsProperty, value); }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, BooleanBoxes.Box(value)); }
        }
        
        public void Clear()
        {
            foreach (PageGroupModel cat in Groups)
            {
                cat.Clear();
            }
            Groups.Clear();
        }
    }*/
}
