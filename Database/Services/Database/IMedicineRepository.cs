using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Core;
using Core.DataStructure;
using Core.Infrastructure;
using DataModel;
using EventShortView = DataModel.EventShortView;
using F001 = DataModel.F001;
using F002 = DataModel.F002;
using F003 = DataModel.F003;
using F005 = DataModel.F005;
using F008 = DataModel.F008;
using F010 = DataModel.F010;
using F011 = DataModel.F011;
using F014 = DataModel.F014;
using FactDocument = DataModel.FactDocument;
using FactEconomicAccount = DataModel.FactEconomicAccount;
using FactEconomicPayment = DataModel.FactEconomicPayment;
using FactEconomicRefuse = DataModel.FactEconomicRefuse;
using FactEconomicRefuseDetail = DataModel.FactEconomicRefuseDetail;
using FactEconomicSurcharge = DataModel.FactEconomicSurcharge;
using FactEconomicSurchargeDetail = DataModel.FactEconomicSurchargeDetail;
using FactExchange = DataModel.FactExchange;
using FactExpertCriterion = DataModel.FactExpertCriterion;
using FactPatient = DataModel.FactPatient;
using FactPerson = DataModel.FactPerson;
using FactPreparedReport = DataModel.FactPreparedReport;
using FactReport = DataModel.FactReport;
using FactTerritoryAccount = DataModel.FactTerritoryAccount;
using globalAccountType = DataModel.globalAccountType;
using globalExaminationGroup = DataModel.globalExaminationGroup;
using globalExaminationType = DataModel.globalExaminationType;
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
using V015 = DataModel.V015;
using localSettings = DataModel.localSettings;
using FactMedicalEvent = DataModel.FactMedicalEvent;

namespace Medical.DatabaseCore.Services.Database
{
    public interface IMedicineRepository
    {
        #region CRUD
        /// <summary>
        /// Вставить данные с автоинкрементом
        /// </summary>
        /// <typeparam name="T">Тип данных</typeparam>
        /// <param name="data">Данные</param>
        /// <returns>В случае успеха, Id новое значение автоинкремента</returns>
        TransactionResult InsertWithIdentity<T>(T data);
        /// <summary>
        /// Получить данные таблицы в соответствии с заданым предикатом
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TransactionResult<IEnumerable<T>> GetAll<T>(Expression<Func<T, bool>> predicate = null) where T : class;
        /// <summary>
        /// Обновить данные
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        TransactionResult<int> Update<T>(T obj);
        /// <summary>
        /// Удалить данные
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        TransactionResult Delete<T>(T data);
        /// <summary>
        /// Вставить последовательность данных
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        TransactionResult InsertBatch<T>(List<T> data);
        /// <summary>
        /// Вставить данные
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        TransactionResult InsertOrUpdate<T>(T data);

        int GetMax(string tableName, string fieldName);
        #endregion

        #region Settings
        TransactionResult UpdateLocalSettings(string key, string value);
        TransactionResult UpdateLocalSettingsMetadata(string key, string metadata);
        #endregion


        TransactionResult<IEnumerable<globalVersion>> GetGlobalVersion();
        TransactionResult<IEnumerable<globalPcel>> GetGlobalPcel();
        TransactionResult<IEnumerable<globalKslp>> GetGlobalKslp();
        TransactionResult<IEnumerable<V002>> GetV002();
        TransactionResult<IEnumerable<V003>> GetV003();
        TransactionResult<IEnumerable<V005>> GetV005();
        TransactionResult<IEnumerable<V006>> GetV006();
        TransactionResult<IEnumerable<V008>> GetV008();
        TransactionResult<IEnumerable<V009>> GetV009();
        TransactionResult<IEnumerable<V010>> GetV010();
        TransactionResult<IEnumerable<V012>> GetV012();
        TransactionResult<IEnumerable<V014>> GetV014();
        TransactionResult<IEnumerable<V015>> GetV015();
        TransactionResult<IEnumerable<V020>> GetV020();
        TransactionResult<IEnumerable<V021>> GetV021();
        TransactionResult<IEnumerable<V024>> GetV024();
        TransactionResult<IEnumerable<V025>> GetV025();
        TransactionResult<IEnumerable<V004>> GetV004();
        TransactionResult<IEnumerable<V026>> GetV026();
        TransactionResult<IEnumerable<V027>> GetV027();
        TransactionResult<IEnumerable<V028>> GetV028();
        TransactionResult<IEnumerable<V029>> GetV029();
        TransactionResult<IEnumerable<M001>> GetM001();
        TransactionResult<IEnumerable<V023>> GetV023();
        TransactionResult<IEnumerable<F001>> GetF001();
        TransactionResult<IEnumerable<F002>> GetF002();
        TransactionResult<IEnumerable<F003>> GetF003();
        TransactionResult<IEnumerable<F003>> GetF003ByOkato(string okato);
        TransactionResult<IEnumerable<F004>> GetF004();
        TransactionResult<IEnumerable<F005>> GetF005();
        TransactionResult<IEnumerable<F006>> GetF006();
        TransactionResult<IEnumerable<F006>> GetF006(Expression<Func<F006, bool>> predicate = null);
        TransactionResult<IEnumerable<F008>> GetF008();
        TransactionResult<IEnumerable<F010>> GetF010();
        TransactionResult<IEnumerable<F014>> GetF014();
        TransactionResult<IEnumerable<globalDirectionView>> GetDirectionView();
        TransactionResult<IEnumerable<globalVidControl>> GetVidControl();
        TransactionResult<IEnumerable<globalActExpertiseStatus>> GetActExpertiStatus();
        TransactionResult<IEnumerable<globalMetIssl>> GetMetIssl();
        TransactionResult<IEnumerable<N007>> GetN007();
        TransactionResult<IEnumerable<N008>> GetN008();
        TransactionResult<IEnumerable<N010>> GetN010();
        TransactionResult<IEnumerable<N011>> GetN011();
        TransactionResult<IEnumerable<N013>> GetN013();
        TransactionResult<IEnumerable<N014>> GetN014();
        TransactionResult<IEnumerable<N015>> GetN015();
        TransactionResult<IEnumerable<N016>> GetN016();
        TransactionResult<IEnumerable<N017>> GetN017();
        TransactionResult<IEnumerable<N018>> GetN018();
        TransactionResult<IEnumerable<N019>> GetN019();
        TransactionResult<IEnumerable<N020>> GetN020();
        TransactionResult<IEnumerable<FactExchange>> GetFactExchange(Expression<Func<FactExchange, bool>> predicate = null);
        TransactionResult<IEnumerable<FactTerritoryAccount>> GetFactTerritoryAccount(Expression<Func<FactTerritoryAccount, bool>> predicate = null);
        TransactionResult<IEnumerable<EventShortView>> GetEventShortView(Expression<Func<EventShortView, bool>> predicate = null);
        TransactionResult<IEnumerable<ZslEventShortView>> GetZslEventShortView(Expression<Func<ZslEventShortView, bool>> predicate = null);
        TransactionResult<IEnumerable<GeneralEventShortView>> GetGeneralEventShortView(Expression<Func<GeneralEventShortView, bool>> predicate = null);
        TransactionResult<IEnumerable<PatientShortView>> GetPatientShortView(Expression<Func<PatientShortView, bool>> predicate = null);
        TransactionResult<IEnumerable<MedicalAccountView>> GetMedicalAccountView(Expression<Func<MedicalAccountView, bool>> predicate = null);
        TransactionResult<IEnumerable<globalExaminationGroup>> GetGlobalExaminationGroup();
        TransactionResult<IEnumerable<globalPaymentStatus>> GetGlobalPaymentStatus();
        TransactionResult<IEnumerable<EconomicPartner>> GetEconomicPartner(string okato);
        TransactionResult<IEnumerable<globalParticularSign>> GetGlobalParticularSign();
        TransactionResult<IEnumerable<globalOldProfile>> GetGlobalOldProfile();
        TransactionResult<IEnumerable<globalRefusalSource>> GetGlobalRefusalSource();
        TransactionResult<IEnumerable<localEmployee>> GetLocalEmployee();
        TransactionResult<IEnumerable<globalExaminationType>> GetGlobalExaminationType();
        TransactionResult<IEnumerable<globalReportType>> GetGlobalReportType();
        TransactionResult<IEnumerable<globalParam>> GetGlobalParam();
        TransactionResult<IEnumerable<globalScope>> GetGlobalScope();
        TransactionResult<IEnumerable<F011>> GetF011();
        TransactionResult<localUser> GetUser(string login);
        TransactionResult<int?> GetKolPaySluch(int accountId, int?[] assistanceConditions);
        TransactionResult<int?> GetKolPayZSluch(int accountId, int?[] assistanceConditions);

