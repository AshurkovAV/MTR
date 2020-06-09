using System.Collections.Generic;
using System.Xml.Serialization;

namespace Medical.DataCore.Interface
{
    public interface IZslEvent
    {
        List<IZEvent> InnerMeventCollection { get; set; }

        decimal? RefusalPrice { get; set; }
        decimal? Price { get; set; }
        string MedicalOrganizationCode { get; set; }
        string ReferralOrganizationNullable { get; set; }
        List<IRefusal> InnerRefusals { get; set; }
    }
}