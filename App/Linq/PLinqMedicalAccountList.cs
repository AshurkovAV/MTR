using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq
{
    public class PLinqMedicalAccountList : PLinqListBase<MedicalAccountView>
    {
        public PLinqMedicalAccountList(IMedicineRepository repository, Expression<Func<MedicalAccountView, bool>> predicate = null)
            : base(repository, predicate)
        {
        }

        public override void GenerateData()
        {
            var result = Repository.GetMedicalAccountView(Predicate);
            if (result.Success)
            {
                Data = new List<MedicalAccountView>(result.Data);
            }
        }
    }
}
