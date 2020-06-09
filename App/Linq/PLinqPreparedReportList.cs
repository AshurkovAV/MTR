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
    public class PLinqPreparedReportList : IListSource
    {
        private static object Locker = new object();
        protected List<FactPreparedReport> Data;

        private readonly Expression<Func<FactPreparedReport, bool>> _predicate;
        private readonly IMedicineRepository _repository;

        public PLinqPreparedReportList(IMedicineRepository repository, Expression<Func<FactPreparedReport, bool>> predicate = null)
        {
            _predicate = predicate;
            _repository = repository;
        }

        #region IListSource Members

        public IList GetList()
        {
            if (Data == null)
            {
                lock (Locker)
                {
                    if (Data == null)
                    {
                        GenerateOrders();
                    }
                }
            }
            return Data;
        }

        public bool ContainsListCollection
        {
            get { return false; }
        }

        #endregion

        public IQueryable<FactPreparedReport> GetAsQueryable()
        {
            return Data.AsQueryable();
        }

        public void GenerateOrders()
        {
            var result = _repository.GetFactPreparedReport(_predicate);
            if (result.Success)
            {
                Data = new List<FactPreparedReport>(result.Data);
            }
        }
    }
}
