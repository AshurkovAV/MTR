using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using BLToolkit.Data.Linq;
using BLToolkit.Mapping;
using Core;
using Core.DataStructure;
using Core.Extensions;
using Core.Helpers;
using Core.Infrastructure;
using Core.Services;
using Core.Utils;
using DataModel;
using Medical.CoreLayer.Service;
//using Medical.DatabaseCore.EntityDataModel;
using EventShortView = DataModel.EventShortView;
using F001 = DataModel.F001;
using F002 = DataModel.F002;
using F003 = DataModel.F003;
using F005 = DataModel.F005;
using F006 = DataModel.F006;
using F008 = DataModel.F008;
using F010 = DataModel.F010;
using F011 = DataModel.F011;
using F014 = DataModel.F014;
using FactDocument = DataModel.FactDocument;
using FactEconomicAccount = DataModel.FactEconomicAccount;
using FactEconomicPayment = DataModel.FactEconomicPayment;
using FactEconomicPaymentDetail = DataModel.FactEconomicPaymentDetail;
using FactEconomicRefuse = DataModel.FactEconomicRefuse;
using FactEconomicRefuseDetail = DataModel.FactEconomicRefuseDetail;
using FactEconomicSurcharge = DataModel.FactEconomicSurcharge;
using FactEconomicSurchargeDetail = DataModel.FactEconomicSurchargeDetail;
using FactEQMA = DataModel.FactEQMA;
using FactExchange = DataModel.FactExchange;
using FactExpertCriterion = DataModel.FactExpertCriterion;
using FactExternalRefuse = DataModel.FactExternalRefuse;
using FactMEC = DataModel.FactMEC;
using FactMedicalAccount = DataModel.FactMedicalAccount;
using FactMedicalEvent = DataModel.FactMedicalEvent;
using FactMEE = DataModel.FactMEE;
using FactPatient = DataModel.FactPatient;
using FactPerson = DataModel.FactPerson;
using FactPreparedReport = DataModel.FactPreparedReport;
using FactReport = DataModel.FactReport;
using FactSrzQuery = DataModel.FactSrzQuery;
using FactTerritoryAccount = DataModel.FactTerritoryAccount;
using globalAccountType = DataModel.globalAccountType;
using globalExaminationGroup = DataModel.globalExaminationGroup;
using globalExaminationType = DataModel.globalExaminationType;
using globalMedicalAssistanceVolume = DataModel.globalMedicalAssistanceVolume;
using globalOldProfile = DataModel.globalOldProfile;
using globalParam = DataModel.globalParam;
using globalParticularSign = DataModel.globalParticularSign;
using globalScope = DataModel.globalScope;
using globalVersion = DataModel.globalVersion;
using localEmployee = DataModel.localEmployee;
using localUser = DataModel.localUser;
using M001 = DataModel.M001;
using MedicalAccountView = DataModel.MedicalAccountView;
using PatientShortView = DataModel.PatientShortView;
using V002 = DataModel.V002;
using V003 = DataModel.V003;
using V004 = DataModel.V004;
using V005 = DataModel.V005;
using V006 = DataModel.V006;
using V008 = DataModel.V008;
using V009 = DataModel.V009;
using V010 = DataModel.V010;
using V012 = DataModel.V012;
using V014 = DataModel.V014;
using V015 = DataModel.V015;
using localSettings = DataModel.localSettings;
using localUserSettings = DataModel.localUserSettings;
using TerritoryAccountView = DataModel.TerritoryAccountView;
using FactActEqma = DataModel.FactActEqma;
using FactActMee = DataModel.FactActMee;
using FactProcessing = DataModel.FactProcessing;
using globalLicenseEntry = DataModel.globalLicenseEntry;
using globalProcessingType = DataModel.globalProcessingType;
using globalRefusalSource = DataModel.globalRefusalSource;
using localF003 = DataModel.localF003;
using localRole = DataModel.localRole;
using shareDoctor = DataModel.shareDoctor;

namespace Medical.DatabaseCore.Services.Database
{
    [Flags]
    public enum UpdateFlag
    {
        None = 0,
        Account = 1,
        MedicalEvent = 2,
        MedicalService = 3,
        ZslMedicalEvent = 4
    }

    public class MedicineRepository : IMedicineRepository
    {
        private readonly IMessageService _messageService;

        private readonly string _defaultProvider;
        private readonly string _defaultName;

        #region Queries
        private static readonly Func<MedicineContext, IEnumerable<globalVersion>> GlobalVersionQuery =
            CompiledQuery.Compile<MedicineContext, IEnumerable<globalVersion>>(db =>
            from e in db.globalVersion
            select e
        );

        private static readonly Func<MedicineContext, IEnumerable<globalPcel>> GlobalPcelQuery =
           CompiledQuery.Compile<MedicineContext, IEnumerable<globalPcel>>(db =>
           from e in db.globalPcel
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<globalKslp>> GlobalKslpQuery =
           CompiledQuery.Compile<MedicineContext, IEnumerable<globalKslp>>(db =>
           from e in db.globalKslp
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<globalRefusalSource>> GlobalRefusalSourceQuery =
            CompiledQuery.Compile<MedicineContext, IEnumerable<globalRefusalSource>>(db =>
            from e in db.globalRefusalSource
            select e
        );

        private static readonly Func<MedicineContext, IEnumerable<V009>> V009Query =
            CompiledQuery.Compile<MedicineContext, IEnumerable<V009>>(db =>
            from e in db.V009
            select e
        );

        private static readonly Func<MedicineContext, IEnumerable<V015>> V015Query =
            CompiledQuery.Compile<MedicineContext, IEnumerable<V015>>(db =>
            from e in db.V015
            select e
        );

        private static readonly Func<MedicineContext, IEnumerable<V020>> V020Query =
           CompiledQuery.Compile<MedicineContext, IEnumerable<V020>>(db =>
           from e in db.V020
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<V021>> V021Query =
           CompiledQuery.Compile<MedicineContext, IEnumerable<V021>>(db =>
           from e in db.V021
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<V025>> V025Query =
           CompiledQuery.Compile<MedicineContext, IEnumerable<V025>>(db =>
           from e in db.V025
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<V024>> V024Query =
           CompiledQuery.Compile<MedicineContext, IEnumerable<V024>>(db =>
           from e in db.V024
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<V004>> V004Query =
            CompiledQuery.Compile<MedicineContext, IEnumerable<V004>>(db =>
            from e in db.V004
            select e
        );

        private static readonly Func<MedicineContext, IEnumerable<V012>> V012Query =
            CompiledQuery.Compile<MedicineContext, IEnumerable<V012>>(db =>
            from e in db.V012
            select e
        );

        private static readonly Func<MedicineContext, IEnumerable<V014>> V014Query =
            CompiledQuery.Compile<MedicineContext, IEnumerable<V014>>(db =>
            from e in db.V014
            select e
        );
        private static readonly Func<MedicineContext, IEnumerable<V026>> V026Query =
            CompiledQuery.Compile<MedicineContext, IEnumerable<V026>>(db =>
            from e in db.V026
            select e
        );

        private static readonly Func<MedicineContext, IEnumerable<V027>> V027Query =
            CompiledQuery.Compile<MedicineContext, IEnumerable<V027>>(db =>
            from e in db.V027
            select e
        );

        private static readonly Func<MedicineContext, IEnumerable<V028>> V028Query =
           CompiledQuery.Compile<MedicineContext, IEnumerable<V028>>(db =>
           from e in db.V028
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<V029>> V029Query =
           CompiledQuery.Compile<MedicineContext, IEnumerable<V029>>(db =>
           from e in db.V029
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<M001>> M001Query =
           CompiledQuery.Compile<MedicineContext, IEnumerable<M001>>(db =>
           from e in db.M001
           orderby e.IDDS
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<V023>> V023Query =
          CompiledQuery.Compile<MedicineContext, IEnumerable<V023>>(db =>
          from e in db.V023
          orderby e.K_KSG
          select e
      );

        private static readonly Func<MedicineContext, IEnumerable<F001>> F001Query =
           CompiledQuery.Compile<MedicineContext, IEnumerable<F001>>(db =>
           from e in db.F001
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<F002>> F002Query =
           CompiledQuery.Compile<MedicineContext, IEnumerable<F002>>(db =>
           from e in db.F002
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<F003>> F003Query =
           CompiledQuery.Compile<MedicineContext, IEnumerable<F003>>(db =>
           from e in db.F003
           select e
       );

        private static readonly Func<MedicineContext,string, IEnumerable<F003>> F003ByOkatoQuery =
           CompiledQuery.Compile<MedicineContext, string, IEnumerable<F003>>((db,okato) =>
           from e in db.F003
           where e.tf_okato == okato
           orderby e.mcod
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<F005>> F005Query =
           CompiledQuery.Compile<MedicineContext, IEnumerable<F005>>(db =>
           from e in db.F005
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<F006>> F006Query =
           CompiledQuery.Compile<MedicineContext, IEnumerable<F006>>(db =>
           from e in db.F006
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<F008>> F008Query =
           CompiledQuery.Compile<MedicineContext, IEnumerable<F008>>(db =>
           from e in db.F008
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<F010>> F010Query =
           CompiledQuery.Compile<MedicineContext, IEnumerable<F010>>(db =>
           from e in db.F010
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<F014>> F014Query =
           CompiledQuery.Compile<MedicineContext, IEnumerable<F014>>(db =>
           from e in db.F014
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<N007>> N007Query =
          CompiledQuery.Compile<MedicineContext, IEnumerable<N007>>(db =>
          from e in db.N007
          select e
      );

        private static readonly Func<MedicineContext, IEnumerable<N008>> N008Query =
          CompiledQuery.Compile<MedicineContext, IEnumerable<N008>>(db =>
          from e in db.N008
          select e
      );
        private static readonly Func<MedicineContext, IEnumerable<N010>> N010Query =
          CompiledQuery.Compile<MedicineContext, IEnumerable<N010>>(db =>
          from e in db.N010
          select e
      );
        private static readonly Func<MedicineContext, IEnumerable<N011>> N011Query =
          CompiledQuery.Compile<MedicineContext, IEnumerable<N011>>(db =>
          from e in db.N011
          select e
      );

        private static readonly Func<MedicineContext, IEnumerable<N013>> N013Query =
          CompiledQuery.Compile<MedicineContext, IEnumerable<N013>>(db =>
          from e in db.N013
          select e
      );

        private static readonly Func<MedicineContext, IEnumerable<N014>> N014Query =
          CompiledQuery.Compile<MedicineContext, IEnumerable<N014>>(db =>
          from e in db.N014
          select e
      );

        private static readonly Func<MedicineContext, IEnumerable<FactExchange>> FactExchangeQuery =
           CompiledQuery.Compile<MedicineContext, IEnumerable<FactExchange>>(db =>
           from e in db.FactExchange
           select e
       );

       
        private static readonly Func<MedicineContext, IEnumerable<EventShortView>> EventShortViewQuery =
          CompiledQuery.Compile<MedicineContext, IEnumerable<EventShortView>>(db =>
          from e in db.EventShortView
          orderby e.ExternalId
          select e
        );

        private static readonly Func<MedicineContext, IEnumerable<ZslEventShortView>> ZslEventShortViewQuery =
         CompiledQuery.Compile<MedicineContext, IEnumerable<ZslEventShortView>>(db =>
         from e in db.ZslEventShortView
         orderby e.ExternalId
         select e
       );

        private static readonly Func<MedicineContext, IEnumerable<GeneralEventShortView>> GeneralEventShortViewQuery =
         CompiledQuery.Compile<MedicineContext, IEnumerable<GeneralEventShortView>>(db =>
         from e in db.GeneralEventShortView
         orderby e.ExternalId
         select e
       );

        private static readonly Func<MedicineContext, IEnumerable<EventExtendedView>> EventExtendedViewQuery =
         CompiledQuery.Compile<MedicineContext, IEnumerable<EventExtendedView>>(db =>
         from e in db.EventExtendedView
         orderby e.ExternalId
         select e
       );

        private static readonly Func<MedicineContext, IEnumerable<MedicalAccountView>> MedicalAccountViewQuery =
          CompiledQuery.Compile<MedicineContext, IEnumerable<MedicalAccountView>>(db =>
          from e in db.MedicalAccountView
          orderby e.Date
          select e
        );

        private static readonly Func<MedicineContext, IEnumerable<V002>> V002Query =
          CompiledQuery.Compile<MedicineContext, IEnumerable<V002>>(db =>
          from e in db.V002
          select e
        );

        private static readonly Func<MedicineContext, IEnumerable<V003>> V003Query =
          CompiledQuery.Compile<MedicineContext, IEnumerable<V003>>(db =>
          from e in db.V003
          select e
        );

        private static readonly Func<MedicineContext, IEnumerable<V005>> V005Query =
          CompiledQuery.Compile<MedicineContext, IEnumerable<V005>>(db =>
          from e in db.V005
          select e
        );

        private static readonly Func<MedicineContext, IEnumerable<V006>> V006Query =
          CompiledQuery.Compile<MedicineContext, IEnumerable<V006>>(db =>
          from e in db.V006
          select e
        );

        private static readonly Func<MedicineContext, IEnumerable<V008>> V008Query =
          CompiledQuery.Compile<MedicineContext, IEnumerable<V008>>(db =>
          from e in db.V008
          select e
        );
       
         private static readonly Func<MedicineContext, string, IEnumerable<EconomicPartner>> EconomicPartnerQuery =
         CompiledQuery.Compile<MedicineContext, string, IEnumerable<EconomicPartner>>((db, s) =>
         from e in db.EconomicPartner
         where e.Okato == s
         select e
       );

        private static readonly Func<MedicineContext, IEnumerable<V010>> V010Query =
          CompiledQuery.Compile<MedicineContext, IEnumerable<V010>>(db =>
          from e in db.V010
          select e
        );

        private static readonly Func<MedicineContext, IEnumerable<globalExaminationGroup>> GlobalExaminationGroupQuery =
          CompiledQuery.Compile<MedicineContext, IEnumerable<globalExaminationGroup>>(db =>
          from e in db.globalExaminationGroup
          select e
        );

        private static readonly Func<MedicineContext, IEnumerable<globalPaymentStatus>> GlobalPaymentStatusQuery =
          CompiledQuery.Compile<MedicineContext, IEnumerable<globalPaymentStatus>>(db =>
          from e in db.globalPaymentStatus
          select e
        );

        private static readonly Func<MedicineContext, IEnumerable<globalParticularSign>> GlobalParticularSignQuery =
          CompiledQuery.Compile<MedicineContext, IEnumerable<globalParticularSign>>(db =>
          from e in db.globalParticularSign
          select e
        );

        private static readonly Func<MedicineContext, IEnumerable<globalOldProfile>> GlobalOldProfileQuery =
          CompiledQuery.Compile<MedicineContext, IEnumerable<globalOldProfile>>(db =>
          from e in db.globalOldProfile
          select e
        );

        private static readonly Func<MedicineContext, IEnumerable<globalExaminationType>> GlobalExaminationTypeQuery =
          CompiledQuery.Compile<MedicineContext, IEnumerable<globalExaminationType>>(db =>
          from e in db.globalExaminationType
          select e
        );

        private static readonly Func<MedicineContext, IEnumerable<globalReportType>> GlobalReportTypeQuery =
            CompiledQuery.Compile<MedicineContext, IEnumerable<globalReportType>>(db =>
                from e in db.globalReportType
                select e
            );

        private static readonly Func<MedicineContext, IEnumerable<globalScope>> GlobalScopeQuery =
         CompiledQuery.Compile<MedicineContext, IEnumerable<globalScope>>(db =>
         from e in db.globalScope
         select e
       );

        private static readonly Func<MedicineContext, IEnumerable<globalParam>> GlobalParamQuery =
         CompiledQuery.Compile<MedicineContext, IEnumerable<globalParam>>(db =>
         from e in db.globalParam
         select e
       );

        private static readonly Func<MedicineContext, IEnumerable<F011>> F011Query =
         CompiledQuery.Compile<MedicineContext, IEnumerable<F011>>(db =>
         from e in db.F011
         select e
       );

        private static readonly Func<MedicineContext, IEnumerable<localEmployee>> LocalEmployeeQuery =
         CompiledQuery.Compile<MedicineContext, IEnumerable<localEmployee>>(db =>
         from e in db.localEmployee
         select e
       );

        private static readonly Func<MedicineContext, string, IEnumerable<localUser>> GetUserQuery =
         CompiledQuery.Compile<MedicineContext, string, IEnumerable<localUser>>((db, s) =>
         from e in db.localUser
         where e.Login == s
         select e
       );

        private static readonly Func<MedicineContext, IEnumerable<FactExpertCriterion>> ExpertCriterionQuery =
         CompiledQuery.Compile<MedicineContext, IEnumerable<FactExpertCriterion>>(db =>
         from e in db.FactExpertCriterion
         select e
       );

        private static readonly Func<MedicineContext, IEnumerable<PatientShortView>> PatientShortViewQuery =
         CompiledQuery.Compile<MedicineContext, IEnumerable<PatientShortView>>(db =>
         from e in db.PatientShortView
         select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<FactPatient>> FactPatientQuery =
         CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactPatient>>((db, n) =>
         from e in db.FactPatient
         where e.PatientId == n
         select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<Tuple<FactPatient,FactDocument,FactPerson>>> FactPatientQueryFull =
         CompiledQuery.Compile<MedicineContext, int, IEnumerable<Tuple<FactPatient,FactDocument,FactPerson>>>((db, n) =>
         from e in db.FactPatient
         where e.PatientId == n
         select new Tuple<FactPatient, FactDocument, FactPerson>(e, e.FACTPATIFPATIENTPFACTPERS.FACTDOCUFDOCUMENTFACTPERSs.FirstOrDefault(), e.FACTPATIFPATIENTPFACTPERS)
       );

        private static readonly Func<MedicineContext, int, IEnumerable<int>> FactPatientIdsByAccountIdQuery =
         CompiledQuery.Compile<MedicineContext, int, IEnumerable<int>>((db, n) =>
         from e in db.FactPatient
         where e.AccountId == n
         orderby e.ExternalId
         select e.PatientId
       );

        private static readonly Func<MedicineContext, int, IEnumerable<int>> FactPatientsIdsByMedicalAccountIdQuery =
         CompiledQuery.Compile<MedicineContext, int, IEnumerable<int>>((db, n) =>
         from e in db.FactPatient
         where e.MedicalAccountId == n
         orderby e.MedicalExternalId
         select e.PatientId
       );

        private static readonly Func<MedicineContext, int, IEnumerable<int>> FactMedicalEventIdsByPatientIdQuery =
         CompiledQuery.Compile<MedicineContext, int, IEnumerable<int>>((db, n) =>
         from e in db.FactMedicalEvent
         where e.PatientId == n
         orderby e.ExternalId
         select e.MedicalEventId
       );

        private static readonly Func<MedicineContext, int, IEnumerable<int>> ZFactMedicalEventIdsByZslMeventIdQuery =
         CompiledQuery.Compile<MedicineContext, int, IEnumerable<int>>((db, n) =>
         from e in db.ZFactMedicalEvent
         where e.ZslMedicalEventId == n
         orderby e.ZmedicalEventId
         select e.ZmedicalEventId
       );

        private static readonly Func<MedicineContext, int, IEnumerable<int>> ZFactKsgKpgIdsByZslMeventIdQuery =
         CompiledQuery.Compile<MedicineContext, int, IEnumerable<int>>((db, n) =>
         from e in db.ZFactKsgKpg
         where e.ZmedicalEventId == n
         orderby e.ZmedicalEventId
         select e.ZksgKpgId
       );

        private static readonly Func<MedicineContext, int, IEnumerable<int>> ZFactMedicalEvenOnktIdsByZslMeventIdQuery =
         CompiledQuery.Compile<MedicineContext, int, IEnumerable<int>>((db, n) =>
         from e in db.ZFactMedicalEventOnk
         where e.ZmedicalEventId == n
         orderby e.ZmedicalEventId
         select e.ZMedicalEventOnkId
       );

        private static readonly Func<MedicineContext, int, IEnumerable<int>> ZslFactMedicalEventIdsByPatientIdQuery =
         CompiledQuery.Compile<MedicineContext, int, IEnumerable<int>>((db, n) =>
         from e in db.ZslFactMedicalEvent
         where e.PatientId == n
         orderby e.ExternalId
         select e.ZslMedicalEventId
       );

        private static readonly Func<MedicineContext, int, IEnumerable<int>> FactMedicalServiceIdsByMedicalEventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<int>>((db, n) =>
        from e in db.FactMedicalServices
        where e.MedicalEventId == n
        orderby e.ExternalId
        select e.MedicalServicesId
      );

        private static readonly Func<MedicineContext, int, IEnumerable<int>> ZFactDirectionIdsByMedicalEventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<int>>((db, n) =>
        from e in db.ZFactDirection
        where e.ZmedicalEventId == n
        orderby e.ZDirectionId
        select e.ZDirectionId
      );

        private static readonly Func<MedicineContext, int, IEnumerable<int>> ZFactMedicalServiceIdsByMedicalEventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<int>>((db, n) =>
        from e in db.ZFactMedicalServices
        where e.ZmedicalEventId == n
        orderby e.ExternalId
        select e.ZmedicalServicesId
      );

        private static readonly Func<MedicineContext, int, IEnumerable<int>> ZFactMedicalServiceOnkIdsByMedicalEventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<int>>((db, n) =>
        from e in db.ZFactMedicalServicesOnk
        where e.ZmedicalEventOnkId == n
        select e.ZmedicalServicesOnkId
      );

        private static readonly Func<MedicineContext, int, IEnumerable<int>> ZFactMedicalConsultationsOnkIdsByMedicalEventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<int>>((db, n) =>
        from e in db.ZFactConsultations
        where e.ZMedicalEventId == n
        select e.ZConsultationsId
      );

        private static readonly Func<MedicineContext, IEnumerable<FactReport>> FactReportQuery =
         CompiledQuery.Compile<MedicineContext, IEnumerable<FactReport>>(db =>
         from e in db.FactReport
         select e
       );

        private static readonly Func<MedicineContext, IEnumerable<FactActExpertise>> FactActExpertiseQuery =
            CompiledQuery.Compile<MedicineContext, IEnumerable<FactActExpertise>>(db =>
                from e in db.FactActExpertise
                select e
            );

        private static readonly Func<MedicineContext, IEnumerable<FactPreparedReport>> FactPreparedReportQuery =
         CompiledQuery.Compile<MedicineContext, IEnumerable<FactPreparedReport>>(db =>
         from e in db.FactPreparedReport
         select e
       );
       // private static readonly Func<MedicineContext, IEnumerable<FactPreparedReportNotBody>> FactPreparedReportQueryNotBody =
       //  CompiledQuery.Compile<MedicineContext, IEnumerable<FactPreparedReportNotBody>>(db =>
       //  from e in db.FactPreparedReportNotBody
       //  select e
       //);

        private static readonly Func<MedicineContext, int, IEnumerable<FactReport>> EnabledFactReportByScopeQuery =
         CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactReport>>((db,n) =>
         from e in db.FactReport
         where e.IsEnable == true && e.Scope == n
         select e
       );

        private static readonly Func<MedicineContext, int, int, IEnumerable<FactReport>> EnabledFactReportByScopeByVersionQuery =
         CompiledQuery.Compile<MedicineContext, int, int, IEnumerable<FactReport>>((db, n, y) =>
         from e in db.FactReport
         where e.IsEnable == true && e.Scope == n && e.Version == y
         select e
       );

        private static readonly Func<MedicineContext, IEnumerable<localEmployee>> EmployeeQuery =
         CompiledQuery.Compile<MedicineContext, IEnumerable<localEmployee>>(db =>
         from e in db.localEmployee
         select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<FactReport>> ReportByIdQuery =
         CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactReport>>((db,n) =>
         from e in db.FactReport
         where e.FactReportID == n
         select e
       );

        private static readonly Func<MedicineContext, int, int, int?, IEnumerable<FactPreparedReport>> PreparedReportByExternalIdAndScopeQuery =
        CompiledQuery.Compile<MedicineContext, int, int, int?, IEnumerable<FactPreparedReport>>((db, n, o, p) =>
        from e in db.FactPreparedReport
        where e.Scope == o && (e.SubId == p || e.ExternalId == n)
        select e
      );

        private static readonly Func<MedicineContext, int, IEnumerable<FactPreparedReport>> PreparedReportByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactPreparedReport>>((db, n) =>
        from e in db.FactPreparedReport
        where e.PreparedReportId == n
        select e
      );

        private static readonly Func<MedicineContext, int, int, IEnumerable<FactExpertCriterion>> EnabledExpertCriterionByScopeAndVersionQuery =
        CompiledQuery.Compile<MedicineContext, int, int, IEnumerable<FactExpertCriterion>>((db, n, o) =>
        from e in db.FactExpertCriterion
        where e.IsEnable == true && e.Scope == n && e.Version == o
        select e
      );

        private static readonly Func<MedicineContext, int, int, IEnumerable<FactProcessing>> EnabledProcessingByScopeAndVersionQuery =
        CompiledQuery.Compile<MedicineContext, int, int, IEnumerable<FactProcessing>>((db, n, o) =>
        from e in db.FactProcessing
        where e.IsEnable == true && e.Scope_ScopeID == n && e.Version_VersionID == o
        select e
      );

        

        private static readonly Func<MedicineContext, int, IEnumerable<FactEconomicAccount>> EconomicAccountByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactEconomicAccount>>((db, n) =>
        from e in db.FactEconomicAccount
        where e.EconomicAccountId == n
        select e
      );
        

        private static readonly Func<MedicineContext, int, IEnumerable<FactEconomicPayment>> PaymentsByEconomicAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactEconomicPayment>>((db, n) =>
        from e in db.FactEconomicPayment
        where e.EconomicAccountId == n
        select e
      );

        private static readonly Func<MedicineContext, int, IEnumerable<FactEconomicPayment>> ExistingAssistanceConditionsByAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactEconomicPayment>>((db, n) =>
            from e in db.FactMedicalEvent
            where e.FACTMEDIPATIENTIDFACTPATI.AccountId == n
            group e by e.AssistanceConditions into g
            select new { AssistanceConditions = g.Key , Amount = g.Sum(e=>e.AcceptPrice)} into h
            select new FactEconomicPayment { Amount = h.Amount ?? 0, AssistanceConditionsId = h.AssistanceConditions ?? 0 }
      );

        private static readonly Func<MedicineContext, int, IEnumerable<FactEconomicPayment>> ZslExistingAssistanceConditionsByAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactEconomicPayment>>((db, n) =>
            from e in db.ZslFactMedicalEvent
            where e.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == n
            group e by e.AssistanceConditions into g
            select new { AssistanceConditions = g.Key, Amount = g.Sum(e => e.AcceptPrice) } into h
            select new FactEconomicPayment { Amount = h.Amount ?? 0, AssistanceConditionsId = h.AssistanceConditions ?? 0 }
      );

        private static readonly Func<MedicineContext, int, IEnumerable<FactEconomicPayment>> ZExistingAssistanceConditionsByAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactEconomicPayment>>((db, n) =>
            from e in db.ZslFactMedicalEvent
            where e.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == n
            group e by e.AssistanceConditions into g
            select new { AssistanceConditions = g.Key, Amount = g.Sum(e => e.AcceptPrice) } into h
            select new FactEconomicPayment { Amount = h.Amount ?? 0, AssistanceConditionsId = h.AssistanceConditions ?? 0 }
      );

        private static readonly Func<MedicineContext, int, IEnumerable<FactEconomicRefuseDetail>> ExistingAssistanceConditionsRefuseByAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactEconomicRefuseDetail>>((db, n) =>
            from e in db.FactMedicalEvent
            where e.FACTMEDIPATIENTIDFACTPATI.AccountId == n
            group e by e.AssistanceConditions into g
            select new { AssistanceConditions = g.Key, Amount = g.Sum(e => e.Price), AcceptAmount = g.Sum(e=>e.AcceptPrice) } into h
            select new FactEconomicRefuseDetail { AmountRefuse = (h.Amount ?? 0) - (h.AcceptAmount ?? 0), AssistanceConditionsId = h.AssistanceConditions ?? 0 }
      );

        private static readonly Func<MedicineContext, int, IEnumerable<FactEconomicRefuseDetail>> ZExistingAssistanceConditionsRefuseByAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactEconomicRefuseDetail>>((db, n) =>
            from e in db.ZslFactMedicalEvent
            where e.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == n
            group e by e.AssistanceConditions into g
            select new { AssistanceConditions = g.Key, Amount = g.Sum(e => e.Price), AcceptAmount = g.Sum(e => e.AcceptPrice) } into h
            select new FactEconomicRefuseDetail { AmountRefuse = (h.Amount ?? 0) - (h.AcceptAmount ?? 0), AssistanceConditionsId = h.AssistanceConditions ?? 0 }
      );

        private static readonly Func<MedicineContext, int, IEnumerable<FactEconomicRefuse>> EconomicRefuseByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactEconomicRefuse>>((db, n) =>
        from e in db.FactEconomicRefuse
        where e.EconomicRefuseId == n
        select e
      );

        private static readonly Func<MedicineContext, int, IEnumerable<FactEconomicRefuseDetail>> RefusesDetailByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactEconomicRefuseDetail>>((db, n) =>
            from e in db.FactEconomicRefuseDetail
            where e.EconomicRefuseId == n
            select e 
      );

        private static readonly Func<MedicineContext, int, IEnumerable<FactEconomicSurcharge>> EconomicSurchargeByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactEconomicSurcharge>>((db, n) =>
        from e in db.FactEconomicSurcharge
        where e.EconomicSurchargeId == n
        select e
      );

        private static readonly Func<MedicineContext, int, IEnumerable<FactEconomicSurchargeDetail>> SurchargeDetailByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactEconomicSurchargeDetail>>((db, n) =>
            from e in db.FactEconomicSurchargeDetail
            where e.EconomicSurchargeId == n
            select e
      );

        private static readonly Func<MedicineContext, int, IEnumerable<FactEconomicAccount>> EconomicAccountByAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactEconomicAccount>>((db, n) =>
            from e in db.FactEconomicAccount
            where e.AccountId == n
            select e
      );

        private static readonly Func<MedicineContext, int, IEnumerable<FactEconomicRefuse>> EconomicRefuseByAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactEconomicRefuse>>((db, n) =>
            from e in db.FactEconomicRefuse
            where e.AccountId == n
            select e
      );

        private static readonly Func<MedicineContext, IEnumerable<FactEconomicAccount>> EconomicAccountQuery =
        CompiledQuery.Compile<MedicineContext, IEnumerable<FactEconomicAccount>>(db =>
            from e in db.FactEconomicAccount
            select e
      );

        private static readonly Func<MedicineContext, IEnumerable<globalAccountStatus>> GlobalAccountStatusQuery =
        CompiledQuery.Compile<MedicineContext, IEnumerable<globalAccountStatus>>(db =>
            from e in db.globalAccountStatus
            select e
      );

        private static readonly Func<MedicineContext, int, IEnumerable<FactEconomicSurcharge>> EconomicSurchargeByAccountIdQuery =
       CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactEconomicSurcharge>>((db, n) =>
           from e in db.FactEconomicSurcharge
           where e.AccountId == n
           select e
        );

        private static readonly Func<MedicineContext, IEnumerable<globalAccountType>> GlobalAccountTypeQuery =
        CompiledQuery.Compile<MedicineContext, IEnumerable<globalAccountType>>(db =>
            from e in db.globalAccountType
            select e
      );

        private static readonly Func<MedicineContext, int, IEnumerable<FactTerritoryAccount>> TerritoryAccountByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactTerritoryAccount>>((db, n) =>
            from e in db.FactTerritoryAccount
            where e.TerritoryAccountId == n
            select e
        );

       
        private static readonly Func<MedicineContext, IEnumerable<localSettings>> LocalSettingsQuery =
        CompiledQuery.Compile<MedicineContext, IEnumerable<localSettings>>(db =>
            from e in db.localSettings
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<FactPatient>> FactPatientsByAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactPatient>>((db, n) =>
            from e in db.FactPatient
            where e.AccountId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<FactPerson>> PersonByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactPerson>>((db, n) =>
            from e in db.FactPerson
            where e.PersonId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<FactDocument>> DocumentByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactDocument>>((db, n) =>
            from e in db.FactDocument
            where e.DocumentId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<FactMedicalEvent>> MeventsByPatientIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactMedicalEvent>>((db, n) =>
            from e in db.FactMedicalEvent
            where e.PatientId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<ZslFactMedicalEvent>> ZslMeventsByPatientIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZslFactMedicalEvent>>((db, n) =>
            from e in db.ZslFactMedicalEvent
            where e.PatientId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactMedicalEvent>> ZMeventsByZmeventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactMedicalEvent>>((db, n) =>
            from e in db.ZFactMedicalEvent
            where e.ZslMedicalEventId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<FactMedicalServices>> ServicesByMeventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactMedicalServices>>((db, n) =>
            from e in db.FactMedicalServices
            where e.MedicalEventId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactMedicalServices>> ZServicesByZMeventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactMedicalServices>>((db, n) =>
            from e in db.ZFactMedicalServices
            where e.ZmedicalEventId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactSlKoef>> SlKoefByKsgKpgIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactSlKoef>>((db, n) =>
            from e in db.ZFactSlKoef
            where e.ZksgKpgId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactCrit>> CritByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactCrit>>((db, n) =>
            from e in db.ZFactCrit
            where e.ZksgKpgId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactDiagBlok>> DiagBlokByMedicalEventOnkIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactDiagBlok>>((db, n) =>
            from e in db.ZFactDiagBlok
            where e.ZMedicalEventOnkId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactDs>> DsByMedicalEventIdQuery =
            CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactDs>>((db, n) =>
                from e in db.ZFactDs
                where e.ZmedicalEventId == n
                select e
            );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactAnticancerDrug>> AnticancerDrugByMedicalServiceOnkIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactAnticancerDrug>>((db, n) =>
            from e in db.ZFactAnticancerDrug
            where e.ZMedicalServiceOnkId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactContraindications>> ContraindicationsByMedicalEventOnkIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactContraindications>>((db, n) =>
            from e in db.ZFactContraindications
            where e.ZMedicalEventOnkId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<FactMEC>> MecByMedicalEventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactMEC>>((db, n) =>
            from e in db.FactMEC
            where e.MedicalEventId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactSank>> SankByMedicalEventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactSank>>((db, n) =>
            from e in db.ZFactSank
            where e.ZmedicalEventId == n
            select e
        );

        private static readonly Func<MedicineContext, int, int, IEnumerable<ZFactSank>> SankByZMedicalEventIdQueryAndType =
        CompiledQuery.Compile<MedicineContext, int, int, IEnumerable<ZFactSank>>((db, n, o) =>
            from e in db.ZFactSank
            where e.ZmedicalEventId == n && e.Type == o
            select e
        );

        private static readonly Func<MedicineContext, int, int, IEnumerable<FactMEC>> MecByMedicalEventIdAndTypeQuery =
        CompiledQuery.Compile<MedicineContext, int, int, IEnumerable<FactMEC>>((db, n, o) =>
            from e in db.FactMEC
            where e.MedicalEventId == n && e.Type == o
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<FactMEE>> MeeByMedicalEventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactMEE>>((db, n) =>
            from e in db.FactMEE
            where e.MedicalEventId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<FactEQMA>> EqmaByMedicalEventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactEQMA>>((db, n) =>
            from e in db.FactEQMA
            where e.MedicalEventId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<FactExternalRefuse>> ExternalRefusesByAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactExternalRefuse>>((db, n) =>
            from e in db.FactExternalRefuse
            where e.FKPatient.AccountId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactExternalRefuse>> ZExternalRefusesByAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactExternalRefuse>>((db, n) =>
            from e in db.ZFactExternalRefuse
            where e.FKZPatient.AccountId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<FactExternalRefuse>> ExternalRefuseByMedicalEventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactExternalRefuse>>((db, n) =>
            from e in db.FactExternalRefuse
            where e.MedicalEventId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactExternalRefuse>> ZexternalRefuseByMedicalEventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactExternalRefuse>>((db, n) =>
            from e in db.ZFactExternalRefuse
            where e.ZmedicalEventId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactExternalRefuse>> ZexternalRefuseIsAgreeByMedicalEventIdQuery =
       CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactExternalRefuse>>((db, n) =>
           from e in db.ZFactExternalRefuse
           where e.ZmedicalEventId == n && e.IsAgree == true
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<FactTerritoryAccount>> ParentTerritoryAccountByParentIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactTerritoryAccount>>((db, n) =>
            from e in db.FactTerritoryAccount
            where e.Parent == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<FactExternalRefuse>> ExternalRefuseByPatientIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactExternalRefuse>>((db, n) =>
            from e in db.FactExternalRefuse
            where e.PatientId == n
            select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactExternalRefuse>> ZExternalRefuseByPatientIdQuery =
       CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactExternalRefuse>>((db, n) =>
           from e in db.ZFactExternalRefuse
           where e.PatientId == n
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<globalMedicalAssistanceVolume>> MedicalAssistanceVolumeQuery =
        CompiledQuery.Compile<MedicineContext, IEnumerable<globalMedicalAssistanceVolume>>(db =>
            from e in db.globalMedicalAssistanceVolume
            select new globalMedicalAssistanceVolume
            {
                MedicalOrganization = e.MedicalOrganization,
                globalMedAssisVolMedicalOrg = e.globalMedAssisVolMedicalOrg,
                Profile = e.Profile,
                globalMedAssisVolProfile = e.globalMedAssisVolProfile,
                Indicator = e.Indicator,
                globalMedAssisVolIndicator = e.globalMedAssisVolIndicator,
                Amount = e.Amount,
                Volume = e.Volume
            }
        );

        private static readonly Func<MedicineContext, int, IEnumerable<PatientShortView>> PatientShortViewByPatientIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<PatientShortView>>((db, n) =>
           from e in db.PatientShortView
           where e.PatientId == n
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<FactTerritoryAccount>> TerritoryAccountByPatientIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactTerritoryAccount>>((db, n) =>
           from e in db.FactPatient
           where e.PatientId == n
           select e.FACTTERACCOUNTID
       );

        private static readonly Func<MedicineContext, int, IEnumerable<TerritoryAccountView>> TerritoryAccountViewByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<TerritoryAccountView>>((db, n) =>
           from e in db.TerritoryAccountView
           where e.TerritoryAccountId == n
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<localUserSettings>> UserSettingsByUserIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<localUserSettings>>((db, n) =>
           from e in db.localUserSettings
           where e.UserID == n
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<FactPerson>> PersonQuery =
        CompiledQuery.Compile<MedicineContext, IEnumerable<FactPerson>>(db =>
          from e in db.FactPerson
          select e
        );

        private static readonly Func<MedicineContext, IEnumerable<FactDocument>> DocumentQuery =
        CompiledQuery.Compile<MedicineContext, IEnumerable<FactDocument>>(db =>
          from e in db.FactDocument
          select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<FactSrzQuery>> SrzQueryByPatientIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactSrzQuery>>((db, n) =>
           from e in db.FactSrzQuery
           where e.PatientId == n
           orderby e.DateQuery
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<FactSrzQuery>> SrzQueryByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactSrzQuery>>((db, n) =>
           from e in db.FactSrzQuery
           where e.SrzQueryId == n
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<MedicalAccountView>> MedicalAccountViewByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<MedicalAccountView>>((db, n) =>
           from e in db.MedicalAccountView
           where e.MedicalAccountId == n
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<EventShortView>> EventShortViewByAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<EventShortView>>((db, n) =>
           from e in db.EventShortView
           where e.AccountId == n
           select e
       );
        private static readonly Func<MedicineContext, int, IEnumerable<ZslEventShortView>> ZslEventShortViewByAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZslEventShortView>>((db, n) =>
           from e in db.ZslEventShortView
           where e.AccountId == n
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<EventShortView>> EventShortViewByPatientIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<EventShortView>>((db, n) =>
           from e in db.EventShortView
           where e.PatientId == n
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<ZslEventShortView>> ZslEventShortViewByPatientIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZslEventShortView>>((db, n) =>
           from e in db.ZslEventShortView
           where e.PatientId == n
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<EventShortView>> EventShortViewByMedicalAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<EventShortView>>((db, n) =>
           from e in db.EventShortView
           where e.MedicalAccountId == n
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<ZslEventShortView>> ZslEventShortViewByMedicalAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZslEventShortView>>((db, n) =>
           from e in db.ZslEventShortView
           where e.MedicalAccountId == n
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<ZslEventView>> ZslEventViewByMedicalAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZslEventView>>((db, n) =>
           from e in db.ZslEventView
           where e.MedicalAccountId == n
           select e
       );


        private static readonly Func<MedicineContext, int, IEnumerable<FactPatient>> PatientWithErrorFromTerritoryAccountQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactPatient>>((db, n) =>
           from e in db.FactPatient
           where e.AccountId == n && e.FACTMEDIPATIENTIDFACTPATIs.Any(r => r.PaymentStatus != 2)
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<FactSrzQuery>> SrzQueriesForTerritoryAccountQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactSrzQuery>>((db, n) =>
           from e in db.FactSrzQuery
           where e.FCFACTPATPATIENTID.AccountId == n
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<EventShortView>> EventShortViewByMedicalEventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<EventShortView>>((db, n) =>
           from e in db.EventShortView
           where e.EventId == n
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<ZslEventShortView>> ZslEventShortViewByMedicalEventIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZslEventShortView>>((db, n) =>
           from e in db.ZslEventShortView
           where e.EventId == n
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<FactTerritoryAccount>> TerritoryAccountQuery =
        CompiledQuery.Compile<MedicineContext, IEnumerable<FactTerritoryAccount>>(db =>
           from e in db.FactTerritoryAccount
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<FactPatient>> PatientWithErrorFromMedicalAccountQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactPatient>>((db, n) =>
           from e in db.FactPatient
           where e.MedicalAccountId == n && e.FACTMEDIPATIENTIDFACTPATIs.Any(r => r.PaymentStatus != 2)
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<FactPatient>> ZPatientWithErrorFromMedicalAccountQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactPatient>>((db, n) =>
           from e in db.FactPatient
           where e.MedicalAccountId == n && e.ZSLFACTMEDIPATIENTIDFACTPATIs.Any(r => r.PaymentStatus != 2)
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<FactSrzQuery>> SrzQueriesForMedicalAccountQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactSrzQuery>>((db, n) =>
           from e in db.FactSrzQuery
           where e.FCFACTPATPATIENTID.MedicalAccountId == n
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<FactMedicalEvent>> MedicalEventByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactMedicalEvent>>((db, n) =>
           from e in db.FactMedicalEvent
           where e.MedicalEventId == n
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactMedicalEvent>> ZMedicalEventByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactMedicalEvent>>((db, n) =>
           from e in db.ZFactMedicalEvent
           where e.ZmedicalEventId == n
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactKsgKpg>> ZKsgKpgByIdQuery =
       CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactKsgKpg>>((db, n) =>
          from e in db.ZFactKsgKpg
          where e.ZksgKpgId == n
          select e
      );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactMedicalEventOnk>> ZMedicalEventOnkByIdQuery =
      CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactMedicalEventOnk>>((db, n) =>
         from e in db.ZFactMedicalEventOnk
         where e.ZMedicalEventOnkId == n
         select e
     );

        private static readonly Func<MedicineContext, int, IEnumerable<ZslFactMedicalEvent>> ZslMedicalEventByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZslFactMedicalEvent>>((db, n) =>
           from e in db.ZslFactMedicalEvent
           where e.ZslMedicalEventId == n
           select e
       );

        private static readonly Func<MedicineContext, int, IEnumerable<FactMedicalServices>> MedicalServiceByIdQuery =
       CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactMedicalServices>>((db, n) =>
          from e in db.FactMedicalServices
          where e.MedicalServicesId == n
          select e
      );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactDirection>> ZDirectionByIdQuery =
       CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactDirection>>((db, n) =>
          from e in db.ZFactDirection
          where e.ZDirectionId == n
          select e
      );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactMedicalServices>> ZMedicalServiceByIdQuery =
       CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactMedicalServices>>((db, n) =>
          from e in db.ZFactMedicalServices
          where e.ZmedicalServicesId == n
          select e
      );

        private static readonly Func<MedicineContext, int, IEnumerable<ZFactMedicalServicesOnk>> ZMedicalServiceOnkByIdQuery =
       CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactMedicalServicesOnk>>((db, n) =>
          from e in db.ZFactMedicalServicesOnk
          where e.ZmedicalServicesOnkId == n
          select e
      );
        private static readonly Func<MedicineContext, int, IEnumerable<ZFactConsultations>> ZMedicalConsultationsOnkByIdQuery =
       CompiledQuery.Compile<MedicineContext, int, IEnumerable<ZFactConsultations>>((db, n) =>
          from e in db.ZFactConsultations
          where e.ZConsultationsId == n
          select e
      );

        private static readonly Func<MedicineContext, int, int?, IEnumerable<FactMEC>> MecByMedicalEventIdAndSourceQuery =
       CompiledQuery.Compile<MedicineContext, int, int?, IEnumerable<FactMEC>>((db, n, o) =>
          from e in db.FactMEC
          where e.MedicalEventId == n && e.Source == o
          select e
      );

        private static readonly Func<MedicineContext, int, int?, IEnumerable<FactMEE>> MeeByMedicalEventIdAndSourceQuery =
       CompiledQuery.Compile<MedicineContext, int, int?, IEnumerable<FactMEE>>((db, n, o) =>
          from e in db.FactMEE
          where e.MedicalEventId == n && e.Source == o
          select e
      );

        private static readonly Func<MedicineContext, int, int?, IEnumerable<FactEQMA>> EqmaByMedicalEventIdAndSourceQuery =
      CompiledQuery.Compile<MedicineContext, int, int?, IEnumerable<FactEQMA>>((db, n, o) =>
         from e in db.FactEQMA
         where e.MedicalEventId == n && e.Source == o
         select e
     );

        private static readonly Func<MedicineContext, IEnumerable<localUser>> LocalUserQuery =
        CompiledQuery.Compile<MedicineContext, IEnumerable<localUser>>(db =>
            from e in db.localUser
            orderby e.LastName,e.FirstName,e.Patronymic
            select e
        );

        private static readonly Func<MedicineContext, IEnumerable<FactProcessing>> ProcessingQuery =
        CompiledQuery.Compile<MedicineContext, IEnumerable<FactProcessing>>(db =>
            from e in db.FactProcessing
            select e
        );

        private static readonly Func<MedicineContext, IEnumerable<globalProcessingType>> GlobalProcessingTypeQuery =
       CompiledQuery.Compile<MedicineContext, IEnumerable<globalProcessingType>>(db =>
           from e in db.globalProcessingType
           select e
       );

        private static readonly Func<MedicineContext, IEnumerable<shareDoctor>> ShareDoctorQuery =
        CompiledQuery.Compile<MedicineContext, IEnumerable<shareDoctor>>(db =>
           from e in db.shareDoctor
           orderby e.MedicalOrganizationCode
           select e
        );

        private static readonly Func<MedicineContext,int, IEnumerable<FactMedicalAccount>> MedicalAccountByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactMedicalAccount>>((db,n) =>
           from e in db.FactMedicalAccount
           where e.MedicalAccountId == n
           select e
        );

        private static readonly Func<MedicineContext, int, IEnumerable<FactActMee>> MeeActByTerritoryAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactActMee>>((db, n) =>
            from e in db.FactActMee
                from p in db.FactMEE
                where p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId == n && p.ActId.HasValue
            where e.ActMeeId == p.ActId
            select e);

        private static readonly Func<MedicineContext, int, IEnumerable<FactActMee>> MeeActByMedicalAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactActMee>>((db, n) =>
            from e in db.FactActMee
                from p in db.FactMEE
                where p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == n && p.ActId.HasValue
            where e.ActMeeId == p.ActId
            select e);

        private static readonly Func<MedicineContext, int, IEnumerable<FactActEqma>>EqmaActByTerritoryAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactActEqma>>((db, n) =>
            from e in db.FactActEqma
            from p in db.FactEQMA
            where p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId == n && p.ActId.HasValue
            where e.ActEqma == p.ActId
            select e);

        private static readonly Func<MedicineContext, int, IEnumerable<FactActEqma>> EqmaActByMedicalAccountIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<FactActEqma>>((db, n) =>
            from e in db.FactActEqma
            from p in db.FactEQMA
            where p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == n && p.ActId.HasValue
            where e.ActEqma == p.ActId
            select e);

        private static readonly Func<MedicineContext, IEnumerable<FactActMee>> MedicalAccountMeeActQuery =
        CompiledQuery.Compile<MedicineContext, IEnumerable<FactActMee>>((db) =>
            from e in db.FactActMee
                from p in db.FactMEE
                where p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId.HasValue && p.ActId.HasValue
            where e.ActMeeId == p.ActId
            select e);

        private static readonly Func<MedicineContext, IEnumerable<FactActMee>> TerritoryAccountMeeActQuery =
        CompiledQuery.Compile<MedicineContext, IEnumerable<FactActMee>>((db) =>
            from e in db.FactActMee
                from p in db.FactMEE
                where p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId.HasValue && p.ActId.HasValue
            where e.ActMeeId == p.ActId
            select e);

        private static readonly Func<MedicineContext, IEnumerable<FactActEqma>> MedicalAccountEqmaActQuery =
        CompiledQuery.Compile<MedicineContext, IEnumerable<FactActEqma>>((db) =>
           from e in db.FactActEqma
           from p in db.FactEQMA
            where p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId.HasValue && p.ActId.HasValue
           where e.ActEqma == p.ActId
           
           select e);

        private static readonly Func<MedicineContext, IEnumerable<FactActEqma>> TerritoryAccountEqmaActQuery =
        CompiledQuery.Compile<MedicineContext, IEnumerable<FactActEqma>>((db) =>
            from e in db.FactActEqma
            from p in db.FactEQMA
            where p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId.HasValue && p.ActId.HasValue
            where e.ActEqma == p.ActId
            select e);

        private static readonly Func<MedicineContext,int, IEnumerable<globalLicenseEntry>> LicenseEntryByIdQuery =
        CompiledQuery.Compile<MedicineContext, int, IEnumerable<globalLicenseEntry>>((db, n) =>
            from e in db.globalLicenseEntry
            where e.License_LicenseId == n
            select e);
        

        #endregion

        static MedicineRepository()
        {
            
        }

        public MedicineRepository(IAppShareSettings settings, IMessageService messageService)
        {
            _messageService = messageService;

            dynamic databaseConfig = settings.Get("database");
            _defaultProvider = databaseConfig.Provider;
            _defaultName = databaseConfig.Name;
        }

        private MedicineContext CreateContext()
        {
            return new MedicineContext(_defaultProvider, _defaultName);
        }

        public TransactionResult InsertBatch<T>(List<T> data)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult();
                try
                {
                    var insertResult = db.InsertBatch(data);
                    if (insertResult == 0)
                    {
                        result.AddError(new InvalidOperationException("Ошибка вставки данных пакетом.\r\n{0}".F(db.LastQuery)));
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult InsertOrUpdate<T>(T data){
            using (var db = CreateContext())
            {
                var result = new TransactionResult();
                try
                {
                    result.Id = db.InsertOrReplace(data);
                    if (result.Id == 0)
                    {
                        result.AddError(new InvalidOperationException("Ошибка вставки данных.\r\n{0}".F(db.LastQuery)));
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<globalVersion>> GetGlobalVersion()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalVersion>>();
                try
                {
                    result.Data = GlobalVersionQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;}
        }

        public TransactionResult<IEnumerable<globalPcel>> GetGlobalPcel()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalPcel>>();
                try
                {
                    result.Data = GlobalPcelQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<globalKslp>> GetGlobalKslp()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalKslp>>();
                try
                {
                    result.Data = GlobalKslpQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V002>> GetV002()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V002>>();
                try
                {
                    result.Data = V002Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V003>> GetV003()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V003>>();
                try
                {
                    result.Data = V003Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V005>> GetV005()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V005>>();
                try
                {
                    result.Data = V005Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V006>> GetV006()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V006>>();
                try
                {
                    result.Data = V006Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V008>> GetV008()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V008>>();
                try
                {
                    result.Data = V008Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V009>> GetV009()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V009>>();
                try
                {
                    result.Data = V009Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V010>> GetV010()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V010>>();
                try
                {
                    result.Data = V010Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V014>> GetV014()
        {
            var result = new TransactionResult<IEnumerable<V014>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = V014Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<V015>> GetV015()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V015>>();
                try
                {
                    result.Data = V015Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V020>> GetV020()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V020>>();
                try
                {
                    result.Data = V020Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V021>> GetV021()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V021>>();
                try
                {
                    result.Data = V021Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V024>> GetV024()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V024>>();
                try
                {
                    result.Data = db.V024.Distinct(p=>p.IDDKK).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V025>> GetV025()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V025>>();
                try
                {
                    result.Data = V025Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V004>> GetV004()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V004>>();
                try
                {
                    result.Data = V004Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<M001>> GetM001()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<M001>>();
                try
                {
                    result.Data = M001Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V023>> GetV023()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V023>>();
                try
                {
                    result.Data = V023Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<F001>> GetF001()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<F001>>();
                try
                {
                    result.Data = F001Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<F002>> GetF002()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<F002>>();
                try
                {
                    result.Data = F002Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<F003>> GetF003ByOkato(string okato)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<F003>>();
                try
                {
                    result.Data = F003ByOkatoQuery(db, okato).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<F003>> GetF003()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<F003>>();
                try
                {
                    result.Data = db.F003.Distinct(p=>p.mcod).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<F004>> GetF004()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<F004>>();
                try
                {
                    result.Data = db.F004.ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }


        public TransactionResult<IEnumerable<F005>> GetF005()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<F005>>();
                try
                {
                    result.Data = F005Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<F006>> GetF006()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<F006>>();
                try
                {
                    result.Data = F006Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<F006>> GetF006(Expression<Func<F006, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<F006>>();
                try
                {
                    result.Data = predicate == null ?
                       F006Query(db).ToList() :
                       db.F006.Where(predicate).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<F008>> GetF008()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<F008>>();
                try
                {
                    result.Data = F008Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<F010>> GetF010()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<F010>>();
                try
                {
                    result.Data = F010Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<globalVidControl>> GetVidControl()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalVidControl>>();
                try
                {
                    result.Data = db.globalVidControl.ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<globalActExpertiseStatus>> GetActExpertiStatus()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalActExpertiseStatus>>();
                try
                {
                    result.Data = db.globalActExpertiseStatus.ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<globalDirectionView>> GetDirectionView()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalDirectionView>>();
                try
                {
                    result.Data = db.globalDirectionView.ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<globalMetIssl>> GetMetIssl()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalMetIssl>>();
                try
                {
                    result.Data = db.globalMetIssl.ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<F014>> GetF014()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<F014>>();
                try
                {
                    result.Data = F014Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }
        
        public TransactionResult<IEnumerable<V012>> GetV012()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V012>>();
                try
                {
                    result.Data = V012Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V026>> GetV026()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V026>>();
                try
                {
                    result.Data = V026Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V027>> GetV027()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V027>>();
                try
                {
                    result.Data = V027Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V028>> GetV028()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V028>>();
                try
                {
                    result.Data = V028Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<V029>> GetV029()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<V029>>();
                try
                {
                    result.Data = V029Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<N007>> GetN007()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<N007>>();
                try
                {
                    result.Data = N007Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<N008>> GetN008()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<N008>>();
                try
                {
                    result.Data = N008Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<N010>> GetN010()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<N010>>();
                try
                {
                    result.Data = N010Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<N011>> GetN011()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<N011>>();
                try
                {
                    result.Data = N011Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<N013>> GetN013()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<N013>>();
                try
                {
                    result.Data = N013Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<N014>> GetN014()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<N014>>();
                try
                {
                    result.Data = N014Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<N015>> GetN015()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<N015>>();
                try
                {
                    result.Data = db.N015.ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<N016>> GetN016()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<N016>>();
                try
                {
                    result.Data = db.N016.ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<N017>> GetN017()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<N017>>();
                try
                {
                    result.Data = db.N017.ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<N018>> GetN018()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<N018>>();
                try
                {
                    result.Data = db.N018.ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<N019>> GetN019()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<N019>>();
                try
                {
                    result.Data = db.N019.ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<N020>> GetN020()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<N020>>();
                try
                {
                    result.Data = db.N020.ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<FactExchange>> GetFactExchange(Expression<Func<FactExchange, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactExchange>>();
                try
                {
                    result.Data = predicate == null ? 
                        FactExchangeQuery(db).ToList() : 
                        db.FactExchange.Where(predicate).ToList();
                    
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<FactTerritoryAccount>> GetFactTerritoryAccount(Expression<Func<FactTerritoryAccount, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactTerritoryAccount>>();
                try
                {
                    result.Data = predicate == null ?
                        db.FactTerritoryAccount.ToList() :
                        db.FactTerritoryAccount.Where(predicate).OrderBy(p=>p.Date).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<EventShortView>> GetEventShortView(Expression<Func<EventShortView, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<EventShortView>>();
                try
                {
                    result.Data = predicate == null ?
                        EventShortViewQuery(db).ToList() :
                        db.EventShortView.Where(predicate).OrderBy(p=>p.ExternalId).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<ZslEventShortView>> GetZslEventShortView(Expression<Func<ZslEventShortView, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<ZslEventShortView>>();
                try
                {
                    result.Data = predicate == null ?
                        ZslEventShortViewQuery(db).ToList() :
                        db.ZslEventShortView.Where(predicate).OrderBy(p => p.ExternalId).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<GeneralEventShortView>> GetGeneralEventShortView(Expression<Func<GeneralEventShortView, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<GeneralEventShortView>>();
                try
                {
                    result.Data = predicate == null ?
                        GeneralEventShortViewQuery(db).ToList() :
                        db.GeneralEventShortView.Where(predicate).OrderBy(p => p.ExternalId).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<EventExtendedView>> GetEventExtendedView(Expression<Func<EventExtendedView, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<EventExtendedView>>();
                try
                {
                    result.Data = predicate == null ?
                        EventExtendedViewQuery(db).ToList() :
                        db.EventExtendedView.Where(predicate).OrderBy(p => p.ExternalId).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<PatientShortView>> GetPatientShortView(Expression<Func<PatientShortView, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<PatientShortView>>();
                try
                {
                    result.Data = predicate == null ?
                        PatientShortViewQuery(db).ToList() :
                        db.PatientShortView.Where(predicate).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<MedicalAccountView>> GetMedicalAccountView(Expression<Func<MedicalAccountView, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<MedicalAccountView>>();
                try
                {
                    result.Data = predicate == null ?
                        MedicalAccountViewQuery(db).ToList() :
                        db.MedicalAccountView.Where(predicate).OrderBy(p=>p.Date).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<globalExaminationGroup>> GetGlobalExaminationGroup()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalExaminationGroup>>();
                try
                {
                    result.Data = GlobalExaminationGroupQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<globalPaymentStatus>> GetGlobalPaymentStatus()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalPaymentStatus>>();
                try
                {
                    result.Data = GlobalPaymentStatusQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<globalParticularSign>> GetGlobalParticularSign()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalParticularSign>>();
                try
                {
                    result.Data = GlobalParticularSignQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<globalOldProfile>> GetGlobalOldProfile()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalOldProfile>>();
                try
                {
                    result.Data = GlobalOldProfileQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<globalRefusalSource>> GetGlobalRefusalSource()
        {
            var result = new TransactionResult<IEnumerable<globalRefusalSource>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = GlobalRefusalSourceQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<localEmployee>> GetLocalEmployee()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<localEmployee>>();
                try
                {
                    result.Data = LocalEmployeeQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<globalExaminationType>> GetGlobalExaminationType()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalExaminationType>>();
                try
                {
                    result.Data = GlobalExaminationTypeQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<globalReportType>> GetGlobalReportType()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalReportType>>();
                try
                {
                    result.Data = GlobalReportTypeQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<globalParam>> GetGlobalParam()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalParam>>();
                try
                {
                    result.Data = GlobalParamQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<globalScope>> GetGlobalScope()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalScope>>();
                try
                {
                    result.Data = GlobalScopeQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<F011>> GetF011()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<F011>>();
                try
                {
                    result.Data = F011Query(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<localUser> GetUser(string login)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<localUser>();
                try
                {
                    result.Data = GetUserQuery(db, login).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<localUser> GetUserById(int id)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<localUser>();
                try
                {
                    result.Data = db.localUser.FirstOrDefault(p => p.UserID == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        

        public TransactionResult InsertWithIdentity<T>(T data)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int>();
                try
                {
                    result.Id = Convert.ToInt32(db.InsertWithIdentity(data));
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<FactExpertCriterion>> GetFactExpertCriterion(Expression<Func<FactExpertCriterion, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactExpertCriterion>>();
                try
                {
                    result.Data = predicate == null ?
                        ExpertCriterionQuery(db).ToList() :
                        db.FactExpertCriterion.Where(predicate).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<int> ChangeTerritoryAccountStatus(int territoryAccountId, int? status)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int>();
                try
                {
                    result.Data = db.FactTerritoryAccount
                            .Where(p => p.TerritoryAccountId == territoryAccountId)
                            .Set(s => s.Status, status)
                            .Update();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<int> ChangeMedicalAccountStatus(int medicalAccountId, int? status)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int>();
                try
                {
                    result.Data = db.FactMedicalAccount
                            .Where(p => p.MedicalAccountId == medicalAccountId)
                            .Set(s => s.Status, status)
                            .Update();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<FactPatient> GetPatient(int patientId)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<FactPatient>();
                try
                {
                    result.Data = FactPatientQuery(db, patientId).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<PatientShortView> GetPatientShortViewByPatientId(int patientId)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<PatientShortView>();
                try
                {
                    result.Data = PatientShortViewByPatientIdQuery(db, patientId).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<Tuple<FactPatient, FactDocument, FactPerson>> GetPatientDocumentPerson(int patientId)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<Tuple<FactPatient, FactDocument, FactPerson>>();
                try
                {
                    result.Data = FactPatientQueryFull(db, patientId).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<List<int>> GetPatientsIdsByAccountId(int id)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<List<int>>();
                try
                {
                    result.Data = FactPatientIdsByAccountIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<List<int>> GetPatientsIdsBySelectZslid(IEnumerable<int> ids)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<List<int>>();
                try
                {
                    var idPat = from p in db.FactPatient
                        join zsl in db.ZslFactMedicalEvent on p.PatientId equals zsl.PatientId
                        where ids.Contains(zsl.ZslMedicalEventId)
                        select new {p.PatientId};
                    result.Data = idPat.Select(x=>x.PatientId).ToList();
                    // db.FactPatient.Where(p=>db.ZslFactMedicalEvent.Where(x => ids.Contains(x.ZslMedicalEventId)).Select(x => x.PatientId).Contains(p.PatientId));
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<List<int>> GetPatientsIdsBySelectSlid(IEnumerable<int> ids)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<List<int>>();
                try
                {
                    var idPat = from p in db.FactPatient
                                join sl in db.FactMedicalEvent on p.PatientId equals sl.PatientId
                                where ids.Contains(sl.MedicalEventId)
                                select new { p.PatientId };
                    result.Data = idPat.Select(x => x.PatientId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<int> Update<T>(T obj)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int>();
                try
                {
                    result.Data = db.Update(obj);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<FactActExpertise>> GetFactActExpertise(Expression<Func<FactActExpertise, bool>> predicate)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactActExpertise>>();
                try
                {
                    result.Data = predicate == null ?
                        FactActExpertiseQuery(db).ToList() :
                        db.FactActExpertise.Where(predicate).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<ActExpertiseShortView>> GetActExpertiseShortView(Expression<Func<ActExpertiseShortView, bool>> predicate)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<ActExpertiseShortView>>();
                try
                {
                    result.Data = predicate == null ?
                        db.ActExpertiseShortView.ToList() :
                        db.ActExpertiseShortView.Where(predicate).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<FactReport>> GetFactReport(Expression<Func<FactReport, bool>> predicate)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactReport>>();
                try
                {
                    result.Data = predicate == null ?
                        FactReportQuery(db).ToList() :
                        db.FactReport.Where(predicate).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<int> DeleteReport(int id)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int>();
                try
                {
                    result.Data = db.FactReport.Delete(p => p.FactReportID == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<FactPreparedReport>> GetFactPreparedReport(Expression<Func<FactPreparedReport, bool>> predicate)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactPreparedReport>>();
                try
                {
                    result.Data = predicate == null ?
                        FactPreparedReportQuery(db).ToList() :
                        db.FactPreparedReport.Where(predicate).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        //public TransactionResult<IEnumerable<FactPreparedReportNotBody>> GetFactPreparedReportNotBody(Expression<Func<FactPreparedReportNotBody, bool>> predicate)
        //{
        //    using (var db = CreateContext())
        //    {
        //        var result = new TransactionResult<IEnumerable<FactPreparedReportNotBody>>();
        //        try
        //        {
        //            result.Data = predicate == null ?
        //                FactPreparedReportQueryNotBody(db).ToList() :
        //                db.FactPreparedReportNotBody.Where(predicate).ToList();

        //        }
        //        catch (Exception exception)
        //        {
        //            result.AddError(exception);
        //        }
        //        return result;
        //    }
        //}

        public TransactionResult<int> DeletePreparedReport(int id)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int>();
                try
                {
                    result.Data = db.FactPreparedReport.Delete(p => p.PreparedReportId == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<FactReport>> GetEnabledReportsByScope(int scope)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactReport>>();
                try
                {
                    result.Data = EnabledFactReportByScopeQuery(db, scope).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<FactReport>> GetEnabledReportsByScopeByVersion(int scope, int version)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactReport>>();
                try
                {
                    result.Data = EnabledFactReportByScopeByVersionQuery(db, scope, version).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<localEmployee>> GetEmployee()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<localEmployee>>();
                try
                {
                    result.Data = EmployeeQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<FactReport> GetReportById(int id)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<FactReport>();
                try
                {
                    result.Data = ReportByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult InsertPreparedReport(int id, int scope, int reportId, string name, int? subId = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult();
                try
                {
                     var exist = db.FactPreparedReport.FirstOrDefault(p => p.ExternalId == id &&
                                                                  p.Scope == scope &&
                                                                  p.ReportId == reportId &&
                                                                  p.SubId == subId);
                     if (exist == null)
                     {
                         int? number = db.FactPreparedReport
                             .Where(p => p.Scope == scope && p.ReportId == reportId &&
                             //Add reset document number every year
                             p.Date.Year == Sql.CurrentTimestamp.Year)
                             .Max(p => (int?)p.Number);
                         number = number.HasValue ? number + 1 : 1;
                         var preparedReport = new FactPreparedReport
                         {
                             ExternalId = id,
                             SubId = subId,
                             Scope = scope,
                             Date = Sql.Date,
                             Name = name,
                             Number = number.Value,
                             ReportId = reportId,
                             Body = new byte[0]
                         };
                         result.Id = SafeConvert.ToInt32(db.InsertWithIdentity(preparedReport)) ?? 0;
                     }
                     else
                     {
                         result.Id = exist.PreparedReportId;
                     }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult UpdatePreparedReport(int id, byte[] body, int pageCount)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult();
                try
                {
                    var exist = db.FactPreparedReport.FirstOrDefault(p => p.PreparedReportId == id);
                    if (exist != null)
                    {
                        result.Id = db.FactPreparedReport
                        .Where(e => e.PreparedReportId == exist.PreparedReportId)
                        .Set(e => e.Body, body)
                        .Set(e => e.PageCount, pageCount)
                        .Update();
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<FactPreparedReport>> GetPreparedReportByExternalIdScopeSubId(int id, int scope, int? subId)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactPreparedReport>>();
                try
                {
                    result.Data = PreparedReportByExternalIdAndScopeQuery(db, id, scope, subId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<FactPreparedReport> GetPreparedReportById(int id)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<FactPreparedReport>();
                try
                {
                    result.Data = PreparedReportByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public bool IsPreparedReportByExternalIdExists(int id, int scope,int? subId = null)
        {
            using (var db = CreateContext())
            {
                try
                {
                    var data = PreparedReportByExternalIdAndScopeQuery(db, id, scope, subId).ToList();
                    return data.Count > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public TransactionResult<int> DeleteExpertCriterion(int id)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int>();
                try
                {
                    result.Data = db.FactExpertCriterion.Delete(p => p.FactExpertCriterionID == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<FactExpertCriterion>> GetEnabledExpertCriterionByScopeAndVersion(int scope, int version)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactExpertCriterion>>();
                try
                {
                    result.Data = EnabledExpertCriterionByScopeAndVersionQuery(db, scope, version).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<FactExpertCriterion>> GetEnabledExpertCriterionByIds(IEnumerable<object> ids)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactExpertCriterion>>();
                try
                {
                    result.Data = db.FactExpertCriterion.Where(p=>ids.Contains(p.FactExpertCriterionID)).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        #region AccountOperations

        public TransactionResult ClearAccountRefusal(int id, IEnumerable<int> examsList)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                //Delete all auto errors that's exist in list
                List<int?> checks = db.FactExpertCriterion
                    .Where(p => examsList.Contains(p.FactExpertCriterionID))
                    .Select(p => p.Reason).ToList();

                db.GetTableQuery<FactMEC>()
                    .Delete(p =>
                        p.MedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == id &&
                        p.Type == 1 &&
                        (p.IsLock == null || p.IsLock == false) &&
                        checks.Contains(p.ReasonId));

                var mevents = db.FactMedicalEvent
                    .Where(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == id)
                    .ToList();

                foreach (var mevent in mevents)
                {
                    UpdateEvent(db, mevent.MedicalEventId);
                }
            }

            return result;
        }
        public virtual void AddRefusal(MedicineContext db, FactMedicalEvent mevent, FactExpertCriterion criterionObject, decimal? price)
        {
            FactMEC doubles = db.GetTableQuery<FactMEC>().FirstOrDefault(p => p.MedicalEventId == mevent.MedicalEventId &&
                                                                              p.PatientId == mevent.PatientId &&
                                                                              p.ReasonId == criterionObject.Reason.Value);

            var externalAmount = db.GetTableQuery<FactExternalRefuse>().Where(p => p.MedicalEventId == mevent.MedicalEventId).Sum(p => p.Amount) ?? 0;
            if (price == 0)
                return;


            price -= externalAmount;
            if (price == 0)
                return;

            if (doubles == null)
            {
                var result = db.InsertWithIdentity(new FactMEC
                {
                    EmployeeId = Constants.SystemAccountId,
                    MedicalEventId = mevent.MedicalEventId,
                    PatientId = mevent.PatientId,
                    ReasonId = criterionObject.Reason,
                    Amount = price * criterionObject.RefusalPercent + (price * criterionObject.PenaltyPercent),
                    Comments = "Auto",
                    Date = DateTime.Today,
                    Type = 1
                });

                if (!Convert.ToBoolean(result))
                {
                    //UnityService.Instance.Resolve<MessageManager>().ShowError("Ошибка записи  МЭК в БД");
                }

            }


            UpdateEvent(db, mevent.MedicalEventId);
        }

        public void UpdateEvent(MedicineContext db, int meventId)
        {
            FactMedicalEvent mevent = db.GetTableQuery<FactMedicalEvent>().FirstOrDefault(p => p.MedicalEventId == meventId);
            if (mevent == null)
                return;

            decimal mec = db.GetTableQuery<FactMEC>()
                .Where(p => p.MedicalEventId == meventId && (p.IsLock == null || p.IsLock == false))
                .Sum(p => p.Amount) ?? 0;

            decimal mee = db.GetTableQuery<FactMEE>()
                .Where(p => p.MedicalEventId == meventId)
                .Sum(p => p.Amount) ?? 0;
            decimal eqma = db.GetTableQuery<FactEQMA>()
                .Where(p => p.MedicalEventId == meventId)
                .Sum(p => p.Amount) ?? 0;
            decimal external = db.GetTableQuery<FactExternalRefuse>()
                .Where(p => p.MedicalEventId == meventId && (p.IsAgree == true || p.IsAgree == null))
                .Sum(p => p.Amount) ?? 0;

            decimal price = mevent.Price ?? 0;
            int payStatus = 2;
            decimal? acceptPrice = mevent.Price - Math.Min((mec + mee + eqma + external), price);

            if (acceptPrice == 0)
            {
                payStatus = 3;
            }
            else if (acceptPrice < mevent.Price)
            {
                payStatus = 4;
            }

            db.GetTableQuery<FactMedicalEvent>().Where(s => s.MedicalEventId == meventId)
                .Set(s => s.MEC, Math.Min((mec + external), price))
                .Set(s => s.MEE, Math.Min((mee), price))
                .Set(s => s.EQMA, Math.Min((eqma), price))
                .Set(s => s.PaymentStatus, payStatus)
                .Set(s => s.AcceptPrice, acceptPrice)
                .Update();

        }

        public TransactionResult UpdateTerritoryAccount(int? accountId)
        {
            TransactionResult result;

            using (var db = CreateContext())
            {
                result = UpdateTerritoryAccount(db, accountId);
            }

            return result;
        }

        public TransactionResult UpdateTerritoryAccount(MedicineContext db, int? accountId)
        {
            var result = new TransactionResult();
            try
            {
                FactTerritoryAccount account = db.GetTableQuery<FactTerritoryAccount>().FirstOrDefault(p => p.TerritoryAccountId == accountId);
                if (account == null)
                {
                    throw new ArgumentException("TerritoryAccount ID {0} не найден".F(accountId));
                }

                //Получаем все МЭКи не блокированные и с источником 'ТФОМС2 к ТФОМС1' или 'Уточнённые санкции ТФОМС2 к ТФОМС1'
                var mecList = db.GetTableQuery<FactMEC>()
                    .Distinct()
                    .Where(p => p.Patient.AccountId == accountId &&
                        (p.IsLock == null || p.IsLock == false) &&
                        (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal)).ToList();

                decimal mec = mecList.Distinct(p => p.MedicalEventId).Sum(p => p.Amount) ?? 0;


                //Получаем все МЭЭ
                decimal mee = db.GetTableQuery<FactMEE>()
                    .Where(p => p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                        (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal))
                    .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;
                //Получаем все ЭКМП
                decimal eqma = db.GetTableQuery<FactEQMA>()
                    .Where(p => p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                        (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal))
                    .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;

                var external = db.GetTableQuery<FactExternalRefuse>()
                    .Where(p => p.FKMedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                        (p.Type == null || p.Type == (int)RefusalType.Unknown) &&
                        (p.IsAgree == true))
                    .Sum(p => p.Amount) ?? 0;

                var externalMec = db.GetTableQuery<FactExternalRefuse>()
                   .Where(p => p.FKMedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                       p.Type == (int)RefusalType.MEC &&
                       (p.IsAgree == true))
                   .Sum(p => p.Amount) ?? 0;

                var externalMee = db.GetTableQuery<FactExternalRefuse>()
                   .Where(p => p.FKMedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                       p.Type == (int)RefusalType.MEE &&
                       (p.IsAgree == true))
                   .Sum(p => p.Amount) ?? 0;

                var externalEqma = db.GetTableQuery<FactExternalRefuse>()
                   .Where(p => p.FKMedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                       p.Type == (int)RefusalType.EQMA &&
                       (p.IsAgree == true))
                   .Sum(p => p.Amount) ?? 0;

                //Получаем сумму выставленную к оплате
                decimal price = db.FactMedicalEvent
                    .Where(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId)
                    .Sum(p => p.Price) ?? 0;

                //считаем принятую к оплате сумму
                decimal? acceptPrice = price - (mec + mee + eqma + external + externalMec + externalMee + externalEqma);

                //обновляем запись счета
                db.GetTableQuery<FactTerritoryAccount>().Where(s => s.TerritoryAccountId == accountId)
                   .Set(s => s.Price, price)
                   .Set(s => s.MECPenalties, mec + externalMec)
                   .Set(s => s.MEEPenalties, mee + externalMee)
                   .Set(s => s.EQMAPenalties, eqma + externalEqma)
                   .Set(s => s.AcceptPrice, acceptPrice)
                   .Update();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult UpdateZTerritoryAccount(int? accountId)
        {
            TransactionResult result;

            using (var db = CreateContext())
            {
                result = UpdateZTerritoryAccount(db, accountId);
            }

            return result;
        }

        public TransactionResult UpdateZTerritoryAccount(MedicineContext db, int? accountId)
        {
            var result = new TransactionResult();

            try
            {
                FactTerritoryAccount account = db.GetTableQuery<FactTerritoryAccount>().FirstOrDefault(p => p.TerritoryAccountId == accountId);
                if (account == null)
                {
                    throw new ArgumentException("TerritoryAccount ID {0} не найден".F(accountId));
                }

                //Получаем все МЭКи не блокированные и с источником 'ТФОМС2 к ТФОМС1' или 'Уточнённые санкции ТФОМС2 к ТФОМС1'
                //(p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal)
                var mecList = db.GetTableQuery<ZFactSank>()
                    .Distinct()
                    .Where(p => p.FactPatient.AccountId == accountId &&
                        (p.IsLock == null || p.IsLock == false) &&
                        p.Type == 1 ).ToList();

                decimal mec = mecList.Distinct(p => p.ZslMedicalEventId).Sum(p => p.ZslAmount) ?? 0;


                //Получаем все МЭЭ
                decimal mee = db.GetTableQuery<ZFactSank>()
                    .Where(p => p.FactPatient.AccountId == accountId &&
                        Constants.Mee.Contains(p.Type) )
                    .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;
                //Получаем все ЭКМП
                decimal eqma = db.GetTableQuery<ZFactSank>()
                    .Where(p => p.FactPatient.AccountId == accountId &&
                                Constants.Eqma.Contains(p.Type) )
                    .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;

                var external = db.GetTableQuery<ZFactExternalRefuse>()
                    .Where(p => p.FKZPatient.AccountId == accountId &&
                        (p.Type == null || p.Type == (int)RefusalType.Unknown) &&
                        (p.IsAgree == true))
                    .Sum(p => p.Amount) ?? 0;

                var externaList = db.GetTableQuery<ZFactExternalRefuse>()
                   .Where(p => p.FKZPatient.AccountId == accountId &&
                       p.Type == (int)RefusalType.MEC &&
                       (p.IsAgree == true));

                var externalMec = externaList.Distinct(p => p.ZslMedicalEventId).Sum(p => p.ZslAmount) ?? 0;

                var externalMee = db.GetTableQuery<ZFactExternalRefuse>()
                   .Where(p => p.FKZPatient.AccountId == accountId &&
                      Constants.Mee.Contains(p.Type) &&
                       (p.IsAgree == true))
                   .Sum(p => p.Amount) ?? 0;

                var externalEqma = db.GetTableQuery<ZFactExternalRefuse>()
                   .Where(p => p.FKZPatient.AccountId == accountId &&
                       Constants.Eqma.Contains(p.Type) &&
                       (p.IsAgree == true))
                   .Sum(p => p.Amount) ?? 0;

                //Получаем сумму выставленную к оплате
                decimal price = db.ZslFactMedicalEvent
                    .Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == accountId)
                    .Sum(p => p.Price) ?? 0;

                //считаем принятую к оплате сумму
                decimal? acceptPrice = price - (mec + mee + eqma + external + externalMec + externalMee + externalEqma);

                //обновляем запись счета
                db.GetTableQuery<FactTerritoryAccount>().Where(s => s.TerritoryAccountId == accountId)
                   .Set(s => s.Price, price)
                   .Set(s => s.MECPenalties, mec + externalMec)
                   .Set(s => s.MEEPenalties, mee + externalMee)
                   .Set(s => s.EQMAPenalties, eqma + externalEqma)
                   .Set(s => s.AcceptPrice, acceptPrice)
                   .Update();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public void AddRefusal(FactMedicalEvent mevent, FactExpertCriterion criterionObject, decimal? price)
        {
            using (var db = CreateContext())
            {
                FactMEC doubles = db.GetTableQuery<FactMEC>().FirstOrDefault(p => p.MedicalEventId == mevent.MedicalEventId &&
                                                                                  p.PatientId == mevent.PatientId &&
                                                                                  p.ReasonId == criterionObject.Reason.Value);

                var externalAmount = GetExternalRefusalAmount(db, mevent.MedicalEventId);
                if (price == 0)
                    return;


                price -= externalAmount;
                if (price == 0)
                    return;

                if (doubles == null)
                {
                    var result = db.InsertWithIdentity(new FactMEC
                    {
                        EmployeeId = Constants.SystemAccountId,
                        MedicalEventId = mevent.MedicalEventId,
                        PatientId = mevent.PatientId,
                        ReasonId = criterionObject.Reason,
                        Amount = price * criterionObject.RefusalPercent + (price * criterionObject.PenaltyPercent),
                        Comments = "Auto",
                        Date = DateTime.Today,
                        Type = 1
                    });

                    if (!Convert.ToBoolean(result))
                    {
                        _messageService.ShowError("Ошибка записи  МЭК в БД");
                    }
                }

                UpdateEvent(db, mevent.MedicalEventId);
            }

        }

        public void AddRefusal(FactMEC mec)
        {
            using (var db = CreateContext())
            {
                try
                {
                    decimal? price = mec.Amount;
                    var externalAmount = GetExternalRefusalAmount(db, mec.MedicalEventId);

                    price -= externalAmount;
                    if (price <= 0)
                        return;

                    var result = db.InsertWithIdentity(mec);
                    if (!Convert.ToBoolean(result))
                    {
                        _messageService.ShowError("Ошибка записи  МЭК в БД");
                    }
                }
                catch (Exception exception)
                {
                    _messageService.ShowException(exception, "При добавлении ошибки МЭК произошло исключение", typeof(MedicineRepository));
                }
            }

        }

        public void RemoveRefusal(int medicalEventId, int reasonId, int type)
        {
            using (var db = CreateContext())
            {
                try
                {
                    db.GetTableQuery<FactMEC>().Delete(p => p.MedicalEventId == medicalEventId && p.ReasonId == reasonId && p.Type == type);
                }
                catch (Exception exception)
                {
                    _messageService.ShowException(exception, "При удалении ошибки МЭК произошло исключение", typeof(MedicineRepository));
                }
            }

        }

        public void RemoveRefusalByAccount(int accountId, List<int?> checks, int type = 1)
        {
            using (var db = CreateContext())
            {
                try
                {
                    db.GetTableQuery<FactMEC>().Delete(
                    p =>
                    p.MedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                    p.Type == 1 &&
                    (p.IsLock == null || p.IsLock == false) &&
                    checks.Contains(p.ReasonId));
                }
                catch (Exception exception)
                {
                    _messageService.ShowException(exception, "При удалении ошибки МЭК произошло исключение", typeof(MedicineRepository));
                }
            }

        }

        public decimal GetExternalRefusalAmount(int medicalEventId)
        {
            using (var db = CreateContext())
            {
                return GetExternalRefusalAmount(db, medicalEventId);
            }
        }

        public decimal GetExternalRefusalAmount(MedicineContext db, int? medicalEventId)
        {
            try
            {
                return db.GetTableQuery<FactExternalRefuse>().Where(p => p.MedicalEventId == medicalEventId).Sum(p => p.Amount) ?? 0;
            }
            catch (Exception exception)
            {
                _messageService.ShowException(exception, "При добавлении ошибки МЭК произошло исключение", typeof(MedicineRepository));
            }
            return 0;
        }
        #endregion

        public TransactionResult<int> GetPatientCountByAccount(int id)
        {
            var result = new TransactionResult<int>();
            try
            {
                using (var db = CreateContext())
                {
                    result.Data = db.FactPatient.Count(p => p.AccountId == id);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<int> GetEventCountByAccount(int id)
        {
            var result = new TransactionResult<int>();
            try
            {
                using (var db = CreateContext())
                {
                    result.Data = db.FactMedicalEvent.Count(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == id);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<int> GetServiceCountByAccount(int id)
        {
            var result = new TransactionResult<int>();
            try
            {
                using (var db = CreateContext())
                {
                    result.Data = db.FactMedicalServices.Count(p => p.FACTMEDIFMSMEIDFACTMEDI.FACTMEDIPATIENTIDFACTPATI.AccountId == id);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        
        public TransactionResult<IEnumerable<int?>> ExecuteExam(int criterionId, int id, int scope)
        {
            var scopeHandler = new Dictionary<int, string>
            {
                {1, "@MedicalAccountID"},
                {2, "@AccountID"},
                {3, "@PatientID"},
                {4, "@EventID"},
                {5, "@ServiceID"}
            };
            var result = new TransactionResult<IEnumerable<int?>>();
            try
            {
                using (var db = CreateContext())
                {
                    db.Command.CommandTimeout = 0;
                    var param = new List<IDbDataParameter>();
                    if (!scopeHandler.ContainsKey(scope))
                    {
                        
                        result.AddError("Invalid scope");
                        return result;
                    }
                    param.Add(db.Parameter(scopeHandler[scope], id));

                    FactExpertCriterion criterionObject = db.FactExpertCriterion.FirstOrDefault(p => p.FactExpertCriterionID == criterionId);
                    if (criterionObject == null || string.IsNullOrWhiteSpace(criterionObject.Query))
                    {
                        result.AddError(string.Format("Exam's Query null or empty ID :{0}",
                                                          criterionObject != null
                                                              ? criterionObject.FactExpertCriterionID
                                                              : 0));
                        return result;
                    }
                    db.SetCommand(criterionObject.Query, param.ToArray());
                    result.Data = db.ExecuteScalarList<int?>();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<int> ExecuteProcessing(int processingId, int id, int scope)
        {
            var scopeHandler = new Dictionary<int, string>
            {
                {1, "@MedicalAccountID"},
                {2, "@AccountID"},
                {3, "@PatientID"},
                {4, "@EventID"},
                {5, "@ServiceID"}
            };
            var result = new TransactionResult<int>();
            try
            {
                using (var db = CreateContext())
                {
                    db.Command.CommandTimeout = 0;
                    var param = new List<IDbDataParameter>();
                    if (!scopeHandler.ContainsKey(scope))
                    {
                        result.AddError("Invalid scope");
                        return result;
                    }
                    param.Add(db.Parameter(scopeHandler[scope], id));

                    FactProcessing processingObject = db.FactProcessing.FirstOrDefault(p => p.ProcessingId == processingId);
                    if (processingObject == null || string.IsNullOrWhiteSpace(processingObject.Query))
                    {
                        result.AddError(string.Format("Exam's Query null or empty ID :{0}",
                                                          processingObject != null
                                                              ? processingObject.ProcessingId
                                                              : 0));
                        return result;
                    }
                    db.SetCommand(processingObject.Query, param.ToArray());
                    result.Data = db.ExecuteNonQuery();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<decimal?> GetAmountByScopeAndId(int scope, int id, int version)
        {
            
            var result = new TransactionResult<decimal?>();
            try
            {
                using (var db = CreateContext())
                {
                    if (version == Constants.Version30)
                    {
                        switch (scope)
                        {
                            case 2:
                                result.Data = db.ZslFactMedicalEvent.Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id).Sum(p => p.Price);
                                break;
                            case 3:
                                result.Data = db.ZslFactMedicalEvent.Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.PatientId == id).Sum(p => p.Price);
                                break;
                            case 9:
                                result.Data = db.ZslFactMedicalEvent.Where(p => p.ZslMedicalEventId == id).Sum(p => p.Price);
                                break;
                            case 5:
                                result.Data = db.ZslFactMedicalEvent.Where(p => p.ZFactMedicalEvents.Any(r => r.ZFactMedicalServicess.Any(y=>y.ZmedicalServicesId == id))).Sum(p => p.Price);
                                break;

                        }
                    }
                    else
                    {
                        switch (scope)
                        {
                            case 2:
                                result.Data = db.FactMedicalEvent.Where(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == id).Sum(p => p.Price);
                                break;
                            case 3:
                                result.Data = db.FactMedicalEvent.Where(p => p.FACTMEDIPATIENTIDFACTPATI.PatientId == id).Sum(p => p.Price);
                                break;
                            case 4:
                                result.Data = db.FactMedicalEvent.Where(p => p.MedicalEventId == id).Sum(p => p.Price);
                                break;
                            case 5:
                                result.Data = db.FactMedicalEvent.Where(p => p.FACTMEDIFMSMEIDFACTMEDIs.Any(r => r.MedicalServicesId == id)).Sum(p => p.Price);
                                break;

                        }
                    }
                   
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<FactEconomicAccount> GetEconomicAccountById(int id)
        {
            var result = new TransactionResult<FactEconomicAccount>();
            try
            {
                using (var db = CreateContext())
                {
                    result.Data = EconomicAccountByIdQuery(db, id).FirstOrDefault();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        //begin Ashurkova
        public TransactionResult<IQueryable<FactEconomicAccount>> GetEconomicAccountParametrById(string paymentOrder,  DateTime? paymentDate)
        {
            var result = new TransactionResult<IQueryable<FactEconomicAccount>>();
            try
            {
                using (var db = CreateContext())
                {
                   result.Data = db.FactEconomicAccount.Where(x=>x.PaymentDate == paymentDate && x.PaymentOrder == paymentOrder);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }
        //end Ashurkova

        public TransactionResult<IEnumerable<FactEconomicPayment>> GetPaymentsByEconomicAccountId(int id)
        {
            var result = new TransactionResult<IEnumerable<FactEconomicPayment>>();
            try
            {
                using (var db = CreateContext())
                {
                    result.Data = PaymentsByEconomicAccountIdQuery(db, id).ToList();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactEconomicPayment>> GetExpectedPaymentsByAccountId(int id)
        {
            var result = new TransactionResult<IEnumerable<FactEconomicPayment>>();
            try
            {
                using (var db = CreateContext())
                {
                    FactTerritoryAccount account =
                        db.GetTableQuery<FactTerritoryAccount>().FirstOrDefault(p => p.TerritoryAccountId == id);
                    if (account == null)
                    {
                        throw new ArgumentException("TerritoryAccount ID {0} не найден".F(id));
                    }
                   
                    if (account.Version != null && Constants.ZterritoryVersion.Contains((int) account.Version))
                    {
                        result.Data = ZslExistingAssistanceConditionsByAccountIdQuery(db, id).ToList(); 
                    }
                    else
                    {
                        result.Data = ExistingAssistanceConditionsByAccountIdQuery(db, id).ToList();
                    }
                   
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactEconomicPayment>> GetZExpectedPaymentsByAccountId(int id)
        {
            var result = new TransactionResult<IEnumerable<FactEconomicPayment>>();
            try
            {
                using (var db = CreateContext())
                {
                    result.Data = ZExistingAssistanceConditionsByAccountIdQuery(db, id).ToList();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<FactEconomicRefuse> GetEconomicRefuseById(int id)
        {
            var result = new TransactionResult<FactEconomicRefuse>();
            try
            {
                using (var db = CreateContext())
                {
                    result.Data = EconomicRefuseByIdQuery(db, id).FirstOrDefault();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactEconomicRefuseDetail>> GetRefusesDetailById(int id)
        {
            var result = new TransactionResult<IEnumerable<FactEconomicRefuseDetail>>();
            try
            {
                using (var db = CreateContext())
                {
                    result.Data = RefusesDetailByIdQuery(db, id).ToList();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactEconomicRefuseDetail>> GetExpectedRefuseByAccountId(int id)
        {
            var result = new TransactionResult<IEnumerable<FactEconomicRefuseDetail>>();
            try
            {
                using (var db = CreateContext())
                {
                    FactTerritoryAccount account =
                       db.GetTableQuery<FactTerritoryAccount>().FirstOrDefault(p => p.TerritoryAccountId == id);
                    if (account == null)
                    {
                        throw new ArgumentException("TerritoryAccount ID {0} не найден".F(id));
                    }

                    if (account.Version != null && Constants.ZterritoryVersion.Contains((int) account.Version))
                    {
                        result.Data = ZExistingAssistanceConditionsRefuseByAccountIdQuery(db, id).ToList();
                    }
                    else
                    {
                        result.Data = ExistingAssistanceConditionsRefuseByAccountIdQuery(db, id).ToList();
                    }
                   
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactEconomicRefuseDetail>> GetZExpectedRefuseByAccountId(int id)
        {
            var result = new TransactionResult<IEnumerable<FactEconomicRefuseDetail>>();
            try
            {
                using (var db = CreateContext())
                {
                    result.Data = ZExistingAssistanceConditionsRefuseByAccountIdQuery(db, id).ToList();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<FactEconomicSurcharge> GetEconomicSurchargeById(int id)
        {
            var result = new TransactionResult<FactEconomicSurcharge>();
            try
            {
                using (var db = CreateContext())
                {
                    result.Data = EconomicSurchargeByIdQuery(db, id).FirstOrDefault();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactEconomicSurchargeDetail>> GetSurchargeDetailById(int id)
        {
            var result = new TransactionResult<IEnumerable<FactEconomicSurchargeDetail>>();
            try
            {
                using (var db = CreateContext())
                {
                    result.Data = SurchargeDetailByIdQuery(db, id).ToList();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<bool> IsEconomicAccountByAccountIdExists(int? id)
        {
            var result = new TransactionResult<bool>();
            if (id.HasValue)
            {
                using (var db = CreateContext())
                {
                    try
                    {
                        var data = EconomicAccountByAccountIdQuery(db, id.Value).ToList();
                        result.Data = data.Count > 0;
                    }
                    catch (Exception exception)
                    {
                        result.AddError(exception);
                    }
                }
            }
            
            return result;
        }

        public TransactionResult<IEnumerable<Tuple<FactEconomicAccount, FactTerritoryAccount, Dictionary<int, Tuple<int, decimal>>>>> GetEconomicAccount1(Expression<Func<FactEconomicAccount, bool>> predicate = null)
        {
            var result = new TransactionResult<IEnumerable<Tuple<FactEconomicAccount, FactTerritoryAccount, Dictionary<int, Tuple<int, decimal>>>>>();
            using (var db = CreateContext())
            {
                try
                {
                    BLToolkit.Common.Configuration.Linq.AllowMultipleQuery = true;
                    if (predicate != null)
                    {
                        var q =
                            new List
                                <Tuple<FactEconomicAccount, FactTerritoryAccount, Dictionary<int, Tuple<int, decimal>>>>
                                (db
                                    .GetTableQuery<FactEconomicAccount>()
                                    .AsQueryable()
                                    .Where(predicate)
                                    .OrderBy(p => p.PaymentDate)
                                    .Select
                                    (
                                        p => Tuple.Create(
                                            p,
                                            p.FACTTERRACCACCID,
                                            p.FACTECONPAYFACTECONACCIDs
                                                .GroupBy(r => r.AssistanceConditionsId)
                                                .Select(g => Tuple.Create(g.Key, g.Sum(r => r.Amount)))
                                                .ToDictionary(h => h.Item1)))
                                );
                        result.Data = q;
                    }
                    else
                    {
                        var q = new List<Tuple<FactEconomicAccount, FactTerritoryAccount, Dictionary<int, Tuple<int, decimal>>>>(db
                            .GetTableQuery<FactEconomicAccount>()
                            .AsQueryable()
                            .OrderBy(p => p.PaymentDate)
                            .Select
                            (
                                p => Tuple.Create(
                                    p,
                                    p.FACTTERRACCACCID,
                                    p.FACTECONPAYFACTECONACCIDs
                                    .GroupBy(r => r.AssistanceConditionsId)
                                    .Select(g => Tuple.Create(g.Key, g.Sum(r => r.Amount)))
                                    .ToDictionary(h => h.Item1)))
                            );
                        result.Data  = q;
                    }

                    BLToolkit.Common.Configuration.Linq.AllowMultipleQuery = false;
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<Tuple<FactEconomicAccount, Dictionary<int, Tuple<int, decimal>>>>> GetEconomicAccount(Expression<Func<FactEconomicAccount, bool>> predicate = null)
        {
            var result = new TransactionResult<IEnumerable<Tuple<FactEconomicAccount, Dictionary<int, Tuple<int, decimal>>>>>();
            using (var db = CreateContext())
            {
                try
                {
                    BLToolkit.Common.Configuration.Linq.AllowMultipleQuery = true;

                    result.Data = predicate != null
                        ? new List<Tuple<FactEconomicAccount, Dictionary<int, Tuple<int, decimal>>>>(db
                            .GetTableQuery<FactEconomicAccount>()
                            .AsQueryable()
                            .Where(predicate)
                            .OrderBy(p => p.PaymentDate)
                            .Select
                            (
                                p => Tuple.Create(p, p.FACTECONPAYFACTECONACCIDs
                                    .GroupBy(r => r.AssistanceConditionsId)
                                    .Select(g => Tuple.Create(g.Key, g.Sum(r => r.Amount)))
                                    .ToDictionary(h => h.Item1)))
                            )
                        : new List<Tuple<FactEconomicAccount, Dictionary<int, Tuple<int, decimal>>>>(db
                            .GetTableQuery<FactEconomicAccount>()
                            .AsQueryable()
                            .OrderBy(p => p.PaymentDate)
                            .Select
                            (
                                p => Tuple.Create(p, p.FACTECONPAYFACTECONACCIDs
                                    .GroupBy(r => r.AssistanceConditionsId)
                                    .Select(g => Tuple.Create(g.Key, g.Sum(r => r.Amount)))
                                    .ToDictionary(h => h.Item1)))
                            );

                    BLToolkit.Common.Configuration.Linq.AllowMultipleQuery = false;
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<Tuple<FactEconomicRefuse, Dictionary<int, Tuple<int, decimal>>>>> GetEconomicRefuse(Expression<Func<FactEconomicRefuse, bool>> predicate = null)
        {
            var result = new TransactionResult<IEnumerable<Tuple<FactEconomicRefuse, Dictionary<int, Tuple<int, decimal>>>>>();
            using (var db = CreateContext())
            {
                try
                {
                    BLToolkit.Common.Configuration.Linq.AllowMultipleQuery = true;

                    result.Data = predicate != null
                        ? new List<Tuple<FactEconomicRefuse, Dictionary<int, Tuple<int, decimal>>>>(db
                            .GetTableQuery<FactEconomicRefuse>()
                            .AsQueryable()
                            .Where(predicate)
                            .OrderBy(p => p.RefuseDate)
                            .Select
                            (
                                p => Tuple.Create(p, p.FACTECONPAYFACTECONREFDs
                                    .GroupBy(r => r.AssistanceConditionsId)
                                    .Select(g => Tuple.Create(g.Key, g.Sum(r => r.AmountRefuse)))
                                    .ToDictionary(h => h.Item1)))
                            )
                        : new List<Tuple<FactEconomicRefuse, Dictionary<int, Tuple<int, decimal>>>>(db
                            .GetTableQuery<FactEconomicRefuse>()
                            .AsQueryable()
                            .OrderBy(p => p.RefuseDate)
                            .Select
                            (
                                p => Tuple.Create(p, p.FACTECONPAYFACTECONREFDs
                                    .GroupBy(r => r.AssistanceConditionsId)
                                    .Select(g => Tuple.Create(g.Key, g.Sum(r => r.AmountRefuse)))
                                    .ToDictionary(h => h.Item1)))
                            );

                    BLToolkit.Common.Configuration.Linq.AllowMultipleQuery = false;
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<Tuple<FactEconomicSurcharge, Dictionary<int, Tuple<int, decimal>>>>> GetEconomicSurcharge(Expression<Func<FactEconomicSurcharge, bool>> predicate = null)
        {
            var result = new TransactionResult<IEnumerable<Tuple<FactEconomicSurcharge, Dictionary<int, Tuple<int, decimal>>>>>();
            using (var db = CreateContext())
            {
                try
                {
                    BLToolkit.Common.Configuration.Linq.AllowMultipleQuery = true;

                    result.Data = predicate != null
                        ? new List<Tuple<FactEconomicSurcharge, Dictionary<int, Tuple<int, decimal>>>>(db
                            .GetTableQuery<FactEconomicSurcharge>()
                            .AsQueryable()
                            .Where(predicate)
                            .OrderBy(p => p.SurchargeDate)
                            .Select
                            (
                                p => Tuple.Create(p, p.FACTECONPAYFACTECONSURDs
                                    .GroupBy(r => r.AssistanceConditionsId)
                                    .Select(g => Tuple.Create(g.Key, g.Sum(r => r.AmountSurcharge)))
                                    .ToDictionary(h => h.Item1)))
                            )
                        : new List<Tuple<FactEconomicSurcharge, Dictionary<int, Tuple<int, decimal>>>>(db
                            .GetTableQuery<FactEconomicSurcharge>()
                            .AsQueryable()
                            .OrderBy(p => p.SurchargeDate)
                            .Select
                            (
                                p => Tuple.Create(p, p.FACTECONPAYFACTECONSURDs
                                    .GroupBy(r => r.AssistanceConditionsId)
                                    .Select(g => Tuple.Create(g.Key, g.Sum(r => r.AmountSurcharge)))
                                    .ToDictionary(h => h.Item1)))
                            );

                    BLToolkit.Common.Configuration.Linq.AllowMultipleQuery = false;
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }


        public TransactionResult<int> ChangeEconomicAccountStatus(int economicAccountId, int? status)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int>();
                try
                {
                    result.Data = db.FactEconomicAccount
                            .Where(p => p.EconomicAccountId == economicAccountId)
                            .Set(s => s.PaymentStatus, status)
                            .Update();

                    var totalAmountResult = UpdateTerritoryAccountTotalAmount(economicAccountId);
                    if (!totalAmountResult.Success)
                    {
                        result.AddError(totalAmountResult.LastError);
                        return result;
                    }
                    var paymentDetailsResult = UpdatePaymentDetail(economicAccountId);
                    if (!paymentDetailsResult.Success)
                    {
                        result.AddError(paymentDetailsResult.LastError);
                        return result;
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<int> ChangeActExperrtiseStatus(int actExpertiseId, int? status)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int>();
                try
                {
                    result.Data = db.FactActExpertise
                        .Where(p => p.ActExpertiseId == actExpertiseId)
                        .Set(s => s.ActExspertiStatusId, status)
                        .Update();

                    return result;
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<int> UpdateTerritoryAccountTotalAmount(int id)
        {
            var result = new TransactionResult<int>();
            try
            {
                using (var db = CreateContext())
                {
                    //Считаем фактически оплаченную сумму для счета
                    decimal? totalAmount = db.GetTableQuery<FactEconomicAccount>()
                        .Where(p => p.AccountId == id && (p.PaymentStatus == 2 || p.PaymentStatus == 3))
                        .Sum(p => p.TotalAmount);

                    //Обновляем данные счета
                    result.Data = db.GetTableQuery<FactTerritoryAccount>()
                        .Where(p => p.TerritoryAccountId == id)
                        .Set(s => s.TotalPaymentAmount, totalAmount)
                        .Update();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public TransactionResult<int> ChangeEconomicAccountStatus(int economicAccountId, int accountId,  int? status)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int>();
                try
                {
                    result.Data = db.FactEconomicAccount
                            .Where(p => p.EconomicAccountId == economicAccountId)
                            .Set(s => s.PaymentStatus, status)
                            .Update();

                    var totalAmountResult = UpdateTerritoryAccountTotalAmount(accountId);
                    if (!totalAmountResult.Success)
                    {
                        result.AddError(totalAmountResult.LastError);
                        return result;
                    }
                    var paymentDetailsResult = UpdatePaymentDetail(economicAccountId);
                    if (!paymentDetailsResult.Success)
                    {
                        result.AddError(paymentDetailsResult.LastError);
                        return result;
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<int> UpdateTerritoryAccountTotalAmount(int economicAccountId, int accountId)
        {
            var result = new TransactionResult<int>();
            try
            {
                using (var db = CreateContext())
                {
                    //Считаем фактически оплаченную сумму для счета
                    decimal? totalAmount = db.GetTableQuery<FactEconomicAccount>()
                        .Where(p => p.AccountId == accountId && p.PaymentStatus == 2)
                        .Sum(p => p.TotalAmount);

                    //Обновляем данные счета
                    result.Data = db.GetTableQuery<FactTerritoryAccount>()
                        .Where(p => p.TerritoryAccountId == accountId)
                        .Set(s => s.TotalPaymentAmount, totalAmount)
                        .Update();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public class PaymentByAssistanceDetails
        {
            public int? Assistance;
            public decimal? AcceptPrice;
            public decimal? Price;
            public decimal? Mec;
            public decimal? ExternalRefuse;
        }

        public TransactionResult<int> UpdatePaymentDetail(int id)
        {
            var result = new TransactionResult<int>();
            try
            {
                using (var db = CreateContext())
                {

                    FactTerritoryAccount account =
                        db.GetTableQuery<FactTerritoryAccount>().FirstOrDefault(p => p.TerritoryAccountId == id);
                    if (account == null)
                    {
                        throw new ArgumentException("TerritoryAccount ID {0} не найден".F(id));
                    }
                    //удаляем все детальные платежи привязанные к счету
                    db.GetTableQuery<FactEconomicPaymentDetail>().Delete(p => p.AccountId == id);
                    Dictionary<int?, PaymentByAssistanceDetails> paymentDetails =
                        new Dictionary<int?, PaymentByAssistanceDetails>();
                    if (account.Version != null && Constants.ZterritoryVersion.Contains((int) account.Version))
                    {
                        //получаем сгрупированные по условиям оказания помощи суммы Price и AcceptPrice
                        paymentDetails = db.GetTableQuery<ZslFactMedicalEvent>
                            ()
                            .Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id)
                            .GroupBy(p => p.AssistanceConditions)
                            .Select(g => new PaymentByAssistanceDetails
                            {
                                Assistance = g.Key,
                                AcceptPrice = g.Sum(p => p.AcceptPrice),
                                Price = g.Sum(p => p.Price)
                            }).ToDictionary(h => h.Assistance);

                    }
                    else
                    {
                        //получаем сгрупированные по условиям оказания помощи суммы Price и AcceptPrice
                        paymentDetails = db.GetTableQuery<FactMedicalEvent>
                            ()
                            .Where(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == id)
                            .GroupBy(p => p.AssistanceConditions)
                            .Select(g => new PaymentByAssistanceDetails
                            {
                                Assistance = g.Key,
                                AcceptPrice = g.Sum(p => p.AcceptPrice),
                                Price = g.Sum(p => p.Price)
                            }).ToDictionary(h => h.Assistance);
                    }
                    foreach (var paymentDetail in paymentDetails)
                    {
                        //Проверка на Условия оказания МП
                        if (!paymentDetail.Value.Assistance.HasValue)
                        {
                            result.AddError(
                                new Exception("Ошибка добавления деталей платежа. AssistanceCondition == null"));
                            return result;
                        }

                        int assistance = paymentDetail.Value.Assistance.Value;

                        decimal payable = paymentDetail.Value.AcceptPrice ?? 0;
                        decimal fact = db.GetTableQuery<FactEconomicPayment>()
                            .Where(
                                p =>
                                    p.AssistanceConditionsId == assistance &&
                                    p.FACTECONPAYFACTECONACCID.AccountId == id &&
                                    p.FACTECONPAYFACTECONACCID.PaymentStatus == 2)
                            .Sum(p => p.Amount);
                        decimal debt = payable - fact;


                        var resultPaymentDetails = db.InsertWithIdentity(new FactEconomicPaymentDetail
                        {
                            AccountId = id,
                            AssistanceConditionsId = assistance,
                            AmountPayable = payable,
                            AmountFact = fact,
                            AmountDebt = debt,
                        });
                        if (!Convert.ToBoolean(resultPaymentDetails))
                        {
                            result.AddError(new Exception("Ошибка добавления деталей платежа", db.LastError));
                            return result;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult<int> DeleteEconomicAccount(int economicAccountId, int accountId)
        {
            var result = new TransactionResult<int>();
            try
            {
                using (var db = CreateContext())
                {
                    //Удаляем суммы оплаты
                    var deleteEconomicPaymentResult = DeleteEconomicPayment(economicAccountId);
                    if (!deleteEconomicPaymentResult.Success)
                    {
                        result.AddError(deleteEconomicPaymentResult.LastError);
                        return result;
                    }

                    //Удаляем информацию об оплате
                    db.GetTableQuery<FactEconomicAccount>().Delete(
                        p => p.EconomicAccountId == economicAccountId);

                    //Обновляем данные счета о фактической оплате
                    var totalAmountResult = UpdateTerritoryAccountTotalAmount(economicAccountId);
                    if (!totalAmountResult.Success)
                    {
                        result.AddError(totalAmountResult.LastError);
                        return result;
                    }

                    //Обновляем детальные данные платежа
                    var paymentDetailsResult = UpdatePaymentDetail(economicAccountId);
                    if (!paymentDetailsResult.Success)
                    {
                        result.AddError(paymentDetailsResult.LastError);
                        return result;
                    }
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult<int> CreateOrUpdateEconomicAccount(FactEconomicAccount data)
        {
            var result = new TransactionResult<int>();
            try
            {
                using (var db = CreateContext())
                {
                    if (data.EconomicAccountId == 0)
                    {
                        //новая запись об оплате
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            result.AddError(new Exception("Ошибка создания информации об оплате", db.LastError));
                            return result;
                        }
                        result.Id = Convert.ToInt32(insertResult);
                    }
                    else
                    {
                        //обновление записи об оплате
                        int updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            result.AddError(new Exception("Ошибка обновления информации об оплате\n{0}", db.LastError));
                            return result;
                        }
                        result.Id = data.EconomicAccountId;
                    }


                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult CreateOrUpdateEconomicRefuse(FactEconomicRefuse data)
        {
            var result = new TransactionResult();
            try
            {
                using (var db = CreateContext())
                {
                    if (data.EconomicRefuseId == 0)
                    {
                        //новая запись об отказах
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            result.AddError(new Exception("Ошибка создания информации об отказaх\n{0}".F(db.LastQuery)));
                            return result;
                        }
                        result.Id = Convert.ToInt32(insertResult);
                    }
                    else
                    {
                        //обновление записи об отказах
                        int updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            result.AddError(new Exception("Ошибка обновления информации об отказах\n{0}".F(db.LastQuery)));
                            return result;
                        }
                        result.Id = data.EconomicRefuseId;
                    }
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult DeleteEconomicSurcharge(int economicSurchargeId)
        {
            var result = new TransactionResult();
            try
            {
                using (var db = CreateContext())
                {
                    db.BeginTransaction();
                    var deleteDetailsResult = db.FactEconomicSurchargeDetail.Delete(
                        p => p.EconomicSurchargeId == economicSurchargeId);
                    if (!Convert.ToBoolean(deleteDetailsResult))
                    {
                        result.AddError(new Exception("Last query return 0\r\n{0}".F(db.LastQuery)));
                        db.RollbackTransaction();
                        return result;
                    }

                    var deleteResult = db.FactEconomicSurcharge.Delete(
                        p => p.EconomicSurchargeId == economicSurchargeId);
                    if (!Convert.ToBoolean(deleteResult))
                    {
                        result.AddError(new Exception("Last query return 0\r\n{0}".F(db.LastQuery)));
                        db.RollbackTransaction();
                        return result;
                    }
                    db.CommitTransaction();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult<int> UpdateEconomicPayment(int accountId, int economicAccountId, IEnumerable<Tuple<decimal,int>> paymentList)
        {
            var result = new TransactionResult<int>();
            try
            {
                using (var db = CreateContext())
                {
                    //удаляем все платежи привязанные к информации об оплате
                    db.FactEconomicPayment.Delete(p => p.EconomicAccountId == economicAccountId);
                    //добавляем информацию о платежах
                    foreach (var payment in paymentList)
                    {
                        var resultPayment = db.InsertWithIdentity(new FactEconomicPayment
                        {
                            EconomicAccountId = economicAccountId,
                            Amount = payment.Item1,
                            AssistanceConditionsId = payment.Item2
                        });
                        if (!Convert.ToBoolean(resultPayment))
                        {
                            result.AddError(new Exception("Ошибка добавления платежа к информации об оплате", db.LastError));
                            return result;
                        }
                    }

                    //Обновляем данные счета о фактической оплате
                    var totalAmountResult = UpdateTerritoryAccountTotalAmount(accountId);
                    if (!totalAmountResult.Success)
                    {
                        result.AddError(totalAmountResult.LastError);
                        return result;
                    }

                    //Обновляем детальные данные платежа
                    var paymentDetailsResult = UpdatePaymentDetail(accountId);
                    if (!paymentDetailsResult.Success)
                    {
                        result.AddError(paymentDetailsResult.LastError);
                        return result;
                    }
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult UpdateEconomicRefuse(int accountId, int economicRefuseId, IEnumerable<Tuple<decimal, int>> refusalList)
        {
            var result = new TransactionResult<int>();
            try
            {
                using (var db = CreateContext())
                {
                    //удаляем все отказы привязанные к информации об отказах
                    db.FactEconomicRefuseDetail.Delete(p => p.EconomicRefuseId == economicRefuseId);
                    //добавляем информацию об отказах
                    foreach (var payment in refusalList)
                    {
                        var resultPayment = db.InsertWithIdentity(new FactEconomicRefuseDetail
                        {
                            EconomicRefuseId = economicRefuseId,
                            AmountRefuse = payment.Item1,
                            AssistanceConditionsId = payment.Item2
                        });
                        if (!Convert.ToBoolean(resultPayment))
                        {
                            result.AddError(new Exception("Ошибка добавления отказа к информации об отказах", db.LastError));
                            return result;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public  TransactionResult UpdateEconomicSurcharge(int accountId, int economicSurchargeId, IEnumerable<Tuple<decimal, int>> surchargeList)
        {
            var result = new TransactionResult();
            try
            {
                using (var db = CreateContext())
                {
                    db.FactEconomicSurchargeDetail.Delete(p => p.EconomicSurchargeId == economicSurchargeId);
                    foreach (var payment in surchargeList)
                    {
                        var resultPayment = db.InsertWithIdentity(new FactEconomicSurchargeDetail
                        {
                            EconomicSurchargeId = economicSurchargeId,
                            AmountSurcharge = payment.Item1,
                            AssistanceConditionsId = payment.Item2
                        });
                        if (!Convert.ToBoolean(resultPayment))
                        {
                            result.AddError(new Exception("Ошибка добавления доплаты к информации о доплатах", db.LastError));
                            return result;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult<bool> IsEconomicRefuseByAccountIdExists(int? id)
        {
            var result = new TransactionResult<bool>();
            if (id.HasValue)
            {
                using (var db = CreateContext())
                {
                    try
                    {
                        var data = EconomicRefuseByAccountIdQuery(db, id.Value).ToList();
                        result.Data = data.Count > 0;
                    }
                    catch (Exception exception)
                    {
                        result.AddError(exception);
                    }
                }
            }

            return result;
        }

        public TransactionResult<IEnumerable<globalAccountStatus>> GetGlobalAccountStatus()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalAccountStatus>>();
                try
                {
                    result.Data = GlobalAccountStatusQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<Tuple<FactTerritoryAccount,FactEconomicAccount,decimal,decimal, Dictionary<int, Tuple<int, decimal, decimal, decimal, decimal>>>>> GetEconomicJournal(Expression<Func<FactEconomicAccount, bool>> predicate)
        {
            var result = new TransactionResult<IEnumerable<Tuple<FactTerritoryAccount,FactEconomicAccount,decimal,decimal, Dictionary<int, Tuple<int, decimal, decimal, decimal, decimal>>>>>();
            using (var db = CreateContext())
            {
                BLToolkit.Common.Configuration.Linq.AllowMultipleQuery = true;

                result.Data = predicate != null ?
                    new List
                        <Tuple< FactTerritoryAccount,FactEconomicAccount,decimal,decimal,
                                    Dictionary<int, Tuple<int,decimal, decimal, decimal, decimal>>>>(db
                                        .GetTableQuery<FactEconomicAccount>()
                                        .AsQueryable()
                                        .Where(predicate)
                                        .OrderBy(p => p.PaymentDate)
                                        .Select
                                        (
                                            p => Tuple.Create(
                                                p.FACTTERRACCACCID,
                                                p,
                                                p.FACTTERRACCACCID.FACTECONPAYDTERRACCs.Sum(o => o.AmountPayable),
                                                p.FACTTERRACCACCID.FACTECONREFTERRACCs.Sum(o => o.RefuseTotalAmount),
                                                p.FACTTERRACCACCID.FACTECONPAYDTERRACCs
                                                    .GroupBy(r => r.AssistanceConditionsId)
                                                    .Select(g => Tuple.Create(g.Key,
                                                        g.Sum(r => r.AmountPayable),
                                                        p.FACTECONPAYFACTECONACCIDs.Where(r => r.AssistanceConditionsId == g.Key).Sum(r => r.Amount),
                                                        p.FACTTERRACCACCID.FACTECONREFTERRACCs.Sum(r => r.FACTECONPAYFACTECONREFDs.Where(s => s.AssistanceConditionsId == g.Key).Sum(s => s.AmountRefuse)),
                                                       g.Sum(r => r.AmountDebt)))
                                                    .ToDictionary(h => h.Item1)))
                        ):
                        new List
                        <Tuple<FactTerritoryAccount, FactEconomicAccount, decimal, decimal,
                                    Dictionary<int, Tuple<int, decimal, decimal, decimal, decimal>>>>(db
                                        .GetTableQuery<FactEconomicAccount>()
                                        .AsQueryable()
                                        .OrderBy(p => p.PaymentDate)
                                        .Select
                                        (
                                            p => Tuple.Create(
                                                p.FACTTERRACCACCID,
                                                p,
                                                p.FACTTERRACCACCID.FACTECONPAYDTERRACCs.Sum(o => o.AmountPayable),
                                                p.FACTTERRACCACCID.FACTECONREFTERRACCs.Sum(o => o.RefuseTotalAmount),
                                                p.FACTTERRACCACCID.FACTECONPAYDTERRACCs
                                                    .GroupBy(r => r.AssistanceConditionsId)
                                                    .Select(g => Tuple.Create(g.Key,
                                                        g.Sum(r => r.AmountPayable),
                                                        p.FACTECONPAYFACTECONACCIDs.Where(r => r.AssistanceConditionsId == g.Key).Sum(r => r.Amount),
                                                        p.FACTTERRACCACCID.FACTECONREFTERRACCs.Sum(r => r.FACTECONPAYFACTECONREFDs.Where(s => s.AssistanceConditionsId == g.Key).Sum(s => s.AmountRefuse)),
                                                       g.Sum(r => r.AmountDebt)))
                                                    .ToDictionary(h => h.Item1)))
                        );

                BLToolkit.Common.Configuration.Linq.AllowMultipleQuery = false;
            }

            return result;
        }


        public TransactionResult CreateOrUpdateEconomicSurcharge(FactEconomicSurcharge data)
        {
            var result = new TransactionResult();
            try
            {
                using (var db = CreateContext())
                {
                    if (data.EconomicSurchargeId == 0)
                    {
                        //новая запись о доплате
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            result.AddError(new Exception("Ошибка создания информации о доплате\n{0}".F(db.LastQuery)));
                            return result;
                        }
                        result.Id = Convert.ToInt32(insertResult);
                    }
                    else
                    {
                        //обновление записи о доплате
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            result.AddError(new Exception("Ошибка обновления информации о доплате\n{0}".F(db.LastQuery)));
                            return result;
                        }

                        result.Id = data.EconomicSurchargeId;
                    }
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult<IEnumerable<localUser>> GetLocalUser(Expression<Func<localUser, bool>> predicate)
        {
            var result = new TransactionResult<IEnumerable<localUser>>();

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = predicate == null ?
                        LocalUserQuery(db).ToList() :
                        db.localUser.Where(predicate).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteUser(int userId)
        {
            var result = new TransactionResult();

            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    var deleteResult = db.localUser.Delete(p => p.UserID == userId);
                    if (!Convert.ToBoolean(deleteResult))
                    {
                        result.AddError(new Exception(db.LastQuery));
                        db.RollbackTransaction();
                    }
                    else
                    {
                        db.CommitTransaction();
                    }
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<int> GetRegisterDLastPacketNumber(string source, string dest)
        {
            var result = new TransactionResult<int>();
            using (var db = CreateContext())
            {
                try
                {
                    if (db.FactExchange.Any(p => p.Source == source && p.Destination == dest))
                    {
                        result.Data = db.GetTableQuery<FactExchange>()
                             .Where(p => p.Source == source && p.Destination == dest)
                             .Max(p => p.PacketNumber);

                        result.Data = Math.Max(1, result.Data + 1);
                    }
                    else
                    {
                        result.Data = 1;
                    }
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactProcessing>> GetProcessing(Expression<Func<FactProcessing, bool>> predicate)
        {
            var result = new TransactionResult<IEnumerable<FactProcessing>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = predicate.IsNull()
                        ? ProcessingQuery(db).ToList()
                        : db.FactProcessing.Where(predicate).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult Delete<T>(T data)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    var deleteResult = db.Delete(data);
                    if (!Convert.ToBoolean(deleteResult))
                    {
                        result.AddError(new Exception(db.LastQuery));
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<globalProcessingType>> GetGlobalProcessingType()
        {
            var result = new TransactionResult<IEnumerable<globalProcessingType>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = GlobalProcessingTypeQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactProcessing>> GetEnabledProcessingByScopeAndVersion(int scope, int version)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactProcessing>>();
                try
                {
                    result.Data = EnabledProcessingByScopeAndVersionQuery(db, scope, version).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<FactProcessing>> GetEnabledProcessingByIds(List<object> ids)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactProcessing>>();
                try
                {
                    result.Data = db.FactProcessing.Where(p => ids.Contains(p.ProcessingId)).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<shareDoctor>> GetShareDoctor(Expression<Func<shareDoctor, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<shareDoctor>>();
                try
                {
                    result.Data = predicate!= null ? 
                        db.shareDoctor.Where(predicate).Distinct(p => p.Code).ToList():
                        ShareDoctorQuery(db).Distinct(p => p.Code).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult DeleteTerritorialRefusal(int accountId, int refusalId, int refusalType, UpdateFlag flag)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    int eventId = 0;
                    switch (refusalType)
                    {
                        //МЭК
                        case 1:
                            var mec = db.FactMEC.FirstOrDefault(p => p.MECId == refusalId);
                            if (mec.IsNull())
                            {
                                db.RollbackTransaction();
                                result.AddError(new InvalidOperationException("mec doesn't exists"));
                                return result;
                            }
                            eventId = mec.MedicalEventId ?? 0;
                            var deleteMecResult = db.Delete(mec);
                            if (!Convert.ToBoolean(deleteMecResult))
                            {
                                db.RollbackTransaction();
                                result.AddError(new InvalidOperationException("mec delete return 0\n{0}".F(db.LastQuery)));
                                return result;
                            }
                            break;
                        //МЭЭ
                        case 2:
                            var mee = db.FactMEE.FirstOrDefault(p => p.MEEId == refusalId);
                            if (mee.IsNull())
                            {
                                db.RollbackTransaction();
                                result.AddError(new InvalidOperationException("mee doesn't exists"));
                                return result;
                            }
                            eventId = mee.MedicalEventId ?? 0;
                            var deleteMeeResult = db.Delete(mee);
                            if (!Convert.ToBoolean(deleteMeeResult))
                            {
                                db.RollbackTransaction();
                                result.AddError(new InvalidOperationException("mee delete return 0\n{0}".F(db.LastQuery)));
                                return result;
                            }
                            break;
                        //ЭКМП
                        case 3:
                            var eqma = db.FactEQMA.FirstOrDefault(p => p.EQMAId == refusalId);
                            if (eqma.IsNull())
                            {
                                db.RollbackTransaction();
                                result.AddError(new InvalidOperationException("eqma doesn't exists"));
                                return result;
                            }
                            eventId = eqma.MedicalEventId ?? 0;
                            var deleteEqmaResult = db.Delete(eqma);
                            if (!Convert.ToBoolean(deleteEqmaResult))
                            {
                                db.RollbackTransaction();
                                result.AddError(new InvalidOperationException("eqma delete return 0\n{0}".F(db.LastQuery)));
                                return result;
                            }
                            break;
                        default:
                            result.AddError(new ArgumentException("Wrong refusalType"));
                            break;
                    }

                    if (flag.HasFlag(UpdateFlag.MedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateMedicalEventAsTerritorial(db, eventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(eventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.Account))
                    {
                        var updateAccountResult = UpdateTerritoryAccount(db, accountId);
                        if (updateAccountResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Счета ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(accountId, db.LastQuery));
                        }
                    }
                    db.CommitTransaction();

                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteZTerritorialRefusal(int accountId, int zslMedicalEventId, int refusalId, int refusalType, UpdateFlag flag)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    int eventId = 0;
                    var mec = db.ZFactSank.FirstOrDefault(p => p.ZSankId == refusalId);
                    if (mec.IsNull())
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("sank doesn't exists"));
                        return result;
                    }
                    eventId = mec.ZmedicalEventId ?? 0;
                    var deleteMecResult = db.Delete(mec);
                    if (!Convert.ToBoolean(deleteMecResult))
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("sank delete return 0\n{0}".F(db.LastQuery)));
                        return result;
                    }

                    if (flag.HasFlag(UpdateFlag.MedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateZMedicalEventAsTerritorial(db, eventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException(
                                "Обновление Случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(eventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.ZslMedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateZslMedicalEventAsTerritorial(db, zslMedicalEventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление законченного случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(zslMedicalEventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.Account))
                    {
                        var updateAccountResult = UpdateZTerritoryAccount(db, accountId);
                        if (updateAccountResult.HasError)
                        {
                            throw new InvalidOperationException(
                                "Обновление Счета ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(accountId, db.LastQuery));
                        }
                    }
                    db.CommitTransaction();


                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }


        public TransactionResult InsertOrUpdateTerritorialMec(FactMEC data, UpdateFlag flag, int? accountId = null)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.MECId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException("Добавление МЭК произошло с ошибкой.\r\nЗапрос:{0}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Обновление МЭК ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.MECId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.MedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateMedicalEventAsTerritorial(db, data.MedicalEventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.MedicalEventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.Account))
                    {
                        var updateAccountResult = UpdateTerritoryAccount(db, accountId);
                        if (updateAccountResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Счета ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(accountId, db.LastQuery));
                        }
                    }


                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult InsertOrUpdateTerritorialSank(ZFactSank data, UpdateFlag flag, int zslMedicalEventId, int? accountId = null)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.ZSankId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException("Добавление МЭК произошло с ошибкой.\r\nЗапрос:{0}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Обновление МЭК ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.ZSankId, db.LastQuery));
                        }
                    }
                  
                    if (flag.HasFlag(UpdateFlag.MedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateZMedicalEventAsTerritorial(db, data.ZmedicalEventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.ZmedicalEventId, db.LastQuery));
                        }
                    }
                    if (flag.HasFlag(UpdateFlag.ZslMedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateZslMedicalEventAsTerritorial(db, zslMedicalEventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление законченного случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(zslMedicalEventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.Account))
                    {
                        var updateAccountResult = UpdateZTerritoryAccount(db, accountId);
                        if (updateAccountResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Счета ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(accountId, db.LastQuery));
                        }
                    }


                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult InsertOrUpdateTerritorialMee(FactMEE data, UpdateFlag flag, int? accountId = null)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.MEEId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException("Добавление МЭЭ произошло с ошибкой.\r\nЗапрос:{1}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Обновление МЭЭ ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.MEEId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.MedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateMedicalEventAsTerritorial(db, data.MedicalEventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.MedicalEventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.Account))
                    {
                        var updateAccountResult = UpdateTerritoryAccount(db, accountId);
                        if (updateAccountResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Счета ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(accountId, db.LastQuery));
                        }
                    }


                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult InsertOrUpdateTerritorialEqma(FactEQMA data, UpdateFlag flag, int? accountId = null)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.EQMAId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException("Добавление ЭКМП произошло с ошибкой.\r\nЗапрос:{1}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Обновление ЭКМП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.EQMAId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.MedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateMedicalEventAsTerritorial(db, data.MedicalEventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.MedicalEventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.Account))
                    {
                        var updateAccountResult = UpdateTerritoryAccount(db, accountId);
                        if (updateAccountResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Счета ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(accountId, db.LastQuery));
                        }
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult UpdateMedicalEventAsTerritorial(MedicineContext db, int? medicalEventId)
        {
            var result = new TransactionResult();

            try
            {
                FactMedicalEvent mevent = db.GetTableQuery<FactMedicalEvent>().FirstOrDefault(p => p.MedicalEventId == medicalEventId);
                if (mevent == null)
                {
                    throw new ArgumentException("MedicalEvent ID {0} не найден".F(medicalEventId));
                }

                //Получаем все МЭКи не блокированные и с источником 'ТФОМС2 к ТФОМС1' или 'Итоговые санкции ТФОМС2 к ТФОМС1'
                var mecList = db.GetTableQuery<FactMEC>()
                    .Distinct()
                    .Where(p => p.MedicalEventId == medicalEventId &&
                        (p.IsLock == null || p.IsLock == false) &&
                        (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal)).ToList();

                decimal mec = mecList.Distinct(p => p.MedicalEventId).Sum(p => p.Amount) ?? 0;

                decimal mee = db.GetTableQuery<FactMEE>()
                    .Where(p => p.MedicalEventId == medicalEventId &&
                        (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal))
                    .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;

                decimal eqma = db.GetTableQuery<FactEQMA>()
                    .Where(p => p.MedicalEventId == medicalEventId &&
                        (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal))
                    .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;

                var external = db.GetTableQuery<FactExternalRefuse>()
                    .Where(p => p.MedicalEventId == medicalEventId && 
                        (p.IsAgree == true))
                    .Sum(p => p.Amount) ?? 0;

                //считаем принятую к оплате сумму и статус оплаты
                decimal price = mevent.Price ?? 0;
                int paymentStatus = 2;
                decimal? acceptPrice = mevent.Price - (mec + mee + eqma + external);
                if (acceptPrice == 0)
                {
                    paymentStatus = 3;
                }
                else if (acceptPrice < mevent.Price)
                {
                    paymentStatus = 4;
                }

                //обновляем запись случая
                db.GetTableQuery<FactMedicalEvent>().Where(s => s.MedicalEventId == medicalEventId)
                   .Set(s => s.MEC, Math.Min((mec + external), price))
                   .Set(s => s.MEE, Math.Min((mee), price))
                   .Set(s => s.EQMA, Math.Min((eqma), price))
                   .Set(s => s.PaymentStatus, paymentStatus)
                   .Set(s => s.AcceptPrice, acceptPrice)
                   .Update();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult UpdateZMedicalEventAsTerritorial(MedicineContext db, int? zMedicalEventId)
        {
            var result = new TransactionResult();

            try
            {
                ZFactMedicalEvent mevent = db.GetTableQuery<ZFactMedicalEvent>().FirstOrDefault(p => p.ZmedicalEventId == zMedicalEventId);
                if (mevent == null)
                {
                    throw new ArgumentException("ZMedicalEvent ID {0} не найден".F(zMedicalEventId));
                }
                ZslFactMedicalEvent zslFactMedicalEvent = db.GetTableQuery<ZslFactMedicalEvent>().FirstOrDefault(p=>p.ZslMedicalEventId == mevent.ZslMedicalEventId);

                //Получаем все санкции не блокированные и с источником 'ТФОМС2 к ТФОМС1' или 'Итоговые санкции ТФОМС2 к ТФОМС1'
                //(p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal)
                var allSankList = db.GetTableQuery<ZFactSank>()
                    .Distinct()
                    .Where(p => p.ZmedicalEventId == zMedicalEventId &&
                        (p.IsLock == null || p.IsLock == false)).ToList();

                var mecList = allSankList.Where(x => x.Type == 1);
                decimal mec = mecList.Distinct(p => p.ZmedicalEventId).Sum(p => p.Amount) ?? 0;

                decimal mee = allSankList.Where(p => Constants.Mee.Contains(p.Type)).Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;

                decimal eqma = allSankList.Where(p => Constants.Eqma.Contains(p.Type)).Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;

                var external = db.GetTableQuery<ZFactExternalRefuse>()
                    .Where(p => p.ZmedicalEventId == zMedicalEventId &&
                        (p.IsAgree == true))
                    .Sum(p => p.Amount) ?? 0;

                //считаем принятую к оплате сумму и статус оплаты
                decimal price = mevent.EventPrice ?? 0;
                decimal? acceptPrice = mevent.EventPrice - (mec + mee + eqma + external);

                //обновляем запись случая
                db.GetTableQuery<ZFactMedicalEvent>().Where(s => s.ZmedicalEventId == zMedicalEventId)
                   .Set(s => s.MEC, Math.Min((mec + external), price))
                   .Set(s => s.MEE, Math.Min((mee), price))
                   .Set(s => s.EQMA, Math.Min((eqma), price))
                   .Set(s => s.AcceptPrice, acceptPrice)
                   .Update();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult UpdateZslMedicalEventAsTerritorial(MedicineContext db, int? zslMedicalEventId)
        {
            var result = new TransactionResult();
            
            try
            {
                var zslMevent = db.GetTableQuery<ZslFactMedicalEvent>().FirstOrDefault(p => p.ZslMedicalEventId == zslMedicalEventId);
                if (zslMevent == null)
                {
                    throw new ArgumentException("ZslMedicalEvent ID {0} не найден".F(zslMedicalEventId));
                }

                var mevent = db.GetTableQuery<ZFactMedicalEvent>().Where(p => p.ZslFactMedicalEvent.ZslMedicalEventId == zslMedicalEventId).ToList();

                if (!mevent.Any())
                {
                    throw new ArgumentException("Не найдено ни одного посещения в обращении ID {0}".F(zslMedicalEventId));
                }

                decimal? acceptPrice = null;
                decimal? refusalPrice = null;
                bool mecFlag = false;
                bool mee_eqmp = false;
                foreach (var zFactMedicalEvent in mevent)
                {
                    var id = zFactMedicalEvent.ZmedicalEventId;
                    //Получаем все санкции не блокированные и с источником 'ТФОМС2 к ТФОМС1' или 'Итоговые санкции ТФОМС2 к ТФОМС1'
                    //(p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal)
                    var allSankList = db.GetTableQuery<ZFactSank>()
                        .Distinct()
                        .Where(p => p.ZmedicalEventId == id &&
                            (p.IsLock == null || p.IsLock == false) ).ToList();

                    if (allSankList.Where(x => x.Type == 1).Any())
                    {
                        mecFlag = true;
                    }
                    if (allSankList.Any(x => Constants.Mee.Contains(x.Type))|| allSankList.Any(x => Constants.Eqma.Contains(x.Type)))
                    {
                        mee_eqmp = true;
                        decimal mee = allSankList.Where(p => Constants.Mee.Contains(p.Type)).Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;
                        decimal eqma = allSankList.Where(p => Constants.Eqma.Contains(p.Type)).Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;

                        refusalPrice = refusalPrice ?? 0 + mee + eqma;
                    }

                    var external = db.GetTableQuery<ZFactExternalRefuse>()
                        .Distinct()
                        .Where(p => p.ZmedicalEventId == id &&
                                    (p.IsAgree == true));
                    if (external.Where(x => x.Type == 1).Any())
                    {
                        mecFlag = true;
                    }
                    if (external.Any(x => Constants.Mee.Contains(x.Type)) || external.Any(x => Constants.Eqma.Contains(x.Type)))
                    {
                        mee_eqmp = true;
                        decimal mee = external.Where(p => Constants.Mee.Contains(p.Type)).Sum(p => p.Amount) ?? 0;
                        decimal eqma = external.Where(p => Constants.Eqma.Contains(p.Type)).Sum(p => p.Amount) ?? 0;

                        refusalPrice = refusalPrice ?? 0 + mee + eqma;
                    }

                    acceptPrice = (acceptPrice == null ? 0: acceptPrice) + (zFactMedicalEvent?.AcceptPrice??0);
                    
                }
                
                if (mecFlag)
                {
                    acceptPrice = 0;
                    refusalPrice = zslMevent.Price;
                }
                else
                {
                    if (!mee_eqmp)
                    {
                        acceptPrice = zslMevent.Price;
                        refusalPrice = 0;
                    }
                }

                int moPaymentStatus = 2;
                int paymentStatus = 2;
                if (acceptPrice == 0)
                {
                    moPaymentStatus = 3;
                    paymentStatus = 3;
                }
                else if (acceptPrice < mevent.Sum(x=>x.EventPrice))
                {
                    moPaymentStatus = 4;
                    paymentStatus = 4;
                }

                //обновляем запись случая
                db.GetTableQuery<ZslFactMedicalEvent>().Where(s => s.ZslMedicalEventId == zslMevent.ZslMedicalEventId)
                   .Set(s => s.MoPaymentStatus, moPaymentStatus)
                   .Set(s => s.PaymentStatus, paymentStatus)
                   .Set(s => s.AcceptPrice, acceptPrice)
                   .Set(s => s.RefusalPrice, refusalPrice)
                   .Update();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult BlockLocalMec(FactMEC data, UpdateFlag flag, int? accountId = null)
        {
            if (data.IsLock.HasValue)
            {
                data.IsLock = !data.IsLock;
            }
            else
            {
                data.IsLock = true;
            }
            return InsertOrUpdateLocalMec(data, flag, accountId);
        }

        public TransactionResult BlockLocalSank(ZFactSank data, UpdateFlag flag, int zslMedicalEventId, int? accountId = null)
        {
            if (data.IsLock.HasValue)
            {
                data.IsLock = !data.IsLock;
            }
            else
            {
                data.IsLock = true;
            }
            return InsertOrUpdateLocalSank(data, flag, zslMedicalEventId, accountId);
        }

        public TransactionResult BlockTerritorialMec(FactMEC data, UpdateFlag flag, int? accountId = null)
        {
            if (data.IsLock.HasValue)
            {
                data.IsLock = !data.IsLock;
            }
            else
            {
                data.IsLock = true;
            }
            return InsertOrUpdateTerritorialMec(data, flag, accountId);
        }

        public TransactionResult BlockTerritorialSank(ZFactSank data, UpdateFlag flag, int zslMedicalEventId, int? accountId = null)
        {
            if (data.IsLock.HasValue)
            {
                data.IsLock = !data.IsLock;
            }
            else
            {
                data.IsLock = true;
            }
            return InsertOrUpdateTerritorialSank(data, flag, zslMedicalEventId, accountId);
        }

        public TransactionResult<FactExternalRefuse> GetExternalRefuseById(int id)
        {
            var result = new TransactionResult<FactExternalRefuse>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactExternalRefuse.FirstOrDefault(p=>p.ExternalRefuseId == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                
            }
            return result;
        }

        public TransactionResult<ZFactExternalRefuse> GetZExternalRefuseById(int id)
        {
            var result = new TransactionResult<ZFactExternalRefuse>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZFactExternalRefuse.FirstOrDefault(p => p.ExternalRefuseId == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }

            }
            return result;
        }

        public TransactionResult InsertOrUpdateExternalRefuse(FactExternalRefuse data, UpdateFlag flag, int? accountId = null)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.ExternalRefuseId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException("Добавление внешнего отказа произошло с ошибкой.\r\nЗапрос:{1}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Обновление внешнего отказа ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.ExternalRefuseId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.MedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateMedicalEventAsTerritorial(db, data.MedicalEventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.MedicalEventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.Account))
                    {
                        var updateAccountResult = UpdateTerritoryAccount(db, accountId);
                        if (updateAccountResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Счета ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(accountId, db.LastQuery));
                        }
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult InsertOrUpdateZExternalRefuse(ZFactExternalRefuse data, UpdateFlag flag, int zslMevemtId, int? accountId = null)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.ExternalRefuseId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException("Добавление внешнего отказа произошло с ошибкой.\r\nЗапрос:{1}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Обновление внешнего отказа ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.ExternalRefuseId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.MedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateZMedicalEventAsTerritorial(db, data.ZmedicalEventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.ZmedicalEventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.ZslMedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateZslMedicalEventAsTerritorial(db, zslMevemtId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление законченного случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(zslMevemtId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.Account))
                    {
                        var updateAccountResult = UpdateZTerritoryAccount(db, accountId);
                        if (updateAccountResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Счета ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(accountId, db.LastQuery));
                        }
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<IEnumerable<TerritoryAccountView>> GetTerritoryAccountView(Expression<Func<TerritoryAccountView, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<TerritoryAccountView>>();
                try
                {
                    result.Data = predicate == null ?
                        db.TerritoryAccountView.ToList() :
                        db.TerritoryAccountView.Where(predicate).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }
        

        public TransactionResult CopyPatientToAnotherTerritoryAccount(int patientId, int territoryAccountId, int? selectedTerritory,
            int? selectedInsurance, string ogrn, List<object> selectedMevents)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
           {
               try
               {
                   var patient = db.GetTableQuery<FactPatient>().FirstOrDefault(p => p.PatientId == patientId);
                   if (patient != null)
                   {
                       db.BeginTransaction();

                       var parentAccountResult = GetTerritoryAccountById(patient.AccountId ?? 0);
                       if (parentAccountResult.HasError)
                       {
                           throw new InvalidOperationException("Ошибка получения данных счета ID {0}, {1}".F(patient.AccountId, db.LastQuery));
                       }
                       var accountResult = GetTerritoryAccountById(territoryAccountId);
                       if (accountResult.HasError)
                       {
                           throw new InvalidOperationException("Ошибка получения данных счета ID {0}, {1}".F(territoryAccountId, db.LastQuery));
                       }
                       var account = accountResult.Data;
                       var parentAccount = parentAccountResult.Data;

                       patient.PatientId = 0;
                       patient.AccountId = territoryAccountId;
                       patient.TerritoryOkato = selectedTerritory;
                       patient.InsuranceId = selectedInsurance;
                       patient.InsuranceOgrn = ogrn;
                       patient.ParentId = patientId;
                       var insertResult = db.InsertWithIdentity(patient);
                       if (!Convert.ToBoolean(insertResult))
                       {
                          throw new InvalidOperationException("Ошибка создания копии пациента ID {0}, {1}".F(patientId, db.LastQuery));
                       }

                       var newPatientId = Convert.ToInt32(insertResult);
                       var mevent = db.GetTableQuery<FactMedicalEvent>().Where(p => p.PatientId == patientId ).ToList();
                       foreach (var medicalEvent in mevent.Where(medicalEvent => selectedMevents.Contains(medicalEvent.MedicalEventId)))
                       {
                           int medicalEventId = medicalEvent.MedicalEventId;
                           medicalEvent.MedicalEventId = 0;
                           medicalEvent.PatientId = newPatientId;
                           medicalEvent.AcceptPrice = medicalEvent.Price;
                           medicalEvent.PaymentStatus = 2;
                           var resultMevent = db.InsertWithIdentity(medicalEvent);
                           if (!Convert.ToBoolean(resultMevent))
                           {
                               throw new InvalidOperationException("Ошибка создания копии случая МП ID {0}, {1}".F(medicalEventId, db.LastQuery));
                           }
                       }

                       db.GetTableQuery<FactPatient>()
                           .Where(p => p.PatientId == patientId)
                           .Set(s => s.MedicalAccountId, default(int?))
                           .Update();
                   

                       var updateAccountResult = UpdateTerritoryAccount(db, territoryAccountId);
                       if (updateAccountResult.HasError)
                       {
                           throw new InvalidOperationException("Ошибка обновления счета с территории ID {0}, {1}".F(territoryAccountId, db.LastQuery));
                       }

                       if (account.Type == (int) AccountType.GeneralPart)
                       {
                           var enumeratePatientResult = EnumeratePatientsOfTerritoryAccountTransact(territoryAccountId, db);
                           if (enumeratePatientResult.HasError)
                           {
                               throw new InvalidOperationException("Ошибка обновления порядка внешних id пациентов в счете с территории ID {0}, {1}".F(territoryAccountId, db.LastQuery));
                           }

                           var enumerateEventResult = EnumerateEventsOfTerritoryAccountTransact(territoryAccountId, db);
                           if (enumerateEventResult.HasError)
                           {
                               throw new InvalidOperationException("Ошибка обновления порядка внешних id случаев в счете с территории ID {0}, {1}".F(territoryAccountId, db.LastQuery));
                           }
                       }

                       db.CommitTransaction();
                    }
               }
               catch (Exception exception)
               {
                   db.RollbackTransaction();
                   result.AddError(exception);
               }
               return result;
           }
        }

        public TransactionResult CopyZPatientToAnotherTerritoryAccount(int patientId, int territoryAccountId, int? selectedTerritory,
            int? selectedInsurance, string ogrn, List<object> selectedMevents)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    var patient = db.GetTableQuery<FactPatient>().FirstOrDefault(p => p.PatientId == patientId);
                    if (patient != null)
                    {
                        db.BeginTransaction();

                        var parentAccountResult = GetTerritoryAccountById(patient.AccountId ?? 0);
                        if (parentAccountResult.HasError)
                        {
                            throw new InvalidOperationException("Ошибка получения данных счета ID {0}, {1}".F(patient.AccountId, db.LastQuery));
                        }
                        var accountResult = GetTerritoryAccountById(territoryAccountId);
                        if (accountResult.HasError)
                        {
                            throw new InvalidOperationException("Ошибка получения данных счета ID {0}, {1}".F(territoryAccountId, db.LastQuery));
                        }
                        var account = accountResult.Data;
                        var parentAccount = parentAccountResult.Data;

                        if (account.Version != parentAccount.Version)
                        {
                            throw new InvalidOperationException("Копирование пациента в разные версии счетов не возможно");
                        }

                        patient.PatientId = 0;
                        patient.AccountId = territoryAccountId;
                        patient.TerritoryOkato = selectedTerritory;
                        patient.InsuranceId = selectedInsurance;
                        patient.InsuranceOgrn = ogrn;
                        patient.ParentId = patientId;
                        var insertResult = db.InsertWithIdentity(patient);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException("Ошибка создания копии пациента ID {0}, {1}".F(patientId, db.LastQuery));
                        }

                        var newPatientId = Convert.ToInt32(insertResult);
                        var zslmevent = db.GetTableQuery<ZslFactMedicalEvent>().Where(p => p.PatientId == patientId).ToList();
                        foreach (var medicalEvent in zslmevent.Where(medicalEvent => selectedMevents.Contains(medicalEvent.ZslMedicalEventId)))
                        {
                            int zslmedicalEventId = medicalEvent.ZslMedicalEventId;
                            medicalEvent.ZslMedicalEventId = 0;
                            medicalEvent.PatientId = newPatientId;
                            medicalEvent.ZslIdGuid = Guid.NewGuid();
                            medicalEvent.AcceptPrice = medicalEvent.Price;
                            medicalEvent.RefusalPrice = 0;
                            medicalEvent.PaymentStatus = 2;
                            var resultZslMevent = db.InsertWithIdentity(medicalEvent);
                            if (!Convert.ToBoolean(resultZslMevent))
                            {
                                throw new InvalidOperationException("Ошибка создания копии законченного случая МП ID {0}, {1}".F(zslmedicalEventId, db.LastQuery));
                            }
                            var newZslMedicalEventId = Convert.ToInt32(resultZslMevent);
                            var mevent = db.GetTableQuery<ZFactMedicalEvent>().Where(p => p.ZslMedicalEventId == zslmedicalEventId).ToList();
                            foreach (var zFactMedicalEvent in mevent)
                            {
                                int zmedicalEventId = zFactMedicalEvent.ZmedicalEventId;
                                zFactMedicalEvent.ZmedicalEventId = 0;
                                zFactMedicalEvent.ZslMedicalEventId = newZslMedicalEventId;
                                zFactMedicalEvent.SlIdGuid = Guid.NewGuid().ToString();
                                var resultMevent = db.InsertWithIdentity(zFactMedicalEvent);
                                if (!Convert.ToBoolean(resultMevent))
                                {
                                    throw new InvalidOperationException("Ошибка создания копии случая МП ID {0}, {1}".F(zmedicalEventId, db.LastQuery));
                                }
                                var resultMeventId = Convert.ToInt32(resultMevent);
                                var service = db.GetTableQuery<ZFactMedicalServices>().Where(p => p.ZmedicalEventId == zmedicalEventId).ToList();
                                foreach (var zFactMedicalServicese in service)
                                {
                                    int zserviceId = zFactMedicalServicese.ZmedicalServicesId;
                                    zFactMedicalServicese.ZmedicalServicesId = 0;
                                    zFactMedicalServicese.ZmedicalEventId = resultMeventId;
                                    var resultService = db.InsertWithIdentity(zFactMedicalServicese);
                                    if (!Convert.ToBoolean(resultService))
                                    {
                                        throw new InvalidOperationException("Ошибка создания копии услуги ID {0}, {1}".F(zserviceId, db.LastQuery));
                                    }
                                }
                               var ksgKgp = db.GetTableQuery<ZFactKsgKpg>().Where(p => p.ZmedicalEventId == zmedicalEventId).ToList();
                                foreach (var zFactKsgKpg in ksgKgp)
                                {
                                    int zksgKpgId = zFactKsgKpg.ZksgKpgId;
                                    zFactKsgKpg.ZksgKpgId = 0;
                                    zFactKsgKpg.ZmedicalEventId = resultMeventId;
                                    var resultService = db.InsertWithIdentity(zFactKsgKpg);
                                    if (!Convert.ToBoolean(resultService))
                                    {
                                        throw new InvalidOperationException("Ошибка создания копии КСГ ID {0}, {1}".F(zksgKpgId, db.LastQuery));
                                    }
                                }
                            }
                        }

                        db.GetTableQuery<FactPatient>()
                            .Where(p => p.PatientId == patientId)
                            .Set(s => s.MedicalAccountId, default(int?))
                            .Update();


                        var updateAccountResult = UpdateZTerritoryAccount(db, territoryAccountId);
                        if (updateAccountResult.HasError)
                        {
                            throw new InvalidOperationException("Ошибка обновления счета с территории ID {0}, {1}".F(territoryAccountId, db.LastQuery));
                        }

                        if (account.Type == (int)AccountType.GeneralPart)
                        {
                            var enumeratePatientResult = EnumeratePatientsOfTerritoryAccountTransact(territoryAccountId, db);
                            if (enumeratePatientResult.HasError)
                            {
                                throw new InvalidOperationException("Ошибка обновления порядка внешних id пациентов в счете с территории ID {0}, {1}".F(territoryAccountId, db.LastQuery));
                            }

                            var enumerateEventResult = EnumerateZEventsOfTerritoryAccountTransact(territoryAccountId, db);
                            if (enumerateEventResult.HasError)
                            {
                                throw new InvalidOperationException("Ошибка обновления порядка внешних id случаев в счете с территории ID {0}, {1}".F(territoryAccountId, db.LastQuery));
                            }
                        }

                        db.CommitTransaction();
                    }
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult MovePatientToAnotherTerritoryAccount(int patientId, int accountId, int parentAccountId,
            int? selectedTerritory, int? selectedInsurance, string ogrn)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
           {
               try
               {
                   var parentAccountResult = GetTerritoryAccountById(parentAccountId);
                   if (parentAccountResult.HasError)
                   {
                       throw new InvalidOperationException("Ошибка получения данных счета ID {0}, {1}".F(parentAccountId, db.LastQuery));
                   }
                   var accountResult = GetTerritoryAccountById(accountId);
                   if (accountResult.HasError)
                   {
                       throw new InvalidOperationException("Ошибка получения данных счета ID {0}, {1}".F(accountId, db.LastQuery));
                   }
                   var account = accountResult.Data;
                   var parentAccount = parentAccountResult.Data;

                   db.BeginTransaction();
                   db.GetTableQuery<FactPatient>()
                           .Where(p => p.PatientId == patientId)
                           .Set(s => s.AccountId, accountId)
                           .Set(s => s.TerritoryOkato, selectedTerritory)
                           .Set(s => s.InsuranceId, selectedInsurance)
                           .Set(s => s.InsuranceOgrn, ogrn )
                           .Update();

                   var updateAccountResult = Constants.Zversion.Contains(accountResult.Data.Version??Constants.Version30) ? 
                       UpdateZTerritoryAccount(db, accountId): 
                       UpdateTerritoryAccount(db, accountId);
                   if (updateAccountResult.HasError)
                   {
                       throw new InvalidOperationException("Ошибка обновления счета с территории ID {0}, {1}".F(accountId, db.LastQuery));
                   }

                   if (account.Type == (int) AccountType.GeneralPart)
                   {
                       var enumeratePatientResult = EnumeratePatientsOfTerritoryAccountTransact(accountId, db);
                       if (enumeratePatientResult.HasError)
                       {
                           throw new InvalidOperationException("Ошибка обновления порядка внешних id пациентов в счете с территории ID {0}, {1}".F(accountId, db.LastQuery));
                       }

                       var enumerateEventResult = Constants.Zversion.Contains(accountResult.Data.Version ?? Constants.Version30) ? 
                            EnumerateZEventsOfTerritoryAccountTransact(accountId, db):
                            EnumerateEventsOfTerritoryAccountTransact(accountId, db);
                       if (enumerateEventResult.HasError)
                       {
                           throw new InvalidOperationException("Ошибка обновления порядка внешних id случаев в счете с территории ID {0}, {1}".F(accountId, db.LastQuery));
                       }
                   }

                   
                   var updateParentAccountResult = Constants.Zversion.Contains(accountResult.Data.Version ?? Constants.Version30) ? 
                       UpdateZTerritoryAccount(db, parentAccountId): 
                       UpdateTerritoryAccount(db, parentAccountId);
                   if (updateParentAccountResult.HasError)
                   {
                       throw new InvalidOperationException("Ошибка обновления родительского счета с территории ID {0}, {1}".F(parentAccountId, db.LastQuery));
                   }

                   if (parentAccount.Type == (int)AccountType.GeneralPart)
                   {
                       var enumerateParentPatientResult = EnumeratePatientsOfTerritoryAccountTransact(parentAccountId, db);
                       if (enumerateParentPatientResult.HasError)
                       {
                           throw new InvalidOperationException("Ошибка обновления порядка внешних id пациентов в родительском счете с территории ID {0}, {1}".F(parentAccountId, db.LastQuery));
                       }

                       var enumerateParentEventResult = Constants.Zversion.Contains(accountResult.Data.Version ?? Constants.Version30) ?
                            EnumerateZEventsOfTerritoryAccountTransact(parentAccountId, db):
                            EnumerateEventsOfTerritoryAccountTransact(parentAccountId, db);
                       if (enumerateParentEventResult.HasError)
                       {
                           throw new InvalidOperationException("Ошибка обновления порядка внешних id случаев в родительском счете с территории ID {0}, {1}".F(parentAccountId, db.LastQuery));
                       }
                   }

                   db.CommitTransaction();
               }
               catch (Exception exception)
               {
                   db.RollbackTransaction();
                   result.AddError(exception);
               }
               return result;
           }
        }

        public TransactionResult InsertRegisterE(FactTerritoryAccount account, List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<FactMedicalEvent, List<FactMedicalServices>, List<FactExternalRefuse>>>>> data, bool isTestLoad)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                db.Command.CommandTimeout = 0;
                db.BeginTransaction();
                try
                {
                    var accountId = SafeConvert.ToInt32(db.InsertWithIdentity(account).ToString(), false);
                    if (!accountId.HasValue)
                    {
                        result.AddError(string.Format("Ошибка при записи счета от территории в базу данных"));
                        db.RollbackTransaction();
                        return result;
                    }

                    result.Id = accountId.Value;

                    foreach (var p in data)
                    {
                        var patient = p.Item1;
                        var person = p.Item2;
                        var document = p.Item3;

                        int? personId = default(int?);
                        int? documentId = default(int?);

                        if (person.IsNotNull())
                        {
                            var insertPersonResult = db.InsertWithIdentity(person);
                            if (Convert.ToInt32(insertPersonResult) == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи персональных данных пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            personId = Convert.ToInt32(insertPersonResult);
                        }

                        if (document.IsNotNull())
                        {
                            if (personId.HasValue)
                            {
                                document.PersonId = personId.Value;
                            }
                            else if (patient.PersonalId.HasValue)
                            {
                                document.PersonId = patient.PersonalId.Value;
                            }
                            else
                            {
                                result.AddError(string.Format("Отсутствует PersonalId у пациента"));
                                db.RollbackTransaction();
                                return result;
                            }

                            var insertDocumentResult = db.InsertWithIdentity(document);
                            if (Convert.ToInt32(insertDocumentResult) == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи данных УДЛ пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            documentId = Convert.ToInt32(insertDocumentResult);
                        }

                        patient.AccountId = accountId;
                        if (personId.HasValue)
                        {
                            patient.PersonalId = personId;
                        }

                        if (documentId.HasValue)
                        {
                            patient.DocumentId = documentId;
                        }


                        var insertPatientResult = db.InsertWithIdentity(patient);
                        var patientId = Convert.ToInt32(insertPatientResult);
                        if (patientId == 0)
                        {
                            result.AddError(string.Format("Ошибка при записи данных пациента в базу данных"));
                            db.RollbackTransaction();
                            return result;
                        }

                        var mevents = p.Item4;
                        foreach (var m in mevents)
                        {
                            var mevent = m.Item1;
                            mevent.PatientId = patientId;
                            var meventInsertResult = db.InsertWithIdentity(mevent);
                            var meventId = Convert.ToInt32(meventInsertResult);
                            if (meventId == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи случая в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }

                            var services = m.Item2.ToList();
                            if (services.Any())
                            {
                                services.ForEach(s => s.MedicalEventId = meventId);
                                var insertServicesResult = db.InsertBatch(services);
                                if (Convert.ToInt32(insertServicesResult) == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи услуг в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }
                            }

                            var refusals = m.Item3.ToList();
                            if (refusals.Any())
                            {
                                refusals.ForEach(s =>
                                {
                                    s.MedicalEventId = meventId;
                                    s.PatientId = patientId;
                                });
                                var insertRefusalsResult = db.InsertBatch(refusals);
                                if (Convert.ToInt32(insertRefusalsResult) == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи отказов от территории в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }
                            }
                        }
                    }


                    /*var updateResult = UpdateTerritoryAccount(db, accountId);
                    if (updateResult.HasError)
                    {
                        result.AddError(string.Format("Ошибка при обновлении счета от территории"));
                        db.RollbackTransaction();
                        return result;
                    }*/

                    if (isTestLoad)
                    {
                        db.RollbackTransaction();
                    }
                    else
                    {
                        db.CommitTransaction();
                    }
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }

            }
            return result;
        }

        public TransactionResult InsertRegisterE(FactTerritoryAccount account, List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>>> data, bool isTestLoad)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                db.Command.CommandTimeout = 0;
                db.BeginTransaction();
                var kol = 0;
                try
                {
                    var accountId = SafeConvert.ToInt32(db.InsertWithIdentity(account).ToString(), false);
                    if (!accountId.HasValue)
                    {
                        result.AddError(string.Format("Ошибка при записи счета от территории в базу данных"));
                        db.RollbackTransaction();
                        return result;
                    }

                    result.Id = accountId.Value;
                    
                    foreach (var p in data)
                    {
                        var patient = p.Item1;
                        var person = p.Item2;
                        var document = p.Item3;

                        int? personId = default(int?);
                        int? documentId = default(int?);

                        if (person.IsNotNull())
                        {
                            var insertPersonResult = db.InsertWithIdentity(person);
                            if (Convert.ToInt32(insertPersonResult) == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи персональных данных пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            personId = Convert.ToInt32(insertPersonResult);
                        }

                        if (document.IsNotNull())
                        {
                            if (personId.HasValue)
                            {
                                document.PersonId = personId.Value;
                            }
                            else if (patient.PersonalId.HasValue)
                            {
                                document.PersonId = patient.PersonalId.Value;
                            }
                            else
                            {
                                result.AddError(string.Format("Отсутствует PersonalId у пациента"));
                                db.RollbackTransaction();
                                return result;
                            }

                            var insertDocumentResult = db.InsertWithIdentity(document);
                            if (Convert.ToInt32(insertDocumentResult) == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи данных УДЛ пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            documentId = Convert.ToInt32(insertDocumentResult);
                        }

                        patient.AccountId = accountId;
                        if (personId.HasValue)
                        {
                            patient.PersonalId = personId;
                        }

                        if (documentId.HasValue)
                        {
                            patient.DocumentId = documentId;
                        }


                        var insertPatientResult = db.InsertWithIdentity(patient);
                        var patientId = Convert.ToInt32(insertPatientResult);
                        if (patientId == 0)
                        {
                            result.AddError(string.Format("Ошибка при записи данных пациента в базу данных"));
                            db.RollbackTransaction();
                            return result;
                        }

                        var zslmevents = p.Item4;
                        
                        foreach (var zslmevent in zslmevents)
                        {
                            kol = kol+ 1;
                            var mevent = zslmevent.Item1;
                            mevent.PatientId = patientId;
                            var zslmeventInsertResult = db.InsertWithIdentity(mevent);
                            var zslmeventId = Convert.ToInt32(zslmeventInsertResult);
                            if (zslmeventId == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи законченного случая в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }

                            foreach (var zmevent in zslmevent.Item2)
                            {
                                var me = zmevent.Item1;
                                me.ZslMedicalEventId = zslmeventId;
                                var zmeventInsertResult = db.InsertWithIdentity(me);
                                var meventId = Convert.ToInt32(zmeventInsertResult);
                                if (meventId == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи случая в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }

                                var ksgKpg = zmevent.Item2.Item1;
                                if (ksgKpg != null)
                                {
                                    ksgKpg.ZmedicalEventId = meventId;
                                    var insertKsfKpgResult = db.InsertWithIdentity(ksgKpg);
                                    var zksgKpgId = Convert.ToInt32(insertKsfKpgResult);
                                    if (zksgKpgId == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи КСГ в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }

                                    var slKoefs = zmevent.Item2.Item2;
                                    if (slKoefs.Any())
                                    {
                                        slKoefs.ForEach(s => s.ZksgKpgId = zksgKpgId);
                                        var insertslKoefsResult = db.InsertBatch(slKoefs);
                                        if (Convert.ToInt32(insertslKoefsResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи Коэффициента сложности лечения пациента в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }
                                }

                                var services = zmevent.Item3.ToList();
                                if (services.Any())
                                {
                                    services.ForEach(s => s.ZmedicalEventId = meventId);
                                    var insertServicesResult = db.InsertBatch(services);
                                    if (Convert.ToInt32(insertServicesResult) == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи услуг в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }
                                }

                                var refusals = zmevent.Item4.ToList();
                                if (refusals.Any())
                                {
                                    refusals.ForEach(s =>
                                    {
                                        s.ZmedicalEventId = meventId;
                                        s.PatientId = patientId;
                                    });
                                    var insertRefusalsResult = db.InsertBatch(refusals);
                                    if (Convert.ToInt32(insertRefusalsResult) == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи отказов от территории в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }
                                }
                            }
                        }
                    }

                    /*var updateResult = UpdateTerritoryAccount(db, accountId);
                    if (updateResult.HasError)
                    {
                        result.AddError(string.Format("Ошибка при обновлении счета от территории"));
                        db.RollbackTransaction();
                        return result;
                    }*/

                    if (isTestLoad)
                    {
                        db.RollbackTransaction();
                    }
                    else
                    {
                        db.CommitTransaction();
                    }
                }
                catch (Exception exception)
                {
                     
                    db.RollbackTransaction();
                    result.AddError(exception);
                }

            }
            return result;
        }

        public TransactionResult InsertRegisterE(FactTerritoryAccount account,
            List<Tuple<FactPatient, FactPerson, FactDocument,
                    List<Tuple<ZslFactMedicalEvent,
                        List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            List<ZFactDs>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
                        List<ZFactExternalRefuse>
                            >>>> data, bool isTestLoad)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                db.Command.CommandTimeout = 0;
                db.BeginTransaction();
                var kol = 0;
                try
                {
                    var accountId = SafeConvert.ToInt32(db.InsertWithIdentity(account).ToString(), false);
                    if (!accountId.HasValue)
                    {
                        result.AddError(string.Format("Ошибка при записи счета от территории в базу данных"));
                        db.RollbackTransaction();
                        return result;
                    }

                    result.Id = accountId.Value;

                    foreach (var p in data)
                    {
                        var patient = p.Item1;
                        var person = p.Item2;
                        var document = p.Item3;

                        int? personId = default(int?);
                        int? documentId = default(int?);

                        if (person.IsNotNull())
                        {
                            var insertPersonResult = db.InsertWithIdentity(person);
                            if (Convert.ToInt32(insertPersonResult) == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи персональных данных пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            personId = Convert.ToInt32(insertPersonResult);
                        }

                        if (document.IsNotNull())
                        {
                            if (personId.HasValue)
                            {
                                document.PersonId = personId.Value;
                            }
                            else if (patient.PersonalId.HasValue)
                            {
                                document.PersonId = patient.PersonalId.Value;
                            }
                            else
                            {
                                result.AddError(string.Format("Отсутствует PersonalId у пациента"));
                                db.RollbackTransaction();
                                return result;
                            }

                            var insertDocumentResult = db.InsertWithIdentity(document);
                            if (Convert.ToInt32(insertDocumentResult) == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи данных УДЛ пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            documentId = Convert.ToInt32(insertDocumentResult);
                        }

                        patient.AccountId = accountId;
                        if (personId.HasValue)
                        {
                            patient.PersonalId = personId;
                        }

                        if (documentId.HasValue)
                        {
                            patient.DocumentId = documentId;
                        }


                        var insertPatientResult = db.InsertWithIdentity(patient);
                        var patientId = Convert.ToInt32(insertPatientResult);
                        if (patientId == 0)
                        {
                            result.AddError(string.Format("Ошибка при записи данных пациента в базу данных"));
                            db.RollbackTransaction();
                            return result;
                        }

                        var zslmevents = p.Item4;

                        foreach (var zslmevent in zslmevents)
                        {
                            kol = kol + 1;
                            var mevent = zslmevent.Item1;
                            mevent.PatientId = patientId;
                            var zslmeventInsertResult = db.InsertWithIdentity(mevent);
                            var zslmeventId = Convert.ToInt32(zslmeventInsertResult);
                            if (zslmeventId == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи законченного случая в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            var refuz = new List<ZFactExternalRefuse>();
                            //int eventCount = 0;
                            //int slidEvent;
                            foreach (var zmevent in zslmevent.Item2)
                            {
                                var me = zmevent.Item1;
                                me.ZslMedicalEventId = zslmeventId;
                                var zmeventInsertResult = db.InsertWithIdentity(me);
                                var meventId = Convert.ToInt32(zmeventInsertResult);
                                if (meventId == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи случая в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }
                                //if (eventCount == 0)
                                //{
                                //    slidEvent = meventId;
                                //}
                                //eventCount++;

                                var directions = zmevent.Item2;
                                if (directions.Any())
                                {
                                    directions.ForEach(s =>
                                    {
                                        s.ZmedicalEventId = meventId;
                                    });
                                    var insertdirectionResult = db.InsertBatch(directions);
                                    if (Convert.ToInt32(insertdirectionResult) == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи направления в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }
                                }
                                var сonsultations = zmevent.Item3;
                                if (сonsultations.Any())
                                {
                                    сonsultations.ForEach(s =>
                                    {
                                        s.ZMedicalEventId = meventId;
                                    });
                                    var insertсonsultationsResult = db.InsertBatch(сonsultations);
                                    if (Convert.ToInt32(insertсonsultationsResult) == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи консилиума в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }
                                }
                                var ds2 = zmevent.Item4;
                                if (ds2.Any())
                                {
                                    ds2.ForEach(s =>
                                    {
                                        s.ZmedicalEventId = meventId;
                                    });
                                    var insertds2Result = db.InsertBatch(ds2);
                                    if (Convert.ToInt32(insertds2Result) == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи сопутствующего диагноза в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }
                                }

                                var medicalEventOnk = zmevent.Item5.Item1;
                                if (medicalEventOnk != null)
                                {
                                    ControlResourcesLoger.LogDedug(zmevent.Item1.SlIdGuid + " - " + zmevent.Item1.ExternalId);
                                    medicalEventOnk.ZmedicalEventId = meventId;
                                    var insertmedicalEventOnkResult = db.InsertWithIdentity(medicalEventOnk);
                                    var zmedicalEventOnkId = Convert.ToInt32(insertmedicalEventOnkResult);
                                    if (zmedicalEventOnkId == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи сведения о случае лечения онкологического заболевания в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }

                                    var diagBloks = zmevent.Item5.Item2;
                                    if (diagBloks.Any())
                                    {
                                        diagBloks.ForEach(s => s.ZMedicalEventOnkId = zmedicalEventOnkId);
                                        var insertdiagBloksResult = db.InsertBatch(diagBloks);
                                        if (Convert.ToInt32(insertdiagBloksResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи диагностического блока в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }
                                    var сontraindications = zmevent.Item5.Item3;
                                    if (сontraindications.Any())
                                    {
                                        сontraindications.ForEach(s => s.ZMedicalEventOnkId = zmedicalEventOnkId);
                                        var insertсontraindicationsResult = db.InsertBatch(сontraindications);
                                        if (Convert.ToInt32(insertсontraindicationsResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи сведений об имеющихся противопоказаниях и отказах в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }
                                    var zFactMedicalServiceOnk = zmevent.Item5.Item4;
                                    if (zFactMedicalServiceOnk.Any())
                                    {
                                        foreach (var usl in zFactMedicalServiceOnk)
                                        {
                                            var zFactmedicalServiceOnk = usl.Item1;
                                            zFactmedicalServiceOnk.ZmedicalEventOnkId = zmedicalEventOnkId;
                                            var zFactmedicalServiceOnkResult = db.InsertWithIdentity(zFactmedicalServiceOnk);
                                            var zFactmedicalServiceOnkResultId = Convert.ToInt32(zFactmedicalServiceOnkResult);
                                            if (zFactmedicalServiceOnkResultId == 0)
                                            {
                                                result.AddError(string.Format("Ошибка при записи онко услуги в базу данных"));
                                                db.RollbackTransaction();
                                                return result;
                                            }

                                            var zfactAnticancerDrugs = usl.Item2;
                                            if (zfactAnticancerDrugs.Any())
                                            {
                                                zfactAnticancerDrugs.ForEach(s => s.ZMedicalServiceOnkId = zFactmedicalServiceOnkResultId);
                                                var insertAnticancerDrugsResult = db.InsertBatch(zfactAnticancerDrugs);
                                                if (Convert.ToInt32(insertAnticancerDrugsResult) == 0)
                                                {
                                                    result.AddError(string.Format("Ошибка при записи cведения о введенном противоопухолевом лекарственном препарате направления в базу данных"));
                                                    db.RollbackTransaction();
                                                    return result;
                                                }
                                            }
                                        }
                                    }
                                }

                                var ksgKpg = zmevent.Item6.Item1;
                                if (ksgKpg != null)
                                {
                                    ksgKpg.ZmedicalEventId = meventId;
                                    var insertKsfKpgResult = db.InsertWithIdentity(ksgKpg);
                                    var zksgKpgId = Convert.ToInt32(insertKsfKpgResult);
                                    if (zksgKpgId == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи КСГ в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }

                                    var slcrit = zmevent.Item6.Item2;
                                    if (slcrit.Any())
                                    {
                                        slcrit.ForEach(s => s.ZksgKpgId = zksgKpgId);
                                        var insertslKoefsResult = db.InsertBatch(slcrit);
                                        if (Convert.ToInt32(insertslKoefsResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи slcrit в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }

                                    var slKoefs = zmevent.Item6.Item3;
                                    if (slKoefs.Any())
                                    {
                                        slKoefs.ForEach(s => s.ZksgKpgId = zksgKpgId);
                                        var insertslKoefsResult = db.InsertBatch(slKoefs);
                                        if (Convert.ToInt32(insertslKoefsResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи Коэффициента сложности лечения пациента в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }
                                }

                                var services = zmevent.Item7;
                                if (services.Any())
                                {
                                    services.ForEach(s => s.ZmedicalEventId = meventId);
                                    var insertServicesResult = db.InsertBatch(services);
                                    if (Convert.ToInt32(insertServicesResult) == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи услуг в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }
                                }
                                refuz.Add(new ZFactExternalRefuse {ZslMedicalEventId = zslmeventId,  ZmedicalEventId = meventId, SlidGuid = me.ExternalId});
                            }
                            var refusals = zslmevent.Item3.ToList();
                            if (refusals.Any())
                            {
                                refusals.ForEach(s =>
                                {
                                    s.ZslMedicalEventId = zslmeventId;
                                    s.PatientId = patientId;
                                });
                                var insertRefusalsResult = db.InsertBatch(refusals);
                                if (Convert.ToInt32(insertRefusalsResult) == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи отказов от территории в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }
                            }
                           
                            foreach (var tuple in refuz)
                            {
                                db.GetTableQuery<ZFactExternalRefuse>().Where(s => s.ZslMedicalEventId == tuple.ZslMedicalEventId && s.SlidGuid == tuple.SlidGuid )
                                .Set(s => s.ZmedicalEventId, tuple.ZmedicalEventId).Update();
                            }


                        }
                    }
                    
                    /*var updateResult = UpdateTerritoryAccount(db, accountId);
                    if (updateResult.HasError)
                    {
                        result.AddError(string.Format("Ошибка при обновлении счета от территории"));
                        db.RollbackTransaction();
                        return result;
                    }*/

                    if (isTestLoad)
                    {
                        db.RollbackTransaction();
                    }
                    else
                    {
                        db.CommitTransaction();
                    }
                }
                catch (Exception exception)
                {

                    db.RollbackTransaction();
                    result.AddError(exception);
                }

            }
            return result;
        }


        public TransactionResult DeleteLocalRefusal(int accountId,int id, int refusalType, UpdateFlag flag)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    int eventId = 0;
                    switch (refusalType)
                    {
                        //МЭК
                        case 1:
                            var mec = db.FactMEC.FirstOrDefault(p => p.MECId == id);
                            if (mec.IsNull())
                            {
                                db.RollbackTransaction();
                                result.AddError(new InvalidOperationException("mec doesn't exists"));
                                return result;
                            }
                            eventId = mec.MedicalEventId ?? 0;
                            var deleteMecResult = db.Delete(mec);
                            if (!Convert.ToBoolean(deleteMecResult))
                            {
                                db.RollbackTransaction();
                                result.AddError(new InvalidOperationException("mec delete return 0\n{0}".F(db.LastQuery)));
                                return result;
                            }
                            break;
                        //МЭЭ
                        case 2:
                            var mee = db.FactMEE.FirstOrDefault(p => p.MEEId == id);
                            if (mee.IsNull())
                            {
                                db.RollbackTransaction();
                                result.AddError(new InvalidOperationException("mee doesn't exists"));
                                return result;
                            }
                            eventId = mee.MedicalEventId ?? 0;
                            var deleteMeeResult = db.Delete(mee);
                            if (!Convert.ToBoolean(deleteMeeResult))
                            {
                                db.RollbackTransaction();
                                result.AddError(new InvalidOperationException("mee delete return 0\n{0}".F(db.LastQuery)));
                                return result;
                            }
                            break;
                        //ЭКМП
                        case 3:
                            var eqma = db.FactEQMA.FirstOrDefault(p => p.EQMAId == id);
                            if (eqma.IsNull())
                            {
                                db.RollbackTransaction();
                                result.AddError(new InvalidOperationException("eqma doesn't exists"));
                                return result;
                            }
                            eventId = eqma.MedicalEventId ?? 0;
                            var deleteEqmaResult = db.Delete(eqma);
                            if (!Convert.ToBoolean(deleteEqmaResult))
                            {
                                db.RollbackTransaction();
                                result.AddError(new InvalidOperationException("eqma delete return 0\n{0}".F(db.LastQuery)));
                                return result;
                            }
                            break;
                        default:
                            result.AddError(new ArgumentException("Wrong refusalType"));
                            break;
                    }

                    if (flag.HasFlag(UpdateFlag.MedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateMedicalEventAsLocal(db, eventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(eventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.Account))
                    {
                        var updateAccountResult = UpdateMedicalAccount(db, accountId);
                        if (updateAccountResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Счета ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(accountId, db.LastQuery));
                        }
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteZLocalRefusal(int accountId, int zslMedicalEventId, int id, UpdateFlag flag)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    int eventId = 0;

                    var mec = db.ZFactSank.FirstOrDefault(p => p.ZSankId == id);
                    if (mec.IsNull())
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("mec doesn't exists"));
                        return result;
                    }
                    eventId = mec.ZmedicalEventId ?? 0;
                    var deleteMecResult = db.Delete(mec);
                    if (!Convert.ToBoolean(deleteMecResult))
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("mec delete return 0\n{0}".F(db.LastQuery)));
                        return result;
                    }

                    if (flag.HasFlag(UpdateFlag.MedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateZMedicalEventAsLocal(db, mec.ZmedicalEventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(mec.ZmedicalEventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.ZslMedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateZslMedicalEventAsLocal(db, zslMedicalEventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление законченного случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(zslMedicalEventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.Account))
                    {
                        var updateAccountResult = UpdateZMedicalAccount(db, accountId);
                        if (updateAccountResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Счета ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(accountId, db.LastQuery));
                        }
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteZdiagBlok(int id)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();

                    var diagBlok = db.ZFactDiagBlok.FirstOrDefault(p => p.ZDiagBlokId == id);
                    if (diagBlok.IsNull())
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("diagBlok doesn't exists"));
                        return result;
                    }
                    var deleteDiagBlok = db.Delete(diagBlok);
                    if (!Convert.ToBoolean(deleteDiagBlok))
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("diagBlok delete return 0\n{0}".F(db.LastQuery)));
                        return result;
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteZds(int id)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();

                    var ds = db.ZFactDs.FirstOrDefault(p => p.ZFactDsId == id);
                    if (ds.IsNull())
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("ZFactDs doesn't exists"));
                        return result;
                    }
                    var deleteds = db.Delete(ds);
                    if (!Convert.ToBoolean(deleteds))
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("ZFactDs delete return 0\n{0}".F(db.LastQuery)));
                        return result;
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteZAnticancerDrug(int id)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();

                    var anticancerDrug = db.ZFactAnticancerDrug.FirstOrDefault(p => p.ZAnticancerDrugId == id);
                    if (anticancerDrug.IsNull())
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("anticancerDrug doesn't exists"));
                        return result;
                    }
                    var deleteanticancerDrug = db.Delete(anticancerDrug);
                    if (!Convert.ToBoolean(deleteanticancerDrug))
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("deleteanticancerDrug delete return 0\n{0}".F(db.LastQuery)));
                        return result;
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteZDirection(int id)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();

                    var direction = db.ZFactDirection.FirstOrDefault(p => p.ZDirectionId == id);
                    if (direction.IsNull())
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("direction doesn't exists"));
                        return result;
                    }
                    var deleteDirection = db.Delete(direction);
                    if (!Convert.ToBoolean(deleteDirection))
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("direction delete return 0\n{0}".F(db.LastQuery)));
                        return result;
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteZContraindications(int id)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();

                    var contraindications = db.ZFactContraindications.FirstOrDefault(p => p.ZContraindicationsId == id);
                    if (contraindications.IsNull())
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("contraindications doesn't exists"));
                        return result;
                    }
                    var deleteContraindications = db.Delete(contraindications);
                    if (!Convert.ToBoolean(deleteContraindications))
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("contraindications delete return 0\n{0}".F(db.LastQuery)));
                        return result;
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }
        public TransactionResult DeleteSlkoef(int id)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();

                    var slkoef = db.ZFactSlKoef.FirstOrDefault(p => p.ZslKoefId == id);
                    if (slkoef.IsNull())
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("slkoef doesn't exists"));
                        return result;
                    }
                    var deleteResult = db.Delete(slkoef);
                    if (!Convert.ToBoolean(deleteResult))
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("slkoef delete return 0\n{0}".F(db.LastQuery)));
                        return result;
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteCrit(int id)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();

                    var crit = db.ZFactCrit.FirstOrDefault(p => p.ZCritId == id);
                    if (crit.IsNull())
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("crit doesn't exists"));
                        return result;
                    }
                    var deleteResult = db.Delete(crit);
                    if (!Convert.ToBoolean(deleteResult))
                    {
                        db.RollbackTransaction();
                        result.AddError(new InvalidOperationException("crit delete return 0\n{0}".F(db.LastQuery)));
                        return result;
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<FactMedicalAccount> GetMedicalAccountById(int id)
        {
            var result = new TransactionResult<FactMedicalAccount>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = MedicalAccountByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<ZFactSank> GetZsankById(int id)
        {
            var result = new TransactionResult<ZFactSank>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZFactSank.FirstOrDefault(p=>p.ZSankId == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<ZFactDiagBlok> GetZDiagBlokById(int id)
        {
            var result = new TransactionResult<ZFactDiagBlok>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZFactDiagBlok.FirstOrDefault(p => p.ZDiagBlokId == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<ZFactDs> GetZDsById(int id)
        {
            var result = new TransactionResult<ZFactDs>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZFactDs.FirstOrDefault(p => p.ZFactDsId == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<ZFactAnticancerDrug> GetZAnticancerDrugById(int id)
        {
            var result = new TransactionResult<ZFactAnticancerDrug>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZFactAnticancerDrug.FirstOrDefault(p => p.ZAnticancerDrugId == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<ZFactDirection> GetZDirectionById(int id)
        {
            var result = new TransactionResult<ZFactDirection>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZFactDirection.FirstOrDefault(p => p.ZDirectionId == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<IEnumerable<ZFactDirection>> GetZDirectionBySlMeventId(int id)
        {
            var result = new TransactionResult<IEnumerable<ZFactDirection>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZFactDirection.Where(p => p.ZmedicalEventId == id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<IEnumerable<ZFactDs>> GetZDsBySlMeventId(int id)
        {
            var result = new TransactionResult<IEnumerable<ZFactDs>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZFactDs.Where(p => p.ZmedicalEventId == id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<IEnumerable<ZFactConsultations>> GetZConsultationsBySlMeventId(int id)
        {
            var result = new TransactionResult<IEnumerable<ZFactConsultations>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZFactConsultations.Where(p => p.ZMedicalEventId == id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<ZFactContraindications> GetZContraindicationsById(int id)
        {
            var result = new TransactionResult<ZFactContraindications>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZFactContraindications.FirstOrDefault(p => p.ZContraindicationsId == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<FactMEC> GetMecById(int id)
        {
            var result = new TransactionResult<FactMEC>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactMEC.FirstOrDefault(p => p.MECId == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<FactMEE> GetMeeById(int id)
        {
            var result = new TransactionResult<FactMEE>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactMEE.FirstOrDefault(p => p.MEEId == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<FactEQMA> GetEqmaById(int id)
        {
            var result = new TransactionResult<FactEQMA>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactEQMA.FirstOrDefault(p => p.EQMAId == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        

        public TransactionResult UpdateMedicalEventAsLocal(int? medicalEventId)
        {
            TransactionResult result;
            using (var db = CreateContext())
            {
                result = UpdateMedicalEventAsLocal(db, medicalEventId);
            }
            return result;
        }

        public TransactionResult UpdateMedicalAccount(int? medicalAccountId)
        {
            TransactionResult result;
            using (var db = CreateContext())
            {
                result = UpdateMedicalAccount(db, medicalAccountId);
            }
            return result;
        }

        //public TransactionResult UpdateMedicalAccount(MedicineContext db, int? medicalAccountId, int? type = null)
        //{
        //    var result = new TransactionResult();

        //    try
        //    {
        //        FactMedicalAccount account = db.GetTableQuery<FactMedicalAccount>().FirstOrDefault(p => p.MedicalAccountId == medicalAccountId);
        //        if (account == null)
        //        {
        //            throw new ArgumentException("MedicalAccount ID {0} не найден".F(medicalAccountId));
        //        }

        //        //Получаем все МЭКи не блокированные и с источником 'ТФОМС1 к МО' или 'Уточнённые санкции ТФОМС1 к МО' (на случай если будет исправленная часть у МО)
        //        var mecList = db.GetTableQuery<ZFactSank>()
        //            .Distinct()
        //            .Where(p => p.FactPatient.MedicalAccountId == medicalAccountId &&
        //                        p.Type == 1 &&
        //                        (p.Source == (int) RefusalSource.Local || p.Source == (int) RefusalSource.LocalCorrected))
        //            .ToList();

        //        decimal mec = mecList.Distinct(p => p.ZslMedicalEventId).Sum(p => p.ZslAmount) ?? 0;


        //        //Получаем все МЭЭ
        //        decimal mee = db.GetTableQuery<ZFactSank>()
        //            .Where(p => p.FactPatient.MedicalAccountId == medicalAccountId &&
        //            p.Type == 2 && (p.Source == 1 || p.Source == 3))
        //            .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;
        //        //Получаем все ЭКМП
        //        decimal eqma = db.GetTableQuery<ZFactSank>()
        //            .Where(p => p.FactPatient.MedicalAccountId == medicalAccountId &&
        //                p.Type == 3 &&
        //                (p.Source == 1 || p.Source == 3))
        //            .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;
        //        //Получаем сумму выставленную к оплате
        //        decimal price = db.ZslFactMedicalEvent
        //            .Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId)
        //            .Sum(p => p.MoPrice) ?? 0;

        //        //считаем принятую к оплате сумму
        //        decimal? acceptPrice = price - (mec + mee + eqma);

        //        //обновляем запись счета
        //        db.GetTableQuery<FactMedicalAccount>().Where(s => s.MedicalAccountId == medicalAccountId)
        //           .Set(s => s.Price, price)
        //           .Set(s => s.MECPenalties, mec)
        //           .Set(s => s.MEEPenalties, mee)
        //           .Set(s => s.EQMAPenalties, eqma)
        //           .Set(s => s.AcceptPrice, acceptPrice)
        //           .Update();
        //    }
        //    catch (Exception exception)
        //    {
        //        result.AddError(exception);
        //    }

        //    return result;
        //}

        public TransactionResult UpdateZMedicalAccount(int? medicalAccountId)
        {
            TransactionResult result;
            using (var db = CreateContext())
            {
                result = UpdateZMedicalAccount(db, medicalAccountId);
            }
            return result;
        }

        public TransactionResult UpdateZMedicalAccount(MedicineContext db, int? medicalAccountId)
        {
            var result = new TransactionResult();

            try
            {
                FactMedicalAccount account = db.GetTableQuery<FactMedicalAccount>().FirstOrDefault(p => p.MedicalAccountId == medicalAccountId);
                if (account == null)
                {
                    throw new ArgumentException("MedicalAccount ID {0} не найден".F(medicalAccountId));
                }

                //Получаем все МЭКи не блокированные и с источником 'ТФОМС1 к МО' или 'Уточнённые санкции ТФОМС1 к МО' (на случай если будет исправленная часть у МО)
                var mecList = db.GetTableQuery<ZFactSank>()
                    .Distinct()
                    .Where(p => p.FactPatient.MedicalAccountId == medicalAccountId &&
                                p.Type == 1 &&
                                (p.Source == (int)RefusalSource.Local || p.Source == (int)RefusalSource.LocalCorrected))
                    .ToList();

                decimal mec = mecList.Distinct(p => p.ZslMedicalEventId).Sum(p => p.ZslAmount) ?? 0;


                //Получаем все МЭЭ
                decimal mee = db.GetTableQuery<ZFactSank>()
                    .Where(p => p.FactPatient.MedicalAccountId == medicalAccountId &&
                    Constants.Mee.Contains(p.Type) && (p.Source == 1 || p.Source == 3))
                    .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;
                //Получаем все ЭКМП
                decimal eqma = db.GetTableQuery<ZFactSank>()
                    .Where(p => p.FactPatient.MedicalAccountId == medicalAccountId &&
                        Constants.Eqma.Contains(p.Type) &&
                        (p.Source == 1 || p.Source == 3))
                    .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;
                //Получаем сумму выставленную к оплате
                decimal price = db.ZslFactMedicalEvent
                    .Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId)
                    .Sum(p => p.MoPrice) ?? 0;

                //считаем принятую к оплате сумму
                decimal? acceptPrice = price - (mec + mee + eqma);

                //обновляем запись счета
                db.GetTableQuery<FactMedicalAccount>().Where(s => s.MedicalAccountId == medicalAccountId)
                   .Set(s => s.Price, price)
                   .Set(s => s.MECPenalties, mec)
                   .Set(s => s.MEEPenalties, mee)
                   .Set(s => s.EQMAPenalties, eqma)
                   .Set(s => s.AcceptPrice, acceptPrice)
                   .Update();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult UpdateMedicalAccount(MedicineContext db, int? medicalAccountId)
        {
            var result = new TransactionResult();

            try
            {
                FactMedicalAccount account = db.GetTableQuery<FactMedicalAccount>().FirstOrDefault(p => p.MedicalAccountId == medicalAccountId);
                if (account == null)
                {
                    throw new ArgumentException("MedicalAccount ID {0} не найден".F(medicalAccountId));
                }

                //Получаем все МЭКи не блокированные и с источником 'ТФОМС1 к МО' или 'Уточнённые санкции ТФОМС1 к МО' (на случай если будет исправленная часть у МО)
                var mecList = db.GetTableQuery<FactMEC>()
                    .Distinct()
                    .Where(p => p.Patient.MedicalAccountId == medicalAccountId &&
                        (p.IsLock == null || p.IsLock == false) &&
                        (p.Source == (int)RefusalSource.Local || p.Source == (int)RefusalSource.LocalCorrected)).ToList();

                decimal mec = mecList.Distinct(p => p.MedicalEventId).Sum(p => p.Amount) ?? 0;


                //Получаем все МЭЭ
                decimal mee = db.GetTableQuery<FactMEE>()
                    .Where(p => p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId &&
                        (p.Source == 1 || p.Source == 3))
                    .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;
                //Получаем все ЭКМП
                decimal eqma = db.GetTableQuery<FactEQMA>()
                    .Where(p => p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId &&
                        (p.Source == 1 || p.Source == 3))
                    .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;
                //Получаем сумму выставленную к оплате
                decimal price = db.FactMedicalEvent
                    .Where(p => p.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId)
                    .Sum(p => p.MoPrice) ?? 0;

                //считаем принятую к оплате сумму
                decimal? acceptPrice = price - (mec + mee + eqma);

                //обновляем запись счета
                db.GetTableQuery<FactMedicalAccount>().Where(s => s.MedicalAccountId == medicalAccountId)
                   .Set(s => s.Price, price)
                   .Set(s => s.MECPenalties, mec)
                   .Set(s => s.MEEPenalties, mee)
                   .Set(s => s.EQMAPenalties, eqma)
                   .Set(s => s.AcceptPrice, acceptPrice)
                   .Update();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult UpdateMedicalEventAsLocal(MedicineContext db, int? medicalEventId)
        {
            var result = new TransactionResult();

            try
            {
                FactMedicalEvent mevent = db.GetTableQuery<FactMedicalEvent>().FirstOrDefault(p => p.MedicalEventId == medicalEventId);
                if (mevent == null)
                {
                    throw new ArgumentException("MedicalEvent ID {0} не найден".F(medicalEventId));
                }

                //Получаем все МЭКи не блокированные и с источником 'ТФОМС1 к МО' или 'Уточнённые санкции ТФОМС1 к МО' (на случай если будет исправленная часть у МО)
                var mecList = db.GetTableQuery<FactMEC>()
                    .Distinct()
                    .Where(p => p.MedicalEventId == medicalEventId &&
                        (p.IsLock == null || p.IsLock == false) &&
                        (p.Source == (int)RefusalSource.Local || p.Source == (int)RefusalSource.LocalCorrected)).ToList();

                decimal mec = mecList.Distinct(p => p.MedicalEventId).Sum(p => p.Amount) ?? 0;

                decimal mee = db.GetTableQuery<FactMEE>()
                    .Where(p => p.MedicalEventId == medicalEventId &&
                        (p.Source == 1 || p.Source == 3))
                    .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;

                decimal eqma = db.GetTableQuery<FactEQMA>()
                    .Where(p => p.MedicalEventId == medicalEventId &&
                        (p.Source == 1 || p.Source == 3))
                    .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;

                //считаем принятую к оплате сумму и статус оплаты
                decimal price = mevent.MoPrice ?? 0;
                int moPaymentStatus = 2;
                int paymentStatus = 2;
                decimal? acceptPrice = mevent.MoPrice - (mec + mee + eqma);
                if (acceptPrice == 0)
                {
                    moPaymentStatus = 3;
                    paymentStatus = 3;
                }
                else if (acceptPrice < mevent.MoPrice)
                {
                    moPaymentStatus = 4;
                }

                //обновляем запись случая
                db.GetTableQuery<FactMedicalEvent>().Where(s => s.MedicalEventId == medicalEventId)
                   .Set(s => s.MoPaymentStatus, moPaymentStatus)
                   .Set(s => s.PaymentStatus, paymentStatus)
                   .Set(s => s.Price, acceptPrice)
                   .Set(s => s.AcceptPrice, acceptPrice)
                   .Update();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult UpdateZMedicalEventAsLocal(MedicineContext db, int? medicalEventId)
        {
            var result = new TransactionResult();

            try
            {
                var mevent = db.GetTableQuery<ZFactMedicalEvent>().FirstOrDefault(p => p.ZmedicalEventId == medicalEventId);
                if (mevent == null)
                {
                    throw new ArgumentException("MedicalEvent ID {0} не найден".F(medicalEventId));
                }

                //Получаем все МЭКи не блокированные и с источником 'ТФОМС1 к МО' или 'Уточнённые санкции ТФОМС1 к МО' (на случай если будет исправленная часть у МО)
                var mecList = db.GetTableQuery<ZFactSank>()
                    .Distinct()
                    .Where(p => p.ZmedicalEventId == medicalEventId &&
                        (p.IsLock == null || p.IsLock == false) &&
                        p.Type == 1 &&
                        (p.Source == (int)RefusalSource.Local || p.Source == (int)RefusalSource.LocalCorrected)).ToList();

                decimal mec = mecList.Distinct(p => p.ZmedicalEventId).Sum(p => p.Amount) ?? 0;

                decimal mee = db.GetTableQuery<ZFactSank>()
                    .Where(p => p.ZmedicalEventId == medicalEventId &&
                        (p.Source == 1 || p.Source == 3) && 
                        Constants.Mee.Contains(p.Type))
                    .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;

                decimal eqma = db.GetTableQuery<ZFactSank>()
                    .Where(p => p.ZmedicalEventId == medicalEventId &&
                        (p.Source == 1 || p.Source == 3) &&
                        Constants.Eqma.Contains(p.Type))
                    .Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;
                
                //считаем принятую к оплате сумму и статус оплаты
                decimal price = mevent.MoPrice ?? 0;
                int moPaymentStatus = 2;
                int paymentStatus = 2;
                decimal? acceptPrice = mevent.MoPrice - (mec + mee + eqma);
                //if (acceptPrice == 0)
                //{
                //    moPaymentStatus = 3;
                //    paymentStatus = 3;
                //}
                //else if (acceptPrice < mevent.MoPrice)
                //{
                //    moPaymentStatus = 4;
                //}

                //обновляем запись случая
                db.GetTableQuery<ZFactMedicalEvent>().Where(s => s.ZmedicalEventId == medicalEventId)
                   .Set(s => s.MEC, Math.Min((mec), price))
                   .Set(s => s.MEE, Math.Min((mee), price))
                   .Set(s => s.EQMA, Math.Min((eqma), price))
                   .Set(s => s.AcceptPrice, acceptPrice)
                   .Update();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult UpdateZslMedicalEventAsLocal(MedicineContext db, int? zslMedicalEventId)
        {
            var result = new TransactionResult();

            try
            {
                var zslMevent = db.GetTableQuery<ZslFactMedicalEvent>().FirstOrDefault(p => p.ZslMedicalEventId == zslMedicalEventId);
                if (zslMevent == null)
                {
                    throw new ArgumentException("ZslMedicalEvent ID {0} не найден".F(zslMedicalEventId));
                }

                var mevent = db.GetTableQuery<ZFactMedicalEvent>().Where(p => p.ZslFactMedicalEvent.ZslMedicalEventId == zslMedicalEventId).ToList();

                if (!mevent.Any())
                {
                    throw new ArgumentException("Не найдено ни одного посещения в обращении ID {0}".F(zslMedicalEventId));
                }

                decimal? acceptPrice = null;
                decimal? refusalPrice = null;
                bool mecFlag = false;
                bool mee_eqmp = false;
                foreach (var zFactMedicalEvent in mevent)
                {
                    var id = zFactMedicalEvent.ZmedicalEventId;
                    //Получаем все МЭКи не блокированные и с источником 'ТФОМС1 к МО' или 'Уточнённые санкции ТФОМС1 к МО' (на случай если будет исправленная часть у МО)
                    var allSankList = db.GetTableQuery<ZFactSank>()
                       .Distinct()
                       .Where(p => p.ZmedicalEventId == id &&
                           (p.IsLock == null || p.IsLock == false) &&
                           (p.Source == (int)RefusalSource.Local || p.Source == (int)RefusalSource.LocalCorrected)).ToList();

                    if (allSankList.Where(x => x.Type == 1).Any())
                    {
                        mecFlag = true;
                    }
                    if (allSankList.Any(x => Constants.Mee.Contains(x.Type)) || allSankList.Any(x => Constants.Eqma.Contains(x.Type)))
                    {
                        mee_eqmp = true;
                        decimal mee = allSankList.Where(p => Constants.Mee.Contains(p.Type)).Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;
                        decimal eqma = allSankList.Where(p => Constants.Eqma.Contains(p.Type)).Sum(p => p.Amount + (p.Penalty ?? 0)) ?? 0;

                        refusalPrice = refusalPrice ?? 0 + mee + eqma;
                    }

                    acceptPrice = acceptPrice ?? 0 + zFactMedicalEvent?.AcceptPrice;
                }
                
                if (mecFlag)
                {
                    acceptPrice = 0;
                    refusalPrice = zslMevent.Price;
                }
                else
                {
                    if (!mee_eqmp)
                    {
                        acceptPrice = zslMevent.Price;
                        refusalPrice = 0;
                    }
                }

                int moPaymentStatus = 2;
                int paymentStatus = 2;
                if (acceptPrice == 0)
                {
                    moPaymentStatus = 3;
                    paymentStatus = 3;
                }
                else if (acceptPrice < mevent.Sum(x => x.EventPrice))
                {
                    moPaymentStatus = 4;
                    paymentStatus = 4;
                }

                //обновляем запись случая
                db.GetTableQuery<ZslFactMedicalEvent>().Where(s => s.ZslMedicalEventId == zslMevent.ZslMedicalEventId)
                    .Set(s => s.MoPaymentStatus, moPaymentStatus)
                    .Set(s => s.PaymentStatus, paymentStatus)
                    .Set(s => s.AcceptPrice, acceptPrice)
                    .Set(s => s.RefusalPrice, refusalPrice)
                    .Update();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult InsertOrUpdateMedicalAccount(FactMedicalAccount account)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (account.MedicalAccountId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(account);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException("Добавление счета произошло с ошибкой.\r\nЗапрос:{0}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(account);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Обновление счета ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(account.MedicalAccountId, db.LastQuery));
                        }
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult InsertOrUpdateActExpertise(FactActExpertise act)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (act.ActExpertiseId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(act);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException("Добавление акта произошло с ошибкой.\r\nЗапрос:{0}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(act);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Обновление акта ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(act.ActExpertiseId, db.LastQuery));
                        }
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult InsertOrUpdateLocalMec(FactMEC data, UpdateFlag flag, int? accountId = null)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.MECId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException("Добавление МЭК произошло с ошибкой.\r\nЗапрос:{0}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Обновление МЭК ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.MECId,db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.MedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateMedicalEventAsLocal(db, data.MedicalEventId); 
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.MedicalEventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.Account))
                    {
                        var updateAccountResult = UpdateMedicalAccount(db, accountId);
                        if (updateAccountResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Счета ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(accountId, db.LastQuery));
                        }
                    }


                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }

            return result;
        }


        public TransactionResult InsertOrUpdateSlKOef(ZFactSlKoef data)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.ZslKoefId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException(
                                "При добавлении slkoef произошла ошибка.\r\nЗапрос:{0}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException(
                                "Обновление slkoef id = {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.ZslKoefId, db.LastQuery));
                        }
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult InsertOrUpdateCrit(ZFactCrit data)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.ZCritId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException(
                                "При добавлении Crit произошла ошибка.\r\nЗапрос:{0}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException(
                                "Обновление Crit id = {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.ZCritId, db.LastQuery));
                        }
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult InsertOrUpdateZDiagBlok(ZFactDiagBlok data)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.ZDiagBlokId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException(
                                "При добавлении DiagBlok произошла ошибка.\r\nЗапрос:{0}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException(
                                "Обновление DiagBlok id = {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.ZDiagBlokId, db.LastQuery));
                        }
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult InsertOrUpdateZDs(ZFactDs data)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.ZFactDsId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException(
                                "При добавлении ZFactDs произошла ошибка.\r\nЗапрос:{0}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException(
                                "Обновление ZFactDs id = {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.ZFactDsId, db.LastQuery));
                        }
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult InsertOrUpdateZDirection(ZFactDirection data)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.ZDirectionId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException(
                                "При добавлении Direction произошла ошибка.\r\nЗапрос:{0}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException(
                                "Обновление Direction id = {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.ZDirectionId, db.LastQuery));
                        }
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult InsertOrUpdateZAnticancerDrug(ZFactAnticancerDrug data)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.ZAnticancerDrugId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException(
                                "При добавлении AnticancerDrug произошла ошибка.\r\nЗапрос:{0}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException(
                                "Обновление AnticancerDrug id = {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.ZAnticancerDrugId, db.LastQuery));
                        }
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult InsertOrUpdateZContraindications(ZFactContraindications data)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.ZContraindicationsId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException(
                                "При добавлении Сontraindications произошла ошибка.\r\nЗапрос:{0}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException(
                                "Обновление Сontraindications id = {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.ZContraindicationsId, db.LastQuery));
                        }
                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult InsertOrUpdateLocalSank(ZFactSank data, UpdateFlag flag, int zslMedicalEventId, int? accountId = null, int? typeRefusal = null)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.ZSankId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException("Добавление {1} произошло с ошибкой.\r\nЗапрос:{0}".F(db.LastQuery, typeRefusal == null ? "МЭК": RefusalType.MEE.GetDisplayShortName()));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Обновление {2} ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.ZSankId, db.LastQuery, typeRefusal == null ? "МЭК" : RefusalType.MEE.GetDisplayShortName()));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.MedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateZMedicalEventAsLocal(db, data.ZmedicalEventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.ZmedicalEventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.ZslMedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateZslMedicalEventAsLocal(db, zslMedicalEventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление законченного случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(zslMedicalEventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.Account))
                    {
                        var updateAccountResult = UpdateZMedicalAccount(db, accountId);
                        if (updateAccountResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Счета ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(accountId, db.LastQuery));
                        }
                    }


                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }

            return result;
        }



        public TransactionResult InsertOrUpdateLocalMee(FactMEE data, UpdateFlag flag, int? accountId = null)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.MEEId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException("Добавление МЭЭ произошло с ошибкой.\r\nЗапрос:{1}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Обновление МЭЭ ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.MEEId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.MedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateMedicalEventAsLocal(db, data.MedicalEventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.MedicalEventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.Account))
                    {
                        var updateAccountResult = UpdateMedicalAccount(db, accountId);
                        if (updateAccountResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Счета ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(accountId, db.LastQuery));
                        }
                    }


                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult InsertOrUpdateLocalEqma(FactEQMA data, UpdateFlag flag, int? accountId = null)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    if (data.EQMAId == 0)
                    {
                        var insertResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertResult))
                        {
                            throw new InvalidOperationException("Добавление ЭКМП произошло с ошибкой.\r\nЗапрос:{1}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        var updateResult = db.Update(data);
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Обновление ЭКМП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.EQMAId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.MedicalEvent))
                    {
                        var updateMedicalEventResult = UpdateMedicalEventAsLocal(db, data.MedicalEventId);
                        if (updateMedicalEventResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Случая МП ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(data.MedicalEventId, db.LastQuery));
                        }
                    }

                    if (flag.HasFlag(UpdateFlag.Account))
                    {
                        var updateAccountResult = UpdateMedicalAccount(db, accountId);
                        if (updateAccountResult.HasError)
                        {
                            throw new InvalidOperationException("Обновление Счета ID {0} произошло с ошибкой.\r\nЗапрос:{1}".F(accountId, db.LastQuery));
                        }
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<IEnumerable<localUser>> GetUsers()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<localUser>>();
                try
                {
                    //TODO убрать из ответа пароль и соль (хотя чего с ними сделают то??? :) и так доступ к БД есть...
                    result.Data = db.localUser.Select(p=>p).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<localRole>> GetRoles()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<localRole>>();
                try
                {
                    result.Data = db.localRole.ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult UpdatePatientPolicyById(int? patientId, string inp, int? territoryOkato)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    var updateResult = db.GetTableQuery<FactPatient>()
                        .Where(p => p.PatientId == patientId)
                        .Set(p => p.INP, inp)
                        .Set(p => p.TerritoryOkato, territoryOkato)
                        .Update();
                    if (!Convert.ToBoolean(updateResult))
                    {
                        result.AddError(new InvalidOperationException("Ошибка обновления данных пациента {0}".F(db.LastQuery)));
                    }
                }
                catch(Exception exception)
                {
                    result.AddError(exception);
                }
                
            }
            return result;
        }

        public TransactionResult AddLocalMecToScope(FactMEC localMec, int? id, int? scope, decimal? percent)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    List<FactMedicalEvent> events;
                    switch (scope)
                    {
                        case 1:
                            events = db.GetTableQuery<FactMedicalEvent>()
                                .Where(p => p.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == id)
                                .ToList();
                            break;
                        case 3:
                            events = db.GetTableQuery<FactMedicalEvent>()
                                .Where(p => p.FACTMEDIPATIENTIDFACTPATI.PatientId == id)
                                .ToList();
                            break;
                        case 4:
                            events = db.GetTableQuery<FactMedicalEvent>()
                                .Where(p => p.MedicalEventId == id)
                                .ToList();
                            break;
                        case 5:
                            events = db.GetTableQuery<FactMedicalServices>()
                                .Where(p => p.MedicalServicesId == id)
                                .Select(p => p.FACTMEDIFMSMEIDFACTMEDI)
                                .ToList();
                            break;
                        default:
                            throw new ArgumentException("Неверная область ошибок для добавления отказа ID {0} ErrorScope {1}".F(id, scope));
                    }

                    foreach (var medicalEvent in events)
                    {
                        var mec = Map.ObjectToObject<FactMEC>(localMec);
                        mec.ExternalGuid = Guid.NewGuid().ToString();
                        mec.Date = DateTime.Now;
                        mec.MedicalEventId = medicalEvent.MedicalEventId;
                        mec.PatientId = medicalEvent.PatientId;
                        mec.Amount = medicalEvent.MoPrice*(percent ?? 1m);

                        var insertResult = InsertOrUpdateLocalMec(mec, UpdateFlag.MedicalEvent);
                        if (insertResult.HasError)
                        {
                            throw new InvalidOperationException("Ошибка добавления отказа случаю МП {0}, {1}".F(medicalEvent.MedicalEventId, db.LastQuery));
                        }
                    }

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }

            }
            return result;
        }

        public TransactionResult AddLocalMecToScope(ZFactSank localMec, int? id, int? scope, decimal? percent)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    List<ZFactMedicalEvent> events;
                    switch (scope)
                    {
                        case 1:
                            events = db.GetTableQuery<ZFactMedicalEvent>()
                                .Where(p => p.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == id)
                                .ToList();
                            break;
                        case 3:
                            events = db.GetTableQuery<ZFactMedicalEvent>()
                                .Where(p => p.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.PatientId == id)
                                .ToList();
                            break;
                        case 4:
                            events = db.GetTableQuery<ZFactMedicalEvent>()
                                .Where(p => p.ZmedicalEventId == id)
                                .ToList();
                            break;
                        case 5:
                            events = db.GetTableQuery<ZFactMedicalServices>()
                                .Where(p => p.ZmedicalServicesId == id)
                                .Select(p => p.ZFactMedicalEvent)
                                .ToList();
                            break;
                        case 9:
                            events = db.ZFactMedicalEvent
                                .Where(p => p.ZslFactMedicalEvent.ZslMedicalEventId == id).ToList();
                            break;
                        default:
                            throw new ArgumentException("Неверная область ошибок для добавления отказа ID {0} ErrorScope {1}".F(id, scope));
                    }

                    foreach (var medicalEvent in events)
                    {
                       
                        var zslMedicalEventId = medicalEvent.ZslMedicalEventId;
                        var zslMedicalEvent = db.ZslFactMedicalEvent.First(x => x.ZslMedicalEventId == zslMedicalEventId);
                        var mec = Map.ObjectToObject<ZFactSank>(localMec);
                        mec.ZslMedicalEventId = zslMedicalEventId;
                        mec.ExternalGuid = Guid.NewGuid().ToString();
                        mec.Date = DateTime.Now;
                        mec.ZmedicalEventId = medicalEvent.ZmedicalEventId;
                        mec.PatientId = zslMedicalEvent.PatientId;
                        mec.Amount = medicalEvent.MoPrice * (percent ?? 1m);
                        mec.ZslAmount = zslMedicalEvent.Price;

                        var insertResult = InsertOrUpdateLocalSank(mec, UpdateFlag.ZslMedicalEvent|UpdateFlag.MedicalEvent, zslMedicalEventId,null, (int)RefusalType.MEC);
                        if (insertResult.HasError)
                        {
                            throw new InvalidOperationException("Ошибка добавления отказа случаю МП {0}, {1}".F(medicalEvent.ZmedicalEventId, db.LastQuery));
                        }
                    }

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }

            }
            return result;
        }

        public TransactionResult AddTerritoryMecToScope(FactMEC localMec, int? id, int? scope, decimal? percent)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    List<FactMedicalEvent> events;
                    switch (scope)
                    {
                        case 2:
                            events = db.GetTableQuery<FactMedicalEvent>()
                                .Where(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == id)
                                .ToList();
                            break;
                        case 3:
                            events = db.GetTableQuery<FactMedicalEvent>()
                                .Where(p => p.FACTMEDIPATIENTIDFACTPATI.PatientId == id)
                                .ToList();
                            break;
                        case 4:
                            events = db.GetTableQuery<FactMedicalEvent>()
                                .Where(p => p.MedicalEventId == id)
                                .ToList();
                            break;
                        case 5:
                            events = db.GetTableQuery<FactMedicalServices>()
                                .Where(p => p.MedicalServicesId == id)
                                .Select(p => p.FACTMEDIFMSMEIDFACTMEDI)
                                .ToList();
                            break;
                        default:
                            throw new ArgumentException("Неверная область ошибок для добавления отказа ID {0} ErrorScope {1}".F(id, scope));
                    }

                    foreach (var medicalEvent in events)
                    {
                        var mec = Map.ObjectToObject<FactMEC>(localMec);
                        mec.ExternalGuid = Guid.NewGuid().ToString();
                        mec.Date = DateTime.Now;
                        mec.MedicalEventId = medicalEvent.MedicalEventId;
                        mec.PatientId = medicalEvent.PatientId;
                        mec.Amount = medicalEvent.Price * (percent ?? 1m);

                        var insertResult = InsertOrUpdateTerritorialMec(mec, UpdateFlag.MedicalEvent);
                        if (insertResult.HasError)
                        {
                            throw new InvalidOperationException("Ошибка добавления отказа случаю МП {0}, {1}".F(medicalEvent.MedicalEventId, db.LastQuery));
                        }
                    }

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }

            }
            return result;
        }

        public TransactionResult AddTerritoryMecToScope(ZFactSank localMec, int? id, int? scope, decimal? percent)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    List<ZFactMedicalEvent> events;
                    IQueryable<ZFactMedicalEvent> qEvents;
                    switch (scope)
                    {
                        case 2:
                            qEvents = db.ZFactMedicalEvent
                                .Where(p => p.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id)
                                ;
                            break;
                        case 3:
                            qEvents = db.ZFactMedicalEvent
                                .Where(p => p.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.PatientId == id);
                            break;
                        case 4:
                            qEvents = db.ZFactMedicalEvent
                                .Where(p => p.ZmedicalEventId == id)
                                ;
                            break;
                        case 5:
                            qEvents = db.ZFactMedicalServices
                                .Where(p => p.ZmedicalServicesId == id)
                                .Select(p => p.ZFactMedicalEvent)
                                ;
                            break;
                        case 9:
                            qEvents = db.ZFactMedicalEvent
                                .Where(p => p.ZslFactMedicalEvent.ZslMedicalEventId == id)
                                ;
                            break;
                        default:
                            throw new ArgumentException("Неверная область ошибок для добавления отказа ID {0} ErrorScope {1}".F(id, scope));
                    }

                    events = qEvents.ToList();

                    foreach (var medicalEvent in events)
                    {
                        var zslMedicalEventId = medicalEvent.ZslMedicalEventId;
                        var zslMedicalEvent = db.ZslFactMedicalEvent.First(x => x.ZslMedicalEventId == zslMedicalEventId);
                        var mec = Map.ObjectToObject<ZFactSank>(localMec);
                        mec.ZslMedicalEventId = zslMedicalEventId;
                        mec.ExternalGuid = Guid.NewGuid().ToString();
                        mec.Date = DateTime.Now;
                        mec.ZmedicalEventId = medicalEvent.ZmedicalEventId;
                        mec.PatientId = zslMedicalEvent.PatientId;
                        mec.Amount = medicalEvent.EventPrice * (percent ?? 1m);
                        mec.ZslAmount = zslMedicalEvent.Price;

                        var insertResult = InsertOrUpdateTerritorialSank(mec, UpdateFlag.MedicalEvent|UpdateFlag.ZslMedicalEvent, zslMedicalEventId);
                        if (insertResult.HasError)
                        {
                            throw new InvalidOperationException("Ошибка добавления отказа случаю МП {0}, {1}".F(medicalEvent.ZmedicalEventId, db.LastQuery));
                        }
                    }

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }

            }
            return result;
        }

        public TransactionResult SplitPatientsWithFullRefusal(int year, int month)
        {
            var result = new TransactionResult();

            using (var db = CreateContext())
            {
                try
                {
                    var patients = db.FactPatient
                        .Where(p => p.AccountId == null &&
                                    p.FACTPATIREGISTERIFACTREGI.Date.Value.Year == year &&
                                    p.FACTPATIREGISTERIFACTREGI.Date.Value.Month == month &&
                                    p.TerritoryOkato.HasValue && p.TerritoryOkato != TerritoryService.TfCode &&
                                    p.FACTMEDIPATIENTIDFACTPATIs.Any(s => s.AcceptPrice != null && s.AcceptPrice > 0)).ToList();

                    var patientsWithoutZero = db.FactPatient
                        .Where(p => p.AccountId == null &&
                                    p.FACTPATIREGISTERIFACTREGI.Date.Value.Year == year &&
                                    p.FACTPATIREGISTERIFACTREGI.Date.Value.Month == month &&
                                    p.TerritoryOkato.HasValue && p.TerritoryOkato != TerritoryService.TfCode &&
                                    p.FACTMEDIPATIENTIDFACTPATIs.All(s => s.AcceptPrice != null && s.AcceptPrice > 0))
                        .Select(p=>p.PatientId)
                        .ToList();

                    if (patients.Count == patientsWithoutZero.Count)
                    {
                        //Повезло...
                        return result;
                    }

                    foreach (var samePatient in patients.GroupBy(p => p.PersonalId))
                    {
                        if (samePatient.All(p => patientsWithoutZero.Contains(p.PatientId)))
                        {
                            continue;
                        }
                        var zeroPatient = samePatient.LastOrDefault();
                        var zeroPatientId = zeroPatient.PatientId;
                        var patient = samePatient.FirstOrDefault();
                        var patientId = patient.PatientId;
                        var count = samePatient.Count();
                        switch (count)
                        {
                            case 1:
                                 var copyResult = CopyPatient(patientId);
                                if (copyResult.Success)
                                {
                                    zeroPatientId = copyResult.Id;
                                }
                                else
                                {
                                    throw copyResult.LastError;
                                }
                                break;
                            case 2:
                                //Все уже сделано
                                break;
                            default:
                                //TODO Сделать слияние
                                throw new InvalidOperationException("Пациентов с одинаковым PersonID больше 2х, необходимо слияние, PersonID {0}".F(samePatient.Key));
                        }
                        
                        db.FactMedicalEvent
                            .Where(e => e.PatientId == zeroPatientId)
                            .Set(e => e.PatientId, patientId)
                            .Update();

                        db.FactMedicalEvent
                            .Where(e => e.PatientId == patientId && (e.AcceptPrice == null || e.AcceptPrice == 0))
                            .Set(e => e.PatientId, zeroPatientId)
                            .Update();
                        
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }
        

        public TransactionResult ZSplitPatientsWithFullRefusal(int year, int month)
        {
            var result = new TransactionResult();
           TerritoryService t = new TerritoryService();
            using (var db = CreateContext())
            {
                try
                {
                    var patients = db.FactPatient
                        .Where(p => p.AccountId == null &&
                                    p.FACTPATIREGISTERIFACTREGI.Date.Value.Year == year &&
                                    p.FACTPATIREGISTERIFACTREGI.Date.Value.Month == month &&
                                    p.TerritoryOkato.HasValue && p.TerritoryOkato != TerritoryService.TfCode &&
                                    p.ZSLFACTMEDIPATIENTIDFACTPATIs.Any(s => s.AcceptPrice != null && s.AcceptPrice > 0)).ToList();

                    var patientsWithoutZero = db.FactPatient
                        .Where(p => p.AccountId == null &&
                                    p.FACTPATIREGISTERIFACTREGI.Date.Value.Year == year &&
                                    p.FACTPATIREGISTERIFACTREGI.Date.Value.Month == month &&
                                    p.TerritoryOkato.HasValue && p.TerritoryOkato != TerritoryService.TfCode &&
                                    p.ZSLFACTMEDIPATIENTIDFACTPATIs.All(s => s.AcceptPrice != null && s.AcceptPrice > 0))
                        .Select(p => p.PatientId)
                        .ToList();

                    if (patients.Count == patientsWithoutZero.Count)
                    {
                        //Повезло...
                        return result;
                    }

                    foreach (var samePatient in patients.GroupBy(p => p.PersonalId))
                    {
                        if (samePatient.All(p => patientsWithoutZero.Contains(p.PatientId)))
                        {
                            continue;
                        }
                        var zeroPatient = samePatient.LastOrDefault();
                        var zeroPatientId = zeroPatient.PatientId;
                        var patient = samePatient.FirstOrDefault();
                        var patientId = patient.PatientId;
                        var count = samePatient.Count();
                        switch (count)
                        {
                            case 1:
                                var copyResult = CopyPatient(patientId);
                                if (copyResult.Success)
                                {
                                    zeroPatientId = copyResult.Id;
                                }
                                else
                                {
                                    throw copyResult.LastError;
                                }
                                break;
                            case 2:
                                //Все уже сделано
                                break;
                            default:
                                //TODO Сделать слияние
                                throw new InvalidOperationException("Пациентов с одинаковым PersonID больше 2х, необходимо слияние, PersonID {0}".F(samePatient.Key));
                        }

                        db.ZslFactMedicalEvent
                            .Where(e => e.PatientId == zeroPatientId)
                            .Set(e => e.PatientId, patientId)
                            .Update();

                        db.ZslFactMedicalEvent
                            .Where(e => e.PatientId == patientId && (e.AcceptPrice == null || e.AcceptPrice == 0))
                            .Set(e => e.PatientId, zeroPatientId)
                            .Update();

                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<bool> IsExchangeExistsForTerritoryAccount(int accountId)
        {
            var result = new TransactionResult<bool>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactExchange.Any(p => p.AccountId == accountId && Constants.InterTerritorialAccountTypes.Contains(p.Type));
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        

        public TransactionResult<FactPatient> CopyPatient(int patientId)
        {
            TransactionResult<FactPatient> result;
            using (var db = CreateContext())
            {
                result = CopyPatient(db, patientId);
            }
            return result;
        }
        public TransactionResult<FactPatient> CopyPatient(MedicineContext db, int patientId)
        {
            var result = new TransactionResult<FactPatient>();
            try
            {
                var patient = FactPatientQuery(db, patientId).FirstOrDefault();
                var copy = Map.ObjectToObject<FactPatient>(patient);
                copy.PatientId = 0;
                var insertResult = db.InsertWithIdentity(copy);
                if (!Convert.ToBoolean(insertResult))
                {
                    throw new InvalidOperationException("Ошибка создания копии пациента ID {0}, {1}".F(patientId, db.LastQuery));
                }
                result.Id = Convert.ToInt32(insertResult);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult<bool> IsEconomicSurchargeByAccountIdExists(int? id)
        {
            var result = new TransactionResult<bool>();
            if (id.HasValue)
            {
                using (var db = CreateContext())
                {
                    try
                    {
                        var data = EconomicSurchargeByAccountIdQuery(db, id.Value).ToList();
                        result.Data = data.Count > 0;
                    }
                    catch (Exception exception)
                    {
                        result.AddError(exception);
                    }
                }
            }

            return result;
        }

        public TransactionResult<IEnumerable<globalAccountType>> GetGlobalAccountType()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<globalAccountType>>();
                try
                {
                    result.Data = GlobalAccountTypeQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        //ashurkova begin
        public TransactionResult<IEnumerable<FactEconomicAccount>> GetEconomicAccountByAccountId(int id)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactEconomicAccount>>();
                try
                {
                    result.Data = EconomicAccountByAccountIdQuery(db, id);
                    if (result.Data == null)
                    {
                        result.AddError(new Exception(CoreMessages.DataNotFound.F(typeof(FactEconomicAccount), id)));
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<EconomicPartner>> GetEconomicPartner(string okato)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<EconomicPartner>>();
                try
                {
                    result.Data = EconomicPartnerQuery(db, okato);
                    if (result.Data == null)
                    {
                        result.AddError(new Exception(CoreMessages.DataNotFound.F(typeof(EconomicPartner))));
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<int?> GetKolPaySluch(int accountId, int?[] assistanceConditions)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int?>();
                try
                {
                    result.Data = db.GetTableQuery<FactMedicalEvent>()
                        .Count(
                            p =>
                                p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId && p.PaymentStatus != 3 &&
                                assistanceConditions.Contains(p.AssistanceConditions));
                    if (result.Data == null)
                    {
                        result.AddError(new Exception(CoreMessages.DataNotFound.F(typeof (EconomicPartner))));
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<int?> GetKolPayZSluch(int accountId, int?[] assistanceConditions)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int?>();
                try
                {
                    result.Data = db.GetTableQuery<ZslFactMedicalEvent>()
                        .Count(
                            p =>
                                p.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == accountId && p.PaymentStatus != 3 &&
                                assistanceConditions.Contains(p.AssistanceConditions));
                    if (result.Data == null)
                    {
                        result.AddError(new Exception(CoreMessages.DataNotFound.F(typeof(EconomicPartner))));
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }



        //ashurkova end



        public TransactionResult<int> GetTerritoryAccountVersionById(int id)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int>();
                try
                {
                    var resultData = TerritoryAccountByIdQuery(db, id).FirstOrDefault();
                    if (resultData == null)
                    {
                        result.AddError(new Exception(CoreMessages.DataNotFound.F(typeof (FactTerritoryAccount), id)));
                    }
                    else
                    {
                        result.Data = resultData.Version != null? (int)resultData.Version : Constants.Version21;
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }
        public TransactionResult<FactTerritoryAccount> GetTerritoryAccountById(int id)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<FactTerritoryAccount>();
                try
                {
                    result.Data = TerritoryAccountByIdQuery(db, id).FirstOrDefault();
                    if ( result.Data == null )
                    {
                        result.AddError(new Exception(CoreMessages.DataNotFound.F(typeof(FactTerritoryAccount),id)));
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<localSettings>> GetLocalSettings()
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<localSettings>>();
                try
                {
                    result.Data = LocalSettingsQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult UpdateLocalSettings(string key, string value)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult();
                try
                {
                    var exist = db.localSettings.FirstOrDefault(p => p.Key == key);
                    if (exist != null)
                    {
                        result.Id = db.localSettings
                        .Where(e => e.Key == exist.Key)
                        .Set(e => e.Value, value)
                        .Update();
                    }
                    else
                    {
                        db.InsertWithIdentity(new localSettings
                        {
                            Key = key,
                            Value = value
                        });
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult UpdateLocalSettingsMetadata(string key, string metadata)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult();
                try
                {
                    var exist = db.localSettings.FirstOrDefault(p => p.Key == key);
                    if (exist != null)
                    {
                        result.Id = db.localSettings
                        .Where(e => e.Key == exist.Key)
                        .Set(e => e.Metadata, metadata)
                        .Update();
                    }
                    else
                    {
                        db.InsertWithIdentity(new localSettings
                        {
                            Key = key,
                            Metadata = metadata
                        });
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult UpdateUserSettings(string key, object value)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult();
                try
                {
                    var exist = db.localUserSettings.FirstOrDefault(p => p.Key == key);
                    if (exist != null)
                    {
                        result.Id = db.localSettings
                        .Where(e => e.Key == exist.Key)
                        .Set(e => e.Value, value)
                        .Update();
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<FactPerson>> GetPerson(Expression<Func<FactPerson, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactPerson>>();
                try
                {
                    result.Data = predicate == null ?
                        PersonQuery(db).ToList() :
                        db.FactPerson.Where(predicate).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<FactDocument>> GetDocument(Expression<Func<FactDocument, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactDocument>>();
                try
                {
                    result.Data = predicate == null ?
                        DocumentQuery(db).ToList() :
                        db.FactDocument.Where(predicate).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult UpdateMedicalAccountById(int id, decimal? price, decimal? acceptPrice, decimal? mecDb, decimal? meeDb,
            decimal? eqmaDb)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    var updateResult = db.FactMedicalAccount.Where(s => s.MedicalAccountId == id)
                           .Set(s => s.Price, price)
                           .Set(s => s.AcceptPrice, acceptPrice)
                           .Set(s => s.MECPenalties, mecDb)
                           .Set(s => s.MEEPenalties, meeDb)
                           .Set(s => s.EQMAPenalties, eqmaDb)
                       .Update();
                    if (updateResult == 0)
                    {
                        result.AddError(new Exception("Ошибка обновления счета МО ID {0}".F(id)));
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
            
        }

        public TransactionResult InsertRegisterD(FactMedicalAccount account, List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactSank>>>>>>> data, bool isTestLoad)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                db.Command.CommandTimeout = 0;
                db.BeginTransaction();
                try
                {
                    var accountId = SafeConvert.ToInt32(db.InsertWithIdentity(account).ToString(), false);
                    if (!accountId.HasValue)
                    {
                        result.AddError(string.Format("Ошибка при записи счета МО в базу данных"));
                        db.RollbackTransaction();
                        return result;
                    }

                    result.Id = accountId.Value;

                    foreach (var p in data)
                    {
                        var patient = p.Item1;
                        var person = p.Item2;
                        var document = p.Item3;

                        int? personId = default(int?);
                        int? documentId = default(int?);

                        if (person.IsNotNull())
                        {
                            var insertPersonResult = db.InsertWithIdentity(person);
                            if (Convert.ToInt32(insertPersonResult) == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи персональных данных пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            personId = Convert.ToInt32(insertPersonResult);
                        }

                        if (document.IsNotNull())
                        {
                            if (personId.HasValue)
                            {
                                document.PersonId = personId.Value;
                            }
                            else if (patient.PersonalId.HasValue)
                            {
                                document.PersonId = patient.PersonalId.Value;
                            }
                            else
                            {
                                result.AddError(string.Format("Отсутствует PersonalId у пациента ID {0}", patient.PatientId));
                                db.RollbackTransaction();
                                return result;
                            }

                            var insertDocumentResult = db.InsertWithIdentity(document);
                            if (Convert.ToInt32(insertDocumentResult) == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи данных УДЛ пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            documentId = Convert.ToInt32(insertDocumentResult);
                        }


                        patient.MedicalAccountId = accountId;
                        if (personId.HasValue)
                        {
                            patient.PersonalId = personId;
                        }

                        if (documentId.HasValue)
                        {
                            patient.DocumentId = documentId;
                        }

                        var insertPatientResult = db.InsertWithIdentity(patient);
                        var patientId = Convert.ToInt32(insertPatientResult);
                        if (patientId == 0)
                        {
                            result.AddError(string.Format("Ошибка при записи данных пациента в базу данных"));
                            db.RollbackTransaction();
                            return result;
                        }

                        var mevents = p.Item4;
                        foreach (var m in mevents)
                        {
                            var mevent = m.Item1;
                            mevent.PatientId = patientId;
                            var meventInsertResult = db.InsertWithIdentity(mevent);
                            var meventId = Convert.ToInt32(meventInsertResult);
                            if (meventId == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи законченного случая в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }

                            foreach (var sl in m.Item2)
                            {
                                var zFactmedicalEvent = sl.Item1;
                                zFactmedicalEvent.ZslMedicalEventId = meventId;
                                var zFactmedicalEventInsertResult = db.InsertWithIdentity(zFactmedicalEvent);
                                var zFactmedicalEventInsertResultId = Convert.ToInt32(zFactmedicalEventInsertResult);
                                if (zFactmedicalEventInsertResultId == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи случая в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }

                                var ksgKpg = sl.Item2.Item1;
                                if (ksgKpg != null)
                                {
                                    ksgKpg.ZmedicalEventId = zFactmedicalEventInsertResultId;
                                    var insertKsfKpgResult = db.InsertWithIdentity(ksgKpg);
                                    var zksgKpgId = Convert.ToInt32(insertKsfKpgResult);
                                    if (zksgKpgId == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи КСГ в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }

                                    var slKoefs = sl.Item2.Item2;
                                    if (slKoefs.Any())
                                    {
                                        slKoefs.ForEach(s => s.ZksgKpgId = zksgKpgId);
                                        var insertslKoefsResult = db.InsertBatch(slKoefs);
                                        if (Convert.ToInt32(insertslKoefsResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи Коэффициента сложности лечения пациента в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }
                                }


                                var service = sl.Item3;
                                if (service.Any())
                                {
                                    service.ForEach(s => s.ZmedicalEventId = zFactmedicalEventInsertResultId);
                                    var insertServicesResult = db.InsertBatch(service);
                                    if (Convert.ToInt32(insertServicesResult) == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи услуг в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }
                                }

                                var mecs = sl.Item4.ToList();
                                if (mecs.Any())
                                {
                                    mecs.ForEach(s =>
                                    {
                                        s.ZmedicalEventId = zFactmedicalEventInsertResultId;
                                        s.ZslMedicalEventId = meventId;
                                        s.ZslAmount = mevent.MoPrice;
                                        s.PatientId = patientId;
                                    });
                                    var insertMecsResult = db.InsertBatch(mecs);
                                    if (Convert.ToInt32(insertMecsResult) == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи санкций в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }
                                }
                            }
                        }

                    }

                    var updateResult = UpdateZMedicalAccount(db, accountId);
                    if (updateResult.HasError)
                    {
                        result.AddError(string.Format("Ошибка при обновлении счета от МО"));
                        db.RollbackTransaction();
                        return result;
                    }

                    if (isTestLoad)
                    {
                        db.RollbackTransaction();
                    }
                    else
                    {
                        db.CommitTransaction();
                    }
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }

            }
            return result;
        }

        public TransactionResult InsertRegisterD(FactMedicalAccount account, List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<FactMedicalEvent, List<FactMedicalServices>, List<FactMEC>, List<FactMEE>, List<FactEQMA>>>>> data, bool isTestLoad)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                db.Command.CommandTimeout = 0;
                db.BeginTransaction();
                try
                {
                    var accountId = SafeConvert.ToInt32(db.InsertWithIdentity(account).ToString(), false);
                    if (!accountId.HasValue)
                    {
                        result.AddError(string.Format("Ошибка при записи счета МО в базу данных"));
                        db.RollbackTransaction();
                        return result;
                    }

                    result.Id = accountId.Value;

                    foreach (var p in data)
                    {
                        var patient = p.Item1;
                        var person = p.Item2;
                        var document = p.Item3;

                        int? personId = default(int?);
                        int? documentId = default(int?);

                        if (person.IsNotNull())
                        {
                            var insertPersonResult = db.InsertWithIdentity(person);
                            if (Convert.ToInt32(insertPersonResult) == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи персональных данных пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            personId = Convert.ToInt32(insertPersonResult);
                        }
                        
                        if (document.IsNotNull())
                        {
                            if (personId.HasValue)
                            {
                                document.PersonId = personId.Value;
                            }
                            else if (patient.PersonalId.HasValue)
                            {
                                document.PersonId = patient.PersonalId.Value;
                            }
                            else
                            {
                                result.AddError(string.Format("Отсутствует PersonalId у пациента ID {0}", patient.PatientId));
                                db.RollbackTransaction();
                                return result;
                            }
                            
                            var insertDocumentResult = db.InsertWithIdentity(document);
                            if (Convert.ToInt32(insertDocumentResult) == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи данных УДЛ пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            documentId = Convert.ToInt32(insertDocumentResult);
                        }
                        
                        
                        patient.MedicalAccountId = accountId;
                        if (personId.HasValue)
                        {
                            patient.PersonalId = personId;
                        }

                        if (documentId.HasValue)
                        {
                            patient.DocumentId = documentId;
                        }
                        
                        var insertPatientResult = db.InsertWithIdentity(patient);
                        var patientId = Convert.ToInt32(insertPatientResult);
                        if (patientId == 0)
                        {
                            result.AddError(string.Format("Ошибка при записи данных пациента в базу данных"));
                            db.RollbackTransaction();
                            return result;
                        }

                        var mevents = p.Item4;
                        foreach (var m in mevents)
                        {
                            var mevent = m.Item1;
                            mevent.PatientId = patientId;
                            var meventInsertResult = db.InsertWithIdentity(mevent);
                            var meventId = Convert.ToInt32(meventInsertResult);
                            if (meventId == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи случая в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }

                            var services = m.Item2.ToList();
                            if (services.Any())
                            {
                                services.ForEach(s => s.MedicalEventId = meventId);
                                var insertServicesResult = db.InsertBatch(services);
                                if (Convert.ToInt32(insertServicesResult) == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи услуг в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }
                            }

                            var mecs = m.Item3.ToList();
                            if (mecs.Any())
                            {
                                mecs.ForEach(s =>
                                {
                                    s.MedicalEventId = meventId;
                                    s.PatientId = patientId;
                                });
                                var insertMecsResult = db.InsertBatch(mecs);
                                if (Convert.ToInt32(insertMecsResult) == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи МЭК в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }
                            }


                            var mees = m.Item4;
                            if (mees.Any())
                            {
                                mees.ForEach(s =>
                                {
                                    s.MedicalEventId = meventId;
                                    s.PatientId = patientId;
                                });
                                var insertMeesResult = db.InsertBatch(mees);
                                if (Convert.ToInt32(insertMeesResult) == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи МЭЭ в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }
                            }


                            var eqmas = m.Item5;
                            if (eqmas.Any())
                            {
                                eqmas.ForEach(s =>
                                {
                                    s.MedicalEventId = meventId;
                                    s.PatientId = patientId;
                                });
                                var insertEqmasResult = db.InsertBatch(eqmas);
                                if (Convert.ToInt32(insertEqmasResult) == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи ЭКМП в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }
                            }
                        }

                    }

                    var updateResult = UpdateMedicalAccount(db, accountId);
                    if (updateResult.HasError)
                    {
                        result.AddError(string.Format("Ошибка при обновлении счета от МО"));
                        db.RollbackTransaction();
                        return result;
                    }

                    if (isTestLoad)
                    {
                        db.RollbackTransaction();
                    }
                    else
                    {
                        db.CommitTransaction();
                    }
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
                
            }
            return result;
        }


        public TransactionResult InsertRegisterD(FactMedicalAccount account, List<Tuple<FactPerson, FactDocument, List<Tuple<FactPatient, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactSank>>>>>>>>> data, bool isTestLoad)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                db.Command.CommandTimeout = 0;
                db.BeginTransaction();
                try
                {
                    var accountId = SafeConvert.ToInt32(db.InsertWithIdentity(account).ToString(), false);
                    if (!accountId.HasValue)
                    {
                        result.AddError(string.Format("Ошибка при записи счета МО в базу данных"));
                        db.RollbackTransaction();
                        return result;
                    }

                    result.Id = accountId.Value;

                    foreach (var p in data)
                    {
                        //var patient = p.Item1;
                        var person = p.Item1;
                        var document = p.Item2;
                        var patMevents = p.Item3;

                        int? personId = default(int?);
                        int? documentId = default(int?);

                        if (person.IsNotNull())
                        {
                            var insertPersonResult = db.InsertWithIdentity(person);
                            if (Convert.ToInt32(insertPersonResult) == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи персональных данных пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            personId = Convert.ToInt32(insertPersonResult);
                        }

                        if (document.IsNotNull())
                        {
                            if (personId.HasValue)
                            {
                                document.PersonId = personId.Value;
                            }
                            else
                            {
                                result.AddError($"Отсутствует PersonalId у пациента ID { personId}");
                                db.RollbackTransaction();
                                return result;
                            }

                            var insertDocumentResult = db.InsertWithIdentity(document);
                            if (Convert.ToInt32(insertDocumentResult) == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи данных УДЛ пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            documentId = Convert.ToInt32(insertDocumentResult);
                        }

                       
                        foreach (var patMevent in patMevents)
                        {
                            var patient = patMevent.Item1;
                            patient.MedicalAccountId = accountId;
                            if (personId.HasValue)
                            {
                                patient.PersonalId = personId;
                            }

                            if (documentId.HasValue)
                            {
                                patient.DocumentId = documentId;
                            }

                            var insertPatientResult = db.InsertWithIdentity(patient);
                            var patientId = Convert.ToInt32(insertPatientResult);
                            if (patientId == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи данных пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }

                            var mevents = patMevent.Item2;
                            foreach (var m in mevents)
                            {
                                var mevent = m.Item1;
                                mevent.PatientId = patientId;
                                var meventInsertResult = db.InsertWithIdentity(mevent);
                                var meventId = Convert.ToInt32(meventInsertResult);
                                if (meventId == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи законченного случая в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }

                                foreach (var sl in m.Item2)
                                {
                                    var zFactmedicalEvent = sl.Item1;
                                    zFactmedicalEvent.ZslMedicalEventId = meventId;
                                    var zFactmedicalEventInsertResult = db.InsertWithIdentity(zFactmedicalEvent);
                                    var zFactmedicalEventInsertResultId = Convert.ToInt32(zFactmedicalEventInsertResult);
                                    if (zFactmedicalEventInsertResultId == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи случая в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }

                                    var ksgKpg = sl.Item2.Item1;
                                    if (ksgKpg != null)
                                    {
                                        ksgKpg.ZmedicalEventId = zFactmedicalEventInsertResultId;
                                        var insertKsfKpgResult = db.InsertWithIdentity(ksgKpg);
                                        var zksgKpgId = Convert.ToInt32(insertKsfKpgResult);
                                        if (zksgKpgId == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи КСГ в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }

                                        var slKoefs = sl.Item2.Item2;
                                        if (slKoefs.Any())
                                        {
                                            slKoefs.ForEach(s => s.ZksgKpgId = zksgKpgId);
                                            var insertslKoefsResult = db.InsertBatch(slKoefs);
                                            if (Convert.ToInt32(insertslKoefsResult) == 0)
                                            {
                                                result.AddError(string.Format("Ошибка при записи Коэффициента сложности лечения пациента в базу данных"));
                                                db.RollbackTransaction();
                                                return result;
                                            }
                                        }
                                    }


                                    var service = sl.Item3;
                                    if (service.Any())
                                    {
                                        service.ForEach(s => s.ZmedicalEventId = zFactmedicalEventInsertResultId);
                                        var insertServicesResult = db.InsertBatch(service);
                                        if (Convert.ToInt32(insertServicesResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи услуг в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }

                                    var mecs = sl.Item4.ToList();
                                    if (mecs.Any())
                                    {
                                        mecs.ForEach(s =>
                                        {
                                            s.ZmedicalEventId = zFactmedicalEventInsertResultId;
                                            s.ZslMedicalEventId = meventId;
                                            s.ZslAmount = mevent.MoPrice;
                                            s.PatientId = patientId;
                                        });
                                        var insertMecsResult = db.InsertBatch(mecs);
                                        if (Convert.ToInt32(insertMecsResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи санкций в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    var updateResult = UpdateZMedicalAccount(db, accountId);
                    if (updateResult.HasError)
                    {
                        result.AddError(string.Format("Ошибка при обновлении счета от МО"));
                        db.RollbackTransaction();
                        return result;
                    }

                    if (isTestLoad)
                    {
                        db.RollbackTransaction();
                    }
                    else
                    {
                        db.CommitTransaction();
                    }
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }

            }
            return result;
        }

        public TransactionResult InsertRegisterD(FactMedicalAccount account, List<Tuple<FactPerson, FactDocument, List<Tuple<FactPatient,
                    List<Tuple<ZslFactMedicalEvent,
                        List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            List<ZFactDs>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>,  List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
                        List<ZFactSank>
                            >>>>>> data, bool isTestLoad)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                db.Command.CommandTimeout = 0;
                db.BeginTransaction();
                try
                {
                    var accountId = SafeConvert.ToInt32(db.InsertWithIdentity(account).ToString(), false);
                    if (!accountId.HasValue)
                    {
                        result.AddError(string.Format("Ошибка при записи счета МО в базу данных"));
                        db.RollbackTransaction();
                        return result;
                    }

                    result.Id = accountId.Value;

                    foreach (var p in data)
                    {
                        //var patient = p.Item1;
                        var person = p.Item1;
                        var document = p.Item2;
                        var patMevents = p.Item3;

                        int? personId = default(int?);
                        int? documentId = default(int?);

                        if (person.IsNotNull())
                        {
                            var insertPersonResult = db.InsertWithIdentity(person);
                            if (Convert.ToInt32(insertPersonResult) == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи персональных данных пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            personId = Convert.ToInt32(insertPersonResult);
                        }

                        if (document.IsNotNull())
                        {
                            if (personId.HasValue)
                            {
                                document.PersonId = personId.Value;
                            }
                            else
                            {
                                result.AddError($"Отсутствует PersonalId у пациента ID { personId}");
                                db.RollbackTransaction();
                                return result;
                            }

                            var insertDocumentResult = db.InsertWithIdentity(document);
                            if (Convert.ToInt32(insertDocumentResult) == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи данных УДЛ пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            documentId = Convert.ToInt32(insertDocumentResult);
                        }


                        foreach (var patMevent in patMevents)
                        {
                            var patient = patMevent.Item1;
                            patient.MedicalAccountId = accountId;
                            if (personId.HasValue)
                            {
                                patient.PersonalId = personId;
                            }

                            if (documentId.HasValue)
                            {
                                patient.DocumentId = documentId;
                            }

                            var insertPatientResult = db.InsertWithIdentity(patient);
                            var patientId = Convert.ToInt32(insertPatientResult);
                            if (patientId == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи данных пациента в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }

                            var mevents = patMevent.Item2;
                            
                            foreach (var m in mevents)
                            {
                                var mevent = m.Item1;
                                mevent.PatientId = patientId;
                                var meventInsertResult = db.InsertWithIdentity(mevent);
                                var meventId = Convert.ToInt32(meventInsertResult);
                                if (meventId == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи законченного случая в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }
                                var refuz = new List<ZFactExternalRefuse>();
                                foreach (var sl in m.Item2)
                                {
                                    var zFactmedicalEvent = sl.Item1;
                                    zFactmedicalEvent.ZslMedicalEventId = meventId;
                                    ControlResourcesLoger.LogDedug(zFactmedicalEvent.SlIdGuid);
                                    var zFactmedicalEventInsertResult = db.InsertWithIdentity(zFactmedicalEvent);
                                    var zFactmedicalEventInsertResultId = Convert.ToInt32(zFactmedicalEventInsertResult);
                                    if (zFactmedicalEventInsertResultId == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи случая в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }

                                    var directions = sl.Item2;
                                    if (directions.Any())
                                    {
                                        directions.ForEach(s =>
                                        {
                                            s.ZmedicalEventId = zFactmedicalEventInsertResultId;
                                        });
                                        var insertdirectionResult = db.InsertBatch(directions);
                                        if (Convert.ToInt32(insertdirectionResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи направления в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }
                                    var сonsultations = sl.Item3;
                                    if (сonsultations.Any())
                                    {
                                        сonsultations.ForEach(s =>
                                        {
                                            s.ZMedicalEventId = zFactmedicalEventInsertResultId;
                                        });
                                        var insertсonsultationsResult = db.InsertBatch(сonsultations);
                                        if (Convert.ToInt32(insertсonsultationsResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи консилиума в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }
                                    var ds2 = sl.Item4;
                                    if (ds2.Any())
                                    {
                                        ds2.ForEach(s =>
                                        {
                                            s.ZmedicalEventId = zFactmedicalEventInsertResultId;
                                        });
                                        var insertds2Result = db.InsertBatch(ds2);
                                        if (Convert.ToInt32(insertds2Result) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи сопутствующего диагноза в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }

                                    var medicalEventOnk = sl.Item5.Item1;
                                    if (medicalEventOnk != null)
                                    {
                                        ControlResourcesLoger.LogDedug(sl.Item1.SlIdGuid + " - " + sl.Item1.ExternalId);
                                        medicalEventOnk.ZmedicalEventId = zFactmedicalEventInsertResultId;
                                        var insertmedicalEventOnkResult = db.InsertWithIdentity(medicalEventOnk);
                                        var zmedicalEventOnkId = Convert.ToInt32(insertmedicalEventOnkResult);
                                        if (zmedicalEventOnkId == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи сведения о случае лечения онкологического заболевания в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }

                                        var diagBloks = sl.Item5.Item2;
                                        if (diagBloks.Any())
                                        {
                                            diagBloks.ForEach(s => s.ZMedicalEventOnkId = zmedicalEventOnkId);
                                            var insertdiagBloksResult = db.InsertBatch(diagBloks);
                                            if (Convert.ToInt32(insertdiagBloksResult) == 0)
                                            {
                                                result.AddError(string.Format("Ошибка при записи диагностического блока в базу данных"));
                                                db.RollbackTransaction();
                                                return result;
                                            }
                                        }
                                        var сontraindications = sl.Item5.Item3;
                                        if (сontraindications.Any())
                                        {
                                            сontraindications.ForEach(s => s.ZMedicalEventOnkId = zmedicalEventOnkId);
                                            var insertсontraindicationsResult = db.InsertBatch(сontraindications);
                                            if (Convert.ToInt32(insertсontraindicationsResult) == 0)
                                            {
                                                result.AddError(string.Format("Ошибка при записи сведений об имеющихся противопоказаниях и отказах в базу данных"));
                                                db.RollbackTransaction();
                                                return result;
                                            }
                                        }
                                        var zFactMedicalServiceOnk = sl.Item5.Item4;
                                        if (zFactMedicalServiceOnk.Any())
                                        {
                                            foreach (var usl in zFactMedicalServiceOnk)
                                            {
                                                var zFactmedicalServiceOnk = usl.Item1;
                                                zFactmedicalServiceOnk.ZmedicalEventOnkId = zmedicalEventOnkId;
                                                var zFactmedicalServiceOnkResult = db.InsertWithIdentity(zFactmedicalServiceOnk);
                                                var zFactmedicalServiceOnkResultId = Convert.ToInt32(zFactmedicalServiceOnkResult);
                                                if (zFactmedicalServiceOnkResultId == 0)
                                                {
                                                    result.AddError(string.Format("Ошибка при записи онко услуги в базу данных"));
                                                    db.RollbackTransaction();
                                                    return result;
                                                }

                                                var zfactAnticancerDrugs = usl.Item2;
                                                if (zfactAnticancerDrugs.Any())
                                                {
                                                    zfactAnticancerDrugs.ForEach(s=>s.ZMedicalServiceOnkId = zFactmedicalServiceOnkResultId);
                                                    var insertAnticancerDrugsResult = db.InsertBatch(zfactAnticancerDrugs);
                                                    if (Convert.ToInt32(insertAnticancerDrugsResult) == 0)
                                                    {
                                                        result.AddError(string.Format("Ошибка при записи cведения о введенном противоопухолевом лекарственном препарате направления в базу данных"));
                                                        db.RollbackTransaction();
                                                        return result;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    var ksgKpg = sl.Item6.Item1;
                                    if (ksgKpg != null)
                                    {
                                        ksgKpg.ZmedicalEventId = zFactmedicalEventInsertResultId;
                                        var insertKsfKpgResult = db.InsertWithIdentity(ksgKpg);
                                        var zksgKpgId = Convert.ToInt32(insertKsfKpgResult);
                                        if (zksgKpgId == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи КСГ в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }

                                        var slcrit = sl.Item6.Item2;
                                        if (slcrit.Any())
                                        {
                                            slcrit.ForEach(s => s.ZksgKpgId = zksgKpgId);
                                            var insertslKoefsResult = db.InsertBatch(slcrit);
                                            if (Convert.ToInt32(insertslKoefsResult) == 0)
                                            {
                                                result.AddError(string.Format("Ошибка при записи slcrit в базу данных"));
                                                db.RollbackTransaction();
                                                return result;
                                            }
                                        }

                                        var slKoefs = sl.Item6.Item3;
                                        if (slKoefs.Any())
                                        {
                                            slKoefs.ForEach(s => s.ZksgKpgId = zksgKpgId);
                                            var insertslKoefsResult = db.InsertBatch(slKoefs);
                                            if (Convert.ToInt32(insertslKoefsResult) == 0)
                                            {
                                                result.AddError(string.Format("Ошибка при записи Коэффициента сложности лечения пациента в базу данных"));
                                                db.RollbackTransaction();
                                                return result;
                                            }
                                        }
                                    }

                                    var service = sl.Item7; 
                                    if (service.Any())
                                    {
                                        service.ForEach(s => s.ZmedicalEventId = zFactmedicalEventInsertResultId);
                                        var insertServicesResult = db.InsertBatch(service);
                                        if (Convert.ToInt32(insertServicesResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи услуг в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }
                                    refuz.Add(new ZFactExternalRefuse { ZslMedicalEventId = meventId, ZmedicalEventId = zFactmedicalEventInsertResultId, SlidGuid = zFactmedicalEvent.SlIdGuid });
                                }


                                var mecs = m.Item3.ToList();
                                if (mecs.Any())
                                {
                                    mecs.ForEach(s =>
                                    {
                                        s.ZslMedicalEventId = meventId;
                                        s.ZslAmount = mevent.MoPrice;
                                        s.PatientId = patientId;
                                    });
                                    var insertMecsResult = db.InsertBatch(mecs);
                                    if (Convert.ToInt32(insertMecsResult) == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи санкций в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }
                                }
                                foreach (var tuple in refuz)
                                {
                                    db.GetTableQuery<ZFactSank>().Where(s => s.ZslMedicalEventId == tuple.ZslMedicalEventId && s.SlidGuid == tuple.SlidGuid)
                                    .Set(s => s.ZmedicalEventId, tuple.ZmedicalEventId).Update();
                                }
                            }
                        }
                    }

                    var updateResult = UpdateZMedicalAccount(db, accountId);
                    if (updateResult.HasError)
                    {
                        result.AddError(string.Format("Ошибка при обновлении счета от МО"));
                        db.RollbackTransaction();
                        return result;
                    }

                    if (isTestLoad)
                    {
                        db.RollbackTransaction();
                    }
                    else
                    {
                        db.CommitTransaction();
                    }
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }

            }
            return result;
        }


        public TransactionResult<IEnumerable<FactSrzQuery>> GetSrzQueryByPatientId(int id)
        {
            var result = new TransactionResult<IEnumerable<FactSrzQuery>>();

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = SrzQueryByPatientIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<FactSrzQuery> GetSrzQueryById(int id)
        {
            var result = new TransactionResult<FactSrzQuery>();

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = SrzQueryByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult MarkSrzQueryAsReadById(int id)
        {
            var result = new TransactionResult();

            using (var db = CreateContext())
            {
                try
                {
                    var updateResult = db.FactSrzQuery
                        .Where(p => p.SrzQueryId == id)
                        .Set(p => p.IsReaded, true)
                        .Update();
                    if (!Convert.ToBoolean(updateResult))
                    {
                        result.AddError(db.LastError);
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;

            
        }

        public TransactionResult<MedicalAccountView> GetMedicalAccountViewById(int id)
        {
            var result = new TransactionResult<MedicalAccountView>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = MedicalAccountViewByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<EventShortView>> GetEventShortViewByAccountId(int id)
        {
            var result = new TransactionResult<IEnumerable<EventShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = EventShortViewByAccountIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZslEventShortView>> GetZslEventShortViewByAccountId(int id)
        {
            var result = new TransactionResult<IEnumerable<ZslEventShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZslEventShortViewByAccountIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<EventShortView>> GetEventShortViewByPatientId(int id)
        {
            var result = new TransactionResult<IEnumerable<EventShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = EventShortViewByPatientIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZslEventShortView>> GetZslEventShortViewByPatientId(int id)
        {
            var result = new TransactionResult<IEnumerable<ZslEventShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZslEventShortViewByPatientIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<EventShortView>> GetEventShortViewByMedicalAccountId(int id)
        {
            var result = new TransactionResult<IEnumerable<EventShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = EventShortViewByMedicalAccountIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZslEventShortView>> GetZslEventShortViewByMedicalAccountId(int id)
        {
            var result = new TransactionResult<IEnumerable<ZslEventShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZslEventShortViewByMedicalAccountIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZslEventView>> GetZslEventViewByMedicalAccountId(int id)
        {
            var result = new TransactionResult<IEnumerable<ZslEventView>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZslEventViewByMedicalAccountIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult InsertSrzQuery(int patientId, string guid, int type)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    var srzQueryModel = new FactSrzQuery
                    {
                        Guid = guid,
                        PatientId = patientId,
                        Status = 0,
                        DateQuery = Sql.GetDate(),
                        DateUpdate = Sql.GetDate(),
                        IsReaded = true,
                        Type = type
                    };
                    var insertResult = db.InsertWithIdentity(srzQueryModel);
                    if (Convert.ToBoolean(insertResult))
                    {
                        result.Id = Convert.ToInt32(insertResult);
                    }
                    else
                    {
                        result.AddError(db.LastError);
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteTerritoryAccount(int id)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                db.BeginTransaction();
                try
                {
                    db.CommandTimeout = 120;
                    FactTerritoryAccount account = db.GetTableQuery<FactTerritoryAccount>().FirstOrDefault(p => p.TerritoryAccountId == id);
                    if (account == null)
                    {
                        throw new ArgumentException("TerritoryAccount ID {0} не найден".F(id));
                    }

                    if (account.Version != null && Constants.ZterritoryVersion.Contains((int)account.Version))
                    {
                        db.GetTableQuery<ZFactAnticancerDrug>().Delete(p => p.ZFactMedicalServicesOnk.ZFactMedicalEventOnk.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<ZFactMedicalServicesOnk>().Delete(p => p.ZFactMedicalEventOnk.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);

                        db.GetTableQuery<ZFactDiagBlok>().Delete(p => p.ZFactMedicalEventOnk.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<ZFactContraindications>().Delete(p => p.ZFactMedicalEventOnk.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<ZFactMedicalEventOnk>().Delete(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);

                        db.GetTableQuery<ZFactDirection>().Delete(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<ZFactConsultations>().Delete(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<ZFactDs>().Delete(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);

                        db.GetTableQuery<ZFactExternalRefuse>().Delete(p => p.FKZMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<ZFactSank>().Delete(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);

                        db.GetTableQuery<ZFactCrit>().Delete(p => p.ZFactKsgKpg.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<ZFactSlKoef>().Delete(p => p.ZFactKsgKpg.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<ZFactKsgKpg>().Delete(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<ZFactMedicalServices>().Delete(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<ZFactMedicalEvent>().Delete(p => p.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<ZslFactMedicalEvent>().Delete(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);

                        db.GetTableQuery<FactSrzQuery>().Delete(p => p.FCFACTPATPATIENTID.AccountId == id);

                        db.GetTableQuery<FactEconomicPayment>().Delete(p => p.FACTECONPAYFACTECONACCID.AccountId == id);
                        db.GetTableQuery<FactEconomicPaymentDetail>().Delete(p => p.AccountId == id);
                        db.GetTableQuery<FactEconomicAccount>().Delete(p => p.AccountId == id);

                        db.GetTableQuery<FactEconomicRefuseDetail>()
                            .Delete(p => p.FACTECONPAYFACTECONREFD.AccountId == id);
                        db.GetTableQuery<FactEconomicRefuse>().Delete(p => p.AccountId == id);

                        db.GetTableQuery<FactEconomicSurchargeDetail>()
                            .Delete(p => p.FACTECONPAYFACTECONSURD.AccountId == id);
                        db.GetTableQuery<FactEconomicSurcharge>().Delete(p => p.AccountId == id);

                        //TODO ASHUR сделать для других таблиц под законченный случай

                        //db.GetTableQuery<FactActEqma>().Delete(p => p.FACTEQMAPATIENTID.AccountId == id);
                        //db.GetTableQuery<FactActMee>().Delete(p => p.FACTMEEPATIENT.AccountId == id);

                        //db.GetTableQuery<FactSrzQuery>().Delete(p => p.FCFACTPATPATIENTID.AccountId == id);

                        //db.GetTableQuery<FactPreparedReport>().Delete(p => p.ExternalId == id && p.Scope == 2);

                        //db.GetTableQuery<FactEconomicPayment>().Delete(p => p.FACTECONPAYFACTECONACCID.AccountId == id);
                        //db.GetTableQuery<FactEconomicPaymentDetail>().Delete(p => p.AccountId == id);
                        //db.GetTableQuery<FactEconomicAccount>().Delete(p => p.AccountId == id);

                        //db.GetTableQuery<FactEconomicRefuseDetail>()
                        //    .Delete(p => p.FACTECONPAYFACTECONREFD.AccountId == id);
                        //db.GetTableQuery<FactEconomicRefuse>().Delete(p => p.AccountId == id);

                        //db.GetTableQuery<FactEconomicSurchargeDetail>()
                        //    .Delete(p => p.FACTECONPAYFACTECONSURD.AccountId == id);
                        //db.GetTableQuery<FactEconomicSurcharge>().Delete(p => p.AccountId == id);

                        db.GetTableQuery<FactPatient>().Delete(p => p.AccountId == id);
                        db.GetTableQuery<FactTerritoryAccount>().Delete(p => p.TerritoryAccountId == id);


                    }
                    else
                    {
                        db.GetTableQuery<FactExternalRefuse>()
                            .Delete(p => p.FKMedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == id);

                        db.GetTableQuery<FactMEC>()
                            .Delete(p => p.MedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<FactMEE>()
                            .Delete(p => p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<FactEQMA>()
                            .Delete(p => p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId == id);

                        db.GetTableQuery<FactActEqma>().Delete(p => p.FACTEQMAPATIENTID.AccountId == id);
                        db.GetTableQuery<FactActMee>().Delete(p => p.FACTMEEPATIENT.AccountId == id);

                        db.GetTableQuery<FactSrzQuery>().Delete(p => p.FCFACTPATPATIENTID.AccountId == id);

                        db.GetTableQuery<FactPreparedReport>().Delete(p => p.ExternalId == id && p.Scope == 2);

                        db.GetTableQuery<FactEconomicPayment>().Delete(p => p.FACTECONPAYFACTECONACCID.AccountId == id);
                        db.GetTableQuery<FactEconomicPaymentDetail>().Delete(p => p.AccountId == id);
                        db.GetTableQuery<FactEconomicAccount>().Delete(p => p.AccountId == id);

                        db.GetTableQuery<FactEconomicRefuseDetail>()
                            .Delete(p => p.FACTECONPAYFACTECONREFD.AccountId == id);
                        db.GetTableQuery<FactEconomicRefuse>().Delete(p => p.AccountId == id);

                        db.GetTableQuery<FactEconomicSurchargeDetail>()
                            .Delete(p => p.FACTECONPAYFACTECONSURD.AccountId == id);
                        db.GetTableQuery<FactEconomicSurcharge>().Delete(p => p.AccountId == id);

                        db.GetTableQuery<FactMedicalServices>()
                            .Delete(p => p.FACTMEDIFMSMEIDFACTMEDI.FACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<FactMedicalEvent>().Delete(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<FactPatient>().Delete(p => p.AccountId == id);
                        db.GetTableQuery<FactTerritoryAccount>().Delete(p => p.TerritoryAccountId == id);

                    }
                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<bool> IsErrorForTerritoryAccountExist(int id)
        {
            var result = new TransactionResult<bool>();

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactPatient.Any(p => p.AccountId == id && p.FACTMEDIPATIENTIDFACTPATIs.Any(r => r.PaymentStatus != 2));
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<bool> IsErrorForZTerritoryAccountExist(int id)
        {
            var result = new TransactionResult<bool>();

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactPatient.Any(p => p.AccountId == id && p.ZSLFACTMEDIPATIENTIDFACTPATIs.Any(r => r.PaymentStatus != 2));
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<bool> IsSrzQueriesForTerritoryAccountExist(int id)
        {
            var result = new TransactionResult<bool>();

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = SrzQueriesForTerritoryAccountQuery(db, id).Any();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult BreakUpTerritoryAccount(int id)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                db.BeginTransaction();
                try
                {
                    FactTerritoryAccount account = db.GetTableQuery<FactTerritoryAccount>().FirstOrDefault(p => p.TerritoryAccountId == id);
                    if (account == null)
                    {
                        throw new ArgumentException("TerritoryAccount ID {0} не найден".F(id));
                    }

                    if (account.Version == 5)
                    {
                        db.GetTableQuery<ZFactExternalRefuse>()
                            .Delete(p => p.FKZMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);

                        db.GetTableQuery<ZFactSank>()
                            .Delete(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        //TODO ASHUR сделать для других таблиц под законченный случай

                        //db.GetTableQuery<FactActEqma>().Delete(p => p.FACTEQMAPATIENTID.AccountId == id);
                        //db.GetTableQuery<FactActMee>().Delete(p => p.FACTMEEPATIENT.AccountId == id);

                        //db.GetTableQuery<FactSrzQuery>().Delete(p => p.FCFACTPATPATIENTID.AccountId == id);

                        //db.GetTableQuery<FactPreparedReport>().Delete(p => p.ExternalId == id && p.Scope == 2);

                        //db.GetTableQuery<FactEconomicPayment>().Delete(p => p.FACTECONPAYFACTECONACCID.AccountId == id);
                        //db.GetTableQuery<FactEconomicPaymentDetail>().Delete(p => p.AccountId == id);
                        //db.GetTableQuery<FactEconomicAccount>().Delete(p => p.AccountId == id);

                        //db.GetTableQuery<FactEconomicRefuseDetail>()
                        //    .Delete(p => p.FACTECONPAYFACTECONREFD.AccountId == id);
                        //db.GetTableQuery<FactEconomicRefuse>().Delete(p => p.AccountId == id);

                        //db.GetTableQuery<FactEconomicSurchargeDetail>()
                        //    .Delete(p => p.FACTECONPAYFACTECONSURD.AccountId == id);
                        //db.GetTableQuery<FactEconomicSurcharge>().Delete(p => p.AccountId == id);

                        db.GetTableQuery<FactPatient>()
                            .Where(p => p.AccountId == id)
                            .Set(s => s.AccountId, default(int?))
                            .Update();

                        db.GetTableQuery<FactTerritoryAccount>().Delete(p => p.TerritoryAccountId == id);

                        db.CommitTransaction();
                    }
                    else
                    {
                        db.GetTableQuery<FactExternalRefuse>()
                           .Delete(p => p.FKMedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == id);

                        db.GetTableQuery<FactMEC>()
                            .Delete(p => p.MedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<FactMEE>()
                            .Delete(p => p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId == id);
                        db.GetTableQuery<FactEQMA>()
                            .Delete(p => p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId == id);

                        db.GetTableQuery<FactActEqma>().Delete(p => p.FACTEQMAPATIENTID.AccountId == id);
                        db.GetTableQuery<FactActMee>().Delete(p => p.FACTMEEPATIENT.AccountId == id);

                        db.GetTableQuery<FactSrzQuery>().Delete(p => p.FCFACTPATPATIENTID.AccountId == id);

                        db.GetTableQuery<FactPreparedReport>().Delete(p => p.ExternalId == id && p.Scope == 2);

                        db.GetTableQuery<FactEconomicPayment>().Delete(p => p.FACTECONPAYFACTECONACCID.AccountId == id);
                        db.GetTableQuery<FactEconomicPaymentDetail>().Delete(p => p.AccountId == id);
                        db.GetTableQuery<FactEconomicAccount>().Delete(p => p.AccountId == id);

                        db.GetTableQuery<FactEconomicRefuseDetail>()
                            .Delete(p => p.FACTECONPAYFACTECONREFD.AccountId == id);
                        db.GetTableQuery<FactEconomicRefuse>().Delete(p => p.AccountId == id);

                        db.GetTableQuery<FactEconomicSurchargeDetail>()
                            .Delete(p => p.FACTECONPAYFACTECONSURD.AccountId == id);
                        db.GetTableQuery<FactEconomicSurcharge>().Delete(p => p.AccountId == id);

                        db.GetTableQuery<FactPatient>()
                            .Where(p => p.AccountId == id)
                            .Set(s => s.AccountId, default(int?))
                            .Update();

                        db.GetTableQuery<FactTerritoryAccount>().Delete(p => p.TerritoryAccountId == id);

                        db.CommitTransaction();
                    }
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;

            
        }

        public TransactionResult<IEnumerable<EventShortView>> GetEventShortViewByMedicalEventId(int id)
        {
            var result = new TransactionResult<IEnumerable<EventShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = EventShortViewByMedicalEventIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZslEventShortView>> GetZslEventShortViewByMedicalEventId(int id)
        {
            var result = new TransactionResult<IEnumerable<ZslEventShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZslEventShortViewByMedicalEventIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<int>> GetAvailableYearsForMedicalAccounts()
        {
            var result = new TransactionResult<IEnumerable<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.GetTableQuery<FactMedicalAccount>()
                        .Where(p=>p.Date.HasValue)
                        .Select(p => p.Date.Value.Year)
                        .Distinct()
                        .OrderBy(p => p)
                        .ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<int>> GetAvailableYearsForMedicalAccounts2018()
        {
            var result = new TransactionResult<IEnumerable<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.GetTableQuery<FactMedicalAccount>()
                        .Where(p => p.Date.HasValue && p.Date.Value.Year == 2018) //
                        .Select(p => p.Date.Value.Year)
                        .Distinct()
                        .OrderBy(p => p)
                        .ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<CommonTuple>> GetAvailableMonthsForMedicalAccounts()
        {
            var result = new TransactionResult<IEnumerable<CommonTuple>>();
            using (var db = CreateContext())
            {
                try
                {
                    var culture = new CultureInfo("ru-RU");
                    result.Data = db.GetTableQuery<FactMedicalAccount>()
                        .Where(p => p.Date.HasValue)
                        .Select(p => p.Date.Value.Month)
                        .Distinct()
                        .OrderBy(p => p)
                        .Select(p => new CommonTuple
                        {
                            ValueField = p,
                            DisplayField = culture.DateTimeFormat.MonthNames[p-1]
                        })
                        .ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IDictionary<F010,IGrouping<F010,FactPatient>>> GetUnmakePatientsByDate(int year, int month)
        {
            var result = new TransactionResult<IDictionary<F010,IGrouping<F010,FactPatient>>>();

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactPatient
                        .Where(p => p.AccountId == null &&
                                    p.FACTPATIREGISTERIFACTREGI.Date.Value.Year == year &&
                                    p.FACTPATIREGISTERIFACTREGI.Date.Value.Month == month &&
                                    p.TerritoryOkato.HasValue && p.TerritoryOkato != TerritoryService.TfCode &&
                                    p.FACTMEDIPATIENTIDFACTPATIs.All(s => s.AcceptPrice != null && s.AcceptPrice > 0))
                        .GroupBy(p => p.FACTPERSF010)
                        .ToDictionary(r => r.Key);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
            
        }
       
        public TransactionResult<IDictionary<F010, IGrouping<F010, FactPatient>>> GetZUnmakePatientsByDate(int year, int month)
        {
            var result = new TransactionResult<IDictionary<F010, IGrouping<F010, FactPatient>>>();
           
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactPatient
                        .Where(p => p.AccountId == null &&
                                    p.FACTPATIREGISTERIFACTREGI.Date.Value.Year == year &&
                                    p.FACTPATIREGISTERIFACTREGI.Date.Value.Month == month &&
                                    p.TerritoryOkato.HasValue && p.TerritoryOkato != TerritoryService.TfCode &&
                                    p.ZSLFACTMEDIPATIENTIDFACTPATIs.All(s => s.AcceptPrice != null && s.AcceptPrice > 0))
                        .GroupBy(p => p.FACTPERSF010)
                        .ToDictionary(r => r.Key);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;

        }

        public TransactionResult<IEnumerable<FactTerritoryAccount>> GetTerritoryAccount(Expression<Func<FactTerritoryAccount, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactTerritoryAccount>>();
                try
                {
                    result.Data = predicate == null ?
                        TerritoryAccountQuery(db).ToList() :
                        db.FactTerritoryAccount.Where(predicate).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<int> GetTerritoryAccountLastPacketNumber(Expression<Func<FactTerritoryAccount, bool>> predicate)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int>();
                try
                {
                    result.Data = 1;
                    int? packetNumber = predicate == null ?
                        db.FactTerritoryAccount
                               .Max(p => p.PacketNumber) :
                        db.FactTerritoryAccount
                               .Where(predicate)
                               .Max(p => p.PacketNumber);
                    if (packetNumber.HasValue)
                    {
                        result.Data = packetNumber.Value;
                    } 
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<int> GetTerritoryAccountLastAccountNumber(Expression<Func<FactTerritoryAccount, bool>> predicate)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int>();
                try
                {
                    if (db.FactTerritoryAccount.Any(predicate))
                    {
                        result.Data = db.GetTableQuery<FactTerritoryAccount>()
                            .Where(predicate)
                            .Max(p => Convert.ToInt32(p.AccountNumber));
                    }

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult CreateTerritoryAccount(string okato, int year, int month, int packetNumber, int accountNumber, int version)
        {
            var result = new TransactionResult();

            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    var account = new FactTerritoryAccount
                    {
                        Date = new DateTime(year, month, 1),
                        AccountNumber = accountNumber.ToString(CultureInfo.InvariantCulture),
                        AccountDate = Sql.GetDate(),
                        Price = 0,
                        AcceptPrice = 0,
                        MECPenalties = 0,
                        MEEPenalties = 0,
                        EQMAPenalties = 0,
                        Status = 1,
                        Type = 1,
                        Destination = okato,
                        Source = TerritoryService.TerritoryOkato,
                        Comments = "",
                        ExternalId = 0,
                        Direction = 0,
                        Parent = 0,
                        PacketNumber = packetNumber,
                        Version = version
                    };
                    var accountResult = db.InsertWithIdentity(account);
                    if (Convert.ToBoolean(accountResult))
                    {
                        var id = SafeConvert.ToInt32(accountResult.ToString(), false);
                        if (id.HasValue)
                        {
                            result.Id = id.Value;
                        }

                        var updateResult = db.FactTerritoryAccount.Where(s => s.TerritoryAccountId == result.Id)
                            .Set(s => s.ExternalId, result.Id)
                            .Update();
                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Ошибка обновления внешнего ID счета на территорию.\r\n{0}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        throw  new InvalidOperationException("Ошибка создания нового счета на территорию.\r\n{0}".F(db.LastQuery));
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult AttachPatientsToTerritoryAccount(Expression<Func<FactPatient, bool>> predicate, int accountId)
        {
            var result = new TransactionResult();

            using (var db = CreateContext())
            {
                try
                {
                    var updateResult = db.GetTableQuery<FactPatient>()
                            .Where(predicate)
                            .Set(s => s.AccountId, accountId)
                            .Update();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
            
        }

        public TransactionResult EnumeratePatientsOfTerritoryAccountTransact(int accountId, MedicineContext context = null)
        {
            var result = new TransactionResult();
            MedicineContext db = context.IsNotNull() ? context : CreateContext();
            
            try
            {
                db.SetCommand(@"UPDATE x
                            SET x.ExternalId = x.NewExternalId
                            FROM (
                                    SELECT ExternalId, ROW_NUMBER() OVER (ORDER BY FactPerson.Surname, FactPerson.PName, FactPerson.Patronymic) AS NewExternalId
			
                                    FROM FactPatient 
			                            INNER JOIN FactPerson ON FactPatient.PersonalId = FactPerson.PersonId
			                            WHERE AccountId = @AccountId
                                    ) x ", db.Parameter("@AccountId", accountId))
                    .ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            if (context.IsNull())
            {
                db.Dispose();
            }

            return result;
        }

        public TransactionResult EnumeratePatientsOfTerritoryAccount(int accountId)
        {
            var result = new TransactionResult();
            using (var db =  CreateContext())
            {
                try
                {
                    db.SetCommand(@"UPDATE x
                                    SET x.ExternalId = x.NewExternalId
                                    FROM (
                                            SELECT ExternalId, ROW_NUMBER() OVER (ORDER BY FactPerson.Surname, FactPerson.PName, FactPerson.Patronymic) AS NewExternalId
			
                                            FROM FactPatient 
			                                    INNER JOIN FactPerson ON FactPatient.PersonalId = FactPerson.PersonId
			                                    WHERE AccountId = @AccountId
                                            ) x ", db.Parameter("@AccountId", accountId))
                        .ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult EnumerateEventsOfTerritoryAccountTransact(int accountId, MedicineContext context = null)
        {
            var result = new TransactionResult();
            MedicineContext db = context.IsNotNull() ? context : CreateContext();
            try
            {
                db.SetCommand(@"UPDATE x
                                SET x.ExternalId = x.NewExternalId
                                FROM (
                                        SELECT FactMedicalEvent.ExternalId, ROW_NUMBER() OVER (ORDER BY FactPatient.ExternalId) AS NewExternalId
			
                                        FROM FactMedicalEvent 
			                                INNER JOIN FactPatient ON FactPatient.PatientId = FactMedicalEvent.PatientId
			                                INNER JOIN FactPerson ON FactPatient.PersonalId = FactPerson.PersonId
			                                WHERE FactPatient.AccountId = @AccountId
                                        ) x", db.Parameter("@AccountId", accountId))
                    .ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            if (context.IsNull())
            {
                db.Dispose();
            }
            
            return result;
        }

        public TransactionResult EnumerateZEventsOfTerritoryAccountTransact(int accountId, MedicineContext context = null)
        {
            var result = new TransactionResult();
            MedicineContext db = context.IsNotNull() ? context : CreateContext();

            try
            {
                db.SetCommand(@"UPDATE x
                                    SET x.ExternalId = x.NewExternalId
                                    FROM (
                                            SELECT ZslFactMedicalEvent.ExternalId, ROW_NUMBER() OVER (ORDER BY FactPatient.ExternalId) AS NewExternalId
			
                                            FROM ZslFactMedicalEvent 
			                                    INNER JOIN FactPatient ON FactPatient.PatientId = ZslFactMedicalEvent.PatientId
			                                    INNER JOIN FactPerson ON FactPatient.PersonalId = FactPerson.PersonId
			                                    WHERE FactPatient.AccountId = @AccountId
                                            ) x", db.Parameter("@AccountId", accountId))
                    .ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            if (context.IsNull())
            {
                db.Dispose();
            }
            return result;
        }

        public TransactionResult EnumerateZslEventsOfTerritoryAccount(int accountId)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.SetCommand(@"UPDATE x
                                    SET x.ExternalId = x.NewExternalId
                                    FROM (
                                            SELECT ZslFactMedicalEvent.ExternalId, ROW_NUMBER() OVER (ORDER BY FactPatient.ExternalId) AS NewExternalId
			
                                            FROM ZslFactMedicalEvent 
			                                    INNER JOIN FactPatient ON FactPatient.PatientId = ZslFactMedicalEvent.PatientId
			                                    INNER JOIN FactPerson ON FactPatient.PersonalId = FactPerson.PersonId
			                                    WHERE FactPatient.AccountId = @AccountId
                                            ) x", db.Parameter("@AccountId", accountId))
                        .ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult EnumerateZEventsOfTerritoryAccount(int accountId)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.SetCommand(@"UPDATE x
                                    SET x.ExternalId = x.NewExternalId
                                    FROM (
                                            SELECT zme.ExternalId, ROW_NUMBER() OVER (PARTITION BY ZslFactMedicalEvent.ExternalId ORDER BY ZslFactMedicalEvent.ExternalId) AS NewExternalId
			
                                           FROM ZslFactMedicalEvent
                                            INNER JOIN ZFactMedicalEvent AS zme ON zme.ZslMedicalEventId = ZslFactMedicalEvent.ZslMedicalEventId
			                                    INNER JOIN FactPatient ON FactPatient.PatientId = ZslFactMedicalEvent.PatientId
			                                    INNER JOIN FactPerson ON FactPatient.PersonalId = FactPerson.PersonId
			                                    WHERE FactPatient.AccountId = @AccountId
                                            ) x", db.Parameter("@AccountId", accountId))
                        .ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }


        public TransactionResult EnumerateEventsOfTerritoryAccount(int accountId)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.SetCommand(@"UPDATE x
                                    SET x.ExternalId = x.NewExternalId
                                    FROM (
                                            SELECT FactMedicalEvent.ExternalId, ROW_NUMBER() OVER (ORDER BY FactPatient.ExternalId) AS NewExternalId
			
                                            FROM FactMedicalEvent 
			                                    INNER JOIN FactPatient ON FactPatient.PatientId = FactMedicalEvent.PatientId
			                                    INNER JOIN FactPerson ON FactPatient.PersonalId = FactPerson.PersonId
			                                    WHERE FactPatient.AccountId = @AccountId
                                            ) x", db.Parameter("@AccountId", accountId))
                        .ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<bool> IsTerritoryAccountExistsForMedicalAccount(int medicalAccountId)
        {
            var result = new TransactionResult<bool>();
            using (var db = CreateContext())
            {
                try
                {
                    var patients = db.FactPatient.Count(p => p.MedicalAccountId == medicalAccountId && p.AccountId != null);
                    result.Data = patients > 0;
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteMedicalAccount(int medicalAccountId)
        {
            var result = new TransactionResult<bool>();
            using (var db = CreateContext())
            {
                try
                {
                    db.CommandTimeout = 2400;
                    db.GetTableQuery<FactExternalRefuse>().Delete(p => p.FKMedicalEvent.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);

                    db.GetTableQuery<FactMEC>().Delete(p => p.MedicalEvent.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<FactMEE>().Delete(p => p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<FactEQMA>().Delete(p => p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);

                    db.GetTableQuery<FactActEqma>().Delete(p => p.FACTEQMAPATIENTID.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<FactActMee>().Delete(p => p.FACTMEEPATIENT.MedicalAccountId == medicalAccountId);

                    db.GetTableQuery<FactSrzQuery>().Delete(p => p.FCFACTPATPATIENTID.MedicalAccountId == medicalAccountId);

                    db.GetTableQuery<FactMedicalServices>().Delete(p => p.FACTMEDIFMSMEIDFACTMEDI.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<FactMedicalEvent>().Delete(p => p.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<FactPatient>().Delete(p => p.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<FactMedicalAccount>().Delete(p => p.MedicalAccountId == medicalAccountId);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;

            
        }

        public TransactionResult DeleteZMedicalAccount(int medicalAccountId)
        {
            var result = new TransactionResult<bool>();
            using (var db = CreateContext())
            {
                try
                {
                    db.CommandTimeout = 2400;
                    db.GetTableQuery<ZFactExternalRefuse>().Delete(p => p.FKZMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);

                    //db.GetTableQuery<FactActEqma>().Delete(p => p.FACTEQMAPATIENTID.MedicalAccountId == medicalAccountId);
                    //db.GetTableQuery<FactActMee>().Delete(p => p.FACTMEEPATIENT.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<FactSrzQuery>().Delete(p => p.FCFACTPATPATIENTID.MedicalAccountId == medicalAccountId);

                    db.GetTableQuery<ZFactAnticancerDrug>().Delete(p => p.ZFactMedicalServicesOnk.ZFactMedicalEventOnk.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<ZFactMedicalServicesOnk>().Delete(p => p.ZFactMedicalEventOnk.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);

                    db.GetTableQuery<ZFactDiagBlok>().Delete(p => p.ZFactMedicalEventOnk.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<ZFactContraindications>().Delete(p => p.ZFactMedicalEventOnk.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<ZFactMedicalEventOnk>().Delete(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);

                    db.GetTableQuery<ZFactDirection>().Delete(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<ZFactConsultations>().Delete(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<ZFactDs>().Delete(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);

                    db.GetTableQuery<ZFactSank>().Delete(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);

                    db.GetTableQuery<ZFactCrit>().Delete(p => p.ZFactKsgKpg.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<ZFactSlKoef>().Delete(p => p.ZFactKsgKpg.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<ZFactKsgKpg>().Delete(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);

                    db.GetTableQuery<ZFactMedicalServices>().Delete(p => p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<ZFactMedicalEvent>().Delete(p => p.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<ZslFactMedicalEvent>().Delete(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<FactPatient>().Delete(p => p.MedicalAccountId == medicalAccountId);
                    db.GetTableQuery<FactMedicalAccount>().Delete(p => p.MedicalAccountId == medicalAccountId);
                    
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<bool> IsErrorForMedicalAccountExist(int id)
        {
            var result = new TransactionResult<bool>();

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = PatientWithErrorFromMedicalAccountQuery(db, id).Any();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<bool> IsErrorForZMedicalAccountExist(int id)
        {
            var result = new TransactionResult<bool>();

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZPatientWithErrorFromMedicalAccountQuery(db, id).Any();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<bool> IsSrzQueriesForMedicalAccountExist(int id)
        {
            var result = new TransactionResult<bool>();

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = SrzQueriesForMedicalAccountQuery(db, id).Any();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<FactMedicalEvent> GetMedicalEventById(int id)
        {
            var result = new TransactionResult<FactMedicalEvent>();

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = MedicalEventByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<ZFactMedicalEvent> GetZMedicalEventById(int id)
        {
            var result = new TransactionResult<ZFactMedicalEvent>();

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZMedicalEventByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<string> GetZMedicalEventBySlidGuid(int? id)
        {
            var result = new TransactionResult<string>();

            using (var db = CreateContext())
            {
                try
                {
                    if (id.IsNotNull())
                    {
                        result.Data = db.ZFactMedicalEvent.FirstOrDefault(p => p.ZmedicalEventId == id)?.SlIdGuid;
                    }
                    
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<string> GetZMedicalEventByExternalId(int? id)
        {
            var result = new TransactionResult<string>();

            using (var db = CreateContext())
            {
                try
                {
                    if (id.IsNotNull())
                    {
                        result.Data = db.ZFactMedicalEvent.FirstOrDefault(p => p.ZmedicalEventId == id)?.ExternalId;
                    }

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<ZFactKsgKpg> GetZKsgKpgById(int id)
        {
            var result = new TransactionResult<ZFactKsgKpg>();

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZKsgKpgByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<ZFactMedicalEventOnk> GetZMedicalEventOnkById(int id)
        {
            var result = new TransactionResult<ZFactMedicalEventOnk>();

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZMedicalEventOnkByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<ZslFactMedicalEvent> GetZslMedicalEventById(int id)
        {
            var result = new TransactionResult<ZslFactMedicalEvent>();

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZslMedicalEventByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<List<int>> GetMedicalEventIdsByPatientId(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = FactMedicalEventIdsByPatientIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }
        public TransactionResult<List<int>> GetZMedicalEventIdsByZslMeventId(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZFactMedicalEventIdsByZslMeventIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<List<int>> GetZKsgKpgIdsByZslMeventId(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZFactKsgKpgIdsByZslMeventIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<List<int>> GetZMedicalEventOnkIdsByZslMeventId(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZFactMedicalEvenOnktIdsByZslMeventIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<List<int>> GetZslMedicalEventIdsByPatientId(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZslFactMedicalEventIdsByPatientIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<List<int>> GetMedicalServiceIdsByMedicalEventId(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = FactMedicalServiceIdsByMedicalEventIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<List<int>> GetZDirectionIdsByMedicalEventId(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZFactDirectionIdsByMedicalEventIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<List<int>> GetZMedicalServiceIdsByMedicalEventId(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZFactMedicalServiceIdsByMedicalEventIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<List<int>> GetZMedicalServiceOnkIdsByMedicalEventId(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZFactMedicalServiceOnkIdsByMedicalEventIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactMedicalServicesOnk>> GetZMedicalServiceOnkByMedicalEventId(int id)
        {
            var result = new TransactionResult<IEnumerable<ZFactMedicalServicesOnk>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZFactMedicalServicesOnk.Where(p=>p.ZmedicalEventOnkId == id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<List<int>> GetZMedicalConsultationsOnkIdsByMedicalEventId(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZFactMedicalConsultationsOnkIdsByMedicalEventIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<FactMedicalServices> GetMedicalServiceById(int id)
        {
            var result = new TransactionResult<FactMedicalServices>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = MedicalServiceByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<ZFactDirection> GetDirectionById(int id)
        {
            var result = new TransactionResult<ZFactDirection>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZDirectionByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<ZFactMedicalServices> GetZMedicalServiceById(int id)
        {
            var result = new TransactionResult<ZFactMedicalServices>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZMedicalServiceByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<ZFactMedicalServicesOnk> GetZMedicalServiceOnkById(int id)
        {
            var result = new TransactionResult<ZFactMedicalServicesOnk>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZMedicalServiceOnkByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<ZFactConsultations> GetZMedicalConsultationsOnkById(int id)
        {
            var result = new TransactionResult<ZFactConsultations>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZMedicalConsultationsOnkByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<List<int>> GetPatientsIdsByMedicalAccountId(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = FactPatientsIdsByMedicalAccountIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<List<int>> GetPatientsIdsByMedicalAccountIdWithError(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactMedicalEvent
                        .Where(p=>p.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == id && p.MoPaymentStatus != 2)
                        .OrderBy(p=>p.FACTMEDIPATIENTIDFACTPATI.MedicalExternalId)
                        .Select(p=>p.FACTMEDIPATIENTIDFACTPATI.PatientId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<List<int>> GetZPatientsIdsByMedicalAccountIdWithError(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZslFactMedicalEvent
                        .Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == id && p.MoPaymentStatus != 2)
                        .OrderBy(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalExternalId)
                        .Select(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.PatientId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<List<int>> GetPatientsIdsByMedicalAccountIdWithSrzQuery(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.GetTableQuery<FactSrzQuery>()
                        .Where(p => p.FCFACTPATPATIENTID.MedicalAccountId == id)
                        .OrderBy(p => p.FCFACTPATPATIENTID.ExternalId)
                        .Select(p => p.FCFACTPATPATIENTID)
                        .Distinct()
                        .Select(s=>s.PatientId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
            
        }

        public TransactionResult<List<int>> GetPatientsIdsByAccountIdWithError(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactMedicalEvent
                        .Where(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == id && p.PaymentStatus != 2)
                        .OrderBy(p => p.FACTMEDIPATIENTIDFACTPATI.ExternalId)
                        .Select(p => p.FACTMEDIPATIENTIDFACTPATI)
                        .Distinct()
                        .Select(p => p.PatientId)
                        .ToList();

                    //by Ira
                    /*var result1 = db.FactMedicalEvent
                        .Where(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == id && p.PaymentStatus != 2)
                        .OrderBy(p => p.FACTMEDIPATIENTIDFACTPATI.ExternalId)
                        .Select(p => p.FACTMEDIPATIENTIDFACTPATI);
                    result.Data = result1.Distinct().Select(p => p.PatientId)
                        .ToList();*/

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<List<int>> GetZPatientsIdsByAccountIdWithError(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZslFactMedicalEvent
                        .Where(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id && p.PaymentStatus != 2)
                        .OrderBy(p => p.ZSLFACTMEDIPATIENTIDFACTPATI.ExternalId)
                        .Select(p => p.ZSLFACTMEDIPATIENTIDFACTPATI)
                        .Distinct()
                        .Select(p => p.PatientId)
                        .ToList();

                    //by Ira
                    /*var result1 = db.FactMedicalEvent
                        .Where(p => p.FACTMEDIPATIENTIDFACTPATI.AccountId == id && p.PaymentStatus != 2)
                        .OrderBy(p => p.FACTMEDIPATIENTIDFACTPATI.ExternalId)
                        .Select(p => p.FACTMEDIPATIENTIDFACTPATI);
                    result.Data = result1.Distinct().Select(p => p.PatientId)
                        .ToList();*/

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<List<int>> GetPatientsIdsByAccountIdWithSrzQuery(int id)
        {
            var result = new TransactionResult<List<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.GetTableQuery<FactSrzQuery>()
                        .Where(p => p.FCFACTPATPATIENTID.AccountId == id)
                        .OrderBy(p => p.FCFACTPATPATIENTID.ExternalId)
                        .Select(p => p.FCFACTPATPATIENTID)
                        .Distinct()
                        .Select(s => s.PatientId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactMEC>> GetMecByMedicalEventIdAndSource(int id, int? source)
        {
            var result = new TransactionResult<IEnumerable<FactMEC>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = MecByMedicalEventIdAndSourceQuery(db, id, source).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactMEE>> GetMeeByMedicalEventIdAndSource(int id, int? source)
        {
            var result = new TransactionResult<IEnumerable<FactMEE>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = MeeByMedicalEventIdAndSourceQuery(db, id, source).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactEQMA>> GetEqmaByMedicalEventIdAndSource(int id, int? source)
        {
            var result = new TransactionResult<IEnumerable<FactEQMA>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = EqmaByMedicalEventIdAndSourceQuery(db, id, source).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult ExcludePatientFromTerritorryAccount(int patientId, int accountId)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    var updateResult = db.FactPatient.Where(p => p.PatientId == patientId)
                            .Set(s => s.AccountId, default(int?))
                            .Update();
                    if (!Convert.ToBoolean(updateResult))
                    {
                        result.AddError(db.LastError);
                        db.RollbackTransaction();
                    }

                    var accountResult = GetTerritoryAccountById(accountId);
                    if (accountResult.HasError)
                    {
                        throw new InvalidOperationException("При исключении пациента из счета ID {0} данные счета не загружены".F(accountId));
                    }

                    if (accountResult.Data.Type == AccountType.GeneralPart.ToInt32()) {
                        var enumeratePatientsResult = EnumeratePatientsOfTerritoryAccountTransact(accountId, db);
                        if (enumeratePatientsResult.HasError)
                        {
                            result.AddError(enumeratePatientsResult.LastError);
                            db.RollbackTransaction();
                        }
                        var enumerateEventResult = EnumerateEventsOfTerritoryAccountTransact(accountId, db);
                        if (enumerateEventResult.HasError)
                        {
                            result.AddError(enumerateEventResult.LastError);
                            db.RollbackTransaction();
                        }
                    }

                    var updateAccount = UpdateTerritoryAccount(db, accountId);
                    if (updateAccount.HasError)
                    {
                        result.AddError(updateAccount.LastError);
                        db.RollbackTransaction();
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult CreateOrUpdateExchange(FactExchange data)
        {
            var result = new TransactionResult();

            using (var db = CreateContext())
            {
                try
                {
                    var exist = db.GetTableQuery<FactExchange>().FirstOrDefault(p => p.AccountId == data.AccountId && p.Type == data.Type);
                    if (exist != null)
                    {
                        var updateResult = db.FactExchange
                            .Where(e => e.ExchangeId == exist.ExchangeId)
                            .Set(e => e.ActionDate, data.ActionDate)
                            .Set(e => e.Data, data.Data)
                            .Set(e => e.FileName, data.FileName)
                            .Set(e => e.Source, data.Source)
                            .Set(e => e.Destination, data.Destination)
                            .Set(e => e.RecordCounts, data.RecordCounts)
                            .Set(e => e.Date, data.Date)
                            .Update();

                        if (!Convert.ToBoolean(updateResult))
                        {
                            result.AddError(new InvalidOperationException("Ошибка обновления записи в журнале информационного обмена {0}".F(db.LastError)));
                        }
                    }
                    else
                    {
                        switch (data.Type)
                        {
                            case 0:
                            case 1:
                            case 2:
                            case 3:
                                var accountUpdateResult = db.FactTerritoryAccount
                                    .Where(e => e.TerritoryAccountId == data.AccountId)
                                    .Set(e => e.PacketNumber, data.PacketNumber)
                                    .Update();

                                if (!Convert.ToBoolean(accountUpdateResult))
                                {
                                    result.AddError(new InvalidOperationException("Ошибка записи номера пакета в запись счета {0}".F(db.LastError)));
                                }
                                else
                                {
                                    var insertResult = db.InsertWithIdentity(data);
                                    if (!Convert.ToBoolean(insertResult))
                                    {
                                        result.AddError(new InvalidOperationException("Ошибка записи в журнал информационного обмена {0}".F(db.LastError)));
                                    }
                                }
                                break;
                            case 5:
                            case 6:
                                var insert6Result = db.InsertWithIdentity(data);
                                if (!Convert.ToBoolean(insert6Result))
                                {
                                    result.AddError(new InvalidOperationException("Ошибка записи в журнал информационного обмена {0}".F(db.LastError)));
                                }
                                break;
                        }
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }

            return result;
        }

        public TransactionResult<IEnumerable<FactPatient>> GetPatientsByAccountId(int id)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<IEnumerable<FactPatient>>();
                try
                {
                    result.Data = FactPatientsByAccountIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<FactPerson> GetPersonById(int id)
        { 
            var result = new TransactionResult<FactPerson>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = PersonByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<FactDocument> GetDocumentById(int id)
        {
            var result = new TransactionResult<FactDocument>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = DocumentByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<FactDocument> GetDocumentByPersonId(int id)
        {
            var result = new TransactionResult<FactDocument>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactDocument.Where(p=>p.PersonId == id && p.DocType.HasValue).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactMedicalEvent>> GetMeventsByPatientId(int patientId)
        {
            var result = new TransactionResult<IEnumerable<FactMedicalEvent>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = MeventsByPatientIdQuery(db, patientId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZslFactMedicalEvent>> GetZslMeventsByPatientId(int patientId)
        {
            var result = new TransactionResult<IEnumerable<ZslFactMedicalEvent>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZslMeventsByPatientIdQuery(db, patientId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactMedicalEvent>> GetZMeventsByZslMeventId(int zslMeventId)
        {
            var result = new TransactionResult<IEnumerable<ZFactMedicalEvent>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZMeventsByZmeventIdQuery(db, zslMeventId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactMedicalServices>> GetServicesByMeventId(int medicalEventId)
        {
            var result = new TransactionResult<IEnumerable<FactMedicalServices>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ServicesByMeventIdQuery(db, medicalEventId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }
        public TransactionResult<IEnumerable<ZFactMedicalServices>> GetZServicesByZMeventId(int zmedicalEventId)
        {
            var result = new TransactionResult<IEnumerable<ZFactMedicalServices>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZServicesByZMeventIdQuery(db, zmedicalEventId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<ZFactKsgKpg> GetZKsgKpgByZMeventId(int zmedicalEventId)
        {
            var result = new TransactionResult<ZFactKsgKpg>();
            using (var db = CreateContext())
            {
                try
                {
                    var queryKsgKpg = db.ZFactKsgKpg.Where(p=>p.ZmedicalEventId == zmedicalEventId);
                    if (queryKsgKpg.Any())
                    {
                        result.Data = queryKsgKpg.FirstOrDefault();
                    }
                    else
                    {
                        throw new Exception("Нет данных для выбора");
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<ZFactMedicalEventOnk> GetZMedicalEventOnkByZMeventId(int zmedicalEventId)
        {
            var result = new TransactionResult<ZFactMedicalEventOnk>();
            using (var db = CreateContext())
            {
                try
                {
                    var queryMedicalEventOnk = db.ZFactMedicalEventOnk.Where(p => p.ZmedicalEventId == zmedicalEventId);
                    if (queryMedicalEventOnk.Any())
                    {
                        result.Data = queryMedicalEventOnk.FirstOrDefault();
                    }
                    else
                    {
                        throw new Exception("Нет данных для выбора");
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactSlKoef>> GetSlKoefByKsgKpgId(int ksgKpgId)
        {
            var result = new TransactionResult<IEnumerable<ZFactSlKoef>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = SlKoefByKsgKpgIdQuery(db, ksgKpgId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactCrit>> GetCritByksgKpgId(int ksgKpgId)
        {
            var result = new TransactionResult<IEnumerable<ZFactCrit>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = CritByIdQuery(db, ksgKpgId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactDirection>> GetDirectionByMedicalServicesId(int medicalServicesId)
        {
            var result = new TransactionResult<IEnumerable<ZFactDirection>>();
            using (var db = CreateContext())
            {
                try
                {
                   // result.Data = db.ZFactDirection.Where(x=>x.ZmedicalServicesId == medicalServicesId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactDiagBlok>> GetDiafBlokByMedicalEventOnkId(int medicalEventOnkId)
        {
            var result = new TransactionResult<IEnumerable<ZFactDiagBlok>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = DiagBlokByMedicalEventOnkIdQuery(db, medicalEventOnkId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactDs>> GetDsByMedicalEventId(int medicalEventId)
        {
            var result = new TransactionResult<IEnumerable<ZFactDs>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = DsByMedicalEventIdQuery(db, medicalEventId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactAnticancerDrug>> GetAnticancerDrugByMedicalServiceOnkId(int medicalServiceOnkId)
        {
            var result = new TransactionResult<IEnumerable<ZFactAnticancerDrug>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = AnticancerDrugByMedicalServiceOnkIdQuery(db, medicalServiceOnkId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactContraindications>> GetContraindicationsByMedicalEventOnkId(int medicalEventOnkId)
        {
            var result = new TransactionResult<IEnumerable<ZFactContraindications>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ContraindicationsByMedicalEventOnkIdQuery(db, medicalEventOnkId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }


        public TransactionResult<IEnumerable<FactMEC>> GetMecByMedicalEventId(int medicalEventId)
        {
            var result = new TransactionResult<IEnumerable<FactMEC>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = MecByMedicalEventIdQuery(db, medicalEventId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactSank>> GetSankByMedicalEventId(int medicalEventId)
        {
            var result = new TransactionResult<IEnumerable<ZFactSank>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = SankByMedicalEventIdQuery(db, medicalEventId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactSank>> GetSankByZMedicalEventIdAndType(Expression<Func<ZFactSank, bool>> predicate = null)
        {
            var result = new TransactionResult<IEnumerable<ZFactSank>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = predicate == null ? db.ZFactSank.ToList() : db.ZFactSank.Where(predicate).ToList();

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactMEC>> GetMecByMedicalEventIdAndType(int id, int type)
        {
            var result = new TransactionResult<IEnumerable<FactMEC>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = MecByMedicalEventIdAndTypeQuery(db, id, type).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactMEE>> GetMeeByMedicalEventId(int medicalEventId)
        {
            var result = new TransactionResult<IEnumerable<FactMEE>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = MeeByMedicalEventIdQuery(db, medicalEventId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactEQMA>> GetEqmaByMedicalEventId(int medicalEventId)
        {
            var result = new TransactionResult<IEnumerable<FactEQMA>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = EqmaByMedicalEventIdQuery(db, medicalEventId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<int?> GetFactExchangeLastPacketNumber(Expression<Func<FactExchange, bool>> predicate = null)
        {
            using (var db = CreateContext())
            {
                var result = new TransactionResult<int?>();
                try
                {
                    result.Data = predicate == null ?
                        FactExchangeQuery(db).Max(p => (int?)p.PacketNumber) :
                        db.FactExchange.Where(predicate).Max(p => (int?)p.PacketNumber);

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<IEnumerable<FactExternalRefuse>> GetExternalRefusesByAccountId(int id)
        {
            var result = new TransactionResult<IEnumerable<FactExternalRefuse>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ExternalRefusesByAccountIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactExternalRefuse>> GetZExternalRefusesByAccountId(int id)
        {
            var result = new TransactionResult<IEnumerable<ZFactExternalRefuse>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZExternalRefusesByAccountIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactExternalRefuse>> GetExternalRefuseByMedicalEventId(int id)
        {
            var result = new TransactionResult<IEnumerable<FactExternalRefuse>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ExternalRefuseByMedicalEventIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactExternalRefuse>> GetZExternalRefuseByMedicalEventId(int id)
        {
            var result = new TransactionResult<IEnumerable<ZFactExternalRefuse>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZexternalRefuseByMedicalEventIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactExternalRefuse>> GetZExternalIsAgreeRefuseByMedicalEventId(int id)
        {
            var result = new TransactionResult<IEnumerable<ZFactExternalRefuse>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZexternalRefuseIsAgreeByMedicalEventIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactExternalRefuse>> GetZExternalRefuseByZslMedicalEventId(int id)
        {
            var result = new TransactionResult<IEnumerable<ZFactExternalRefuse>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZFactExternalRefuse.Where(n=>n.ZslMedicalEventId == id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<FactTerritoryAccount> GetParentTerritoryAccountByParentId(int? parent)
        {
            var result = new TransactionResult<FactTerritoryAccount>();
            if (!parent.HasValue)
            {
                return result;
            }

            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ParentTerritoryAccountByParentIdQuery(db, parent.Value).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactExternalRefuse>> GetExternalRefuseByPatientId(int id)
        {
            var result = new TransactionResult<IEnumerable<FactExternalRefuse>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ExternalRefuseByPatientIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactExternalRefuse>> GetZExternalRefuseByPatientId(int id)
        {
            var result = new TransactionResult<IEnumerable<ZFactExternalRefuse>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = ZExternalRefuseByPatientIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<globalMedicalAssistanceVolume>> GetMedicalAssistanceVolume()
        {
            var result = new TransactionResult<IEnumerable<globalMedicalAssistanceVolume>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = MedicalAssistanceVolumeQuery(db).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteExchange(int id)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                
                try
                {
                    var deleteResult = db.FactExchange.Delete(p => p.ExchangeId == id);
                    if (!Convert.ToBoolean(deleteResult))
                    {
                        result.AddError(db.LastError);
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
                return result;
            }
        }

        public TransactionResult<FactTerritoryAccount> GetTerritoryAccountByPatientId(int id)
        {
            var result = new TransactionResult<FactTerritoryAccount>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = TerritoryAccountByPatientIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<TerritoryAccountView> GetTerritoryAccountViewById(int id)
        {
            var result = new TransactionResult<TerritoryAccountView>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = TerritoryAccountViewByIdQuery(db, id).FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<localUserSettings>> GetUserSettings(int id)
        {
            var result = new TransactionResult<IEnumerable<localUserSettings>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = UserSettingsByUserIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<int> DeleteActAxpertise(int actAxpertiseId)
        {
            var result = new TransactionResult<int>();
            try
            {
                using (var db = CreateContext())
                {
                    //Удаляем акт экспертизы
                    result.Data = db.GetTableQuery<FactActExpertise>().Delete(
                        p => p.ActExpertiseId == actAxpertiseId);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult<int> DeleteEconomicPayment(int economicAccountId)
        {
            var result = new TransactionResult<int>();
            try
            {
                using (var db = CreateContext())
                {
                    //Удаляем суммы оплаты
                    result.Data = db.GetTableQuery<FactEconomicPayment>().Delete(
                        p => p.EconomicAccountId == economicAccountId);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult DeleteEconomicRefuse(int economicRefuseId)
        {
            var result = new TransactionResult();
            try
            {
                using (var db = CreateContext())
                {
                    db.BeginTransaction();
                    var deleteDetailsResult = db.FactEconomicRefuseDetail.Delete(
                        p => p.EconomicRefuseId == economicRefuseId);
                    if (!Convert.ToBoolean(deleteDetailsResult))
                    {
                        result.AddError(new Exception("Last query return 0\r\n{0}".F(db.LastQuery)));
                        db.RollbackTransaction();
                        return result;
                    }

                    var deleteResult = db.FactEconomicRefuse.Delete(
                        p => p.EconomicRefuseId == economicRefuseId);
                    if (!Convert.ToBoolean(deleteResult))
                    {
                        result.AddError(new Exception("Last query return 0\r\n{0}".F(db.LastQuery)));
                        db.RollbackTransaction();
                        return result;
                    }
                    db.CommitTransaction();
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public TransactionResult UpdateTerritoryAccountPacketNumber(int accountId, int packetNumber)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    var updateResult = db.GetTableQuery<FactTerritoryAccount>().Where(s => s.TerritoryAccountId == accountId)
                             .Set(s => s.PacketNumber, packetNumber)
                             .Update();
                    if (!Convert.ToBoolean(updateResult))
                    {
                        throw new InvalidOperationException("Ошибка обновления номера пакета {0} для территориального счета ID {1}.\r\n{2}".F(packetNumber, accountId, db.LastQuery));
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<int> GetTerritoryAccountGenerationByParentId(int accountId)
        {
            var result = new TransactionResult<int>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactTerritoryAccount
                        .Count(p => p.Parent == accountId && p.Type == (int)AccountType.CorrectedPart) + 1;
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
            
        }

        public TransactionResult<IEnumerable<FactPatient>> GetPatientByMeventExternalIdAndAccountId(int meventExternalId, int accountId)
        {
            var result = new TransactionResult<IEnumerable<FactPatient>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactMedicalEvent.Where(p =>
                               p.ExternalId == meventExternalId &&
                               p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId).Select(p => p.FACTMEDIPATIENTIDFACTPATI).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactPatient>> GetPatientByZslMeventExternalIdAndAccountId(int meventExternalId, int accountId)
        {
            var result = new TransactionResult<IEnumerable<FactPatient>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZslFactMedicalEvent.Where(p =>
                               p.ExternalId == meventExternalId &&
                               p.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == accountId).Select(p => p.ZSLFACTMEDIPATIENTIDFACTPATI).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<FactPatient> GetPatientByExternalIdAndAccountId(int? externalId, int accountId)
        {
            var result = new TransactionResult<FactPatient>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactPatient.FirstOrDefault(p =>
                        p.AccountId == accountId &&
                        p.ExternalId == externalId);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult SetMedicalAsFullPaymentByExternalIdAndAccountId(int externalId, int accountId)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    var updateResult = db.FactMedicalEvent.Where(p => p.ExternalId == externalId &&
                                                                   p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId)
                                                                   .Update(
                                                                       p => new FactMedicalEvent { PaymentStatus = 2, AcceptPrice = p.Price });
                    if (!Convert.ToBoolean(updateResult))
                    {
                        throw new InvalidOperationException("Ошибка обновления статуса оплаты случая МП ExternalID {0}.\r\n{1}".F(externalId, db.LastQuery));
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult SetZMedicalAsFullPaymentByExternalIdAndAccountId(int externalId, int accountId)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    var updateResult = db.ZslFactMedicalEvent.Where(p => p.ExternalId == externalId &&
                                                                   p.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == accountId)
                                                                   .Update(
                                                                       p => new ZslFactMedicalEvent { PaymentStatus = 2, AcceptPrice = p.Price });
                    if (!Convert.ToBoolean(updateResult))
                    {
                        throw new InvalidOperationException("Ошибка обновления статуса оплаты случая МП ExternalID {0}.\r\n{1}".F(externalId, db.LastQuery));
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<FactMedicalEvent> GetEventByExternalIdAndAccountId(int externalId, int accountId)
        {
            var result = new TransactionResult<FactMedicalEvent>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactMedicalEvent
                        .FirstOrDefault(p => 
                            p.ExternalId == externalId &&
                            p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<ZslFactMedicalEvent> GetZslEventByExternalIdAndAccountId(int externalId, int accountId)
        {
            var result = new TransactionResult<ZslFactMedicalEvent>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZslFactMedicalEvent
                        .FirstOrDefault(p =>
                            p.ExternalId == externalId &&
                            p.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == accountId);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<ZFactMedicalEvent> GetZEventByExternalIdAndAccountId(string externalId, int zslMedicalEventId, int accountId)
        {
            var result = new TransactionResult<ZFactMedicalEvent>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZFactMedicalEvent
                        .FirstOrDefault(p =>
                            p.ExternalId == externalId &&
                            p.ZslMedicalEventId == zslMedicalEventId &&
                            p.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == accountId);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<ZFactMedicalEvent> GetZEventBySlidGuidAndAccountId(string slidGuid, int zslMedicalEventId, int accountId)
        {
            var result = new TransactionResult<ZFactMedicalEvent>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.ZFactMedicalEvent
                        .FirstOrDefault(p =>
                            p.SlIdGuid == slidGuid &&
                            p.ZslMedicalEventId == zslMedicalEventId &&
                            p.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == accountId);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactMedicalServices>> GetMedicalService(Expression<Func<FactMedicalServices, bool>> predicate = null)
        {
            var result = new TransactionResult<IEnumerable<FactMedicalServices>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = predicate == null ? db.FactMedicalServices.ToList() : db.FactMedicalServices.Where(predicate).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<ZFactMedicalServices>> GetMedicalZService(Expression<Func<ZFactMedicalServices, bool>> predicate = null)
        {
            var result = new TransactionResult<IEnumerable<ZFactMedicalServices>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = predicate == null ? db.ZFactMedicalServices.ToList() : db.ZFactMedicalServices.Where(predicate).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult InsertRegisterEAnswer(FactTerritoryAccount account, List<Tuple<FactPatient, List<Tuple<FactMedicalEvent, List<FactMedicalServices>, List<FactExternalRefuse>>>>> data, bool isTestLoad)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                db.Command.CommandTimeout = 0;
                db.BeginTransaction();
                try
                {
                    var accountId = SafeConvert.ToInt32(db.InsertWithIdentity(account).ToString(), false);
                    if (!accountId.HasValue)
                    {
                        result.AddError(string.Format("Ошибка при записи счета от территории в базу данных"));
                        db.RollbackTransaction();
                        return result;
                    }

                    db.GetTableQuery<FactTerritoryAccount>().Where(s => s.TerritoryAccountId == accountId)
                        .Set(s => s.ExternalId, accountId)
                        .Update();

                    result.Id = accountId.Value;

                    foreach (var p in data)
                    {
                        var patient = p.Item1;
                        patient.AccountId = accountId;
                        
                        var insertPatientResult = db.InsertWithIdentity(patient);
                        var patientId = Convert.ToInt32(insertPatientResult);
                        if (patientId == 0)
                        {
                            result.AddError(string.Format("Ошибка при записи данных пациента в базу данных"));
                            db.RollbackTransaction();
                            return result;
                        }

                        var mevents = p.Item2;
                        foreach (var m in mevents)
                        {
                            var mevent = m.Item1;
                            mevent.PatientId = patientId;
                            var meventInsertResult = db.InsertWithIdentity(mevent);
                            var meventId = Convert.ToInt32(meventInsertResult);
                            if (meventId == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи случая в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }

                            var services = m.Item2.ToList();
                            if (services.Any())
                            {
                                services.ForEach(s => s.MedicalEventId = meventId);
                                var insertServicesResult = db.InsertBatch(services);
                                if (Convert.ToInt32(insertServicesResult) == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи услуг в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }
                            }

                            var refusals = m.Item3.ToList();
                            if (refusals.Any())
                            {
                                refusals.ForEach(s =>
                                {
                                    s.MedicalEventId = meventId;
                                    s.PatientId = patientId;
                                });
                                var insertRefusalsResult = db.InsertBatch(refusals);
                                if (Convert.ToInt32(insertRefusalsResult) == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи отказов от территории в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }
                            }
                        }
                    }

                    var updateResult = UpdateTerritoryAccount(db, accountId);
                    if (updateResult.HasError)
                    {
                        result.AddError(string.Format("Ошибка при обновлении счета от территории"));
                        db.RollbackTransaction();
                        return result;
                    }

                    if (isTestLoad)
                    {
                        db.RollbackTransaction();
                    }
                    else
                    {
                        db.CommitTransaction();
                    }
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }

            }
            return result;
        }

        public TransactionResult InsertRegisterEAnswer(FactTerritoryAccount account, List<Tuple<FactPatient, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>>> data, bool isTestLoad)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                db.Command.CommandTimeout = 0;
                db.BeginTransaction();
                try
                {
                    var accountId = SafeConvert.ToInt32(db.InsertWithIdentity(account).ToString(), false);
                    if (!accountId.HasValue)
                    {
                        result.AddError(string.Format("Ошибка при записи счета от территории в базу данных"));
                        db.RollbackTransaction();
                        return result;
                    }

                    db.GetTableQuery<FactTerritoryAccount>().Where(s => s.TerritoryAccountId == accountId)
                        .Set(s => s.ExternalId, accountId)
                        .Update();

                    result.Id = accountId.Value;

                    foreach (var p in data)
                    {
                        var patient = p.Item1;
                        patient.AccountId = accountId;

                        var insertPatientResult = db.InsertWithIdentity(patient);
                        var patientId = Convert.ToInt32(insertPatientResult);
                        if (patientId == 0)
                        {
                            result.AddError(string.Format("Ошибка при записи данных пациента в базу данных"));
                            db.RollbackTransaction();
                            return result;
                        }

                        var zslmevents = p.Item2;
                        foreach (var zslm in zslmevents)
                        {
                            var zslmevent = zslm.Item1;
                            zslmevent.PatientId = patientId;
                            var zslmeventInsertResult = db.InsertWithIdentity(zslmevent);
                            var zslMeventId = Convert.ToInt32(zslmeventInsertResult);
                            if (zslMeventId == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи законченного случая в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            var mevents = zslm.Item2;
                            foreach (var m in mevents)
                            {
                                var mevent = m.Item1;
                                mevent.ZslMedicalEventId = zslMeventId;
                                var meventInsertResult = db.InsertWithIdentity(mevent);
                                var meventId = Convert.ToInt32(meventInsertResult);
                                if (meventId == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи случая в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }

                                var ksgKpg = m.Item2.Item1;
                                if (ksgKpg != null)
                                {
                                    ksgKpg.ZmedicalEventId = meventId;
                                    var insertKsfKpgResult = db.InsertWithIdentity(ksgKpg);
                                    var zksgKpgId = Convert.ToInt32(insertKsfKpgResult);
                                    if (zksgKpgId == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи КСГ в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }

                                    var slKoefs = m.Item2.Item2;
                                    if (slKoefs.Any())
                                    {
                                        slKoefs.ForEach(s => s.ZksgKpgId = zksgKpgId);
                                        var insertslKoefsResult = db.InsertBatch(slKoefs);
                                        if (Convert.ToInt32(insertslKoefsResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи Коэффициента сложности лечения пациента в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }
                                }

                                var services = m.Item3.ToList();
                                if (services.Any())
                                {
                                    services.ForEach(s => s.ZmedicalEventId = meventId);
                                    var insertServicesResult = db.InsertBatch(services);
                                    if (Convert.ToInt32(insertServicesResult) == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи услуг в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }
                                }

                                var refusals = m.Item4.ToList();
                                if (refusals.Any())
                                {
                                    refusals.ForEach(s =>
                                    {
                                        s.ZmedicalEventId = meventId;
                                        s.PatientId = patientId;
                                        s.ZslMedicalEventId = zslMeventId;
                                        s.ZslAmount = zslmevent.Price;
                                    });
                                    var insertRefusalsResult = db.InsertBatch(refusals);
                                    if (Convert.ToInt32(insertRefusalsResult) == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи отказов от территории в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }
                                }
                            }
                            
                        }
                    }

                    var updateResult = UpdateZTerritoryAccount(db, accountId);
                    if (updateResult.HasError)
                    {
                        result.AddError(string.Format("Ошибка при обновлении счета от территории"));
                        db.RollbackTransaction();
                        return result;
                    }

                    if (isTestLoad)
                    {
                        db.RollbackTransaction();
                    }
                    else
                    {
                        db.CommitTransaction();
                    }
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }

            }
            return result;
        }

        public TransactionResult InsertRegisterEAnswer(FactTerritoryAccount account, List<Tuple<FactPatient,
                    List<Tuple<ZslFactMedicalEvent,
                        List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            List<ZFactDs>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
                        List<ZFactExternalRefuse>
                            >>>> data, bool isTestLoad)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                db.Command.CommandTimeout = 0;
                db.BeginTransaction();
                try
                {
                    var accountId = SafeConvert.ToInt32(db.InsertWithIdentity(account).ToString(), false);
                    if (!accountId.HasValue)
                    {
                        result.AddError(string.Format("Ошибка при записи счета от территории в базу данных"));
                        db.RollbackTransaction();
                        return result;
                    }

                    db.GetTableQuery<FactTerritoryAccount>().Where(s => s.TerritoryAccountId == accountId)
                        .Set(s => s.ExternalId, accountId)
                        .Update();

                    result.Id = accountId.Value;

                    foreach (var p in data)
                    {
                        var patient = p.Item1;
                        patient.AccountId = accountId;

                        var insertPatientResult = db.InsertWithIdentity(patient);
                        var patientId = Convert.ToInt32(insertPatientResult);
                        if (patientId == 0)
                        {
                            result.AddError(string.Format("Ошибка при записи данных пациента в базу данных"));
                            db.RollbackTransaction();
                            return result;
                        }

                        var zslmevents = p.Item2;

                        foreach (var zslmevent in zslmevents)
                        {
                            var mevent = zslmevent.Item1;
                            mevent.PatientId = patientId;
                            var zslmeventInsertResult = db.InsertWithIdentity(mevent);
                            var zslmeventId = Convert.ToInt32(zslmeventInsertResult);
                            if (zslmeventId == 0)
                            {
                                result.AddError(string.Format("Ошибка при записи законченного случая в базу данных"));
                                db.RollbackTransaction();
                                return result;
                            }
                            var refuz = new List<ZFactExternalRefuse>();
                            foreach (var zmevent in zslmevent.Item2)
                            {
                                var me = zmevent.Item1;
                                me.ZslMedicalEventId = zslmeventId;
                                var zmeventInsertResult = db.InsertWithIdentity(me);
                                var meventId = Convert.ToInt32(zmeventInsertResult);
                                if (meventId == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи случая в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }

                                var directions = zmevent.Item2;
                                if (directions.Any())
                                {
                                    directions.ForEach(s =>
                                    {
                                        s.ZmedicalEventId = meventId;
                                    });
                                    var insertdirectionResult = db.InsertBatch(directions);
                                    if (Convert.ToInt32(insertdirectionResult) == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи направления в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }
                                }
                                var сonsultations = zmevent.Item3;
                                if (сonsultations.Any())
                                {
                                    сonsultations.ForEach(s =>
                                    {
                                        s.ZMedicalEventId = meventId;
                                    });
                                    var insertсonsultationsResult = db.InsertBatch(сonsultations);
                                    if (Convert.ToInt32(insertсonsultationsResult) == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи консилиума в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }
                                }

                                var ds2 = zmevent.Item4;
                                if (ds2.Any())
                                {
                                    ds2.ForEach(s =>
                                    {
                                        s.ZmedicalEventId = meventId;
                                    });
                                    var insertds2Result = db.InsertBatch(ds2);
                                    if (Convert.ToInt32(insertds2Result) == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи сопутствующего диагноза в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }
                                }
                                var medicalEventOnk = zmevent.Item5.Item1;
                                if (medicalEventOnk != null)
                                {
                                    ControlResourcesLoger.LogDedug(zmevent.Item1.SlIdGuid + " - " + zmevent.Item1.ExternalId);
                                    medicalEventOnk.ZmedicalEventId = meventId;
                                    var insertmedicalEventOnkResult = db.InsertWithIdentity(medicalEventOnk);
                                    var zmedicalEventOnkId = Convert.ToInt32(insertmedicalEventOnkResult);
                                    if (zmedicalEventOnkId == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи сведения о случае лечения онкологического заболевания в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }

                                    var diagBloks = zmevent.Item5.Item2;
                                    if (diagBloks.Any())
                                    {
                                        diagBloks.ForEach(s => s.ZMedicalEventOnkId = zmedicalEventOnkId);
                                        var insertdiagBloksResult = db.InsertBatch(diagBloks);
                                        if (Convert.ToInt32(insertdiagBloksResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи диагностического блока в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }
                                    var сontraindications = zmevent.Item5.Item3;
                                    if (сontraindications.Any())
                                    {
                                        сontraindications.ForEach(s => s.ZMedicalEventOnkId = zmedicalEventOnkId);
                                        var insertсontraindicationsResult = db.InsertBatch(сontraindications);
                                        if (Convert.ToInt32(insertсontraindicationsResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи сведений об имеющихся противопоказаниях и отказах в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }
                                    var zFactMedicalServiceOnk = zmevent.Item5.Item4;
                                    if (zFactMedicalServiceOnk.Any())
                                    {
                                        foreach (var usl in zFactMedicalServiceOnk)
                                        {
                                            var zFactmedicalServiceOnk = usl.Item1;
                                            zFactmedicalServiceOnk.ZmedicalEventOnkId = zmedicalEventOnkId;
                                            var zFactmedicalServiceOnkResult = db.InsertWithIdentity(zFactmedicalServiceOnk);
                                            var zFactmedicalServiceOnkResultId = Convert.ToInt32(zFactmedicalServiceOnkResult);
                                            if (zFactmedicalServiceOnkResultId == 0)
                                            {
                                                result.AddError(string.Format("Ошибка при записи онко услуги в базу данных"));
                                                db.RollbackTransaction();
                                                return result;
                                            }

                                            var zfactAnticancerDrugs = usl.Item2;
                                            if (zfactAnticancerDrugs.Any())
                                            {
                                                zfactAnticancerDrugs.ForEach(s => s.ZMedicalServiceOnkId = zFactmedicalServiceOnkResultId);
                                                var insertAnticancerDrugsResult = db.InsertBatch(zfactAnticancerDrugs);
                                                if (Convert.ToInt32(insertAnticancerDrugsResult) == 0)
                                                {
                                                    result.AddError(string.Format("Ошибка при записи cведения о введенном противоопухолевом лекарственном препарате направления в базу данных"));
                                                    db.RollbackTransaction();
                                                    return result;
                                                }
                                            }
                                        }
                                    }
                                }

                                var ksgKpg = zmevent.Item6.Item1;
                                if (ksgKpg != null)
                                {
                                    ksgKpg.ZmedicalEventId = meventId;
                                    var insertKsfKpgResult = db.InsertWithIdentity(ksgKpg);
                                    var zksgKpgId = Convert.ToInt32(insertKsfKpgResult);
                                    if (zksgKpgId == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи КСГ в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }

                                    var slcrit = zmevent.Item6.Item2;
                                    if (slcrit.Any())
                                    {
                                        slcrit.ForEach(s => s.ZksgKpgId = zksgKpgId);
                                        var insertslKoefsResult = db.InsertBatch(slcrit);
                                        if (Convert.ToInt32(insertslKoefsResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи slcrit в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }

                                    var slKoefs = zmevent.Item6.Item3;
                                    if (slKoefs.Any())
                                    {
                                        slKoefs.ForEach(s => s.ZksgKpgId = zksgKpgId);
                                        var insertslKoefsResult = db.InsertBatch(slKoefs);
                                        if (Convert.ToInt32(insertslKoefsResult) == 0)
                                        {
                                            result.AddError(string.Format("Ошибка при записи Коэффициента сложности лечения пациента в базу данных"));
                                            db.RollbackTransaction();
                                            return result;
                                        }
                                    }
                                }

                                var services = zmevent.Item7;
                                if (services.Any())
                                {
                                    services.ForEach(s => s.ZmedicalEventId = meventId);
                                    var insertServicesResult = db.InsertBatch(services);
                                    if (Convert.ToInt32(insertServicesResult) == 0)
                                    {
                                        result.AddError(string.Format("Ошибка при записи услуг в базу данных"));
                                        db.RollbackTransaction();
                                        return result;
                                    }
                                }
                                refuz.Add(new ZFactExternalRefuse { ZslMedicalEventId = zslmeventId, ZmedicalEventId = meventId, SlidGuid = me.SlIdGuid });
                            }
                            var refusals = zslmevent.Item3.ToList();
                            if (refusals.Any())
                            {
                                refusals.ForEach(s =>
                                {
                                    s.ZslMedicalEventId = zslmeventId;
                                    s.ZslAmount = mevent.Price;
                                    s.PatientId = patientId;
                                });
                                var insertRefusalsResult = db.InsertBatch(refusals);
                                if (Convert.ToInt32(insertRefusalsResult) == 0)
                                {
                                    result.AddError(string.Format("Ошибка при записи отказов от территории в базу данных"));
                                    db.RollbackTransaction();
                                    return result;
                                }
                            }
                            foreach (var tuple in refuz)
                            {
                                db.GetTableQuery<ZFactExternalRefuse>().Where(s => s.ZslMedicalEventId == tuple.ZslMedicalEventId && s.SlidGuid == tuple.SlidGuid)
                                .Set(s => s.ZmedicalEventId, tuple.ZmedicalEventId).Update();
                            }
                        }
                    }

                    var updateResult = UpdateZTerritoryAccount(db, accountId);
                    if (updateResult.HasError)
                    {
                        result.AddError(string.Format("Ошибка при обновлении счета от территории"));
                        db.RollbackTransaction();
                        return result;
                    }

                    if (isTestLoad)
                    {
                        db.RollbackTransaction();
                    }
                    else
                    {
                        db.CommitTransaction();
                    }
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }

            }
            return result;
        }

        public TransactionResult SetMedicalEventPaymentStatusByExternalIdAndAccountId(int externalId, int accountId, int? paymentStatus)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    var updateResult = db.FactMedicalEvent
                        .Where(p => p.ExternalId == externalId &&
                            p.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId)
                        .Update(p => new FactMedicalEvent { PaymentStatus = paymentStatus });

                    if (!Convert.ToBoolean(updateResult))
                    {
                        result.AddError(new InvalidOperationException(Constants.DbErrorCommonMsg.F("обновлении", "статуса оплаты", db.LastQuery)));
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        /// <summary>
        /// Обновляем статус законченного случая по внешнему ключу 
        /// </summary>
        /// <param name="externalId"></param>
        /// <param name="accountId"></param>
        /// <param name="paymentStatus"></param>
        /// <returns></returns>
        public TransactionResult SetZslMedicalEventPaymentStatusByExternalIdAndAccountId(int externalId, int accountId, int? paymentStatus)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    var updateResult = db.ZslFactMedicalEvent
                        .Where(p => p.ExternalId == externalId &&
                            p.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == accountId)
                        .Update(p => new ZslFactMedicalEvent { PaymentStatus = paymentStatus });

                    if (!Convert.ToBoolean(updateResult))
                    {
                        result.AddError(new InvalidOperationException(Constants.DbErrorCommonMsg.F("обновлении", "статуса оплаты", db.LastQuery)));
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="refusalCodeIds"></param>
        /// <returns></returns>
        public TransactionResult DeleteAllTerritorialMec(int accountId, IEnumerable<int?> refusalCodeIds)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    //Получаем все случаи с МЭК
                    var meventIds = db.FactMEC
                        .Where(p =>
                            p.MedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                            (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal) &&
                            (p.IsLock == null || p.IsLock == false) &&
                            refusalCodeIds.Contains(p.ReasonId))
                            .Select(p=>p.MedicalEventId).ToList();

                    if (meventIds.Any())
                    {
                        //Удаляем отказы
                        db.FactMEC
                        .Delete(p =>
                            p.MedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                            (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal) &&
                            (p.IsLock == null || p.IsLock == false) &&
                            refusalCodeIds.Contains(p.ReasonId));

                        //Обновляем случаи
                        foreach (var id in meventIds)
                        {
                            UpdateMedicalEventAsTerritorial(db, id);
                        }
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="refusalCodeIds"></param>
        /// <returns></returns>
        public TransactionResult DeleteAllTerritorialSank(int accountId, IEnumerable<int?> refusalCodeIds)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    //Получаем все случаи с МЭК
                    var meventIds = db.ZFactSank
                        .Where(p =>
                            p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                           // (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal) &&
                            (p.IsLock == null || p.IsLock == false) &&
                            p.Type == 1 &&
                            refusalCodeIds.Contains(p.ReasonId))
                            .Select(p => p.ZmedicalEventId).ToList();

                    if (meventIds.Any())
                    {
                        //Удаляем отказы
                        db.ZFactSank
                        .Delete(p =>
                            p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                           // (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal) &&
                            (p.IsLock == null || p.IsLock == false) &&
                            p.Type == 1 &&
                            refusalCodeIds.Contains(p.ReasonId));

                        //Обновляем случаи
                        foreach (var id in meventIds)
                        {
                            UpdateZMedicalEventAsTerritorial(db, id);
                        }

                        var zslMeventIds =
                            db.ZFactMedicalEvent.Where(x => meventIds.Contains(x.ZmedicalEventId))
                                .Select(y => y.ZslMedicalEventId).ToList();

                        foreach (var zslMeventId in zslMeventIds)
                        {
                            UpdateZslMedicalEventAsTerritorial(db, zslMeventId);
                        }
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="refusalCodeIds"></param>
        /// <param name="patientId"></param>
        /// <returns></returns>
        public TransactionResult DeletePatientTerritorialMec(int accountId, IEnumerable<int?> refusalCodeIds, int patientId)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                   
                    db.BeginTransaction();

                   
                        //Получаем все случаи с МЭК
                        var meventIds = db.FactMEC
                            .Where(p =>
                                p.MedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal) &&
                                (p.IsLock == null || p.IsLock == false) &&
                                p.PatientId == patientId &&
                                refusalCodeIds.Contains(p.ReasonId))
                                .Select(p => p.MedicalEventId).ToList();

                        if (meventIds.Any())
                        {
                            //Удаляем отказы
                            db.FactMEC
                            .Delete(p =>
                                p.MedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                                (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal) &&
                                (p.IsLock == null || p.IsLock == false) &&
                                p.PatientId == patientId &&
                                refusalCodeIds.Contains(p.ReasonId));

                            //Обновляем случаи
                            foreach (var id in meventIds)
                            {
                                UpdateMedicalEventAsTerritorial(db, id);
                            }

                            UpdateTerritoryAccount(db, accountId);
                        }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeletePatientTerritorialSank(int accountId, IEnumerable<int?> refusalCodeIds, int patientId)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    //Получаем все случаи с МЭК
                    var meventIds = db.ZFactSank
                        .Where(p =>
                            p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                          //  (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal) &&
                            (p.IsLock == null || p.IsLock == false) &&
                            p.Type == 1 &&
                            p.PatientId == patientId &&
                            refusalCodeIds.Contains(p.ReasonId))
                            .Select(p => p.ZmedicalEventId).ToList();

                    if (meventIds.Any())
                    {
                        //Удаляем отказы
                        db.ZFactSank
                        .Delete(p =>
                            p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                         //   (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal) &&
                            (p.IsLock == null || p.IsLock == false) &&
                            p.Type == 1 &&
                            p.PatientId == patientId &&
                            refusalCodeIds.Contains(p.ReasonId));

                        //Обновляем случаи
                        foreach (var id in meventIds)
                        {
                            UpdateZMedicalEventAsTerritorial(db, id);
                        }

                        var zslMeventIds =
                            db.ZFactMedicalEvent.Where(x => meventIds.Contains(x.ZmedicalEventId))
                                .Select(y => y.ZslMedicalEventId).ToList();

                        foreach (var zslMeventId in zslMeventIds)
                        {
                            UpdateZslMedicalEventAsTerritorial(db, zslMeventId);
                        }

                        UpdateZTerritoryAccount(db, accountId);
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;

        }

        public bool IsEqmaWithoutActExistsForTerritoryAccount(int accountId)
        {
            using (var db = CreateContext())
            {
                try
                {
                    return db.FactEQMA.Count(p =>
                        p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                        !p.ActId.HasValue &&
                        (p.Source == (int)RefusalSource.InterTerritorial ||
                         p.Source == (int)RefusalSource.InterTerritorialTotal)) > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsMeeWithoutActExistsForTerritoryAccount(int accountId)
        {
            using (var db = CreateContext())
            {
                try
                {
                    return db.FactMEE.Count(p =>
                        p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                        !p.ActId.HasValue &&
                        (p.Source == (int) RefusalSource.InterTerritorial ||
                         p.Source == (int) RefusalSource.InterTerritorialTotal)) > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsEqmaWithActExistsForTerritoryAccount(int accountId)
        {
            using (var db = CreateContext())
            {
                try
                {
                    return db.FactEQMA.Count(p =>
                        p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                        p.ActId.HasValue &&
                        (p.Source == (int)RefusalSource.InterTerritorial ||
                         p.Source == (int)RefusalSource.InterTerritorialTotal)) > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public TransactionResult<IEnumerable<FactActMee>> GetMeeActByTerritoryAccountId(int accountId)
        {
            var result = new TransactionResult<IEnumerable<FactActMee>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = MeeActByTerritoryAccountIdQuery(db, accountId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactActEqma>> GetEqmaActByTerritoryAccountId(int accountId)
        {
            var result = new TransactionResult<IEnumerable<FactActEqma>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = EqmaActByTerritoryAccountIdQuery(db, accountId).DistinctBy(p => p.ActEqma).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public bool IsMeeWithActExistsForTerritoryAccount(int accountId)
        {
            using (var db = CreateContext())
            {
                try
                {
                    return db.FactMEE.Count(p =>
                        p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                        p.ActId.HasValue &&
                        (p.Source == (int)RefusalSource.InterTerritorial ||
                         p.Source == (int)RefusalSource.InterTerritorialTotal)) > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public TransactionResult CreateTerritoryAccountAuto(string okato, int year, int month, int version)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    var existAccount =
                    db.GetTableQuery<FactTerritoryAccount>()
                        .FirstOrDefault(p => p.Destination == okato &&
                                                p.Source == TerritoryService.TerritoryOkato &&
                                                p.Type == 1 &&
                                                //проверка отчетного года
                                                p.Date.Value.Year == year &&
                                                //проверка отчетного месяца года
                                                p.Date.Value.Month == month);

                    if (existAccount == null)
                    {
                        int? packetNumber = db.GetTableQuery<FactTerritoryAccount>()
                            .Where(p => p.Destination == okato &&
                                        p.Source == TerritoryService.TerritoryOkato &&
                                        p.Type == 1)
                            .Max(p => p.PacketNumber) ?? 0;

                        int accountNumber = 1;

                        if (db.GetTableQuery<FactTerritoryAccount>()
                            .Any(p => p.Source == TerritoryService.TerritoryOkato &&
                                      p.Date.Value.Year == year &&
                                      p.Type == 1))
                        {
                            accountNumber = db.GetTableQuery<FactTerritoryAccount>()
                                .Where(p => p.Source == TerritoryService.TerritoryOkato &&
                                            p.Date.Value.Year == year &&
                                            p.Type == 1)
                                .Max(p => Convert.ToInt32(p.AccountNumber)) + 1;
                        }

                        var accountCreateResult = CreateTerritoryAccount(okato, year, month, (packetNumber.Value) + 1, accountNumber, version);
                        if (accountCreateResult.Success)
                        {
                            result.Id = accountCreateResult.Id;
                        }
                    }
                    else
                    {
                        result.AddError(new InvalidOperationException("Счет на территорию уже существует"));
                        result.Id = existAccount.TerritoryAccountId;
                    }

                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteMeeAct(int id)
        {
            //TODO delete prepared reports
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                db.BeginTransaction();
                try
                {
                    var updateResult = db.FactMEE
                            .Where(p => p.ActId == id)
                            .Set(s => s.ActId, default(int?))
                            .Update();

                    if (!Convert.ToBoolean(updateResult))
                    {
                        //Logger.Warn("При удалении акта МЭЭ не найдены привязанные к ActId отказы в FactMEE, ActId {0}", id);
                    }

                    var deleteResult = db.FactActMee.Delete(p => p.ActMeeId == id);
                    if (!Convert.ToBoolean(deleteResult))
                    {
                        throw new InvalidOperationException(Constants.DbErrorCommonMsg.F("удалении", "акта МЭЭ", db.LastQuery));

                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                    //Logger.LogException(LogLevel.Error, "Ошибка при удалении акта МЭЭ", exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteEqmaAct(int id)
        {
            //TODO delete prepared reports
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                db.BeginTransaction();
                try
                {
                    var updateResult = db.FactEQMA
                            .Where(p => p.ActId == id)
                            .Set(s => s.ActId, default(int?))
                            .Update();

                    if (!Convert.ToBoolean(updateResult))
                    {
                        //Logger.Warn("При удалении акта ЭКМП не найдены привязанные к ActId отказы в FactEQMA, ActId {0}", id);
                    }

                    var deleteResult = db.FactActEqma.Delete(p => p.ActEqma == id);
                    if (!Convert.ToBoolean(deleteResult))
                    {
                        throw new InvalidOperationException(Constants.DbErrorCommonMsg.F("удалении", "акта ЭКМП", db.LastQuery));
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<PatientShortView>> GetPatientsByAccountIdWithMeeWithAct(int accountId)
        {
            var result = new TransactionResult<IEnumerable<PatientShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    var patientQuery = db.FactMEE.Where(p =>
                        p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                        p.ActId.HasValue &&
                        (p.Source == (int)RefusalSource.InterTerritorial ||
                         p.Source == (int)RefusalSource.InterTerritorialTotal))
                        .Select(p => p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.PatientId);

                    result.Data = db.PatientShortView.Where(s => patientQuery.Contains(s.PatientId)).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeletePreparedReportByExternalIdAndScope(int externalId, int scope)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.FactPreparedReport.Delete(p => p.ExternalId == externalId && p.Scope == scope);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<PatientShortView>> GetPatientsByAccountIdWithMeeWithoutAct(int accountId)
        {
            var result = new TransactionResult<IEnumerable<PatientShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    var patientQuery = db.FactMEE.Where(p =>
                        p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                        !p.ActId.HasValue &&
                        (p.Source == (int) RefusalSource.InterTerritorial ||
                         p.Source == (int) RefusalSource.InterTerritorialTotal))
                        .Select(p => p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.PatientId);

                    result.Data = db.PatientShortView.Where(s => patientQuery.Contains(s.PatientId)).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<PatientShortView>> GetPatientsByMedicalAccountIdWithMeeWithoutAct(int medicalAccountId)
        {
            var result = new TransactionResult<IEnumerable<PatientShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    var patientQuery = db.FactMEE.Where(p =>
                        p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId &&
                        !p.ActId.HasValue &&
                        (p.Source == (int)RefusalSource.Local ||
                         p.Source == (int)RefusalSource.LocalCorrected))
                        .Select(p => p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.PatientId);

                    result.Data = db.PatientShortView.Where(s => patientQuery.Contains(s.PatientId)).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<PatientShortView>> GetPatientsByAccountIdWithEqmaWithoutAct(int accountId)
        {
            var result = new TransactionResult<IEnumerable<PatientShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    var patientQuery = db.FactEQMA.Where(p =>
                        p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.AccountId == accountId &&
                        !p.ActId.HasValue &&
                        (p.Source == (int)RefusalSource.InterTerritorial ||
                         p.Source == (int)RefusalSource.InterTerritorialTotal))
                        .Select(p => p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.PatientId);

                    result.Data = db.PatientShortView.Where(s => patientQuery.Contains(s.PatientId)).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<PatientShortView>> GetPatientsByMedicalAccountIdWithEqmaWithoutAct(int medicalAccountId)
        {
            var result = new TransactionResult<IEnumerable<PatientShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    var patientQuery = db.FactEQMA.Where(p =>
                        p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId &&
                        !p.ActId.HasValue &&
                        (p.Source == (int)RefusalSource.Local ||
                         p.Source == (int)RefusalSource.LocalCorrected))
                        .Select(p => p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.PatientId);

                    result.Data = db.PatientShortView.Where(s => patientQuery.Contains(s.PatientId)).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }


        public TransactionResult<IEnumerable<EventShortView>> GetMedicalEventByPatientIdWithMeeWithoutAct(int patientId, int scope)
        {
            var result = new TransactionResult<IEnumerable<EventShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    IQueryable<int> meventQuery = null;
                    switch (scope)
                    {
                        case Constants.ScopeLocalAccount:
                            meventQuery = db.FactMEE.Where(p =>
                                p.FACTMEEMEVENTID.PatientId == patientId &&
                                !p.ActId.HasValue &&
                                (p.Source == (int)RefusalSource.Local ||
                                 p.Source == (int)RefusalSource.LocalCorrected))
                                .Select(p => p.FACTMEEMEVENTID.MedicalEventId);
                            break;
                        case Constants.ScopeInterTerritorialAccount:
                            meventQuery = db.FactMEE.Where(p =>
                                p.FACTMEEMEVENTID.PatientId == patientId &&
                                !p.ActId.HasValue &&
                                (p.Source == (int)RefusalSource.InterTerritorial ||
                                 p.Source == (int)RefusalSource.InterTerritorialTotal))
                                .Select(p => p.FACTMEEMEVENTID.MedicalEventId);
                            break;
                        default:
                            throw new InvalidOperationException("Неверный scope {0}".F(scope));
                    }
                    

                    result.Data = db.EventShortView.Where(s => meventQuery.Contains(s.EventId)).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<EventShortView>> GetMedicalEventByPatientIdWithEqmaWithoutAct(int patientId, int scope)
        {
            var result = new TransactionResult<IEnumerable<EventShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    IQueryable<int> meventQuery = null;
                    switch (scope)
                    {
                        case Constants.ScopeLocalAccount:
                            meventQuery = db.FactEQMA.Where(p =>
                                p.FACTEQMAMEVENTID.PatientId == patientId &&
                                !p.ActId.HasValue &&
                                (p.Source == (int)RefusalSource.Local ||
                                 p.Source == (int)RefusalSource.LocalCorrected))
                                .Select(p => p.FACTEQMAMEVENTID.MedicalEventId);
                            break;
                        case Constants.ScopeInterTerritorialAccount:
                            meventQuery = db.FactEQMA.Where(p =>
                                p.FACTEQMAMEVENTID.PatientId == patientId &&
                                !p.ActId.HasValue &&
                                (p.Source == (int)RefusalSource.InterTerritorial ||
                                 p.Source == (int)RefusalSource.InterTerritorialTotal))
                                .Select(p => p.FACTEQMAMEVENTID.MedicalEventId);
                            break;
                        default:
                            throw new InvalidOperationException("Неверный scope {0}".F(scope));
                    }


                    result.Data = db.EventShortView.Where(s => meventQuery.Contains(s.EventId)).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<EventShortView>> GetMedicalEventByPatientIdWithMeeWithAct(int patientId, int scope)
        {
            var result = new TransactionResult<IEnumerable<EventShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    IQueryable<int> meventQuery = null;
                    switch (scope)
                    {
                        case Constants.ScopeLocalAccount:
                            meventQuery = db.FactMEE.Where(p =>
                                p.FACTMEEMEVENTID.PatientId == patientId &&
                                p.ActId.HasValue &&
                                (p.Source == (int)RefusalSource.Local ||
                                 p.Source == (int)RefusalSource.LocalCorrected))
                                .Select(p => p.FACTMEEMEVENTID.MedicalEventId);
                            break;
                        case Constants.ScopeInterTerritorialAccount:
                            meventQuery = db.FactMEE.Where(p =>
                                p.FACTMEEMEVENTID.PatientId == patientId &&
                                p.ActId.HasValue &&
                                (p.Source == (int)RefusalSource.InterTerritorial ||
                                 p.Source == (int)RefusalSource.InterTerritorialTotal))
                                .Select(p => p.FACTMEEMEVENTID.MedicalEventId);
                            break;
                        default:
                            throw new InvalidOperationException("Неверный scope {0}".F(scope));
                    }

                    result.Data = db.EventShortView.Where(s => meventQuery.Contains(s.EventId)).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<EventShortView>> GetMedicalEventByPatientIdWithEqmaWithAct(int patientId, int scope)
        {
            var result = new TransactionResult<IEnumerable<EventShortView>>();
            using (var db = CreateContext())
            {
                try
                {
                    IQueryable<int> meventQuery = null;
                    switch (scope)
                    {
                        case Constants.ScopeLocalAccount:
                            meventQuery = db.FactEQMA.Where(p =>
                                p.FACTEQMAMEVENTID.PatientId == patientId &&
                                p.ActId.HasValue &&
                                (p.Source == (int)RefusalSource.Local ||
                                 p.Source == (int)RefusalSource.LocalCorrected))
                                .Select(p => p.FACTEQMAMEVENTID.MedicalEventId);
                            break;
                        case Constants.ScopeInterTerritorialAccount:
                            meventQuery = db.FactEQMA.Where(p =>
                                p.FACTEQMAMEVENTID.PatientId == patientId &&
                                p.ActId.HasValue &&
                                (p.Source == (int)RefusalSource.InterTerritorial ||
                                 p.Source == (int)RefusalSource.InterTerritorialTotal))
                                .Select(p => p.FACTEQMAMEVENTID.MedicalEventId);
                            break;
                        default:
                            throw new InvalidOperationException("Неверный scope {0}".F(scope));
                    }

                    result.Data = db.EventShortView.Where(s => meventQuery.Contains(s.EventId)).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<FactActMee> GetMeeActById(int? id)
        {
            var result = new TransactionResult<FactActMee>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactActMee.FirstOrDefault(p => p.ActMeeId == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<FactActEqma> GetEqmaActById(int? id)
        {
            var result = new TransactionResult<FactActEqma>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactActEqma.FirstOrDefault(p => p.ActEqma == id);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public int GetMeeActLastNumber(int scope)
        {
            int result = 1;
            using (var db = CreateContext())
            {
                try
                {
                    switch (scope)
                    {
                        case Constants.ScopeLocalAccount:
                            result = db.FactActMee.Where(p=>p.AccountMoId.HasValue).Max(p => p.Number) + 1 ?? 1;
                            break;
                        case Constants.ScopeInterTerritorialAccount:
                            result = db.FactActMee.Where(p => p.AccountId.HasValue).Max(p => p.Number) + 1 ?? 1;
                            break;
                        default:
                            break;
                    }
                    
                }
                catch (Exception exception)
                {
                    //TODO log
                }
            }
            return result;
        }

        public int GetEqmaActLastNumber(int scope)
        {
            int result = 1;
            using (var db = CreateContext())
            {
                try
                {
                    switch (scope)
                    {
                        case Constants.ScopeLocalAccount:
                            result = db.FactActEqma.Where(p => p.AccountMoId.HasValue).Max(p => p.Number) + 1 ?? 1;
                            break;
                        case Constants.ScopeInterTerritorialAccount:
                             result = db.FactActMee.Where(p => p.AccountId.HasValue).Max(p => p.Number) + 1 ?? 1;
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception exception)
                {
                    //TODO log
                }
            }
            return result;
        }

        public TransactionResult<FactPerson> GetPersonByPatientId(int patientId)
        {
            var result = new TransactionResult<FactPerson>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactPatient
                        .Where(p => p.PatientId == patientId)
                        .Select(p=>p.FACTPATIFPATIENTPFACTPERS)
                        .FirstOrDefault();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactMedicalEvent>> GetMedicalEventsByIds(List<object> list)
        {
            var result = new TransactionResult<IEnumerable<FactMedicalEvent>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactMedicalEvent
                        .Where(p => list.Contains(p.MedicalEventId))
                        .ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public void TrancateTable(string name)
        {
            using (var db = CreateContext())
            {
                try
                {
                    db.TrancateTable(name);
                }
                catch (Exception exception)
                {
                    
                }
            }
        }

        public void DeleteTable(string name)
        {
            using (var db = CreateContext())
            {
                try
                {
                    db.DeleteTable(name);
                }
                catch (Exception exception)
                {

                }
            }
        }

        public TransactionResult<IEnumerable<T>> GetAll<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            var result = new TransactionResult<IEnumerable<T>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = predicate == null ? 
                        db.GetTableQuery<T>().ToList() :
                        db.GetTableQuery<T>().Where(predicate).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<int> InsertOrUpdateMeeAct(FactActMee data, IEnumerable<int> meeIds)
        {
            var result = new TransactionResult<int>();
            using (var db = CreateContext())
            {
                db.BeginTransaction();
                try
                {
                    if (data.ActMeeId == 0)
                    {
                        //Новый акт МЭЭ
                        var insertMeeActResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertMeeActResult))
                        {
                            throw new InvalidOperationException("Ошибка добавления акта МЭЭ.\r\n{0}".F(db.LastQuery));
                        }
                        int actId = Convert.ToInt32(insertMeeActResult);
                        result.Data = actId;

                        var updateResult = db.FactMEE
                            .Where(p => meeIds.Contains(p.MEEId))
                            .Set(s => s.ActId, actId)
                            .Update();

                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Ошибка добавления ID акта МЭЭ в отказы.\r\n{0}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        //Обновление акта МЭЭ
                        db.FactMEE
                            .Where(p => p.ActId == data.ActMeeId)
                            .Set(s => s.ActId, default(int?))
                            .Update();

                        var updateActResult = db.Update(data);
                        if (!Convert.ToBoolean(updateActResult))
                        {
                            throw new InvalidOperationException("Ошибка обновления акта МЭЭ.\r\n{0}".F(db.LastQuery));
                        }

                        var updateResult = db.FactMEE
                            .Where(p => meeIds.Contains(p.MEEId))
                            .Set(s => s.ActId, data.ActMeeId)
                            .Update();

                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Ошибка добавления ID акта МЭЭ в отказы.\r\n{0}".F(db.LastQuery));
                        }
                        result.Data = data.ActMeeId;
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<int> InsertOrUpdateEqmaAct(FactActEqma data, IEnumerable<int> eqmaIds)
        {
            var result = new TransactionResult<int>();
            using (var db = CreateContext())
            {
                db.BeginTransaction();
                try
                {
                    if (data.ActEqma == 0)
                    {
                        //Новый акт ЭКМП
                        var insertEqmaActResult = db.InsertWithIdentity(data);
                        if (!Convert.ToBoolean(insertEqmaActResult))
                        {
                            throw new InvalidOperationException("Ошибка добавления акта ЭКМП.\r\n{0}".F(db.LastQuery));
                        }
                        int actId = Convert.ToInt32(insertEqmaActResult);
                        result.Data = actId;

                        var updateResult = db.FactEQMA
                            .Where(p => eqmaIds.Contains(p.EQMAId))
                            .Set(s => s.ActId, actId)
                            .Update();

                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Ошибка добавления ID акта ЭКМП в отказы.\r\n{0}".F(db.LastQuery));
                        }
                    }
                    else
                    {
                        //Обновление акта МЭЭ
                        db.FactMEE
                            .Where(p => p.ActId == data.ActEqma)
                            .Set(s => s.ActId, default(int?))
                            .Update();

                        var updateActResult = db.Update(data);
                        if (!Convert.ToBoolean(updateActResult))
                        {
                            throw new InvalidOperationException("Ошибка обновления акта ЭКМП.\r\n{0}".F(db.LastQuery));
                        }

                        var updateResult = db.FactEQMA
                            .Where(p => eqmaIds.Contains(p.EQMAId))
                            .Set(s => s.ActId, data.ActEqma)
                            .Update();

                        if (!Convert.ToBoolean(updateResult))
                        {
                            throw new InvalidOperationException("Ошибка добавления ID акта ЭКМП в отказы.\r\n{0}".F(db.LastQuery));
                        }
                        result.Data = data.ActEqma;
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<int>> GetTerritoryAccountDate() {
            var result = new TransactionResult<IEnumerable<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactTerritoryAccount
                        .Where(p=>p.Date.HasValue)
                        .Select(p=>p.Date.Value.Year)
                        .Distinct()
                        .ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<int>> GetMedicalAccountDate()
        {
            var result = new TransactionResult<IEnumerable<int>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.FactMedicalAccount
                        .Where(p => p.Date.HasValue)
                        .Select(p => p.Date.Value.Year)
                        .Distinct()
                        .ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }


        public TransactionResult TrancateAllData() {
            var tables = new List<string>
            {
                "FactActEqma",
                "FactActMee",
                "FactEQMA",
                "FactMEC",
                "FactMEE",
                "FactExternalRefuse",
                "FactMedicalServices",
                "FactMedicalEvent",
                "FactPatient",
                "FactDocument",
                "FactPerson",
                "FactMedicalAccount",
                "FactTerritoryAccount",
                "FactExchange"
            };

            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    tables.ForEach(p => db.DeleteTable(p));
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }
        public bool IsTerritoryAccountHaveMedicalAccount(int accountId) 
        {
            var result = false;
            using (var db = CreateContext())
            {
                try
                {
                    result =  db.FactPatient.Where(p => p.AccountId == accountId).Any(p => p.MedicalAccountId.HasValue);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return result;
        }

        public TransactionResult DeleteAllLocalAutoMec(int id, IEnumerable<int?> checks)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    //Получаем все случаи с МЭК
                    var meventIds = db.FactMEC
                        .Where(p =>
                            p.MedicalEvent.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == id &&
                            (p.Source == (int)RefusalSource.Local || p.Source == (int)RefusalSource.LocalCorrected) &&
                            p.EmployeeId == Constants.SystemAccountId &&
                            (p.IsLock == null || p.IsLock == false) &&
                            checks.Contains(p.ReasonId))
                            .Select(p => p.MedicalEventId).ToList();

                    if (meventIds.Any())
                    {
                        //Удаляем отказы
                        db.FactMEC
                        .Delete(p =>
                            p.MedicalEvent.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == id &&
                            (p.Source == (int)RefusalSource.Local || p.Source == (int)RefusalSource.LocalCorrected) &&
                            p.EmployeeId == Constants.SystemAccountId &&
                            (p.IsLock == null || p.IsLock == false) &&
                            checks.Contains(p.ReasonId));

                        //Обновляем случаи
                        foreach (var meventId in meventIds)
                        {
                            UpdateMedicalEventAsLocal(db, meventId);
                        }
                    }

                    db.CommitTransaction();
                    
                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteAllLocalAutoSank(int id, IEnumerable<int?> checks)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    //Получаем все случаи с МЭК
                    var meventIds = db.ZFactSank
                        .Where(p =>
                             p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id &&
                            (p.Source == (int)RefusalSource.Local || p.Source == (int)RefusalSource.LocalCorrected) &&
                            p.EmployeeId == Constants.SystemAccountId &&
                            p.Type == 1 &&
                            (p.IsLock == null || p.IsLock == false) &&
                            checks.Contains(p.ReasonId))
                            .Select(p => p.ZmedicalEventId).ToList();

                    if (meventIds.Any())
                    {
                        //Удаляем отказы
                        db.ZFactSank
                        .Delete(p =>
                            p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id &&
                            (p.Source == (int)RefusalSource.Local || p.Source == (int)RefusalSource.LocalCorrected) &&
                            p.Type == 1 &&
                            p.EmployeeId == Constants.SystemAccountId &&
                            (p.IsLock == null || p.IsLock == false) &&
                            checks.Contains(p.ReasonId));

                        //Обновляем случаи
                        foreach (var meventId in meventIds)
                        {
                            UpdateMedicalEventAsLocal(db, meventId);
                        }
                    }

                    db.CommitTransaction();

                }
                catch (Exception exception)
                {
                    db.RollbackTransaction();
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteAllTerritoryAutoMec(int id, IEnumerable<int?> checks)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    //Получаем все случаи с МЭК
                    var meventIds = db.FactMEC
                        .Where(p =>
                            p.MedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == id &&
                            (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal) &&
                            p.EmployeeId == Constants.SystemAccountId &&
                            (p.IsLock == null || p.IsLock == false) &&
                            checks.Contains(p.ReasonId))
                            .Select(p => p.MedicalEventId).ToList();

                    if (meventIds.Any())
                    {
                        //Удаляем отказы
                        db.FactMEC
                        .Delete(p =>
                            p.MedicalEvent.FACTMEDIPATIENTIDFACTPATI.AccountId == id &&
                            (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal) &&
                            p.EmployeeId == Constants.SystemAccountId &&
                            (p.IsLock == null || p.IsLock == false) &&
                            checks.Contains(p.ReasonId));

                        //Обновляем случаи
                        foreach (var meventId in meventIds)
                        {
                            UpdateMedicalEventAsTerritorial(db, meventId);
                        }
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult DeleteAllTerritoryAutoSank(int id, IEnumerable<int?> checks)
        {
            var result = new TransactionResult();
            using (var db = CreateContext())
            {
                try
                {
                    db.BeginTransaction();
                    //Получаем все случаи с МЭК
                    var meventIds = db.ZFactSank
                        .Where(p =>
                            p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id &&
                         //   (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal) &&
                            p.EmployeeId == Constants.SystemAccountId &&
                            p.Type == 1 &&
                            (p.IsLock == null || p.IsLock == false) &&
                            checks.Contains(p.ReasonId))
                            .Select(p => p.ZmedicalEventId).ToList();

                    if (meventIds.Any())
                    {
                        //Удаляем отказы
                        db.ZFactSank
                        .Delete(p =>
                            p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.AccountId == id &&
                           // (p.Source == (int)RefusalSource.InterTerritorial || p.Source == (int)RefusalSource.InterTerritorialTotal) &&
                            p.EmployeeId == Constants.SystemAccountId &&
                            p.Type == 1 &&
                            (p.IsLock == null || p.IsLock == false) &&
                            checks.Contains(p.ReasonId));

                        //Обновляем случаи
                        foreach (var meventId in meventIds)
                        {
                            UpdateZMedicalEventAsTerritorial(db, meventId);
                        }
                        var zslMeventIds =
                            db.ZFactMedicalEvent.Where(x => meventIds.Contains(x.ZmedicalEventId))
                                .Select(y => y.ZslMedicalEventId).ToList();

                        foreach (var zslMeventId in zslMeventIds)
                        {
                            UpdateZslMedicalEventAsTerritorial(db, zslMeventId);
                        }
                    }

                    db.CommitTransaction();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public bool IsSankWithoutActExistsForMedicalAccount(int medicalAccountId)
        {
            using (var db = CreateContext())
            {
                try
                {
                    return db.ZFactSank.Count(p =>
                        p.ZFactMedicalEvent.ZslFactMedicalEvent.ZSLFACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId &&
                        (p.Source == (int)RefusalSource.Local ||
                         p.Source == (int)RefusalSource.LocalCorrected)) > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsEqmaWithoutActExistsForMedicalAccount(int medicalAccountId)
        {
            using (var db = CreateContext())
            {
                try
                {
                    return db.FactEQMA.Count(p =>
                        p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId &&
                        !p.ActId.HasValue &&
                        (p.Source == (int)RefusalSource.Local ||
                         p.Source == (int)RefusalSource.LocalCorrected)) > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsMeeWithoutActExistsForMedicalAccount(int medicalAccountId)
        {
            using (var db = CreateContext())
            {
                try
                {
                    return db.FactMEE.Count(p =>
                        p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId &&
                        !p.ActId.HasValue &&
                        (p.Source == (int)RefusalSource.Local ||
                         p.Source == (int)RefusalSource.LocalCorrected)) > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsEqmaWithActExistsForMedicalAccount(int medicalAccountId)
        {
            using (var db = CreateContext())
            {
                try
                {
                    return db.FactEQMA.Count(p =>
                        p.FACTEQMAMEVENTID.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId &&
                        p.ActId.HasValue &&
                        (p.Source == (int)RefusalSource.Local ||
                         p.Source == (int)RefusalSource.LocalCorrected)) > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsMeeWithActExistsForMedicalAccount(int medicalAccountId)
        {
            using (var db = CreateContext())
            {
                try
                {
                    return db.FactMEE.Count(p =>
                        p.FACTMEEMEVENTID.FACTMEDIPATIENTIDFACTPATI.MedicalAccountId == medicalAccountId &&
                        p.ActId.HasValue &&
                        (p.Source == (int)RefusalSource.Local ||
                         p.Source == (int)RefusalSource.LocalCorrected)) > 0;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public TransactionResult<IEnumerable<FactActMee>> GetMeeActByMedicalAccountId(int medicalAccountId)
        {
            var result = new TransactionResult<IEnumerable<FactActMee>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = MeeActByMedicalAccountIdQuery(db, medicalAccountId).DistinctBy(p => p.ActMeeId).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactActEqma>>  GetEqmaActByAccountIdAndScope(int id, int scope)
        {
            var result = new TransactionResult<IEnumerable<FactActEqma>>();
            using (var db = CreateContext())
            {
                try
                {
                    switch (scope)
                    {
                        case Constants.ScopeLocalAccount:
                            result.Data = EqmaActByMedicalAccountIdQuery(db, id).DistinctBy(p => p.ActEqma).ToList();
                            break;
                        case Constants.ScopeInterTerritorialAccount:
                            result.Data = EqmaActByTerritoryAccountIdQuery(db, id).DistinctBy(p => p.ActEqma).ToList();
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactActMee>> GetMeeActByAccountIdAndScope(int id, int scope) 
        {
            var result = new TransactionResult<IEnumerable<FactActMee>>();
            using (var db = CreateContext())
            {
                try
                {
                    switch (scope)
                    {
                        case Constants.ScopeLocalAccount:
                            result.Data = MeeActByMedicalAccountIdQuery(db, id).DistinctBy(p => p.ActMeeId).ToList();
                            break;
                        case Constants.ScopeInterTerritorialAccount:
                            result.Data = MeeActByTerritoryAccountIdQuery(db, id).DistinctBy(p => p.ActMeeId).ToList();
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactActMee>> GetMeeActByScope(int scope)
        {
            var result = new TransactionResult<IEnumerable<FactActMee>>();
            using (var db = CreateContext())
            {
                try
                {
                    switch (scope)
                    {
                        case Constants.ScopeLocalAccount:
                            result.Data = MedicalAccountMeeActQuery(db).DistinctBy(p => p.ActMeeId).ToList();
                            break;
                        case Constants.ScopeInterTerritorialAccount:
                            result.Data = TerritoryAccountMeeActQuery(db).DistinctBy(p => p.ActMeeId).ToList();
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<FactActEqma>> GetEqmaActByScope(int scope)
        {
            var result = new TransactionResult<IEnumerable<FactActEqma>>();
            using (var db = CreateContext())
            {
                try
                {
                    switch (scope)
                    {
                        case Constants.ScopeLocalAccount:
                            result.Data = MedicalAccountEqmaActQuery(db).DistinctBy(p=>p.ActEqma).ToList();
                            break;
                        case Constants.ScopeInterTerritorialAccount:
                            result.Data = TerritoryAccountEqmaActQuery(db).DistinctBy(p => p.ActEqma).ToList();
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<IEnumerable<globalLicenseEntry>> GetLicenseEntryById(int id) {
            var result = new TransactionResult<IEnumerable<globalLicenseEntry>>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = LicenseEntryByIdQuery(db, id).ToList();
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public TransactionResult<localF003> GetLocalF003ByCode(string code)
        {
            var result = new TransactionResult<localF003>();
            using (var db = CreateContext())
            {
                try
                {
                    result.Data = db.localF003.FirstOrDefault(p => p.Code == code);
                }
                catch (Exception exception)
                {
                    result.AddError(exception);
                }
            }
            return result;
        }

        public int GetMax(string tableName, string fieldName)
        {
            using (var db = CreateContext())
            {
                try
                {
                    db.SetCommand("SELECT MAX({0}) FROM {1}".F(fieldName, tableName));
                    return (db.ExecuteScalar<int>()) + 1;
                }
                catch (Exception )
                {
                    return 1;
                }
            }
        }

        public void UpdateGuid(int accountId) {
            using (var db = CreateContext())
            {
                try
                {
                    db.SetCommand(@"UPDATE FactMEC 
                                    SET ExternalGuid = NEWID() 
                                    from FactMEC m
                                    INNER JOIN FactMedicalEvent e ON e.MedicalEventId = m.MedicalEventId
                                    INNER JOIN FactPatient p ON p.PatientId = e.PatientId AND p.AccountId = {0}
                                    where ExternalGuid IS NULL OR ExternalGuid = ''".F(accountId));
                    db.ExecuteNonQuery();

                    db.SetCommand(@"UPDATE FactMEE 
                                    SET ExternalGuid = NEWID() 
                                    from FactMEE m
                                    INNER JOIN FactMedicalEvent e ON e.MedicalEventId = m.MedicalEventId
                                    INNER JOIN FactPatient p ON p.PatientId = e.PatientId AND p.AccountId = {0}
                                    where ExternalGuid IS NULL OR ExternalGuid = ''".F(accountId));
                    db.ExecuteNonQuery();

                    db.SetCommand(@"UPDATE FactEQMA 
                                    SET ExternalGuid = NEWID() 
                                    from FactEQMA m
                                    INNER JOIN FactMedicalEvent e ON e.MedicalEventId = m.MedicalEventId
                                    INNER JOIN FactPatient p ON p.PatientId = e.PatientId AND p.AccountId = {0}
                                    where ExternalGuid IS NULL OR ExternalGuid = ''".F(accountId));
                    db.ExecuteNonQuery();

                    db.SetCommand(@"UPDATE FactMedicalServices 
                                    SET ExternalGuid = NEWID() 
                                    from FactMedicalServices s
                                    INNER JOIN FactMedicalEvent e ON e.MedicalEventId = s.MedicalEventId
                                    INNER JOIN FactPatient p ON p.PatientId = e.PatientId AND p.AccountId = 13378
                                    where ExternalGuid IS NULL OR ExternalGuid = ''".F(accountId));
                    db.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    
                }
            }
        }
    }
}
