using Autofac;
using Core.Attributes;
using Core.Infrastructure;
using Core.Services;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.DatabaseCore
{
    [Module(Name = "Доступ к данным", ClassType = typeof(InitCommand), Priority = ModulePriority.Normal, Order = 2)]
    public class InitCommand : IModuleActivator
    {
        public void Run()
        {

            var builder = new ContainerBuilder();
            //Регистрация кеша данных
            builder.RegisterType<CacheRepository>().As<ICacheRepository>().SingleInstance();
            
            builder.RegisterType<IDC10Cache>().Named<ICache>("IDC10Cache").SingleInstance();
            builder.RegisterType<F002Cache>().Named<ICache>("F002Cache").SingleInstance();
            builder.RegisterType<F002ToTfOkatoCache>().Named<ICache>("F002ToTfOkatoCache").SingleInstance();
            builder.RegisterType<F003Cache>().Named<ICache>("F003Cache").SingleInstance();
            builder.RegisterType<F003MCodeToNameCache>().Named<ICache>("F003MCodeToNameCache").SingleInstance();
            builder.RegisterType<F004Cache>().Named<ICache>("F004Cache").SingleInstance();
            builder.RegisterType<F005Cache>().Named<ICache>("F005Cache").SingleInstance();
            builder.RegisterType<F006Cache>().Named<ICache>("F006Cache").SingleInstance();
            builder.RegisterType<F008Cache>().Named<ICache>("F008Cache").SingleInstance();
            builder.RegisterType<F014Cache>().Named<ICache>("F014Cache").SingleInstance();
            builder.RegisterType<F010Cache>().Named<ICache>("F010Cache").SingleInstance();
            builder.RegisterType<F010OkatoToNameCache>().Named<ICache>("F010OkatoToNameCache").SingleInstance();
            builder.RegisterType<F010TfCodeToNameCache>().Named<ICache>("F010TfCodeToNameCache").SingleInstance();
            builder.RegisterType<F011Cache>().Named<ICache>("F011Cache").SingleInstance();
            builder.RegisterType<V004Cache>().Named<ICache>("V004Cache").SingleInstance();
            builder.RegisterType<V009Cache>().Named<ICache>("V009Cache").SingleInstance();
            builder.RegisterType<V012Cache>().Named<ICache>("V012Cache").SingleInstance();
            builder.RegisterType<V015Cache>().Named<ICache>("V015Cache").SingleInstance();
            builder.RegisterType<V021Cache>().Named<ICache>("V021Cache").SingleInstance();
            builder.RegisterType<V024Cache>().Named<ICache>("V024Cache").SingleInstance();
            builder.RegisterType<VersionCache>().Named<ICache>("VersionCache").SingleInstance();
            builder.RegisterType<F014aCache>().Named<ICache>("F014aCache").SingleInstance();
            builder.RegisterType<ParamCache>().Named<ICache>("ParamCache").SingleInstance();
            builder.RegisterType<ScopeCache>().Named<ICache>("ScopeCache").SingleInstance();
            builder.RegisterType<V006Cache>().Named<ICache>("V006Cache").SingleInstance();
            builder.RegisterType<PaymentStatusCache>().Named<ICache>("PaymentStatusCache").SingleInstance();
            builder.RegisterType<AccountStatusCache>().Named<ICache>("AccountStatusCache").SingleInstance();
            builder.RegisterType<AccountTypeCache>().Named<ICache>("AccountTypeCache").SingleInstance();
            builder.RegisterType<TortilaPolicyTypeCache>().Named<ICache>("TortilaPolicyTypeCache").SingleInstance();
            builder.RegisterType<F002IdToNameCache>().Named<ICache>("F002IdToNameCache").SingleInstance();
            builder.RegisterType<ShareDoctorToCodeCache>().Named<ICache>("ShareDoctorToCodeCache").SingleInstance();
            builder.RegisterType<RoleCache>().Named<ICache>("RoleCache").SingleInstance();
            builder.RegisterType<RefusalPenaltyCache>().Named<ICache>("RefusalPenaltyCache").SingleInstance();
            builder.RegisterType<ExaminationTypeCache>().Named<ICache>(CacheRepository.ExaminationTypeCache).SingleInstance();
            builder.RegisterType<ExaminationGroupCache>().Named<ICache>(CacheRepository.ExaminationGroupCache).SingleInstance();
            builder.RegisterType<M001Cache>().Named<ICache>(CacheRepository.M001Cache).SingleInstance();
            builder.RegisterType<V002Cache>().Named<ICache>(CacheRepository.V002Cache).SingleInstance();
            builder.RegisterType<V005Cache>().Named<ICache>(CacheRepository.V005Cache).SingleInstance();
            builder.RegisterType<V014Cache>().Named<ICache>(CacheRepository.V014Cache).SingleInstance();
            builder.RegisterType<N001Cache>().Named<ICache>(CacheRepository.N001Cache).SingleInstance();
            builder.RegisterType<N002Cache>().Named<ICache>(CacheRepository.N002Cache).SingleInstance();
            builder.RegisterType<N003Cache>().Named<ICache>(CacheRepository.N003Cache).SingleInstance();
            builder.RegisterType<N004Cache>().Named<ICache>(CacheRepository.N004Cache).SingleInstance();
            builder.RegisterType<N005Cache>().Named<ICache>("N005Cache").SingleInstance();
            builder.RegisterType<N007Cache>().Named<ICache>(CacheRepository.N007Cache).SingleInstance();
            builder.RegisterType<N008Cache>().Named<ICache>(CacheRepository.N008Cache).SingleInstance();
            builder.RegisterType<N010Cache>().Named<ICache>(CacheRepository.N010Cache).SingleInstance();
            builder.RegisterType<N011Cache>().Named<ICache>(CacheRepository.N011Cache).SingleInstance();
            builder.RegisterType<N013Cache>().Named<ICache>(CacheRepository.N013Cache).SingleInstance();
            builder.RegisterType<N014Cache>().Named<ICache>(CacheRepository.N014Cache).SingleInstance();
            builder.RegisterType<N015Cache>().Named<ICache>(CacheRepository.N015Cache).SingleInstance();
            builder.RegisterType<N016Cache>().Named<ICache>(CacheRepository.N016Cache).SingleInstance();
            builder.RegisterType<N017Cache>().Named<ICache>(CacheRepository.N017Cache).SingleInstance();
            builder.RegisterType<N018Cache>().Named<ICache>(CacheRepository.N018Cache).SingleInstance();
            builder.RegisterType<N019Cache>().Named<ICache>(CacheRepository.N019Cache).SingleInstance();
            builder.RegisterType<N020Cache>().Named<ICache>(CacheRepository.N020Cache).SingleInstance();
            builder.RegisterType<V008Cache>().Named<ICache>(CacheRepository.V008Cache).SingleInstance();
            builder.RegisterType<V010Cache>().Named<ICache>(CacheRepository.V010Cache).SingleInstance();
            builder.RegisterType<V020Cache>().Named<ICache>(CacheRepository.V020Cache).SingleInstance();
            builder.RegisterType<V023Cache>().Named<ICache>(CacheRepository.V023Cache).SingleInstance();
            builder.RegisterType<V025Cache>().Named<ICache>(CacheRepository.V025Cache).SingleInstance();
            builder.RegisterType<V026Cache>().Named<ICache>(CacheRepository.V026Cache).SingleInstance();
            builder.RegisterType<V027Cache>().Named<ICache>(CacheRepository.V027Cache).SingleInstance();
            builder.RegisterType<V028Cache>().Named<ICache>(CacheRepository.V028Cache).SingleInstance();
            builder.RegisterType<V029Cache>().Named<ICache>(CacheRepository.V029Cache).SingleInstance();
            builder.RegisterType<IDC10PayableCache>().Named<ICache>(CacheRepository.IDC10PayableCache).SingleInstance();
            builder.RegisterType<ConsultationOnkCache>().Named<ICache>(CacheRepository.ConsultationOnkCache).SingleInstance();
            builder.RegisterType<GlobalRegionalAttributeCache>().Named<ICache>(CacheRepository.GlobalRegionalAttributeCache).SingleInstance();

            Di.Update(builder);
        }
    }
}
