using System;
using System.Globalization;
using System.Windows.Data;
using Autofac;
using Core.Extensions;
using Core.Services;
using DevExpress.Data;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.AppLayer.Converters
{
    public class RoleConverter : IValueConverter
    {
        private static readonly ICacheRepository Repository;

        static RoleConverter()
        {
            if (Di.I.IsRegistered<ICacheRepository>())
            {
                Repository = Di.I.Resolve<ICacheRepository>();
            }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            if (value is NotLoadedObject)
            {
                return null;
            }

            if (Repository.IsNotNull())
            {
                return Repository.Get(CacheRepository.RoleCache).GetString(value.ToInt32Nullable());
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}