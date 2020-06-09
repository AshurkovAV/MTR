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
    public class PLinqEconomicJournalList : IListSource
    {
        private static object Locker = new object();
        protected List<EconomicJournalCustomModel> Data;
         private readonly Expression<Func<FactEconomicAccount, bool>> _predicate;
        private readonly IMedicineRepository _repository;

        public PLinqEconomicJournalList(IMedicineRepository repository, Expression<Func<FactEconomicAccount, bool>> predicate = null)
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

        public IQueryable<EconomicJournalCustomModel> GetAsQueryable()
        {
            return Data.AsQueryable();
        }

        public bool IsEmpty()
        {
            return Data != null && Data.Any();
        }

        public void GenerateOrders()
        {
            var result = _repository.GetEconomicJournal(_predicate);
            if (result.Success)
            {
                Data = new List<EconomicJournalCustomModel>(result.Data.Select(p => new EconomicJournalCustomModel
                {
                    TerritoryAccount = p.Item1,
                    Account = p.Item2,
                    TotalAmountPayable = p.Item3,
                    TotalAmountRefuse = p.Item4,
                    AssistanceDetailsSum = new Dictionary<int, AssistanceDetailsSums>(p.Item5.ToDictionary(pair => pair.Key, pair => new AssistanceDetailsSums
                    {
                        Assistance = pair.Value.Item1, 
	                    AmountPayable= pair.Value.Item2,
                        AmountFact =  pair.Value.Item3,
                        AmountRefuse =  pair.Value.Item4,
	                    AmountDebt= pair.Value.Item5,
                    }))
                }));
            }
            

           
        }
    }
}
