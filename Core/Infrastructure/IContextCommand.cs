using System.Windows.Input;

namespace Core.Infrastructure
{
    public interface IContextCommand
    {
        string Id { get; set; }
        ICommand Command { get; set; }
        object CommandParameter { get; set; }
        string Caption { get; set; }
        string LargeGlyph { get; set; }
        string SmallGlyph { get; set; }
        string Page { get; set; }
        bool IsParent { get; set; }
        string Parent { get; set; }
        int Role { get; set; }
    }
}
