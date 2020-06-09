using System.Collections.Generic;
using System.Linq;

namespace Medical.DatabaseCore.Services.Cache
{
    public class TortilaPolicyTypeCache : DictionaryCache
    {
        public TortilaPolicyTypeCache()
        {
            BackDictionary = new Dictionary<object, int?>
            {
                {"no", 0},
                {"С", 1},
                {"В", 2},
                {"П", 3}
            };
            Dictionary = BackDictionary.ToDictionary(x => x.Value, x => x.Key);
        }
    }
}
