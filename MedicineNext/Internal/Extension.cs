using Core.Infrastructure;

namespace MedicineNext.Internal
{

    public static class Extension
    {
        public static string Hash(this WorkspaceViewModel model)
        {
            if (model.DataContext is IHash)
            {
                return (model.DataContext as IHash).Hash;
            }
            return string.Empty;
        }
    }
}
