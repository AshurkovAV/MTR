using System.Collections.Generic;

namespace Medical.DataCore.Interface
{
    public interface IKsgKpg
    {
        List<ISlKoef> InnerSlCoefCollection { get; set; }
        List<string> CritXml { get; set; }
        string Kkpg { get; set; }
    }
}