        TransactionResult<IEnumerable<FactExpertCriterion>> GetFactExpertCriterion(Expression<Func<FactExpertCriterion, bool>> predicate = null);
        TransactionResult<int> ChangeTerritoryAccountStatus(int territoryAccountId,int? status);
        TransactionResult<int> ChangeMedicalAccountStatus(int medicalAccountId, int? status);
        TransactionResult<FactPatient> GetPatient(int patientId);
        TransactionResult<PatientShortView> GetPatientShortViewByPatientId(int patientId);
        TransactionResult<Tuple<FactPatient, FactDocument, FactPerson>> GetPatientDocumentPerson(int patientId);
        TransactionResult<List<int>> GetPatientsIdsByAccountId(int id);
        TransactionResult<List<int>> GetPatientsIdsBySelectZslid(IEnumerable<int> ids);
        TransactionResult<List<int>> GetPatientsIdsBySelectSlid(IEnumerable<int> ids);

        TransactionResult<IQueryable<FactEconomicAccount>> GetEconomicAccountParametrById(string paymentOrder,
            DateTime? paymentDate);


        TransactionResult<IEnumerable<FactReport>> GetFactReport(Expression<Func<FactReport, bool>> predicate);
        TransactionResult<IEnumerable<FactActExpertise>> GetFactActExpertise(Expression<Func<FactActExpertise, bool>> predicate);
        TransactionResult<IEnumerable<ActExpertiseShortView>> GetActExpertiseShortView(Expression<Func<ActExpertiseShortView, bool>> predicate);
        TransactionResult<int> DeleteReport(int id);
        TransactionResult<IEnumerable<FactPreparedReport>> GetFactPreparedReport(Expression<Func<FactPreparedReport, bool>> predicate);
       // TransactionResult<IEnumerable<FactPreparedReportNotBody>> GetFactPreparedReportNotBody(Expression<Func<FactPreparedReportNotBody, bool>> predicate);

        TransactionResult<int> DeletePreparedReport(int id);
        TransactionResult InsertOrUpdateSlKOef(ZFactSlKoef data);
        TransactionResult InsertOrUpdateCrit(ZFactCrit data);
        TransactionResult<IEnumerable<FactReport>> GetEnabledReportsByScope(int scope);
        TransactionResult<IEnumerable<FactReport>> GetEnabledReportsByScopeByVersion(int scope, int version);

        [Obsolete("Старая система пользователей, заменить!!!")]
        TransactionResult<IEnumerable<localEmployee>> GetEmployee();

        TransactionResult<FactReport> GetReportById(int id);
        TransactionResult InsertPreparedReport(int id, int scope, int reportId, string name, int? subId = null);
        TransactionResult UpdatePreparedReport(int id, byte[] body, int pageCount);

        TransactionResult<IEnumerable<FactPreparedReport>> GetPreparedReportByExternalIdScopeSubId(int id, int scope, int? subId = null);
        TransactionResult<FactPreparedReport> GetPreparedReportById(int id);
        bool IsPreparedReportByExternalIdExists(int id, int scope, int? subId = null);

