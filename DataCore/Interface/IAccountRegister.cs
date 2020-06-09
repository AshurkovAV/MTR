using System.Collections.Generic;
using System.Xml.Serialization;

namespace Medical.DataCore.Interface
{
    public interface IAccountRegister
    {
        IHeaderD InnerHeader { get; set; }
        IAccountD InnerAccount { get; set; }
        List<IRecordsCollectionD> InnerRecordCollection { get; set; }
    }
  
}