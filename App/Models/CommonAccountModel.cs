using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.AppLayer.Models
{
    public class CommonAccountModel
    {
        public string AccountNumber { get; set; }
        public DateTime? AccountDate { get; set; }
        public string Source { get; set; }

        public string MedicalOrganization { get; set; }
        public int? MedicalOrganizationId { get; set; }

    }
}
