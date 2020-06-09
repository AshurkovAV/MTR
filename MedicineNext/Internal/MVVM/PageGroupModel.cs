using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace MedicineNext.Internal.MVVM
{
    public class PageGroupModel : ViewModelBase
    {
        public string Name { get; set; }

        public ObservableCollection<CommandModel> Commands { get; set; }

        public PageGroupModel()
        {
            Commands = new ObservableCollection<CommandModel>();
        }

        public void Clear()
        {
            Commands.Clear();
        }
    }

    /*public class PageGroupModel: ModelBase 
    {
        public static readonly DependencyProperty CommandsProperty;
        public static readonly DependencyProperty GlyphProperty;

        static PageGroupModel()
        {
            CommandsProperty = DependencyProperty.Register("Commands", typeof(ObservableCollection<CommandModel>), typeof(PageGroupModel), new PropertyMetadata(null));
            GlyphProperty = DependencyProperty.Register("Glyph", typeof(ImageSource), typeof(PageGroupModel), new PropertyMetadata(null, new PropertyChangedCallback(OnGlyphPropertyChanged)));
        }
        public PageGroupModel()
        {
            Commands = new ObservableCollection<CommandModel>();
        }

        public ObservableCollection<CommandModel> Commands
        {
            get { return ((ObservableCollection<CommandModel>)GetValue(CommandsProperty)); }
            set { SetValue(CommandsProperty, value); }
        }
        public ImageSource Glyph
        {
            get { return (ImageSource)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }
        protected internal static void OnGlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PageGroupModel)d).OnGlyphChanged(e);
        }
        protected internal void OnGlyphChanged(DependencyPropertyChangedEventArgs e)
        {
            ((ImageSource)e.NewValue).Freeze();
        }
        public void Clear()
        {
            Commands.Clear();
        }
        #region ICommand
        bool b = false;
        public bool CanExecute(object parameter)
        {
            if (b == true) CanExecuteChanged.Invoke(this, new EventArgs());
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            MessageBox.Show(Name + "'s command executed");
        }
        #endregion
    }*/
}
