using System;
using System.Collections.Generic;
using BLToolkit.Data.DataProvider;

namespace Medical.CoreLayer.Helpers
{
    public static class DatabaseHelpers
    {
        private static readonly Dictionary<string, Type> DataProvidersHandlers = new Dictionary<string, Type>
        {
            {ProviderName.MsSql, typeof (SqlDataProvider)}
        };

        public static DataProviderBase ToDataProvider(this string provider)
        {
            return DataProvidersHandlers.ContainsKey(provider)
                ? (DataProviderBase) Activator.CreateInstance(DataProvidersHandlers[provider])
                : null;
        }
    }
}
