using System.Collections.Generic;

namespace Medical.DataCore.Interface
{
    public interface IZRecordAnswer
    {
        int ExternalId { get; set; }
        IPatientAnswer InnerPatient { get; set; }
        List<IZslMeventAnswer> InnerEventCollection { get; set; }
    }
}