using System;

namespace Medical.DataCore.Interface
{
    public interface IPatient
    {
        int? DocType { get; set; }
        string DocOrg { get; set; }
        DateTime? DocDate { get; set; }
        string DocSeries { get; set; }
        string DocNumber { get; set; }
        string INP { get; set; }
        string InsuranceDocNumber { get; set; }
        string InsuranceDocSeries { get; set; }
        int? InsuranceDocType { get; set; }
        string Newborn { get; set; }

        string PolicyNumber { get; set; }
        string AddressRegXml { get; set; }
        string AddressLiveXml { get; set; }
    }
}