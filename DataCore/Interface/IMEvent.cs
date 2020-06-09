using System.Collections.Generic;
using System.Xml.Serialization;

namespace Medical.DataCore.Interface
{
    public interface IMEvent
    {
        List<IService> InnerServiceCollection { get; set; }
        List<object> InnerRefusals { get; set; }
        int?  SpecialityCode { get; set; }
        string SpecialityClassifierVersion { get; set; }
        int? SpecialityCodeV015 { get; set; }
        decimal? RefusalPrice { get; set; }
        decimal? Price { get; set; }
    }
}