        TransactionResult<int> DeleteExpertCriterion(int id);
        TransactionResult<IEnumerable<FactExpertCriterion>> GetEnabledExpertCriterionByScopeAndVersion(int scope, int version);
        TransactionResult<IEnumerable<FactExpertCriterion>> GetEnabledExpertCriterionByIds(IEnumerable<object> ids);

        TransactionResult<int> GetPatientCountByAccount(int id);
        TransactionResult<int> GetEventCountByAccount(int id);

        TransactionResult<IEnumerable<int?>> ExecuteExam(int criterionId, int id, int scope);
        TransactionResult<int> ExecuteProcessing(int processingId, int id, int scope);

        TransactionResult<decimal?> GetAmountByScopeAndId(int scope, int id, int version);
        TransactionResult<FactEconomicAccount> GetEconomicAccountById(int id);
        TransactionResult<IEnumerable<FactEconomicPayment>> GetPaymentsByEconomicAccountId(int id);
        TransactionResult<IEnumerable<FactEconomicPayment>> GetExpectedPaymentsByAccountId(int id);
        TransactionResult<IEnumerable<FactEconomicPayment>> GetZExpectedPaymentsByAccountId(int id);
        TransactionResult<FactEconomicRefuse> GetEconomicRefuseById(int id);
        TransactionResult<IEnumerable<FactEconomicRefuseDetail>> GetRefusesDetailById(int id);
        TransactionResult<IEnumerable<FactEconomicRefuseDetail>> GetExpectedRefuseByAccountId(int id);
        TransactionResult<IEnumerable<FactEconomicRefuseDetail>> GetZExpectedRefuseByAccountId(int id);

        TransactionResult<FactEconomicSurcharge> GetEconomicSurchargeById(int id);
        TransactionResult<IEnumerable<FactEconomicSurchargeDetail>> GetSurchargeDetailById(int id);

        TransactionResult<bool> IsEconomicAccountByAccountIdExists(int? id);

        TransactionResult <IEnumerable<Tuple<FactEconomicAccount, FactTerritoryAccount, Dictionary<int, Tuple<int, decimal>>>>> GetEconomicAccount1(Expression<Func<FactEconomicAccount, bool>> predicate = null);
        TransactionResult<IEnumerable<Tuple<FactEconomicAccount,Dictionary<int,Tuple<int,decimal>>>>> GetEconomicAccount(Expression<Func<FactEconomicAccount, bool>> predicate = null);
        TransactionResult<IEnumerable<Tuple<FactEconomicRefuse, Dictionary<int, Tuple<int, decimal>>>>> GetEconomicRefuse(Expression<Func<FactEconomicRefuse, bool>> predicate = null);
        TransactionResult<IEnumerable<Tuple<FactEconomicSurcharge, Dictionary<int, Tuple<int, decimal>>>>> GetEconomicSurcharge(Expression<Func<FactEconomicSurcharge, bool>> predicate = null);
        
        TransactionResult<int> ChangeEconomicAccountStatus(int economicAccountId, int? status);
        TransactionResult<int> ChangeEconomicAccountStatus(int economicAccountId, int territoriAccountId, int? status);
        TransactionResult<int> ChangeActExperrtiseStatus(int actExpertiseId, int? status);
        TransactionResult<int> UpdateTerritoryAccountTotalAmount(int id);
        TransactionResult<int> UpdatePaymentDetail(int id);

        TransactionResult<int> DeleteEconomicAccount(int economicAccountId, int accountId);
        TransactionResult<int> DeleteActAxpertise(int actAxpertiseId);
        TransactionResult<int> CreateOrUpdateEconomicAccount(FactEconomicAccount data);
        TransactionResult<int> UpdateEconomicPayment(int accountId, int economicAccountId, IEnumerable<Tuple<decimal, int>> paymentList);
        TransactionResult UpdateEconomicRefuse(int accountId, int economicRefuseId, IEnumerable<Tuple<decimal, int>> refusalList);
        TransactionResult UpdateEconomicSurcharge(int accountId, int economicSurchargeId, IEnumerable<Tuple<decimal, int>> surchargeList);
        TransactionResult<bool> IsEconomicRefuseByAccountIdExists(int? id);

        TransactionResult<IEnumerable<globalAccountStatus>> GetGlobalAccountStatus();
        TransactionResult<IEnumerable<Tuple<FactTerritoryAccount,FactEconomicAccount,decimal,decimal, Dictionary<int, Tuple<int, decimal, decimal, decimal, decimal>>>>> GetEconomicJournal(Expression<Func<FactEconomicAccount, bool>> predicate);
        TransactionResult<bool> IsEconomicSurchargeByAccountIdExists(int? id);

        TransactionResult<IEnumerable<globalAccountType>> GetGlobalAccountType();

        TransactionResult<FactTerritoryAccount> GetTerritoryAccountById(int id);
        TransactionResult<int> GetTerritoryAccountVersionById(int id);
        //ashurkova begin
        TransactionResult<IEnumerable<FactEconomicAccount>> GetEconomicAccountByAccountId(int id);
        //ashurkova end
        TransactionResult<IEnumerable<localSettings>> GetLocalSettings();
        
