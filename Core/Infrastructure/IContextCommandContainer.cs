using System.Collections.ObjectModel;

namespace Core.Infrastructure
{
    public interface IContextCommandContainer
    {
        ObservableCollection<IContextCommand> ContextCommands { get; set; }
        string PageName { get; set; }
    }
}
