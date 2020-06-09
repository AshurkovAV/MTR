using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Autofac;
using Core.Helpers;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;
using Medical.DataCore.Interface;

namespace Medical.DataCore.v30.E
{
    public class SlKoefE: ISlKoef
    {
        [XmlElement(ElementName = "IDSL")]
        public int? NumberDifficultyTreatment { get; set; }
        [XmlElement(ElementName = "Z_SL")]
        public string ValueDifficultyTreatmentXml
        {
            get
            {
                return ValueDifficultyTreatment.HasValue ? ValueDifficultyTreatment.Value.ToString("F2", CultureInfo.InvariantCulture) : null;
            }
            set { ValueDifficultyTreatment = SafeConvert.ToDecimal(value); }
        }

        [XmlIgnore]
        public decimal? ValueDifficultyTreatment { get; set; }

    }
}
