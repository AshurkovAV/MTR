using System.Collections.ObjectModel;
using System.Windows;

namespace MedicineNext.Internal.MVVM
{
    public class MyGroupCommand : CommandModel
    {
        public ObservableCollection<CommandModel> Commands { get; set; }

        public MyGroupCommand()
        {
            Commands = new ObservableCollection<CommandModel>();
        }
    }

    /*public class MyGroupCommand : CommandModel
    {
        public static readonly DependencyProperty CommandsProperty;

        public ObservableCollection<CommandModel> Commands
        {
            get { return (ObservableCollection<CommandModel>)GetValue(CommandsProperty); }
            set { SetValue(CommandsProperty, value); }
        }
        static MyGroupCommand()
        {
            CommandsProperty = DependencyProperty.Register("Commands", typeof(ObservableCollection<CommandModel>), typeof(MyGroupCommand), new PropertyMetadata(null));
        }
        public MyGroupCommand()
            : base()
        {
            Commands = new ObservableCollection<CommandModel>();
        }
    }*/
}
