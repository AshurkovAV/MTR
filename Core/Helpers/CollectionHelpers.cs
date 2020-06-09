using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.Helpers
{
    public static class CollectionHelpers
    {
        public static ObservableCollection<T> ToObservableCollection<T>(IEnumerable<T> coll)
        {
            var c = new ObservableCollection<T>();
            foreach (var e in coll) c.Add(e);
            return c;
        }


    }
}
