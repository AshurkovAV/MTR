using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;

namespace Core.Infrastructure
{
    public class OperationResult<T> : OperationResult, IOperationResult<T>
    {
        public T Data { get; set; }

        public new static OperationResult<T> Create()
        {
            return new OperationResult<T>();
        }

        public static OperationResult<T> Create(T value)
        {
            var result = new OperationResult<T>
            {
                Data = value
            };
            return result;
        }
    }

    public class OperationResult<T1, T2> : OperationResult, IOperationResult<T1,T2>
    {
        public T1 Data1 { get; set; }
        public T2 Data2 { get; set; }

        public new static OperationResult<T1, T2> Create()
        {
            return new OperationResult<T1, T2>();
        } 
    }

    public class OperationResult : IOperationResult, IError, IErrorMessage
    {
        public int Id { get; set; }
        public string Log { get; set; }
        public bool IsCanceled { get; set; }
        public bool HasError
        {
            get { return Errors.Count > 0; }
        }

        public bool Success
        {
            get { return Errors.Count == 0 && ErrorMessage.IsNullOrWhiteSpace() && !IsCanceled; }
        }

        public IList<object> Errors { get; private set; }
        public Exception LastError
        {
            get
            {
                return HasError ? Errors.LastOrDefault() as Exception : null;
            }
        }

        public OperationResult()
        {
            Errors = new List<object>();
        }
        public void AddError(object error)
        {
            if (error is string)
            {
                Errors.Add(new Exception(error as string));
                ErrorMessage = error as string;
            }else if (error is Exception)
            {
                Errors.Add(error);
                ErrorMessage = error.ToString();
            }
            else
            {
                throw new Exception("Используйте в качестве ошибки исключение или строку.");
            }
            
        }

        public string ErrorMessage { get; private set; }

        public static OperationResult Create()
        {
            return new OperationResult();
        } 
    }

}
