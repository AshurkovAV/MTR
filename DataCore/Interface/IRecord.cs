using System.Collections.Generic;

namespace Medical.DataCore.Interface
{
    public interface IRecord
    {
        int ExternalId { get; set; }
        IPatient InnerPatient { get; set; }
        List<IMEvent> InnerEventCollection { get; set; }
    }
}