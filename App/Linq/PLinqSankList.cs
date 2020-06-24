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
    public class PLinqSankList : IListSource
    {
        private static object Locker = new object();
        protected List<SankShortView> Data;

        private readonly Expression<Func<SankShortView, bool>> _predicate;
        private readonly IMedicineRepository _repository;

        public PLinqSankList(IMedicineRepository repository, Expression<Func<SankShortView, bool>> predicate = null)
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

        public Expression<Func<SankShortView, bool>> Predicate
        {
            get { return _predicate; }
        }

        public IQueryable<SankShortView> GetAsQueryable()
        {
            return Data.AsQueryable();
        }

       public void GenerateData()
        {
            var result = _repository.GetSankShortView(_predicate);
            if (result.Success)
            {
                Data = new List<SankShortView>(result.Data);
            }
        }
    }
}
