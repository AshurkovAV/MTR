using System;
using BLToolkit.Mapping;

namespace Medical.AppLayer.Models
{
    public class MedicalEventCompleted
    {
        [MapField("LPU")]
        public string MedicalOrganizationCode { get; set; }
        [MapField("PROFIL")]
        public int? Profile { get; set; }

        [MapField("ED_COL")]
        public int? IndicatorValue { get; set; }

        [MapField("SUMP")]
        public decimal? IndicatorPrice { get; set; }

        [MapField("SUMV")]
        public decimal? Price { get; set; }

        [MapField("SMO")]
        public int? Insurance { get; set; }

        [MapField("SMO_OK")]
        public string InsuranceOkato { get; set; }
        
        [MapField("W")]
        public int? Sex { get; set; }

        [MapField("VZST")]
        public int? Age { get; set; }

        [MapField("IDCASE")]
        public int? ExternalId { get; set; }

        [MapField("USL_OK")]
        public int? AssistanceConditions { get; set; }

        [MapField("VIDPOM")]
        public int? AssistanceType { get; set; }

        [MapField("SLUCH_TYPE")]
        public int? EventType { get; set; }

        [MapField("DATE_1")]
        public DateTime? EventBegin { get; set; }

        [MapField("DATE_2")]
        public DateTime? EventEnd { get; set; }
    }
}