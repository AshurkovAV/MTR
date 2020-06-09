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
    public class PLinqEconomicSurchargeList : IListSource
    {
        private static object Locker = new object();
        protected List<EconomicSurchargeCustomModel> Data;

        private readonly Expression<Func<FactEconomicSurcharge, bool>> _predicate;
        private readonly IMedicineRepository _repository;

        public PLinqEconomicSurchargeList(IMedicineRepository repository, Expression<Func<FactEconomicSurcharge, bool>> predicate = null)
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
                    GenerateOrders();
                }
            }
            return Data;
        }

        public bool ContainsListCollection
        {
            get { return false; }
        }

        #endregion

        public Expression<Func<FactEconomicSurcharge, bool>> Predicate
        {
            get { return _predicate; }
        }

        public IQueryable<EconomicSurchargeCustomModel> GetAsQueryable()
        {
            return Data.AsQueryable();
        }

        public void GenerateOrders()
        {
            var result = _repository.GetEconomicSurcharge(Predicate);
            if (result.Success)
            {
                Data = new List<EconomicSurchargeCustomModel>(result.Data.Select(p => new EconomicSurchargeCustomModel
                {
                    Surcharge = p.Item1,
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
