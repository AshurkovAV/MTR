using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Medical.AppLayer.Economic.Models;
using Medical.AppLayer.Economic.Models.Helpers;
using Medical.DatabaseCore.Services.Database;
using DataModel;

namespace Medical.AppLayer.Linq
{
    public class PLinqEconomicRefuseList : IListSource
    {
        private static object Locker = new object();
        protected List<EconomicRefuseCustomModel> Data;

        private readonly Expression<Func<FactEconomicRefuse, bool>> _predicate;
        private readonly IMedicineRepository _repository;

        public PLinqEconomicRefuseList(IMedicineRepository repository, 
            Expression<Func<FactEconomicRefuse, bool>> predicate = null)
        {
            _predicate = predicate;
            _repository = repository;
        }

        #region IListSource Members

        public IList GetList()
        {
            if (Data != null) return Data;
            lock (Locker)
            {
                if (Data == null)
                {
                    GenerateData();
                }
            }
            return Data;
        }

        public bool ContainsListCollection
        {
            get { return false; }
        }

        #endregion

        public Expression<Func<FactEconomicRefuse, bool>> Predicate
        {
            get { return _predicate; }
        }

        public IQueryable<EconomicRefuseCustomModel> GetAsQueryable()
        {
            return Data.AsQueryable();
        }

        public void GenerateData()
        {
            var result = _repository.GetEconomicRefuse(Predicate);
            if (result.Success)
            {
                Data = new List<EconomicRefuseCustomModel>(result.Data.Select(p => new EconomicRefuseCustomModel
                {
                    Refuse = p.Item1,
                    AssistanceSum = new Dictionary<int, AssistanceSums>(p.Item2.ToDictionary(pair => pair.Key, pair => new AssistanceSums
                    {
                        Assistance = pair.Value.Item1,
                        Sum = pair.Value.Item2
                    }))
                }));
            }
        }
    }
}