        TransactionResult CreateOrUpdateExchange(FactExchange data);
        TransactionResult<IEnumerable<FactPatient>> GetPatientsByAccountId(int id);
        TransactionResult<FactPerson> GetPersonById(int id);
        TransactionResult<FactDocument> GetDocumentById(int id);
        TransactionResult<FactDocument> GetDocumentByPersonId(int id);
        TransactionResult<IEnumerable<FactMedicalEvent>> GetMeventsByPatientId(int patientId);
        TransactionResult<IEnumerable<ZslFactMedicalEvent>> GetZslMeventsByPatientId(int patientId);
        TransactionResult<IEnumerable<ZFactMedicalEvent>> GetZMeventsByZslMeventId(int zslMeventId);
        TransactionResult<IEnumerable<ZFactDirection>> GetZDirectionBySlMeventId(int id);
        TransactionResult<IEnumerable<ZFactConsultations>> GetZConsultationsBySlMeventId(int id);
        TransactionResult<IEnumerable<ZFactDs>> GetZDsBySlMeventId(int id);
        TransactionResult<IEnumerable<FactMedicalServices>> GetServicesByMeventId(int medicalEventId);
        TransactionResult<IEnumerable<ZFactMedicalServices>> GetZServicesByZMeventId(int zmedicalEventId);
        TransactionResult<ZFactKsgKpg> GetZKsgKpgByZMeventId(int zmedicalEventId);
        TransactionResult<ZFactMedicalEventOnk> GetZMedicalEventOnkByZMeventId(int zmedicalEventId);
        TransactionResult<IEnumerable<ZFactSlKoef>> GetSlKoefByKsgKpgId(int ksgKpgId);
        TransactionResult<IEnumerable<ZFactCrit>> GetCritByksgKpgId(int ksgKpgId);
        TransactionResult<IEnumerable<ZFactDiagBlok>> GetDiafBlokByMedicalEventOnkId(int medicalEventOnkId);
        TransactionResult<IEnumerable<ZFactDs>> GetDsByMedicalEventId(int medicalEventId);
        TransactionResult<IEnumerable<ZFactAnticancerDrug>> GetAnticancerDrugByMedicalServiceOnkId(int medicalServiceOnkId);
        TransactionResult<IEnumerable<ZFactDirection>> GetDirectionByMedicalServicesId(int medicalServicesId);
        TransactionResult<IEnumerable<ZFactContraindications>> GetContraindicationsByMedicalEventOnkId(int medicalEventOnkId);
        TransactionResult<IEnumerable<ZFactMedicalServicesOnk>> GetZMedicalServiceOnkByMedicalEventId(int id);
        TransactionResult<IEnumerable<FactMEC>> GetMecByMedicalEventId(int medicalEventId);
        TransactionResult<IEnumerable<ZFactSank>> GetSankByMedicalEventId(int medicalEventId);
        TransactionResult<IEnumerable<ZFactSank>> GetSankByZMedicalEventIdAndType(Expression<Func<ZFactSank, bool>> predicate = null);
        TransactionResult<IEnumerable<FactMEC>> GetMecByMedicalEventIdAndType(int id, int type);
        TransactionResult<IEnumerable<FactMEE>> GetMeeByMedicalEventId(int medicalEventId);
        TransactionResult<IEnumerable<FactEQMA>> GetEqmaByMedicalEventId(int medicalEventId);
        TransactionResult<int?> GetFactExchangeLastPacketNumber(Expression<Func<FactExchange, bool>> predicate = null);
        TransactionResult<IEnumerable<FactExternalRefuse>> GetExternalRefusesByAccountId(int id);
        TransactionResult<IEnumerable<ZFactExternalRefuse>> GetZExternalRefusesByAccountId(int id);
        TransactionResult<IEnumerable<FactExternalRefuse>> GetExternalRefuseByMedicalEventId(int id);
        TransactionResult<IEnumerable<ZFactExternalRefuse>> GetZExternalRefuseByMedicalEventId(int id);
        TransactionResult<IEnumerable<ZFactExternalRefuse>> GetZExternalIsAgreeRefuseByMedicalEventId(int id);
        TransactionResult<IEnumerable<ZFactExternalRefuse>> GetZExternalRefuseByZslMedicalEventId(int id);
        TransactionResult<FactTerritoryAccount> GetParentTerritoryAccountByParentId(int? parent);
        TransactionResult<IEnumerable<FactExternalRefuse>> GetExternalRefuseByPatientId(int id);

        TransactionResult<IEnumerable<ZFactExternalRefuse>> GetZExternalRefuseByPatientId(int id);
        TransactionResult<IEnumerable<globalMedicalAssistanceVolume>> GetMedicalAssistanceVolume();
        TransactionResult DeleteExchange(int id);
        TransactionResult<FactTerritoryAccount> GetTerritoryAccountByPatientId(int id);

        TransactionResult<TerritoryAccountView> GetTerritoryAccountViewById(int id);
        TransactionResult InsertOrUpdateMedicalAccount(FactMedicalAccount account);
        TransactionResult InsertOrUpdateActExpertise(FactActExpertise act);

        TransactionResult<IEnumerable<localUserSettings>> GetUserSettings(int id);
        TransactionResult UpdateUserSettings(string key, object value);
        TransactionResult<IEnumerable<FactPerson>> GetPerson(Expression<Func<FactPerson, bool>> predicate = null);
        TransactionResult<IEnumerable<FactDocument>> GetDocument(Expression<Func<FactDocument, bool>> predicate = null);
        TransactionResult UpdateMedicalAccountById(int id, decimal? price, decimal? acceptPrice, decimal? mecDb, decimal? meeDb, decimal? eqmaDb);
        TransactionResult InsertRegisterD(FactMedicalAccount account, List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<FactMedicalEvent, List<FactMedicalServices>, List<FactMEC>, List<FactMEE>, List<FactEQMA>>>>> data, bool isTestLoad);
        TransactionResult InsertRegisterD(FactMedicalAccount account, List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactSank>>>>>>> data, bool isTestLoad);
        TransactionResult InsertRegisterD(FactMedicalAccount account, List<Tuple<FactPerson, FactDocument, List<Tuple<FactPatient, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactSank>>>>>>>>> data, bool isTestLoad);

