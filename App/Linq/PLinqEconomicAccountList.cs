using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using DataModel;
using Medical.AppLayer.Economic.Models;
using Medical.AppLayer.Economic.Models.Helpers;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq
{
    public class PLinqEconomicAccountList : IListSource
    {
        private static object Locker = new object();
        protected List<EconomicAccountCustomModel> Data;

        private readonly Expression<Func<FactEconomicAccount, bool>> _predicate;
        private readonly IMedicineRepository _repository;

        public PLinqEconomicAccountList(IMedicineRepository repository, Expression<Func<FactEconomicAccount, bool>> predicate = null)
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

        public Expression<Func<FactEconomicAccount, bool>> Predicate
        {
            get { return _predicate; }
        }

        public IQueryable<EconomicAccountCustomModel> GetAsQueryable()
        {
            return Data.AsQueryable();
        }

       public void GenerateData()
        {
            var result = _repository.GetEconomicAccount(_predicate);
            if (result.Success)
            {
                Data = new List<EconomicAccountCustomModel>(result.Data.Select(p=>new EconomicAccountCustomModel
                {
                   // Direction = p.Item2.Direction,
                    Account = p.Item1,
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
