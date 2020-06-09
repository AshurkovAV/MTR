using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Infrastructure
{
    public class MappingResult<T>
    {
        private ICollection<string> _affected;
        public T Data { get; set; }
        public ICollection<string> Affected
        {
            get { return _affected ?? (_affected = new List<string>()); }
            set { _affected = value; }
        }
    }
}
