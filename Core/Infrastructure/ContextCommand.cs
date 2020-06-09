using System.Windows.Input;

namespace Core.Infrastructure
{
    public class ContextCommand : IContextCommand
    {
        public string Id { get; set; }
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }
        public string Caption { get; set; }
        public string LargeGlyph { get; set; }
        public string SmallGlyph { get; set; }
        public string Page { get; set; }
        public bool IsParent { get; set; }
        public string Parent { get; set; }
        public int Role { get; set; }

        
    }
}