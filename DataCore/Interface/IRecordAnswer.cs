using System.Collections.Generic;

namespace Medical.DataCore.Interface
{
    public interface IRecordAnswer
    {
        int ExternalId { get; set; }
        IPatientAnswer InnerPatient { get; set; }
        List<IMEventAnswer> InnerEventCollection { get; set; }
    }
}