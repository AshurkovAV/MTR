using System.Collections.Generic;

namespace Medical.DataCore.Interface
{
    public interface IZRegisterAnswer
    {
        List<IZRecordAnswer> InnerRecordCollection { get; set; }
        IAccountAnswer InnerAccount { get; set; }
        IHeaderAnswer InnerHeader { get; set; }
    }
}