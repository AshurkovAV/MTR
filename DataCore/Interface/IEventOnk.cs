using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.DataCore.Interface
{
    public interface IEventOnk
    {
        List<IDiagBlokOnk> InnerDiagBlokOnkCollection { get; set; }
        List<IContraindicationsOnk> InnerСontraindicationsOnkCollection { get; set; }
        List<IServiceOnk> InnerServiceOnkCollection { get; set; }
    }
}
