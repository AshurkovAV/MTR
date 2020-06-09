using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Linq.Log
{
    public class PLinqMedicineLogBase<T> : IListSource
    {
        private static object Locker = new object();
        protected List<T> Data;

        private readonly Expression<Func<T, bool>> _predicate;
        private readonly IMedicineLogRepository _repository;

        public PLinqMedicineLogBase(IMedicineLogRepository repository, Expression<Func<T, bool>> predicate = null)
        {
            _repository = repository;
            _predicate = predicate;
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

        public Expression<Func<T, bool>> Predicate
        {
            get { return _predicate; }
        }

        public IMedicineLogRepository Repository
        {
            get { return _repository; }
        }

        public IQueryable<T> GetAsQueryable()
        {
            return Data != null ? Data.AsQueryable() : null;
        }

        public virtual void GenerateData()
        {
            throw new Exception("GenerateData must override!");
        }
    }
}
