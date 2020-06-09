using System.Collections.Generic;
using System.Linq;
using Medical.AppLayer.Models.ClassifierModels;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq
{
    public class PLinqDoctorList : PLinqListBase<EditShareDoctor>
    {
        public PLinqDoctorList(IMedicineRepository repository)
            : base(repository, null)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetShareDoctor();
            if (result.Success)
            {
                Data = new List<EditShareDoctor>(result.Data.Select(p=>new EditShareDoctor(p)));
            }
        }
    }
}
