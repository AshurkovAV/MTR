using System;

namespace Core.Infrastructure
{
    public interface IDataConverter<out T1, out T2>
    {
        T1 Convert(object value, Type targetType, object parameter);
        T2 ConvertBack(object value, Type targetType, object parameter);
    }
}