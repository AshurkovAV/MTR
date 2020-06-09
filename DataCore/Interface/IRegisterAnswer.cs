using System.Collections.Generic;

namespace Medical.DataCore.Interface
{
    public interface IRegisterAnswer
    {
        List<IRecordAnswer> InnerRecordCollection { get; set; }
        IAccountAnswer InnerAccount { get; set; }
        IHeaderAnswer InnerHeader { get; set; }
    }
}