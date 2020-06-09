using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight;

namespace MedicineNext.Internal.MVVM
{
    public class CommandModel : ViewModelBase
    {
        private string _caption;
        private string _toolTip;
        private ICommand _command;
        private object _commandParameter;
        private ImageSource _largeGlyph;
        private ImageSource _smallGlyph;

        public string Id { get; set; }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; RaisePropertyChanged(()=>Caption); }
        }

        public string ToolTip
        {
            get { return _toolTip; }
            set { _toolTip = value; RaisePropertyChanged(()=>ToolTip); }
        }

        public ICommand Command
        {
            get { return _command; }
            set { _command = value; RaisePropertyChanged(()=>Command); }
        }

        public object CommandParameter
        {
            get { return _commandParameter; }
            set { _commandParameter = value; RaisePropertyChanged(()=>CommandParameter); }
        }
        public ImageSource LargeGlyph
        {
            get { return _largeGlyph; }
            set
            {
                _largeGlyph = value; RaisePropertyChanged(()=>LargeGlyph);
            }
        }
        public ImageSource SmallGlyph
        {
            get { return _smallGlyph; }
            set
            {
                _smallGlyph = value; RaisePropertyChanged(() => SmallGlyph);
            }
        }
    }
    /*public class CommandModel : DependencyObject
    {
        //private Action action;
        public static readonly DependencyProperty CommandProperty;
        public static readonly DependencyProperty CommandParameterProperty;
        public static readonly DependencyProperty CaptionProperty;
        public static readonly DependencyProperty ToolTipProperty;
        public static readonly DependencyProperty LargeGlyphProperty;
        public static readonly DependencyProperty SmallGlyphProperty;

        public string Id { get; set; }

        static CommandModel()
        {
            CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(CommandModel), new PropertyMetadata(""));
            ToolTipProperty = DependencyProperty.Register("ToolTip", typeof(string), typeof(CommandModel), new PropertyMetadata(""));
            CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandModel), new PropertyMetadata(null));
            CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandModel), new PropertyMetadata(null));
            LargeGlyphProperty = DependencyProperty.Register("LargeGlyph", typeof(ImageSource), typeof(CommandModel), new PropertyMetadata(null, new PropertyChangedCallback(OnGlyphPropertyChanged)));
            SmallGlyphProperty = DependencyProperty.Register("SmallGlyph", typeof(ImageSource), typeof(CommandModel), new PropertyMetadata(null, new PropertyChangedCallback(OnGlyphPropertyChanged)));
        }
        public CommandModel()
        {
        }

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public string ToolTip
        {
            get { return (string)GetValue(ToolTipProperty); }
            set { SetValue(ToolTipProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        public ImageSource LargeGlyph
        {
            get { return (ImageSource)GetValue(LargeGlyphProperty); }
            set
            {
                SetValue(LargeGlyphProperty, value);
            }
        }
        public ImageSource SmallGlyph
        {
            get { return (ImageSource)GetValue(SmallGlyphProperty); }
            set { SetValue(SmallGlyphProperty, value); }
        }

        protected internal static void OnGlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CommandModel)d).OnGlyphChanged(e);
        }
        protected internal void OnGlyphChanged(DependencyPropertyChangedEventArgs e)
        {
            ((ImageSource)e.NewValue).Freeze();
        }
    }  */
}
