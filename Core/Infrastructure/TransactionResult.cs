using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Infrastructure
{
    public class TransactionResult <T> : TransactionResult, ITransactionResult<T>
    {
        public T Data { get; set; }
    }

    public class TransactionResult : ITransactionResult, IError
    {
        public int Id { get; set; }
        public bool HasError
        {
            get { return Errors.Count > 0; }
        }

        public bool Success
        {
            get { return Errors.Count == 0; }
        }

        public IList<object> Errors { get; private set; }
        public Exception LastError
        {
            get
            {
                return HasError ? Errors.LastOrDefault() as Exception : null;
            }
        }

        public TransactionResult()
        {
            Errors = new List<object>();
        }
        public void AddError(object error)
        {
            Errors.Add(error);
        }
    }
}