        TransactionResult InsertRegisterD(FactMedicalAccount account,
            List<Tuple<FactPerson, FactDocument, List<Tuple<FactPatient,
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
                    >>>>>> data, bool isTestLoad);
        TransactionResult<IEnumerable<FactSrzQuery>> GetSrzQueryByPatientId(int id);
        TransactionResult<FactSrzQuery> GetSrzQueryById(int id);
        TransactionResult MarkSrzQueryAsReadById(int id);
        TransactionResult<MedicalAccountView> GetMedicalAccountViewById(int id);
        TransactionResult<IEnumerable<EventShortView>> GetEventShortViewByAccountId(int id);
        TransactionResult<IEnumerable<ZslEventShortView>> GetZslEventShortViewByAccountId(int id);
        TransactionResult<IEnumerable<EventShortView>> GetEventShortViewByPatientId(int id);
        TransactionResult<IEnumerable<ZslEventShortView>> GetZslEventShortViewByPatientId(int id);
        TransactionResult<IEnumerable<EventShortView>> GetEventShortViewByMedicalAccountId(int id);
        TransactionResult<IEnumerable<ZslEventShortView>> GetZslEventShortViewByMedicalAccountId(int id);
        TransactionResult<IEnumerable<ZslEventView>> GetZslEventViewByMedicalAccountId(int id);
        TransactionResult InsertSrzQuery(int patientId, string guid, int type);
        TransactionResult DeleteTerritoryAccount(int id);
        TransactionResult<bool> IsErrorForTerritoryAccountExist(int id);
        TransactionResult<bool> IsErrorForZTerritoryAccountExist(int id);
        TransactionResult<bool> IsSrzQueriesForTerritoryAccountExist(int id);
        TransactionResult BreakUpTerritoryAccount(int id);

        TransactionResult<IEnumerable<EventShortView>> GetEventShortViewByMedicalEventId(int id);
        TransactionResult<IEnumerable<ZslEventShortView>> GetZslEventShortViewByMedicalEventId(int id);
        TransactionResult<IEnumerable<int>> GetAvailableYearsForMedicalAccounts();
        /// <summary>
        /// Для теста другой территории
        /// </summary>
        /// <returns></returns>
        TransactionResult<IEnumerable<int>> GetAvailableYearsForMedicalAccounts2018();
        TransactionResult<IEnumerable<CommonTuple>> GetAvailableMonthsForMedicalAccounts();
        TransactionResult<IDictionary<F010, IGrouping<F010, FactPatient>>> GetUnmakePatientsByDate(int year, int month);
        TransactionResult<IDictionary<F010, IGrouping<F010, FactPatient>>> GetZUnmakePatientsByDate(int year, int month);
        TransactionResult<IEnumerable<FactTerritoryAccount>> GetTerritoryAccount(Expression<Func<FactTerritoryAccount, bool>> predicate = null);
        TransactionResult<int> GetTerritoryAccountLastPacketNumber(Expression<Func<FactTerritoryAccount, bool>> predicate = null);
        TransactionResult<int> GetTerritoryAccountLastAccountNumber(Expression<Func<FactTerritoryAccount, bool>> predicate);
        TransactionResult CreateTerritoryAccount(string okato, int year, int month, int packetNumber, int accountNumber, int version);
        TransactionResult AttachPatientsToTerritoryAccount(Expression<Func<FactPatient,bool>> predicate, int accountId);
        
        TransactionResult<bool> IsTerritoryAccountExistsForMedicalAccount(int medicalAccountId);
        TransactionResult DeleteMedicalAccount(int medicalAccountId);
        TransactionResult DeleteZMedicalAccount(int medicalAccountId);
        TransactionResult<bool> IsErrorForMedicalAccountExist(int id);
        TransactionResult<bool> IsErrorForZMedicalAccountExist(int id);
        TransactionResult<bool> IsSrzQueriesForMedicalAccountExist(int id);
        TransactionResult<FactMedicalEvent> GetMedicalEventById(int id);
        TransactionResult<ZFactMedicalEvent> GetZMedicalEventById(int id);
        TransactionResult<string> GetZMedicalEventBySlidGuid(int? id);
        TransactionResult<string> GetZMedicalEventByExternalId(int? id);
        TransactionResult<ZFactKsgKpg> GetZKsgKpgById(int id);
        TransactionResult<ZFactMedicalEventOnk> GetZMedicalEventOnkById(int id);
        TransactionResult<ZslFactMedicalEvent> GetZslMedicalEventById(int id);
        TransactionResult<List<int>> GetMedicalEventIdsByPatientId(int id);
        TransactionResult<List<int>> GetZMedicalEventIdsByZslMeventId(int id);
        TransactionResult<List<int>> GetZKsgKpgIdsByZslMeventId(int id);
        TransactionResult<List<int>> GetZMedicalEventOnkIdsByZslMeventId(int id);
        TransactionResult<List<int>> GetZslMedicalEventIdsByPatientId(int id);
        TransactionResult<List<int>> GetMedicalServiceIdsByMedicalEventId(int id);
        TransactionResult<List<int>> GetZMedicalServiceIdsByMedicalEventId(int id);
        TransactionResult<List<int>> GetZDirectionIdsByMedicalEventId(int id);
        TransactionResult<List<int>> GetZMedicalServiceOnkIdsByMedicalEventId(int id);
        TransactionResult<List<int>> GetZMedicalConsultationsOnkIdsByMedicalEventId(int id);
        TransactionResult<FactMedicalServices> GetMedicalServiceById(int id);
        TransactionResult<ZFactMedicalServices> GetZMedicalServiceById(int id);
        TransactionResult<ZFactDirection> GetDirectionById(int id);
        TransactionResult<ZFactMedicalServicesOnk> GetZMedicalServiceOnkById(int id);
        TransactionResult<ZFactConsultations> GetZMedicalConsultationsOnkById(int id);


        TransactionResult<List<int>> GetPatientsIdsByMedicalAccountId(int id);
        TransactionResult<List<int>> GetPatientsIdsByMedicalAccountIdWithError(int id);
        TransactionResult<List<int>> GetZPatientsIdsByMedicalAccountIdWithError(int id);
        TransactionResult<List<int>> GetPatientsIdsByMedicalAccountIdWithSrzQuery(int id);

