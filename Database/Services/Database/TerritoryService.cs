namespace Medical.DatabaseCore.Services.Database
{
    public class TerritoryService
    {
        public static IAppRemoteSettings _remoteSettings;
        private static dynamic _tfCode;
        private static dynamic _terrCode;

        public TerritoryService()
        {
          //  _remoteSettings = remoteSettings;
        }

        /// <summary>
        ///  {"tf_code":46,}
        /// </summary>
        public static int? TfCode
        {
            get
            {
                _tfCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
                return (int?) _tfCode.tf_code;
            }
        }
        /// <summary>
        /// "tf_okato":"38000"
        /// </summary>
        public static string TerritoryOkato
        {
            get
            {
                _terrCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
                return (string) _terrCode.tf_okato;
            }
        }
        /// <summary>
        /// Версия формата oms по умолчанию
        /// </summary>
        public static int TerritoryDefaultOmsVersion
        {
            get
            {
                _terrCode = _remoteSettings.Get(AppRemoteSettings.DefaultOmsVersion);
                return (int)_terrCode.version;
            }
        }
    }
}
