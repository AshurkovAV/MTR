using System.Collections.Generic;
using System.Xml.Serialization;

namespace Medical.DataCore.Interface
{
    public interface IZEvent
    {
        List<IService> InnerServiceCollection { get; set; }
        List<object> InnerRefusals { get; set; }
        string SpecialityClassifierVersion { get; set; }
        IKsgKpg InnerKsgKpg { get; set; }

        int? SpecialityCodeV021 { get; set; }
        string ExternalId { get; set; }
        List<IDirectionOnkE> InnerDirectionOnkCollection { get; set; }
        List<IConsultationsOnk> InnerConsultationsOnkCollection { get; set; }
        IEventOnk InnerEventOnk { get; set; }
        List<string> DiagnosisSecondaryXml { get; set; }
    }
}