        TransactionResult<List<int>> GetPatientsIdsByAccountIdWithError(int id);
        TransactionResult<List<int>> GetZPatientsIdsByAccountIdWithError(int id);
        TransactionResult<List<int>> GetPatientsIdsByAccountIdWithSrzQuery(int id);
        TransactionResult<IEnumerable<FactMEC>> GetMecByMedicalEventIdAndSource(int id, int? source);
        TransactionResult<IEnumerable<FactMEE>> GetMeeByMedicalEventIdAndSource(int id, int? source);
        TransactionResult<IEnumerable<FactEQMA>> GetEqmaByMedicalEventIdAndSource(int id, int? source);


        TransactionResult ExcludePatientFromTerritorryAccount(int patientId, int accountId);
        
        TransactionResult UpdateTerritoryAccount(int? accountId);
        TransactionResult UpdateZTerritoryAccount(int? accountId);
        TransactionResult UpdateTerritoryAccount(MedicineContext db, int? accountId);
        TransactionResult UpdateZTerritoryAccount(MedicineContext db, int? accountId);
        TransactionResult DeleteEconomicRefuse(int economicRefuseId);
        TransactionResult CreateOrUpdateEconomicRefuse(FactEconomicRefuse data);

        TransactionResult DeleteEconomicSurcharge(int economicSurchargeId);

        TransactionResult CreateOrUpdateEconomicSurcharge(FactEconomicSurcharge data);

        TransactionResult<IEnumerable<localUser>> GetLocalUser(Expression<Func<localUser, bool>> predicate);
        TransactionResult DeleteUser(int userId);

        TransactionResult<int> GetRegisterDLastPacketNumber(string source, string dest);
        TransactionResult<IEnumerable<FactProcessing>> GetProcessing(Expression<Func<FactProcessing, bool>> predicate);
        
        TransactionResult<IEnumerable<globalProcessingType>>  GetGlobalProcessingType();
        TransactionResult<IEnumerable<FactProcessing>> GetEnabledProcessingByScopeAndVersion(int scope, int version);
        TransactionResult<IEnumerable<FactProcessing>> GetEnabledProcessingByIds(List<object> ids);
        TransactionResult<IEnumerable<shareDoctor>> GetShareDoctor(Expression<Func<shareDoctor, bool>> predicate = null);

        TransactionResult DeleteLocalRefusal(int accountId, int id, int refusalType, UpdateFlag flag);
        TransactionResult DeleteZLocalRefusal(int accountId, int zslMedicalEventId, int id, UpdateFlag flag);
        TransactionResult DeleteZdiagBlok(int id);
        TransactionResult DeleteZds(int id);
        TransactionResult DeleteZAnticancerDrug(int id);
        TransactionResult DeleteZDirection(int id);
        TransactionResult DeleteZContraindications(int id);
        TransactionResult DeleteSlkoef(int id);
        TransactionResult DeleteCrit(int id);
        TransactionResult<FactMedicalAccount> GetMedicalAccountById(int id);
        TransactionResult<ZFactDiagBlok> GetZDiagBlokById(int id);
        TransactionResult<ZFactDs> GetZDsById(int id);
        TransactionResult<ZFactAnticancerDrug> GetZAnticancerDrugById(int id);
        TransactionResult<ZFactDirection> GetZDirectionById(int id);
        TransactionResult<ZFactContraindications> GetZContraindicationsById(int id);
        TransactionResult<ZFactSank> GetZsankById(int id);
        TransactionResult<FactMEC> GetMecById(int id);
        TransactionResult<FactMEE> GetMeeById(int id);
        TransactionResult<FactEQMA> GetEqmaById(int id);
        TransactionResult InsertOrUpdateLocalMec(FactMEC data, UpdateFlag flag, int? accountId = null);
        TransactionResult InsertOrUpdateZDiagBlok(ZFactDiagBlok data);
        TransactionResult InsertOrUpdateZDs(ZFactDs data);
        TransactionResult InsertOrUpdateZAnticancerDrug(ZFactAnticancerDrug data);
        TransactionResult InsertOrUpdateZDirection(ZFactDirection data);
        TransactionResult InsertOrUpdateZContraindications(ZFactContraindications data);
        TransactionResult InsertOrUpdateLocalSank(ZFactSank data, UpdateFlag flag, int zslMedicalEventId, int? accountId = null, int? typeRefusal = null);
        TransactionResult InsertOrUpdateLocalMee(FactMEE data, UpdateFlag flag, int? accountId = null);
        TransactionResult InsertOrUpdateLocalEqma(FactEQMA data, UpdateFlag flag, int? accountId = null);
        TransactionResult UpdateMedicalEventAsLocal(int? medicalEventId);
        TransactionResult UpdateMedicalEventAsLocal(MedicineContext db, int? medicalEventId);
        TransactionResult UpdateMedicalAccount(int? medicalAccountId);
        TransactionResult UpdateMedicalAccount(MedicineContext db, int? medicalAccountId);
        TransactionResult UpdateZMedicalAccount(int? medicalAccountId);
        TransactionResult UpdateZMedicalAccount(MedicineContext db, int? medicalAccountId);
        TransactionResult<IEnumerable<localUser>> GetUsers();
        TransactionResult<IEnumerable<localRole>> GetRoles();
        TransactionResult UpdatePatientPolicyById(int? patientId, string inp, int? territoryOkato);
        TransactionResult AddLocalMecToScope(FactMEC localMec, int? id, int? scope, decimal? percent);
        TransactionResult AddLocalMecToScope(ZFactSank localMec, int? id, int? scope, decimal? percent);
        TransactionResult SplitPatientsWithFullRefusal(int year, int month);
        TransactionResult ZSplitPatientsWithFullRefusal(int year, int month);
        TransactionResult<bool> IsExchangeExistsForTerritoryAccount(int accountId);
        TransactionResult DeleteTerritorialRefusal(int accountId, int refusalId, int refusalType, UpdateFlag updateFlag);
        TransactionResult DeleteZTerritorialRefusal(int accountId, int zslMedicalEventId, int refusalId, int refusalType, UpdateFlag flag);
        TransactionResult InsertOrUpdateTerritorialMec(FactMEC data, UpdateFlag flag, int? accountId = null);
        TransactionResult InsertOrUpdateTerritorialSank(ZFactSank data, UpdateFlag flag, int zslMedicalEventId, int? accountId = null);
        TransactionResult InsertOrUpdateTerritorialMee(FactMEE data, UpdateFlag flag, int? accountId = null);
        TransactionResult InsertOrUpdateTerritorialEqma(FactEQMA data, UpdateFlag flag, int? accountId = null);
        TransactionResult UpdateMedicalEventAsTerritorial(MedicineContext db, int? medicalEventId);
        TransactionResult BlockLocalMec(FactMEC data, UpdateFlag flag, int? accountId = null);
        TransactionResult BlockLocalSank(ZFactSank data, UpdateFlag flag, int zslMedicalEventId, int? accountId = null);
        TransactionResult BlockTerritorialMec(FactMEC data, UpdateFlag flag, int? accountId = null);
        TransactionResult BlockTerritorialSank(ZFactSank data, UpdateFlag flag, int zslMedicalEventId, int? accountId = null);
        TransactionResult<FactExternalRefuse> GetExternalRefuseById(int id);
        TransactionResult<ZFactExternalRefuse> GetZExternalRefuseById(int id);
        TransactionResult InsertOrUpdateExternalRefuse(FactExternalRefuse data, UpdateFlag flag, int? accountId = null);

