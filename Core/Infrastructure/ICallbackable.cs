using System;

namespace Core.Infrastructure
{
    public interface ICallbackable
    {
        Action OkCallback { get; set; }
        Action CancelCallback { get; set; }
        Action CreateCallback { get; set; }
    }
}