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
    public class PLinqEventDetailList: IListSource
    {
        private static object Locker = new object();
        protected List<EventShortView> Data;
        private readonly Expression<Func<EventShortView, bool>> _predicate;
        private readonly IMedicineRepository _repository;

        public PLinqEventDetailList(IMedicineRepository repository, Expression<Func<EventShortView, bool>> predicate = null)
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

        public IQueryable<EventShortView> GetAsQueryable()
        {
            return Data.AsQueryable();
        }

        public void GenerateOrders()
        {
            var result = _repository.GetEventShortView(_predicate);
            if (result.Success)
            {
                Data = new List<EventShortView>(result.Data);
            }
        }
    }
}