        TransactionResult InsertOrUpdateZExternalRefuse(ZFactExternalRefuse data, UpdateFlag flag, int zslMevemtId, int? accountId = null);
        TransactionResult<IEnumerable<TerritoryAccountView>> GetTerritoryAccountView(Expression<Func<TerritoryAccountView, bool>> predicate = null);
        TransactionResult CopyPatientToAnotherTerritoryAccount(int patientId, int accountId, int? selectedTerritory, int? selectedInsurance, string ogrn, List<object> selectedMevents);
        TransactionResult CopyZPatientToAnotherTerritoryAccount(int patientId, int accountId, int? selectedTerritory, int? selectedInsurance, string ogrn, List<object> selectedMevents);

        TransactionResult MovePatientToAnotherTerritoryAccount(int patientId, int accountId, int parentAccountId, int? selectedTerritory, int? selectedInsurance, string ogrn);

        TransactionResult InsertRegisterE(FactTerritoryAccount account, List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<FactMedicalEvent, List<FactMedicalServices>, List<FactExternalRefuse>>>>> patients, bool isTestLoad);
        TransactionResult InsertRegisterE(FactTerritoryAccount account, List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>>> data, bool isTestLoad);
        TransactionResult InsertRegisterE(FactTerritoryAccount account,
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
                    >>>> data, bool isTestLoad);
        TransactionResult UpdateTerritoryAccountPacketNumber(int accountId, int packetNumber);
        TransactionResult<int> GetTerritoryAccountGenerationByParentId(int accountId);

        TransactionResult<IEnumerable<FactPatient>> GetPatientByMeventExternalIdAndAccountId(int meventExternalId, int accountId);

        TransactionResult<IEnumerable<FactPatient>> GetPatientByZslMeventExternalIdAndAccountId(int meventExternalId, int accountId);
        TransactionResult<FactPatient> GetPatientByExternalIdAndAccountId(int? externalId, int accountId);
        TransactionResult SetMedicalAsFullPaymentByExternalIdAndAccountId(int externalId, int accountId);
        TransactionResult SetZMedicalAsFullPaymentByExternalIdAndAccountId(int externalId, int accountId);

        TransactionResult<FactMedicalEvent> GetEventByExternalIdAndAccountId(int externalId, int accountId);
        TransactionResult<ZslFactMedicalEvent> GetZslEventByExternalIdAndAccountId(int externalId, int accountId);
        TransactionResult<ZFactMedicalEvent> GetZEventByExternalIdAndAccountId(string externalId, int zslMedicalEventId, int accountId);
        TransactionResult<ZFactMedicalEvent> GetZEventBySlidGuidAndAccountId(string slidGuid, int zslMedicalEventId, int accountId);

        TransactionResult<IEnumerable<FactMedicalServices>> GetMedicalService(Expression<Func<FactMedicalServices, bool>> predicate = null);
        TransactionResult<IEnumerable<ZFactMedicalServices>> GetMedicalZService(Expression<Func<ZFactMedicalServices, bool>> predicate = null);
        TransactionResult InsertRegisterEAnswer(FactTerritoryAccount account, List<Tuple<FactPatient, List<Tuple<FactMedicalEvent, List<FactMedicalServices>, List<FactExternalRefuse>>>>> patients, bool isTestLoad);
        TransactionResult InsertRegisterEAnswer(FactTerritoryAccount account, List<Tuple<FactPatient, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>>> data, bool isTestLoad);

        TransactionResult InsertRegisterEAnswer(FactTerritoryAccount account, List<Tuple<FactPatient,
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
                >>>> data, bool isTestLoad);
        TransactionResult SetMedicalEventPaymentStatusByExternalIdAndAccountId(int externalId, int accountId, int? paymentStatus);
        TransactionResult SetZslMedicalEventPaymentStatusByExternalIdAndAccountId(int externalId, int accountId, int? paymentStatus);
        TransactionResult DeleteAllTerritorialMec(int accountId, IEnumerable<int?> refusalCodeIds);
        TransactionResult DeleteAllTerritorialSank(int accountId, IEnumerable<int?> refusalCodeIds);
        TransactionResult DeletePatientTerritorialMec(int accountId, IEnumerable<int?> refusalCodeIds, int patientId);
        TransactionResult DeletePatientTerritorialSank(int accountId, IEnumerable<int?> refusalCodeIds, int patientId);

