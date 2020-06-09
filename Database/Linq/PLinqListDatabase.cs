using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Medical.DatabaseCore.Linq
{
    public class PLinqListDatabase<T> : IListSource where T : class
    {
        private static object Locker = new object();
        protected List<T> Data;
        public Expression<Func<T, bool>> Predicate { get; set; }

        public PLinqListDatabase()
        {
        }

        public PLinqListDatabase(Expression<Func<T, bool>> predicate)
        {
            Predicate = predicate;
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

        public IQueryable<T> GetAsQueryable()
        {
            return Data.AsQueryable();
        }

        public void GenerateOrders()
        {
            //TODO
            Data = null;
                //Predicate != null ? new List<T>(UnityService.Instance.Resolve<DatabaseRepository>().Ge<T>().Where(Predicate)) : new List<T>(UnityService.Instance.Resolve<DatabaseRepository>().GetTable<T>());
        }
    }
}
