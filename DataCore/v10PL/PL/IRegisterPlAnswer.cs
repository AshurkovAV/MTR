using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v10PL.PL
{
    public interface IRegisterPlAnswer
    {
        string FileName { get; set; }
        InformationPlAnswer InnerAccount { get; set; }
      
    }
}
