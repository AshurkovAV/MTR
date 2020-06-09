using System.Collections.Generic;

namespace Medical.DataCore.Interface
{
    public interface IZRecord
    {
        int ExternalId { get; set; }
        IPatient InnerPatient { get; set; }
        List<IZslEvent> InnerZslEventCollection { get; set; }
    }
}