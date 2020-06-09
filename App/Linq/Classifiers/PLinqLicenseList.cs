using System.Collections.Generic;
using System.Linq;
using DataModel;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Classifiers
{
    public class PLinqLicenseList : PLinqListBase<EditLicense>
    {
        public PLinqLicenseList(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetAll<globalLicense>();
            if (result.Success)
            {
                var licenses = result.Data.ToList();
                for (int i = 0; i < licenses.Count(); i++)
                {
                    var resultDetails = Repository.GetLicenseEntryById(licenses[i].LicenseId);
                    if (resultDetails.Success)
                    {
                        licenses[i].dbo_globalLicenseEntrydbo_globalLicenseLicenseLicenseIds = resultDetails.Data;
                    }
                }

                Data = new List<EditLicense>(licenses.Select(p => new EditLicense(p)));
            }
        }
    }
}
