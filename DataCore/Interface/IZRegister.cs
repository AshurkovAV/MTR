using System.Collections.Generic;

namespace Medical.DataCore.Interface
{
    public interface IZRegister
    {
        List<IZRecord> InnerRecordCollection { get; set; }
        IAccount InnerAccount { get; set; }
        IHeader InnerHeader { get; set; }
    }
}