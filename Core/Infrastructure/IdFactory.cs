using System.Collections.Generic;

namespace Core.Infrastructure
{
    public class IdFactory : IIdFactory
    {
        public const string WorkbenchViewsCategory = "WorkbenchViewsCategory";

        readonly Dictionary<string,int> _repository = new Dictionary<string, int>(); 
        public int CurrentId { get; set; }

        public int NextId(string category = null)
        {
            int id;
            if (category == null)
            {
                id = CurrentId;
                CurrentId += 1;
            }
            else
            {
                if (!_repository.ContainsKey(category))
                {
                    _repository[category] = 1;
                }

                id = _repository[category];
                _repository[category] += 1;
            }

            return id;
        }
    }
}
