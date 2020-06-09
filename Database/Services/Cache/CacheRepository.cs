using System.Collections.Generic;
using System.Linq;
using Autofac;
using Core.Services;

namespace Medical.DatabaseCore.Services.Cache
{
    public class CacheRepository : ICacheRepository
    {
        public static string VersionCache = "VersionCache";

        public static string F002Cache = "F002Cache";
        public static string F003Cache = "F003Cache";
        public static string F004Cache = "F004Cache";
        public static string F005Cache = "F005Cache";
        public static string F006Cache = "F006Cache";
        public static string F008Cache = "F008Cache";
        public static string F010Cache = "F010Cache";
        public static string F011Cache = "F011Cache";
        public static string F010OkatoToNameCache = "F010OkatoToNameCache";
        public static string F010TfCodeToNameCache = "F010TfCodeToNameCache";
        public static string F003MCodeToNameCache = "F003MCodeToNameCache";
        public static string F002ToTfOkatoCache = "F002ToTfOkatoCache";
        public static string TortilaPolicyTypeCache = "TortilaPolicyTypeCache";
        public static string F014aCache = "F014aCache";
        public static string IDC10Cache = "IDC10Cache";
        public static string IDC10PayableCache = "IDC10PayableCache";
        public static string F002IdToNameCache = "F002IdToNameCache";
        public static string ShareDoctorToCodeCache = "ShareDoctorToCodeCache";
        public static string RoleCache = "RoleCache";
        public static string V004Cache = "V004Cache";
        public static string V015Cache = "V015Cache";
        public static string RefusalPenaltyCache = "RefusalPenaltyCache";
        public static string ExaminationTypeCache = "ExaminationTypeCache";
        public static string ExaminationGroupCache = "ExaminationGroupCache";
        public static string M001Cache = "M001Cache";
        public static string V002Cache = "V002Cache";
        public static string V005Cache = "V005Cache";
        public static string V006Cache = "V006Cache";
        public static string V008Cache = "V008Cache";
        public static string V012Cache = "V012Cache";
        public static string V010Cache = "V010Cache";
        public static string V014Cache = "V014Cache";
        public static string V020Cache = "V020Cache";
        public static string V021Cache = "V021Cache";
        public static string V023Cache = "V023Cache";
        public static string V024Cache = "V024Cache";
        public static string V025Cache = "V025Cache";
        public static string V026Cache = "V026Cache";
        public static string V027Cache = "V027Cache";
        public static string V028Cache = "V028Cache";
        public static string V029Cache = "V029Cache";
        public static string N001Cache = "N001Cache";
        public static string N002Cache = "N002Cache";
        public static string N003Cache = "N003Cache";
        public static string N004Cache = "N004Cache";
        public static string N007Cache = "N007Cache";
        public static string N010Cache = "N010Cache";
        public static string N008Cache = "N008Cache";
        public static string N011Cache = "N011Cache";
        public static string N013Cache = "N013Cache";
        public static string N014Cache = "N014Cache";
        public static string N015Cache = "N015Cache";
        public static string N016Cache = "N016Cache";
        public static string N017Cache = "N017Cache";
        public static string N018Cache = "N018Cache";
        public static string N019Cache = "N019Cache";
        public static string N020Cache = "N020Cache";
        public static string ConsultationOnkCache = "ConsultationOnkCache";
        public static string GlobalRegionalAttributeCache = "GlobalRegionalAttributeCache";


        protected static Dictionary<string, ICache> Caches = new Dictionary<string, ICache>();
        protected IEnumerable<string> CachesList = new List<string>
        {
            IDC10Cache,
            F002Cache,
            F003Cache,
            F004Cache,
            F005Cache,
            F006Cache,
            F008Cache,
            F010Cache,
            F011Cache,
            "F014Cache",
            VersionCache,
            "F014aCache",
            "V009Cache",
            F010OkatoToNameCache,
            F010TfCodeToNameCache,
            F003MCodeToNameCache,
            "ParamCache",
            "ScopeCache",
            "PaymentStatusCache",
            "AccountStatusCache",
            "AccountTypeCache",
            F002ToTfOkatoCache,
            TortilaPolicyTypeCache,
            F002IdToNameCache,
            ShareDoctorToCodeCache,
            RoleCache,
            RefusalPenaltyCache,
            ExaminationTypeCache,
            ExaminationGroupCache,
            M001Cache,
            V002Cache,
            V004Cache,
            V005Cache,
            V006Cache,
            V008Cache,
            V010Cache,
            V012Cache,
            V014Cache,
            V015Cache,
            V020Cache,
            V021Cache,
            V023Cache,
            V024Cache,
            V025Cache,
            V026Cache,
            V027Cache,
            V028Cache,
            V029Cache,
            N001Cache,
            N002Cache,
            N003Cache,
            N004Cache,
            "N005Cache",
            N007Cache,
            N010Cache,
            N008Cache,
            N011Cache,
            N013Cache,
            N014Cache,
            N015Cache,
            N016Cache,
            N017Cache,
            N018Cache,
            N019Cache,
            N020Cache,
            IDC10PayableCache,
            ConsultationOnkCache,
            GlobalRegionalAttributeCache
        };
        
        public CacheRepository()
        {
            
            CachesList.ToList().ForEach(p => Caches.Add(p, Di.I.ResolveNamed<ICache>(p)));
        }

        

        public ICache Get(string name)
        {
            if (Caches.ContainsKey(name))
            {
                return Caches[name];
            }
            return null;
        }

        public bool Has(string name)
        {
            return Caches.ContainsKey(name);
        }

        public void Put(string name, ICache cache)
        {
            if (!Caches.ContainsKey(name))
            {
                Caches.Add(name, cache);
            }
        }
    }
}
