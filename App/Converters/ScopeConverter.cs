﻿using System;
using System.Globalization;
using System.Windows.Data;
using Autofac;
using Core.Extensions;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.AppLayer.Converters
{
    public class ScopeConverter : IValueConverter
    {
        private static readonly ICacheRepository Repository;

        static ScopeConverter()
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

            if (Repository.IsNotNull())
            {
                return Repository.Get("ScopeCache").GetString(value.ToInt32Nullable());
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
