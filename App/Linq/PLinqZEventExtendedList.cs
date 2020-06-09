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
    public class PLinqZEventExtendedList : IListSource
    {
        private static object Locker = new object();
        protected List<ZslEventShortView> Data;
        private readonly Expression<Func<ZslEventShortView, bool>> _predicate;
        private readonly IMedicineRepository _repository;

        public PLinqZEventExtendedList(IMedicineRepository repository, Expression<Func<ZslEventShortView, bool>> predicate = null)
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

        public IQueryable<ZslEventShortView> GetAsQueryable()
        {
            return Data.AsQueryable();
        }

        public void GenerateOrders()
        {
            var result = _repository.GetZslEventShortView(_predicate);
            if (result.Success)
            {
                Data = new List<ZslEventShortView>(result.Data);
            }
        }

        
    }
}
