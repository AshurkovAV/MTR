using System;
using System.Collections.Generic;

namespace Core.Infrastructure
{
    public interface IError
    {
        bool HasError { get;}
        bool Success { get; }
        IList<object> Errors { get; }
        Exception LastError { get; }
        void AddError(object error);
    }
}