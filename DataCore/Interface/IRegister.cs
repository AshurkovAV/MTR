using System.Collections.Generic;

namespace Medical.DataCore.Interface
{
    public interface IRegister
    {
        List<IRecord> InnerRecordCollection { get; set; }
        IAccount InnerAccount { get; set; }
        IHeader InnerHeader { get; set; }
    }
}