using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.AppLayer.Services
{
    public class RegionService
    {
        /// <summary>
        /// Маппинг кодов лпу для еврейской области
        /// </summary>
        /// <param name="medicalOrganizationCode"></param>
        /// <returns></returns>
        public string LpuEaoMapping(string medicalOrganizationCode)
        {
            string result;
            try
            {
                string posmedCode = String.Empty;
                if (medicalOrganizationCode.Length > 5)
                {
                    posmedCode = medicalOrganizationCode.Substring(2, 1);
                }
                if (posmedCode != "0")
                {
                    medicalOrganizationCode = medicalOrganizationCode.Substring(0, 2) + "0" +
                                              medicalOrganizationCode.Substring(3, 3);
                }

                switch (medicalOrganizationCode)
                {
                    case "790015":
                        result = "790013";
                        break;
                    case "790020":
                        result = "790013";
                        break;
                    case "790029":
                        result = "790013";
                        break;
                    case "790017":
                        result = "790040";
                        break;
                    default:
                        result = medicalOrganizationCode;
                        break;
                }
            }
            catch (Exception)
            {
                result = medicalOrganizationCode;
            }

            return result;
        }
    }
}
