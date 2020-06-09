using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.DataCore.v10PL.PL
{
    public interface IAccountPlAnswer
    {
        string AccountNumber { get; set; }
        DateTime? AccountDate { get; set; }
        string NameIshodReestr { get; set; }
        decimal? SuumAccount { get; set; }
        int? NumberSlAccount { get; set; } 
    }
}