        TransactionResult<IEnumerable<PatientShortView>> GetPatientsByMedicalAccountIdWithMeeWithoutAct(int medicalAccountId);
        TransactionResult<IEnumerable<PatientShortView>> GetPatientsByAccountIdWithMeeWithoutAct(int accountId);

        TransactionResult<localUser> GetUserById(int id);
        

        TransactionResult<FactActMee> GetMeeActById(int? id);
        TransactionResult<FactActEqma> GetEqmaActById(int? id);

        int GetMeeActLastNumber(int scope);
        int GetEqmaActLastNumber(int scope);

        TransactionResult<FactPerson> GetPersonByPatientId(int patientId);

        TransactionResult<IEnumerable<FactMedicalEvent>> GetMedicalEventsByIds(List<object> list);


        void TrancateTable(string name);
        void DeleteTable(string name);

        #region Territory account
        TransactionResult EnumeratePatientsOfTerritoryAccount(int accountId);
        TransactionResult EnumerateEventsOfTerritoryAccount(int accountId);
        TransactionResult EnumerateZEventsOfTerritoryAccount(int accountId);
        TransactionResult EnumerateZslEventsOfTerritoryAccount(int accountId);
        TransactionResult EnumeratePatientsOfTerritoryAccountTransact(int accountId, MedicineContext context = null);
        TransactionResult EnumerateEventsOfTerritoryAccountTransact(int accountId, MedicineContext context = null);
        #endregion

        #region Acts
        TransactionResult<int> InsertOrUpdateMeeAct(FactActMee data, IEnumerable<int> meeIds);
        TransactionResult<int> InsertOrUpdateEqmaAct(FactActEqma data, IEnumerable<int> eqmaIds);
        bool IsEqmaWithoutActExistsForTerritoryAccount(int accountId);
        bool IsMeeWithoutActExistsForTerritoryAccount(int accountId);

        bool IsMeeWithActExistsForTerritoryAccount(int accountId);
        bool IsEqmaWithActExistsForTerritoryAccount(int accountId);

        TransactionResult<IEnumerable<FactActMee>> GetMeeActByTerritoryAccountId(int accountId);
        TransactionResult<IEnumerable<FactActEqma>> GetEqmaActByTerritoryAccountId(int accountId);

        //Medical account//

        /// <summary>
        /// 
        /// </summary>
        /// <param name="medicalAccountId"></param>
        /// <returns></returns>
        bool IsMeeWithActExistsForMedicalAccount(int medicalAccountId);
        bool IsEqmaWithActExistsForMedicalAccount(int medicalAccountId);

        bool IsMeeWithoutActExistsForMedicalAccount(int medicalAccountId);
        bool IsEqmaWithoutActExistsForMedicalAccount(int medicalAccountId);
        bool IsSankWithoutActExistsForMedicalAccount(int medicalAccountId);

        TransactionResult<IEnumerable<FactActMee>> GetMeeActByMedicalAccountId(int medicalAccountId);
        #endregion

        TransactionResult CreateTerritoryAccountAuto(string okato, int year, int month, int version);


        TransactionResult DeleteMeeAct(int id);
        TransactionResult DeleteEqmaAct(int id);
        TransactionResult<IEnumerable<PatientShortView>> GetPatientsByAccountIdWithMeeWithAct(int accountId);

        TransactionResult<IEnumerable<PatientShortView>> GetPatientsByMedicalAccountIdWithEqmaWithoutAct(int medicalAccountId);

        TransactionResult<IEnumerable<PatientShortView>> GetPatientsByAccountIdWithEqmaWithoutAct(int accountId);

        TransactionResult DeletePreparedReportByExternalIdAndScope(int externalId, int scope);

        TransactionResult<IEnumerable<int>> GetTerritoryAccountDate();

        TransactionResult<IEnumerable<int>> GetMedicalAccountDate();


        TransactionResult TrancateAllData();

        bool IsTerritoryAccountHaveMedicalAccount(int accountId);

        TransactionResult AddTerritoryMecToScope(FactMEC mec, int? id, int? scope, decimal? percent);
        TransactionResult AddTerritoryMecToScope(ZFactSank localMec, int? id, int? scope, decimal? percent);


        TransactionResult DeleteAllLocalAutoMec(int id, IEnumerable<int?> selectedExamsList);

        TransactionResult DeleteAllTerritoryAutoMec(int id, IEnumerable<int?> selectedExamsList);
        TransactionResult DeleteAllTerritoryAutoSank(int id, IEnumerable<int?> checks);


        TransactionResult<IEnumerable<FactActMee>> GetMeeActByAccountIdAndScope(int id, int scope);
        TransactionResult<IEnumerable<FactActMee>> GetMeeActByScope(int scope);

        TransactionResult<IEnumerable<FactActEqma>> GetEqmaActByAccountIdAndScope(int id, int scope);

        TransactionResult<IEnumerable<FactActEqma>> GetEqmaActByScope(int scope);


        TransactionResult<IEnumerable<globalLicenseEntry>> GetLicenseEntryById(int id);

        TransactionResult<IEnumerable<EventShortView>> GetMedicalEventByPatientIdWithMeeWithoutAct(int patientId, int scope);

        TransactionResult<IEnumerable<EventShortView>> GetMedicalEventByPatientIdWithMeeWithAct(int patientId, int scope);

        TransactionResult<IEnumerable<EventShortView>> GetMedicalEventByPatientIdWithEqmaWithoutAct(int patientId, int scope);

        TransactionResult<IEnumerable<EventShortView>> GetMedicalEventByPatientIdWithEqmaWithAct(int patientIdp, int scope);

        TransactionResult<localF003> GetLocalF003ByCode(string code);


        TransactionResult<IEnumerable<EventExtendedView>> GetEventExtendedView(Expression<Func<EventExtendedView, bool>> predicate = null);

        void UpdateGuid(int accountId);
    }
}