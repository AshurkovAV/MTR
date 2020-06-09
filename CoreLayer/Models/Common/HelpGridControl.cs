using System.Linq;
using DevExpress.Xpf.Grid;

namespace Medical.CoreLayer.Models.Common
{
    public static class HelpGridControl
    {
        static public object GetSelectedGridRow(GridControl gridControl)
        {
            var handles = gridControl.GetSelectedRowHandles();
            if (handles != null && handles.Any())
            {
                foreach (var handle in handles)
                {
                    if (handle >= 0)
                    {
                        var row = (DevExpress.Data.Async.Helpers.ReadonlyThreadSafeProxyForObjectFromAnotherThread)gridControl.GetRow(handle);
                        return row.OriginalRow;
                    }
                }
            }
            return null;
        }

        static public object GetAnonymousValue(object obj, string field)
        {
            return obj.GetType().GetProperty(field)?.GetValue(obj, null);
        }
    }
}
