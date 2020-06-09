using System;
using System.Globalization;
using System.Windows.Data;
using Autofac;
using Core.Extensions;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.AppLayer.Converters
{
    public class MedicalOrganizationConverter : IValueConverter
    {
        private static readonly ICacheRepository Repository;

        static MedicalOrganizationConverter()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                if (scope.IsRegistered<ICacheRepository>())
                {
                    Repository = scope.Resolve<ICacheRepository>();
                }
                
            }
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            
            switch (value.ToString().Length)
            {
                case 1:
                case 2:
                    return Repository.Get(CacheRepository.F010TfCodeToNameCache).GetString(value.ToInt32Nullable());
                case 5:
                    //throw new NotImplementedException("для страховых компаний конвертер не реализован!");
                case 6:
                    return value;//Repository.Get(CacheRepository.F003MCodeToNameCache).GetString(value.ToInt32Nullable());
                default:
                    return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}