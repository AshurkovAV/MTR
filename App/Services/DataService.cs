using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using BLToolkit.Data.Linq;
using BLToolkit.Mapping;
using BLToolkit.Validation;
using Core;
using Core.Extensions;
using Core.Helpers;
using Core.Infrastructure;
using DataModel;
using DevExpress.Data.Filtering;
using DevExpress.Mvvm.POCO;
using DevExpress.XtraRichEdit.Model;
using Medical.AppLayer.Managers;
using Medical.CoreLayer.Validation.Algorithm;
using Medical.DatabaseCore.Services.Cache;
using Medical.DatabaseCore.Services.Database;
using Medical.DataCore;
using Medical.DataCore.Interface;
using Medical.DataCore.v10PL.PL;
using Medical.DataCore.v21.EAnswer;
using Medical.DataCore.v31.E;
using RegisterE = Medical.DataCore.v21.E.RegisterE;
using v10 = Medical.DataCore.v10;
using v21 = Medical.DataCore.v21;
using v30K1 = Medical.DataCore.v30K1;
using v30 = Medical.DataCore.v30;
using v31 = Medical.DataCore.v31;
using v32 = Medical.DataCore.v32;
using v21K2 = Medical.DataCore.v21K2;
using v31K1 = Medical.DataCore.v31K1;
using v32K1 = Medical.DataCore.v32K1;


namespace Medical.AppLayer.Services
{
    [Flags]
    public enum XmlOperations
    {
        None = 0,
        RemoveEmptyOrNull = 1,
    }

    [Flags]
    public enum ProcessingOperations
    {
        PrimaryCheck = 1,
        PolicyCheck = 2,
        TortillaPolicyCheck = 4,
        DuplicateCheck = 8,
        IgnoreScheme = 16,
        NotInTerr = 46
    }

    [Flags]
    public enum ExportOptions
    {
        ExportServices = 1
    }

    /// <summary>
    /// Операции с данными информационного обмена (xml данными), имплементация IDataService
    /// </summary>
    public class DataService : IDataService
    {
        private readonly IMedicineRepository _repository;
        private readonly ICacheRepository _cache;
        private readonly ICommonService _commonService;
        private readonly IDockLayoutManager _dockLayoutManager;
        private readonly IAppRemoteSettings _remoteSettings;
        public DataService(IMedicineRepository repository, 
            ICacheRepository cache, 
            ICommonService commonService,
            IDockLayoutManager dockLayoutManager,
            IAppRemoteSettings remoteSettings)
        {
            _repository = repository;
            _cache = cache;
            _commonService = commonService;
            _dockLayoutManager = dockLayoutManager;
            _remoteSettings = remoteSettings;
        }
        public int GetAccountType(int id)
        {
            var result = _repository.GetTerritoryAccountById(id);
            if (result.Success && result.Data.Direction.HasValue)
            {
                switch ((DirectionType)result.Data.Direction)
                {
                    case DirectionType.Out:
                        //Исходящий счет
                        return result.Data.Type == 1 ? 0 : 2;
                    case DirectionType.In:
                        //Входящий счет
                        return 1;
                }
            }

            return 0;
        }

        public int GetRefusalSource(int id, OperatorMode mode)
        {
            switch (mode)
            {
                case OperatorMode.Local:
                case OperatorMode.Zlocal:
                case OperatorMode.LocalError:
                case OperatorMode.LocalSrzQuery:
                    return (int)RefusalSource.Local;
                case OperatorMode.InterTerritorial:
                case OperatorMode.InterTerritorialError:
                case OperatorMode.InterTerritorialSrzQuery:
                case OperatorMode.ZInterTerritorial:
                    var resultInterTerritorial = _repository.GetTerritoryAccountById(id);
                    if (resultInterTerritorial.Success)
                    {
                        var account = resultInterTerritorial.Data;
                        //Исправленная часть входящий
                        if (account.Type == 2 && account.Direction == 1 )
                        {
                            return (int)RefusalSource.InterTerritorialTotal;
                        }
                        //Основная часть входящий
                        if (account.Type == 1 && account.Direction == 1)
                        {
                            return (int)RefusalSource.InterTerritorial;
                        }
                        //Исправленная часть исходящий
                        if (account.Type == 2 && account.Direction == 0)
                        {
                            return (int)RefusalSource.LocalCorrected;
                        }
                        //Основная часть исходящий
                        if (account.Type == 1 && account.Direction == 0)
                        {
                            return (int)RefusalSource.Local;
                        }
                    }
                break;
            }
            return 0;
        }

        public int GetRefusalSourceByScope(int id, int scope)
        {
            switch (scope)
            {
                case Constants.ScopeLocalAccount:
                    return (int)RefusalSource.Local;
                case Constants.ScopeInterTerritorialAccount:
                    var resultInterTerritorial = _repository.GetTerritoryAccountById(id);
                    if (resultInterTerritorial.Success)
                    {
                        var account = resultInterTerritorial.Data;
                        //Исправленная часть
                        if (account.Generation > 0)
                        {
                            return (int)RefusalSource.InterTerritorialTotal;
                        }
                        //Основная часть
                        return (int)RefusalSource.InterTerritorial;
                    }
                    break;
            }
            return 0;
        }

        public OperationResult SerializeToFile<T>(string fileName, T data, XmlOperations operations = 0, string encoding = "windows-1251")
        {
            var result = new OperationResult();
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var writer = new FileStream(fileName, FileMode.Create))
                {
                    var settings = new XmlWriterSettings
                    {
                        Encoding = Encoding.GetEncoding(encoding),
                        Indent = true,
                        IndentChars = "\t",
                        NewLineChars = Environment.NewLine,
                        ConformanceLevel = ConformanceLevel.Document
                    };

                    using (XmlWriter xmlTextWriter = XmlWriter.Create(writer, settings))
                    {
                        serializer.Serialize(xmlTextWriter, data, new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty) }));
                    }
                }

                if (!operations.Has(XmlOperations.None))
                {
                    using (var writer = new FileStream(fileName, FileMode.Open))
                    {
                        if (operations.Has(XmlOperations.RemoveEmptyOrNull))
                        {
                            Stream conv = XmlHelpers.RemoveEmptyOrNull(writer);
                            writer.SetLength(0);
                            conv.CopyTo(writer);
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

        public OperationResult ExchangeCreateOrUpdate(string omsFilename, RegisterEInfo registerEInfo, int id, int count, DateTime date, int version, int type, int direction)
        {
            var fileName = Path.GetFileName(omsFilename);
            var result = new OperationResult();
            using (var stream = new FileStream(omsFilename, FileMode.Open, FileAccess.Read))
            {
                using (var memory = new MemoryStream())
                {
                    stream.CopyTo(memory);
                    var createOrUpdateResult = _repository.CreateOrUpdateExchange(new FactExchange
                    {
                        ActionDate = DateTime.Now,
                        Data = memory.GetBuffer(),
                        FileName = fileName,
                        Type = type,
                        Direction = direction,
                        AccountId = id,
                        PacketNumber = registerEInfo.NumberXXXX,
                        Source = registerEInfo.SourceTerritory.ToString(CultureInfo.InvariantCulture),
                        Destination = registerEInfo.DestinationTerritory.ToString(CultureInfo.InvariantCulture),
                        RecordCounts = count,
                        Date = date,
                        Version = version
                    });
                    if (!createOrUpdateResult.Success)
                    {
                        result.AddError(createOrUpdateResult.LastError);
                    }
                }
            }

            return result;
        }

        public OperationResult ExchangeCreateOrUpdate(string omsFilename, RegisterDInfo registerDInfo, int id, int count, DateTime date,
            int version, int type, int direction)
        {
            var fileName = Path.GetFileName(omsFilename);
            var result = new OperationResult();
            using (var stream = new FileStream(omsFilename, FileMode.Open))
            {
                using (var memory = new MemoryStream())
                {
                    stream.CopyTo(memory);
                    var createOrUpdateResult = _repository.CreateOrUpdateExchange(new FactExchange
                    {
                        ActionDate = DateTime.Now,
                        Data = memory.GetBuffer(),
                        FileName = fileName,
                        Type = type,
                        Direction = direction,
                        AccountId = id,
                        PacketNumber = registerDInfo.NumberXXXX,
                        Source = registerDInfo.SourceTerritory.ToString(CultureInfo.InvariantCulture),
                        Destination = registerDInfo.DestinationTerritory.ToString(CultureInfo.InvariantCulture),
                        RecordCounts = count,
                        Date = date,
                        Version = version
                    });
                    if (!createOrUpdateResult.Success)
                    {
                        result.AddError(createOrUpdateResult.LastError);
                    }
                }
            }

            return result;
        }
        private RegionService _regionService = new RegionService();

        public OperationResult<Tuple<T1, RegisterEInfo, int>> Write<T1, T2, T3, T4, T5, T6>(int accountId, int version, int type, ExportOptions options)
            where T1 : IRegister, new()
            where T2 : IAccount, new()
            where T3 : IHeader, new()
            where T4 : IRecord, new()
            where T5 : IPatient, new()
            where T6 : IMEvent, new()
        {
            var result = new OperationResult<Tuple<T1, RegisterEInfo, int>>();
            try
            {
                var accountResult = _repository.GetTerritoryAccountById(accountId);
                if (accountResult.HasError)
                {
                    result.AddError(accountResult.LastError);
                    return result;
                }

                _repository.UpdateGuid(accountId);

                var register = new T1
                {
                    InnerAccount = Map.ObjectToObject<T2>(accountResult.Data, typeof(T2))
                };

                register.InnerAccount.Year = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0;
                register.InnerAccount.Month = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Month : 0;
                register.InnerHeader = new T3
                {
                    TargetOkato = accountResult.Data.Destination,
                    SourceOkato = accountResult.Data.Source,
                    Version = version,
                    Date = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value : default(DateTime?)
                };

                var patientsResult = _repository.GetPatientsByAccountId(accountId);
                if (patientsResult.Success)
                {
                    var records = new List<IRecord>();
                    foreach (FactPatient factPatient in patientsResult.Data)
                    {
                        FactPatient patient = factPatient;
                        FactPerson person = null;
                        FactDocument document = null;

                        if (patient.PersonalId.HasValue)
                        {
                            var personResult = _repository.GetPersonById(patient.PersonalId.Value);
                            if (personResult.Success)
                            {
                                person = personResult.Data;
                            }
                        }

                        if (patient.PersonalId.HasValue)
                        {
                            var documentResult = _repository.GetDocumentByPersonId(patient.PersonalId.Value);
                            if (documentResult.Success)
                            {
                                document = documentResult.Data;
                            }
                        }

                        var record = new T4();
                        record.InnerPatient = Map.ObjectToObject<T5>(person, typeof(T5));

                        record.ExternalId = factPatient.ExternalId ?? 0;
                        if (document != null)
                        {
                            record.InnerPatient.DocNumber = document.DocNum;
                            record.InnerPatient.DocSeries = document.DocSeries;
                            record.InnerPatient.DocType = document.DocType;
                        }

                        record.InnerPatient.Newborn = patient.Newborn;
                        record.InnerPatient.InsuranceDocType = patient.InsuranceDocType;
                        record.InnerPatient.InsuranceDocSeries = patient.InsuranceDocSeries;

                        switch (version)
                        {
                            case Constants.Version10:
                                record.InnerPatient.PolicyNumber = patient.InsuranceDocType == 3 ? patient.INP : patient.InsuranceDocNumber;
                                break;
                            case Constants.Version21:
                                record.InnerPatient.InsuranceDocNumber = patient.InsuranceDocNumber;
                                record.InnerPatient.INP = patient.INP;

                                if (record.InnerPatient.InsuranceDocNumber.IsNullOrWhiteSpace() && patient.INP.IsNotNullOrWhiteSpace())
                                {
                                    record.InnerPatient.InsuranceDocNumber = record.InnerPatient.INP;
                                }

                                break;
                        }

                        var meventResult = _repository.GetMeventsByPatientId(patient.PatientId);
                        if (meventResult.Success)
                        {
                            var events = new List<IMEvent>();
                            foreach (FactMedicalEvent factMedicalEvent in meventResult.Data)
                            {
                                factMedicalEvent.DoctorId = null;
                                var meventCopy = Map.ObjectToObject<T6>(factMedicalEvent, typeof(T6));

                                if (version == Constants.Version21)
                                {
                                    if (!factMedicalEvent.SpecialityCodeV015.HasValue)
                                    {
                                        meventCopy.SpecialityCode = factMedicalEvent.SpecialityCode;
                                    }
                                    else
                                    {
                                        meventCopy.SpecialityClassifierVersion = "V015";
                                        meventCopy.SpecialityCodeV015 = factMedicalEvent.SpecialityCodeV015;
                                    }

                                }
                                var refusals = new List<object>();
                                decimal? totalMec = 0.0m;

                                var refusalsMecResult = _repository.GetMecByMedicalEventId(factMedicalEvent.MedicalEventId);
                                if (refusalsMecResult.Success &&
                                    refusalsMecResult.Data.Any(p => (p.IsLock == null || p.IsLock == false)))
                                {
                                    switch (version)
                                    {
                                        case Constants.Version10:
                                            refusals.AddRange(refusalsMecResult.Data
                                                .Where(p => (p.IsLock == null || p.IsLock == false) &&
                                                    p.ReasonId.HasValue &&
                                                    p.Source == (int)RefusalSource.InterTerritorial)
                                                .Select(p => p.ReasonId.Value as object)
                                                .ToList());
                                            break;
                                        case Constants.Version21:
                                            foreach (var factMec in refusalsMecResult.Data.Where(p => p.Source == RefusalSource.InterTerritorial.ToInt32()).Distinct(p => p.MedicalEventId))
                                            {
                                                FactMEC mec = factMec;

                                                var refusal = new v21.E.RefusalE();
                                                refusal.RefusalCode = mec.ReasonId;
                                                refusal.RefusalRate = mec.Amount;
                                                refusal.RefusalSource = mec.Source;
                                                refusal.RefusalType = (int)RefusalType.MEC;
                                                refusal.ExternalGuid = mec.ExternalGuid;
                                                refusal.Comments = mec.Comments;

                                                refusals.Add(refusal);
                                            }
                                            totalMec += refusalsMecResult.Data.Sum(p => p.Amount) ?? 0;
                                            break;
                                    }

                                }

                                /*var refusalsMeeResult = _repository.GetMeeByMedicalEventId(factMedicalEvent.MedicalEventId);
                                if (refusalsMeeResult.Success)
                                {
                                    refusals.AddRange(refusalsMeeResult.Data
                                        .Where(p => p.ReasonId.HasValue)
                                        .Select(p => p.ReasonId.Value as object)
                                        .ToList());
                                }

                                var refusalsEqmaResult = _repository.GetEqmaByMedicalEventId(factMedicalEvent.MedicalEventId);
                                if (refusalsEqmaResult.Success)
                                {
                                    refusals.AddRange(refusalsEqmaResult.Data
                                        .Where(p => p.ReasonId.HasValue)
                                        .Select(p => p.ReasonId.Value as object)
                                        .ToList());
                                }*/


                                meventCopy.InnerRefusals = new List<object>(refusals);
                                meventCopy.RefusalPrice = Math.Min(totalMec.Value, meventCopy.Price ?? 0);
                                //выгрузка услуг
                                if (options.Has(Services.ExportOptions.ExportServices))
                                {
                                    var services = new List<IService>();
                                    var serviceResult = _repository.GetServicesByMeventId(factMedicalEvent.MedicalEventId);
                                    if (serviceResult.Success)
                                    {
                                        foreach (FactMedicalServices serviceE in serviceResult.Data)
                                        {
                                            var serviceCopy = serviceE;
                                            var service = Map.ObjectToObject<v21.E.ServiceE>(serviceCopy);
                                            service.SpecialityCode = serviceCopy.SpecialityCodeV015;
                                            services.Add(service);
                                        }
                                    }
                                    meventCopy.InnerServiceCollection = new List<IService>(services);
                                }

                                events.Add(meventCopy);
                            }
                            record.InnerEventCollection = new List<IMEvent>(events);
                        }

                        records.Add(record);
                    }
                    register.InnerRecordCollection = new List<IRecord>(records);
                    register.InnerRecordCollection = register.InnerRecordCollection.OrderBy(p => p.ExternalId).ToList();
                }


                if (!accountResult.Data.PacketNumber.HasValue)
                {
                    var packetNumberResult = _repository.GetFactExchangeLastPacketNumber(
                        exchange => exchange.Direction == accountResult.Data.Direction &&
                            exchange.Type == type &&
                            exchange.Date.Year == accountResult.Data.Date.Value.Year);
                    if (packetNumberResult.Success)
                    {
                        accountResult.Data.PacketNumber = packetNumberResult.Data;
                        accountResult.Data.PacketNumber = accountResult.Data.PacketNumber.HasValue ? accountResult.Data.PacketNumber + 1 : 1;
                    }
                }

                var f010Cache = _cache.Get(CacheRepository.F010Cache);
                var registerInfo = new RegisterEInfo
                {
                    Type = "R",
                    SourceTerritory = f010Cache.GetBackInt(accountResult.Data.Source),
                    DestinationTerritory = f010Cache.GetBackInt(accountResult.Data.Destination),
                    NumberXXXX = accountResult.Data.PacketNumber.Value,
                    YearXX = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0
                };

                result.Data = Tuple.Create(register, registerInfo, accountResult.Data.Direction ?? 0);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<Tuple<T1, RegisterEInfo, int>> WriteZ<T1, T2, T3, T4, T5, T6>(int accountId, int version, int type, ExportOptions options)
            where T1 : IZRegister, new()
            where T2 : IAccount, new()
            where T3 : IHeader, new()
            where T4 : IZRecord, new()
            where T5 : IPatient, new()
            where T6 : IZslEvent, new()
        {
            var result = new OperationResult<Tuple<T1, RegisterEInfo, int>>();
            
            try
            {
                var accountResult = _repository.GetTerritoryAccountById(accountId);
                if (accountResult.HasError)
                {
                    result.AddError(accountResult.LastError);
                    return result;
                }
                dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
                string dest = Convert.ToString(terCode.tf_code);

                _repository.UpdateGuid(accountId);

                var register = new T1
                {
                    InnerAccount = Map.ObjectToObject<T2>(accountResult.Data, typeof(T2))
                };

                register.InnerAccount.Year = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0;
                register.InnerAccount.Month = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Month : 0;
                register.InnerHeader = new T3
                {
                    TargetOkato = accountResult.Data.Destination,
                    SourceOkato = accountResult.Data.Source,
                    Version = version,
                    Date = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value : default(DateTime?)
                };

                var patientsResult = _repository.GetPatientsByAccountId(accountId);
                if (patientsResult.Success)
                {
                    var records = new List<IZRecord>();
                    foreach (FactPatient factPatient in patientsResult.Data)
                    {
                        FactPatient patient = factPatient;
                        FactPerson person = null;
                        FactDocument document = null;

                        if (patient.PersonalId.HasValue)
                        {
                            var personResult = _repository.GetPersonById(patient.PersonalId.Value);
                            if (personResult.Success)
                            {
                                person = personResult.Data;
                            }
                        }

                        if (patient.PersonalId.HasValue)
                        {
                            var documentResult = _repository.GetDocumentByPersonId(patient.PersonalId.Value);
                            if (documentResult.Success)
                            {
                                document = documentResult.Data;
                            }
                        }

                        var record = new T4();
                        record.InnerPatient = Map.ObjectToObject<T5>(person, typeof(T5));

                        record.ExternalId = factPatient.ExternalId ?? 0;
                        if (document != null)
                        {
                            record.InnerPatient.DocNumber = document.DocNum;
                            record.InnerPatient.DocSeries = document.DocSeries;
                            record.InnerPatient.DocType = document.DocType;
                        }

                        record.InnerPatient.Newborn = patient.Newborn;
                        record.InnerPatient.InsuranceDocType = patient.InsuranceDocType;
                        record.InnerPatient.InsuranceDocSeries = patient.InsuranceDocSeries;
                        record.InnerPatient.INP = patient.INP;

                        switch (version)
                        {
                            case Constants.Version30:
                                record.InnerPatient.InsuranceDocNumber = patient.InsuranceDocNumber;

                                if (record.InnerPatient.InsuranceDocNumber.IsNullOrWhiteSpace() && patient.INP.IsNotNullOrWhiteSpace())
                                {
                                    record.InnerPatient.InsuranceDocNumber = patient.INP;
                                }
                                break;
                        }

                        var zslMeventResult = _repository.GetZslMeventsByPatientId(patient.PatientId);
                        if (zslMeventResult.Success)
                        {
                            var events = new List<IZslEvent>();
                            foreach (ZslFactMedicalEvent zslfactMedicalEvent in zslMeventResult.Data)
                            {
                                var zslMeventCopy = Map.ObjectToObject<T6>(zslfactMedicalEvent, typeof (T6));
                                //Для ЕАО то делаем подмену кода лпу
                                if (dest == "79")
                                {
                                    zslMeventCopy.MedicalOrganizationCode =
                                        _regionService.LpuEaoMapping(zslMeventCopy.MedicalOrganizationCode);
                                    zslMeventCopy.ReferralOrganizationNullable =
                                        _regionService.LpuEaoMapping(zslMeventCopy.ReferralOrganizationNullable);
                                }

                                var meventResult =
                                    _repository.GetZMeventsByZslMeventId(zslfactMedicalEvent.ZslMedicalEventId);
                                if (meventResult.Success)
                                {
                                    var mevents = new List<IZEvent>();
                                    foreach (ZFactMedicalEvent zFactMedicalEvent in meventResult.Data)
                                    {
                                        var meventCopy = Map.ObjectToObject<v30.E.EventE>(zFactMedicalEvent, typeof(v30.E.EventE));
                                        meventCopy.SpecialityClassifierVersion = "V021";
                                        meventCopy.SpecialityCodeV021 = zFactMedicalEvent.SpecialityCodeV021;
                                        
                                        //var mevent = Map.ObjectToObject<v30.E.EventE>(meventCopy);
                                        


                                        var refusals = new List<object>();
                                        decimal? totalMec = 0.0m;

                                        var refusalsMecResult = _repository.GetSankByZMedicalEventIdAndType(mec=> mec.ZmedicalEventId == zFactMedicalEvent.ZmedicalEventId);
                                        if (refusalsMecResult.Success &&
                                            refusalsMecResult.Data.Any(p => (p.IsLock == null || p.IsLock == false)))
                                        {
                                            foreach (
                                                var factsank in
                                                    refusalsMecResult.Data.Where(
                                                        p => (p.Source == RefusalSource.InterTerritorial.ToInt32() && p.Type == 1))
                                                        .Distinct(p => p.ZmedicalEventId))
                                            {
                                                ZFactSank sank = factsank;

                                                var refusal = new v30.E.RefusalE();
                                                refusal.RefusalCode = sank.ReasonId;
                                                refusal.RefusalRate = sank.Amount;
                                                refusal.RefusalSource = sank.Source;
                                                refusal.RefusalType = sank.Type;
                                                refusal.ExternalGuid = sank.ExternalGuid;
                                                refusal.Comments = sank.Comments;

                                                refusals.Add(refusal);
                                            }

                                            foreach (
                                                var factsank in
                                                    refusalsMecResult.Data.Where(p => Constants.Mee.Contains(p.Type) || Constants.Eqma.Contains(p.Type)))
                                            {
                                                ZFactSank sank = factsank;

                                                var refusal = new v30.E.RefusalE();
                                                refusal.RefusalCode = sank.ReasonId;
                                                refusal.RefusalRate = sank.Amount;
                                                refusal.RefusalSource = sank.Source;
                                                refusal.RefusalType = sank.Type;
                                                refusal.ExternalGuid = sank.ExternalGuid;
                                                refusal.Comments = sank.Comments;
                                                
                                                refusals.Add(refusal);
                                            }

                                        }
                                        meventCopy.InnerRefusals = new List<object>(refusals);

                                        //выгрузка услуг
                                        if (options.Has(Services.ExportOptions.ExportServices))
                                        {
                                            var services = new List<IService>();
                                            var serviceResult = _repository.GetZServicesByZMeventId(zFactMedicalEvent.ZmedicalEventId);
                                            if (serviceResult.Success)
                                            {
                                                foreach (ZFactMedicalServices serviceE in serviceResult.Data)
                                                {
                                                    var serviceCopy = serviceE;
                                                    var service = Map.ObjectToObject<v30.E.ServiceE>(serviceCopy);
                                                    //Для ЕАО то делаем подмену кода лпу
                                                    if (dest == "79")
                                                    {
                                                        service.MedicalOrganizationCode =
                                                            _regionService.LpuEaoMapping(service.MedicalOrganizationCode);
                                                    }
                                                    service.SpecialityCode = serviceCopy.SpecialityCodeV021;
                                                    services.Add(service);
                                                }
                                            }
                                            meventCopy.InnerServiceCollection = new List<IService>(services);
                                        }

                                        //выгрузка КСГ
                                        v30.E.KsgKpgE ksgkpg = null;
                                        var ksgKpgResult = _repository.GetZKsgKpgByZMeventId(zFactMedicalEvent.ZmedicalEventId);
                                        if (ksgKpgResult.Success)
                                        {
                                            var ksgKpgCopy = ksgKpgResult;
                                            ksgkpg = Map.ObjectToObject<v30.E.KsgKpgE>(ksgKpgCopy.Data);
                                            var slkoefResult =
                                                _repository.GetSlKoefByKsgKpgId(ksgKpgResult.Data.ZksgKpgId);
                                            ksgkpg.Kkpg = null; //У нас на територии он не выгружается
                                            var slKoefs = new List<ISlKoef>();
                                            if (slkoefResult.Success)
                                            {
                                                foreach (var zFactSlKoef in slkoefResult.Data)
                                                {
                                                    var slKoefCopy = zFactSlKoef;
                                                    var slkoef = Map.ObjectToObject<v30.E.SlKoefE>(slKoefCopy);
                                                    slKoefs.Add(slkoef);
                                                }
                                            }
                                            ksgkpg.InnerSlCoefCollection = new List<ISlKoef>(slKoefs);
                                        }
                                        if (ksgkpg != null)
                                        {
                                            meventCopy.InnerKsgKpg = ksgkpg as IKsgKpg;
                                        }

                                        mevents.Add(meventCopy);
                                    }
                                    zslMeventCopy.InnerMeventCollection = new List<IZEvent>(mevents);
                                }
                                events.Add(zslMeventCopy);
                            }
                            record.InnerZslEventCollection = new List<IZslEvent>(events);
                        }

                        records.Add(record);
                    }
                    register.InnerRecordCollection = new List<IZRecord>(records);
                    register.InnerRecordCollection = register.InnerRecordCollection.OrderBy(p => p.ExternalId).ToList();
                }


                if (!accountResult.Data.PacketNumber.HasValue)
                {
                    var packetNumberResult = _repository.GetFactExchangeLastPacketNumber(
                        exchange => exchange.Direction == accountResult.Data.Direction && 
                            exchange.Type == type && 
                            exchange.Date.Year == accountResult.Data.Date.Value.Year);
                    if (packetNumberResult.Success)
                    {
                        accountResult.Data.PacketNumber = packetNumberResult.Data;
                        accountResult.Data.PacketNumber = accountResult.Data.PacketNumber.HasValue ? accountResult.Data.PacketNumber + 1 : 1;
                    }
                }

                var f010Cache = _cache.Get(CacheRepository.F010Cache);
                var registerInfo = new RegisterEInfo
                {
                    Type = "R",
                    SourceTerritory = f010Cache.GetBackInt(accountResult.Data.Source),
                    DestinationTerritory = f010Cache.GetBackInt(accountResult.Data.Destination),
                    NumberXXXX = accountResult.Data.PacketNumber.Value,
                    YearXX = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0
                };
                
                result.Data = Tuple.Create(register, registerInfo, accountResult.Data.Direction ?? 0);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<Tuple<T1, RegisterEInfo, int>> WriteZOnk<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12,T13, T14,T15,T16,T17, T18>(int accountId, int version, int type, ExportOptions options)
            where T1 : IZRegister, new()
            where T2 : IAccount, new()
            where T3 : IHeader, new()
            where T4 : IZRecord, new()
            where T5 : IPatient, new()
            where T6 : IZslEvent, new()
            where T7 : IZEvent, new()
            where T8 : IDirectionOnkE, new()
            where T9 : IConsultationsOnk, new()
            where T10 : IEventOnk, new()
            where T11 : IDiagBlokOnk, new()
            where T12 : IContraindicationsOnk, new()
            where T13 : IServiceOnk, new()
            where T14 : IAnticancerDrugOnk, new()
            where T15 : IKsgKpg, new()
            where T16 : ISlKoef, new()
            where T17 : IRefusal, new()
            where T18 : IService, new()
        {
            var result = new OperationResult<Tuple<T1, RegisterEInfo, int>>();
            try
            {
                dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
                string dest = Convert.ToString(terCode.tf_code);

                var accountResult = _repository.GetTerritoryAccountById(accountId);
                if (accountResult.HasError)
                {
                    result.AddError(accountResult.LastError);
                    return result;
                }

                _repository.UpdateGuid(accountId);

                var register = new T1
                {
                    InnerAccount = Map.ObjectToObject<T2>(accountResult.Data, typeof(T2))
                };

                register.InnerAccount.Year = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0;
                register.InnerAccount.Month = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Month : 0;
                register.InnerHeader = new T3
                {
                    TargetOkato = accountResult.Data.Destination,
                    SourceOkato = accountResult.Data.Source,
                    Version = version,
                    Date = DateTime.Now //accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value : default(DateTime?)
                };

                var patientsResult = _repository.GetPatientsByAccountId(accountId);
                if (patientsResult.Success)
                {
                    var records = new List<IZRecord>();
                    foreach (FactPatient factPatient in patientsResult.Data)
                    {
                        FactPatient patient = factPatient;
                        FactPerson person = null;
                        FactDocument document = null;

                        if (patient.PersonalId.HasValue)
                        {
                            var personResult = _repository.GetPersonById(patient.PersonalId.Value);
                            if (personResult.Success)
                            {
                                person = personResult.Data;
                            }
                        }
                        
                        if (patient.PersonalId.HasValue)
                        {
                            var documentResult = _repository.GetDocumentByPersonId(patient.PersonalId.Value);
                            if (documentResult.Success)
                            {
                                document = documentResult.Data;
                            }
                        }

                        var record = new T4();
                        record.InnerPatient = Map.ObjectToObject<T5>(person, typeof(T5));

                        record.ExternalId = factPatient.ExternalId ?? 0;
                        if (document != null)
                        {
                            record.InnerPatient.DocNumber = document.DocNum;
                            record.InnerPatient.DocSeries = document.DocSeries;
                            record.InnerPatient.DocType = document.DocType;
                            record.InnerPatient.DocOrg = document.DocOrg;
                            record.InnerPatient.DocDate = document.DocDate;
                        }

                        if (patient.ExternalId == 86)
                        {
                            var d = 1;
                        }
                        record.InnerPatient.Newborn = patient.Newborn;
                        record.InnerPatient.InsuranceDocType = patient.InsuranceDocType;
                        record.InnerPatient.InsuranceDocSeries = patient.InsuranceDocSeries;
                        record.InnerPatient.InsuranceDocNumber = patient.InsuranceDocNumber;
                        record.InnerPatient.INP = patient.INP;
                        
                        //if (person.AddressLive.IsNotNull() && person.AddressLive.ToString().Length < 11)
                        //    record.InnerPatient.AddressLiveXml = String.Format("{0:f11}", person.AddressLive).Replace(",", "");
                        //if (person.AddressReg.IsNotNull() && person.AddressReg.ToString().Length < 11)
                        //    record.InnerPatient.AddressRegXml = String.Format("{0:f11}", person.AddressReg).Replace(",", "");
                        //record.InnerPatient.INP = patient.INP;

                        switch (version)
                        {
                            case Constants.Version30:
                                record.InnerPatient.InsuranceDocNumber = patient.InsuranceDocNumber;

                                if (record.InnerPatient.InsuranceDocNumber.IsNullOrWhiteSpace() && patient.INP.IsNotNullOrWhiteSpace())
                                {
                                    record.InnerPatient.InsuranceDocNumber = patient.INP;
                                }
                                break;
                        }

                        var zslMeventResult = _repository.GetZslMeventsByPatientId(patient.PatientId);
                        if (zslMeventResult.Success)
                        {
                            var events = new List<IZslEvent>();
                            foreach (ZslFactMedicalEvent zslfactMedicalEvent in zslMeventResult.Data)
                            {
                                var zslMeventCopy = Map.ObjectToObject<T6>(zslfactMedicalEvent, typeof(T6));
                                //Для ЕАО то делаем подмену кода лпу
                                if (dest == "79")
                                {
                                    zslMeventCopy.MedicalOrganizationCode =
                                        _regionService.LpuEaoMapping(zslMeventCopy.MedicalOrganizationCode);
                                    zslMeventCopy.ReferralOrganizationNullable =
                                        _regionService.LpuEaoMapping(zslMeventCopy.ReferralOrganizationNullable);
                                }
                                var meventResult =
                                    _repository.GetZMeventsByZslMeventId(zslfactMedicalEvent.ZslMedicalEventId);
                                if (meventResult.Success)
                                {
                                    var mevents = new List<IZEvent>();
                                    foreach (ZFactMedicalEvent zFactMedicalEvent in meventResult.Data)
                                    {
                                        var meventCopy = Map.ObjectToObject<T7>(zFactMedicalEvent, typeof(T7));
                                        meventCopy.SpecialityClassifierVersion = "V021";
                                        meventCopy.SpecialityCodeV021 = zFactMedicalEvent.SpecialityCodeV021;
                                        meventCopy.ExternalId = zFactMedicalEvent.SlIdGuid;

                                        var directionResult = _repository.GetZDirectionBySlMeventId(zFactMedicalEvent.ZmedicalEventId);
                                        if (directionResult.Success)
                                        {
                                            var directions = new List<IDirectionOnkE>();
                                            foreach (var zFactDirection in directionResult.Data)
                                            {
                                                var directionCopy = Map.ObjectToObject<T8>(zFactDirection, typeof(T8));
                                                directions.Add(directionCopy);
                                            }
                                            meventCopy.InnerDirectionOnkCollection = new List<IDirectionOnkE>(directions);
                                        }

                                        var сonsultationsResult = _repository.GetZConsultationsBySlMeventId(zFactMedicalEvent.ZmedicalEventId);
                                        if (сonsultationsResult.Success)
                                        {
                                            var consultations = new List<IConsultationsOnk>();
                                            foreach (var zFactConsultationse in сonsultationsResult.Data)
                                            {
                                                var consultationsCopy = Map.ObjectToObject<T9>(zFactConsultationse, typeof(T9));
                                                consultations.Add(consultationsCopy);
                                            }
                                            meventCopy.InnerConsultationsOnkCollection = new List<IConsultationsOnk>(consultations);
                                        }

                                        var dsesult = _repository.GetZDsBySlMeventId(zFactMedicalEvent.ZmedicalEventId);
                                        if (dsesult.Success)
                                        {
                                            var ds = new List<string>();
                                            foreach (var zFactDs in dsesult.Data)
                                            {
                                                var dsCopy = zFactDs.Ds;
                                                ds.Add(dsCopy);
                                            }
                                            meventCopy.DiagnosisSecondaryXml = new List<string>(ds);
                                        }

                                        //выгрузка услуг
                                        if (options.Has(Services.ExportOptions.ExportServices))
                                        {
                                            var services = new List<IService>();
                                            var serviceResult = _repository.GetZServicesByZMeventId(zFactMedicalEvent.ZmedicalEventId);
                                            if (serviceResult.Success)
                                            {
                                                foreach (ZFactMedicalServices serviceE in serviceResult.Data)
                                                {
                                                    var serviceCopy = serviceE;
                                                    var service = Map.ObjectToObject<T18>(serviceCopy);
                                                    //Для ЕАО то делаем подмену кода лпу
                                                    if (dest == "79")
                                                    {
                                                        service.MedicalOrganizationCode =
                                                            _regionService.LpuEaoMapping(service.MedicalOrganizationCode);
                                                    }
                                                    service.SpecialityCode = serviceCopy.SpecialityCodeV021;
                                                    services.Add(service);
                                                }
                                            }
                                            meventCopy.InnerServiceCollection = new List<IService>(services);
                                        }
                                        //выгрузка онко случай
                                        T10 eventOnk = default(T10);
                                        var eventOnkResult = _repository.GetZMedicalEventOnkByZMeventId(zFactMedicalEvent.ZmedicalEventId);
                                        if (eventOnkResult.Success)
                                        {
                                            eventOnk = Map.ObjectToObject<T10>(eventOnkResult.Data);
                                            var diagBlokResult = _repository.GetDiafBlokByMedicalEventOnkId(eventOnkResult.Data.ZMedicalEventOnkId);
                                            var contraindicationsResult = _repository.GetContraindicationsByMedicalEventOnkId(eventOnkResult.Data.ZMedicalEventOnkId);

                                            var diagBloks = new List<IDiagBlokOnk>();
                                            if (diagBlokResult.Success)
                                            {
                                                foreach (var zFactDiagBlok in diagBlokResult.Data)
                                                {
                                                    var dBlok = Map.ObjectToObject<T11>(zFactDiagBlok);
                                                    diagBloks.Add(dBlok);
                                                }
                                            }
                                            eventOnk.InnerDiagBlokOnkCollection = new List<IDiagBlokOnk>(diagBloks);

                                            var contraindications = new List<IContraindicationsOnk>();
                                            if (contraindicationsResult.Success)
                                            {
                                                foreach (var zContraindications in contraindicationsResult.Data)
                                                {
                                                    var contraindication = Map.ObjectToObject<T12>(zContraindications);
                                                    contraindications.Add(contraindication);
                                                }
                                            }
                                            eventOnk.InnerСontraindicationsOnkCollection = new List<IContraindicationsOnk>(contraindications);

                                            var serviceOnks = new List<IServiceOnk>();
                                            var serviceOnkResult = _repository.GetZMedicalServiceOnkByMedicalEventId(eventOnkResult.Data.ZMedicalEventOnkId);
                                            if (serviceOnkResult.Success)
                                            {
                                                foreach (var zFactMedicalServicesOnk in serviceOnkResult.Data)
                                                {
                                                    var serviceOnkMap = Map.ObjectToObject<T13>(zFactMedicalServicesOnk);

                                                    var lekPrResult = _repository.GetAnticancerDrugByMedicalServiceOnkId(zFactMedicalServicesOnk.ZmedicalServicesOnkId);

                                                    var lekPrs = new List<IAnticancerDrugOnk>();
                                                    if (lekPrResult.Success)
                                                    {
                                                        foreach (var zFactAnticancerDrug in lekPrResult.Data)
                                                        {
                                                            var lekpr = Map.ObjectToObject<T14>(zFactAnticancerDrug);
                                                            lekPrs.Add(lekpr);
                                                        }
                                                        serviceOnkMap.InnerAnticancerDrugOnkCollection = new List<IAnticancerDrugOnk>(lekPrs);
                                                    }
                                                    serviceOnks.Add(serviceOnkMap);
                                                }
                                                eventOnk.InnerServiceOnkCollection = serviceOnks;
                                            }
                                            meventCopy.InnerEventOnk = eventOnk;
                                        }
                                        
                                        //выгрузка КСГ
                                        T15 ksgkpg = default(T15);
                                        var ksgKpgResult = _repository.GetZKsgKpgByZMeventId(zFactMedicalEvent.ZmedicalEventId);
                                        if (ksgKpgResult.Success)
                                        {
                                            var ksgKpgCopy = ksgKpgResult;
                                            ksgkpg = Map.ObjectToObject<T15>(ksgKpgCopy.Data);

                                            var cirtResult = _repository.GetCritByksgKpgId(ksgKpgResult.Data.ZksgKpgId);
                                            if (cirtResult.Success)
                                            {
                                                ksgkpg.CritXml = new List<string>();
                                                foreach (var zFactCrit in cirtResult.Data)
                                                {
                                                    ksgkpg.CritXml.Add(zFactCrit.IdDkk);
                                                }
                                            }

                                            var slkoefResult = _repository.GetSlKoefByKsgKpgId(ksgKpgResult.Data.ZksgKpgId);
                                            ksgkpg.Kkpg = null; //У нас на територии он не выгружается
                                            var slKoefs = new List<ISlKoef>();
                                            if (slkoefResult.Success)
                                            {
                                                foreach (var zFactSlKoef in slkoefResult.Data)
                                                {
                                                    var slKoefCopy = zFactSlKoef;
                                                    var slkoef = Map.ObjectToObject<T16>(slKoefCopy);
                                                    slKoefs.Add(slkoef);
                                                }
                                            }
                                            ksgkpg.InnerSlCoefCollection = new List<ISlKoef>(slKoefs);
                                        }
                                        if (ksgkpg != null)
                                        {
                                            meventCopy.InnerKsgKpg = new T15();
                                            meventCopy.InnerKsgKpg = ksgkpg;
                                        }

                                        mevents.Add(meventCopy);
                                    }
                                    zslMeventCopy.InnerMeventCollection = new List<IZEvent>(mevents);
                                }

                                var refusals = new List<IRefusal>();
                                decimal? totalMec = 0.0m;

                                var refusalsMecResult = _repository.GetSankByZMedicalEventIdAndType(mec => mec.ZslMedicalEventId == zslfactMedicalEvent.ZslMedicalEventId);
                                if (refusalsMecResult.Success &&
                                    refusalsMecResult.Data.Any(p => (p.IsLock == null || p.IsLock == false)))
                                {
                                    foreach (
                                        var factMec in
                                            refusalsMecResult.Data.Where(p => (p.Source == RefusalSource.InterTerritorial.ToInt32() && p.Type == 1))
                                            .Distinct(p => p.ZmedicalEventId))
                                    {
                                        ZFactSank mec = factMec;
                                        var guidmeventId = _repository.GetZMedicalEventBySlidGuid(factMec.ZmedicalEventId);

                                        var refusal = new T17();
                                        refusal.RefusalCode = mec.ReasonId;
                                        refusal.RefusalRate = mec.Amount;
                                        refusal.RefusalSource = mec.Source;
                                        refusal.RefusalType = factMec.Type;
                                        refusal.ExternalGuid = mec.ExternalGuid;
                                        refusal.SlidGuid = guidmeventId.Data;
                                        refusal.Comments = mec.Comments;

                                        refusals.Add(refusal);
                                    }

                                    foreach (
                                        var factMec in
                                            refusalsMecResult.Data.Where(p => Constants.Mee.Contains(p.Type) || Constants.Eqma.Contains(p.Type)))
                                    {
                                        ZFactSank sank = factMec;
                                        var guidmeventId = _repository.GetZMedicalEventBySlidGuid(factMec.ZmedicalEventId);

                                        var refusal = new T17();
                                        refusal.RefusalCode = sank.ReasonId;
                                        refusal.RefusalRate = sank.Amount;
                                        refusal.Date = sank.Date;
                                        refusal.NumAct = sank.NumAct;
                                        refusal.CodeExp = sank.CodeExp;
                                        refusal.RefusalSource = sank.Source;
                                        refusal.RefusalType = factMec.Type;
                                        refusal.ExternalGuid = sank.ExternalGuid;
                                        refusal.SlidGuid = guidmeventId.Data;
                                        refusal.Comments = sank.Comments;
                                        refusals.Add(refusal);
                                    }
                                }
                                zslMeventCopy.InnerRefusals = new List<IRefusal> (refusals);
                                if (!zslMeventCopy.InnerRefusals.Any())
                                {
                                    zslMeventCopy.RefusalPrice = null;
                                }
                                events.Add(zslMeventCopy);
                            }
                            record.InnerZslEventCollection = new List<IZslEvent>(events);
                        }

                        records.Add(record);
                    }
                    register.InnerRecordCollection = new List<IZRecord>(records);
                    register.InnerRecordCollection = register.InnerRecordCollection.OrderBy(p => p.ExternalId).ToList();
                }


                if (!accountResult.Data.PacketNumber.HasValue)
                {
                    var packetNumberResult = _repository.GetFactExchangeLastPacketNumber(
                        exchange => exchange.Direction == accountResult.Data.Direction &&
                            exchange.Type == type &&
                            exchange.Date.Year == accountResult.Data.Date.Value.Year);
                    if (packetNumberResult.Success)
                    {
                        accountResult.Data.PacketNumber = packetNumberResult.Data;
                        accountResult.Data.PacketNumber = accountResult.Data.PacketNumber.HasValue ? accountResult.Data.PacketNumber + 1 : 1;
                    }
                }

                var f010Cache = _cache.Get(CacheRepository.F010Cache);
                var registerInfo = new RegisterEInfo
                {
                    Type = "R",
                    SourceTerritory = f010Cache.GetBackInt(accountResult.Data.Source),
                    DestinationTerritory = f010Cache.GetBackInt(accountResult.Data.Destination),
                    NumberXXXX = accountResult.Data.PacketNumber.Value,
                    YearXX = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0
                };

                result.Data = Tuple.Create(register, registerInfo, accountResult.Data.Direction ?? 0);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }


        //public OperationResult<Tuple<T1, RegisterEInfo, int>> WriteZOnk<T1, T2, T3, T4, T5, T6, T7>(int accountId, int version, int type, ExportOptions options)
        //    where T1 : IZRegister, new()
        //    where T2 : IAccount, new()
        //    where T3 : IHeader, new()
        //    where T4 : IZRecord, new()
        //    where T5 : IPatient, new()
        //    where T6 : IZslEvent, new()
        //    where T7 : IZEvent, new()
        //{
        //    var result = new OperationResult<Tuple<T1, RegisterEInfo, int>>();
        //    try
        //    {
        //        dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
        //        string dest = Convert.ToString(terCode.tf_code);

        //        var accountResult = _repository.GetTerritoryAccountById(accountId);
        //        if (accountResult.HasError)
        //        {
        //            result.AddError(accountResult.LastError);
        //            return result;
        //        }

        //        _repository.UpdateGuid(accountId);

        //        var register = new T1
        //        {
        //            InnerAccount = Map.ObjectToObject<T2>(accountResult.Data, typeof(T2))
        //        };

        //        register.InnerAccount.Year = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0;
        //        register.InnerAccount.Month = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Month : 0;
        //        register.InnerHeader = new T3
        //        {
        //            TargetOkato = accountResult.Data.Destination,
        //            SourceOkato = accountResult.Data.Source,
        //            Version = version,
        //            Date = DateTime.Now //accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value : default(DateTime?)
        //        };

        //        var patientsResult = _repository.GetPatientsByAccountId(accountId);
        //        if (patientsResult.Success)
        //        {
        //            var records = new List<IZRecord>();
        //            foreach (FactPatient factPatient in patientsResult.Data)
        //            {
        //                FactPatient patient = factPatient;
        //                FactPerson person = null;
        //                FactDocument document = null;

        //                if (patient.PersonalId.HasValue)
        //                {
        //                    var personResult = _repository.GetPersonById(patient.PersonalId.Value);
        //                    if (personResult.Success)
        //                    {
        //                        person = personResult.Data;
        //                    }
        //                }

        //                if (patient.PersonalId.HasValue)
        //                {
        //                    var documentResult = _repository.GetDocumentByPersonId(patient.PersonalId.Value);
        //                    if (documentResult.Success)
        //                    {
        //                        document = documentResult.Data;
        //                    }
        //                }

        //                var record = new T4();
        //                record.InnerPatient = Map.ObjectToObject<T5>(person, typeof(T5));

        //                record.ExternalId = factPatient.ExternalId ?? 0;
        //                if (document != null)
        //                {
        //                    record.InnerPatient.DocNumber = document.DocNum;
        //                    record.InnerPatient.DocSeries = document.DocSeries;
        //                    record.InnerPatient.DocType = document.DocType;
        //                }

        //                record.InnerPatient.Newborn = patient.Newborn;
        //                record.InnerPatient.InsuranceDocType = patient.InsuranceDocType;
        //                record.InnerPatient.InsuranceDocSeries = patient.InsuranceDocSeries;
        //                record.InnerPatient.INP = patient.INP;

        //                switch (version)
        //                {
        //                    case Constants.Version30:
        //                        record.InnerPatient.InsuranceDocNumber = patient.InsuranceDocNumber;

        //                        if (record.InnerPatient.InsuranceDocNumber.IsNullOrWhiteSpace() && patient.INP.IsNotNullOrWhiteSpace())
        //                        {
        //                            record.InnerPatient.InsuranceDocNumber = patient.INP;
        //                        }
        //                        break;
        //                }

        //                var zslMeventResult = _repository.GetZslMeventsByPatientId(patient.PatientId);
        //                if (zslMeventResult.Success)
        //                {
        //                    var events = new List<IZslEvent>();
        //                    foreach (ZslFactMedicalEvent zslfactMedicalEvent in zslMeventResult.Data)
        //                    {
        //                        var zslMeventCopy = Map.ObjectToObject<v31.E.ZslEventE>(zslfactMedicalEvent, typeof(T6));
        //                        //Для ЕАО то делаем подмену кода лпу
        //                        if (dest == "79")
        //                        {
        //                            zslMeventCopy.MedicalOrganizationCode =
        //                                _regionService.LpuEaoMapping(zslMeventCopy.MedicalOrganizationCode);
        //                            zslMeventCopy.ReferralOrganizationNullable =
        //                                _regionService.LpuEaoMapping(zslMeventCopy.ReferralOrganizationNullable);
        //                        }
        //                        var meventResult =
        //                            _repository.GetZMeventsByZslMeventId(zslfactMedicalEvent.ZslMedicalEventId);
        //                        if (meventResult.Success)
        //                        {
        //                            var mevents = new List<IZEvent>();
        //                            foreach (ZFactMedicalEvent zFactMedicalEvent in meventResult.Data)
        //                            {
        //                                var meventCopy = Map.ObjectToObject<v31.E.EventE>(zFactMedicalEvent, typeof(v31.E.EventE));
        //                                meventCopy.SpecialityClassifierVersion = "V021";
        //                                meventCopy.SpecialityCodeV021 = zFactMedicalEvent.SpecialityCodeV021;
        //                                meventCopy.ExternalId = zFactMedicalEvent.SlIdGuid;

        //                                var directionResult = _repository.GetZDirectionBySlMeventId(zFactMedicalEvent.ZmedicalEventId);
        //                                if (directionResult.Success)
        //                                {
        //                                    meventCopy.DirectionOnkXml = new List<DirectionOnkE>();
        //                                    foreach (var zFactDirection in directionResult.Data)
        //                                    {
        //                                        var directionCopy = Map.ObjectToObject<v31.E.DirectionOnkE>(zFactDirection, typeof(v31.E.DirectionOnkE));
        //                                        meventCopy.DirectionOnkXml.Add(directionCopy);
        //                                    }
        //                                }

        //                                var сonsultationsResult = _repository.GetZConsultationsBySlMeventId(zFactMedicalEvent.ZmedicalEventId);
        //                                if (сonsultationsResult.Success)
        //                                {
        //                                    meventCopy.ConsultationsOnkXml = new List<ConsultationsOnkE>();
        //                                    foreach (var zFactConsultationse in сonsultationsResult.Data)
        //                                    {
        //                                        var consultationsCopy = Map.ObjectToObject<v31.E.ConsultationsOnkE>(zFactConsultationse, typeof(v31.E.ConsultationsOnkE));
        //                                        meventCopy.ConsultationsOnkXml.Add(consultationsCopy);
        //                                    }
        //                                }

        //                                //выгрузка услуг
        //                                if (options.Has(Services.ExportOptions.ExportServices))
        //                                {
        //                                    var services = new List<IService>();
        //                                    var serviceResult = _repository.GetZServicesByZMeventId(zFactMedicalEvent.ZmedicalEventId);
        //                                    if (serviceResult.Success)
        //                                    {
        //                                        foreach (ZFactMedicalServices serviceE in serviceResult.Data)
        //                                        {
        //                                            var serviceCopy = serviceE;
        //                                            var service = Map.ObjectToObject<v31.E.ServiceE>(serviceCopy);
        //                                            //Для ЕАО то делаем подмену кода лпу
        //                                            if (dest == "79")
        //                                            {
        //                                                service.MedicalOrganizationCode =
        //                                                    _regionService.LpuEaoMapping(service.MedicalOrganizationCode);
        //                                            }
        //                                            service.SpecialityCode = serviceCopy.SpecialityCodeV021;
        //                                            services.Add(service);
        //                                        }
        //                                    }
        //                                    meventCopy.InnerServiceCollection = new List<IService>(services);
        //                                }
        //                                //выгрузка онко случай
        //                                v31.E.EventOnkE eventOnk = null;
        //                                var eventOnkResult = _repository.GetZMedicalEventOnkByZMeventId(zFactMedicalEvent.ZmedicalEventId);
        //                                if (eventOnkResult.Success)
        //                                {
        //                                    eventOnk = Map.ObjectToObject<v31.E.EventOnkE>(eventOnkResult.Data);
        //                                    var diagBlokResult = _repository.GetDiafBlokByMedicalEventOnkId(eventOnkResult.Data.ZMedicalEventOnkId);
        //                                    var contraindicationsResult = _repository.GetContraindicationsByMedicalEventOnkId(eventOnkResult.Data.ZMedicalEventOnkId);

        //                                    var diagBloks = new List<v31.E.DiagBlokOnkE>();
        //                                    if (diagBlokResult.Success)
        //                                    {
        //                                        foreach (var zFactDiagBlok in diagBlokResult.Data)
        //                                        {
        //                                            var dBlok = Map.ObjectToObject<v31.E.DiagBlokOnkE>(zFactDiagBlok);
        //                                            diagBloks.Add(dBlok);
        //                                        }
        //                                    }
        //                                    eventOnk.DiagBlokOnk = new List<v31.E.DiagBlokOnkE>(diagBloks);
        //                                    var contraindications = new List<v31.E.ContraindicationsOnkE>();
        //                                    if (contraindicationsResult.Success)
        //                                    {
        //                                        foreach (var zContraindications in contraindicationsResult.Data)
        //                                        {
        //                                            var contraindication = Map.ObjectToObject<v31.E.ContraindicationsOnkE>(zContraindications);
        //                                            contraindications.Add(contraindication);
        //                                        }
        //                                    }
        //                                    eventOnk.СontraindicationsOnk = new List<v31.E.ContraindicationsOnkE>(contraindications);

        //                                    var serviceOnks = new List<v31.E.ServiceOnkE>();
        //                                    var serviceOnkResult = _repository.GetZMedicalServiceOnkByMedicalEventId(eventOnkResult.Data.ZMedicalEventOnkId);
        //                                    if (serviceOnkResult.Success)
        //                                    {
        //                                        foreach (var zFactMedicalServicesOnk in serviceOnkResult.Data)
        //                                        {
        //                                            var serviceOnkMap = Map.ObjectToObject<v31.E.ServiceOnkE>(zFactMedicalServicesOnk);

        //                                            var lekPrResult = _repository.GetAnticancerDrugByMedicalServiceOnkId(zFactMedicalServicesOnk.ZmedicalServicesOnkId);

        //                                            var lekPrs = new List<v31.E.AnticancerDrugOnkE>();
        //                                            if (lekPrResult.Success)
        //                                            {
        //                                                foreach (var zFactAnticancerDrug in lekPrResult.Data)
        //                                                {
        //                                                    var lekpr = Map.ObjectToObject<v31.E.AnticancerDrugOnkE>(zFactAnticancerDrug);
        //                                                    lekPrs.Add(lekpr);
        //                                                }
        //                                                serviceOnkMap.AnticancerDrugOnkXml = lekPrs;
        //                                            }
        //                                            serviceOnks.Add(serviceOnkMap);
        //                                        }
        //                                        eventOnk.ServiceOnk = serviceOnks;
        //                                    }
        //                                    meventCopy.EventOnk = eventOnk;
        //                                }

        //                                //выгрузка КСГ
        //                                v31.E.KsgKpgE ksgkpg = null;
        //                                var ksgKpgResult = _repository.GetZKsgKpgByZMeventId(zFactMedicalEvent.ZmedicalEventId);
        //                                if (ksgKpgResult.Success)
        //                                {
        //                                    var ksgKpgCopy = ksgKpgResult;
        //                                    ksgkpg = Map.ObjectToObject<v31.E.KsgKpgE>(ksgKpgCopy.Data);

        //                                    var cirtResult = _repository.GetCritByksgKpgId(ksgKpgResult.Data.ZksgKpgId);
        //                                    if (cirtResult.Success)
        //                                    {
        //                                        ksgkpg.CritXml = new List<string>();
        //                                        foreach (var zFactCrit in cirtResult.Data)
        //                                        {
        //                                            ksgkpg.CritXml.Add(zFactCrit.IdDkk);
        //                                        }
        //                                    }

        //                                    var slkoefResult = _repository.GetSlKoefByKsgKpgId(ksgKpgResult.Data.ZksgKpgId);
        //                                    ksgkpg.Kkpg = null; //У нас на територии он не выгружается
        //                                    var slKoefs = new List<ISlKoef>();
        //                                    if (slkoefResult.Success)
        //                                    {
        //                                        foreach (var zFactSlKoef in slkoefResult.Data)
        //                                        {
        //                                            var slKoefCopy = zFactSlKoef;
        //                                            var slkoef = Map.ObjectToObject<v31.E.SlKoefE>(slKoefCopy);
        //                                            slKoefs.Add(slkoef);
        //                                        }
        //                                    }
        //                                    ksgkpg.InnerSlCoefCollection = new List<ISlKoef>(slKoefs);
        //                                }
        //                                if (ksgkpg != null)
        //                                {
        //                                    meventCopy.InnerKsgKpg = ksgkpg;
        //                                }

        //                                mevents.Add(meventCopy);
        //                            }
        //                            zslMeventCopy.InnerMeventCollection = new List<IZEvent>(mevents);
        //                        }

        //                        var refusals = new List<v31.E.RefusalE>();
        //                        decimal? totalMec = 0.0m;

        //                        var refusalsMecResult = _repository.GetSankByZMedicalEventIdAndType(mec => mec.ZslMedicalEventId == zslfactMedicalEvent.ZslMedicalEventId);
        //                        if (refusalsMecResult.Success &&
        //                            refusalsMecResult.Data.Any(p => (p.IsLock == null || p.IsLock == false)))
        //                        {
        //                            foreach (
        //                                var factMec in
        //                                    refusalsMecResult.Data.Where(p => (p.Source == RefusalSource.InterTerritorial.ToInt32() && p.Type == 1))
        //                                    .Distinct(p => p.ZmedicalEventId))
        //                            {
        //                                ZFactSank mec = factMec;
        //                                var guidmeventId = _repository.GetZMedicalEventBySlidGuid(factMec.ZmedicalEventId);

        //                                var refusal = new v31.E.RefusalE();
        //                                refusal.RefusalCode = mec.ReasonId;
        //                                refusal.RefusalRate = mec.Amount;
        //                                refusal.RefusalSource = mec.Source;
        //                                refusal.RefusalType = factMec.Type;
        //                                refusal.ExternalGuid = mec.ExternalGuid;
        //                                refusal.SlidGuid = guidmeventId.Data;
        //                                refusal.Comments = mec.Comments;

        //                                refusals.Add(refusal);
        //                            }

        //                            foreach (
        //                                var factMec in
        //                                    refusalsMecResult.Data.Where(p => Constants.Mee.Contains(p.Type) || Constants.Eqma.Contains(p.Type)))
        //                            {
        //                                ZFactSank sank = factMec;
        //                                var guidmeventId = _repository.GetZMedicalEventBySlidGuid(factMec.ZmedicalEventId);

        //                                var refusal = new v31.E.RefusalE();
        //                                refusal.RefusalCode = sank.ReasonId;
        //                                refusal.RefusalRate = sank.Amount;
        //                                refusal.Date = sank.Date;
        //                                refusal.NumAct = sank.NumAct;
        //                                refusal.CodeExp = sank.CodeExp;
        //                                refusal.RefusalSource = sank.Source;
        //                                refusal.RefusalType = factMec.Type;
        //                                refusal.ExternalGuid = sank.ExternalGuid;
        //                                refusal.SlidGuid = guidmeventId.Data;
        //                                refusal.Comments = sank.Comments;
        //                                refusals.Add(refusal);
        //                            }
        //                        }
        //                        zslMeventCopy.RefusalsXml = new List<v31.E.RefusalE>(refusals);

        //                        events.Add(zslMeventCopy);
        //                    }
        //                    record.InnerZslEventCollection = new List<IZslEvent>(events);
        //                }

        //                records.Add(record);
        //            }
        //            register.InnerRecordCollection = new List<IZRecord>(records);
        //            register.InnerRecordCollection = register.InnerRecordCollection.OrderBy(p => p.ExternalId).ToList();
        //        }


        //        if (!accountResult.Data.PacketNumber.HasValue)
        //        {
        //            var packetNumberResult = _repository.GetFactExchangeLastPacketNumber(
        //                exchange => exchange.Direction == accountResult.Data.Direction &&
        //                    exchange.Type == type &&
        //                    exchange.Date.Year == accountResult.Data.Date.Value.Year);
        //            if (packetNumberResult.Success)
        //            {
        //                accountResult.Data.PacketNumber = packetNumberResult.Data;
        //                accountResult.Data.PacketNumber = accountResult.Data.PacketNumber.HasValue ? accountResult.Data.PacketNumber + 1 : 1;
        //            }
        //        }

        //        var f010Cache = _cache.Get(CacheRepository.F010Cache);
        //        var registerInfo = new RegisterEInfo
        //        {
        //            Type = "R",
        //            SourceTerritory = f010Cache.GetBackInt(accountResult.Data.Source),
        //            DestinationTerritory = f010Cache.GetBackInt(accountResult.Data.Destination),
        //            NumberXXXX = accountResult.Data.PacketNumber.Value,
        //            YearXX = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0
        //        };

        //        result.Data = Tuple.Create(register, registerInfo, accountResult.Data.Direction ?? 0);
        //    }
        //    catch (Exception exception)
        //    {
        //        result.AddError(exception);
        //    }

        //    return result;
        //}

        public OperationResult<Tuple<T1, RegisterEInfo, int>> WriteBack<T1, T2, T3, T4, T5, T6>(int accountId, int version, int type, ExportOptions options)
            where T1 : IRegister, new()
            where T2 : IAccount, new()
            where T3 : IHeader, new()
            where T4 : IRecord, new()
            where T5 : IPatient, new()
            where T6 : IMEvent, new()
        {
            var result = new OperationResult<Tuple<T1, RegisterEInfo, int>>();
            try
            {
                var accountResult = _repository.GetTerritoryAccountById(accountId);
                if (accountResult.HasError)
                {
                    result.AddError(accountResult.LastError);
                    return result;
                }

                var externalRefuseResult = _repository.GetExternalRefusesByAccountId(accountId);
                if (externalRefuseResult.Success && !externalRefuseResult.Data.Any())
                {
                    result.AddError("Для реестра счетов ID {0} (исправленная часть) нечего выгружать, нет отказов с территории.".F(accountId));
                    return result;
                }

                _repository.UpdateGuid(accountId);

                var register = new T1
                {
                    InnerAccount = Map.ObjectToObject<T2>(accountResult.Data, typeof(T2))
                };

                register.InnerAccount.Year = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0;
                register.InnerAccount.Month = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Month : 0;
                register.InnerHeader = new T3
                {
                    TargetOkato = accountResult.Data.Destination,
                    SourceOkato = accountResult.Data.Source,
                    Version = version,
                    Date = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value : default(DateTime?)
                };

                var parentAccountResult = _repository.GetParentTerritoryAccountByParentId(accountResult.Data.Parent);
                if (parentAccountResult.HasError)
                {
                    result.AddError(parentAccountResult.LastError);
                    return result;
                }

                if (parentAccountResult.Data == null)
                {
                    register.InnerHeader.Date = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value : default(DateTime?);
                }
                else
                {
                    register.InnerHeader.Date = parentAccountResult.Data.Date.HasValue ? parentAccountResult.Data.Date.Value : default(DateTime?);

                }



                var patientsResult = _repository.GetPatientsByAccountId(accountId);
                if (patientsResult.Success)
                {
                    var records = new List<IRecord>();
                    foreach (FactPatient factPatient in patientsResult.Data)
                    {
                        FactPatient patient = factPatient;
                        FactPerson person = null;
                        FactDocument document = null;

                        if (patient.PersonalId.HasValue)
                        {
                            var personResult = _repository.GetPersonById(patient.PersonalId.Value);
                            if (personResult.Success)
                            {
                                person = personResult.Data;
                            }
                        }

                        if (patient.PersonalId.HasValue)
                        {
                            var documentResult = _repository.GetDocumentByPersonId(patient.PersonalId.Value);
                            if (documentResult.Success)
                            {
                                document = documentResult.Data;
                            }
                        }

                        var checkRefusalsExternalExistsResult = _repository.GetExternalRefuseByPatientId(factPatient.PatientId);
                        if (checkRefusalsExternalExistsResult.Success && checkRefusalsExternalExistsResult.Data.Any())
                        {
                            var record = new T4();
                            record.InnerPatient = Map.ObjectToObject<T5>(person, typeof(T5));
                            
                            record.ExternalId = factPatient.ExternalId ?? 0;
                            if (document != null)
                            {
                                record.InnerPatient.DocNumber = document.DocNum;
                                record.InnerPatient.DocSeries = document.DocSeries;
                                record.InnerPatient.DocType = document.DocType;
                            }

                            record.InnerPatient.Newborn = patient.Newborn;
                            record.InnerPatient.InsuranceDocType = patient.InsuranceDocType;
                            record.InnerPatient.InsuranceDocSeries = patient.InsuranceDocSeries;

                            switch (version)
                            {
                                case Constants.Version10:
                                    record.InnerPatient.PolicyNumber = patient.InsuranceDocType == 3 ? patient.INP : patient.InsuranceDocNumber;
                                    break;
                                case Constants.Version21:
                                    record.InnerPatient.InsuranceDocNumber = patient.InsuranceDocNumber;
                                    record.InnerPatient.INP = patient.INP;

                                    if (record.InnerPatient.InsuranceDocNumber.IsNullOrWhiteSpace() && patient.INP.IsNotNullOrWhiteSpace())
                                    {
                                        record.InnerPatient.InsuranceDocNumber = record.InnerPatient.INP;
                                    }

                                    break;
                            }

                            var meventResult = _repository.GetMeventsByPatientId(patient.PatientId);
                            if (meventResult.Success)
                            {
                                var events = new List<IMEvent>();
                                foreach (FactMedicalEvent factMedicalEvent in meventResult.Data)
                                {
                                    factMedicalEvent.DoctorId = null;
                                    var meventCopy = Map.ObjectToObject<T6>(factMedicalEvent, typeof(T6));

                                    if (version == Constants.Version21)
                                    {
                                        if (!factMedicalEvent.SpecialityCodeV015.HasValue)
                                        {
                                            meventCopy.SpecialityCode = factMedicalEvent.SpecialityCode;
                                        }
                                        else
                                        {
                                            meventCopy.SpecialityClassifierVersion = "V015";
                                            meventCopy.SpecialityCodeV015 = factMedicalEvent.SpecialityCodeV015;
                                        }

                                    }
                                    meventCopy.RefusalPrice = 0.0m;

                                    var refusals = new List<object>();
                                    decimal? totalMec = 0.0m;
                                    decimal? totalMee = 0.0m;
                                    decimal? totalEqma = 0.0m;

                                    var refusalsMecResult = _repository.GetMecByMedicalEventId(factMedicalEvent.MedicalEventId);
                                    if (refusalsMecResult.Success &&
                                        refusalsMecResult.Data.Any(p => (p.IsLock == null || p.IsLock == false)))
                                    {
                                        switch (version)
                                        {
                                            case Constants.Version10:
                                                refusals.AddRange(refusalsMecResult.Data
                                                    .Where(p => (p.IsLock == null || p.IsLock == false) &&
                                                        p.ReasonId.HasValue &&
                                                        p.Source == (int)RefusalSource.InterTerritorialTotal)
                                                    .Select(p => p.ReasonId.Value as object)
                                                    .ToList());
                                                break;
                                            case Constants.Version21:
                                                foreach (var factMec in refusalsMecResult.Data.Where(p => p.Source == RefusalSource.InterTerritorialTotal.ToInt32()).Distinct(p => p.MedicalEventId))
                                                {
                                                    FactMEC mec = factMec;

                                                    var refusal = new v21.E.RefusalE();
                                                    refusal.RefusalCode = mec.ReasonId;
                                                    refusal.RefusalRate = mec.Amount;
                                                    refusal.RefusalSource = mec.Source;
                                                    refusal.RefusalType = (int)RefusalType.MEC;
                                                    refusal.ExternalGuid = mec.ExternalGuid;
                                                    refusal.Comments = mec.Comments;

                                                    refusals.Add(refusal);
                                                }
                                                totalMec += refusalsMecResult.Data.Sum(p => p.Amount) ?? 0;
                                                break;
                                        }

                                    }

                                    var refusalsExternalResult = _repository.GetExternalRefuseByMedicalEventId(factMedicalEvent.MedicalEventId);
                                    if (refusalsExternalResult.Success)
                                    {
                                        switch (version)
                                        {
                                            case Constants.Version10:
                                                refusals.AddRange(refusalsExternalResult.Data
                                                    .Where(p=>p.IsAgree == false)
                                                    .Select(p => p.ReasonId as object)
                                                    .ToList());
                                                break;
                                            case Constants.Version21:
                                                foreach (var refuse in refusalsExternalResult.Data.Where(p => p.IsAgree == true))
                                                {
                                                    var refuseCopy = refuse;
                                                    var refusal = new v21.E.RefusalE
                                                    {
                                                        RefusalCode = refuseCopy.ReasonId,
                                                        RefusalRate = refuseCopy.Amount,
                                                        RefusalSource = refuseCopy.Source,
                                                        RefusalType = refuseCopy.Type,
                                                        ExternalGuid = refuseCopy.ExternalGuid,
                                                        Comments = refuseCopy.Comment
                                                    };

                                                    refusals.Add(refusal);
                                                }
                                                totalMec += refusalsExternalResult.Data.Where(p => p.Type == (int)RefusalType.MEC && p.IsAgree == true).Sum(p => p.Amount) ?? 0;
                                                totalMee += refusalsExternalResult.Data.Where(p => p.Type == (int)RefusalType.MEE && p.IsAgree == true).Sum(p => p.Amount) ?? 0;
                                                totalEqma += refusalsExternalResult.Data.Where(p => p.Type == (int)RefusalType.EQMA && p.IsAgree == true).Sum(p => p.Amount) ?? 0;
                                                break;
                                        }
                                    }

                                    var refusalsMeeResult = _repository.GetMeeByMedicalEventId(factMedicalEvent.MedicalEventId);
                                    if (refusalsMeeResult.Success)
                                    {
                                        switch (version)
                                        {
                                            case Constants.Version10:
                                                refusals.AddRange(refusalsMeeResult.Data
                                                    .Select(p => p.ReasonId as object)
                                                    .ToList());
                                                break;
                                            case Constants.Version21:
                                                foreach (var refuse in refusalsMeeResult.Data)
                                                {
                                                    var refuseCopy = refuse;
                                                    var refusal = new v21.E.RefusalE
                                                    {
                                                        RefusalCode = refuseCopy.ReasonId,
                                                        RefusalRate = refuseCopy.Amount,
                                                        RefusalSource = refuseCopy.Source,
                                                        RefusalType = (int)RefusalType.MEE,
                                                        ExternalGuid = refuseCopy.ExternalGuid,
                                                        Comments = refuseCopy.Comments
                                                    };

                                                    refusals.Add(refusal);
                                                }
                                                totalMee += refusalsMeeResult.Data.Sum(p => p.Amount) ?? 0;
                                                break;
                                        }
                                    }

                                    var refusalsEqmaResult = _repository.GetEqmaByMedicalEventId(factMedicalEvent.MedicalEventId);
                                    if (refusalsEqmaResult.Success)
                                    {
                                        switch (version)
                                        {
                                            case Constants.Version10:
                                                refusals.AddRange(refusalsEqmaResult.Data
                                                    .Select(p => p.ReasonId as object)
                                                    .ToList());
                                                break;
                                            case Constants.Version21:
                                                foreach (var refuse in refusalsEqmaResult.Data)
                                                {
                                                    var refuseCopy = refuse;
                                                    var refusal = new v21.E.RefusalE
                                                    {
                                                        RefusalCode = refuseCopy.ReasonId,
                                                        RefusalRate = refuseCopy.Amount,
                                                        RefusalSource = refuseCopy.Source,
                                                        RefusalType = (int)RefusalType.EQMA,
                                                        ExternalGuid = refuseCopy.ExternalGuid,
                                                        Comments = refuseCopy.Comments
                                                    };

                                                    refusals.Add(refusal);
                                                }
                                                totalEqma += refusalsEqmaResult.Data.Sum(p => p.Amount) ?? 0;
                                                break;
                                        }
                                    }

 
                                    meventCopy.InnerRefusals = new List<object>(refusals);
                                    meventCopy.RefusalPrice = Math.Min(totalMec.Value, meventCopy.Price??0) + totalMee + totalEqma;

                                    if (options.Has(Services.ExportOptions.ExportServices))
                                    {
                                        var services = new List<IService>();
                                        var serviceResult = _repository.GetServicesByMeventId(factMedicalEvent.MedicalEventId);
                                        if (serviceResult.Success)
                                        {
                                            foreach (FactMedicalServices serviceE in serviceResult.Data)
                                            {
                                                var serviceCopy = serviceE;
                                                var service = Map.ObjectToObject<v21.E.ServiceE>(serviceCopy);
                                                if (!service.MedicalOrganization.HasValue)
                                                {
                                                    service.MedicalOrganizationXml = factMedicalEvent.MedicalOrganizationCode;
                                                }
                                                service.SpecialityCode = serviceCopy.SpecialityCodeV015;
                                                services.Add(service);
                                            }
                                        }
                                        meventCopy.InnerServiceCollection = new List<IService>(services);
                                    }

                                    events.Add(meventCopy);
                                }
                                record.InnerEventCollection = new List<IMEvent>(events);
                            }
                            records.Add(record);
                        }
                    }

                    register.InnerRecordCollection = new List<IRecord>(records);
                    register.InnerRecordCollection = register.InnerRecordCollection.OrderBy(p => p.ExternalId).ToList();
                }


                if (!accountResult.Data.PacketNumber.HasValue)
                {
                    var packetNumberResult = _repository.GetFactExchangeLastPacketNumber(
                        exchange => exchange.Direction == accountResult.Data.Direction &&
                            exchange.Type == type &&
                            exchange.Date.Year == accountResult.Data.Date.Value.Year);
                    if (packetNumberResult.Success)
                    {
                        accountResult.Data.PacketNumber = packetNumberResult.Data;
                        accountResult.Data.PacketNumber = accountResult.Data.PacketNumber.HasValue ? accountResult.Data.PacketNumber + 1 : 1;
                    }
                }

                var f010Cache = _cache.Get(CacheRepository.F010Cache);
                var registerInfo = new RegisterEInfo
                {
                    Type = "D",
                    SourceTerritory = f010Cache.GetBackInt(accountResult.Data.Source),
                    DestinationTerritory = f010Cache.GetBackInt(accountResult.Data.Destination),
                    NumberXXXX = accountResult.Data.PacketNumber.Value,
                    YearXX = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0
                };

                result.Data = Tuple.Create(register, registerInfo, accountResult.Data.Direction ?? 0);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<Tuple<T1, RegisterEInfo, int>> WriteBackZ<T1, T2, T3, T4, T5, T6>(int accountId, int version, int type, ExportOptions options)
           where T1 : IZRegister, new()
           where T2 : IAccount, new()
           where T3 : IHeader, new()
           where T4 : IZRecord, new()
           where T5 : IPatient, new()
           where T6 : IZslEvent, new()
        {
            var result = new OperationResult<Tuple<T1, RegisterEInfo, int>>();
            try
            {

                //   result.AddError("Выбрання версия выгрузки не поддерживается ");
                //     return result;
                var accountResult = _repository.GetTerritoryAccountById(accountId);
                if (accountResult.HasError)
                {
                    result.AddError(accountResult.LastError);
                    return result;
                }

                var externalRefuseResult = _repository.GetZExternalRefusesByAccountId(accountId);
                if (externalRefuseResult.Success && !externalRefuseResult.Data.Any())
                {
                    result.AddError("Для реестра счетов ID {0} (исправленная часть) нечего выгружать, нет отказов с территории.".F(accountId));
                    return result;
                }

                _repository.UpdateGuid(accountId);

                var register = new T1
                {
                    InnerAccount = Map.ObjectToObject<T2>(accountResult.Data, typeof(T2))
                };

                register.InnerAccount.Year = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0;
                register.InnerAccount.Month = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Month : 0;
                register.InnerHeader = new T3
                {
                    TargetOkato = accountResult.Data.Destination,
                    SourceOkato = accountResult.Data.Source,
                    Version = version,
                    Date = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value : default(DateTime?)
                };

                var parentAccountResult = _repository.GetParentTerritoryAccountByParentId(accountResult.Data.Parent);
                if (parentAccountResult.HasError)
                {
                    result.AddError(parentAccountResult.LastError);
                    return result;
                }

                if (parentAccountResult.Data == null)
                {
                    register.InnerHeader.Date = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value : default(DateTime?);
                }
                else
                {
                    register.InnerHeader.Date = parentAccountResult.Data.Date.HasValue ? parentAccountResult.Data.Date.Value : default(DateTime?);

                }
                
                var patientsResult = _repository.GetPatientsByAccountId(accountId);
                if (patientsResult.Success)
                {
                    var records = new List<IZRecord>();
                    foreach (FactPatient factPatient in patientsResult.Data)
                    {
                        FactPatient patient = factPatient;
                        FactPerson person = null;
                        FactDocument document = null;

                        if (patient.PersonalId.HasValue)
                        {
                            var personResult = _repository.GetPersonById(patient.PersonalId.Value);
                            if (personResult.Success)
                            {
                                person = personResult.Data;
                            }
                        }

                        if (patient.PersonalId.HasValue)
                        {
                            var documentResult = _repository.GetDocumentByPersonId(patient.PersonalId.Value);
                            if (documentResult.Success)
                            {
                                document = documentResult.Data;
                            }
                        }

                        var checkRefusalsExternalExistsResult = _repository.GetZExternalRefuseByPatientId(factPatient.PatientId);
                        if (checkRefusalsExternalExistsResult.Success && checkRefusalsExternalExistsResult.Data.Any())
                        {
                            var record = new T4();
                            record.InnerPatient = Map.ObjectToObject<T5>(person, typeof(T5));

                            record.ExternalId = factPatient.ExternalId ?? 0;
                            if (document != null)
                            {
                                record.InnerPatient.DocNumber = document.DocNum;
                                record.InnerPatient.DocSeries = document.DocSeries;
                                record.InnerPatient.DocType = document.DocType;
                            }

                            record.InnerPatient.Newborn = patient.Newborn;
                            record.InnerPatient.InsuranceDocType = patient.InsuranceDocType;
                            record.InnerPatient.InsuranceDocSeries = patient.InsuranceDocSeries;
                            record.InnerPatient.INP = patient.INP;

                            switch (version)
                            {
                                case Constants.Version30:
                                    record.InnerPatient.InsuranceDocNumber = patient.InsuranceDocNumber;

                                    if (record.InnerPatient.InsuranceDocNumber.IsNullOrWhiteSpace() && patient.INP.IsNotNullOrWhiteSpace())
                                    {
                                        record.InnerPatient.InsuranceDocNumber = patient.INP;
                                    }
                                    break;
                            }

                            var zslMeventResult = _repository.GetZslMeventsByPatientId(patient.PatientId);
                            if (zslMeventResult.Success)
                            {
                                var events = new List<IZslEvent>();
                                foreach (ZslFactMedicalEvent zslfactMedicalEvent in zslMeventResult.Data)
                                {
                                    var zslMeventCopy = Map.ObjectToObject<T6>(zslfactMedicalEvent, typeof(T6));
                                    zslMeventCopy.RefusalPrice = 0.0m;

                                    var meventResult = _repository.GetZMeventsByZslMeventId(zslfactMedicalEvent.ZslMedicalEventId);
                                    decimal? totalMec = 0.0m;
                                    decimal? totalMee = 0.0m;
                                    decimal? totalEqma = 0.0m;
                                    int countMec = 0;

                                    if (meventResult.Success)
                                    {
                                        var mevents = new List<IZEvent>();
                                        foreach (ZFactMedicalEvent zFactMedicalEvent in meventResult.Data)
                                        {
                                            var meventCopy = Map.ObjectToObject<v30.E.EventE>(zFactMedicalEvent,
                                                typeof(v30.E.EventE));
                                            meventCopy.SpecialityClassifierVersion = "V021";
                                            meventCopy.SpecialityCodeV021 = zFactMedicalEvent.SpecialityCodeV021;
                                            
                                            var refusals = new List<object>();

                                            var refusalsResult = _repository.GetSankByZMedicalEventIdAndType(mec => mec.ZmedicalEventId == zFactMedicalEvent.ZmedicalEventId);
                                            if (refusalsResult.Success &&
                                                refusalsResult.Data.Any(p => (p.IsLock == null || p.IsLock == false)))
                                            {
                                                foreach (
                                                var factsank in
                                                    refusalsResult.Data.Where(
                                                        p => p.Source == RefusalSource.InterTerritorial.ToInt32() || p.Source == RefusalSource.LocalCorrected.ToInt32())
                                                        .Distinct(p => p.ZmedicalEventId))
                                                {
                                                    ZFactSank sank = factsank;

                                                    var refusal = new v30.E.RefusalE();
                                                    refusal.RefusalCode = sank.ReasonId;
                                                    refusal.RefusalRate = sank.Amount;
                                                    refusal.RefusalSource = sank.Source;
                                                    refusal.RefusalType = sank.Type;
                                                    refusal.ExternalGuid = sank.ExternalGuid;
                                                    refusal.Comments = sank.Comments;

                                                    refusals.Add(refusal);
                                                    if (factsank.Type == 1) countMec = countMec + 1;
                                                }
                                                totalMec += refusalsResult.Data.Where(p => p.Type == 1).Sum(p => p.Amount) ?? 0;
                                                totalMee += refusalsResult.Data.Where(p => Constants.Mee.Contains(p.Type)).Sum(p => p.Amount) ?? 0;
                                                totalEqma += refusalsResult.Data.Where(p => Constants.Eqma.Contains(p.Type)).Sum(p => p.Amount) ?? 0;

                                            }
                                            meventCopy.InnerRefusals = new List<object>(refusals);

                                            var refusalsExternalResult = _repository.GetZExternalRefuseByMedicalEventId(zFactMedicalEvent.ZmedicalEventId);
                                            if (refusalsExternalResult.Success)
                                            {

                                                foreach (var refuse in refusalsExternalResult.Data.Where(p => p.IsAgree == true))
                                                {
                                                    var refuseCopy = refuse;
                                                    var refusal = new v30.E.RefusalE
                                                    {
                                                        RefusalCode = refuseCopy.ReasonId,
                                                        RefusalRate = refuseCopy.Amount,
                                                        RefusalSource = (int)RefusalSource.LocalCorrected,//refuseCopy.Source,
                                                        RefusalType = refuseCopy.Type,
                                                        ExternalGuid = refuseCopy.ExternalGuid,
                                                        Comments = refuseCopy.Comment
                                                    };

                                                    refusals.Add(refusal);
                                                    if (refuse.Type == 1) countMec = countMec + 1;
                                                }
                                                totalMec += refusalsExternalResult.Data.Where(p => p.Type == (int)RefusalType.MEC && p.IsAgree == true).Sum(p => p.Amount) ?? 0;
                                                totalMee += refusalsExternalResult.Data.Where(p => Constants.Mee.Contains(p.Type) && p.IsAgree == true).Sum(p => p.Amount) ?? 0;
                                                totalEqma += refusalsExternalResult.Data.Where(p => Constants.Eqma.Contains(p.Type) && p.IsAgree == true).Sum(p => p.Amount) ?? 0;
                                            }

                                            meventCopy.InnerRefusals = new List<object>(refusals);


                                            //выгрузка услуг
                                            if (options.Has(Services.ExportOptions.ExportServices))
                                            {
                                                var services = new List<IService>();
                                                var serviceResult = _repository.GetZServicesByZMeventId(zFactMedicalEvent.ZmedicalEventId);
                                                if (serviceResult.Success)
                                                {
                                                    foreach (ZFactMedicalServices serviceE in serviceResult.Data)
                                                    {
                                                        var serviceCopy = serviceE;
                                                        var service = Map.ObjectToObject<v30.E.ServiceE>(serviceCopy);
                                                        service.SpecialityCode = serviceCopy.SpecialityCodeV021;
                                                        services.Add(service);
                                                    }
                                                }
                                                meventCopy.InnerServiceCollection = new List<IService>(services);
                                            }

                                            //выгрузка КСГ
                                            v30.E.KsgKpgE ksgkpg = null;
                                            var ksgKpgResult = _repository.GetZKsgKpgByZMeventId(zFactMedicalEvent.ZmedicalEventId);
                                            if (ksgKpgResult.Success)
                                            {
                                                var ksgKpgCopy = ksgKpgResult;
                                                ksgkpg = Map.ObjectToObject<v30.E.KsgKpgE>(ksgKpgCopy.Data);
                                                var slkoefResult =
                                                    _repository.GetSlKoefByKsgKpgId(ksgKpgResult.Data.ZksgKpgId);
                                                ksgkpg.Kkpg = null; //У нас на територии он не выгружается
                                                var slKoefs = new List<ISlKoef>();
                                                if (slkoefResult.Success)
                                                {
                                                    foreach (var zFactSlKoef in slkoefResult.Data)
                                                    {
                                                        var slKoefCopy = zFactSlKoef;
                                                        var slkoef = Map.ObjectToObject<v30.E.SlKoefE>(slKoefCopy);
                                                        slKoefs.Add(slkoef);
                                                    }
                                                }
                                                ksgkpg.InnerSlCoefCollection = new List<ISlKoef>(slKoefs);
                                            }
                                            if (ksgkpg != null)
                                            {
                                                meventCopy.InnerKsgKpg = ksgkpg as IKsgKpg;
                                            }

                                            mevents.Add(meventCopy);
                                        }
                                        zslMeventCopy.InnerMeventCollection = new List<IZEvent>(mevents);
                                        zslMeventCopy.RefusalPrice = totalMec.Value >= 0 && countMec > 0 ?
                                        zslMeventCopy.Price + totalMee + totalEqma : totalMee + totalEqma;
                                        //zslMeventCopy.RefusalPrice = totalMec.Value > 0 ? zslMeventCopy.Price + totalMee + totalEqma : totalMee + totalEqma;
                                    }

                                    events.Add(zslMeventCopy);
                                }
                                record.InnerZslEventCollection = new List<IZslEvent>(events);
                            }
                            records.Add(record);
                        }
                    }

                    register.InnerRecordCollection = new List<IZRecord>(records);
                    register.InnerRecordCollection = register.InnerRecordCollection.OrderBy(p => p.ExternalId).ToList();
                }


                if (!accountResult.Data.PacketNumber.HasValue)
                {
                    var packetNumberResult = _repository.GetFactExchangeLastPacketNumber(
                        exchange => exchange.Direction == accountResult.Data.Direction &&
                            exchange.Type == type &&
                            exchange.Date.Year == accountResult.Data.Date.Value.Year);
                    if (packetNumberResult.Success)
                    {
                        accountResult.Data.PacketNumber = packetNumberResult.Data;
                        accountResult.Data.PacketNumber = accountResult.Data.PacketNumber.HasValue ? accountResult.Data.PacketNumber + 1 : 1;
                    }
                }

                var f010Cache = _cache.Get(CacheRepository.F010Cache);
                var registerInfo = new RegisterEInfo
                {
                    Type = "D",
                    SourceTerritory = f010Cache.GetBackInt(accountResult.Data.Source),
                    DestinationTerritory = f010Cache.GetBackInt(accountResult.Data.Destination),
                    NumberXXXX = accountResult.Data.PacketNumber.Value,
                    YearXX = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0
                };

                result.Data = Tuple.Create(register, registerInfo, accountResult.Data.Direction ?? 0);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<Tuple<T1, RegisterEInfo, int>> WriteBackZOnk<T1, T2, T3, T4, T5, T6>(int accountId, int version, int type, ExportOptions options)
           where T1 : IZRegister, new()
           where T2 : IAccount, new()
           where T3 : IHeader, new()
           where T4 : IZRecord, new()
           where T5 : IPatient, new()
           where T6 : IZslEvent, new()
        {
            var result = new OperationResult<Tuple<T1, RegisterEInfo, int>>();
            try
            {

             //   result.AddError("Выбрання версия выгрузки не поддерживается ");
               //     return result;
                var accountResult = _repository.GetTerritoryAccountById(accountId);
                if (accountResult.HasError)
                {
                    result.AddError(accountResult.LastError);
                    return result;
                }

                var externalRefuseResult = _repository.GetZExternalRefusesByAccountId(accountId);
                if (externalRefuseResult.Success && !externalRefuseResult.Data.Any())
                {
                    result.AddError("Для реестра счетов ID {0} (исправленная часть) нечего выгружать, нет отказов с территории.".F(accountId));
                    return result;
                }

                _repository.UpdateGuid(accountId);

                var register = new T1
                {
                    InnerAccount = Map.ObjectToObject<T2>(accountResult.Data, typeof(T2))
                };

                register.InnerAccount.Year = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0;
                register.InnerAccount.Month = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Month : 0;
                register.InnerHeader = new T3
                {
                    TargetOkato = accountResult.Data.Destination,
                    SourceOkato = accountResult.Data.Source,
                    Version = version,
                    Date = DateTime.Now //accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value : default(DateTime?)
                };

                var parentAccountResult = _repository.GetParentTerritoryAccountByParentId(accountResult.Data.Parent);
                if (parentAccountResult.HasError)
                {
                    result.AddError(parentAccountResult.LastError);
                    return result;
                }

                if (parentAccountResult.Data == null)
                {
                    register.InnerHeader.Date = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value : default(DateTime?);
                }
                else
                {
                    register.InnerHeader.Date = parentAccountResult.Data.Date.HasValue ? parentAccountResult.Data.Date.Value : default(DateTime?);

                }

                var patientsResult = _repository.GetPatientsByAccountId(accountId);
                if (patientsResult.Success)
                {
                    var records = new List<IZRecord>();
                    foreach (FactPatient factPatient in patientsResult.Data)
                    {
                        FactPatient patient = factPatient;
                        FactPerson person = null;
                        FactDocument document = null;

                        if (patient.PersonalId.HasValue)
                        {
                            var personResult = _repository.GetPersonById(patient.PersonalId.Value);
                            if (personResult.Success)
                            {
                                person = personResult.Data;
                            }
                        }

                        if (patient.PersonalId.HasValue)
                        {
                            var documentResult = _repository.GetDocumentByPersonId(patient.PersonalId.Value);
                            if (documentResult.Success)
                            {
                                document = documentResult.Data;
                            }
                        }

                        var checkRefusalsExternalExistsResult = _repository.GetZExternalRefuseByPatientId(factPatient.PatientId);
                        if (checkRefusalsExternalExistsResult.Success && checkRefusalsExternalExistsResult.Data.Any())
                        {
                            var record = new T4();
                            record.InnerPatient = Map.ObjectToObject<T5>(person, typeof(T5));

                            record.ExternalId = factPatient.ExternalId ?? 0;
                            if (document != null)
                            {
                                record.InnerPatient.DocNumber = document.DocNum;
                                record.InnerPatient.DocSeries = document.DocSeries;
                                record.InnerPatient.DocType = document.DocType;
                            }

                            record.InnerPatient.Newborn = patient.Newborn;
                            record.InnerPatient.InsuranceDocType = patient.InsuranceDocType;
                            record.InnerPatient.InsuranceDocSeries = patient.InsuranceDocSeries;
                            record.InnerPatient.INP = patient.INP;

                            switch (version)
                            {
                                case Constants.Version31:
                                case Constants.Version32:
                                    record.InnerPatient.InsuranceDocNumber = patient.InsuranceDocNumber;

                                    if (record.InnerPatient.InsuranceDocNumber.IsNullOrWhiteSpace() && patient.INP.IsNotNullOrWhiteSpace())
                                    {
                                        record.InnerPatient.InsuranceDocNumber = patient.INP;
                                    }
                                    break;
                            }

                            var zslMeventResult = _repository.GetZslMeventsByPatientId(patient.PatientId);
                            if (zslMeventResult.Success)
                            {
                                var events = new List<IZslEvent>();
                                foreach (ZslFactMedicalEvent zslfactMedicalEvent in zslMeventResult.Data)
                                {
                                    var zslMeventCopy = Map.ObjectToObject<v31.E.ZslEventE>(zslfactMedicalEvent, typeof(T6));
                                    zslMeventCopy.RefusalPrice = 0.0m;

                                    var meventResult = _repository.GetZMeventsByZslMeventId(zslfactMedicalEvent.ZslMedicalEventId);
                                    decimal? totalMec = 0.0m;
                                    decimal? totalMee = 0.0m;
                                    decimal? totalEqma = 0.0m;
                                    int countMec = 0;

                                    if (meventResult.Success)
                                    {
                                        var mevents = new List<IZEvent>();
                                        foreach (ZFactMedicalEvent zFactMedicalEvent in meventResult.Data)
                                        {
                                            var meventCopy = Map.ObjectToObject<v31.E.EventE>(zFactMedicalEvent, typeof(v31.E.EventE));
                                            meventCopy.SpecialityClassifierVersion = "V021";
                                            meventCopy.SpecialityCodeV021 = zFactMedicalEvent.SpecialityCodeV021;
                                            meventCopy.ExternalId = zFactMedicalEvent.SlIdGuid;

                                            var directionResult = _repository.GetZDirectionBySlMeventId(zFactMedicalEvent.ZmedicalEventId);
                                            if (directionResult.Success)
                                            {
                                                meventCopy.DirectionOnkXml = new List<DirectionOnkE>();
                                                foreach (var zFactDirection in directionResult.Data)
                                                {
                                                    var directionCopy = Map.ObjectToObject<v31.E.DirectionOnkE>(zFactDirection, typeof(v31.E.DirectionOnkE));
                                                    meventCopy.DirectionOnkXml.Add(directionCopy);
                                                }
                                            }

                                            var сonsultationsResult = _repository.GetZConsultationsBySlMeventId(zFactMedicalEvent.ZmedicalEventId);
                                            if (сonsultationsResult.Success)
                                            {
                                                meventCopy.ConsultationsOnkXml = new List<ConsultationsOnkE>();
                                                foreach (var zFactConsultationse in сonsultationsResult.Data)
                                                {
                                                    var consultationsCopy = Map.ObjectToObject<v31.E.ConsultationsOnkE>(zFactConsultationse, typeof(v31.E.ConsultationsOnkE));
                                                    meventCopy.ConsultationsOnkXml.Add(consultationsCopy);
                                                }
                                            }

                                            //выгрузка услуг
                                            if (options.Has(Services.ExportOptions.ExportServices))
                                            {
                                                var services = new List<IService>();
                                                var serviceResult = _repository.GetZServicesByZMeventId(zFactMedicalEvent.ZmedicalEventId);
                                                if (serviceResult.Success)
                                                {
                                                    foreach (ZFactMedicalServices serviceE in serviceResult.Data)
                                                    {
                                                        var serviceCopy = serviceE;
                                                        var service = Map.ObjectToObject<v31.E.ServiceE>(serviceCopy);
                                                        service.SpecialityCode = serviceCopy.SpecialityCodeV021;
                                                        services.Add(service);
                                                    }
                                                }
                                                meventCopy.InnerServiceCollection = new List<IService>(services);
                                                //meventCopy.InnerServiceCollection = services;
                                            }

                                            //выгрузка онко случай
                                            v31.E.EventOnkE eventOnk = null;
                                            var eventOnkResult = _repository.GetZMedicalEventOnkByZMeventId(zFactMedicalEvent.ZmedicalEventId);
                                            if (eventOnkResult.Success)
                                            {
                                                eventOnk = Map.ObjectToObject<v31.E.EventOnkE>(eventOnkResult.Data);
                                                var diagBlokResult = _repository.GetDiafBlokByMedicalEventOnkId(eventOnkResult.Data.ZMedicalEventOnkId);
                                                var contraindicationsResult = _repository.GetContraindicationsByMedicalEventOnkId(eventOnkResult.Data.ZMedicalEventOnkId);

                                                var diagBloks = new List<v31.E.DiagBlokOnkE>();
                                                if (diagBlokResult.Success)
                                                {
                                                    foreach (var zFactDiagBlok in diagBlokResult.Data)
                                                    {
                                                        var dBlok = Map.ObjectToObject<v31.E.DiagBlokOnkE>(zFactDiagBlok);
                                                        diagBloks.Add(dBlok);
                                                    }
                                                }
                                                eventOnk.DiagBlokOnk = new List<v31.E.DiagBlokOnkE>(diagBloks);
                                                var contraindications = new List<v31.E.ContraindicationsOnkE>();
                                                if (contraindicationsResult.Success)
                                                {
                                                    foreach (var zContraindications in contraindicationsResult.Data)
                                                    {
                                                        var contraindication = Map.ObjectToObject<v31.E.ContraindicationsOnkE>(zContraindications);
                                                        contraindications.Add(contraindication);
                                                    }
                                                }
                                                eventOnk.СontraindicationsOnk = new List<v31.E.ContraindicationsOnkE>(contraindications);

                                                var serviceOnks = new List<v31.E.ServiceOnkE>();
                                                var serviceOnkResult = _repository.GetZMedicalServiceOnkByMedicalEventId(eventOnkResult.Data.ZMedicalEventOnkId);
                                                if (serviceOnkResult.Success)
                                                {
                                                    foreach (var zFactMedicalServicesOnk in serviceOnkResult.Data)
                                                    {
                                                        var serviceOnkMap = Map.ObjectToObject<v31.E.ServiceOnkE>(zFactMedicalServicesOnk);

                                                        var lekPrResult = _repository.GetAnticancerDrugByMedicalServiceOnkId(zFactMedicalServicesOnk.ZmedicalServicesOnkId);

                                                        var lekPrs = new List<v31.E.AnticancerDrugOnkE>();
                                                        if (lekPrResult.Success)
                                                        {
                                                            foreach (var zFactAnticancerDrug in lekPrResult.Data)
                                                            {
                                                                var lekpr = Map.ObjectToObject<v31.E.AnticancerDrugOnkE>(zFactAnticancerDrug);
                                                                lekPrs.Add(lekpr);
                                                            }
                                                            serviceOnkMap.AnticancerDrugOnkXml = lekPrs;
                                                        }
                                                        serviceOnks.Add(serviceOnkMap);
                                                    }
                                                    eventOnk.ServiceOnk = serviceOnks;
                                                }
                                                meventCopy.EventOnk = eventOnk;
                                            }

                                            //выгрузка КСГ
                                            v31.E.KsgKpgE ksgkpg = null;
                                            var ksgKpgResult = _repository.GetZKsgKpgByZMeventId(zFactMedicalEvent.ZmedicalEventId);
                                            if (ksgKpgResult.Success)
                                            {
                                                var ksgKpgCopy = ksgKpgResult;
                                                ksgkpg = Map.ObjectToObject<v31.E.KsgKpgE>(ksgKpgCopy.Data);

                                                var cirtResult = _repository.GetCritByksgKpgId(ksgKpgResult.Data.ZksgKpgId);
                                                if (cirtResult.Success)
                                                {
                                                    ksgkpg.CritXml = new List<string>();
                                                    foreach (var zFactCrit in cirtResult.Data)
                                                    {
                                                        ksgkpg.CritXml.Add(zFactCrit.IdDkk);
                                                    }
                                                }

                                                var slkoefResult = _repository.GetSlKoefByKsgKpgId(ksgKpgResult.Data.ZksgKpgId);
                                                ksgkpg.Kkpg = null; //У нас на територии он не выгружается
                                                var slKoefs = new List<ISlKoef>();
                                                if (slkoefResult.Success)
                                                {
                                                    foreach (var zFactSlKoef in slkoefResult.Data)
                                                    {
                                                        var slKoefCopy = zFactSlKoef;
                                                        var slkoef = Map.ObjectToObject<v31.E.SlKoefE>(slKoefCopy);
                                                        slKoefs.Add(slkoef);
                                                    }
                                                }
                                                ksgkpg.InnerSlCoefCollection = new List<ISlKoef>(slKoefs);
                                            }
                                            if (ksgkpg != null)
                                            {
                                                meventCopy.InnerKsgKpg = ksgkpg;
                                            }
                                            mevents.Add(meventCopy);
                                        }
                                        zslMeventCopy.InnerMeventCollection = new List<IZEvent>(mevents);
                                    }

                                    var refusals = new List<v31.E.RefusalE>();

                                    var refusalsMecResult = _repository.GetSankByZMedicalEventIdAndType(mec => mec.ZslMedicalEventId == zslfactMedicalEvent.ZslMedicalEventId);
                                    if (refusalsMecResult.Success &&
                                        refusalsMecResult.Data.Any(p => (p.IsLock == null || p.IsLock == false)))
                                    {
                                        foreach (var factMec in refusalsMecResult.Data.Where(p =>
                                                        ((p.Source == RefusalSource.Local.ToInt32() || p.Source == RefusalSource.LocalCorrected.ToInt32()) &&
                                                         p.Type == 1)).Distinct(p => p.ZmedicalEventId))
                                        {
                                            ZFactSank mec = factMec;
                                            var guidmeventId = _repository.GetZMedicalEventBySlidGuid(factMec.ZmedicalEventId);

                                            var refusal = new v31.E.RefusalE();
                                            refusal.RefusalCode = mec.ReasonId;
                                            refusal.RefusalRate = mec.Amount;
                                            refusal.Date = mec.Date;
                                            refusal.NumAct = mec.NumAct;
                                            refusal.RefusalSource = mec.Source;
                                            refusal.RefusalType = factMec.Type;
                                            refusal.ExternalGuid = mec.ExternalGuid;
                                            refusal.SlidGuid = guidmeventId.Data;
                                            refusal.Comments = mec.Comments;

                                            refusals.Add(refusal);
                                            countMec = countMec + 1;
                                        }
                                        foreach (
                                        var factsank in
                                            refusalsMecResult.Data.Where(p => Constants.Mee.Contains(p.Type) || Constants.Eqma.Contains(p.Type)))
                                        {
                                            ZFactSank sank = factsank;
                                            var guidmeventId = _repository.GetZMedicalEventBySlidGuid(factsank.ZmedicalEventId);

                                            var refusal = new v31.E.RefusalE();
                                            refusal.RefusalCode = sank.ReasonId;
                                            refusal.RefusalRate = sank.Amount;
                                            refusal.Date = sank.Date;
                                            refusal.NumAct = sank.NumAct;
                                            refusal.CodeExp = sank.CodeExp;
                                            refusal.RefusalSource = sank.Source;
                                            refusal.RefusalType = factsank.Type;
                                            refusal.ExternalGuid = sank.ExternalGuid;
                                            refusal.SlidGuid = guidmeventId.Data;
                                            refusal.Comments = sank.Comments;
                                            refusals.Add(refusal);
                                        }

                                        totalMec += refusalsMecResult.Data.Where(p => p.Type == 1).Sum(p => p.Amount) ?? 0;
                                        totalMee += refusalsMecResult.Data.Where(p => Constants.Mee.Contains(p.Type)).Sum(p => p.Amount) ?? 0;
                                        totalEqma += refusalsMecResult.Data.Where(p => Constants.Eqma.Contains(p.Type)).Sum(p => p.Amount) ?? 0;

                                    }

                                    var refusalsExternalResult = _repository.GetZExternalRefuseByZslMedicalEventId(zslfactMedicalEvent.ZslMedicalEventId);
                                    if (refusalsExternalResult.Success)
                                    {

                                        foreach (var refuse in refusalsExternalResult.Data.Where(p => p.IsAgree == true))
                                        {
                                            var guidmeventId = _repository.GetZMedicalEventBySlidGuid(refuse.ZmedicalEventId);
                                            var refuseCopy = refuse;
                                            var refusal = new v31.E.RefusalE
                                            {
                                                RefusalCode = refuseCopy.ReasonId,
                                                RefusalRate = refuseCopy.Amount,
                                                Date = DateTime.Now, //refuseCopy.Date
                                                NumAct = refuse.ZmedicalEventId.ToString(),//refuseCopy.NumAct
                                                CodeExp = refuseCopy.CodeExp,
                                                RefusalSource = (int)RefusalSource.LocalCorrected, //refuseCopy.Source,
                                                RefusalType = refuseCopy.Type,
                                                ExternalGuid = refuseCopy.ExternalGuid,
                                                SlidGuid = guidmeventId.Data,
                                                Comments = refuseCopy.Comment

                                            };
                                            if (refuse.Type == (int)RefusalType.MEC && refuse.IsAgree == true) countMec = countMec + 1;
                                            refusals.Add(refusal);
                                        }
                                        totalMec += refusalsExternalResult.Data.Where(p => p.Type == (int)RefusalType.MEC && p.IsAgree == true).Sum(p => p.Amount) ?? 0;
                                        totalMee += refusalsExternalResult.Data.Where(p => Constants.Mee.Contains(p.Type) && p.IsAgree == true).Sum(p => p.Amount) ?? 0;
                                        totalEqma += refusalsExternalResult.Data.Where(p => Constants.Eqma.Contains(p.Type) && p.IsAgree == true).Sum(p => p.Amount) ?? 0;
                                    }
                                    zslMeventCopy.RefusalPrice = totalMec.Value >= 0 && countMec > 0 ? zslMeventCopy.Price + totalMee + totalEqma : totalMee + totalEqma;

                                    zslMeventCopy.RefusalsXml = new List<v31.E.RefusalE>(refusals);

                                    events.Add(zslMeventCopy);
                                }
                                record.InnerZslEventCollection = new List<IZslEvent>(events);
                            }
                            records.Add(record);
                        }
                    }

                    register.InnerRecordCollection = new List<IZRecord>(records);
                    register.InnerRecordCollection = register.InnerRecordCollection.OrderBy(p => p.ExternalId).ToList();
                }


                if (!accountResult.Data.PacketNumber.HasValue)
                {
                    var packetNumberResult = _repository.GetFactExchangeLastPacketNumber(
                        exchange => exchange.Direction == accountResult.Data.Direction &&
                            exchange.Type == type &&
                            exchange.Date.Year == accountResult.Data.Date.Value.Year);
                    if (packetNumberResult.Success)
                    {
                        accountResult.Data.PacketNumber = packetNumberResult.Data;
                        accountResult.Data.PacketNumber = accountResult.Data.PacketNumber.HasValue ? accountResult.Data.PacketNumber + 1 : 1;
                    }
                }

                var f010Cache = _cache.Get(CacheRepository.F010Cache);
                var registerInfo = new RegisterEInfo
                {
                    Type = "D",
                    SourceTerritory = f010Cache.GetBackInt(accountResult.Data.Source),
                    DestinationTerritory = f010Cache.GetBackInt(accountResult.Data.Destination),
                    NumberXXXX = accountResult.Data.PacketNumber.Value,
                    YearXX = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0
                };

                result.Data = Tuple.Create(register, registerInfo, accountResult.Data.Direction ?? 0);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<Tuple<T1, RegisterEInfo, int>> WriteBackZOnk<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(int accountId, int version, int type, ExportOptions options)
            where T1 : IZRegister, new()
            where T2 : IAccount, new()
            where T3 : IHeader, new()
            where T4 : IZRecord, new()
            where T5 : IPatient, new()
            where T6 : IZslEvent, new()
            where T7 : IZEvent, new()
            where T8 : IDirectionOnkE, new()
            where T9 : IConsultationsOnk, new()
            where T10 : IEventOnk, new()
            where T11 : IDiagBlokOnk, new()
            where T12 : IContraindicationsOnk, new()
            where T13 : IServiceOnk, new()
            where T14 : IAnticancerDrugOnk, new()
            where T15 : IKsgKpg, new()
            where T16 : ISlKoef, new()
            where T17 : IRefusal, new()
            where T18 : IService, new()
        {
            var result = new OperationResult<Tuple<T1, RegisterEInfo, int>>();
            try
            {

                //   result.AddError("Выбрання версия выгрузки не поддерживается ");
                //     return result;
                var accountResult = _repository.GetTerritoryAccountById(accountId);
                if (accountResult.HasError)
                {
                    result.AddError(accountResult.LastError);
                    return result;
                }

                var externalRefuseResult = _repository.GetZExternalRefusesByAccountId(accountId);
                if (externalRefuseResult.Success && !externalRefuseResult.Data.Any())
                {
                    result.AddError("Для реестра счетов ID {0} (исправленная часть) нечего выгружать, нет отказов с территории.".F(accountId));
                    return result;
                }

                _repository.UpdateGuid(accountId);

                var register = new T1
                {
                    InnerAccount = Map.ObjectToObject<T2>(accountResult.Data, typeof(T2))
                };

                register.InnerAccount.Year = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0;
                register.InnerAccount.Month = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Month : 0;
                register.InnerHeader = new T3
                {
                    TargetOkato = accountResult.Data.Destination,
                    SourceOkato = accountResult.Data.Source,
                    Version = version,
                    Date = DateTime.Now //accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value : default(DateTime?)
                };

                var parentAccountResult = _repository.GetParentTerritoryAccountByParentId(accountResult.Data.Parent);
                if (parentAccountResult.HasError)
                {
                    result.AddError(parentAccountResult.LastError);
                    return result;
                }

                //Что за хня честно говоря не понимаю пока отключил 12,03,2020
                //if (parentAccountResult.Data == null)
                //{
                //    register.InnerHeader.Date = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value : default(DateTime?);
                //}
                //else
                //{
                //    register.InnerHeader.Date = parentAccountResult.Data.Date.HasValue ? parentAccountResult.Data.Date.Value : default(DateTime?);

                //}

                var patientsResult = _repository.GetPatientsByAccountId(accountId);
                if (patientsResult.Success)
                {
                    var records = new List<IZRecord>();
                    foreach (FactPatient factPatient in patientsResult.Data)
                    {
                        FactPatient patient = factPatient;
                        FactPerson person = null;
                        FactDocument document = null;

                        if (patient.PersonalId.HasValue)
                        {
                            var personResult = _repository.GetPersonById(patient.PersonalId.Value);
                            if (personResult.Success)
                            {
                                person = personResult.Data;
                            }
                        }

                        if (patient.PersonalId.HasValue)
                        {
                            var documentResult = _repository.GetDocumentByPersonId(patient.PersonalId.Value);
                            if (documentResult.Success)
                            {
                                document = documentResult.Data;
                            }
                        }

                        var checkRefusalsExternalExistsResult = _repository.GetZExternalRefuseByPatientId(factPatient.PatientId);
                        if (checkRefusalsExternalExistsResult.Success && checkRefusalsExternalExistsResult.Data.Any())
                        {
                            var record = new T4();
                            record.InnerPatient = Map.ObjectToObject<T5>(person, typeof(T5));

                            record.ExternalId = factPatient.ExternalId ?? 0;
                            if (document != null)
                            {
                                record.InnerPatient.DocNumber = document.DocNum;
                                record.InnerPatient.DocSeries = document.DocSeries;
                                record.InnerPatient.DocType = document.DocType;
                            }

                            record.InnerPatient.Newborn = patient.Newborn;
                            record.InnerPatient.InsuranceDocType = patient.InsuranceDocType;
                            record.InnerPatient.InsuranceDocSeries = patient.InsuranceDocSeries;
                            record.InnerPatient.INP = patient.INP;

                            switch (version)
                            {
                                case Constants.Version31:
                                case Constants.Version32:
                                    record.InnerPatient.InsuranceDocNumber = patient.InsuranceDocNumber;

                                    if (record.InnerPatient.InsuranceDocNumber.IsNullOrWhiteSpace() && patient.INP.IsNotNullOrWhiteSpace())
                                    {
                                        record.InnerPatient.InsuranceDocNumber = patient.INP;
                                    }
                                    break;
                            }

                            var zslMeventResult = _repository.GetZslMeventsByPatientId(patient.PatientId);
                            if (zslMeventResult.Success)
                            {
                                var events = new List<IZslEvent>();
                                foreach (ZslFactMedicalEvent zslfactMedicalEvent in zslMeventResult.Data)
                                {
                                    var zslMeventCopy = Map.ObjectToObject<T6>(zslfactMedicalEvent, typeof(T6));
                                    zslMeventCopy.RefusalPrice = 0.0m;

                                    var meventResult = _repository.GetZMeventsByZslMeventId(zslfactMedicalEvent.ZslMedicalEventId);
                                    decimal? totalMec = 0.0m;
                                    decimal? totalMee = 0.0m;
                                    decimal? totalEqma = 0.0m;
                                    int countMec = 0;

                                    if (meventResult.Success)
                                    {
                                        var mevents = new List<IZEvent>();
                                        foreach (ZFactMedicalEvent zFactMedicalEvent in meventResult.Data)
                                        {
                                            var meventCopy = Map.ObjectToObject<T7>(zFactMedicalEvent, typeof(T7));
                                            meventCopy.SpecialityClassifierVersion = "V021";
                                            meventCopy.SpecialityCodeV021 = zFactMedicalEvent.SpecialityCodeV021;
                                            meventCopy.ExternalId = zFactMedicalEvent.SlIdGuid;

                                            var directionResult = _repository.GetZDirectionBySlMeventId(zFactMedicalEvent.ZmedicalEventId);
                                            if (directionResult.Success)
                                            {
                                                var directions = new List<IDirectionOnkE>();
                                                foreach (var zFactDirection in directionResult.Data)
                                                {
                                                    var directionCopy = Map.ObjectToObject<T8>(zFactDirection, typeof(T8));
                                                    directions.Add(directionCopy);
                                                }
                                                meventCopy.InnerDirectionOnkCollection = new List<IDirectionOnkE>(directions);
                                            }

                                            var сonsultationsResult = _repository.GetZConsultationsBySlMeventId(zFactMedicalEvent.ZmedicalEventId);
                                            if (сonsultationsResult.Success)
                                            {
                                                var consultations = new List<IConsultationsOnk>();
                                                foreach (var zFactConsultationse in сonsultationsResult.Data)
                                                {
                                                    var consultationsCopy = Map.ObjectToObject<T9>(zFactConsultationse, typeof(T9));
                                                    consultations.Add(consultationsCopy);
                                                }
                                                meventCopy.InnerConsultationsOnkCollection = new List<IConsultationsOnk>(consultations);
                                            }

                                            //выгрузка услуг
                                            if (options.Has(Services.ExportOptions.ExportServices))
                                            {
                                                var services = new List<IService>();
                                                var serviceResult = _repository.GetZServicesByZMeventId(zFactMedicalEvent.ZmedicalEventId);
                                                if (serviceResult.Success)
                                                {
                                                    foreach (ZFactMedicalServices serviceE in serviceResult.Data)
                                                    {
                                                        var serviceCopy = serviceE;
                                                        var service = Map.ObjectToObject<T18>(serviceCopy);
                                                        service.SpecialityCode = serviceCopy.SpecialityCodeV021;
                                                        services.Add(service);
                                                    }
                                                }
                                                meventCopy.InnerServiceCollection = new List<IService>(services);
                                                //meventCopy.InnerServiceCollection = services;
                                            }

                                            //выгрузка онко случай
                                            T10 eventOnk = default(T10);
                                            var eventOnkResult = _repository.GetZMedicalEventOnkByZMeventId(zFactMedicalEvent.ZmedicalEventId);
                                            if (eventOnkResult.Success)
                                            {
                                                eventOnk = Map.ObjectToObject<T10>(eventOnkResult.Data);
                                                var diagBlokResult = _repository.GetDiafBlokByMedicalEventOnkId(eventOnkResult.Data.ZMedicalEventOnkId);
                                                var contraindicationsResult = _repository.GetContraindicationsByMedicalEventOnkId(eventOnkResult.Data.ZMedicalEventOnkId);

                                                var diagBloks = new List<IDiagBlokOnk>();
                                                if (diagBlokResult.Success)
                                                {
                                                    foreach (var zFactDiagBlok in diagBlokResult.Data)
                                                    {
                                                        var dBlok = Map.ObjectToObject<T11>(zFactDiagBlok);
                                                        diagBloks.Add(dBlok);
                                                    }
                                                }
                                                eventOnk.InnerDiagBlokOnkCollection = new List<IDiagBlokOnk>(diagBloks);

                                                var contraindications = new List<IContraindicationsOnk>();
                                                if (contraindicationsResult.Success)
                                                {
                                                    foreach (var zContraindications in contraindicationsResult.Data)
                                                    {
                                                        var contraindication = Map.ObjectToObject<T12>(zContraindications);
                                                        contraindications.Add(contraindication);
                                                    }
                                                }
                                                eventOnk.InnerСontraindicationsOnkCollection = new List<IContraindicationsOnk>(contraindications);

                                                var serviceOnks = new List<IServiceOnk>();
                                                var serviceOnkResult = _repository.GetZMedicalServiceOnkByMedicalEventId(eventOnkResult.Data.ZMedicalEventOnkId);
                                                if (serviceOnkResult.Success)
                                                {
                                                    foreach (var zFactMedicalServicesOnk in serviceOnkResult.Data)
                                                    {
                                                        var serviceOnkMap = Map.ObjectToObject<T13>(zFactMedicalServicesOnk);

                                                        var lekPrResult = _repository.GetAnticancerDrugByMedicalServiceOnkId(zFactMedicalServicesOnk.ZmedicalServicesOnkId);

                                                        var lekPrs = new List<IAnticancerDrugOnk>();
                                                        if (lekPrResult.Success)
                                                        {
                                                            foreach (var zFactAnticancerDrug in lekPrResult.Data)
                                                            {
                                                                var lekpr = Map.ObjectToObject<T14>(zFactAnticancerDrug);
                                                                lekPrs.Add(lekpr);
                                                            }
                                                            serviceOnkMap.InnerAnticancerDrugOnkCollection = new List<IAnticancerDrugOnk>(lekPrs);
                                                        }
                                                        serviceOnks.Add(serviceOnkMap);
                                                    }
                                                    eventOnk.InnerServiceOnkCollection = serviceOnks;
                                                }
                                                meventCopy.InnerEventOnk = eventOnk;
                                            }

                                            //выгрузка КСГ
                                            T15 ksgkpg = default(T15);
                                            var ksgKpgResult = _repository.GetZKsgKpgByZMeventId(zFactMedicalEvent.ZmedicalEventId);
                                            if (ksgKpgResult.Success)
                                            {
                                                var ksgKpgCopy = ksgKpgResult;
                                                ksgkpg = Map.ObjectToObject<T15>(ksgKpgCopy.Data);

                                                var cirtResult = _repository.GetCritByksgKpgId(ksgKpgResult.Data.ZksgKpgId);
                                                if (cirtResult.Success)
                                                {
                                                    ksgkpg.CritXml = new List<string>();
                                                    foreach (var zFactCrit in cirtResult.Data)
                                                    {
                                                        ksgkpg.CritXml.Add(zFactCrit.IdDkk);
                                                    }
                                                }

                                                var slkoefResult = _repository.GetSlKoefByKsgKpgId(ksgKpgResult.Data.ZksgKpgId);
                                                ksgkpg.Kkpg = null; //У нас на територии он не выгружается
                                                var slKoefs = new List<ISlKoef>();
                                                if (slkoefResult.Success)
                                                {
                                                    foreach (var zFactSlKoef in slkoefResult.Data)
                                                    {
                                                        var slKoefCopy = zFactSlKoef;
                                                        var slkoef = Map.ObjectToObject<T16>(slKoefCopy);
                                                        slKoefs.Add(slkoef);
                                                    }
                                                }
                                                ksgkpg.InnerSlCoefCollection = new List<ISlKoef>(slKoefs);
                                            }
                                            if (ksgkpg != null)
                                            {
                                                meventCopy.InnerKsgKpg = new T15();
                                                meventCopy.InnerKsgKpg = ksgkpg;
                                            }
                                            mevents.Add(meventCopy);
                                        }
                                        zslMeventCopy.InnerMeventCollection = new List<IZEvent>(mevents);
                                    }

                                    var refusals = new List<IRefusal>();

                                    var refusalsMecResult = _repository.GetSankByZMedicalEventIdAndType(mec => mec.ZslMedicalEventId == zslfactMedicalEvent.ZslMedicalEventId);
                                    if (refusalsMecResult.Success &&
                                        refusalsMecResult.Data.Any(p => (p.IsLock == null || p.IsLock == false)))
                                    {
                                        foreach (var factMec in refusalsMecResult.Data.Where(p =>
                                                        ((p.Source == RefusalSource.Local.ToInt32() || p.Source == RefusalSource.LocalCorrected.ToInt32()) &&
                                                         p.Type == 1)).Distinct(p => p.ZmedicalEventId))
                                        {
                                            ZFactSank mec = factMec;
                                            var guidmeventId = _repository.GetZMedicalEventBySlidGuid(factMec.ZmedicalEventId);

                                            var refusal = new T17();
                                            refusal.RefusalCode = mec.ReasonId;
                                            refusal.RefusalRate = mec.Amount;
                                            refusal.Date = mec.Date;
                                            refusal.NumAct = mec.NumAct;
                                            refusal.RefusalSource = mec.Source;
                                            refusal.RefusalType = factMec.Type;
                                            refusal.ExternalGuid = mec.ExternalGuid;
                                            refusal.SlidGuid = guidmeventId.Data;
                                            refusal.Comments = mec.Comments;

                                            refusals.Add(refusal);
                                            countMec = countMec + 1;
                                        }
                                        foreach (
                                        var factsank in
                                            refusalsMecResult.Data.Where(p => Constants.Mee.Contains(p.Type) || Constants.Eqma.Contains(p.Type)))
                                        {
                                            ZFactSank sank = factsank;
                                            var guidmeventId = _repository.GetZMedicalEventBySlidGuid(factsank.ZmedicalEventId);

                                            var refusal = new T17();
                                            refusal.RefusalCode = sank.ReasonId;
                                            refusal.RefusalRate = sank.Amount;
                                            refusal.Date = sank.Date;
                                            refusal.NumAct = sank.NumAct;
                                            refusal.CodeExp = sank.CodeExp;
                                            refusal.RefusalSource = sank.Source;
                                            refusal.RefusalType = factsank.Type;
                                            refusal.ExternalGuid = sank.ExternalGuid;
                                            refusal.SlidGuid = guidmeventId.Data;
                                            refusal.Comments = sank.Comments;
                                            refusals.Add(refusal);
                                        }

                                        totalMec += refusalsMecResult.Data.Where(p => p.Type == 1).Sum(p => p.Amount) ?? 0;
                                        totalMee += refusalsMecResult.Data.Where(p => Constants.Mee.Contains(p.Type)).Sum(p => p.Amount) ?? 0;
                                        totalEqma += refusalsMecResult.Data.Where(p => Constants.Eqma.Contains(p.Type)).Sum(p => p.Amount) ?? 0;

                                    }

                                    var refusalsExternalResult = _repository.GetZExternalRefuseByZslMedicalEventId(zslfactMedicalEvent.ZslMedicalEventId);
                                    if (refusalsExternalResult.Success)
                                    {

                                        foreach (var refuse in refusalsExternalResult.Data.Where(p => p.IsAgree == true))
                                        {
                                            var guidmeventId = _repository.GetZMedicalEventBySlidGuid(refuse.ZmedicalEventId);
                                            var refuseCopy = refuse;
                                            var refusal = new T17
                                            {
                                                RefusalCode = refuseCopy.ReasonId,
                                                RefusalRate = refuseCopy.Amount,
                                                Date = DateTime.Now, //refuseCopy.Date
                                                NumAct = refuse.ZmedicalEventId.ToString(),//refuseCopy.NumAct
                                                CodeExp = refuseCopy.CodeExp,
                                                RefusalSource = (int)RefusalSource.LocalCorrected, //refuseCopy.Source,
                                                RefusalType = refuseCopy.Type,
                                                ExternalGuid = refuseCopy.ExternalGuid,
                                                SlidGuid = guidmeventId.Data,
                                                Comments = refuseCopy.Comment

                                            };
                                            if (refuse.Type == (int)RefusalType.MEC && refuse.IsAgree == true) countMec = countMec + 1;
                                            refusals.Add(refusal);
                                        }
                                        totalMec += refusalsExternalResult.Data.Where(p => p.Type == (int)RefusalType.MEC && p.IsAgree == true).Sum(p => p.Amount) ?? 0;
                                        totalMee += refusalsExternalResult.Data.Where(p => Constants.Mee.Contains(p.Type) && p.IsAgree == true).Sum(p => p.Amount) ?? 0;
                                        totalEqma += refusalsExternalResult.Data.Where(p => Constants.Eqma.Contains(p.Type) && p.IsAgree == true).Sum(p => p.Amount) ?? 0;
                                    }
                                    zslMeventCopy.RefusalPrice = totalMec.Value >= 0 && countMec > 0 ? zslMeventCopy.Price + totalMee + totalEqma : totalMee + totalEqma;
                                    if (zslMeventCopy.RefusalPrice == 0) zslMeventCopy.RefusalPrice = null;

                                    zslMeventCopy.InnerRefusals = new List<IRefusal>(refusals);

                                    events.Add(zslMeventCopy);
                                }
                                record.InnerZslEventCollection = new List<IZslEvent>(events);
                            }
                            records.Add(record);
                        }
                    }

                    register.InnerRecordCollection = new List<IZRecord>(records);
                    register.InnerRecordCollection = register.InnerRecordCollection.OrderBy(p => p.ExternalId).ToList();
                }


                if (!accountResult.Data.PacketNumber.HasValue)
                {
                    var packetNumberResult = _repository.GetFactExchangeLastPacketNumber(
                        exchange => exchange.Direction == accountResult.Data.Direction &&
                            exchange.Type == type &&
                            exchange.Date.Year == accountResult.Data.Date.Value.Year);
                    if (packetNumberResult.Success)
                    {
                        accountResult.Data.PacketNumber = packetNumberResult.Data;
                        accountResult.Data.PacketNumber = accountResult.Data.PacketNumber.HasValue ? accountResult.Data.PacketNumber + 1 : 1;
                    }
                }

                var f010Cache = _cache.Get(CacheRepository.F010Cache);
                var registerInfo = new RegisterEInfo
                {
                    Type = "D",
                    SourceTerritory = f010Cache.GetBackInt(accountResult.Data.Source),
                    DestinationTerritory = f010Cache.GetBackInt(accountResult.Data.Destination),
                    NumberXXXX = accountResult.Data.PacketNumber.Value,
                    YearXX = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0
                };

                result.Data = Tuple.Create(register, registerInfo, accountResult.Data.Direction ?? 0);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<int> Load(DataCore.v21K2.D.AccountRegisterD registerData, DataCore.v21K2.D.PersonalRegisterD personData, bool isTestLoad, ProcessingOperations operations)
        {
            var result = new OperationResult<int>();
            try
            {
                IEnumerable<Tuple<FactPatient,FactPerson,FactDocument,IEnumerable<Tuple<FactMedicalEvent,IEnumerable<FactMedicalServices>,IEnumerable<FactMEC>,IEnumerable<FactMEE>,IEnumerable<FactEQMA>>>>> data = null;
                
                DateTime starttime = DateTime.Now;
                var LastPatientId = 0;

                string moName = string.Empty;
                //Check for already load account
                var existAccountResult = _repository.GetMedicalAccountView(
                    view => view.AccountNumber == registerData.Account.AccountNumber &&
                            view.Date.Value.Year == registerData.Account.Year &&
                            view.Date.Value.Month == registerData.Account.Month &&
                            view.AccountDate == registerData.Account.AccountDate &&
                            view.CodeMo == registerData.Account.MedicalOrganizationCode);
                if (existAccountResult.Success && existAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Счет №{0} МО {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, existAccountResult.Data.First().ShortNameMo, existAccountResult.Data.First().Date));
                    return result;
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    result.AddError(string.Format("В счете №{0} от МО {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, moName));
                    return result;
                }

                var account = new FactMedicalAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = registerData.Account.AccountNumber,
                    AccountDate = registerData.Account.AccountDate,
                    Price = registerData.Account.Price,
                    AcceptPrice = registerData.Account.AcceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    MedicalOrganization = _cache.Get(CacheRepository.F003Cache).GetBack(registerData.Account.MedicalOrganizationCode),
                    Comments = registerData.Account.Comments
                };
                    
                var total = registerData.RecordsCollection.Count;
                personData.PersonalsCollection = personData.PersonalsCollection
                        .OrderBy(p => p.Surname)
                        .ThenBy(p => p.PName)
                        .ThenBy(p => p.Patronymic).ToList();

                var lastPatientCount = 0;

                var patients = new List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<FactMedicalEvent, List<FactMedicalServices>, List<FactMEC>, List<FactMEE>, List<FactEQMA>>>>>();
                foreach (DataCore.v21K2.D.PersonalD entry in personData.PersonalsCollection)
                {
                        
                    FactPerson personDb = null;
                    FactDocument documentDb = null;

                    int personId = 0;
                    int? documentId = default(int?);

                    var personCopy = Map.ObjectToObject<FactPerson>(entry);

                    var personCheckResult = _repository.GetPerson(p => p.PName == entry.PName &&
                                p.Surname == entry.Surname &&
                                p.Patronymic == entry.Patronymic &&
                                p.Birthday == entry.Birthday &&
                                p.RepresentativeSurname == entry.RepresentativeSurname &&
                                p.RepresentativeName == entry.RepresentativeName &&
                                p.RepresentativePatronymic == entry.RepresentativePatronymic &&
                                p.RepresentativeBirthday == entry.RepresentativeBirthday);

                    if (personCheckResult.Success)
                    {
                        if (!personCheckResult.Data.Any())
                        {
                            personDb = personCopy;
                            if (entry.DocType.HasValue)
                            {
                                documentDb = new FactDocument
                                {
                                    DocType = entry.DocType,
                                    DocNum = entry.DocNumber,
                                    DocSeries = entry.DocSeries
                                };
                            }
                        }
                        else
                        {
                            personId = personCheckResult.Data.First().PersonId;
                            var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
                                                                                p.DocType == entry.DocType &&
                                                                                p.DocSeries == entry.DocSeries);

                            if (!docCheckResult.Data.Any())
                            {
                                if (entry.DocType.HasValue)
                                {
                                    documentDb = new FactDocument
                                    {
                                        PersonId = personId,
                                        DocType = entry.DocType,
                                        DocNum = entry.DocNumber,
                                        DocSeries = entry.DocSeries
                                    };
                                }
                            }
                            else
                            {
                                documentId = docCheckResult.Data.First().DocumentId;
                            }
                        }
                    }
                    else
                    {
                        result.AddError(new Exception("Ошибка поиска персональных данных"));
                        return result;
                    }

                    var patientCopy = registerData.RecordsCollection.FirstOrDefault(p => p.Patient.PatientGuid == entry.PatientGuid);
                    LastPatientId = patientCopy.ExternalId;

                    FactPatient patientDb = Map.ObjectToObject<FactPatient>(patientCopy.Patient);
                    if (personDb.IsNull())
                    {
                        patientDb.PersonalId = personId;
                    }
                    if (documentDb.IsNull())
                    {
                        patientDb.DocumentId = documentId;
                    }
                        
                    patientDb.MedicalExternalId = LastPatientId;

                    //TODO Зачем это ??? 
                    /*if (patientCopy.Patient.InsuranceId.HasValue && (!patientCopy.Patient.TerritoryOkato.HasValue || patientCopy.Patient.TerritoryOkato == 46))
                    {
                        var tfOkato = _cache.Get(CacheRepository.F002ToTfOkatoCache).GetString(patientCopy.Patient.InsuranceId);
                        patientDb.TerritoryOkato = _cache.Get(CacheRepository.F010Cache).GetBack(tfOkato);
                    }*/

                    patientDb.Comments = personCopy.Comments;
                    if (!patientDb.InsuranceDocType.HasValue)
                    {
                        if (patientCopy.Patient.InsuranceDocNumber.IsNotNullOrEmpty())
                        {
                            if (SrzAlgorithms.IsEnpK(patientCopy.Patient.InsuranceDocNumber))
                            {
                                patientDb.InsuranceDocType = 3;
                                patientDb.INP = patientCopy.Patient.InsuranceDocNumber;
                            }
                            else
                            {
                                patientDb.InsuranceDocType = 1;
                                patientDb.InsuranceDocNumber = patientCopy.Patient.InsuranceDocNumber;
                                patientDb.InsuranceDocSeries = patientCopy.Patient.InsuranceDocSeries;
                            }
                        }
                    }


                    var mevents = new List<Tuple<FactMedicalEvent, List<FactMedicalServices>, List<FactMEC>, List<FactMEE>, List<FactEQMA>>>();
                    foreach (DataCore.v21K2.D.EventD eventD in patientCopy.EventCollection)
                    {
                        var mevent = Map.ObjectToObject<FactMedicalEvent>(eventD);

                        mevent.AcceptPrice = eventD.AcceptPrice ?? eventD.Price;
                        mevent.MoPrice = mevent.Price;
                        mevent.Price = mevent.AcceptPrice;

                        mevent.MoPaymentStatus = GetPaymentStatus(mevent.AcceptPrice, mevent.MoPrice);
                        mevent.PaymentStatus = GetPaymentStatus(mevent.AcceptPrice, mevent.Price);
                            
                        if (eventD.ParticularCaseCollection != null && eventD.ParticularCaseCollection.Count > 0)
                        {
                            mevent.ParticularCase = eventD.ParticularCaseCollection.Aggregate((p, next) => string.Format("{0};{1}", p, next));
                        }

                        var mecs = new List<FactMEC>();
                        var mees = new List<FactMEE>();
                        var eqmas = new List<FactEQMA>();

                        foreach (DataCore.v21K2.D.RefusalD refusal in eventD.Refusals)
                        {
                            if (refusal == null)
                                continue;

                            switch ((RefusalType?)refusal.RefusalType)
                            {
                                case RefusalType.MEC:
                                    var mec = new FactMEC
                                    {
                                        ReasonId = refusal.RefusalCode,
                                        Amount = refusal.RefusalRate,
                                        Source = (int)RefusalSource.Local,
                                        Comments = refusal.Comments,
                                        ExternalGuid = refusal.ExternalGuid
                                    };
                                    mecs.Add(mec);
                                    break;
                                case RefusalType.MEE:
                                    var mee = new FactMEE
                                    {
                                        ReasonId = refusal.RefusalCode,
                                        Amount = refusal.RefusalRate,
                                        Source = (int)RefusalSource.Local,
                                        Comments = refusal.Comments,
                                        ExternalGuid = refusal.ExternalGuid
                                    };
                                    mees.Add(mee);
                                    break;
                                case RefusalType.EQMA:
                                    var eqma = new FactEQMA
                                    {
                                        ReasonId = refusal.RefusalCode,
                                        Amount = refusal.RefusalRate,
                                        Source = (int)RefusalSource.Local,
                                        Comments = refusal.Comments,
                                        ExternalGuid = refusal.ExternalGuid
                                    };
                                    eqmas.Add(eqma);
                                    break;
                            }
                        }

                        var services = new List<FactMedicalServices>();
                        foreach (DataCore.v21K2.D.ServiceD serviceD in eventD.Service)
                        {
                            var serviceCopy = serviceD;
                            var service = Map.ObjectToObject<FactMedicalServices>(serviceCopy);
                            service.ExternalGUID = service.ExternalGUID.IsNullOrWhiteSpace()
                                ? Guid.NewGuid().ToString()
                                : service.ExternalGUID;
                            services.Add(service);
                        }

                        mevents.Add(Tuple.Create(mevent,services, mecs, mees, eqmas));
                    }

                    patients.Add(Tuple.Create(patientDb, personDb, documentDb, mevents));
                    
                    lastPatientCount++;
                    _dockLayoutManager.SetOverlayProgress(total,lastPatientCount);
                }

                var insertRegisterResult = _repository.InsertRegisterD(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            
            return result;
        }

        public OperationResult<int> Load(v30K1.D.AccountRegisterD registerData, v30K1.D.PersonalRegisterD personData, bool isTestLoad, ProcessingOperations operations)
        {
            var result = new OperationResult<int>();
            try
            {
               // IEnumerable<Tuple<FactPatient, FactPerson, FactDocument, IEnumerable<Tuple<FactMedicalEvent, IEnumerable<FactMedicalServices>, IEnumerable<FactMEC>, IEnumerable<FactMEE>, IEnumerable<FactEQMA>>>>> data = null;

                DateTime starttime = DateTime.Now;
                var LastPatientId = 0;

                string moName = string.Empty;
                //Check for already load account
                var existAccountResult = _repository.GetMedicalAccountView(
                    view => view.AccountNumber == registerData.Account.AccountNumber &&
                            view.Date.Value.Year == registerData.Account.Year &&
                            view.Date.Value.Month == registerData.Account.Month &&
                            view.AccountDate == registerData.Account.AccountDate &&
                            view.CodeMo == registerData.Account.MedicalOrganizationCode);
                if (existAccountResult.Success && existAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Счет №{0} МО {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, existAccountResult.Data.First().ShortNameMo, existAccountResult.Data.First().Date));
                    return result;
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    result.AddError(string.Format("В счете №{0} от МО {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, moName));
                    return result;
                }

                var account = new FactMedicalAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = registerData.Account.AccountNumber,
                    AccountDate = registerData.Account.AccountDate,
                    Price = registerData.Account.Price,
                    AcceptPrice = registerData.Account.AcceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    MedicalOrganization = _cache.Get(CacheRepository.F003Cache).GetBack(registerData.Account.MedicalOrganizationCode),
                    Comments = registerData.Account.Comments,
                    Version = _cache.Get(CacheRepository.VersionCache).GetBack(registerData.Header.Version)
                };

                var total = registerData.RecordsCollection.Count;
                personData.PersonalsCollection = personData.PersonalsCollection
                        .OrderBy(p => p.Surname)
                        .ThenBy(p => p.PName)
                        .ThenBy(p => p.Patronymic).ToList();

                var lastPatientCount = 0;

                var patients = new List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactSank>>>>>>>();
                foreach (v30K1.D.PersonalD entry in personData.PersonalsCollection)
                {

                    FactPerson personDb = null;
                    FactDocument documentDb = null;

                    int personId = 0;
                    int? documentId = default(int?);

                    var personCopy = Map.ObjectToObject<FactPerson>(entry);

                    var personCheckResult = _repository.GetPerson(p => p.PName == entry.PName &&
                                p.Surname == entry.Surname &&
                                p.Patronymic == entry.Patronymic &&
                                p.Birthday == entry.Birthday &&
                                p.RepresentativeSurname == entry.RepresentativeSurname &&
                                p.RepresentativeName == entry.RepresentativeName &&
                                p.RepresentativePatronymic == entry.RepresentativePatronymic &&
                                p.RepresentativeBirthday == entry.RepresentativeBirthday);

                    if (personCheckResult.Success)
                    {
                        if (!personCheckResult.Data.Any())
                        {
                            personDb = personCopy;
                            if (entry.DocType.HasValue)
                            {
                                documentDb = new FactDocument
                                {
                                    DocType = entry.DocType,
                                    DocNum = entry.DocNumber,
                                    DocSeries = entry.DocSeries
                                };
                            }
                        }
                        else
                        {
                            personId = personCheckResult.Data.First().PersonId;
                            var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
                                                                                p.DocType == entry.DocType &&
                                                                                p.DocSeries == entry.DocSeries);

                            if (!docCheckResult.Data.Any())
                            {
                                if (entry.DocType.HasValue)
                                {
                                    documentDb = new FactDocument
                                    {
                                        PersonId = personId,
                                        DocType = entry.DocType,
                                        DocNum = entry.DocNumber,
                                        DocSeries = entry.DocSeries
                                    };
                                }
                            }
                            else
                            {
                                documentId = docCheckResult.Data.First().DocumentId;
                            }
                        }
                    }
                    else
                    {
                        result.AddError(new Exception("Ошибка поиска персональных данных"));
                        return result;
                    }

                    var patientCopy = registerData.RecordsCollection.FirstOrDefault(p => p.Patient.PatientGuid == entry.PatientGuid);
                    LastPatientId = patientCopy.ExternalId;

                    FactPatient patientDb = Map.ObjectToObject<FactPatient>(patientCopy.Patient);
                    if (personDb.IsNull())
                    {
                        patientDb.PersonalId = personId;
                    }
                    if (documentDb.IsNull())
                    {
                        patientDb.DocumentId = documentId;
                    }

                    patientDb.MedicalExternalId = LastPatientId;

                    //TODO Зачем это ??? 
                    /*if (patientCopy.Patient.InsuranceId.HasValue && (!patientCopy.Patient.TerritoryOkato.HasValue || patientCopy.Patient.TerritoryOkato == 46))
                    {
                        var tfOkato = _cache.Get(CacheRepository.F002ToTfOkatoCache).GetString(patientCopy.Patient.InsuranceId);
                        patientDb.TerritoryOkato = _cache.Get(CacheRepository.F010Cache).GetBack(tfOkato);
                    }*/

                    patientDb.Comments = personCopy.Comments;
                    if (!patientDb.InsuranceDocType.HasValue)
                    {
                        if (patientCopy.Patient.InsuranceDocNumber.IsNotNullOrEmpty())
                        {
                            if (SrzAlgorithms.IsEnpK(patientCopy.Patient.InsuranceDocNumber))
                            {
                                patientDb.InsuranceDocType = 3;
                                patientDb.INP = patientCopy.Patient.InsuranceDocNumber;
                            }
                            else
                            {
                                patientDb.InsuranceDocType = 1;
                                patientDb.InsuranceDocNumber = patientCopy.Patient.InsuranceDocNumber;
                                patientDb.InsuranceDocSeries = patientCopy.Patient.InsuranceDocSeries;
                            }
                        }
                    }


                    var mevents = new List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactSank>>>>>();
                    foreach (v30K1.D.ZslEventD zslEventD in patientCopy.EventCollection)
                    {
                        var zslevent = Map.ObjectToObject<ZslFactMedicalEvent>(zslEventD);

                        zslevent.AcceptPrice = zslEventD.AcceptPrice ?? zslEventD.Price;//
                        zslevent.MoPrice = zslevent.Price;
                        zslevent.Price = zslevent.Price;

                        zslevent.MoPaymentStatus = GetPaymentStatus(zslevent.AcceptPrice, zslevent.MoPrice);
                        zslevent.PaymentStatus = GetPaymentStatus(zslevent.AcceptPrice, zslevent.Price);

                        if (zslEventD.ParticularCaseCollection != null && zslEventD.ParticularCaseCollection.Count > 0)
                        {
                            zslevent.ParticularCase = zslEventD.ParticularCaseCollection.Aggregate((p, next) => string.Format("{0};{1}", p, next));
                        }

                        var zevents = new List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactSank>>>();
                        foreach (var eventD in zslEventD.Event)
                        {
                            var eventDCopy = eventD;
                            var zeventD = Map.ObjectToObject<ZFactMedicalEvent>(eventDCopy);
                            zeventD.MoPrice = eventD.EventPrice;

                            var slKoefs = new List<ZFactSlKoef>();
                            ZFactKsgKpg ksgKpg = null;
                            if (eventD.InnerKsgKpg != null)
                            {
                                ksgKpg = Map.ObjectToObject<ZFactKsgKpg>(eventD.InnerKsgKpg);

                                foreach (var slKoefD in eventD.InnerKsgKpg.InnerSlCoefCollection)
                                {
                                    if (slKoefD == null)
                                        continue;
                                    var slKoef = new ZFactSlKoef
                                    {
                                        NumberDifficultyTreatment = slKoefD.NumberDifficultyTreatment,
                                        ValueDifficultyTreatment = slKoefD.ValueDifficultyTreatment
                                    };
                                    slKoefs.Add(slKoef);
                                }
                            }

                            var sanks = new List<ZFactSank>();
                            foreach (v30K1.D.RefusalD refusal in eventD.Refusals)
                            {
                                if (refusal == null)
                                    continue;

                                var sank = new ZFactSank
                                {
                                    ReasonId = refusal.RefusalCode,
                                    Amount = refusal.RefusalRate,
                                    Source = (int) RefusalSource.Local,
                                    Comments = refusal.Comments,
                                    Type = refusal.RefusalType,
                                    ExternalGuid = refusal.ExternalGuid,
                                    Date = refusal.Date,
                                    EmployeeId = Constants.SystemAccountId
                                };
                                sanks.Add(sank);
                            }

                            var services = new List<ZFactMedicalServices>();
                            foreach (v30K1.D.ServiceD serviceD in eventD.Service)
                            {
                                var serviceCopy = serviceD;
                                var service = Map.ObjectToObject<ZFactMedicalServices>(serviceCopy);
                                service.ExternalGUID = service.ExternalGUID.IsNullOrWhiteSpace()
                                    ? Guid.NewGuid().ToString()
                                    : service.ExternalGUID;
                                services.Add(service);
                            }
                            zevents.Add(Tuple.Create(zeventD, Tuple.Create(ksgKpg, slKoefs),  services, sanks));
                        }
                        mevents.Add(Tuple.Create(zslevent, zevents));
                    }

                    patients.Add(Tuple.Create(patientDb, personDb, documentDb, mevents));

                    lastPatientCount++;
                    _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                }

                var insertRegisterResult = _repository.InsertRegisterD(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<int> LoadZ(v30K1.D.AccountRegisterD registerData, v30K1.D.PersonalRegisterD personData, bool isTestLoad, ProcessingOperations operations)
        {
            var result = new OperationResult<int>();
            try
            {
                // IEnumerable<Tuple<FactPatient, FactPerson, FactDocument, IEnumerable<Tuple<FactMedicalEvent, IEnumerable<FactMedicalServices>, IEnumerable<FactMEC>, IEnumerable<FactMEE>, IEnumerable<FactEQMA>>>>> data = null;

                DateTime starttime = DateTime.Now;
                var LastPatientId = 0;

                string moName = string.Empty;
                //Check for already load account
                var existAccountResult = _repository.GetMedicalAccountView(
                    view => view.AccountNumber == registerData.Account.AccountNumber &&
                            view.Date.Value.Year == registerData.Account.Year &&
                            view.Date.Value.Month == registerData.Account.Month &&
                            view.AccountDate == registerData.Account.AccountDate &&
                            view.CodeMo == registerData.Account.MedicalOrganizationCode);
                if (existAccountResult.Success && existAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Счет №{0} МО {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, existAccountResult.Data.First().ShortNameMo, existAccountResult.Data.First().Date));
                    return result;
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    result.AddError(string.Format("В счете №{0} от МО {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, moName));
                    return result;
                }
                dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
                string dest = Convert.ToString(terCode.tf_code);
                var account = new FactMedicalAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = registerData.Account.AccountNumber,
                    AccountDate = registerData.Account.AccountDate,
                    Price = registerData.Account.Price,
                    AcceptPrice = registerData.Account.AcceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    MedicalOrganization = _cache.Get(CacheRepository.F003Cache).GetBack(registerData.Account.MedicalOrganizationCode),
                    Comments = registerData.Account.Comments,
                    Version = dest == "46" ? _cache.Get(CacheRepository.VersionCache).GetBack(registerData.Header.Version):
                    4
                };

                var total = registerData.RecordsCollection.Count;
                personData.PersonalsCollection = personData.PersonalsCollection
                        .OrderBy(p => p.Surname)
                        .ThenBy(p => p.PName)
                        .ThenBy(p => p.Patronymic).ToList();

                var lastPatientCount = 0;

                var patients = new List<Tuple<FactPerson, FactDocument, List<Tuple<FactPatient, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactSank>>>>>>>>>();

                var pers = new List<Tuple<FactPerson, FactDocument, int, int?, v30K1.D.PersonalD>>();
                foreach (v30K1.D.PersonalD entry in personData.PersonalsCollection)
                {
                    FactPerson personDb = null;
                    FactDocument documentDb = null;

                    int personId = 0;
                    int? documentId = default(int?);

                    var personCopy = Map.ObjectToObject<FactPerson>(entry);

                    var personCheckResult = _repository.GetPerson(p => p.PName == entry.PName &&
                                                                       p.Surname == entry.Surname &&
                                                                       p.Patronymic == entry.Patronymic &&
                                                                       p.Birthday == entry.Birthday &&
                                                                       p.RepresentativeSurname ==
                                                                       entry.RepresentativeSurname &&
                                                                       p.RepresentativeName == entry.RepresentativeName &&
                                                                       p.RepresentativePatronymic ==
                                                                       entry.RepresentativePatronymic &&
                                                                       p.RepresentativeBirthday ==
                                                                       entry.RepresentativeBirthday);

                    if (personCheckResult.Success)
                    {
                        if (!personCheckResult.Data.Any())
                        {
                            personDb = personCopy;
                            if (entry.DocType.HasValue)
                            {
                                documentDb = new FactDocument
                                {
                                    DocType = entry.DocType,
                                    DocNum = entry.DocNumber,
                                    DocSeries = entry.DocSeries
                                };
                            }
                        }
                        else
                        {
                            personId = personCheckResult.Data.First().PersonId;
                            personDb = personCheckResult.Data.First();
                            var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
                                                                              p.DocType == entry.DocType &&
                                                                              p.DocSeries == entry.DocSeries);

                            if (!docCheckResult.Data.Any())
                            {
                                if (entry.DocType.HasValue)
                                {
                                    documentDb = new FactDocument
                                    {
                                        PersonId = personId,
                                        DocType = entry.DocType,
                                        DocNum = entry.DocNumber,
                                        DocSeries = entry.DocSeries
                                    };
                                }
                            }
                            else
                            {
                                documentId = docCheckResult.Data.First().DocumentId;
                            }
                        }
                    }
                    else
                    {
                        result.AddError(new Exception("Ошибка поиска персональных данных"));
                        return result;
                    }
                    pers.Add(Tuple.Create(personDb, documentDb, personId, documentId, entry));

                    // var patientCopy = registerData.RecordsCollection.FirstOrDefault(p => p.Patient.PatientGuid == entry.PatientGuid);
                }


                foreach (var recordsCollection in registerData.RecordsCollection)
                {
                    var patMevents = new List<Tuple<FactPatient, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactSank>>>>>>>();

                    var patientCopy = recordsCollection.Patient;
                    var per = pers.FirstOrDefault(x => x.Item5.PatientGuid == patientCopy.PatientGuid);

                    FactPatient patientDb = Map.ObjectToObject<FactPatient>(patientCopy);

                    if (per.Item1.IsNull())
                    {
                        patientDb.PersonalId = per.Item3;
                    }
                    if (per.Item2.IsNull())
                    {
                        patientDb.DocumentId = per.Item4;
                    }

                    patientDb.MedicalExternalId = recordsCollection.ExternalId;
                    patientDb.Comments = per.Item1?.Comments;

                    if (!patientDb.InsuranceDocType.HasValue)
                    {
                        if (patientCopy.InsuranceDocNumber.IsNotNullOrEmpty())
                        {
                            if (SrzAlgorithms.IsEnpK(patientCopy.InsuranceDocNumber))
                            {
                                patientDb.InsuranceDocType = 3;
                                patientDb.INP = patientCopy.InsuranceDocNumber;
                            }
                            else
                            {
                                patientDb.InsuranceDocType = 1;
                                patientDb.InsuranceDocNumber = patientCopy.InsuranceDocNumber;
                                patientDb.InsuranceDocSeries = patientCopy.InsuranceDocSeries;
                            }
                        }
                    }

                    var mevents = new List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactSank>>>>>();
                    foreach (v30K1.D.ZslEventD zslEventD in recordsCollection.EventCollection)
                    {
                        var zslevent = Map.ObjectToObject<ZslFactMedicalEvent>(zslEventD);

                        zslevent.AcceptPrice = zslEventD.AcceptPrice ?? zslEventD.Price;//
                        zslevent.MoPrice = zslevent.Price;
                        zslevent.Price = zslevent.Price;

                        zslevent.MoPaymentStatus = GetPaymentStatus(zslevent.AcceptPrice, zslevent.MoPrice);
                        zslevent.PaymentStatus = GetPaymentStatus(zslevent.AcceptPrice, zslevent.Price);

                        if (zslEventD.ParticularCaseCollection != null && zslEventD.ParticularCaseCollection.Count > 0)
                        {
                            zslevent.ParticularCase = zslEventD.ParticularCaseCollection.Aggregate((p, next) => string.Format("{0};{1}", p, next));
                        }

                        var zevents = new List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactSank>>>();
                        foreach (var eventD in zslEventD.Event)
                        {
                            var eventDCopy = eventD;
                            var zeventD = Map.ObjectToObject<ZFactMedicalEvent>(eventDCopy);
                            zeventD.MoPrice = eventD.EventPrice;

                            var slKoefs = new List<ZFactSlKoef>();
                            ZFactKsgKpg ksgKpg = null;
                            if (eventD.InnerKsgKpg != null)
                            {
                                ksgKpg = Map.ObjectToObject<ZFactKsgKpg>(eventD.InnerKsgKpg);

                                foreach (var slKoefD in eventD.InnerKsgKpg.InnerSlCoefCollection)
                                {
                                    if (slKoefD == null)
                                        continue;
                                    var slKoef = new ZFactSlKoef
                                    {
                                        NumberDifficultyTreatment = slKoefD.NumberDifficultyTreatment,
                                        ValueDifficultyTreatment = slKoefD.ValueDifficultyTreatment
                                    };
                                    slKoefs.Add(slKoef);
                                }
                            }

                            var sanks = new List<ZFactSank>();
                            foreach (v30K1.D.RefusalD refusal in eventD.Refusals)
                            {
                                if (refusal == null)
                                    continue;

                                var sank = new ZFactSank
                                {
                                    ReasonId = refusal.RefusalCode,
                                    Amount = refusal.RefusalRate,
                                    Source = (int)RefusalSource.Local,
                                    Comments = refusal.Comments,
                                    Type = refusal.RefusalType,
                                    ExternalGuid = refusal.ExternalGuid,
                                    Date = refusal.Date,
                                    EmployeeId = Constants.SystemAccountId
                                };
                                sanks.Add(sank);
                            }

                            var services = new List<ZFactMedicalServices>();
                            foreach (v30K1.D.ServiceD serviceD in eventD.Service)
                            {
                                var serviceCopy = serviceD;
                                var service = Map.ObjectToObject<ZFactMedicalServices>(serviceCopy);
                                service.ExternalGUID = service.ExternalGUID.IsNullOrWhiteSpace()
                                    ? Guid.NewGuid().ToString()
                                    : service.ExternalGUID;
                                services.Add(service);
                            }
                            zevents.Add(Tuple.Create(zeventD, Tuple.Create(ksgKpg, slKoefs), services, sanks));
                        }
                        mevents.Add(Tuple.Create(zslevent, zevents));
                    }

                    patMevents.Add(Tuple.Create(patientDb, mevents));

                    patients.Add(Tuple.Create(per.Item1, per.Item2, patMevents));

                    lastPatientCount++;
                    _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                }

                var insertRegisterResult = _repository.InsertRegisterD(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.Errors[0]);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<int> LoadZ_testParallel(v31K1.D.AccountRegisterD registerData, v31K1.D.PersonalRegisterD personData, bool isTestLoad, ProcessingOperations operations, ProcessingOperations processing)
        {
            var result = new OperationResult<int>();
            try
            {
                //// IEnumerable<Tuple<FactPatient, FactPerson, FactDocument, IEnumerable<Tuple<FactMedicalEvent, IEnumerable<FactMedicalServices>, IEnumerable<FactMEC>, IEnumerable<FactMEE>, IEnumerable<FactEQMA>>>>> data = null;

                //DateTime starttime = DateTime.Now;
                //var LastPatientId = 0;

                //string moName = string.Empty;
                ////Check for already load account
                //var existAccountResult = _repository.GetMedicalAccountView(
                //    view => view.AccountNumber == registerData.Account.AccountNumber &&
                //            view.Date.Value.Year == registerData.Account.Year &&
                //            view.Date.Value.Month == registerData.Account.Month &&
                //            view.AccountDate == registerData.Account.AccountDate &&
                //            view.CodeMo == registerData.Account.MedicalOrganizationCode);

                ////Sql.DateDiff(Sql.DateParts.Year, view.Date, registerData.Header.Date) == 0 &&
                ////          Sql.DateDiff(Sql.DateParts.Month, view.AccountDate, registerData.Header.Date) == 0 &&
                //if (existAccountResult.Success && existAccountResult.Data.Any())
                //{
                //    result.AddError(string.Format("Счет №{0} МО {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                //                                registerData.Account.AccountNumber, existAccountResult.Data.First().ShortNameMo, existAccountResult.Data.First().Date));
                //    return result;
                //}

                //if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                //{
                //    result.AddError(string.Format("В счете №{0} от МО {1} отсутствует отчетная дата (месяц, год).",
                //                                    registerData.Account.AccountNumber, moName));
                //    return result;
                //}
                //dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
                //string dest = Convert.ToString(terCode.tf_code);
                //var account = new FactMedicalAccount
                //{
                //    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                //    AccountNumber = registerData.Account.AccountNumber,
                //    AccountDate = registerData.Account.AccountDate,
                //    Price = registerData.Account.Price,
                //    AcceptPrice = registerData.Account.AcceptPrice,
                //    MECPenalties = registerData.Account.MECPenalties,
                //    MEEPenalties = registerData.Account.MEEPenalties,
                //    EQMAPenalties = registerData.Account.EQMAPenalties,
                //    Status = AccountStatus.NotProcessed.ToInt32(),
                //    MedicalOrganization = _cache.Get(CacheRepository.F003Cache).GetBack(registerData.Account.MedicalOrganizationCode),
                //    Comments = registerData.Account.Comments,
                //    Version = dest == "46" ?_cache.Get(CacheRepository.VersionCache).GetBack(registerData.Header.Version):
                //    6
                //};

                //var total = registerData.RecordsCollection.Count;
                //personData.PersonalsCollection = personData.PersonalsCollection
                //        .OrderBy(p => p.Surname)
                //        .ThenBy(p => p.PName)
                //        .ThenBy(p => p.Patronymic).ToList();

                //var lastPatientCount = 0;

                ////var patients = new List<Tuple<FactPerson, FactDocument, List<Tuple<FactPatient, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, List<ZFactDirection>, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>>, List<Tuple<ZFactMedicalServices, ZFactMedicalServicesOnk>>, List<ZFactSank>>>>>>>>>();
                //var patients = new List<Tuple<FactPerson, FactDocument, List<Tuple<FactPatient, 
                //    List<Tuple<ZslFactMedicalEvent, 
                //        List<Tuple<ZFactMedicalEvent, 
                //            List<ZFactDirection>, 
                //            List<ZFactConsultations>, 
                //            Tuple<ZFactMedicalEventOnk, 
                //                List<ZFactDiagBlok>, 
                //                List<ZFactContraindications>, 
                //                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>, 
                //            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
                //        List<ZFactSank>
                //            >>>>>>();

                //var pers = new List<Tuple<FactPerson, FactDocument, int, int?, v31K1.D.PersonalD>>();
                //Parallel.ForEach(personData.PersonalsCollection, entry =>
                //{
                //    FactPerson personDb = null;
                //    FactDocument documentDb = null;

                //    int personId = 0;
                //    int? documentId = default(int?);

                //    var personCopy = Map.ObjectToObject<FactPerson>(entry);

                //    var personCheckResult = _repository.GetPerson(p => p.PName == entry.PName &&
                //                                                       p.Surname == entry.Surname &&
                //                                                       p.Patronymic == entry.Patronymic &&
                //                                                       p.Birthday == entry.Birthday &&
                //                                                       p.RepresentativeSurname ==
                //                                                       entry.RepresentativeSurname &&
                //                                                       p.RepresentativeName == entry.RepresentativeName &&
                //                                                       p.RepresentativePatronymic ==
                //                                                       entry.RepresentativePatronymic &&
                //                                                       p.RepresentativeBirthday ==
                //                                                       entry.RepresentativeBirthday);

                //    if (personCheckResult.Success)
                //    {
                //        if (!personCheckResult.Data.Any())
                //        {
                //            personDb = personCopy;
                //            if (entry.DocType.HasValue)
                //            {
                //                documentDb = new FactDocument
                //                {
                //                    DocType = entry.DocType,
                //                    DocNum = entry.DocNumber,
                //                    DocSeries = entry.DocSeries
                //                };
                //            }
                //        }
                //        else
                //        {
                //            personId = personCheckResult.Data.First().PersonId;
                //            personDb = personCheckResult.Data.First();
                //            var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
                //                                                              p.DocType == entry.DocType &&
                //                                                              p.DocSeries == entry.DocSeries);

                //            if (!docCheckResult.Data.Any())
                //            {
                //                if (entry.DocType.HasValue)
                //                {
                //                    documentDb = new FactDocument
                //                    {
                //                        PersonId = personId,
                //                        DocType = entry.DocType,
                //                        DocNum = entry.DocNumber,
                //                        DocSeries = entry.DocSeries
                //                    };
                //                }
                //            }
                //            else
                //            {
                //                documentId = docCheckResult.Data.First().DocumentId;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        result.AddError(new Exception("Ошибка поиска персональных данных"));
                //        //return result;
                //    }
                //    pers.Add(Tuple.Create(personDb, documentDb, personId, documentId, entry));
                //});

                //Parallel.ForEach(registerData.RecordsCollection, recordsCollection =>
                //{
                //    var patMevents = new List<Tuple<FactPatient,
                //        List<Tuple<ZslFactMedicalEvent,
                //            List<Tuple<ZFactMedicalEvent,
                //                List<ZFactDirection>,
                //                List<ZFactConsultations>,
                //                Tuple<ZFactMedicalEventOnk,
                //                    List<ZFactDiagBlok>,
                //                    List<ZFactContraindications>,
                //                    List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                //                Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
                //            List<ZFactSank>
                //            >>>>();
                //    var patientCopy = recordsCollection.Patient;
                //    var per = pers.FirstOrDefault(x => x.Item5.PatientGuid == patientCopy.PatientGuid);


                //    //if (operations.Has(ProcessingOperations.NotInTerr))
                //    //{
                //    //    if (patientCopy.Smo.IsNotNull())
                //    //    {
                //    //        if (patientCopy.Smo.StartsWith(dest))
                //    //        {
                //    //            continue;
                //    //        }
                //    //    }
                //    //}
                //    FactPatient patientDb = Map.ObjectToObject<FactPatient>(patientCopy);

                //    if (per.Item1.IsNull())
                //    {
                //        patientDb.PersonalId = per.Item3;
                //    }
                //    if (per.Item2.IsNull())
                //    {
                //        patientDb.DocumentId = per.Item4;
                //    }

                //    patientDb.MedicalExternalId = recordsCollection.ExternalId;
                //    patientDb.Comments = per.Item1?.Comments;

                //    if (!patientDb.InsuranceDocType.HasValue)
                //    {
                //        if (patientCopy.InsuranceDocNumber.IsNotNullOrEmpty())
                //        {
                //            if (SrzAlgorithms.IsEnpK(patientCopy.InsuranceDocNumber))
                //            {
                //                patientDb.InsuranceDocType = 3;
                //                patientDb.INP = patientCopy.InsuranceDocNumber;
                //            }
                //            else
                //            {
                //                patientDb.InsuranceDocType = 1;
                //                patientDb.InsuranceDocNumber = patientCopy.InsuranceDocNumber;
                //                patientDb.InsuranceDocSeries = patientCopy.InsuranceDocSeries;
                //            }
                //        }
                //    }

                //    // var mevents = new List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>>, List<Tuple<ZFactMedicalServices, List<ZFactDirection>, ZFactMedicalServicesOnk>>, List<ZFactSank>>>>>();
                //    var mevents = new List<Tuple<ZslFactMedicalEvent,
                //        List<Tuple<ZFactMedicalEvent,
                //            List<ZFactDirection>,
                //            List<ZFactConsultations>,
                //            Tuple<ZFactMedicalEventOnk,
                //                List<ZFactDiagBlok>,
                //                List<ZFactContraindications>,
                //                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                //            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
                //        List<ZFactSank>
                //        >>();
                //    Parallel.ForEach(recordsCollection.EventCollection, zslEventD =>
                //    {
                //        var zslevent = Map.ObjectToObject<ZslFactMedicalEvent>(zslEventD);

                //        zslevent.AcceptPrice = zslEventD.AcceptPrice ?? zslEventD.Price;//
                //        zslevent.MoPrice = zslevent.Price;
                //        zslevent.Price = zslevent.Price;

                //        zslevent.MoPaymentStatus = GetPaymentStatus(zslevent.AcceptPrice, zslevent.MoPrice);
                //        zslevent.PaymentStatus = GetPaymentStatus(zslevent.AcceptPrice, zslevent.Price);

                //        if (zslEventD.ParticularCaseCollection != null && zslEventD.ParticularCaseCollection.Count > 0)
                //        {
                //            zslevent.ParticularCase = zslEventD.ParticularCaseCollection.Aggregate((p, next) => string.Format("{0};{1}", p, next));
                //        }

                //        //var zevents = new List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>>, List<Tuple<ZFactMedicalServices, List<ZFactDirection>, ZFactMedicalServicesOnk>>, List<ZFactSank>>>();
                //        var zevents = new List<Tuple<ZFactMedicalEvent,
                //            List<ZFactDirection>,
                //            List<ZFactConsultations>,
                //            Tuple<ZFactMedicalEventOnk,
                //                List<ZFactDiagBlok>,
                //                List<ZFactContraindications>,
                //                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                //            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>();
                //        Parallel.ForEach(zslEventD.Event, eventD =>
                //        {
                //            var eventDCopy = eventD;
                //            var zeventD = Map.ObjectToObject<ZFactMedicalEvent>(eventDCopy);
                //            zeventD.MoPrice = eventD.EventPrice;

                //            var directions = new List<ZFactDirection>();

                //            Parallel.ForEach(eventD.DirectionOnkXml, direc =>
                //            {
                //                if (direc != null)
                //                {
                //                    var direction = new ZFactDirection
                //                    {
                //                        DirectionDate = direc.DirectionDate,
                //                        DirectionMo = direc.DirectionMo,
                //                        DirectionViewId = direc.DirectionViewId,
                //                        MetIsslId = direc.MetIsslId,
                //                        DirectionService = direc.DirectionService
                //                    };
                //                    directions.Add(direction);
                //                }
                //            });

                //            var сonsultations = new List<ZFactConsultations>();
                //            Parallel.ForEach(eventD.ConsultationsOnkXml, cons =>
                //            {
                //                if (cons != null)
                //                {
                //                    var сonsultation = new ZFactConsultations
                //                    {
                //                        DtCons = cons.DtCons,
                //                        PrCons = cons.PrCons
                //                    };
                //                    сonsultations.Add(сonsultation);
                //                }
                //            });

                //            var slKoefs = new List<ZFactSlKoef>();
                //            var critList = new List<ZFactCrit>();
                //            ZFactKsgKpg ksgKpg = null;
                //            if (eventD.KsgKpg != null)
                //            {
                //                ksgKpg = Map.ObjectToObject<ZFactKsgKpg>(eventD.KsgKpg);
                //                Parallel.ForEach(eventD.KsgKpg.CritXml, i =>
                //                {
                //                    critList.Add(new ZFactCrit { IdDkk = i });
                //                });
                //                Parallel.ForEach(eventD.KsgKpg.InnerSlCoefCollection, slKoefD =>
                //                {
                //                    if (slKoefD != null)
                //                    {
                //                        var slKoef = new ZFactSlKoef
                //                        {
                //                            NumberDifficultyTreatment = slKoefD.NumberDifficultyTreatment,
                //                            ValueDifficultyTreatment = slKoefD.ValueDifficultyTreatment
                //                        };
                //                        slKoefs.Add(slKoef);
                //                    }
                //                });
                //            }

                //            var medicalEventOnk =
                //                new Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>,
                //                    List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>(null, null, null, null);
                //            if (eventD.EventOnk != null)
                //            {
                //                var diagBloks = new List<ZFactDiagBlok>();
                //                var contraindications = new List<ZFactContraindications>();
                //                var serviceUslOnk = new List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>();
                //                Parallel.ForEach(eventD.EventOnk.DiagBlokOnk, diagBlokOnk =>
                //                {
                //                    if (diagBlokOnk != null)
                //                    { diagBloks.Add(Map.ObjectToObject<ZFactDiagBlok>(diagBlokOnk)); }
                //                });

                //                Parallel.ForEach(eventD.EventOnk.СontraindicationsOnk, сontraindicationsOnk =>
                //                {
                //                    if (сontraindicationsOnk != null)
                //                    {
                //                        contraindications.Add(Map.ObjectToObject<ZFactContraindications>(сontraindicationsOnk));
                //                    }
                //                });
                //                Parallel.ForEach(eventD.EventOnk.ServiceOnk, serviceOnk =>
                //                {
                //                    if (serviceOnk != null)
                //                    {
                //                        var anticancerDrugOnkDs = new List<ZFactAnticancerDrug>();
                //                        Parallel.ForEach(serviceOnk.AnticancerDrugOnkXml, anticancerDrugOnkD =>
                //                        {
                //                            if (anticancerDrugOnkD != null)
                //                            {
                //                                anticancerDrugOnkDs.Add(Map.ObjectToObject<ZFactAnticancerDrug>(anticancerDrugOnkD));
                //                            }
                //                        });
                //                        serviceUslOnk.Add(Tuple.Create(Map.ObjectToObject<ZFactMedicalServicesOnk>(serviceOnk), anticancerDrugOnkDs));
                //                    }
                //                });
                              

                //                medicalEventOnk = Tuple.Create(Map.ObjectToObject<ZFactMedicalEventOnk>(eventD.EventOnk), diagBloks, contraindications, serviceUslOnk);
                //            }
                //            var services = new List<ZFactMedicalServices>();
                //            Parallel.ForEach(eventD.Service, serviceD =>
                //            {
                //                var serviceCopy = serviceD;
                //                var service = Map.ObjectToObject<ZFactMedicalServices>(serviceCopy);
                //                service.ExternalGUID = service.ExternalGUID.IsNullOrWhiteSpace()
                //                    ? Guid.NewGuid().ToString()
                //                    : service.ExternalGUID;
                //                services.Add(service);
                //            });
                //            zevents.Add(Tuple.Create(zeventD, directions, сonsultations, medicalEventOnk, Tuple.Create(ksgKpg, critList, slKoefs), services));

                //        });

                //        var sanks = new List<ZFactSank>();
                //        Parallel.ForEach(zslEventD.Refusals, refusal =>
                //        {

                //            if (refusal != null)
                //            {
                //                var sank = new ZFactSank
                //                {
                //                    ReasonId = refusal.RefusalCode,
                //                    SlidGuid = refusal.SlidGuid,
                //                    Amount = refusal.RefusalRate,
                //                    Source = (int)RefusalSource.Local,
                //                    Comments = refusal.Comments,
                //                    Type = refusal.RefusalType,
                //                    ExternalGuid = refusal.ExternalGuid,
                //                    Date = refusal.Date,
                //                    EmployeeId = Constants.SystemAccountId
                //                };
                //                sanks.Add(sank);
                //            }
                //        });

                //        mevents.Add(Tuple.Create(zslevent, zevents, sanks));

                //    });

                //    patMevents.Add(Tuple.Create(patientDb, mevents));

                //    patients.Add(Tuple.Create(per.Item1, per.Item2, patMevents));

                //    lastPatientCount++;
                //    _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);

                //});

                //var insertRegisterResult = _repository.InsertRegisterD(account, patients, isTestLoad);

                //if (insertRegisterResult.Success)
                //{
                //    result.Data = insertRegisterResult.Id;
                //}
                //else
                //{
                //    result.AddError(insertRegisterResult.Errors[0]);
                //}
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }


        public OperationResult<int> LoadZ(v31K1.D.AccountRegisterD registerData, v31K1.D.PersonalRegisterD personData, bool isTestLoad, ProcessingOperations operations, ProcessingOperations processing)
        {
            var result = new OperationResult<int>();
            try
            {
                // IEnumerable<Tuple<FactPatient, FactPerson, FactDocument, IEnumerable<Tuple<FactMedicalEvent, IEnumerable<FactMedicalServices>, IEnumerable<FactMEC>, IEnumerable<FactMEE>, IEnumerable<FactEQMA>>>>> data = null;

                DateTime starttime = DateTime.Now;
                var LastPatientId = 0;

                string moName = string.Empty;
                //Check for already load account
                var existAccountResult = _repository.GetMedicalAccountView(
                    view => view.AccountNumber == registerData.Account.AccountNumber &&
                            view.Date.Value.Year == registerData.Account.Year &&
                            view.Date.Value.Month == registerData.Account.Month &&
                            view.AccountDate == registerData.Account.AccountDate &&
                            view.CodeMo == registerData.Account.MedicalOrganizationCode);

                //Sql.DateDiff(Sql.DateParts.Year, view.Date, registerData.Header.Date) == 0 &&
                //          Sql.DateDiff(Sql.DateParts.Month, view.AccountDate, registerData.Header.Date) == 0 &&
                if (existAccountResult.Success && existAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Счет №{0} МО {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, existAccountResult.Data.First().ShortNameMo, existAccountResult.Data.First().Date));
                    return result;
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    result.AddError(string.Format("В счете №{0} от МО {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, moName));
                    return result;
                }
                dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
                string dest = Convert.ToString(terCode.tf_code);
                var account = new FactMedicalAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = registerData.Account.AccountNumber,
                    AccountDate = registerData.Account.AccountDate,
                    Price = registerData.Account.Price,
                    AcceptPrice = registerData.Account.AcceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    MedicalOrganization = _cache.Get(CacheRepository.F003Cache).GetBack(registerData.Account.MedicalOrganizationCode),
                    Comments = registerData.Account.Comments,
                    Version = dest == "46" ? _cache.Get(CacheRepository.VersionCache).GetBack(registerData.Header.Version) :
                    6
                };

                var total = registerData.RecordsCollection.Count;
                personData.PersonalsCollection = personData.PersonalsCollection
                        .OrderBy(p => p.Surname)
                        .ThenBy(p => p.PName)
                        .ThenBy(p => p.Patronymic).ToList();

                var lastPatientCount = 0;

                //var patients = new List<Tuple<FactPerson, FactDocument, List<Tuple<FactPatient, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, List<ZFactDirection>, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>>, List<Tuple<ZFactMedicalServices, ZFactMedicalServicesOnk>>, List<ZFactSank>>>>>>>>>();
                var patients = new List<Tuple<FactPerson, FactDocument, List<Tuple<FactPatient,
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
                        List<ZFactSank>
                            >>>>>>();

                var pers = new List<Tuple<FactPerson, FactDocument, int, int?, v31K1.D.PersonalD>>();
                foreach (v31K1.D.PersonalD entry in personData.PersonalsCollection)
                {
                    FactPerson personDb = null;
                    FactDocument documentDb = null;

                    int personId = 0;
                    int? documentId = default(int?);

                    var personCopy = Map.ObjectToObject<FactPerson>(entry);

                    var personCheckResult = _repository.GetPerson(p => p.PName == entry.PName &&
                                                                       p.Surname == entry.Surname &&
                                                                       p.Patronymic == entry.Patronymic &&
                                                                       p.Birthday == entry.Birthday &&
                                                                       p.RepresentativeSurname ==
                                                                       entry.RepresentativeSurname &&
                                                                       p.RepresentativeName == entry.RepresentativeName &&
                                                                       p.RepresentativePatronymic ==
                                                                       entry.RepresentativePatronymic &&
                                                                       p.RepresentativeBirthday ==
                                                                       entry.RepresentativeBirthday);

                    if (personCheckResult.Success)
                    {
                        if (!personCheckResult.Data.Any())
                        {
                            personDb = personCopy;
                            if (entry.DocType.HasValue)
                            {
                                documentDb = new FactDocument
                                {
                                    DocType = entry.DocType,
                                    DocNum = entry.DocNumber,
                                    DocSeries = entry.DocSeries,
                                    DocDate = entry.DocDate,
                                    DocOrg = entry.DocOrg,
                                };
                            }
                        }
                        else
                        {
                            personId = personCheckResult.Data.First().PersonId;
                            personDb = personCheckResult.Data.First();
                            var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
                                                                              p.DocType == entry.DocType &&
                                                                              p.DocSeries == entry.DocSeries);

                            if (!docCheckResult.Data.Any())
                            {
                                if (entry.DocType.HasValue)
                                {
                                    documentDb = new FactDocument
                                    {
                                        PersonId = personId,
                                        DocType = entry.DocType,
                                        DocNum = entry.DocNumber,
                                        DocSeries = entry.DocSeries,
                                        DocDate = entry.DocDate,
                                        DocOrg = entry.DocOrg,
                                    };
                                }
                            }
                            else
                            {
                                documentId = docCheckResult.Data.First().DocumentId;
                            }
                        }
                    }
                    else
                    {
                        result.AddError(new Exception("Ошибка поиска персональных данных"));
                        return result;
                    }
                    pers.Add(Tuple.Create(personDb, documentDb, personId, documentId, entry));

                    // var patientCopy = registerData.RecordsCollection.FirstOrDefault(p => p.Patient.PatientGuid == entry.PatientGuid);
                }


                foreach (var recordsCollection in registerData.RecordsCollection)
                {
                    //var patMevents = new List<Tuple<FactPatient, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>>, List<Tuple<ZFactMedicalServices, List<ZFactDirection>, ZFactMedicalServicesOnk>>, List<ZFactSank>>>>>>>();
                    var patMevents = new List<Tuple<FactPatient,
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
                            List<ZFactSank>
                            >>>>();
                    var patientCopy = recordsCollection.Patient;
                    var per = pers.FirstOrDefault(x => x.Item5.PatientGuid == patientCopy.PatientGuid);


                    //if (operations.Has(ProcessingOperations.NotInTerr))
                    //{
                    //    if (patientCopy.Smo.IsNotNull())
                    //    {
                    //        if (patientCopy.Smo.StartsWith(dest))
                    //        {
                    //            continue;
                    //        }
                    //    }
                    //}
                    FactPatient patientDb = Map.ObjectToObject<FactPatient>(patientCopy);

                    if (per.Item1.IsNull())
                    {
                        patientDb.PersonalId = per.Item3;
                    }
                    if (per.Item2.IsNull())
                    {
                        patientDb.DocumentId = per.Item4;
                    }

                    patientDb.MedicalExternalId = recordsCollection.ExternalId;
                    patientDb.Comments = per.Item1?.Comments;

                    if (!patientDb.InsuranceDocType.HasValue)
                    {
                        if (patientCopy.InsuranceDocNumber.IsNotNullOrEmpty())
                        {
                            if (SrzAlgorithms.IsEnpK(patientCopy.InsuranceDocNumber))
                            {
                                patientDb.InsuranceDocType = 3;
                                patientDb.INP = patientCopy.InsuranceDocNumber;
                            }
                            else
                            {
                                patientDb.InsuranceDocType = 1;
                                patientDb.InsuranceDocNumber = patientCopy.InsuranceDocNumber;
                                patientDb.InsuranceDocSeries = patientCopy.InsuranceDocSeries;
                            }
                        }
                    }

                    // var mevents = new List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>>, List<Tuple<ZFactMedicalServices, List<ZFactDirection>, ZFactMedicalServicesOnk>>, List<ZFactSank>>>>>();
                    var mevents = new List<Tuple<ZslFactMedicalEvent,
                        List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            List<ZFactDs>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
                        List<ZFactSank>
                        >>();

                    foreach (v31K1.D.ZslEventD zslEventD in recordsCollection.EventCollection)
                    {
                        var zslevent = Map.ObjectToObject<ZslFactMedicalEvent>(zslEventD);

                        zslevent.AcceptPrice = zslEventD.AcceptPrice ?? zslEventD.Price;//
                        zslevent.MoPrice = zslevent.Price;
                        zslevent.Price = zslevent.Price;

                        zslevent.MoPaymentStatus = GetPaymentStatus(zslevent.AcceptPrice, zslevent.MoPrice);
                        zslevent.PaymentStatus = GetPaymentStatus(zslevent.AcceptPrice, zslevent.Price);

                        if (zslEventD.ParticularCaseCollection != null && zslEventD.ParticularCaseCollection.Count > 0)
                        {
                            zslevent.ParticularCase = zslEventD.ParticularCaseCollection.Aggregate((p, next) => string.Format("{0};{1}", p, next));
                        }

                        //var zevents = new List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>>, List<Tuple<ZFactMedicalServices, List<ZFactDirection>, ZFactMedicalServicesOnk>>, List<ZFactSank>>>();
                        var zevents = new List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            List<ZFactDs>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>();

                        foreach (var eventD in zslEventD.Event)
                        {
                            var eventDCopy = eventD;
                            var zeventD = Map.ObjectToObject<ZFactMedicalEvent>(eventDCopy);
                            zeventD.MoPrice = eventD.EventPrice;

                            var directions = new List<ZFactDirection>();
                            foreach (v31K1.D.DirectionOnkD direc in eventD.DirectionOnkXml)
                            {
                                if (direc == null) continue;
                                var direction = new ZFactDirection
                                {
                                    DirectionDate = direc.DirectionDate,
                                    DirectionMo = direc.DirectionMo,
                                    DirectionViewId = direc.DirectionViewId,
                                    MetIsslId = direc.MetIsslId,
                                    DirectionService = direc.DirectionService
                                };
                                directions.Add(direction);
                            }

                            var сonsultations = new List<ZFactConsultations>();
                            foreach (v31K1.D.ConsultationsOnkD cons in eventD.ConsultationsOnkXml)
                            {
                                if (cons == null) continue;
                                var сonsultation = new ZFactConsultations
                                {
                                    DtCons = cons.DtCons,
                                    PrCons = cons.PrCons
                                };
                                сonsultations.Add(сonsultation);
                            }

                            var ds2 = new List<ZFactDs>();
                            foreach (string ds in eventD.DiagnosisSecondaryXml)
                            {
                                if (ds == null) continue;
                                var factDs = new ZFactDs
                                {
                                    Ds = ds,
                                    DsType = 2
                                };
                                ds2.Add(factDs);
                            }

                            var slKoefs = new List<ZFactSlKoef>();
                            var critList = new List<ZFactCrit>();
                            ZFactKsgKpg ksgKpg = null;
                            if (eventD.KsgKpg != null)
                            {
                                ksgKpg = Map.ObjectToObject<ZFactKsgKpg>(eventD.KsgKpg);
                                foreach (var i in (eventD.KsgKpg.CritXml))
                                {
                                    critList.Add(new ZFactCrit { IdDkk = i });
                                }

                                foreach (var slKoefD in eventD.KsgKpg.InnerSlCoefCollection)
                                {
                                    if (slKoefD == null)
                                        continue;
                                    var slKoef = new ZFactSlKoef
                                    {
                                        NumberDifficultyTreatment = slKoefD.NumberDifficultyTreatment,
                                        ValueDifficultyTreatment = slKoefD.ValueDifficultyTreatment
                                    };
                                    slKoefs.Add(slKoef);
                                }
                            }

                            var medicalEventOnk =
                                new Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>,
                                    List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>(null, null, null, null);
                            if (eventD.EventOnk != null)
                            {
                                var diagBloks = new List<ZFactDiagBlok>();
                                var contraindications = new List<ZFactContraindications>();
                                var serviceUslOnk = new List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>();

                                foreach (var diagBlokOnk in eventD.EventOnk.DiagBlokOnk)
                                {
                                    if (diagBlokOnk == null)
                                        continue;
                                    diagBloks.Add(Map.ObjectToObject<ZFactDiagBlok>(diagBlokOnk));
                                }
                                foreach (var сontraindicationsOnk in eventD.EventOnk.СontraindicationsOnk)
                                {
                                    if (сontraindicationsOnk == null)
                                        continue;
                                    contraindications.Add(Map.ObjectToObject<ZFactContraindications>(сontraindicationsOnk));
                                }
                                foreach (var serviceOnk in eventD.EventOnk.ServiceOnk)
                                {
                                    if (serviceOnk == null)
                                        continue;
                                    var anticancerDrugOnkDs = new List<ZFactAnticancerDrug>();
                                    foreach (var anticancerDrugOnkD in serviceOnk.AnticancerDrugOnkXml)
                                    {
                                        if (anticancerDrugOnkD == null)
                                            continue;
                                        anticancerDrugOnkDs.Add(Map.ObjectToObject<ZFactAnticancerDrug>(anticancerDrugOnkD));

                                    }
                                    serviceUslOnk.Add(Tuple.Create(Map.ObjectToObject<ZFactMedicalServicesOnk>(serviceOnk), anticancerDrugOnkDs));
                                }

                                medicalEventOnk = Tuple.Create(Map.ObjectToObject<ZFactMedicalEventOnk>(eventD.EventOnk), diagBloks, contraindications, serviceUslOnk);
                            }
                            var services = new List<ZFactMedicalServices>();
                            foreach (v31K1.D.ServiceD serviceD in eventD.Service)
                            {
                                var serviceCopy = serviceD;
                                var service = Map.ObjectToObject<ZFactMedicalServices>(serviceCopy);
                                service.ExternalGUID = service.ExternalGUID.IsNullOrWhiteSpace()
                                    ? Guid.NewGuid().ToString()
                                    : service.ExternalGUID;
                                services.Add(service);
                            }
                            zevents.Add(Tuple.Create(zeventD, directions, сonsultations, ds2, medicalEventOnk, Tuple.Create(ksgKpg, critList, slKoefs), services));
                        }

                        var sanks = new List<ZFactSank>();
                        foreach (v31K1.D.RefusalD refusal in zslEventD.Refusals)
                        {
                            if (refusal == null)
                                continue;

                            var sank = new ZFactSank
                            {
                                ReasonId = refusal.RefusalCode,
                                SlidGuid = refusal.SlidGuid,
                                Amount = refusal.RefusalRate,
                                Source = (int)RefusalSource.Local,
                                Comments = refusal.Comments,
                                Type = refusal.RefusalType,
                                ExternalGuid = refusal.ExternalGuid,
                                Date = refusal.Date,
                                EmployeeId = Constants.SystemAccountId
                            };
                            sanks.Add(sank);
                        }

                        mevents.Add(Tuple.Create(zslevent, zevents, sanks));
                    }

                    patMevents.Add(Tuple.Create(patientDb, mevents));

                    patients.Add(Tuple.Create(per.Item1, per.Item2, patMevents));

                    lastPatientCount++;
                    _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                }

                var insertRegisterResult = _repository.InsertRegisterD(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.Errors[0]);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<int> LoadZ(v31K1.DV.AccountRegisterD registerData, v31K1.DV.PersonalRegisterD personData, bool isTestLoad, ProcessingOperations operations, ProcessingOperations processing)
        {
            var result = new OperationResult<int>();
            try
            {
                DateTime starttime = DateTime.Now;
                var LastPatientId = 0;

                string moName = string.Empty;
                //Check for already load account
                var existAccountResult = _repository.GetMedicalAccountView(
                    view => view.AccountNumber == registerData.Account.AccountNumber &&
                            view.Date.Value.Year == registerData.Account.Year &&
                            view.Date.Value.Month == registerData.Account.Month &&
                            view.AccountDate == registerData.Account.AccountDate &&
                            view.CodeMo == registerData.Account.MedicalOrganizationCode);

                if (existAccountResult.Success && existAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Счет №{0} МО {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, existAccountResult.Data.First().ShortNameMo, existAccountResult.Data.First().Date));
                    return result;
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    result.AddError(string.Format("В счете №{0} от МО {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, moName));
                    return result;
                }
                dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
                string dest = Convert.ToString(terCode.tf_code);
                var account = new FactMedicalAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = registerData.Account.AccountNumber,
                    AccountDate = registerData.Account.AccountDate,
                    Price = registerData.Account.Price,
                    AcceptPrice = registerData.Account.AcceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    MedicalOrganization = _cache.Get(CacheRepository.F003Cache).GetBack(registerData.Account.MedicalOrganizationCode),
                    Comments = registerData.Account.Comments,
                    Version = dest == "46" ? _cache.Get(CacheRepository.VersionCache).GetBack(registerData.Header.Version) :
                    6
                };

                var total = registerData.RecordsCollection.Count;
                personData.PersonalsCollection = personData.PersonalsCollection
                        .OrderBy(p => p.Surname)
                        .ThenBy(p => p.PName)
                        .ThenBy(p => p.Patronymic).ToList();

                var lastPatientCount = 0;

                var patients = new List<Tuple<FactPerson, FactDocument, List<Tuple<FactPatient,
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
                        List<ZFactSank>
                            >>>>>>();

                var pers = new List<Tuple<FactPerson, FactDocument, int, int?, v31K1.DV.PersonalD>>();
                foreach (v31K1.DV.PersonalD entry in personData.PersonalsCollection)
                {
                    FactPerson personDb = null;
                    FactDocument documentDb = null;

                    int personId = 0;
                    int? documentId = default(int?);

                    var personCopy = Map.ObjectToObject<FactPerson>(entry);

                    var personCheckResult = _repository.GetPerson(p => p.PName == entry.PName &&
                                                                       p.Surname == entry.Surname &&
                                                                       p.Patronymic == entry.Patronymic &&
                                                                       p.Birthday == entry.Birthday &&
                                                                       p.RepresentativeSurname ==
                                                                       entry.RepresentativeSurname &&
                                                                       p.RepresentativeName == entry.RepresentativeName &&
                                                                       p.RepresentativePatronymic ==
                                                                       entry.RepresentativePatronymic &&
                                                                       p.RepresentativeBirthday ==
                                                                       entry.RepresentativeBirthday);

                    if (personCheckResult.Success)
                    {
                        if (!personCheckResult.Data.Any())
                        {
                            personDb = personCopy;
                            if (entry.DocType.HasValue && entry.DocType != 0)
                            {
                                documentDb = new FactDocument
                                {
                                    DocType = entry.DocType,
                                    DocNum = entry.DocNumber,
                                    DocSeries = entry.DocSeries,
                                    DocDate = entry.DocDate,
                                    DocOrg = entry.DocOrg,
                                };
                            }
                        }
                        else
                        {
                            personId = personCheckResult.Data.First().PersonId;
                            personDb = personCheckResult.Data.First();
                            var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
                                                                              p.DocType == entry.DocType &&
                                                                              p.DocSeries == entry.DocSeries);

                            if (!docCheckResult.Data.Any())
                            {
                                if (entry.DocType.HasValue && entry.DocType != 0)
                                {
                                    documentDb = new FactDocument
                                    {
                                        PersonId = personId,
                                        DocType = entry.DocType,
                                        DocNum = entry.DocNumber,
                                        DocSeries = entry.DocSeries,
                                        DocDate = entry.DocDate,
                                        DocOrg = entry.DocOrg,
                                    };
                                }
                            }
                            else
                            {
                                documentId = docCheckResult.Data.First().DocumentId;
                            }
                        }
                    }
                    else
                    {
                        result.AddError(new Exception("Ошибка поиска персональных данных"));
                        return result;
                    }
                    pers.Add(Tuple.Create(personDb, documentDb, personId, documentId, entry));
                }

                foreach (var recordsCollection in registerData.RecordsCollection)
                {
                    var patMevents = new List<Tuple<FactPatient,
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
                            List<ZFactSank>
                            >>>>();
                    var patientCopy = recordsCollection.Patient;
                    var per = pers.FirstOrDefault(x => x.Item5.PatientGuid == patientCopy.PatientGuid);
                    
                    FactPatient patientDb = Map.ObjectToObject<FactPatient>(patientCopy);
                    if (per.Item1.IsNull())
                    {
                        patientDb.PersonalId = per.Item3;
                    }
                    if (per.Item2.IsNull())
                    {
                        patientDb.DocumentId = per.Item4;
                    }

                    patientDb.MedicalExternalId = recordsCollection.ExternalId;
                    patientDb.Comments = per.Item1?.Comments;

                    if (!patientDb.InsuranceDocType.HasValue)
                    {
                        if (patientCopy.InsuranceDocNumber.IsNotNullOrEmpty())
                        {
                            if (SrzAlgorithms.IsEnpK(patientCopy.InsuranceDocNumber))
                            {
                                patientDb.InsuranceDocType = 3;
                                patientDb.INP = patientCopy.InsuranceDocNumber;
                            }
                            else
                            {
                                patientDb.InsuranceDocType = 1;
                                patientDb.InsuranceDocNumber = patientCopy.InsuranceDocNumber;
                                patientDb.InsuranceDocSeries = patientCopy.InsuranceDocSeries;
                            }
                        }
                    }

                    var mevents = new List<Tuple<ZslFactMedicalEvent,
                        List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            List<ZFactDs>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
                        List<ZFactSank>
                        >>();

                    foreach (v31K1.DV.ZslEventD zslEventD in recordsCollection.EventCollection)
                    {
                        var zslevent = Map.ObjectToObject<ZslFactMedicalEvent>(zslEventD);

                        zslevent.AcceptPrice = zslEventD.AcceptPrice ?? zslEventD.Price;//
                        zslevent.MoPrice = zslevent.Price;
                        zslevent.Price = zslevent.Price;

                        zslevent.MoPaymentStatus = GetPaymentStatus(zslevent.AcceptPrice, zslevent.MoPrice);
                        zslevent.PaymentStatus = GetPaymentStatus(zslevent.AcceptPrice, zslevent.Price);

                        if (zslEventD.ParticularCaseCollection != null && zslEventD.ParticularCaseCollection.Count > 0)
                        {
                            zslevent.ParticularCase = zslEventD.ParticularCaseCollection.Aggregate((p, next) => string.Format("{0};{1}", p, next));
                        }

                        var zevents = new List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            List<ZFactDs>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>();

                        foreach (var eventD in zslEventD.Event)
                        {
                            var eventDCopy = eventD;
                            var zeventD = Map.ObjectToObject<ZFactMedicalEvent>(eventDCopy);
                            zeventD.MoPrice = eventD.EventPrice;

                            var directions = new List<ZFactDirection>();
                            foreach (v31K1.DV.DirectionOnkD direc in eventD.DirectionOnkXml)
                            {
                                if (direc == null) continue;
                                var direction = new ZFactDirection
                                {
                                    DirectionDate = direc.DirectionDate,
                                    DirectionMo = direc.DirectionMo,
                                    DirectionViewId = direc.DirectionViewId,
                                    MetIsslId = direc.MetIsslId,
                                    DirectionService = direc.DirectionService
                                };
                                directions.Add(direction);
                            }

                            var сonsultations = new List<ZFactConsultations>();
                            foreach (v31K1.DV.ConsultationsOnkD cons in eventD.ConsultationsOnkXml)
                            {
                                if (cons == null) continue;
                                var сonsultation = new ZFactConsultations
                                {
                                    DtCons = cons.DtCons,
                                    PrCons = cons.PrCons
                                };
                                сonsultations.Add(сonsultation);
                            }
                            var ds2 = new List<ZFactDs>();
                            foreach (v31K1.DV.DsD ds in eventD.DiagnosisSecondaryXml)
                            {
                                if (ds == null) continue;
                                var factDs = new ZFactDs
                                {
                                    Ds = ds.Ds,
                                    DsType = ds.DsType,
                                    PrDs2N = ds.PrDs2N,
                                    DsPr = ds.DsPr
                                };
                                ds2.Add(factDs);
                            }

                            var slKoefs = new List<ZFactSlKoef>();
                            var critList = new List<ZFactCrit>();
                            ZFactKsgKpg ksgKpg = null;
                            if (eventD.KsgKpg != null)
                            {
                                ksgKpg = Map.ObjectToObject<ZFactKsgKpg>(eventD.KsgKpg);
                                foreach (var i in (eventD.KsgKpg.CritXml))
                                {
                                    critList.Add(new ZFactCrit { IdDkk = i });
                                }

                                foreach (var slKoefD in eventD.KsgKpg.InnerSlCoefCollection)
                                {
                                    if (slKoefD == null)
                                        continue;
                                    var slKoef = new ZFactSlKoef
                                    {
                                        NumberDifficultyTreatment = slKoefD.NumberDifficultyTreatment,
                                        ValueDifficultyTreatment = slKoefD.ValueDifficultyTreatment
                                    };
                                    slKoefs.Add(slKoef);
                                }
                            }

                            var medicalEventOnk =
                                new Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>,
                                    List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>(null, null, null, null);
                            if (eventD.EventOnk != null)
                            {
                                var diagBloks = new List<ZFactDiagBlok>();
                                var contraindications = new List<ZFactContraindications>();
                                var serviceUslOnk = new List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>();

                                foreach (var diagBlokOnk in eventD.EventOnk.DiagBlokOnk)
                                {
                                    if (diagBlokOnk == null)
                                        continue;
                                    diagBloks.Add(Map.ObjectToObject<ZFactDiagBlok>(diagBlokOnk));
                                }
                                foreach (var сontraindicationsOnk in eventD.EventOnk.СontraindicationsOnk)
                                {
                                    if (сontraindicationsOnk == null)
                                        continue;
                                    contraindications.Add(Map.ObjectToObject<ZFactContraindications>(сontraindicationsOnk));
                                }
                                foreach (var serviceOnk in eventD.EventOnk.ServiceOnk)
                                {
                                    if (serviceOnk == null)
                                        continue;
                                    var anticancerDrugOnkDs = new List<ZFactAnticancerDrug>();
                                    foreach (var anticancerDrugOnkD in serviceOnk.AnticancerDrugOnkXml)
                                    {
                                        if (anticancerDrugOnkD == null)
                                            continue;
                                        anticancerDrugOnkDs.Add(Map.ObjectToObject<ZFactAnticancerDrug>(anticancerDrugOnkD));

                                    }
                                    serviceUslOnk.Add(Tuple.Create(Map.ObjectToObject<ZFactMedicalServicesOnk>(serviceOnk), anticancerDrugOnkDs));
                                }

                                medicalEventOnk = Tuple.Create(Map.ObjectToObject<ZFactMedicalEventOnk>(eventD.EventOnk), diagBloks, contraindications, serviceUslOnk);
                            }
                            var services = new List<ZFactMedicalServices>();
                            foreach (v31K1.DV.ServiceD serviceD in eventD.Service)
                            {
                                var serviceCopy = serviceD;
                                var service = Map.ObjectToObject<ZFactMedicalServices>(serviceCopy);
                                service.ExternalGUID = service.ExternalGUID.IsNullOrWhiteSpace()
                                    ? Guid.NewGuid().ToString()
                                    : service.ExternalGUID;
                                services.Add(service);
                            }
                            zevents.Add(Tuple.Create(zeventD, directions, сonsultations, ds2, medicalEventOnk, Tuple.Create(ksgKpg, critList, slKoefs), services));
                        }

                        var sanks = new List<ZFactSank>();
                        foreach (v31K1.DV.RefusalD refusal in zslEventD.Refusals)
                        {
                            if (refusal == null)
                                continue;

                            var sank = new ZFactSank
                            {
                                ReasonId = refusal.RefusalCode,
                                SlidGuid = refusal.SlidGuid,
                                Amount = refusal.RefusalRate,
                                Source = (int)RefusalSource.Local,
                                Comments = refusal.Comments,
                                Type = refusal.RefusalType,
                                ExternalGuid = refusal.ExternalGuid,
                                Date = refusal.Date,
                                EmployeeId = Constants.SystemAccountId
                            };
                            sanks.Add(sank);
                        }

                        mevents.Add(Tuple.Create(zslevent, zevents, sanks));
                    }

                    patMevents.Add(Tuple.Create(patientDb, mevents));

                    patients.Add(Tuple.Create(per.Item1, per.Item2, patMevents));

                    lastPatientCount++;
                    _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                }

                var insertRegisterResult = _repository.InsertRegisterD(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.Errors[0]);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<int> LoadZ(v32K1.D.AccountRegisterD registerData, v32K1.D.PersonalRegisterD personData, bool isTestLoad, ProcessingOperations operations, ProcessingOperations processing)
        {
            var result = new OperationResult<int>();
            try
            {
                // IEnumerable<Tuple<FactPatient, FactPerson, FactDocument, IEnumerable<Tuple<FactMedicalEvent, IEnumerable<FactMedicalServices>, IEnumerable<FactMEC>, IEnumerable<FactMEE>, IEnumerable<FactEQMA>>>>> data = null;

                DateTime starttime = DateTime.Now;
                var LastPatientId = 0;

                string moName = string.Empty;
                //Check for already load account
                var existAccountResult = _repository.GetMedicalAccountView(
                    view => view.AccountNumber == registerData.Account.AccountNumber &&
                            view.Date.Value.Year == registerData.Account.Year &&
                            view.Date.Value.Month == registerData.Account.Month &&
                            view.AccountDate == registerData.Account.AccountDate &&
                            view.CodeMo == registerData.Account.MedicalOrganizationCode);

                //Sql.DateDiff(Sql.DateParts.Year, view.Date, registerData.Header.Date) == 0 &&
                //          Sql.DateDiff(Sql.DateParts.Month, view.AccountDate, registerData.Header.Date) == 0 &&
                if (existAccountResult.Success && existAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Счет №{0} МО {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, existAccountResult.Data.First().ShortNameMo, existAccountResult.Data.First().Date));
                    return result;
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    result.AddError(string.Format("В счете №{0} от МО {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, moName));
                    return result;
                }
                dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
                string dest = Convert.ToString(terCode.tf_code);
                var account = new FactMedicalAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = registerData.Account.AccountNumber,
                    AccountDate = registerData.Account.AccountDate,
                    Price = registerData.Account.Price,
                    AcceptPrice = registerData.Account.AcceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    MedicalOrganization = _cache.Get(CacheRepository.F003Cache).GetBack(registerData.Account.MedicalOrganizationCode),
                    Comments = registerData.Account.Comments,
                    Version = dest == "46" ? _cache.Get(CacheRepository.VersionCache).GetBack(registerData.Header.Version) :
                    8
                };

                var total = registerData.RecordsCollection.Count;
                personData.PersonalsCollection = personData.PersonalsCollection
                        .OrderBy(p => p.Surname)
                        .ThenBy(p => p.PName)
                        .ThenBy(p => p.Patronymic).ToList();

                var lastPatientCount = 0;

                //var patients = new List<Tuple<FactPerson, FactDocument, List<Tuple<FactPatient, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, List<ZFactDirection>, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>>, List<Tuple<ZFactMedicalServices, ZFactMedicalServicesOnk>>, List<ZFactSank>>>>>>>>>();
                var patients = new List<Tuple<FactPerson, FactDocument, List<Tuple<FactPatient,
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
                        List<ZFactSank>
                            >>>>>>();

                var pers = new List<Tuple<FactPerson, FactDocument, int, int?, v32K1.D.PersonalD>>();
                foreach (v32K1.D.PersonalD entry in personData.PersonalsCollection)
                {
                    FactPerson personDb = null;
                    FactDocument documentDb = null;

                    int personId = 0;
                    int? documentId = default(int?);

                    var personCopy = Map.ObjectToObject<FactPerson>(entry);

                    var personCheckResult = _repository.GetPerson(p => p.PName == entry.PName &&
                                                                       p.Surname == entry.Surname &&
                                                                       p.Patronymic == entry.Patronymic &&
                                                                       p.Birthday == entry.Birthday &&
                                                                       p.RepresentativeSurname ==
                                                                       entry.RepresentativeSurname &&
                                                                       p.RepresentativeName == entry.RepresentativeName &&
                                                                       p.RepresentativePatronymic ==
                                                                       entry.RepresentativePatronymic &&
                                                                       p.RepresentativeBirthday ==
                                                                       entry.RepresentativeBirthday);

                    if (personCheckResult.Success)
                    {
                        if (!personCheckResult.Data.Any())
                        {
                            personDb = personCopy;
                            if (entry.DocType.HasValue)
                            {
                                documentDb = new FactDocument
                                {
                                    DocType = entry.DocType,
                                    DocNum = entry.DocNumber,
                                    DocSeries = entry.DocSeries,
                                    DocDate = entry.DocDate,
                                    DocOrg = entry.DocOrg,
                                };
                            }
                        }
                        else
                        {
                            personId = personCheckResult.Data.First().PersonId;
                            personDb = personCheckResult.Data.First();
                            var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
                                                                              p.DocType == entry.DocType &&
                                                                              p.DocSeries == entry.DocSeries);

                            if (!docCheckResult.Data.Any())
                            {
                                if (entry.DocType.HasValue)
                                {
                                    documentDb = new FactDocument
                                    {
                                        PersonId = personId,
                                        DocType = entry.DocType,
                                        DocNum = entry.DocNumber,
                                        DocSeries = entry.DocSeries,
                                        DocDate = entry.DocDate,
                                        DocOrg = entry.DocOrg,
                                    };
                                }
                            }
                            else
                            {
                                documentId = docCheckResult.Data.First().DocumentId;
                            }
                        }
                    }
                    else
                    {
                        result.AddError(new Exception("Ошибка поиска персональных данных"));
                        return result;
                    }
                    pers.Add(Tuple.Create(personDb, documentDb, personId, documentId, entry));

                    // var patientCopy = registerData.RecordsCollection.FirstOrDefault(p => p.Patient.PatientGuid == entry.PatientGuid);
                }


                foreach (var recordsCollection in registerData.RecordsCollection)
                {
                    //var patMevents = new List<Tuple<FactPatient, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>>, List<Tuple<ZFactMedicalServices, List<ZFactDirection>, ZFactMedicalServicesOnk>>, List<ZFactSank>>>>>>>();
                    var patMevents = new List<Tuple<FactPatient,
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
                            List<ZFactSank>
                            >>>>();
                    var patientCopy = recordsCollection.Patient;
                    var per = pers.FirstOrDefault(x => x.Item5.PatientGuid == patientCopy.PatientGuid);


                    //if (operations.Has(ProcessingOperations.NotInTerr))
                    //{
                    //    if (patientCopy.Smo.IsNotNull())
                    //    {
                    //        if (patientCopy.Smo.StartsWith(dest))
                    //        {
                    //            continue;
                    //        }
                    //    }
                    //}
                    FactPatient patientDb = Map.ObjectToObject<FactPatient>(patientCopy);

                    if (per.Item1.IsNull())
                    {
                        patientDb.PersonalId = per.Item3;
                    }
                    if (per.Item2.IsNull())
                    {
                        patientDb.DocumentId = per.Item4;
                    }

                    patientDb.MedicalExternalId = recordsCollection.ExternalId;
                    patientDb.Comments = per.Item1?.Comments;

                    if (!patientDb.InsuranceDocType.HasValue)
                    {
                        if (patientCopy.InsuranceDocNumber.IsNotNullOrEmpty())
                        {
                            if (SrzAlgorithms.IsEnpK(patientCopy.InsuranceDocNumber))
                            {
                                patientDb.InsuranceDocType = 3;
                                patientDb.INP = patientCopy.InsuranceDocNumber;
                            }
                            else
                            {
                                patientDb.InsuranceDocType = 1;
                                patientDb.InsuranceDocNumber = patientCopy.InsuranceDocNumber;
                                patientDb.InsuranceDocSeries = patientCopy.InsuranceDocSeries;
                            }
                        }
                    }

                    // var mevents = new List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>>, List<Tuple<ZFactMedicalServices, List<ZFactDirection>, ZFactMedicalServicesOnk>>, List<ZFactSank>>>>>();
                    var mevents = new List<Tuple<ZslFactMedicalEvent,
                        List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            List<ZFactDs>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
                        List<ZFactSank>
                        >>();

                    foreach (v32K1.D.ZslEventD zslEventD in recordsCollection.EventCollection)
                    {
                        var zslevent = Map.ObjectToObject<ZslFactMedicalEvent>(zslEventD);

                        zslevent.AcceptPrice = zslEventD.AcceptPrice ?? zslEventD.Price;//
                        zslevent.MoPrice = zslevent.Price;
                        zslevent.Price = zslevent.Price;

                        zslevent.MoPaymentStatus = GetPaymentStatus(zslevent.AcceptPrice, zslevent.MoPrice);
                        zslevent.PaymentStatus = GetPaymentStatus(zslevent.AcceptPrice, zslevent.Price);

                        if (zslEventD.ParticularCaseCollection != null && zslEventD.ParticularCaseCollection.Count > 0)
                        {
                            zslevent.ParticularCase = zslEventD.ParticularCaseCollection.Aggregate((p, next) => string.Format("{0};{1}", p, next));
                        }

                        //var zevents = new List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>>, List<Tuple<ZFactMedicalServices, List<ZFactDirection>, ZFactMedicalServicesOnk>>, List<ZFactSank>>>();
                        var zevents = new List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            List<ZFactDs>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>();

                        foreach (var eventD in zslEventD.Event)
                        {
                            var eventDCopy = eventD;
                            var zeventD = Map.ObjectToObject<ZFactMedicalEvent>(eventDCopy);
                            zeventD.MoPrice = eventD.EventPrice;

                            var directions = new List<ZFactDirection>();
                            foreach (v32K1.D.DirectionOnkD direc in eventD.DirectionOnkXml)
                            {
                                if (direc == null) continue;
                                var direction = new ZFactDirection
                                {
                                    DirectionDate = direc.DirectionDate,
                                    DirectionMo = direc.DirectionMo,
                                    DirectionViewId = direc.DirectionViewId,
                                    MetIsslId = direc.MetIsslId,
                                    DirectionService = direc.DirectionService
                                };
                                directions.Add(direction);
                            }

                            var сonsultations = new List<ZFactConsultations>();
                            foreach (v32K1.D.ConsultationsOnkD cons in eventD.ConsultationsOnkXml)
                            {
                                if (cons == null) continue;
                                var сonsultation = new ZFactConsultations
                                {
                                    DtCons = cons.DtCons,
                                    PrCons = cons.PrCons
                                };
                                сonsultations.Add(сonsultation);
                            }
                            var ds2 = new List<ZFactDs>();
                            foreach (string ds in eventD.DiagnosisSecondaryXml)
                            {
                                if (ds == null) continue;
                                var factDs = new ZFactDs
                                {
                                    Ds = ds,
                                    DsType = 2
                                };
                                ds2.Add(factDs);
                            }

                            var slKoefs = new List<ZFactSlKoef>();
                            var critList = new List<ZFactCrit>();
                            ZFactKsgKpg ksgKpg = null;
                            if (eventD.KsgKpg != null)
                            {
                                ksgKpg = Map.ObjectToObject<ZFactKsgKpg>(eventD.KsgKpg);
                                foreach (var i in (eventD.KsgKpg.CritXml))
                                {
                                    critList.Add(new ZFactCrit { IdDkk = i });
                                }

                                foreach (var slKoefD in eventD.KsgKpg.InnerSlCoefCollection)
                                {
                                    if (slKoefD == null)
                                        continue;
                                    var slKoef = new ZFactSlKoef
                                    {
                                        NumberDifficultyTreatment = slKoefD.NumberDifficultyTreatment,
                                        ValueDifficultyTreatment = slKoefD.ValueDifficultyTreatment
                                    };
                                    slKoefs.Add(slKoef);
                                }
                            }

                            var medicalEventOnk =
                                new Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>,
                                    List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>(null, null, null, null);
                            if (eventD.EventOnk != null)
                            {
                                var diagBloks = new List<ZFactDiagBlok>();
                                var contraindications = new List<ZFactContraindications>();
                                var serviceUslOnk = new List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>();

                                foreach (var diagBlokOnk in eventD.EventOnk.DiagBlokOnk)
                                {
                                    if (diagBlokOnk == null)
                                        continue;
                                    diagBloks.Add(Map.ObjectToObject<ZFactDiagBlok>(diagBlokOnk));
                                }
                                foreach (var сontraindicationsOnk in eventD.EventOnk.СontraindicationsOnk)
                                {
                                    if (сontraindicationsOnk == null)
                                        continue;
                                    contraindications.Add(Map.ObjectToObject<ZFactContraindications>(сontraindicationsOnk));
                                }
                                foreach (var serviceOnk in eventD.EventOnk.ServiceOnk)
                                {
                                    if (serviceOnk == null)
                                        continue;
                                    var anticancerDrugOnkDs = new List<ZFactAnticancerDrug>();
                                    foreach (var anticancerDrugOnkD in serviceOnk.AnticancerDrugOnkXml)
                                    {
                                        if (anticancerDrugOnkD == null)
                                            continue;
                                        anticancerDrugOnkDs.Add(Map.ObjectToObject<ZFactAnticancerDrug>(anticancerDrugOnkD));

                                    }
                                    serviceUslOnk.Add(Tuple.Create(Map.ObjectToObject<ZFactMedicalServicesOnk>(serviceOnk), anticancerDrugOnkDs));
                                }

                                medicalEventOnk = Tuple.Create(Map.ObjectToObject<ZFactMedicalEventOnk>(eventD.EventOnk), diagBloks, contraindications, serviceUslOnk);
                            }
                            var services = new List<ZFactMedicalServices>();
                            foreach (v32K1.D.ServiceD serviceD in eventD.Service)
                            {
                                var serviceCopy = serviceD;
                                var service = Map.ObjectToObject<ZFactMedicalServices>(serviceCopy);
                                service.ExternalGUID = service.ExternalGUID.IsNullOrWhiteSpace()
                                    ? Guid.NewGuid().ToString()
                                    : service.ExternalGUID;
                                services.Add(service);
                            }
                            zevents.Add(Tuple.Create(zeventD, directions, сonsultations, ds2, medicalEventOnk, Tuple.Create(ksgKpg, critList, slKoefs), services));
                        }

                        var sanks = new List<ZFactSank>();
                        foreach (v32K1.D.RefusalD refusal in zslEventD.Refusals)
                        {
                            if (refusal == null)
                                continue;

                            var sank = new ZFactSank
                            {
                                ReasonId = refusal.RefusalCode,
                                SlidGuid = refusal.SlidGuid,
                                Amount = refusal.RefusalRate,
                                Source = (int)RefusalSource.Local,
                                Comments = refusal.Comments,
                                Type = refusal.RefusalType,
                                ExternalGuid = refusal.ExternalGuid,
                                Date = refusal.Date,
                                EmployeeId = Constants.SystemAccountId
                            };
                            sanks.Add(sank);
                        }

                        mevents.Add(Tuple.Create(zslevent, zevents, sanks));
                    }

                    patMevents.Add(Tuple.Create(patientDb, mevents));

                    patients.Add(Tuple.Create(per.Item1, per.Item2, patMevents));

                    lastPatientCount++;
                    _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                }

                var insertRegisterResult = _repository.InsertRegisterD(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.Errors[0]);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }


        //public OperationResult<int> LoadZ(IAccountRegister registerData, IPersonalRegisterD personData, bool isTestLoad, ProcessingOperations operations, ProcessingOperations processing)
        //{
        //    var result = new OperationResult<int>();
        //    try
        //    {
        //        DateTime starttime = DateTime.Now;
        //        var LastPatientId = 0;

        //        string moName = string.Empty;
        //        //Check for already load account
        //        var existAccountResult = _repository.GetMedicalAccountView(
        //            view => view.AccountNumber == registerData.InnerAccount.AccountNumber &&
        //                    view.Date.Value.Year == registerData.InnerAccount.Year &&
        //                    view.Date.Value.Month == registerData.InnerAccount.Month &&
        //                    view.AccountDate == registerData.InnerAccount.AccountDate &&
        //                    view.CodeMo == registerData.InnerAccount.MedicalOrganizationCode);

        //        //Sql.DateDiff(Sql.DateParts.Year, view.Date, registerData.Header.Date) == 0 &&
        //        //          Sql.DateDiff(Sql.DateParts.Month, view.AccountDate, registerData.Header.Date) == 0 &&
        //        if (existAccountResult.Success && existAccountResult.Data.Any())
        //        {
        //            result.AddError(string.Format("Счет №{0} МО {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
        //                                        registerData.InnerAccount.AccountNumber, existAccountResult.Data.First().ShortNameMo, existAccountResult.Data.First().Date));
        //            return result;
        //        }

        //        if (!registerData.InnerAccount.Year.HasValue || !registerData.InnerAccount.Month.HasValue)
        //        {
        //            result.AddError(string.Format("В счете №{0} от МО {1} отсутствует отчетная дата (месяц, год).",
        //                                            registerData.InnerAccount.AccountNumber, moName));
        //            return result;
        //        }
        //        dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
        //        string dest = Convert.ToString(terCode.tf_code);
        //        var account = new FactMedicalAccount
        //        {
        //            Date = new DateTime(registerData.InnerAccount.Year.Value, registerData.InnerAccount.Month.Value, 1),
        //            AccountNumber = registerData.InnerAccount.AccountNumber,
        //            AccountDate = registerData.InnerAccount.AccountDate,
        //            Price = registerData.InnerAccount.Price,
        //            AcceptPrice = registerData.InnerAccount.AcceptPrice,
        //            MECPenalties = registerData.InnerAccount.MECPenalties,
        //            MEEPenalties = registerData.InnerAccount.MEEPenalties,
        //            EQMAPenalties = registerData.InnerAccount.EQMAPenalties,
        //            Status = AccountStatus.NotProcessed.ToInt32(),
        //            MedicalOrganization = _cache.Get(CacheRepository.F003Cache).GetBack(registerData.InnerAccount.MedicalOrganizationCode),
        //            Comments = registerData.InnerAccount.Comments,
        //            Version = dest == "46" ? _cache.Get(CacheRepository.VersionCache).GetBack(registerData.Header.Version) :
        //            8
        //        };

        //        var total = registerData.RecordsCollection.Count;
        //        personData.PersonalsCollection = personData.PersonalsCollection
        //                .OrderBy(p => p.Surname)
        //                .ThenBy(p => p.PName)
        //                .ThenBy(p => p.Patronymic).ToList();

        //        var lastPatientCount = 0;

        //        //var patients = new List<Tuple<FactPerson, FactDocument, List<Tuple<FactPatient, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, List<ZFactDirection>, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>>, List<Tuple<ZFactMedicalServices, ZFactMedicalServicesOnk>>, List<ZFactSank>>>>>>>>>();
        //        var patients = new List<Tuple<FactPerson, FactDocument, List<Tuple<FactPatient,
        //            List<Tuple<ZslFactMedicalEvent,
        //                List<Tuple<ZFactMedicalEvent,
        //                    List<ZFactDirection>,
        //                    List<ZFactConsultations>,
        //                    Tuple<ZFactMedicalEventOnk,
        //                        List<ZFactDiagBlok>,
        //                        List<ZFactContraindications>,
        //                        List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
        //                    Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
        //                List<ZFactSank>
        //                    >>>>>>();

        //        var pers = new List<Tuple<FactPerson, FactDocument, int, int?, v32K1.D.PersonalD>>();
        //        foreach (v32K1.D.PersonalD entry in personData.PersonalsCollection)
        //        {
        //            FactPerson personDb = null;
        //            FactDocument documentDb = null;

        //            int personId = 0;
        //            int? documentId = default(int?);

        //            var personCopy = Map.ObjectToObject<FactPerson>(entry);

        //            var personCheckResult = _repository.GetPerson(p => p.PName == entry.PName &&
        //                                                               p.Surname == entry.Surname &&
        //                                                               p.Patronymic == entry.Patronymic &&
        //                                                               p.Birthday == entry.Birthday &&
        //                                                               p.RepresentativeSurname ==
        //                                                               entry.RepresentativeSurname &&
        //                                                               p.RepresentativeName == entry.RepresentativeName &&
        //                                                               p.RepresentativePatronymic ==
        //                                                               entry.RepresentativePatronymic &&
        //                                                               p.RepresentativeBirthday ==
        //                                                               entry.RepresentativeBirthday);

        //            if (personCheckResult.Success)
        //            {
        //                if (!personCheckResult.Data.Any())
        //                {
        //                    personDb = personCopy;
        //                    if (entry.DocType.HasValue)
        //                    {
        //                        documentDb = new FactDocument
        //                        {
        //                            DocType = entry.DocType,
        //                            DocNum = entry.DocNumber,
        //                            DocSeries = entry.DocSeries
        //                        };
        //                    }
        //                }
        //                else
        //                {
        //                    personId = personCheckResult.Data.First().PersonId;
        //                    personDb = personCheckResult.Data.First();
        //                    var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
        //                                                                      p.DocType == entry.DocType &&
        //                                                                      p.DocSeries == entry.DocSeries);

        //                    if (!docCheckResult.Data.Any())
        //                    {
        //                        if (entry.DocType.HasValue)
        //                        {
        //                            documentDb = new FactDocument
        //                            {
        //                                PersonId = personId,
        //                                DocType = entry.DocType,
        //                                DocNum = entry.DocNumber,
        //                                DocSeries = entry.DocSeries,
        //                                DocDate = entry.DocDate,
        //                                DocOrg = entry.DocOrg,
        //                            };
        //                        }
        //                    }
        //                    else
        //                    {
        //                        documentId = docCheckResult.Data.First().DocumentId;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                result.AddError(new Exception("Ошибка поиска персональных данных"));
        //                return result;
        //            }
        //            pers.Add(Tuple.Create(personDb, documentDb, personId, documentId, entry));

        //            // var patientCopy = registerData.RecordsCollection.FirstOrDefault(p => p.Patient.PatientGuid == entry.PatientGuid);
        //        }


        //        foreach (var recordsCollection in registerData.RecordsCollection)
        //        {
        //            //var patMevents = new List<Tuple<FactPatient, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>>, List<Tuple<ZFactMedicalServices, List<ZFactDirection>, ZFactMedicalServicesOnk>>, List<ZFactSank>>>>>>>();
        //            var patMevents = new List<Tuple<FactPatient,
        //                List<Tuple<ZslFactMedicalEvent,
        //                    List<Tuple<ZFactMedicalEvent,
        //                        List<ZFactDirection>,
        //                        List<ZFactConsultations>,
        //                        Tuple<ZFactMedicalEventOnk,
        //                            List<ZFactDiagBlok>,
        //                            List<ZFactContraindications>,
        //                            List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
        //                        Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
        //                    List<ZFactSank>
        //                    >>>>();
        //            var patientCopy = recordsCollection.Patient;
        //            var per = pers.FirstOrDefault(x => x.Item5.PatientGuid == patientCopy.PatientGuid);


        //            //if (operations.Has(ProcessingOperations.NotInTerr))
        //            //{
        //            //    if (patientCopy.Smo.IsNotNull())
        //            //    {
        //            //        if (patientCopy.Smo.StartsWith(dest))
        //            //        {
        //            //            continue;
        //            //        }
        //            //    }
        //            //}
        //            FactPatient patientDb = Map.ObjectToObject<FactPatient>(patientCopy);

        //            if (per.Item1.IsNull())
        //            {
        //                patientDb.PersonalId = per.Item3;
        //            }
        //            if (per.Item2.IsNull())
        //            {
        //                patientDb.DocumentId = per.Item4;
        //            }

        //            patientDb.MedicalExternalId = recordsCollection.ExternalId;
        //            patientDb.Comments = per.Item1?.Comments;

        //            if (!patientDb.InsuranceDocType.HasValue)
        //            {
        //                if (patientCopy.InsuranceDocNumber.IsNotNullOrEmpty())
        //                {
        //                    if (SrzAlgorithms.IsEnpK(patientCopy.InsuranceDocNumber))
        //                    {
        //                        patientDb.InsuranceDocType = 3;
        //                        patientDb.INP = patientCopy.InsuranceDocNumber;
        //                    }
        //                    else
        //                    {
        //                        patientDb.InsuranceDocType = 1;
        //                        patientDb.InsuranceDocNumber = patientCopy.InsuranceDocNumber;
        //                        patientDb.InsuranceDocSeries = patientCopy.InsuranceDocSeries;
        //                    }
        //                }
        //            }

        //            // var mevents = new List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>>, List<Tuple<ZFactMedicalServices, List<ZFactDirection>, ZFactMedicalServicesOnk>>, List<ZFactSank>>>>>();
        //            var mevents = new List<Tuple<ZslFactMedicalEvent,
        //                List<Tuple<ZFactMedicalEvent,
        //                    List<ZFactDirection>,
        //                    List<ZFactConsultations>,
        //                    Tuple<ZFactMedicalEventOnk,
        //                        List<ZFactDiagBlok>,
        //                        List<ZFactContraindications>,
        //                        List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
        //                    Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
        //                List<ZFactSank>
        //                >>();

        //            foreach (v32K1.D.ZslEventD zslEventD in recordsCollection.EventCollection)
        //            {
        //                var zslevent = Map.ObjectToObject<ZslFactMedicalEvent>(zslEventD);

        //                zslevent.AcceptPrice = zslEventD.AcceptPrice ?? zslEventD.Price;//
        //                zslevent.MoPrice = zslevent.Price;
        //                zslevent.Price = zslevent.Price;

        //                zslevent.MoPaymentStatus = GetPaymentStatus(zslevent.AcceptPrice, zslevent.MoPrice);
        //                zslevent.PaymentStatus = GetPaymentStatus(zslevent.AcceptPrice, zslevent.Price);

        //                if (zslEventD.ParticularCaseCollection != null && zslEventD.ParticularCaseCollection.Count > 0)
        //                {
        //                    zslevent.ParticularCase = zslEventD.ParticularCaseCollection.Aggregate((p, next) => string.Format("{0};{1}", p, next));
        //                }

        //                //var zevents = new List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>>, List<Tuple<ZFactMedicalServices, List<ZFactDirection>, ZFactMedicalServicesOnk>>, List<ZFactSank>>>();
        //                var zevents = new List<Tuple<ZFactMedicalEvent,
        //                    List<ZFactDirection>,
        //                    List<ZFactConsultations>,
        //                    Tuple<ZFactMedicalEventOnk,
        //                        List<ZFactDiagBlok>,
        //                        List<ZFactContraindications>,
        //                        List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
        //                    Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>();

        //                foreach (var eventD in zslEventD.Event)
        //                {
        //                    var eventDCopy = eventD;
        //                    var zeventD = Map.ObjectToObject<ZFactMedicalEvent>(eventDCopy);
        //                    zeventD.MoPrice = eventD.EventPrice;

        //                    var directions = new List<ZFactDirection>();
        //                    foreach (v32K1.D.DirectionOnkD direc in eventD.DirectionOnkXml)
        //                    {
        //                        if (direc == null) continue;
        //                        var direction = new ZFactDirection
        //                        {
        //                            DirectionDate = direc.DirectionDate,
        //                            DirectionMo = direc.DirectionMo,
        //                            DirectionViewId = direc.DirectionViewId,
        //                            MetIsslId = direc.MetIsslId,
        //                            DirectionService = direc.DirectionService
        //                        };
        //                        directions.Add(direction);
        //                    }

        //                    var сonsultations = new List<ZFactConsultations>();
        //                    foreach (v32K1.D.ConsultationsOnkD cons in eventD.ConsultationsOnkXml)
        //                    {
        //                        if (cons == null) continue;
        //                        var сonsultation = new ZFactConsultations
        //                        {
        //                            DtCons = cons.DtCons,
        //                            PrCons = cons.PrCons
        //                        };
        //                        сonsultations.Add(сonsultation);
        //                    }


        //                    var slKoefs = new List<ZFactSlKoef>();
        //                    var critList = new List<ZFactCrit>();
        //                    ZFactKsgKpg ksgKpg = null;
        //                    if (eventD.KsgKpg != null)
        //                    {
        //                        ksgKpg = Map.ObjectToObject<ZFactKsgKpg>(eventD.KsgKpg);
        //                        foreach (var i in (eventD.KsgKpg.CritXml))
        //                        {
        //                            critList.Add(new ZFactCrit { IdDkk = i });
        //                        }

        //                        foreach (var slKoefD in eventD.KsgKpg.InnerSlCoefCollection)
        //                        {
        //                            if (slKoefD == null)
        //                                continue;
        //                            var slKoef = new ZFactSlKoef
        //                            {
        //                                NumberDifficultyTreatment = slKoefD.NumberDifficultyTreatment,
        //                                ValueDifficultyTreatment = slKoefD.ValueDifficultyTreatment
        //                            };
        //                            slKoefs.Add(slKoef);
        //                        }
        //                    }

        //                    var medicalEventOnk =
        //                        new Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>,
        //                            List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>(null, null, null, null);
        //                    if (eventD.EventOnk != null)
        //                    {
        //                        var diagBloks = new List<ZFactDiagBlok>();
        //                        var contraindications = new List<ZFactContraindications>();
        //                        var serviceUslOnk = new List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>();

        //                        foreach (var diagBlokOnk in eventD.EventOnk.DiagBlokOnk)
        //                        {
        //                            if (diagBlokOnk == null)
        //                                continue;
        //                            diagBloks.Add(Map.ObjectToObject<ZFactDiagBlok>(diagBlokOnk));
        //                        }
        //                        foreach (var сontraindicationsOnk in eventD.EventOnk.СontraindicationsOnk)
        //                        {
        //                            if (сontraindicationsOnk == null)
        //                                continue;
        //                            contraindications.Add(Map.ObjectToObject<ZFactContraindications>(сontraindicationsOnk));
        //                        }
        //                        foreach (var serviceOnk in eventD.EventOnk.ServiceOnk)
        //                        {
        //                            if (serviceOnk == null)
        //                                continue;
        //                            var anticancerDrugOnkDs = new List<ZFactAnticancerDrug>();
        //                            foreach (var anticancerDrugOnkD in serviceOnk.AnticancerDrugOnkXml)
        //                            {
        //                                if (anticancerDrugOnkD == null)
        //                                    continue;
        //                                anticancerDrugOnkDs.Add(Map.ObjectToObject<ZFactAnticancerDrug>(anticancerDrugOnkD));

        //                            }
        //                            serviceUslOnk.Add(Tuple.Create(Map.ObjectToObject<ZFactMedicalServicesOnk>(serviceOnk), anticancerDrugOnkDs));
        //                        }

        //                        medicalEventOnk = Tuple.Create(Map.ObjectToObject<ZFactMedicalEventOnk>(eventD.EventOnk), diagBloks, contraindications, serviceUslOnk);
        //                    }
        //                    var services = new List<ZFactMedicalServices>();
        //                    foreach (v32K1.D.ServiceD serviceD in eventD.Service)
        //                    {
        //                        var serviceCopy = serviceD;
        //                        var service = Map.ObjectToObject<ZFactMedicalServices>(serviceCopy);
        //                        service.ExternalGUID = service.ExternalGUID.IsNullOrWhiteSpace()
        //                            ? Guid.NewGuid().ToString()
        //                            : service.ExternalGUID;
        //                        services.Add(service);
        //                    }
        //                    zevents.Add(Tuple.Create(zeventD, directions, сonsultations, medicalEventOnk, Tuple.Create(ksgKpg, critList, slKoefs), services));
        //                }

        //                var sanks = new List<ZFactSank>();
        //                foreach (v32K1.D.RefusalD refusal in zslEventD.Refusals)
        //                {
        //                    if (refusal == null)
        //                        continue;

        //                    var sank = new ZFactSank
        //                    {
        //                        ReasonId = refusal.RefusalCode,
        //                        SlidGuid = refusal.SlidGuid,
        //                        Amount = refusal.RefusalRate,
        //                        Source = (int)RefusalSource.Local,
        //                        Comments = refusal.Comments,
        //                        Type = refusal.RefusalType,
        //                        ExternalGuid = refusal.ExternalGuid,
        //                        Date = refusal.Date,
        //                        EmployeeId = Constants.SystemAccountId
        //                    };
        //                    sanks.Add(sank);
        //                }

        //                mevents.Add(Tuple.Create(zslevent, zevents, sanks));
        //            }

        //            patMevents.Add(Tuple.Create(patientDb, mevents));

        //            patients.Add(Tuple.Create(per.Item1, per.Item2, patMevents));

        //            lastPatientCount++;
        //            _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
        //        }

        //        var insertRegisterResult = _repository.InsertRegisterD(account, patients, isTestLoad);

        //        if (insertRegisterResult.Success)
        //        {
        //            result.Data = insertRegisterResult.Id;
        //        }
        //        else
        //        {
        //            result.AddError(insertRegisterResult.Errors[0]);
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        result.AddError(exception);
        //    }

        //    return result;
        //}

        public int GetPaymentStatus(decimal? acceptPrice, decimal? price)
        {
            if (acceptPrice == 0)
            {
                return 3;
            }
            if (acceptPrice < price)
            {
                return 4;
            }
            return 2;
        }

        /// <summary>
        /// Загрузка основного счета с территории версии 2.1
        /// </summary>
        /// <param name="registerData"></param>
        /// <param name="packetNumber"></param>
        /// <param name="isTestLoad"></param>
        /// <param name="operations"></param>
        /// <returns></returns>
        public OperationResult<int> Load(v21.E.RegisterE registerData,int packetNumber, bool isTestLoad, ProcessingOperations operations)
        {
            var result = new OperationResult<int>();
            try
            {
                IEnumerable<Tuple<FactPatient, FactPerson, FactDocument, IEnumerable<Tuple<FactMedicalEvent, IEnumerable<FactMedicalServices>, IEnumerable<FactMEC>, IEnumerable<FactMEE>, IEnumerable<FactEQMA>>>>> data = null;

                string territoryName = string.Empty;

                //Проверяем дублирование счета
                var existAccountResult = _repository.GetTerritoryAccountView(
                    view => view.AccountNumber == registerData.Account.AccountNumber &&
                            view.Destination == registerData.Header.TargetOkato &&
                            view.Source == registerData.Header.SourceOkato &&
                            view.ExternalId == registerData.Account.ExternalId &&
                            view.Type == (int)AccountType.GeneralPart &&
                            Sql.DateDiff(Sql.DateParts.Year, view.Date, registerData.Header.Date) == 0 
                            );
                
                if (existAccountResult.Success && existAccountResult.Data.Any())
                {
                    throw new InvalidOperationException(string.Format("Счет №{0} от территории {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, existAccountResult.Data.First().SourceName, existAccountResult.Data.First().Date));
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    throw new InvalidOperationException(string.Format("В счете №{0} от территории {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, existAccountResult.Data.First().SourceName));
                }

                int direction = GetDirectionByOkato(registerData.Header.TargetOkato);
                var account = new FactTerritoryAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = registerData.Account.AccountNumber,
                    AccountDate = registerData.Account.AccountDate,
                    Price = registerData.Account.Price,
                    AcceptPrice = registerData.Account.AcceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    Type = AccountType.GeneralPart.ToInt32(),
                    Destination = registerData.Header.TargetOkato,
                    Source = registerData.Header.SourceOkato,
                    Comments = registerData.Account.Comments,
                    ExternalId = registerData.Account.ExternalId,
                    Direction = direction,
                    Parent = 0,
                    PacketNumber = packetNumber,
                    Version = registerData.Header.Version
                };

                var total = registerData.RecordsCollection.Count;
                var lastPatientCount = 0;
                var patients = new List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<FactMedicalEvent, List<FactMedicalServices>, List<FactExternalRefuse>>>>>();
                foreach (v21.E.RecordsE entry in registerData.RecordsCollection)
                {
                    FactPerson personDb = null;
                    FactDocument documentDb = null;

                    int personId = 0;
                    int? documentId = default(int?);

                    var personCheckResult = _repository.GetPerson(p => p.PName == entry.Patient.PName &&
                                p.Surname == entry.Patient.Surname &&
                                p.Patronymic == entry.Patient.Patronymic &&
                                p.Birthday == entry.Patient.Birthday &&
                                p.RepresentativeSurname == entry.Patient.RepresentativeSurname &&
                                p.RepresentativeName == entry.Patient.RepresentativeName &&
                                p.RepresentativePatronymic == entry.Patient.RepresentativePatronymic &&
                                p.RepresentativeBirthday == entry.Patient.RepresentativeBirthday);

                    if (personCheckResult.Success)
                    {
                        if (!personCheckResult.Data.Any())
                        {
                            personDb = Map.ObjectToObject<FactPerson>(entry.Patient);
                            if (entry.Patient.DocType.HasValue)
                            {
                                documentDb = new FactDocument
                                {
                                    DocType = entry.Patient.DocType,
                                    DocNum = entry.Patient.DocNumber,
                                    DocSeries = entry.Patient.DocSeries
                                };
                            }
                        }
                        else
                        {
                            personId = personCheckResult.Data.First().PersonId;
                            var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
                                                                                p.DocType == entry.Patient.DocType &&
                                                                                p.DocSeries == entry.Patient.DocSeries);

                            if (!docCheckResult.Data.Any())
                            {
                                if (entry.Patient.DocType.HasValue)
                                {
                                    documentDb = new FactDocument
                                    {
                                        PersonId = personId,
                                        DocType = entry.Patient.DocType,
                                        DocNum = entry.Patient.DocNumber,
                                        DocSeries = entry.Patient.DocSeries
                                    };
                                }
                            }
                            else
                            {
                                documentId = docCheckResult.Data.First().DocumentId;
                            }
                        }
                    }
                    else
                    {
                        result.AddError(new Exception("Ошибка поиска персональных данных"));
                        return result;
                    }

                    var lastPatientId = entry.ExternalId;

                    FactPatient patientDb = Map.ObjectToObject<FactPatient>(entry.Patient);
                    if (personDb.IsNull())
                    {
                        patientDb.PersonalId = personId;
                    }
                    if (documentDb.IsNull())
                    {
                        patientDb.DocumentId = documentId;
                    }

                    patientDb.ExternalId = lastPatientId;

                    var mevents = new List<Tuple<FactMedicalEvent, List<FactMedicalServices>, List<FactExternalRefuse>>>();
                    foreach (DataCore.v21.E.EventE eventE in entry.EventCollection)
                    {
                        var mevent = Map.ObjectToObject<FactMedicalEvent>(eventE);

                        var refusals = new List<FactExternalRefuse>();

                        foreach (DataCore.v21.E.RefusalE refusal in eventE.InnerRefusals.Where(p=>p.IsNotNull()))
                        {
                            var externalRefusal = new FactExternalRefuse
                            {
                                ReasonId = refusal.RefusalCode ?? 53,
                                Amount = refusal.RefusalRate,
                                Source = (int)RefusalSource.Local,
                                Comment = refusal.Comments,
                                ExternalGuid = refusal.ExternalGuid,
                                Type = refusal.RefusalType ?? (int)RefusalType.MEC,
                                Generation = 0,
                            };
                            refusals.Add(externalRefusal);
                        }

                        var services = new List<FactMedicalServices>();
                        foreach (DataCore.v21.E.ServiceE serviceE in eventE.InnerServiceCollection)
                        {
                            var serviceCopy = serviceE;
                            var service = Map.ObjectToObject<FactMedicalServices>(serviceCopy);

                            //Исправление специальности V015, т.к. нет в услуге указания версии справочника
                            if (eventE.SpecialityCodeV015.HasValue)
                            {
                                service.SpecialityCodeV015 = _cache.Get(CacheRepository.V015Cache).GetBack(serviceCopy.SpecialityCodeXml);
                                service.SpecialityCode = null;
                            }
                            else
                            {
                                service.SpecialityCodeV015 = null;
                                service.SpecialityCode = _cache.Get(CacheRepository.V004Cache).GetBack(serviceCopy.SpecialityCodeXml);
                            }

                            services.Add(service);
                        }

                        mevents.Add(Tuple.Create(mevent, services, refusals));
                    }

                    patients.Add(Tuple.Create(patientDb, personDb, documentDb, mevents));

                    lastPatientCount++;
                    _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                }

                var insertRegisterResult = _repository.InsertRegisterE(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        /// <summary>
        /// Загрузка основного счета с территории версии 3.0
        /// </summary>
        /// <param name="registerData"></param>
        /// <param name="packetNumber"></param>
        /// <param name="isTestLoad"></param>
        /// <param name="operations"></param>
        /// <returns></returns>
        public OperationResult<int> LoadZ(v30.E.RegisterE registerData, int packetNumber, bool isTestLoad, ProcessingOperations operations)
        {
            var result = new OperationResult<int>();
            try
            {
                string territoryName = string.Empty;

                //Проверяем дублирование счета
                var existAccountResult = _repository.GetTerritoryAccountView(
                    view => view.AccountNumber == registerData.Account.AccountNumber &&
                            view.Destination == registerData.Header.TargetOkato &&
                            view.Source == registerData.Header.SourceOkato &&
                            view.ExternalId == registerData.Account.ExternalId &&
                            view.Type == (int)AccountType.GeneralPart &&
                            view.Date.Value.Year == registerData.Account.Year);
                //Sql.DateDiff(Sql.DateParts.Year, view.Date, registerData.Header.Date) == 0

                if (existAccountResult.Success && existAccountResult.Data.Any())
                {
                    throw new InvalidOperationException(string.Format("Счет №{0} от территории {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, existAccountResult.Data.First().SourceName, existAccountResult.Data.First().Date));
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    throw new InvalidOperationException(string.Format("В счете №{0} от территории {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, existAccountResult.Data.First().SourceName));
                }

                int direction = GetDirectionByOkato(registerData.Header.TargetOkato);
                var account = new FactTerritoryAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = registerData.Account.AccountNumber,
                    AccountDate = registerData.Account.AccountDate,
                    Price = registerData.Account.Price,
                    AcceptPrice = registerData.Account.AcceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    Type = AccountType.GeneralPart.ToInt32(),
                    Destination = registerData.Header.TargetOkato,
                    Source = registerData.Header.SourceOkato,
                    Comments = registerData.Account.Comments,
                    ExternalId = registerData.Account.ExternalId,
                    Direction = direction,
                    Parent = 0,
                    PacketNumber = packetNumber,
                    Version = registerData.Header.Version
                };

                var total = registerData.RecordsCollection.Count;
                var lastPatientCount = 0;
                var patients = new List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>>>();
                foreach (v30.E.RecordsE entry in registerData.RecordsCollection)
                {
                    FactPerson personDb = null;
                    FactDocument documentDb = null;

                    int personId = 0;
                    int? documentId = default(int?);

                    var personCheckResult = _repository.GetPerson(p => p.PName == entry.Patient.PName &&
                                p.Surname == entry.Patient.Surname &&
                                p.Patronymic == entry.Patient.Patronymic &&
                                p.Birthday == entry.Patient.Birthday &&
                                p.RepresentativeSurname == (entry.Patient.RepresentativeSurname ?? "") &&
                                p.RepresentativeName == (entry.Patient.RepresentativeName ?? "") &&
                                p.RepresentativePatronymic == (entry.Patient.RepresentativePatronymic ?? "") &&
                                p.RepresentativeBirthday == entry.Patient.RepresentativeBirthday);

                    if (personCheckResult.Success)
                    {
                        if (!personCheckResult.Data.Any())
                        {
                            personDb = Map.ObjectToObject<FactPerson>(entry.Patient);
                            if (entry.Patient.DocType.HasValue)
                            {
                                documentDb = new FactDocument
                                {
                                    DocType = entry.Patient.DocType,
                                    DocNum = entry.Patient.DocNumber,
                                    DocSeries = entry.Patient.DocSeries
                                };
                            }
                        }
                        else
                        {
                            personId = personCheckResult.Data.First().PersonId;
                            var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
                                                                                p.DocType == entry.Patient.DocType &&
                                                                                p.DocSeries == entry.Patient.DocSeries);

                            if (!docCheckResult.Data.Any())
                            {
                                if (entry.Patient.DocType.HasValue)
                                {
                                    documentDb = new FactDocument
                                    {
                                        PersonId = personId,
                                        DocType = entry.Patient.DocType,
                                        DocNum = entry.Patient.DocNumber,
                                        DocSeries = entry.Patient.DocSeries
                                    };
                                }
                            }
                            else
                            {
                                documentId = docCheckResult.Data.First().DocumentId;
                            }
                        }
                    } 
                    else
                    {
                        result.AddError(new Exception("Ошибка поиска персональных данных"));
                        return result;
                    }

                    var lastPatientId = entry.ExternalId;

                    FactPatient patientDb = Map.ObjectToObject<FactPatient>(entry.Patient);
                    if (personDb.IsNull())
                    {
                        patientDb.PersonalId = personId;
                    }
                    if (documentDb.IsNull())
                    {
                        patientDb.DocumentId = documentId;
                    }

                    patientDb.ExternalId = lastPatientId;

                    var mevents = new List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>();
                    foreach (v30.E.ZslEventE zslEventE in entry.EventCollection)
                    {
                        var zsleventECopy = zslEventE;
                        var zslmevent = Map.ObjectToObject<ZslFactMedicalEvent>(zsleventECopy);

                        var zevents = new List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>();
                        foreach (v30.E.EventE eventE in zslEventE.InnerMeventCollection)
                        {
                            var zeventCopy = eventE;
                            ZFactMedicalEvent zmevent = Map.ObjectToObject<ZFactMedicalEvent>(zeventCopy);

                            var slKoefs = new List<ZFactSlKoef>();
                            ZFactKsgKpg ksgKpg = null;
                            if (eventE.InnerKsgKpg != null)
                            {
                                ksgKpg = Map.ObjectToObject<ZFactKsgKpg>(eventE.InnerKsgKpg);

                                foreach (var slKoefD in eventE.InnerKsgKpg.InnerSlCoefCollection)
                                {
                                    if (slKoefD == null)
                                        continue;
                                    var slKoef = new ZFactSlKoef
                                    {
                                        NumberDifficultyTreatment = slKoefD.NumberDifficultyTreatment,
                                        ValueDifficultyTreatment = slKoefD.ValueDifficultyTreatment
                                    };
                                    slKoefs.Add(slKoef);
                                }
                            }

                            var refusals = new List<ZFactExternalRefuse>();

                            foreach (v30.E.RefusalE refusal in eventE.InnerRefusals.Where(p => p.IsNotNull()))
                            {
                                var externalRefusal = new ZFactExternalRefuse
                                {
                                    ReasonId = refusal.RefusalCode ?? 53,
                                    Amount = refusal.RefusalRate,
                                    Source = (int)RefusalSource.Local,
                                    Comment = refusal.Comments,
                                    ExternalGuid = refusal.ExternalGuid,
                                    Type = refusal.RefusalType ?? (int)RefusalType.MEC,
                                    Generation = 0,
                                };
                                refusals.Add(externalRefusal);
                            }

                            var services = new List<ZFactMedicalServices>();
                            foreach (v30.E.ServiceE serviceE in eventE.InnerServiceCollection)
                            {
                                var serviceCopy = serviceE;
                                var service = Map.ObjectToObject<ZFactMedicalServices>(serviceCopy);
                                //Исправление специальности V015, т.к. нет в услуге указания версии справочника
                                if (eventE.SpecialityCodeV015.HasValue)
                                {
                                    service.SpecialityCodeV015 = _cache.Get(CacheRepository.V015Cache).GetBack(serviceCopy.SpecialityCodeV021Xml);
                                    service.SpecialityCodeV021 = null;
                                }
                                else
                                {
                                    service.SpecialityCodeV015 = null;
                                    service.SpecialityCodeV021 = _cache.Get(CacheRepository.V021Cache).GetBack(serviceCopy.SpecialityCodeV021Xml);
                                }
                                services.Add(service);
                            }
                            zevents.Add(Tuple.Create(zmevent, Tuple.Create(ksgKpg, slKoefs), services, refusals));
                        }
                        mevents.Add(Tuple.Create(zslmevent, zevents));

                    }

                    patients.Add(Tuple.Create(patientDb, personDb, documentDb, mevents));

                    lastPatientCount++;
                    _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                }

                var insertRegisterResult = _repository.InsertRegisterE(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        /// <summary>
        /// Загрузка основного счета с территории версии 3.1
        /// </summary>
        /// <param name="registerData"></param>
        /// <param name="packetNumber"></param>
        /// <param name="isTestLoad"></param>
        /// <param name="operations"></param>
        /// <returns></returns>
        public OperationResult<int> LoadZ(v31.E.RegisterE registerData, int packetNumber, bool isTestLoad, ProcessingOperations operations)
        {
            var result = new OperationResult<int>();
            try
            {
                string territoryName = string.Empty;

                //Проверяем дублирование счета
                var existAccountResult = _repository.GetTerritoryAccountView(
                    view => view.AccountNumber == registerData.Account.AccountNumber &&
                            view.Destination == registerData.Header.TargetOkato &&
                            view.Source == registerData.Header.SourceOkato &&
                            view.ExternalId == registerData.Account.ExternalId &&
                            view.Type == (int) AccountType.GeneralPart &&
                            view.Date.Value.Year == registerData.Account.Year
                    //Sql.DateDiff(Sql.DateParts.Year, view.Date, registerData.Header.Date) == 0
                    );

                if (existAccountResult.Success && existAccountResult.Data.Any())
                {
                    throw new InvalidOperationException(string.Format("Счет №{0} от территории {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, existAccountResult.Data.First().SourceName, existAccountResult.Data.First().Date));
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    throw new InvalidOperationException(string.Format("В счете №{0} от территории {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, existAccountResult.Data.First().SourceName));
                }

                int directionOkato = GetDirectionByOkato(registerData.Header.TargetOkato);
                var account = new FactTerritoryAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = registerData.Account.AccountNumber,
                    AccountDate = registerData.Account.AccountDate,
                    Price = registerData.Account.Price,
                    AcceptPrice = registerData.Account.AcceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    Type = AccountType.GeneralPart.ToInt32(),
                    Destination = registerData.Header.TargetOkato,
                    Source = registerData.Header.SourceOkato,
                    Comments = registerData.Account.Comments,
                    ExternalId = registerData.Account.ExternalId,
                    Direction = directionOkato,
                    Parent = 0,
                    PacketNumber = packetNumber,
                    Version = registerData.Header.Version
                };

                var total = registerData.RecordsCollection.Count;
                var lastPatientCount = 0;
                //var patients = new List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>>>();
                var patients = new List<Tuple<FactPatient, FactPerson, FactDocument, 
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
                            >>>>();
                foreach (v31.E.RecordsE entry in registerData.RecordsCollection)
                {
                    FactPerson personDb = null;
                    FactDocument documentDb = null;

                    int personId = 0;
                    int? documentId = default(int?);

                    var personCheckResult = _repository.GetPerson(p => p.PName == entry.Patient.PName &&
                                p.Surname == entry.Patient.Surname &&
                                p.Patronymic == entry.Patient.Patronymic &&
                                p.Birthday == entry.Patient.Birthday &&
                                p.RepresentativeSurname == (entry.Patient.RepresentativeSurname ?? "") &&
                                p.RepresentativeName == (entry.Patient.RepresentativeName ?? "") &&
                                p.RepresentativePatronymic == (entry.Patient.RepresentativePatronymic ?? "") &&
                                p.RepresentativeBirthday == entry.Patient.RepresentativeBirthday);

                    if (personCheckResult.Success)
                    {
                        if (!personCheckResult.Data.Any())
                        {
                            personDb = Map.ObjectToObject<FactPerson>(entry.Patient);
                            if (entry.Patient.DocType.HasValue)
                            {
                                documentDb = new FactDocument
                                {
                                    DocType = entry.Patient.DocType,
                                    DocNum = entry.Patient.DocNumber,
                                    DocSeries = entry.Patient.DocSeries
                                };
                            }
                        }
                        else
                        {
                            personId = personCheckResult.Data.First().PersonId;
                            var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
                                                                                p.DocType == entry.Patient.DocType &&
                                                                                p.DocSeries == entry.Patient.DocSeries);

                            if (!docCheckResult.Data.Any())
                            {
                                if (entry.Patient.DocType.HasValue)
                                {
                                    documentDb = new FactDocument
                                    {
                                        PersonId = personId,
                                        DocType = entry.Patient.DocType,
                                        DocNum = entry.Patient.DocNumber,
                                        DocSeries = entry.Patient.DocSeries
                                    };
                                }
                            }
                            else
                            {
                                documentId = docCheckResult.Data.First().DocumentId;
                            }
                        }
                    }
                    else
                    {
                        result.AddError(new Exception("Ошибка поиска персональных данных"));
                        return result;
                    }

                    var lastPatientId = entry.ExternalId;

                    FactPatient patientDb = Map.ObjectToObject<FactPatient>(entry.Patient);
                    if (personDb.IsNull())
                    {
                        patientDb.PersonalId = personId;
                    }
                    if (documentDb.IsNull())
                    {
                        patientDb.DocumentId = documentId;
                    }

                    patientDb.ExternalId = lastPatientId;

                    //var mevents = new List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>();
                    var mevents = new List<Tuple<ZslFactMedicalEvent,
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
                        >>();
                    foreach (v31.E.ZslEventE zslEventE in entry.EventCollection)
                    {
                        var zsleventECopy = zslEventE;
                        var zslmevent = Map.ObjectToObject<ZslFactMedicalEvent>(zsleventECopy);

                        //var zevents = new List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>();
                        var zevents = new List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            List<ZFactDs>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>();
                        int count = 0;
                        string externalId = string.Empty;
                        foreach (v31.E.EventE eventE in zslEventE.InnerMeventCollection)
                        {
                            var zeventCopy = eventE;
                            ZFactMedicalEvent zmevent = Map.ObjectToObject<ZFactMedicalEvent>(zeventCopy);
                            if (count == 0)
                            {
                                externalId = zeventCopy.ExternalId;
                            }

                            count++;
                            var directions = new List<ZFactDirection>();
                            foreach (v31.E.DirectionOnkE direc in eventE.DirectionOnkXml)
                            {
                                if (direc == null) continue;
                                var direction = new ZFactDirection
                                {
                                    DirectionDate = direc.DirectionDate,
                                    DirectionMo = direc.DirectionMo,
                                    DirectionViewId = direc.DirectionViewId,
                                    MetIsslId = direc.MetIsslId,
                                    DirectionService = direc.DirectionService
                                };
                                directions.Add(direction);
                            }

                            var сonsultations = new List<ZFactConsultations>();
                            foreach (v31.E.ConsultationsOnkE cons in eventE.ConsultationsOnkXml)
                            {
                                if (cons == null) continue;
                                var сonsultation = new ZFactConsultations
                                {
                                    DtCons = cons.DtCons,
                                    PrCons = cons.PrCons
                                };
                                сonsultations.Add(сonsultation);
                            }

                            var ds2 = new List<ZFactDs>();
                            foreach (string ds in eventE.DiagnosisSecondaryXml)
                            {
                                if (ds == null) continue;
                                var factDs = new ZFactDs
                                {
                                    Ds = ds,
                                    DsType = 2
                                };
                                ds2.Add(factDs);
                            }

                            var slKoefs = new List<ZFactSlKoef>();
                            var critList = new List<ZFactCrit>();
                            ZFactKsgKpg ksgKpg = null;
                            if (eventE.InnerKsgKpg != null)
                            {
                                string c = null; 
                                ksgKpg = Map.ObjectToObject<ZFactKsgKpg>(eventE.InnerKsgKpg);
                                if (ksgKpg.Kkpg == string.Empty)
                                {
                                    ksgKpg.Kkpg = null;
                                }
                                if (ksgKpg.Kksg == string.Empty)
                                {
                                    ksgKpg.Kksg = null;
                                }
                                
                                foreach (var i in (eventE.InnerKsgKpg.CritXml))
                                {
                                    critList.Add(new ZFactCrit { IdDkk = i });
                                }

                                foreach (var slKoefD in eventE.InnerKsgKpg.InnerSlCoefCollection)
                                {
                                    if (slKoefD == null)
                                        continue;
                                    var slKoef = new ZFactSlKoef
                                    {
                                        NumberDifficultyTreatment = slKoefD.NumberDifficultyTreatment,
                                        ValueDifficultyTreatment = slKoefD.ValueDifficultyTreatment
                                    };
                                    slKoefs.Add(slKoef);
                                }
                            }

                            var medicalEventOnk =
                                new Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>,
                                    List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>(null, null, null, null);
                            if (eventE.EventOnk != null)
                            {
                                var diagBloks = new List<ZFactDiagBlok>();
                                var contraindications = new List<ZFactContraindications>();
                                var serviceUslOnk = new List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>();

                                foreach (var diagBlokOnk in eventE.EventOnk.DiagBlokOnk)
                                {
                                    if (diagBlokOnk == null)
                                        continue;
                                    diagBloks.Add(Map.ObjectToObject<ZFactDiagBlok>(diagBlokOnk));
                                }
                                foreach (var сontraindicationsOnk in eventE.EventOnk.СontraindicationsOnk)
                                {
                                    if (сontraindicationsOnk == null)
                                        continue;
                                    contraindications.Add(Map.ObjectToObject<ZFactContraindications>(сontraindicationsOnk));
                                }
                                foreach (var serviceOnk in eventE.EventOnk.ServiceOnk)
                                {
                                    if (serviceOnk == null)
                                        continue;
                                    var anticancerDrugOnkDs = new List<ZFactAnticancerDrug>();
                                    foreach (var anticancerDrugOnkD in serviceOnk.AnticancerDrugOnkXml)
                                    {
                                        if (anticancerDrugOnkD == null)
                                            continue;
                                        anticancerDrugOnkDs.Add(Map.ObjectToObject<ZFactAnticancerDrug>(anticancerDrugOnkD));

                                    }
                                    serviceUslOnk.Add(Tuple.Create(Map.ObjectToObject<ZFactMedicalServicesOnk>(serviceOnk), anticancerDrugOnkDs));
                                }

                                medicalEventOnk = Tuple.Create(Map.ObjectToObject<ZFactMedicalEventOnk>(eventE.EventOnk), diagBloks, contraindications, serviceUslOnk);
                            }

                            var services = new List<ZFactMedicalServices>();
                            foreach (v31.E.ServiceE serviceE in eventE.InnerServiceCollection)
                            {
                                var serviceCopy = serviceE;
                                var service = Map.ObjectToObject<ZFactMedicalServices>(serviceCopy);
                                //Исправление специальности V015, т.к. нет в услуге указания версии справочника
                                if (eventE.SpecialityCodeV015.HasValue)
                                {
                                    service.SpecialityCodeV015 = _cache.Get(CacheRepository.V015Cache).GetBack(serviceCopy.SpecialityCodeV021Xml);
                                    service.SpecialityCodeV021 = null;
                                }
                                else
                                {
                                    service.SpecialityCodeV015 = null;
                                    service.SpecialityCodeV021 = _cache.Get(CacheRepository.V021Cache).GetBack(serviceCopy.SpecialityCodeV021Xml);
                                }
                                services.Add(service);
                            }
                            zevents.Add(Tuple.Create(zmevent, directions, сonsultations, ds2, medicalEventOnk, Tuple.Create(ksgKpg, critList, slKoefs), services));
                        }

                        var refusals = new List<ZFactExternalRefuse>();

                        foreach (v31.E.RefusalE refusal in zslEventE.RefusalsXml.Where(p => p.IsNotNull()))
                        {
                            var externalRefusal = new ZFactExternalRefuse
                            {
                                ReasonId = refusal.RefusalCode,
                                Amount = refusal.RefusalRate,
                                Source = (int)RefusalSource.Local,
                                Comment = refusal.Comments,
                                SlidGuid = refusal.SlidGuid ?? externalId,
                                ExternalGuid = refusal.ExternalGuid,
                                Type = refusal.RefusalType,
                                Generation = 0,
                            };
                            refusals.Add(externalRefusal);
                        }
                        mevents.Add(Tuple.Create(zslmevent, zevents, refusals));
                    }

                    patients.Add(Tuple.Create(patientDb, personDb, documentDb, mevents));

                    lastPatientCount++;
                    _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                }

                var insertRegisterResult = _repository.InsertRegisterE(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        /// <summary>
        /// Загрузка основного счета с территории версии 3.2
        /// </summary>
        /// <param name="registerData"></param>
        /// <param name="packetNumber"></param>
        /// <param name="isTestLoad"></param>
        /// <param name="operations"></param>
        /// <returns></returns>
        public OperationResult<int> LoadZ(v32.E.RegisterE registerData, int packetNumber, bool isTestLoad, ProcessingOperations operations)
        {
            var result = new OperationResult<int>();
            try
            {
                string territoryName = string.Empty;

                //Проверяем дублирование счета
                var existAccountResult = _repository.GetTerritoryAccountView(
                    view => view.AccountNumber == registerData.Account.AccountNumber &&
                            view.Destination == registerData.Header.TargetOkato &&
                            view.Source == registerData.Header.SourceOkato &&
                            view.ExternalId == registerData.Account.ExternalId &&
                            view.Type == (int)AccountType.GeneralPart &&
                            view.Date.Value.Year == registerData.Account.Year
                    );

                if (existAccountResult.Success && existAccountResult.Data.Any())
                {
                    throw new InvalidOperationException(string.Format("Счет №{0} от территории {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, existAccountResult.Data.First().SourceName, existAccountResult.Data.First().Date));
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    throw new InvalidOperationException(string.Format("В счете №{0} от территории {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, existAccountResult.Data.First().SourceName));
                }

                int directionOkato = GetDirectionByOkato(registerData.Header.TargetOkato);
                var account = new FactTerritoryAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = registerData.Account.AccountNumber,
                    AccountDate = registerData.Account.AccountDate,
                    Price = registerData.Account.Price,
                    AcceptPrice = registerData.Account.AcceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    Type = AccountType.GeneralPart.ToInt32(),
                    Destination = registerData.Header.TargetOkato,
                    Source = registerData.Header.SourceOkato,
                    Comments = registerData.Account.Comments,
                    ExternalId = registerData.Account.ExternalId,
                    Direction = directionOkato,
                    Parent = 0,
                    PacketNumber = packetNumber,
                    Version = registerData.Header.Version
                };

                var total = registerData.RecordsCollection.Count;
                var lastPatientCount = 0;
                //var patients = new List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>>>();
                var patients = new List<Tuple<FactPatient, FactPerson, FactDocument,
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
                            >>>>();
                foreach (v32.E.RecordsE entry in registerData.RecordsCollection)
                {
                    FactPerson personDb = null;
                    FactDocument documentDb = null;

                    int personId = 0;
                    int? documentId = default(int?);

                    var personCheckResult = _repository.GetPerson(p => p.PName == entry.Patient.PName &&
                                p.Surname == entry.Patient.Surname &&
                                p.Patronymic == entry.Patient.Patronymic &&
                                p.Birthday == entry.Patient.Birthday &&
                                p.RepresentativeSurname == (entry.Patient.RepresentativeSurname ?? "") &&
                                p.RepresentativeName == (entry.Patient.RepresentativeName ?? "") &&
                                p.RepresentativePatronymic == (entry.Patient.RepresentativePatronymic ?? "") &&
                                p.RepresentativeBirthday == entry.Patient.RepresentativeBirthday);

                    if (personCheckResult.Success)
                    {
                        if (!personCheckResult.Data.Any())
                        {
                            personDb = Map.ObjectToObject<FactPerson>(entry.Patient);
                            if (entry.Patient.DocType.HasValue)
                            {
                                documentDb = new FactDocument
                                {
                                    DocType = entry.Patient.DocType,
                                    DocNum = entry.Patient.DocNumber,
                                    DocSeries = entry.Patient.DocSeries,
                                    DocDate = entry.Patient.DocDate,
                                    DocOrg = entry.Patient.DocOrg,
                                };
                            }
                        }
                        else
                        {
                            personId = personCheckResult.Data.First().PersonId;
                            var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
                                                                                p.DocType == entry.Patient.DocType &&
                                                                                p.DocSeries == entry.Patient.DocSeries);

                            if (!docCheckResult.Data.Any())
                            {
                                if (entry.Patient.DocType.HasValue)
                                {
                                    documentDb = new FactDocument
                                    {
                                        PersonId = personId,
                                        DocType = entry.Patient.DocType,
                                        DocNum = entry.Patient.DocNumber,
                                        DocSeries = entry.Patient.DocSeries,
                                        DocDate = entry.Patient.DocDate,
                                        DocOrg = entry.Patient.DocOrg,
                                    };
                                }
                            }
                            else
                            {
                                documentId = docCheckResult.Data.First().DocumentId;
                            }
                        }
                    }
                    else
                    {
                        result.AddError(new Exception("Ошибка поиска персональных данных"));
                        return result;
                    }

                    var lastPatientId = entry.ExternalId;

                    FactPatient patientDb = Map.ObjectToObject<FactPatient>(entry.Patient);
                    if (personDb.IsNull())
                    {
                        patientDb.PersonalId = personId;
                    }
                    if (documentDb.IsNull())
                    {
                        patientDb.DocumentId = documentId;
                    }

                    patientDb.ExternalId = lastPatientId;

                    //var mevents = new List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>();
                    var mevents = new List<Tuple<ZslFactMedicalEvent,
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
                        >>();
                    foreach (v32.E.ZslEventE zslEventE in entry.EventCollection)
                    {
                        var zsleventECopy = zslEventE;
                        var zslmevent = Map.ObjectToObject<ZslFactMedicalEvent>(zsleventECopy);

                        //var zevents = new List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>();
                        var zevents = new List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            List<ZFactDs>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>();

                        int count = 0;
                        string externalId = string.Empty;
                        foreach (v32.E.EventE eventE in zslEventE.InnerMeventCollection)
                        {
                            var zeventCopy = eventE;
                            ZFactMedicalEvent zmevent = Map.ObjectToObject<ZFactMedicalEvent>(zeventCopy);
                            if (count == 0)
                            {
                                externalId = zeventCopy.ExternalId;
                            }

                            count++;
                            var directions = new List<ZFactDirection>();
                            foreach (v32.E.DirectionOnkE direc in eventE.DirectionOnkXml)
                            {
                                if (direc == null) continue;
                                var direction = new ZFactDirection
                                {
                                    DirectionDate = direc.DirectionDate,
                                    DirectionMo = direc.DirectionMo,
                                    DirectionViewId = direc.DirectionViewId,
                                    MetIsslId = direc.MetIsslId,
                                    DirectionService = direc.DirectionService
                                };
                                directions.Add(direction);
                            }

                            var сonsultations = new List<ZFactConsultations>();
                            foreach (v32.E.ConsultationsOnkE cons in eventE.ConsultationsOnkXml)
                            {
                                if (cons == null) continue;
                                var сonsultation = new ZFactConsultations
                                {
                                    DtCons = cons.DtCons,
                                    PrCons = cons.PrCons
                                };
                                сonsultations.Add(сonsultation);
                            }

                            var ds2 = new List<ZFactDs>();
                            foreach (string ds in eventE.DiagnosisSecondaryXml)
                            {
                                if (ds == null) continue;
                                var factDs = new ZFactDs
                                {
                                    Ds = ds,
                                    DsType = 2
                                };
                                ds2.Add(factDs);
                            }

                            var slKoefs = new List<ZFactSlKoef>();
                            var critList = new List<ZFactCrit>();
                            ZFactKsgKpg ksgKpg = null;
                            if (eventE.InnerKsgKpg != null)
                            {
                                ksgKpg = Map.ObjectToObject<ZFactKsgKpg>(eventE.InnerKsgKpg);
                                if (ksgKpg.Kkpg == string.Empty)
                                {
                                    ksgKpg.Kkpg = null;
                                }
                                if (ksgKpg.Kksg == string.Empty)
                                {
                                    ksgKpg.Kksg = null;
                                }
                                foreach (var i in (eventE.InnerKsgKpg.CritXml))
                                {
                                    critList.Add(new ZFactCrit { IdDkk = i });
                                }

                                foreach (var slKoefD in eventE.InnerKsgKpg.InnerSlCoefCollection)
                                {
                                    if (slKoefD == null)
                                        continue;
                                    var slKoef = new ZFactSlKoef
                                    {
                                        NumberDifficultyTreatment = slKoefD.NumberDifficultyTreatment,
                                        ValueDifficultyTreatment = slKoefD.ValueDifficultyTreatment
                                    };
                                    slKoefs.Add(slKoef);
                                }
                            }

                            var medicalEventOnk =
                                new Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>,
                                    List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>(null, null, null, null);
                            if (eventE.EventOnk != null)
                            {
                                var diagBloks = new List<ZFactDiagBlok>();
                                var contraindications = new List<ZFactContraindications>();
                                var serviceUslOnk = new List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>();

                                foreach (var diagBlokOnk in eventE.EventOnk.DiagBlokOnk)
                                {
                                    if (diagBlokOnk == null)
                                        continue;
                                    diagBloks.Add(Map.ObjectToObject<ZFactDiagBlok>(diagBlokOnk));
                                }
                                foreach (var сontraindicationsOnk in eventE.EventOnk.СontraindicationsOnk)
                                {
                                    if (сontraindicationsOnk == null)
                                        continue;
                                    contraindications.Add(Map.ObjectToObject<ZFactContraindications>(сontraindicationsOnk));
                                }
                                foreach (var serviceOnk in eventE.EventOnk.ServiceOnk)
                                {
                                    if (serviceOnk == null)
                                        continue;
                                    var anticancerDrugOnkDs = new List<ZFactAnticancerDrug>();
                                    foreach (var anticancerDrugOnkD in serviceOnk.AnticancerDrugOnkXml)
                                    {
                                        if (anticancerDrugOnkD == null)
                                            continue;
                                        anticancerDrugOnkDs.Add(Map.ObjectToObject<ZFactAnticancerDrug>(anticancerDrugOnkD));

                                    }
                                    serviceUslOnk.Add(Tuple.Create(Map.ObjectToObject<ZFactMedicalServicesOnk>(serviceOnk), anticancerDrugOnkDs));
                                }

                                medicalEventOnk = Tuple.Create(Map.ObjectToObject<ZFactMedicalEventOnk>(eventE.EventOnk), diagBloks, contraindications, serviceUslOnk);
                            }

                            var services = new List<ZFactMedicalServices>();
                            foreach (v32.E.ServiceE serviceE in eventE.InnerServiceCollection)
                            {
                                var serviceCopy = serviceE;
                                var service = Map.ObjectToObject<ZFactMedicalServices>(serviceCopy);
                                //Исправление специальности V015, т.к. нет в услуге указания версии справочника
                                if (eventE.SpecialityCodeV015.HasValue)
                                {
                                    service.SpecialityCodeV015 = _cache.Get(CacheRepository.V015Cache).GetBack(serviceCopy.SpecialityCodeV021Xml);
                                    service.SpecialityCodeV021 = null;
                                }
                                else
                                {
                                    service.SpecialityCodeV015 = null;
                                    service.SpecialityCodeV021 = _cache.Get(CacheRepository.V021Cache).GetBack(serviceCopy.SpecialityCodeV021Xml);
                                }
                                services.Add(service);
                            }
                            zevents.Add(Tuple.Create(zmevent, directions, сonsultations, ds2, medicalEventOnk, Tuple.Create(ksgKpg, critList, slKoefs), services));
                        }

                        var refusals = new List<ZFactExternalRefuse>();

                        foreach (v32.E.RefusalE refusal in zslEventE.RefusalsXml.Where(p => p.IsNotNull()))
                        {
                            var externalRefusal = new ZFactExternalRefuse
                            {
                                ReasonId = refusal.RefusalCode,
                                Amount = refusal.RefusalRate,
                                Source = (int)RefusalSource.Local,
                                Comment = refusal.Comments,
                                SlidGuid = refusal.SlidGuid?? externalId,
                                ExternalGuid = refusal.ExternalGuid,
                                Type = refusal.RefusalType,
                                Generation = 0,
                            };
                            refusals.Add(externalRefusal);
                        }
                        mevents.Add(Tuple.Create(zslmevent, zevents, refusals));
                    }

                    patients.Add(Tuple.Create(patientDb, personDb, documentDb, mevents));

                    lastPatientCount++;
                    _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                }

                var insertRegisterResult = _repository.InsertRegisterE(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<int> LoadBack(RegisterE registerData, int packetNumber, bool isTestLoad, ProcessingOperations operations)
        {
            var result = new OperationResult<int>();
            try
            {
                IEnumerable<Tuple<FactPatient, FactPerson, FactDocument, IEnumerable<Tuple<FactMedicalEvent, IEnumerable<FactMedicalServices>, IEnumerable<FactMEC>, IEnumerable<FactMEE>, IEnumerable<FactEQMA>>>>> data = null;

                string territoryName = string.Empty;

                //Проверяем дублирование счета
                var existAccountResult = _repository.GetTerritoryAccountView(
                    view => view.AccountNumber == registerData.Account.AccountNumber &&
                            view.Destination == registerData.Header.TargetOkato &&
                            view.Source == registerData.Header.SourceOkato &&
                            view.ExternalId == registerData.Account.ExternalId &&
                            view.Type == (int)AccountType.CorrectedPart &&
                            Sql.DateDiff(Sql.DateParts.Year, view.Date, registerData.Header.Date) == 0);

                if (existAccountResult.Success && existAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Счет №{0}(исправленная часть) от территории {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, existAccountResult.Data.FirstOrDefault()?.SourceName, existAccountResult.Data.FirstOrDefault()?.Date));
                    return result;
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    result.AddError(string.Format("В счете №{0}(исправленная часть) от территории {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, existAccountResult.Data.FirstOrDefault()?.SourceName));
                    return result;
                }

                //Поиск основной части
                var parentAccountResult = _repository.GetTerritoryAccountView(
                    p => p.AccountNumber == registerData.Account.AccountNumber &&
                    p.Destination == registerData.Header.TargetOkato &&
                    p.Date == new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1) &&
                    p.Type == (int)AccountType.GeneralPart);
                if (parentAccountResult.HasError || parentAccountResult.Data.IsNull() || !parentAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Для счета №{0}(исправленная часть) от территории {1} не найдена основная часть.",
                                                    registerData.Account.AccountNumber, existAccountResult.Data.FirstOrDefault()?.SourceName));
                    return result;
                }

                var parentAccount = parentAccountResult.Data.First();

                int direction = GetDirectionByOkato(registerData.Header.TargetOkato);
                var account = new FactTerritoryAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = registerData.Account.AccountNumber,
                    AccountDate = registerData.Account.AccountDate,
                    Price = registerData.Account.Price,
                    AcceptPrice = registerData.Account.AcceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    Type = AccountType.CorrectedPart.ToInt32(),
                    Destination = registerData.Header.TargetOkato,
                    Source = registerData.Header.SourceOkato,
                    Comments = registerData.Account.Comments,
                    ExternalId = registerData.Account.ExternalId,
                    Direction = direction,
                    Parent = parentAccount.TerritoryAccountId,
                    PacketNumber = packetNumber,
                    Version = registerData.Header.Version
                };

                var total = registerData.RecordsCollection.Count;
                var lastPatientCount = 0;
                var patients = new List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<FactMedicalEvent, List<FactMedicalServices>, List<FactExternalRefuse>>>>>();
                foreach (v21.E.RecordsE entry in registerData.RecordsCollection)
                {
                    FactPerson personDb = null;
                    FactDocument documentDb = null;

                    int personId = 0;
                    int? documentId = default(int?);

                    var personCheckResult = _repository.GetPerson(p => p.PName == entry.Patient.PName &&
                                p.Surname == entry.Patient.Surname &&
                                p.Patronymic == entry.Patient.Patronymic &&
                                p.Birthday == entry.Patient.Birthday &&
                                p.RepresentativeSurname == entry.Patient.RepresentativeSurname &&
                                p.RepresentativeName == entry.Patient.RepresentativeName &&
                                p.RepresentativePatronymic == entry.Patient.RepresentativePatronymic &&
                                p.RepresentativeBirthday == entry.Patient.RepresentativeBirthday);

                    if (personCheckResult.Success)
                    {
                        if (!personCheckResult.Data.Any())
                        {
                            personDb = Map.ObjectToObject<FactPerson>(entry.Patient);
                            if (entry.Patient.DocType.HasValue)
                            {
                                documentDb = new FactDocument
                                {
                                    DocType = entry.Patient.DocType,
                                    DocNum = entry.Patient.DocNumber,
                                    DocSeries = entry.Patient.DocSeries
                                };
                            }
                        }
                        else
                        {
                            personId = personCheckResult.Data.First().PersonId;
                            var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
                                                                                p.DocType == entry.Patient.DocType &&
                                                                                p.DocSeries == entry.Patient.DocSeries);

                            if (!docCheckResult.Data.Any())
                            {
                                if (entry.Patient.DocType.HasValue)
                                {
                                    documentDb = new FactDocument
                                    {
                                        PersonId = personId,
                                        DocType = entry.Patient.DocType,
                                        DocNum = entry.Patient.DocNumber,
                                        DocSeries = entry.Patient.DocSeries
                                    };
                                }
                            }
                            else
                            {
                                documentId = docCheckResult.Data.First().DocumentId;
                            }
                        }
                    }
                    else
                    {
                        result.AddError(new Exception("Ошибка поиска персональных данных"));
                        return result;
                    }

                    var lastPatientId = entry.ExternalId;

                    FactPatient patientDb = Map.ObjectToObject<FactPatient>(entry.Patient);
                    if (personDb.IsNull())
                    {
                        patientDb.PersonalId = personId;
                    }
                    if (documentDb.IsNull())
                    {
                        patientDb.DocumentId = documentId;
                    }

                    patientDb.ExternalId = lastPatientId;

                    var mevents = new List<Tuple<FactMedicalEvent, List<FactMedicalServices>, List<FactExternalRefuse>>>();
                    foreach (DataCore.v21.E.EventE eventE in entry.EventCollection)
                    {
                        var mevent = Map.ObjectToObject<FactMedicalEvent>(eventE);

                        var refusals = new List<FactExternalRefuse>();

                        foreach (DataCore.v21.E.RefusalE refusal in eventE.InnerRefusals.Where(p => p.IsNotNull()))
                        {
                            var externalRefusal = new FactExternalRefuse
                            {
                                ReasonId = refusal.RefusalCode ?? 53,
                                Amount = refusal.RefusalRate,
                                Source = (int)RefusalSource.Local,
                                Comment = refusal.Comments,
                                ExternalGuid = refusal.ExternalGuid,
                                Type = refusal.RefusalType ?? (int)RefusalType.MEC,
                                Generation = 0,
                            };
                            refusals.Add(externalRefusal);
                        }

                        var services = new List<FactMedicalServices>();
                        foreach (v21.E.ServiceE serviceE in eventE.InnerServiceCollection)
                        {
                            var serviceCopy = serviceE;
                            var service = Map.ObjectToObject<FactMedicalServices>(serviceCopy);

                            //Исправление специальности V015, т.к. нет в услуге указания версии справочника
                            if (eventE.SpecialityCodeV015.HasValue)
                            {
                                service.SpecialityCodeV015 = _cache.Get(CacheRepository.V015Cache).GetBack(serviceCopy.SpecialityCode);
                                service.SpecialityCode = null;
                            }
                            else
                            {
                                service.SpecialityCodeV015 = null;
                                service.SpecialityCode = _cache.Get(CacheRepository.V004Cache).GetBack(serviceCopy.SpecialityCode);
                            }

                            services.Add(service);
                        }

                        mevents.Add(Tuple.Create(mevent, services, refusals));
                    }

                    patients.Add(Tuple.Create(patientDb, personDb, documentDb, mevents));

                    lastPatientCount++;
                    _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                }

                var insertRegisterResult = _repository.InsertRegisterE(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<int> LoadBackZ(v30.E.RegisterE registerData, int packetNumber, bool isTestLoad, ProcessingOperations operations)
        {
            var result = new OperationResult<int>();
            try
            {
               // IEnumerable<Tuple<FactPatient, FactPerson, FactDocument, IEnumerable<Tuple<FactMedicalEvent, IEnumerable<FactMedicalServices>, IEnumerable<FactMEC>, IEnumerable<FactMEE>, IEnumerable<FactEQMA>>>>> data = null;

                string territoryName = string.Empty;

                //Проверяем дублирование счета
                var existAccountResult = _repository.GetTerritoryAccountView(
                    view => view.AccountNumber == registerData.Account.AccountNumber &&
                            view.Destination == registerData.Header.TargetOkato &&
                            view.Source == registerData.Header.SourceOkato &&
                            view.ExternalId == registerData.Account.ExternalId &&
                            view.Type == (int)AccountType.CorrectedPart &&
                            Sql.DateDiff(Sql.DateParts.Year, view.Date, registerData.Header.Date) == 0);

                if (existAccountResult.Success && existAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Счет №{0}(исправленная часть) от территории {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, existAccountResult.Data?.FirstOrDefault()?.SourceName, existAccountResult.Data?.FirstOrDefault()?.Date));
                    return result;
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    result.AddError(string.Format("В счете №{0}(исправленная часть) от территории {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, existAccountResult.Data?.FirstOrDefault()?.SourceName));
                    return result;
                }

                //Поиск основной части
                var parentAccountResult = _repository.GetTerritoryAccountView(
                    p => p.AccountNumber == registerData.Account.AccountNumber &&
                    p.Destination == registerData.Header.TargetOkato &&
                    p.Date == new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1) &&
                    p.Type == (int)AccountType.GeneralPart);
                if (parentAccountResult.HasError || parentAccountResult.Data.IsNull() || !parentAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Для счета №{0}(исправленная часть) от территории {1} не найдена основная часть.",
                                                    registerData.Account.AccountNumber, existAccountResult.Data?.FirstOrDefault()?.SourceName));
                    return result;
                }

                var parentAccount = parentAccountResult.Data.First();

                int direction = GetDirectionByOkato(registerData.Header.TargetOkato);
                var account = new FactTerritoryAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = registerData.Account.AccountNumber,
                    AccountDate = registerData.Account.AccountDate,
                    Price = registerData.Account.Price,
                    AcceptPrice = registerData.Account.AcceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    Type = AccountType.CorrectedPart.ToInt32(),
                    Destination = registerData.Header.TargetOkato,
                    Source = registerData.Header.SourceOkato,
                    Comments = registerData.Account.Comments,
                    ExternalId = registerData.Account.ExternalId,
                    Direction = direction,
                    Parent = parentAccount.TerritoryAccountId,
                    PacketNumber = packetNumber,
                    Version = registerData.Header.Version
                };

                var total = registerData.RecordsCollection.Count;
                var lastPatientCount = 0;
                var patients = new List<Tuple<FactPatient, FactPerson, FactDocument, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>>>();
                foreach (v30.E.RecordsE entry in registerData.RecordsCollection)
                {
                    FactPerson personDb = null;
                    FactDocument documentDb = null;

                    int personId = 0;
                    int? documentId = default(int?);

                    var personCheckResult = _repository.GetPerson(p => p.PName == entry.Patient.PName &&
                                p.Surname == entry.Patient.Surname &&
                                p.Patronymic == entry.Patient.Patronymic &&
                                p.Birthday == entry.Patient.Birthday &&
                                p.RepresentativeSurname == (entry.Patient.RepresentativeSurname ?? "") &&
                                p.RepresentativeName == (entry.Patient.RepresentativeName ?? "") &&
                                p.RepresentativePatronymic == (entry.Patient.RepresentativePatronymic ?? "") &&
                                p.RepresentativeBirthday == entry.Patient.RepresentativeBirthday);

                    if (personCheckResult.Success)
                    {
                        if (!personCheckResult.Data.Any())
                        {
                            personDb = Map.ObjectToObject<FactPerson>(entry.Patient);
                            if (entry.Patient.DocType.HasValue)
                            {
                                documentDb = new FactDocument
                                {
                                    DocType = entry.Patient.DocType,
                                    DocNum = entry.Patient.DocNumber,
                                    DocSeries = entry.Patient.DocSeries
                                };
                            }
                        }
                        else
                        {
                            personId = personCheckResult.Data.First().PersonId;
                            var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
                                                                                p.DocType == entry.Patient.DocType &&
                                                                                p.DocSeries == entry.Patient.DocSeries);

                            if (!docCheckResult.Data.Any())
                            {
                                if (entry.Patient.DocType.HasValue)
                                {
                                    documentDb = new FactDocument
                                    {
                                        PersonId = personId,
                                        DocType = entry.Patient.DocType,
                                        DocNum = entry.Patient.DocNumber,
                                        DocSeries = entry.Patient.DocSeries
                                    };
                                }
                            }
                            else
                            {
                                documentId = docCheckResult.Data.First().DocumentId;
                            }
                        }
                    }
                    else
                    {
                        result.AddError(new Exception("Ошибка поиска персональных данных"));
                        return result;
                    }

                    var lastPatientId = entry.ExternalId;

                    FactPatient patientDb = Map.ObjectToObject<FactPatient>(entry.Patient);
                    if (personDb.IsNull())
                    {
                        patientDb.PersonalId = personId;
                    }
                    if (documentDb.IsNull())
                    {
                        patientDb.DocumentId = documentId;
                    }

                    patientDb.ExternalId = lastPatientId;

                    var mevents = new List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>();
                    foreach (v30.E.ZslEventE zslEventE in entry.EventCollection)
                    {
                        var zsleventECopy = zslEventE;
                        var zslmevent = Map.ObjectToObject<ZslFactMedicalEvent>(zsleventECopy);

                        var zevents = new List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>();
                        foreach (v30.E.EventE eventE in zslEventE.InnerMeventCollection)
                        {
                            var zeventCopy = eventE;
                            ZFactMedicalEvent zmevent = Map.ObjectToObject<ZFactMedicalEvent>(zeventCopy);

                            var slKoefs = new List<ZFactSlKoef>();
                            ZFactKsgKpg ksgKpg = null;
                            if (eventE.KsgKpgXml != null)
                            {
                                ksgKpg = Map.ObjectToObject<ZFactKsgKpg>(eventE.KsgKpgXml);

                                foreach (var slKoefD in eventE.KsgKpgXml.InnerSlCoefCollection)
                                {
                                    if (slKoefD == null)
                                        continue;
                                    var slKoef = new ZFactSlKoef
                                    {
                                        NumberDifficultyTreatment = slKoefD.NumberDifficultyTreatment,
                                        ValueDifficultyTreatment = slKoefD.ValueDifficultyTreatment
                                    };
                                    slKoefs.Add(slKoef);
                                }
                            }

                            var refusals = new List<ZFactExternalRefuse>();

                            foreach (v30.E.RefusalE refusal in eventE.InnerRefusals.Where(p => p.IsNotNull()))
                            {
                                var externalRefusal = new ZFactExternalRefuse
                                {
                                    ReasonId = refusal.RefusalCode ?? 53,
                                    Amount = refusal.RefusalRate,
                                    Source = (int)RefusalSource.Local,
                                    Comment = refusal.Comments,
                                    ExternalGuid = refusal.ExternalGuid,
                                    Type = refusal.RefusalType ?? (int)RefusalType.MEC,
                                    Generation = 0,
                                };
                                refusals.Add(externalRefusal);
                            }

                            var services = new List<ZFactMedicalServices>();
                            foreach (v30.E.ServiceE serviceE in eventE.InnerServiceCollection)
                            {
                                var serviceCopy = serviceE;
                                var service = Map.ObjectToObject<ZFactMedicalServices>(serviceCopy);
                                //Исправление специальности V015, т.к. нет в услуге указания версии справочника
                                if (eventE.SpecialityCodeV015.HasValue)
                                {
                                    service.SpecialityCodeV015 = _cache.Get(CacheRepository.V015Cache).GetBack(serviceCopy.SpecialityCodeV021Xml);
                                    service.SpecialityCodeV021 = null;
                                }
                                else
                                {
                                    service.SpecialityCodeV015 = null;
                                    service.SpecialityCodeV021 = _cache.Get(CacheRepository.V021Cache).GetBack(serviceCopy.SpecialityCodeV021Xml);
                                }
                                services.Add(service);
                            }
                            zevents.Add(Tuple.Create(zmevent, Tuple.Create(ksgKpg, slKoefs), services, refusals));
                        }
                        mevents.Add(Tuple.Create(zslmevent, zevents));
                    }

                    patients.Add(Tuple.Create(patientDb, personDb, documentDb, mevents));

                    lastPatientCount++;
                    _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                }

                var insertRegisterResult = _repository.InsertRegisterE(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<int> LoadBackZ(v31.E.RegisterE registerData, int packetNumber, bool isTestLoad, ProcessingOperations operations)
        {
            var result = new OperationResult<int>();
            try
            {
                // IEnumerable<Tuple<FactPatient, FactPerson, FactDocument, IEnumerable<Tuple<FactMedicalEvent, IEnumerable<FactMedicalServices>, IEnumerable<FactMEC>, IEnumerable<FactMEE>, IEnumerable<FactEQMA>>>>> data = null;

                string territoryName = string.Empty;

                //Проверяем дублирование счета
                var existAccountResult = _repository.GetTerritoryAccountView(
                    view => view.AccountNumber == registerData.Account.AccountNumber &&
                            view.Destination == registerData.Header.TargetOkato &&
                            view.Source == registerData.Header.SourceOkato &&
                            view.ExternalId == registerData.Account.ExternalId &&
                            view.Type == (int)AccountType.CorrectedPart &&
                            Sql.DateDiff(Sql.DateParts.Year, view.Date, registerData.Header.Date) == 0);

                if (existAccountResult.Success && existAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Счет №{0}(исправленная часть) от территории {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, existAccountResult.Data?.FirstOrDefault()?.SourceName, existAccountResult.Data?.FirstOrDefault()?.Date));
                    return result;
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    result.AddError(string.Format("В счете №{0}(исправленная часть) от территории {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, existAccountResult.Data?.FirstOrDefault()?.SourceName));
                    return result;
                }

                //Поиск основной части
                var parentAccountResult = _repository.GetTerritoryAccountView(
                    p => p.AccountNumber == registerData.Account.AccountNumber &&
                    p.Destination == registerData.Header.TargetOkato &&
                    p.Date == new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1) &&
                    p.Type == (int)AccountType.GeneralPart);
                if (parentAccountResult.HasError || parentAccountResult.Data.IsNull() || !parentAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Для счета №{0}(исправленная часть) от территории {1} не найдена основная часть.",
                                                    registerData.Account.AccountNumber, existAccountResult.Data?.FirstOrDefault()?.SourceName));
                    return result;
                }

                var parentAccount = parentAccountResult.Data.First();

                int direct = GetDirectionByOkato(registerData.Header.TargetOkato);
                var account = new FactTerritoryAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = registerData.Account.AccountNumber,
                    AccountDate = registerData.Account.AccountDate,
                    Price = registerData.Account.Price,
                    AcceptPrice = registerData.Account.AcceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    Type = AccountType.CorrectedPart.ToInt32(),
                    Destination = registerData.Header.TargetOkato,
                    Source = registerData.Header.SourceOkato,
                    Comments = registerData.Account.Comments,
                    ExternalId = registerData.Account.ExternalId,
                    Direction = direct,
                    Parent = parentAccount.TerritoryAccountId,
                    PacketNumber = packetNumber,
                    Version = registerData.Header.Version
                };

                var total = registerData.RecordsCollection.Count;
                var lastPatientCount = 0;
                var patients = new List<Tuple<FactPatient, FactPerson, FactDocument,
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
                            >>>>();
                foreach (v31.E.RecordsE entry in registerData.RecordsCollection)
                {
                    FactPerson personDb = null;
                    FactDocument documentDb = null;

                    int personId = 0;
                    int? documentId = default(int?);

                    var personCheckResult = _repository.GetPerson(p => p.PName == entry.Patient.PName &&
                                p.Surname == entry.Patient.Surname &&
                                p.Patronymic == entry.Patient.Patronymic &&
                                p.Birthday == entry.Patient.Birthday &&
                                p.RepresentativeSurname == (entry.Patient.RepresentativeSurname ?? "") &&
                                p.RepresentativeName == (entry.Patient.RepresentativeName ?? "") &&
                                p.RepresentativePatronymic == (entry.Patient.RepresentativePatronymic ?? "") &&
                                p.RepresentativeBirthday == entry.Patient.RepresentativeBirthday);

                    if (personCheckResult.Success)
                    {
                        if (!personCheckResult.Data.Any())
                        {
                            personDb = Map.ObjectToObject<FactPerson>(entry.Patient);
                            if (entry.Patient.DocType.HasValue)
                            {
                                documentDb = new FactDocument
                                {
                                    DocType = entry.Patient.DocType,
                                    DocNum = entry.Patient.DocNumber,
                                    DocSeries = entry.Patient.DocSeries
                                };
                            }
                        }
                        else
                        {
                            personId = personCheckResult.Data.First().PersonId;
                            var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
                                                                                p.DocType == entry.Patient.DocType &&
                                                                                p.DocSeries == entry.Patient.DocSeries);

                            if (!docCheckResult.Data.Any())
                            {
                                if (entry.Patient.DocType.HasValue)
                                {
                                    documentDb = new FactDocument
                                    {
                                        PersonId = personId,
                                        DocType = entry.Patient.DocType,
                                        DocNum = entry.Patient.DocNumber,
                                        DocSeries = entry.Patient.DocSeries
                                    };
                                }
                            }
                            else
                            {
                                documentId = docCheckResult.Data.First().DocumentId;
                            }
                        }
                    }
                    else
                    {
                        result.AddError(new Exception("Ошибка поиска персональных данных"));
                        return result;
                    }

                    var lastPatientId = entry.ExternalId;

                    FactPatient patientDb = Map.ObjectToObject<FactPatient>(entry.Patient);
                    if (personDb.IsNull())
                    {
                        patientDb.PersonalId = personId;
                    }
                    if (documentDb.IsNull())
                    {
                        patientDb.DocumentId = documentId;
                    }

                    patientDb.ExternalId = lastPatientId;

                    //var mevents = new List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>();
                    var mevents = new List<Tuple<ZslFactMedicalEvent,
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
                        >>();
                    foreach (v31.E.ZslEventE zslEventE in entry.EventCollection)
                    {
                        var zsleventECopy = zslEventE;
                        var zslmevent = Map.ObjectToObject<ZslFactMedicalEvent>(zsleventECopy);

                        //var zevents = new List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>();
                        var zevents = new List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            List<ZFactDs>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>();
                        int count = 0;
                        string externalId = string.Empty;
                        foreach (v31.E.EventE eventE in zslEventE.InnerMeventCollection)
                        {
                            var zeventCopy = eventE;
                            ZFactMedicalEvent zmevent = Map.ObjectToObject<ZFactMedicalEvent>(zeventCopy);
                            if (count == 0)
                            {
                                externalId = zeventCopy.ExternalId;
                            }

                            count++;
                            var directions = new List<ZFactDirection>();
                            foreach (v31.E.DirectionOnkE direc in eventE.DirectionOnkXml)
                            {
                                if (direc == null) continue;
                                var direction = new ZFactDirection
                                {
                                    DirectionDate = direc.DirectionDate,
                                    DirectionMo = direc.DirectionMo,
                                    DirectionViewId = direc.DirectionViewId,
                                    MetIsslId = direc.MetIsslId,
                                    DirectionService = direc.DirectionService
                                };
                                directions.Add(direction);
                            }

                            var сonsultations = new List<ZFactConsultations>();
                            foreach (v31.E.ConsultationsOnkE cons in eventE.ConsultationsOnkXml)
                            {
                                if (cons == null) continue;
                                var сonsultation = new ZFactConsultations
                                {
                                    DtCons = cons.DtCons,
                                    PrCons = cons.PrCons
                                };
                                сonsultations.Add(сonsultation);
                            }

                            var ds2 = new List<ZFactDs>();
                            foreach (string ds in eventE.DiagnosisSecondaryXml)
                            {
                                if (ds == null) continue;
                                var factDs = new ZFactDs
                                {
                                    Ds = ds,
                                    DsType = 2
                                };
                                ds2.Add(factDs);
                            }

                            var slKoefs = new List<ZFactSlKoef>();
                            var critList = new List<ZFactCrit>();
                            ZFactKsgKpg ksgKpg = null;
                            if (eventE.KsgKpgXml != null)
                            {
                                ksgKpg = Map.ObjectToObject<ZFactKsgKpg>(eventE.KsgKpgXml);
                                foreach (var i in (eventE.KsgKpgXml.CritXml))
                                {
                                    critList.Add(new ZFactCrit { IdDkk = i });
                                }

                                foreach (var slKoefD in eventE.KsgKpgXml.InnerSlCoefCollection)
                                {
                                    if (slKoefD == null)
                                        continue;
                                    var slKoef = new ZFactSlKoef
                                    {
                                        NumberDifficultyTreatment = slKoefD.NumberDifficultyTreatment,
                                        ValueDifficultyTreatment = slKoefD.ValueDifficultyTreatment
                                    };
                                    slKoefs.Add(slKoef);
                                }
                            }

                            var medicalEventOnk =
                                new Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>,
                                    List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>(null, null, null, null);
                            if (eventE.EventOnk != null)
                            {
                                var diagBloks = new List<ZFactDiagBlok>();
                                var contraindications = new List<ZFactContraindications>();
                                var serviceUslOnk = new List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>();

                                foreach (var diagBlokOnk in eventE.EventOnk.DiagBlokOnk)
                                {
                                    if (diagBlokOnk == null)
                                        continue;
                                    diagBloks.Add(Map.ObjectToObject<ZFactDiagBlok>(diagBlokOnk));
                                }
                                foreach (var сontraindicationsOnk in eventE.EventOnk.СontraindicationsOnk)
                                {
                                    if (сontraindicationsOnk == null)
                                        continue;
                                    contraindications.Add(Map.ObjectToObject<ZFactContraindications>(сontraindicationsOnk));
                                }
                                foreach (var serviceOnk in eventE.EventOnk.ServiceOnk)
                                {
                                    if (serviceOnk == null)
                                        continue;
                                    var anticancerDrugOnkDs = new List<ZFactAnticancerDrug>();
                                    foreach (var anticancerDrugOnkD in serviceOnk.AnticancerDrugOnkXml)
                                    {
                                        if (anticancerDrugOnkD == null)
                                            continue;
                                        anticancerDrugOnkDs.Add(Map.ObjectToObject<ZFactAnticancerDrug>(anticancerDrugOnkD));

                                    }
                                    serviceUslOnk.Add(Tuple.Create(Map.ObjectToObject<ZFactMedicalServicesOnk>(serviceOnk), anticancerDrugOnkDs));
                                }

                                medicalEventOnk = Tuple.Create(Map.ObjectToObject<ZFactMedicalEventOnk>(eventE.EventOnk), diagBloks, contraindications, serviceUslOnk);
                            }

                            var services = new List<ZFactMedicalServices>();
                            foreach (v31.E.ServiceE serviceE in eventE.InnerServiceCollection)
                            {
                                var serviceCopy = serviceE;
                                var service = Map.ObjectToObject<ZFactMedicalServices>(serviceCopy);
                                //Исправление специальности V015, т.к. нет в услуге указания версии справочника
                                if (eventE.SpecialityCodeV015.HasValue)
                                {
                                    service.SpecialityCodeV015 = _cache.Get(CacheRepository.V015Cache).GetBack(serviceCopy.SpecialityCodeV021Xml);
                                    service.SpecialityCodeV021 = null;
                                }
                                else
                                {
                                    service.SpecialityCodeV015 = null;
                                    service.SpecialityCodeV021 = _cache.Get(CacheRepository.V021Cache).GetBack(serviceCopy.SpecialityCodeV021Xml);
                                }
                                services.Add(service);
                            }
                            zevents.Add(Tuple.Create(zmevent, directions, сonsultations, ds2, medicalEventOnk, Tuple.Create(ksgKpg, critList, slKoefs), services));
                        }

                        var refusals = new List<ZFactExternalRefuse>();

                        foreach (v31.E.RefusalE refusal in zslEventE.RefusalsXml.Where(p => p.IsNotNull()))
                        {
                            var externalRefusal = new ZFactExternalRefuse
                            {
                                ReasonId = refusal.RefusalCode,
                                Amount = refusal.RefusalRate,
                                Source = refusal.RefusalSource,
                                Comment = refusal.Comments,
                                SlidGuid = refusal.SlidGuid??externalId,
                                ExternalGuid = refusal.ExternalGuid,
                                CodeExp = refusal.CodeExp,
                                Date = refusal.Date,
                                NumAct = refusal.NumAct,
                                Type = refusal.RefusalType,
                                Generation = 0,
                            };
                            refusals.Add(externalRefusal);
                        }
                        mevents.Add(Tuple.Create(zslmevent, zevents, refusals));
                    }

                    patients.Add(Tuple.Create(patientDb, personDb, documentDb, mevents));

                    lastPatientCount++;
                    _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                }
               
                var insertRegisterResult = _repository.InsertRegisterE(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<int> LoadBackZ(v32.E.RegisterE registerData, int packetNumber, bool isTestLoad, ProcessingOperations operations)
        {
            var result = new OperationResult<int>();
            try
            {
                // IEnumerable<Tuple<FactPatient, FactPerson, FactDocument, IEnumerable<Tuple<FactMedicalEvent, IEnumerable<FactMedicalServices>, IEnumerable<FactMEC>, IEnumerable<FactMEE>, IEnumerable<FactEQMA>>>>> data = null;

                string territoryName = string.Empty;

                //Проверяем дублирование счета
                var existAccountResult = _repository.GetTerritoryAccountView(
                    view => view.AccountNumber == registerData.Account.AccountNumber &&
                            view.Destination == registerData.Header.TargetOkato &&
                            view.Source == registerData.Header.SourceOkato &&
                            view.ExternalId == registerData.Account.ExternalId &&
                            view.Type == (int)AccountType.CorrectedPart &&
                            Sql.DateDiff(Sql.DateParts.Year, view.Date, registerData.Header.Date) == 0);

                if (existAccountResult.Success && existAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Счет №{0}(исправленная часть) от территории {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, existAccountResult.Data?.FirstOrDefault()?.SourceName, existAccountResult.Data?.FirstOrDefault()?.Date));
                    return result;
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    result.AddError(string.Format("В счете №{0}(исправленная часть) от территории {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, existAccountResult.Data?.FirstOrDefault()?.SourceName));
                    return result;
                }

                //Поиск основной части
                var parentAccountResult = _repository.GetTerritoryAccountView(
                    p => p.AccountNumber == registerData.Account.AccountNumber &&
                    p.Destination == registerData.Header.TargetOkato &&
                    p.Date == new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1) &&
                    p.Type == (int)AccountType.GeneralPart);
                if (parentAccountResult.HasError || parentAccountResult.Data.IsNull() || !parentAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Для счета №{0}(исправленная часть) от территории {1} не найдена основная часть.",
                                                    registerData.Account.AccountNumber, existAccountResult.Data?.FirstOrDefault()?.SourceName));
                    return result;
                }

                var parentAccount = parentAccountResult.Data.First();

                int direct = GetDirectionByOkato(registerData.Header.TargetOkato);
                var account = new FactTerritoryAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = registerData.Account.AccountNumber,
                    AccountDate = registerData.Account.AccountDate,
                    Price = registerData.Account.Price,
                    AcceptPrice = registerData.Account.AcceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    Type = AccountType.CorrectedPart.ToInt32(),
                    Destination = registerData.Header.TargetOkato,
                    Source = registerData.Header.SourceOkato,
                    Comments = registerData.Account.Comments,
                    ExternalId = registerData.Account.ExternalId,
                    Direction = direct,
                    Parent = parentAccount.TerritoryAccountId,
                    PacketNumber = packetNumber,
                    Version = registerData.Header.Version
                };

                var total = registerData.RecordsCollection.Count;
                var lastPatientCount = 0;
                var patients = new List<Tuple<FactPatient, FactPerson, FactDocument,
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
                            >>>>();
                foreach (v32.E.RecordsE entry in registerData.RecordsCollection)
                {
                    FactPerson personDb = null;
                    FactDocument documentDb = null;

                    int personId = 0;
                    int? documentId = default(int?);

                    var personCheckResult = _repository.GetPerson(p => p.PName == entry.Patient.PName &&
                                p.Surname == entry.Patient.Surname &&
                                p.Patronymic == entry.Patient.Patronymic &&
                                p.Birthday == entry.Patient.Birthday &&
                                p.RepresentativeSurname == (entry.Patient.RepresentativeSurname ?? "") &&
                                p.RepresentativeName == (entry.Patient.RepresentativeName ?? "") &&
                                p.RepresentativePatronymic == (entry.Patient.RepresentativePatronymic ?? "") &&
                                p.RepresentativeBirthday == entry.Patient.RepresentativeBirthday);

                    if (personCheckResult.Success)
                    {
                        if (!personCheckResult.Data.Any())
                        {
                            personDb = Map.ObjectToObject<FactPerson>(entry.Patient);
                            if (entry.Patient.DocType.HasValue)
                            {
                                documentDb = new FactDocument
                                {
                                    DocType = entry.Patient.DocType,
                                    DocNum = entry.Patient.DocNumber,
                                    DocSeries = entry.Patient.DocSeries,
                                    DocDate = entry.Patient.DocDate,
                                    DocOrg = entry.Patient.DocOrg,
                                };
                            }
                        }
                        else
                        {
                            personId = personCheckResult.Data.First().PersonId;
                            var docCheckResult = _repository.GetDocument(p => p.PersonId == personId &&
                                                                                p.DocType == entry.Patient.DocType &&
                                                                                p.DocSeries == entry.Patient.DocSeries);

                            if (!docCheckResult.Data.Any())
                            {
                                if (entry.Patient.DocType.HasValue)
                                {
                                    documentDb = new FactDocument
                                    {
                                        PersonId = personId,
                                        DocType = entry.Patient.DocType,
                                        DocNum = entry.Patient.DocNumber,
                                        DocSeries = entry.Patient.DocSeries,
                                        DocDate = entry.Patient.DocDate,
                                        DocOrg = entry.Patient.DocOrg,
                                    };
                                }
                            }
                            else
                            {
                                documentId = docCheckResult.Data.First().DocumentId;
                            }
                        }
                    }
                    else
                    {
                        result.AddError(new Exception("Ошибка поиска персональных данных"));
                        return result;
                    }

                    var lastPatientId = entry.ExternalId;

                    FactPatient patientDb = Map.ObjectToObject<FactPatient>(entry.Patient);
                    if (personDb.IsNull())
                    {
                        patientDb.PersonalId = personId;
                    }
                    if (documentDb.IsNull())
                    {
                        patientDb.DocumentId = documentId;
                    }

                    patientDb.ExternalId = lastPatientId;

                    //var mevents = new List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>();
                    var mevents = new List<Tuple<ZslFactMedicalEvent,
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
                        >>();
                    foreach (v32.E.ZslEventE zslEventE in entry.EventCollection)
                    {
                        var zsleventECopy = zslEventE;
                        var zslmevent = Map.ObjectToObject<ZslFactMedicalEvent>(zsleventECopy);

                        //var zevents = new List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>();
                        var zevents = new List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            List<ZFactDs>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>();
                        int count = 0;
                        string externalId = string.Empty;
                        foreach (v32.E.EventE eventE in zslEventE.InnerMeventCollection)
                        {
                            var zeventCopy = eventE;
                            ZFactMedicalEvent zmevent = Map.ObjectToObject<ZFactMedicalEvent>(zeventCopy);
                            if (count == 0)
                            {
                                externalId = zeventCopy.ExternalId;
                            }

                            count++;
                            var directions = new List<ZFactDirection>();
                            foreach (v32.E.DirectionOnkE direc in eventE.DirectionOnkXml)
                            {
                                if (direc == null) continue;
                                var direction = new ZFactDirection
                                {
                                    DirectionDate = direc.DirectionDate,
                                    DirectionMo = direc.DirectionMo,
                                    DirectionViewId = direc.DirectionViewId,
                                    MetIsslId = direc.MetIsslId,
                                    DirectionService = direc.DirectionService
                                };
                                directions.Add(direction);
                            }

                            var сonsultations = new List<ZFactConsultations>();
                            foreach (v32.E.ConsultationsOnkE cons in eventE.ConsultationsOnkXml)
                            {
                                if (cons == null) continue;
                                var сonsultation = new ZFactConsultations
                                {
                                    DtCons = cons.DtCons,
                                    PrCons = cons.PrCons
                                };
                                сonsultations.Add(сonsultation);
                            }

                            var ds2 = new List<ZFactDs>();
                            foreach (string ds in eventE.DiagnosisSecondaryXml)
                            {
                                if (ds == null) continue;
                                var factDs = new ZFactDs
                                {
                                    Ds = ds,
                                    DsType = 2
                                };
                                ds2.Add(factDs);
                            }


                            var slKoefs = new List<ZFactSlKoef>();
                            var critList = new List<ZFactCrit>();
                            ZFactKsgKpg ksgKpg = null;
                            if (eventE.KsgKpgXml != null)
                            {
                                ksgKpg = Map.ObjectToObject<ZFactKsgKpg>(eventE.KsgKpgXml);
                                foreach (var i in (eventE.KsgKpgXml.CritXml))
                                {
                                    critList.Add(new ZFactCrit { IdDkk = i });
                                }

                                foreach (var slKoefD in eventE.KsgKpgXml.InnerSlCoefCollection)
                                {
                                    if (slKoefD == null)
                                        continue;
                                    var slKoef = new ZFactSlKoef
                                    {
                                        NumberDifficultyTreatment = slKoefD.NumberDifficultyTreatment,
                                        ValueDifficultyTreatment = slKoefD.ValueDifficultyTreatment
                                    };
                                    slKoefs.Add(slKoef);
                                }
                            }

                            var medicalEventOnk =
                                new Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>,
                                    List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>(null, null, null, null);
                            if (eventE.EventOnk != null)
                            {
                                var diagBloks = new List<ZFactDiagBlok>();
                                var contraindications = new List<ZFactContraindications>();
                                var serviceUslOnk = new List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>();

                                foreach (var diagBlokOnk in eventE.EventOnk.DiagBlokOnk)
                                {
                                    if (diagBlokOnk == null)
                                        continue;
                                    diagBloks.Add(Map.ObjectToObject<ZFactDiagBlok>(diagBlokOnk));
                                }
                                foreach (var сontraindicationsOnk in eventE.EventOnk.СontraindicationsOnk)
                                {
                                    if (сontraindicationsOnk == null)
                                        continue;
                                    contraindications.Add(Map.ObjectToObject<ZFactContraindications>(сontraindicationsOnk));
                                }
                                foreach (var serviceOnk in eventE.EventOnk.ServiceOnk)
                                {
                                    if (serviceOnk == null)
                                        continue;
                                    var anticancerDrugOnkDs = new List<ZFactAnticancerDrug>();
                                    foreach (var anticancerDrugOnkD in serviceOnk.AnticancerDrugOnkXml)
                                    {
                                        if (anticancerDrugOnkD == null)
                                            continue;
                                        anticancerDrugOnkDs.Add(Map.ObjectToObject<ZFactAnticancerDrug>(anticancerDrugOnkD));

                                    }
                                    serviceUslOnk.Add(Tuple.Create(Map.ObjectToObject<ZFactMedicalServicesOnk>(serviceOnk), anticancerDrugOnkDs));
                                }

                                medicalEventOnk = Tuple.Create(Map.ObjectToObject<ZFactMedicalEventOnk>(eventE.EventOnk), diagBloks, contraindications, serviceUslOnk);
                            }

                            var services = new List<ZFactMedicalServices>();
                            foreach (v32.E.ServiceE serviceE in eventE.InnerServiceCollection)
                            {
                                var serviceCopy = serviceE;
                                var service = Map.ObjectToObject<ZFactMedicalServices>(serviceCopy);
                                //Исправление специальности V015, т.к. нет в услуге указания версии справочника
                                if (eventE.SpecialityCodeV015.HasValue)
                                {
                                    service.SpecialityCodeV015 = _cache.Get(CacheRepository.V015Cache).GetBack(serviceCopy.SpecialityCodeV021Xml);
                                    service.SpecialityCodeV021 = null;
                                }
                                else
                                {
                                    service.SpecialityCodeV015 = null;
                                    service.SpecialityCodeV021 = _cache.Get(CacheRepository.V021Cache).GetBack(serviceCopy.SpecialityCodeV021Xml);
                                }
                                services.Add(service);
                            }
                            zevents.Add(Tuple.Create(zmevent, directions, сonsultations, ds2, medicalEventOnk, Tuple.Create(ksgKpg, critList, slKoefs), services));
                        }

                        var refusals = new List<ZFactExternalRefuse>();

                        foreach (v32.E.RefusalE refusal in zslEventE.RefusalsXml.Where(p => p.IsNotNull()))
                        {
                            var externalRefusal = new ZFactExternalRefuse
                            {
                                ReasonId = refusal.RefusalCode,
                                Amount = refusal.RefusalRate,
                                Source = refusal.RefusalSource,
                                Comment = refusal.Comments,
                                SlidGuid = refusal.SlidGuid??externalId,
                                ExternalGuid = refusal.ExternalGuid,
                                CodeExp = refusal.CodeExp,
                                Date = refusal.Date,
                                NumAct = refusal.NumAct,
                                Type = refusal.RefusalType,
                                Generation = 0,
                            };
                            refusals.Add(externalRefusal);
                        }
                        mevents.Add(Tuple.Create(zslmevent, zevents, refusals));
                    }

                    patients.Add(Tuple.Create(patientDb, personDb, documentDb, mevents));

                    lastPatientCount++;
                    _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                }

                var insertRegisterResult = _repository.InsertRegisterE(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<int> LoadAnswer(v21.EAnswer.RegisterEAnswer registerData, RegisterEInfo info, bool isTestLoad, ProcessingOperations operations)
        {
            var result = new OperationResult<int>();
            try
            {
                IEnumerable<Tuple<FactPatient, FactPerson, FactDocument, IEnumerable<Tuple<FactMedicalEvent, IEnumerable<FactMedicalServices>, IEnumerable<FactMEC>, IEnumerable<FactMEE>, IEnumerable<FactEQMA>>>>> data = null;

                string sourceTerritoryOkato = info.SourceTerritory.ToString(CultureInfo.InvariantCulture);
                
                //Проверяем дублирование счета
                var isExistExchange = _repository.GetFactExchange(
                    p => p.FileName == Path.GetFileNameWithoutExtension(info.FileName) &&
                    p.Destination == info.DestinationTerritory.ToString(CultureInfo.InvariantCulture) &&
                    p.Source == info.SourceTerritory.ToString(CultureInfo.InvariantCulture) &&
                    p.Type == (int) AccountType.LogPart &&
                    p.PacketNumber == info.NumberXXXX);
                if (isExistExchange.HasError)
                {
                    result.AddError(string.Format("Ошибка поиска счета №{0}(протокол обработки) от территории {1} отчетная дата {2:MMMM YYYY}.",
                                                registerData.Account.AccountNumber, sourceTerritoryOkato, registerData.Header.Date));
                    return result;
                }

                if (isExistExchange.Success && isExistExchange.Data.Any())
                {
                    result.AddError(string.Format("Счет №{0}(протокол обработки) от территории {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, sourceTerritoryOkato, registerData.Header.Date));
                    return result;
                }             

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    result.AddError(string.Format("В счете №{0}(протокол обработки) от территории {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, sourceTerritoryOkato));
                    return result;
                }

                //Поиск основной части
                var parentAccountResult = _repository.GetTerritoryAccountView(
                    p => p.AccountNumber == registerData.Account.AccountNumber &&
                    p.Destination == registerData.Header.TargetOkato &&
                    p.Date == new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1) &&
                    p.Type == (int)AccountType.GeneralPart);
                if (parentAccountResult.HasError || parentAccountResult.Data.IsNull() || !parentAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Для счета №{0}(протокол обработки) от территории {1} не найдена основная часть.",
                                                    registerData.Account.AccountNumber, sourceTerritoryOkato));
                    return result;
                }

                var parentAccount = parentAccountResult.Data.First();

                var accountGenerationResult = _repository.GetTerritoryAccountGenerationByParentId(parentAccount.TerritoryAccountId);
                if (accountGenerationResult.HasError)
                {
                    result.AddError(string.Format("Для счета №{0}(протокол обработки) от территории {1} не определено поколение счета.",
                                                    registerData.Account.AccountNumber, sourceTerritoryOkato));
                    return result;
                }

                var accountGeneration = accountGenerationResult.Data;
                decimal? price = registerData.RecordsCollection.Select(p => p.EventCollection.Where(r => r.PaymentStatus != 2).Sum(r => r.Price)).Sum(p => p);
                decimal? acceptPrice = registerData.RecordsCollection.Select(p => p.EventCollection.Where(r => r.PaymentStatus != 2).Sum(r => r.AcceptPrice)).Sum(p => p);



                int direction = GetDirectionByOkato(registerData.Header.TargetOkato);
                var account = new FactTerritoryAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = parentAccount.AccountNumber,
                    AccountDate = DateTime.Now,
                    Price = price,
                    AcceptPrice = acceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    Type = AccountType.CorrectedPart.ToInt32(),
                    Destination = registerData.Header.TargetOkato,
                    Source = parentAccount.Source,
                    ExternalId = null,
                    Direction = direction,
                    Parent = parentAccount.TerritoryAccountId,
                    Generation = accountGeneration,
                    PacketNumber = info.NumberXXXX,
                    Version = registerData.Header.Version
                };

                var total = registerData.RecordsCollection.Count;
                var lastPatientCount = 0;
                var patients = new List<Tuple<FactPatient, List<Tuple<FactMedicalEvent, List<FactMedicalServices>, List<FactExternalRefuse>>>>>();
                foreach (v21.EAnswer.RecordsEAnswer entry in registerData.RecordsCollection)
                {
                    if (entry.EventCollection.Count == 0)
                    {
                        result.AddError(string.Format("Не найдены случаи у пациента с ID = {0} счет № {1}", entry.ExternalId, parentAccount.AccountNumber)); 
                        return result;
                    }

                    var patientsResult = _repository.GetPatientByMeventExternalIdAndAccountId(entry.EventCollection.First().ExternalId, parentAccount.TerritoryAccountId);
                    if (patientsResult.HasError || patientsResult.Data.IsNull() || !patientsResult.Data.Any())
                    {
                        result.AddError(string.Format("Не найден пациент с ID = {0} счет № {1}", entry.ExternalId, parentAccount.AccountNumber));
                        return result;
                    }

                    var tmpPatient = patientsResult.Data.First();

                    var patientsDuplicate = patients.FirstOrDefault(p => p.Item1.ExternalId == tmpPatient.ExternalId);

                    FactPatient patientDb = null;

                    if (patientsDuplicate.IsNull())
                    {
                        if (entry.EventCollection.Any(p => 
                            p.PaymentStatus != 2 && 
                            p.RefusalsXml.Any(r => r.RefusalSource != (int)RefusalSource.Local || r.RefusalSource != (int)RefusalSource.LocalCorrected)))
                        {
                            patientDb = tmpPatient;
                            patientDb.AccountId = account.TerritoryAccountId;
                            patientDb.PatientId = 0;
                            patientDb.InsuranceDocNumber = entry.Patient.InsuranceDocNumber;
                            patientDb.InsuranceDocSeries = entry.Patient.InsuranceDocSeries;
                            patientDb.INP = entry.Patient.INP;
                            patientDb.InsuranceDocType = entry.Patient.InsuranceDocType;
                        }
                        else
                        {

                            continue;
                        }
                    }
                    else
                    {
                        patientDb = patientsDuplicate.Item1;
                    }

                    var mevents = new List<Tuple<FactMedicalEvent, List<FactMedicalServices>, List<FactExternalRefuse>>>();
                    foreach (DataCore.v21.EAnswer.EventEAnswer eventE in entry.EventCollection)
                    {
                        //Полная оплата случая
                        if (eventE.PaymentStatus == 2)
                        {
                            //Обновляем статус оплаты у случая в родительском счете
                            var setResult = _repository.SetMedicalAsFullPaymentByExternalIdAndAccountId(eventE.ExternalId, parentAccount.TerritoryAccountId);
                            if (setResult.HasError)
                            {
                                result.AddError(string.Format("Ошибка обновления статуса оплаты в родительском счете ID {0}.\r\n{1}", parentAccount.AccountNumber, setResult.LastError));
                                return result;
                            }

                        }
                        //Случай частично отказан или полностью
                        else if (eventE.PaymentStatus == 3 || eventE.PaymentStatus == 4)
                        {
                            //Все отказы ТФОМС1->МО не грузим
                            if (eventE.RefusalsXml.All(p => p.RefusalSource == (int)RefusalSource.Local || p.RefusalSource == (int)RefusalSource.LocalCorrected))
                            {
                                continue;
                            }

                            //Обновляем статус оплаты у случая в родительском счете
                            var setResult = _repository.SetMedicalEventPaymentStatusByExternalIdAndAccountId(eventE.ExternalId, parentAccount.TerritoryAccountId, eventE.PaymentStatus);
                            if (setResult.HasError)
                            {
                                result.AddError(string.Format("Ошибка обновления статуса оплаты в родительском счете ID {0}.\r\n{1}", parentAccount.AccountNumber, setResult.LastError));
                                return result;
                            }

                            var eventsResult = _repository.GetEventByExternalIdAndAccountId(eventE.ExternalId, parentAccount.TerritoryAccountId);
                            if (eventsResult.HasError || eventsResult.Data.IsNull())
                            {
                                result.AddError(string.Format("Не найден случай МП ID = {0} счет № {1}", eventE.ExternalId, parentAccount.AccountNumber));
                                return result;
                            }
                            
                            var mevent = eventsResult.Data;
                            var eventId = mevent.MedicalEventId;

                            mevent.History = eventE.History;
                            mevent.PaymentMethod = eventE.PaymentMethod;
                            mevent.Quantity = eventE.Quantity;
                            mevent.Price = eventE.Price;
                            mevent.AcceptPrice = eventE.AcceptPrice;
                            mevent.PaymentStatus = eventE.PaymentStatus;
                            mevent.Comments = eventE.Comments;
                            mevent.MedicalEventId = 0;

                            var services = new List<FactMedicalServices>();

                            var servicesResult = _repository.GetMedicalService(p=>p.MedicalEventId == eventId);
                            if (servicesResult.Success && servicesResult.Data.Any())
                            {
                                foreach (var mservice in servicesResult.Data)
                                {
                                    var serviceCopy = mservice;
                                    var service = Map.ObjectToObject<FactMedicalServices>(serviceCopy);
                                    services.Add(service);
                                }
                            }

                            var refusals = new List<FactExternalRefuse>();

                            foreach (v21.E.RefusalE refusal in eventE.InnerRefusals.Where(p => p.IsNotNull()))
                            {
                                var externalRefusal = new FactExternalRefuse
                                {
                                    ReasonId = refusal.RefusalCode ?? 53,
                                    Amount = refusal.RefusalRate,
                                    Source = refusal.RefusalSource,
                                    Comment = refusal.Comments,
                                    ExternalGuid = refusal.ExternalGuid,
                                    Type = refusal.RefusalType ?? (int)RefusalType.MEC,
                                    Generation = 0,
                                };
                                refusals.Add(externalRefusal);
                            }

                            mevents.Add(Tuple.Create(mevent, services, refusals));
                        }

                        
                    }

                    if (mevents.Any())
                    {
                        patients.Add(Tuple.Create(patientDb, mevents));
                    }

                    lastPatientCount++;
                    _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                }

                var insertRegisterResult = _repository.InsertRegisterEAnswer(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<int> LoadAnswer(v30.EAnswer.RegisterEAnswer registerData, RegisterEInfo info, bool isTestLoad, ProcessingOperations operations)
        {
            var result = new OperationResult<int>();
            try
            {
                //result.AddError("Импорт протокола обратотки версии 3.0 не реализован");
                //return result;
                //   IEnumerable<Tuple<FactPatient, FactPerson, FactDocument, IEnumerable<Tuple<FactMedicalEvent, IEnumerable<FactMedicalServices>, IEnumerable<FactMEC>, IEnumerable<FactMEE>, IEnumerable<FactEQMA>>>>> data = null;

                string sourceTerritoryOkato = info.SourceTerritory.ToString(CultureInfo.InvariantCulture);

                //Проверяем дублирование счета
                var isExistExchange = _repository.GetFactExchange(
                    p => p.FileName == Path.GetFileNameWithoutExtension(info.FileName) &&
                    p.Destination == info.DestinationTerritory.ToString(CultureInfo.InvariantCulture) &&
                    p.Source == info.SourceTerritory.ToString(CultureInfo.InvariantCulture) &&
                    p.Type == (int)AccountType.LogPart &&
                    p.PacketNumber == info.NumberXXXX);
                if (isExistExchange.HasError)
                {
                    result.AddError(string.Format("Ошибка поиска счета №{0}(протокол обработки) от территории {1} отчетная дата {2:MMMM YYYY}.",
                                                registerData.Account.AccountNumber, sourceTerritoryOkato, registerData.Header.Date));
                    return result;
                }

                if (isExistExchange.Success && isExistExchange.Data.Any())
                {
                    result.AddError(string.Format("Счет №{0}(протокол обработки) от территории {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, sourceTerritoryOkato, registerData.Header.Date));
                    return result;
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    result.AddError(string.Format("В счете №{0}(протокол обработки) от территории {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, sourceTerritoryOkato));
                    return result;
                }

                //Поиск основной части
                var parentAccountResult = _repository.GetTerritoryAccountView(
                    p => p.AccountNumber == registerData.Account.AccountNumber &&
                    p.Destination == registerData.Header.TargetOkato &&
                    p.Date == new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1) &&
                    p.Type == (int)AccountType.GeneralPart);
                if (parentAccountResult.HasError || parentAccountResult.Data.IsNull() || !parentAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Для счета №{0}(протокол обработки) от территории {1} не найдена основная часть.",
                                                    registerData.Account.AccountNumber, sourceTerritoryOkato));
                    return result;
                }

                var parentAccount = parentAccountResult.Data.First();

                var accountGenerationResult = _repository.GetTerritoryAccountGenerationByParentId(parentAccount.TerritoryAccountId);
                if (accountGenerationResult.HasError)
                {
                    result.AddError(string.Format("Для счета №{0}(протокол обработки) от территории {1} не определено поколение счета.",
                                                    registerData.Account.AccountNumber, sourceTerritoryOkato));
                    return result;
                }

                var accountGeneration = accountGenerationResult.Data;
                decimal? price = registerData.RecordsCollection.Select(p => p.EventCollection.Where(r => r.PaymentStatus != 2).Sum(r => r.Price)).Sum(p => p);
                decimal? acceptPrice = registerData.RecordsCollection.Select(p => p.EventCollection.Where(r => r.PaymentStatus != 2).Sum(r => r.AcceptPrice)).Sum(p => p);



                int direction = GetDirectionByOkato(registerData.Header.TargetOkato);
                var account = new FactTerritoryAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = parentAccount.AccountNumber,
                    AccountDate = DateTime.Now,
                    Price = price,
                    AcceptPrice = acceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    Type = AccountType.CorrectedPart.ToInt32(),
                    Destination = registerData.Header.TargetOkato,
                    Source = parentAccount.Source,
                    ExternalId = null,
                    Direction = direction,
                    Parent = parentAccount.TerritoryAccountId,
                    Generation = accountGeneration,
                    PacketNumber = info.NumberXXXX,
                    Version = registerData.Header.Version
                };

                var total = registerData.RecordsCollection.Count;
                var lastPatientCount = 0;
                //var patients = new List<Tuple<FactPatient, List<Tuple<ZslFactMedicalEvent, List<Tuple<List<ZFactMedicalEvent>, List<FactMedicalServices>, List<FactExternalRefuse>>>>>>>();
                var patients = new List<Tuple<FactPatient, List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>>>();
                foreach (v30.EAnswer.RecordsEAnswer entry in registerData.RecordsCollection)
                {
                    if (entry.EventCollection.Count == 0)
                    {
                        result.AddError(string.Format("Не найдены случаи у пациента с ID = {0} счет № {1}", entry.ExternalId, parentAccount.AccountNumber));
                        return result;
                    }

                    var patientsResult = _repository.GetPatientByZslMeventExternalIdAndAccountId(entry.EventCollection.First().ExternalId, parentAccount.TerritoryAccountId);
                    if (patientsResult.HasError || patientsResult.Data.IsNull() || !patientsResult.Data.Any())
                    {
                        result.AddError(string.Format("Не найден пациент с ID = {0} счет № {1}", entry.ExternalId, parentAccount.AccountNumber));
                        return result;
                    }

                    var tmpPatient = patientsResult.Data.First();

                    var patientsDuplicate = patients.FirstOrDefault(p => p.Item1.ExternalId == tmpPatient.ExternalId);

                    FactPatient patientDb = null;

                    if (patientsDuplicate.IsNull())
                    {
                        if (entry.EventCollection.Any(p =>
                            p.PaymentStatus != 2 &&
                            p.InnerEventCollection.Any(x=>x.InnerRefusals.Any(r => r.RefusalSource != (int)RefusalSource.Local || r.RefusalSource != (int)RefusalSource.LocalCorrected))))
                        {
                            patientDb = tmpPatient;
                            patientDb.AccountId = account.TerritoryAccountId;
                            patientDb.PatientId = 0;
                            patientDb.InsuranceDocNumber = entry.Patient.InsuranceDocNumber;
                            patientDb.InsuranceDocSeries = entry.Patient.InsuranceDocSeries;
                            patientDb.INP = entry.Patient.INP;
                            patientDb.InsuranceDocType = entry.Patient.InsuranceDocType;
                        }
                        else
                        {

                            continue;
                        }
                    }
                    else
                    {
                        patientDb = patientsDuplicate.Item1;
                    }

                    //Тег законченый случай
                    var zslMevents = new List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>();
                    foreach (v30.EAnswer.ZslEventEAnswer eventE in entry.EventCollection)
                    {
                        //Полная оплата случая, обновляем статус оплаты случая 
                        if (eventE.PaymentStatus == 2)
                        {
                            //Обновляем статус оплаты у случая в родительском счете
                            var setResult = _repository.SetZMedicalAsFullPaymentByExternalIdAndAccountId(eventE.ExternalId, parentAccount.TerritoryAccountId);
                            if (setResult.HasError)
                            {
                                result.AddError(string.Format("Ошибка обновления статуса оплаты в родительском счете ID {0}.\r\n{1}", parentAccount.AccountNumber, setResult.LastError));
                                return result;
                            }

                        }
                        //Случай частично отказан или полностью
                        else if (eventE.PaymentStatus == 3 || eventE.PaymentStatus == 4)
                        {
                            //Обновляем статус оплаты у законченного случая в родительском счете
                            var setResult =
                                _repository.SetZslMedicalEventPaymentStatusByExternalIdAndAccountId(eventE.ExternalId,
                                    parentAccount.TerritoryAccountId, eventE.PaymentStatus);
                            if (setResult.HasError)
                            {
                                result.AddError(
                                    string.Format(
                                        "Ошибка обновления статуса оплаты в родительском счете ID {0}.\r\n{1}",
                                        parentAccount.AccountNumber, setResult.LastError));
                                return result;
                            }

                            var zslEventsResult = _repository.GetZslEventByExternalIdAndAccountId(eventE.ExternalId,
                                parentAccount.TerritoryAccountId);
                            if (zslEventsResult.HasError || zslEventsResult.Data.IsNull())
                            {
                                result.AddError(string.Format("Не найден законченный случай МП ID = {0} счет № {1}",
                                    eventE.ExternalId, parentAccount.AccountNumber));
                                return result;
                            }

                            var zslMevent = zslEventsResult.Data;
                            var zslEventId = zslMevent.ZslMedicalEventId;
                            zslMevent.PaymentMethod = eventE.PaymentMethod;
                            zslMevent.Price = eventE.Price;
                            zslMevent.AcceptPrice = eventE.AcceptPrice;
                            zslMevent.PaymentStatus = eventE.PaymentStatus;
                            zslMevent.ZslMedicalEventId = 0;

                            //тег случай
                            var mevents = new List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>();
                            foreach (v30.EAnswer.EventEAnswer eventEAnswer in eventE.InnerEventCollection)
                            {
                                //Все отказы ТФОМС1->МО не грузим
                                if(eventEAnswer.RefusalsXml.Any())
                                if (
                                    eventEAnswer.RefusalsXml.All(
                                        p =>
                                            p.RefusalSource == (int) RefusalSource.Local ||
                                            p.RefusalSource == (int) RefusalSource.LocalCorrected))
                                {
                                    continue;
                                }
                                var eventsResult = _repository.GetZEventByExternalIdAndAccountId(eventEAnswer.ExternalId, zslEventId, parentAccount.TerritoryAccountId);
                                if (eventsResult.HasError || eventsResult.Data.IsNull())
                                {
                                    result.AddError(string.Format(
                                        "Не найден случай МП ID = {0} счет № {1}", eventE.ExternalId,
                                        parentAccount.AccountNumber));
                                    return result;
                                }

                                var mevent = eventsResult.Data;
                                var eventId = mevent.ZmedicalEventId;
                                mevent.History = eventEAnswer.History;
                                mevent.ZmedicalEventId = 0;//обнуляем старый ид чтобы при вставке загрузить новый иначе обход

                                var slKoefs = new List<ZFactSlKoef>();
                                ZFactKsgKpg ksgKpg = null;
                                var ksgKpgResult = _repository.GetZKsgKpgByZMeventId(eventId);
                                if (ksgKpgResult.Success && ksgKpgResult.Data.IsNotNull())
                                {
                                    var slkoefResult = _repository.GetSlKoefByKsgKpgId(ksgKpgResult.Data.ZksgKpgId);
                                    if (slkoefResult.Success && slkoefResult.Data.Any())
                                    {
                                        foreach (var mslkoef in slkoefResult.Data)
                                        {
                                            var slkoef = Map.ObjectToObject<ZFactSlKoef>(mslkoef);
                                            slKoefs.Add(slkoef);
                                        }
                                    }
                                    ksgKpg = ksgKpgResult.Data;
                                }

                                var services = new List<ZFactMedicalServices>();

                                var servicesResult = _repository.GetMedicalZService(p => p.ZmedicalEventId == eventId);
                                if (servicesResult.Success && servicesResult.Data.Any())
                                {
                                    foreach (var mservice in servicesResult.Data)
                                    {
                                        var serviceCopy = mservice;
                                        var service = Map.ObjectToObject<ZFactMedicalServices>(serviceCopy);
                                        services.Add(service);
                                    }
                                }

                                var refusals = new List<ZFactExternalRefuse>();

                                foreach (v30.E.RefusalE refusal in eventEAnswer.InnerRefusals.Where(p => p.IsNotNull()))
                                {
                                    var externalRefusal = new ZFactExternalRefuse
                                    {
                                        ReasonId = refusal.RefusalCode ?? 53,
                                        Amount = refusal.RefusalRate,
                                        Source = refusal.RefusalSource,
                                        Comment = refusal.Comments,
                                        ExternalGuid = refusal.ExternalGuid,
                                        Type = refusal.RefusalType ?? (int) RefusalType.MEC,
                                        Generation = 0,
                                    };
                                    refusals.Add(externalRefusal);
                                }
                                mevents.Add(Tuple.Create(mevent, Tuple.Create(ksgKpg, slKoefs), services, refusals));

                            }
                            if (mevents.Any())
                            {
                                zslMevents.Add(Tuple.Create(zslMevent, mevents));
                            }
                        }

                        if (zslMevents.Any())
                        {
                            patients.Add(Tuple.Create(patientDb, zslMevents));
                        }
                        lastPatientCount++;
                        _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);

                    }
                }

                var insertRegisterResult = _repository.InsertRegisterEAnswer(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<int> LoadAnswer(v31.EAnswer.RegisterEAnswer registerData, RegisterEInfo info, bool isTestLoad, ProcessingOperations operations)
        {
            var result = new OperationResult<int>();
            try
            {
                //result.AddError("Импорт протокола обратотки версии 3.1 не реализован");
                //return result;
                //   IEnumerable<Tuple<FactPatient, FactPerson, FactDocument, IEnumerable<Tuple<FactMedicalEvent, IEnumerable<FactMedicalServices>, IEnumerable<FactMEC>, IEnumerable<FactMEE>, IEnumerable<FactEQMA>>>>> data = null;

                string sourceTerritoryOkato = info.SourceTerritory.ToString(CultureInfo.InvariantCulture);

                //Проверяем дублирование счета
                var isExistExchange = _repository.GetFactExchange(
                    p => p.FileName == Path.GetFileNameWithoutExtension(info.FileName) &&
                    p.Destination == info.DestinationTerritory.ToString(CultureInfo.InvariantCulture) &&
                    p.Source == info.SourceTerritory.ToString(CultureInfo.InvariantCulture) &&
                    p.Type == (int)AccountType.LogPart &&
                    p.PacketNumber == info.NumberXXXX);
                if (isExistExchange.HasError)
                {
                    result.AddError(string.Format("Ошибка поиска счета №{0}(протокол обработки) от территории {1} отчетная дата {2:MMMM YYYY}.",
                                                registerData.Account.AccountNumber, sourceTerritoryOkato, registerData.Header.Date));
                    return result;
                }

                if (isExistExchange.Success && isExistExchange.Data.Any())
                {
                    result.AddError(string.Format("Счет №{0}(протокол обработки) от территории {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, sourceTerritoryOkato, registerData.Header.Date));
                    return result;
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    result.AddError(string.Format("В счете №{0}(протокол обработки) от территории {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, sourceTerritoryOkato));
                    return result;
                }

                //Поиск основной части
                var parentAccountResult = _repository.GetTerritoryAccountView(
                    p => p.AccountNumber == registerData.Account.AccountNumber &&
                    p.Destination == registerData.Header.TargetOkato &&
                    p.Date == new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1) &&
                    p.Type == (int)AccountType.GeneralPart);
                if (parentAccountResult.HasError || parentAccountResult.Data.IsNull() || !parentAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Для счета №{0}(протокол обработки) от территории {1} не найдена основная часть.",
                                                    registerData.Account.AccountNumber, sourceTerritoryOkato));
                    return result;
                }

                var parentAccount = parentAccountResult.Data.First();

                var accountGenerationResult = _repository.GetTerritoryAccountGenerationByParentId(parentAccount.TerritoryAccountId);
                if (accountGenerationResult.HasError)
                {
                    result.AddError(string.Format("Для счета №{0}(протокол обработки) от территории {1} не определено поколение счета.",
                                                    registerData.Account.AccountNumber, sourceTerritoryOkato));
                    return result;
                }

                var accountGeneration = accountGenerationResult.Data;
                decimal? price = registerData.RecordsCollection.Select(p => p.EventCollection.Where(r => r.PaymentStatus != 2).Sum(r => r.Price)).Sum(p => p);
                decimal? acceptPrice = registerData.RecordsCollection.Select(p => p.EventCollection.Where(r => r.PaymentStatus != 2).Sum(r => r.AcceptPrice)).Sum(p => p);
                
                int direction = GetDirectionByOkato(registerData.Header.TargetOkato);
                var account = new FactTerritoryAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = parentAccount.AccountNumber,
                    AccountDate = DateTime.Now,
                    Price = price,
                    AcceptPrice = acceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    Type = AccountType.CorrectedPart.ToInt32(),
                    Destination = registerData.Header.TargetOkato,
                    Source = parentAccount.Source,
                    ExternalId = null,
                    Direction = direction,
                    Parent = parentAccount.TerritoryAccountId,
                    Generation = accountGeneration,
                    PacketNumber = info.NumberXXXX,
                    Version = parentAccount.Version
                };

                var total = registerData.RecordsCollection.Count;
                var lastPatientCount = 0;

                var patients = new List<Tuple<FactPatient,
                    List<Tuple<ZslFactMedicalEvent,
                        List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
                        List<ZFactExternalRefuse>
                            >>>>();
                foreach (v31.EAnswer.RecordsEAnswer entry in registerData.RecordsCollection)
                {
                    if (entry.EventCollection.Count == 0)
                    {
                        result.AddError(string.Format("Не найдены случаи у пациента с ID = {0} счет № {1}", entry.ExternalId, parentAccount.AccountNumber));
                        return result;
                    }

                    var patientsResult = _repository.GetPatientByZslMeventExternalIdAndAccountId(entry.EventCollection.First().ExternalId, parentAccount.TerritoryAccountId);
                    if (patientsResult.HasError || patientsResult.Data.IsNull() || !patientsResult.Data.Any())
                    {
                        result.AddError(string.Format("Не найден пациент с ID = {0} счет № {1}", entry.ExternalId, parentAccount.AccountNumber));
                        return result;
                    }

                    var tmpPatient = patientsResult.Data.First();

                    var patientsDuplicate = patients.FirstOrDefault(p => p.Item1.ExternalId == tmpPatient.ExternalId);

                    FactPatient patientDb = null;

                    if (patientsDuplicate.IsNull())
                    {
                        if (entry.EventCollection.Any(p =>
                            p.PaymentStatus != 2 && p.InnerRefusals.Any(r => r.RefusalSource != (int)RefusalSource.Local || r.RefusalSource != (int)RefusalSource.LocalCorrected)))
                        {
                            patientDb = tmpPatient;
                            patientDb.AccountId = account.TerritoryAccountId;
                            patientDb.PatientId = 0;
                            patientDb.InsuranceDocNumber = entry.Patient.InsuranceDocNumber;
                            patientDb.InsuranceDocSeries = entry.Patient.InsuranceDocSeries;
                            patientDb.INP = entry.Patient.INP;
                            patientDb.InsuranceDocType = entry.Patient.InsuranceDocType;
                        }
                        else
                        {

                            continue;
                        }
                    }
                    else
                    {
                        patientDb = patientsDuplicate.Item1;
                    }

                    //Тег законченый случай
                    //var zslMevents = new List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>();
                    var zslMevents = new List<Tuple<ZslFactMedicalEvent,
                        List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
                        List<ZFactExternalRefuse>
                        >>();
                    foreach (v31.EAnswer.ZslEventEAnswer eventE in entry.EventCollection)
                    {

                        //Все отказы ТФОМС1->МО не грузим
                        if (eventE.RefusalsXml.Any())
                            if (
                                eventE.RefusalsXml.All(
                                    p =>
                                        p.RefusalSource == (int)RefusalSource.Local ||
                                        p.RefusalSource == (int)RefusalSource.LocalCorrected))
                            {
                                continue;
                            }

                        //Полная оплата случая, обновляем статус оплаты случая 
                        if (eventE.PaymentStatus == 2)
                        {
                            //Обновляем статус оплаты у случая в родительском счете
                            var setResult = _repository.SetZMedicalAsFullPaymentByExternalIdAndAccountId(eventE.ExternalId, parentAccount.TerritoryAccountId);
                            if (setResult.HasError)
                            {
                                result.AddError(string.Format("Ошибка обновления статуса оплаты в родительском счете ID {0}.\r\n{1}", parentAccount.AccountNumber, setResult.LastError));
                                return result;
                            }

                        }
                        //Случай частично отказан или полностью
                        else if (eventE.PaymentStatus == 3 || eventE.PaymentStatus == 4)
                        {
                            //Обновляем статус оплаты у законченного случая в родительском счете
                            var setResult =
                                _repository.SetZslMedicalEventPaymentStatusByExternalIdAndAccountId(eventE.ExternalId,parentAccount.TerritoryAccountId, eventE.PaymentStatus);
                            if (setResult.HasError)
                            {
                                result.AddError(
                                    string.Format(
                                        "Ошибка обновления статуса оплаты в родительском счете ID {0}.\r\n{1}",
                                        parentAccount.AccountNumber, setResult.LastError));
                                return result;
                            }

                            var zslEventsResult = _repository.GetZslEventByExternalIdAndAccountId(eventE.ExternalId,
                                parentAccount.TerritoryAccountId);
                            if (zslEventsResult.HasError || zslEventsResult.Data.IsNull())
                            {
                                result.AddError(string.Format("Не найден законченный случай МП ID = {0} счет № {1}",
                                    eventE.ExternalId, parentAccount.AccountNumber));
                                return result;
                            }

                            var zslMevent = zslEventsResult.Data;
                            var zslEventId = zslMevent.ZslMedicalEventId;
                            zslMevent.PaymentMethod = eventE.PaymentMethod;
                            zslMevent.Price = eventE.Price;
                            zslMevent.AcceptPrice = eventE.AcceptPrice;
                            zslMevent.PaymentStatus = eventE.PaymentStatus;
                            zslMevent.ZslMedicalEventId = 0;

                            //тег случай
                            var mevents = new List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>();
                            foreach (v31.EAnswer.EventEAnswer eventEAnswer in eventE.InnerEventCollection)
                            {
                               
                                var eventsResult = _repository.GetZEventBySlidGuidAndAccountId(eventEAnswer.ExternalId, zslEventId, parentAccount.TerritoryAccountId);
                                if (eventsResult.HasError || eventsResult.Data.IsNull())
                                {
                                    result.AddError(string.Format("Не найден случай МП ID = {0} счет № {1}", eventE.ExternalId,
                                        parentAccount.AccountNumber));
                                    return result;
                                }

                                var mevent = eventsResult.Data;
                                var eventId = mevent.ZmedicalEventId;
                                mevent.History = eventEAnswer.History;
                                mevent.ZmedicalEventId = 0;//обнуляем старый ид чтобы при вставке загрузить новый иначе обход


                                var directionsResult = _repository.GetZDirectionBySlMeventId(eventId);
                                var directions = new List<ZFactDirection>();
                                if (directionsResult.Success && directionsResult.Data.IsNotNull())
                                {
                                    
                                    foreach (var direc in directionsResult.Data)
                                    {
                                        if (direc == null) continue;
                                        directions.Add(Map.ObjectToObject<ZFactDirection>(direc));
                                    }
                                }

                                var consultationsResult = _repository.GetZConsultationsBySlMeventId(eventId);
                                var сonsultations = new List<ZFactConsultations>();
                                if (consultationsResult.Success && consultationsResult.Data.IsNotNull())
                                {
                                    foreach (var cons in consultationsResult.Data)
                                    {
                                        if (cons == null) continue;
                                        сonsultations.Add(Map.ObjectToObject<ZFactConsultations>(cons));
                                    }
                                }

                                var slKoefs = new List<ZFactSlKoef>();
                                var critList = new List<ZFactCrit>();
                                ZFactKsgKpg ksgKpg = null;
                                var ksgKpgResult = _repository.GetZKsgKpgByZMeventId(eventId);
                                if (ksgKpgResult.Success && ksgKpgResult.Data.IsNotNull())
                                {
                                    var critResult = _repository.GetCritByksgKpgId(ksgKpgResult.Data.ZksgKpgId);
                                    if (critResult.Success && critResult.Data.Any())
                                    {
                                        foreach (var i in critResult.Data)
                                        {
                                            var cr = Map.ObjectToObject<ZFactCrit>(i);
                                            critList.Add(cr);
                                        }
                                    }
                                    

                                    var slkoefResult = _repository.GetSlKoefByKsgKpgId(ksgKpgResult.Data.ZksgKpgId);
                                    if (slkoefResult.Success && slkoefResult.Data.Any())
                                    {
                                        foreach (var mslkoef in slkoefResult.Data)
                                        {
                                            var slkoef = Map.ObjectToObject<ZFactSlKoef>(mslkoef);
                                            slKoefs.Add(slkoef);
                                        }
                                    }
                                    ksgKpg = ksgKpgResult.Data;
                                }

                                var medicalEventOnk =
                                new Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>,
                                    List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>(null, null, null, null);
                                var eventOnkResult = _repository.GetZMedicalEventOnkByZMeventId(eventId);
                                if (eventOnkResult.Success && eventOnkResult.Data.IsNotNull())
                                {
                                    var eventOnkId = eventOnkResult.Data.ZMedicalEventOnkId;

                                    var diagBlokResult = _repository.GetDiafBlokByMedicalEventOnkId(eventOnkId);
                                    var diagBloks = new List<ZFactDiagBlok>();
                                    if (diagBlokResult.Success && diagBlokResult.Data.IsNotNull())
                                    {
                                        foreach (var diagBlokOnk in diagBlokResult.Data)
                                        {
                                            if (diagBlokOnk == null) continue;
                                            diagBloks.Add(Map.ObjectToObject<ZFactDiagBlok>(diagBlokOnk));
                                        }
                                    }

                                    var contraindicationsResult = _repository.GetContraindicationsByMedicalEventOnkId(eventOnkId);
                                    var contraindications = new List<ZFactContraindications>();
                                    if (contraindicationsResult.Success && contraindicationsResult.Data.IsNotNull())
                                    {
                                        foreach (var сontraindicationsOnk in contraindicationsResult.Data)
                                        {
                                            if (сontraindicationsOnk == null) continue;
                                            contraindications.Add(Map.ObjectToObject<ZFactContraindications>(сontraindicationsOnk));
                                        }
                                    }

                                    var serviceUslOnkResult = _repository.GetZMedicalServiceOnkByMedicalEventId(eventOnkId);
                                    var serviceUslOnk = new List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>();
                                    if (serviceUslOnkResult.Success && serviceUslOnkResult.Data.IsNotNull())
                                    {
                                        foreach (var serviceOnk in serviceUslOnkResult.Data)
                                        {
                                            if (serviceOnk == null) continue;
                                            var serviceOnkId = serviceOnk.ZmedicalServicesOnkId;

                                            var anticancerDrugOnkResult = _repository.GetAnticancerDrugByMedicalServiceOnkId(serviceOnkId);
                                            var anticancerDrugOnkDs = new List<ZFactAnticancerDrug>();
                                            if (anticancerDrugOnkResult.Success && anticancerDrugOnkResult.Data.IsNotNull())
                                            {
                                                foreach (var anticancerDrugOnkD in anticancerDrugOnkResult.Data)
                                                {
                                                    if (anticancerDrugOnkD == null) continue;
                                                    anticancerDrugOnkDs.Add(Map.ObjectToObject<ZFactAnticancerDrug>(anticancerDrugOnkD));

                                                }
                                                serviceUslOnk.Add(
                                                    Tuple.Create(Map.ObjectToObject<ZFactMedicalServicesOnk>(serviceOnk),
                                                        anticancerDrugOnkDs));
                                            }
                                            
                                        }
                                    }
                                    medicalEventOnk = Tuple.Create(Map.ObjectToObject<ZFactMedicalEventOnk>(eventOnkResult.Data), diagBloks, contraindications, serviceUslOnk);
                                }

                                var services = new List<ZFactMedicalServices>();

                                var servicesResult = _repository.GetMedicalZService(p => p.ZmedicalEventId == eventId);
                                if (servicesResult.Success && servicesResult.Data.Any())
                                {
                                    foreach (var mservice in servicesResult.Data)
                                    {
                                        var service = Map.ObjectToObject<ZFactMedicalServices>(mservice);
                                        services.Add(service);
                                    }
                                }
                                mevents.Add(Tuple.Create(mevent, directions, сonsultations, medicalEventOnk, Tuple.Create(ksgKpg, critList, slKoefs), services));
                            }
                            var refusals = new List<ZFactExternalRefuse>();

                            foreach (v31.E.RefusalE refusal in eventE.RefusalsXml.Where(p => p.IsNotNull()))
                            {
                                var externalRefusal = new ZFactExternalRefuse
                                {
                                    ReasonId = refusal.RefusalCode ?? 53,
                                    Amount = refusal.RefusalRate,
                                    Source = (int)RefusalSource.Local,
                                    Comment = refusal.Comments,
                                    SlidGuid = refusal.SlidGuid,
                                    ExternalGuid = refusal.ExternalGuid,
                                    Type = refusal.RefusalType ?? (int)RefusalType.MEC,
                                    Generation = 0,
                                };
                                refusals.Add(externalRefusal);
                            }
                            if (mevents.Any())
                            {
                                zslMevents.Add(Tuple.Create(zslMevent, mevents, refusals));
                            }
                        }
                        if (zslMevents.Any())
                        {
                            patients.Add(Tuple.Create(patientDb, zslMevents));
                        }
                        lastPatientCount++;
                        _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                    }
                }

                var insertRegisterResult = _repository.InsertRegisterEAnswer(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<int> LoadAnswer(v32.EAnswer.RegisterEAnswer registerData, RegisterEInfo info, bool isTestLoad, ProcessingOperations operations)
        {
            var result = new OperationResult<int>();
            try
            {
                string sourceTerritoryOkato = info.SourceTerritory.ToString(CultureInfo.InvariantCulture);

                //Проверяем дублирование счета
                var isExistExchange = _repository.GetFactExchange(
                    p => p.FileName == Path.GetFileNameWithoutExtension(info.FileName) &&
                    p.Destination == info.DestinationTerritory.ToString(CultureInfo.InvariantCulture) &&
                    p.Source == info.SourceTerritory.ToString(CultureInfo.InvariantCulture) &&
                    p.Type == (int)AccountType.LogPart &&
                    p.PacketNumber == info.NumberXXXX);
                if (isExistExchange.HasError)
                {
                    result.AddError(string.Format("Ошибка поиска счета №{0}(протокол обработки) от территории {1} отчетная дата {2:MMMM YYYY}.",
                                                registerData.Account.AccountNumber, sourceTerritoryOkato, registerData.Header.Date));
                    return result;
                }

                if (isExistExchange.Success && isExistExchange.Data.Any())
                {
                    result.AddError(string.Format("Счет №{0}(протокол обработки) от территории {1} отчетная дата {2:MMMM YYYY} уже был загружен. Удалите его и повторите попытку.",
                                                registerData.Account.AccountNumber, sourceTerritoryOkato, registerData.Header.Date));
                    return result;
                }

                if (!registerData.Account.Year.HasValue || !registerData.Account.Month.HasValue)
                {
                    result.AddError(string.Format("В счете №{0}(протокол обработки) от территории {1} отсутствует отчетная дата (месяц, год).",
                                                    registerData.Account.AccountNumber, sourceTerritoryOkato));
                    return result;
                }

                //Поиск основной части
                var parentAccountResult = _repository.GetTerritoryAccountView(
                    p => p.AccountNumber == registerData.Account.AccountNumber &&
                    p.Destination == registerData.Header.TargetOkato &&
                    p.Date == new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1) &&
                    p.Type == (int)AccountType.GeneralPart);
                if (parentAccountResult.HasError || parentAccountResult.Data.IsNull() || !parentAccountResult.Data.Any())
                {
                    result.AddError(string.Format("Для счета №{0}(протокол обработки) от территории {1} не найдена основная часть.",
                                                    registerData.Account.AccountNumber, sourceTerritoryOkato));
                    return result;
                }

                var parentAccount = parentAccountResult.Data.First();

                var accountGenerationResult = _repository.GetTerritoryAccountGenerationByParentId(parentAccount.TerritoryAccountId);
                if (accountGenerationResult.HasError)
                {
                    result.AddError(string.Format("Для счета №{0}(протокол обработки) от территории {1} не определено поколение счета.",
                                                    registerData.Account.AccountNumber, sourceTerritoryOkato));
                    return result;
                }

                var accountGeneration = accountGenerationResult.Data;
                decimal? price = registerData.RecordsCollection.Select(p => p.EventCollection.Where(r => r.PaymentStatus != 2).Sum(r => r.Price)).Sum(p => p);
                decimal? acceptPrice = registerData.RecordsCollection.Select(p => p.EventCollection.Where(r => r.PaymentStatus != 2).Sum(r => r.AcceptPrice)).Sum(p => p);



                int direction = GetDirectionByOkato(registerData.Header.TargetOkato);
                var account = new FactTerritoryAccount
                {
                    Date = new DateTime(registerData.Account.Year.Value, registerData.Account.Month.Value, 1),
                    AccountNumber = parentAccount.AccountNumber,
                    AccountDate = DateTime.Now,
                    Price = price,
                    AcceptPrice = acceptPrice,
                    MECPenalties = registerData.Account.MECPenalties,
                    MEEPenalties = registerData.Account.MEEPenalties,
                    EQMAPenalties = registerData.Account.EQMAPenalties,
                    Status = AccountStatus.NotProcessed.ToInt32(),
                    Type = AccountType.CorrectedPart.ToInt32(),
                    Destination = registerData.Header.TargetOkato,
                    Source = parentAccount.Source,
                    ExternalId = null,
                    Direction = direction,
                    Parent = parentAccount.TerritoryAccountId,
                    Generation = accountGeneration,
                    PacketNumber = info.NumberXXXX,
                    Version = parentAccount.Version
                };

                var total = registerData.RecordsCollection.Count;
                var lastPatientCount = 0;

                var patients = new List<Tuple<FactPatient,
                    List<Tuple<ZslFactMedicalEvent,
                        List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
                        List<ZFactExternalRefuse>
                            >>>>();
                foreach (v32.EAnswer.RecordsEAnswer entry in registerData.RecordsCollection)
                {
                    if (entry.EventCollection.Count == 0)
                    {
                        result.AddError(string.Format("Не найдены случаи у пациента с ID = {0} счет № {1}", entry.ExternalId, parentAccount.AccountNumber));
                        return result;
                    }

                    var patientsResult = _repository.GetPatientByZslMeventExternalIdAndAccountId(entry.EventCollection.First().ExternalId, parentAccount.TerritoryAccountId);
                    if (patientsResult.HasError || patientsResult.Data.IsNull() || !patientsResult.Data.Any())
                    {
                        result.AddError(string.Format("Не найден пациент с ID = {0} счет № {1}", entry.ExternalId, parentAccount.AccountNumber));
                        return result;
                    }

                    var tmpPatient = patientsResult.Data.First();

                    var patientsDuplicate = patients.FirstOrDefault(p => p.Item1.ExternalId == tmpPatient.ExternalId);

                    FactPatient patientDb = null;

                    if (patientsDuplicate.IsNull())
                    {
                        if (entry.EventCollection.Any(p =>
                            p.PaymentStatus != 2 && p.InnerRefusals.Any(r => r.RefusalSource != (int)RefusalSource.Local || r.RefusalSource != (int)RefusalSource.LocalCorrected)))
                        {
                            patientDb = tmpPatient;
                            patientDb.AccountId = account.TerritoryAccountId;
                            patientDb.PatientId = 0;
                            patientDb.InsuranceDocNumber = entry.Patient.InsuranceDocNumber;
                            patientDb.InsuranceDocSeries = entry.Patient.InsuranceDocSeries;
                            patientDb.INP = entry.Patient.INP;
                            patientDb.InsuranceDocType = entry.Patient.InsuranceDocType;
                        }
                        else
                        {

                            continue;
                        }
                    }
                    else
                    {
                        patientDb = patientsDuplicate.Item1;
                    }

                    //Тег законченый случай
                    //var zslMevents = new List<Tuple<ZslFactMedicalEvent, List<Tuple<ZFactMedicalEvent, Tuple<ZFactKsgKpg, List<ZFactSlKoef>>, List<ZFactMedicalServices>, List<ZFactExternalRefuse>>>>>();
                    var zslMevents = new List<Tuple<ZslFactMedicalEvent,
                        List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>,
                        List<ZFactExternalRefuse>
                        >>();
                    foreach (v32.EAnswer.ZslEventEAnswer eventE in entry.EventCollection)
                    {

                        //Все отказы ТФОМС1->МО не грузим
                        if (eventE.RefusalsXml.Any())
                            if (
                                eventE.RefusalsXml.All(
                                    p =>
                                        p.RefusalSource == (int)RefusalSource.Local ||
                                        p.RefusalSource == (int)RefusalSource.LocalCorrected))
                            {
                                continue;
                            }

                        //Полная оплата случая, обновляем статус оплаты случая 
                        if (eventE.PaymentStatus == 2)
                        {
                            //Обновляем статус оплаты у случая в родительском счете
                            var setResult = _repository.SetZMedicalAsFullPaymentByExternalIdAndAccountId(eventE.ExternalId, parentAccount.TerritoryAccountId);
                            if (setResult.HasError)
                            {
                                result.AddError(string.Format("Ошибка обновления статуса оплаты в родительском счете ID {0}.\r\n{1}", parentAccount.AccountNumber, setResult.LastError));
                                return result;
                            }

                        }
                        //Случай частично отказан или полностью
                        else if (eventE.PaymentStatus == 3 || eventE.PaymentStatus == 4)
                        {
                            //Обновляем статус оплаты у законченного случая в родительском счете
                            var setResult =
                                _repository.SetZslMedicalEventPaymentStatusByExternalIdAndAccountId(eventE.ExternalId, parentAccount.TerritoryAccountId, eventE.PaymentStatus);
                            if (setResult.HasError)
                            {
                                result.AddError(
                                    string.Format(
                                        "Ошибка обновления статуса оплаты в родительском счете ID {0}.\r\n{1}",
                                        parentAccount.AccountNumber, setResult.LastError));
                                return result;
                            }

                            var zslEventsResult = _repository.GetZslEventByExternalIdAndAccountId(eventE.ExternalId,
                                parentAccount.TerritoryAccountId);
                            if (zslEventsResult.HasError || zslEventsResult.Data.IsNull())
                            {
                                result.AddError(string.Format("Не найден законченный случай МП ID = {0} счет № {1}",
                                    eventE.ExternalId, parentAccount.AccountNumber));
                                return result;
                            }

                            var zslMevent = zslEventsResult.Data;
                            var zslEventId = zslMevent.ZslMedicalEventId;
                            zslMevent.PaymentMethod = eventE.PaymentMethod;
                            zslMevent.Price = eventE.Price;
                            zslMevent.AcceptPrice = eventE.AcceptPrice;
                            zslMevent.PaymentStatus = eventE.PaymentStatus;
                            zslMevent.ZslMedicalEventId = 0;

                            //тег случай
                            var mevents = new List<Tuple<ZFactMedicalEvent,
                            List<ZFactDirection>,
                            List<ZFactConsultations>,
                            Tuple<ZFactMedicalEventOnk,
                                List<ZFactDiagBlok>,
                                List<ZFactContraindications>,
                                List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>,
                            Tuple<ZFactKsgKpg, List<ZFactCrit>, List<ZFactSlKoef>>, List<ZFactMedicalServices>>>();
                            foreach (v32.EAnswer.EventEAnswer eventEAnswer in eventE.InnerEventCollection)
                            {

                                var eventsResult = _repository.GetZEventBySlidGuidAndAccountId(eventEAnswer.ExternalId, zslEventId, parentAccount.TerritoryAccountId);
                                if (eventsResult.HasError || eventsResult.Data.IsNull())
                                {
                                    result.AddError(string.Format("Не найден случай МП ID = {0} счет № {1}", eventE.ExternalId,
                                        parentAccount.AccountNumber));
                                    return result;
                                }

                                var mevent = eventsResult.Data;
                                var eventId = mevent.ZmedicalEventId;
                                mevent.History = eventEAnswer.History;
                                mevent.ZmedicalEventId = 0;//обнуляем старый ид чтобы при вставке загрузить новый иначе обход


                                var directionsResult = _repository.GetZDirectionBySlMeventId(eventId);
                                var directions = new List<ZFactDirection>();
                                if (directionsResult.Success && directionsResult.Data.IsNotNull())
                                {

                                    foreach (var direc in directionsResult.Data)
                                    {
                                        if (direc == null) continue;
                                        directions.Add(Map.ObjectToObject<ZFactDirection>(direc));
                                    }
                                }

                                var consultationsResult = _repository.GetZConsultationsBySlMeventId(eventId);
                                var сonsultations = new List<ZFactConsultations>();
                                if (consultationsResult.Success && consultationsResult.Data.IsNotNull())
                                {
                                    foreach (var cons in consultationsResult.Data)
                                    {
                                        if (cons == null) continue;
                                        сonsultations.Add(Map.ObjectToObject<ZFactConsultations>(cons));
                                    }
                                }

                                var slKoefs = new List<ZFactSlKoef>();
                                var critList = new List<ZFactCrit>();
                                ZFactKsgKpg ksgKpg = null;
                                var ksgKpgResult = _repository.GetZKsgKpgByZMeventId(eventId);
                                if (ksgKpgResult.Success && ksgKpgResult.Data.IsNotNull())
                                {
                                    var critResult = _repository.GetCritByksgKpgId(ksgKpgResult.Data.ZksgKpgId);
                                    if (critResult.Success && critResult.Data.Any())
                                    {
                                        foreach (var i in critResult.Data)
                                        {
                                            var cr = Map.ObjectToObject<ZFactCrit>(i);
                                            critList.Add(cr);
                                        }
                                    }


                                    var slkoefResult = _repository.GetSlKoefByKsgKpgId(ksgKpgResult.Data.ZksgKpgId);
                                    if (slkoefResult.Success && slkoefResult.Data.Any())
                                    {
                                        foreach (var mslkoef in slkoefResult.Data)
                                        {
                                            var slkoef = Map.ObjectToObject<ZFactSlKoef>(mslkoef);
                                            slKoefs.Add(slkoef);
                                        }
                                    }
                                    ksgKpg = ksgKpgResult.Data;
                                }

                                var medicalEventOnk =
                                new Tuple<ZFactMedicalEventOnk, List<ZFactDiagBlok>, List<ZFactContraindications>,
                                    List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>>(null, null, null, null);
                                var eventOnkResult = _repository.GetZMedicalEventOnkByZMeventId(eventId);
                                if (eventOnkResult.Success && eventOnkResult.Data.IsNotNull())
                                {
                                    var eventOnkId = eventOnkResult.Data.ZMedicalEventOnkId;

                                    var diagBlokResult = _repository.GetDiafBlokByMedicalEventOnkId(eventOnkId);
                                    var diagBloks = new List<ZFactDiagBlok>();
                                    if (diagBlokResult.Success && diagBlokResult.Data.IsNotNull())
                                    {
                                        foreach (var diagBlokOnk in diagBlokResult.Data)
                                        {
                                            if (diagBlokOnk == null) continue;
                                            diagBloks.Add(Map.ObjectToObject<ZFactDiagBlok>(diagBlokOnk));
                                        }
                                    }

                                    var contraindicationsResult = _repository.GetContraindicationsByMedicalEventOnkId(eventOnkId);
                                    var contraindications = new List<ZFactContraindications>();
                                    if (contraindicationsResult.Success && contraindicationsResult.Data.IsNotNull())
                                    {
                                        foreach (var сontraindicationsOnk in contraindicationsResult.Data)
                                        {
                                            if (сontraindicationsOnk == null) continue;
                                            contraindications.Add(Map.ObjectToObject<ZFactContraindications>(сontraindicationsOnk));
                                        }
                                    }

                                    var serviceUslOnkResult = _repository.GetZMedicalServiceOnkByMedicalEventId(eventOnkId);
                                    var serviceUslOnk = new List<Tuple<ZFactMedicalServicesOnk, List<ZFactAnticancerDrug>>>();
                                    if (serviceUslOnkResult.Success && serviceUslOnkResult.Data.IsNotNull())
                                    {
                                        foreach (var serviceOnk in serviceUslOnkResult.Data)
                                        {
                                            if (serviceOnk == null) continue;
                                            var serviceOnkId = serviceOnk.ZmedicalServicesOnkId;

                                            var anticancerDrugOnkResult = _repository.GetAnticancerDrugByMedicalServiceOnkId(serviceOnkId);
                                            var anticancerDrugOnkDs = new List<ZFactAnticancerDrug>();
                                            if (anticancerDrugOnkResult.Success && anticancerDrugOnkResult.Data.IsNotNull())
                                            {
                                                foreach (var anticancerDrugOnkD in anticancerDrugOnkResult.Data)
                                                {
                                                    if (anticancerDrugOnkD == null) continue;
                                                    anticancerDrugOnkDs.Add(Map.ObjectToObject<ZFactAnticancerDrug>(anticancerDrugOnkD));

                                                }
                                                serviceUslOnk.Add(
                                                    Tuple.Create(Map.ObjectToObject<ZFactMedicalServicesOnk>(serviceOnk),
                                                        anticancerDrugOnkDs));
                                            }

                                        }
                                    }
                                    medicalEventOnk = Tuple.Create(Map.ObjectToObject<ZFactMedicalEventOnk>(eventOnkResult.Data), diagBloks, contraindications, serviceUslOnk);
                                }

                                var services = new List<ZFactMedicalServices>();

                                var servicesResult = _repository.GetMedicalZService(p => p.ZmedicalEventId == eventId);
                                if (servicesResult.Success && servicesResult.Data.Any())
                                {
                                    foreach (var mservice in servicesResult.Data)
                                    {
                                        var service = Map.ObjectToObject<ZFactMedicalServices>(mservice);
                                        services.Add(service);
                                    }
                                }
                                mevents.Add(Tuple.Create(mevent, directions, сonsultations, medicalEventOnk, Tuple.Create(ksgKpg, critList, slKoefs), services));
                            }
                            var refusals = new List<ZFactExternalRefuse>();

                            foreach (v32.E.RefusalE refusal in eventE.RefusalsXml.Where(p => p.IsNotNull()))
                            {
                                var externalRefusal = new ZFactExternalRefuse
                                {
                                    ReasonId = refusal.RefusalCode ?? 53,
                                    Amount = refusal.RefusalRate,
                                    Source = (int)RefusalSource.Local,
                                    Comment = refusal.Comments,
                                    SlidGuid = refusal.SlidGuid,
                                    ExternalGuid = refusal.ExternalGuid,
                                    Type = refusal.RefusalType ?? (int)RefusalType.MEC,
                                    Generation = 0,
                                };
                                refusals.Add(externalRefusal);
                            }
                            if (mevents.Any())
                            {
                                zslMevents.Add(Tuple.Create(zslMevent, mevents, refusals));
                            }
                        }
                        if (zslMevents.Any())
                        {
                            patients.Add(Tuple.Create(patientDb, zslMevents));
                        }
                        lastPatientCount++;
                        _dockLayoutManager.SetOverlayProgress(total, lastPatientCount);
                    }
                }

                var insertRegisterResult = _repository.InsertRegisterEAnswer(account, patients, isTestLoad);

                if (insertRegisterResult.Success)
                {
                    result.Data = insertRegisterResult.Id;
                }
                else
                {
                    result.AddError(insertRegisterResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }
        private int GetDirectionByOkato(string okato)
        {
            dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
            string tfOkato = terCode.tf_okato;
            return okato == tfOkato ? (int)DirectionType.In : (int)DirectionType.Out;
        }

        public OperationResult<T> Deserialize<T>(Stream stream)
        {
            var result = new OperationResult<T>();
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                stream.Position = 0;
                result.Data = (T)serializer.Deserialize(stream);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult<int> GetOmsFileVersion(string fileName)
        {
            var result = OperationResult<int>.Create(Constants.Version10);
            try
            {
                var versionCache = _cache.Get(CacheRepository.VersionCache);
                var tmpFile = fileName;
                var extension = Path.GetExtension(fileName);
                if (extension != null && extension.ToLowerInvariant().Equals(".oms"))
                {
                    var zipHelper = new ZipHelpers();
                    List<string> fileNames = zipHelper.UnpackFiles(fileName);
                    tmpFile = fileNames.FirstOrDefault();
                }

                if (File.Exists(tmpFile))
                {
                    using (XmlReader reader = XmlReader.Create(tmpFile))
                    {
                        XDocument doc = XDocument.Load(reader);
                        IEnumerable<XElement> list = from t in doc.Descendants("VERSION")
                                                     select t;
                        XElement ver = list.FirstOrDefault();
                        if (ver != null)
                        {
                            if (versionCache.ValueExists(ver.Value))
                            {
                                result.Data = versionCache.GetBackInt(ver.Value);
                                result.Log = "<u><br>Автоматическое определение версии:{0}<br></u>".F(ver.Value);
                            }
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

        public OperationResult<int> GetVersion(int version)
        {
            var result = OperationResult<int>.Create(Constants.Version10);
            try
            {
                switch (version)
                {
                    case 1:
                        result.Data = Constants.Version10;
                        break;
                    case 2:
                        result.Data = Constants.Version21;
                        break;
                    case 3:
                        result.Data = Constants.Version30;
                        break;
                    case 4:
                        result.Data = Constants.Version31;
                        break;
                    case 5:
                        result.Data = Constants.Version32;
                        break;
                    default:
                        result.Data = 0;
                        break;
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }


        public OperationResult<LoaderBase> GetLoaderByVersion(int version)
        {
            var result = OperationResult<LoaderBase>.Create();
            try
            {
                switch (version)
                {
                    case 1:
                        result.Data = new v10.E.LoaderE();
                        break;
                    case 2:
                        result.Data = new v21.E.LoaderE();
                        break;
                    case 5:
                        result.Data = new v30.E.LoaderE();
                        break;
                    case 7:
                        result.Data = new v31.E.LoaderE();
                        break;
                    case 9:
                        result.Data = new v32.E.LoaderE();
                        break;
                    default:
                        throw new ArgumentException("Неверная версия загрузчика файла OMS");
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult<LoaderBase> GetLoaderByVersionD(int version)
        {
            var result = OperationResult<LoaderBase>.Create();
            try
            {
                switch (version)
                {
                    case 2:
                    case 3:
                        result.Data = new v21K2.D.LoaderD();
                        break;
                    case 4:
                    case 5:
                        result.Data = new v30K1.D.LoaderD();
                        break;
                    case 6:
                    case 7:
                        result.Data = new v31K1.D.LoaderD();
                        break;
                    case 8:
                    case 9:
                        result.Data = new v32K1.D.LoaderD();
                        break;
                    default:
                        throw new ArgumentException("Неверная версия загрузчика файла OMS");
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult ValidateXml(int version, LoaderBase loader, string file)
        {
            var result = OperationResult.Create();
            try
            {
                switch (((RegisterEInfo)loader.Info).Type.ToLowerInvariant())
                {
                    case "r":
                    case "d":
                        switch (version)
                        {
                            case Constants.Version10:
                                loader.Validate2<v10.E.RegisterE>(file);
                                break;
                            case Constants.Version21:
                                loader.Validate2<v21.E.RegisterE>(file);
                                break;
                            case Constants.Version30:
                                loader.Validate2<v30.E.RegisterE>(file);
                                break;
                            case Constants.Version31:
                                loader.Validate2<v31.E.RegisterE>(file);
                                break;
                            case Constants.Version32:
                                loader.Validate2<v32.E.RegisterE>(file);
                                break;

                        }

                        break;
                    case "a":
                        switch (version)
                        {
                            case Constants.Version10:
                                loader.Validate2<v10.EAnswer.LoaderEAnswer>(file, "Medical.DataCore.v10.Xsd.E3.xsd");

                                break;
                            case Constants.Version21:
                                loader.Validate2<v21.EAnswer.LoaderEAnswer>(file, "Medical.DataCore.v21.Xsd.E3.xsd");
                                break;
                            case Constants.Version30:
                                loader.Validate2<v30.EAnswer.LoaderEAnswer>(file, "Medical.DataCore.v21.Xsd.E3.xsd"); //TODO Нужно переписать схему данных
                                break;
                            case Constants.Version31:
                                loader.Validate2<v31.EAnswer.LoaderEAnswer>(file, "Medical.DataCore.v21.Xsd.E3.xsd"); //TODO Нужно переписать схему данных
                                break;
                        }

                        break;
                    default:
                        throw  new ArgumentException("Неизвестный тип файла xml {0}<br>".F(((RegisterEInfo)loader.Info).Type));
                }

                if (loader.ErrorCount != 0)
                {
                    result.Log = "При валидации реестра произошла ошибка<br>{0}".F(loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next)));
                    throw new ValidationException("При валидации реестра произошла ошибка<br>{0}".F(loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next))));
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult ValidateDXml(int version, LoaderBase loader, string lFile, string hFile)
        {
            var result = OperationResult.Create();
            try
            {
                switch (version)
                {
                    case Constants.Version21K:
                        loader.Validate2<v21K2.D.AccountRegisterD>(hFile);
                        if (loader.ErrorCount != 0)
                        {
                            result.Log =
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next)));
                            throw new ValidationException(
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next))));
                        }

                        loader.Validate2<v21K2.D.PersonalD>(lFile, "Medical.DataCore.v21K2.Xsd.D2.xsd");

                        if (loader.ErrorCount != 0)
                        {
                            result.Log =
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next)));
                            throw new ValidationException(
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next))));
                        }
                        break;
                    case Constants.Version30K:
                        loader.Validate2<v30K1.D.AccountRegisterD>(hFile);
                        if (loader.ErrorCount != 0)
                        {
                            result.Log =
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next)));
                            throw new ValidationException(
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next))));
                        }

                        loader.Validate2<v30K1.D.PersonalD>(lFile, "Medical.DataCore.v30K1.Xsd.D2.xsd");

                        if (loader.ErrorCount != 0)
                        {
                            result.Log =
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next)));
                            throw new ValidationException(
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next))));
                        }
                        break;
                    case Constants.Version31K:
                        loader.Validate2<v31K1.D.AccountRegisterD>(hFile);
                        if (loader.ErrorCount != 0)
                        {
                            result.Log =
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next)));
                            throw new ValidationException(
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next))));
                        }

                        loader.Validate2<v31K1.D.PersonalD>(lFile, "Medical.DataCore.v31K1.Xsd.D2.xsd");

                        if (loader.ErrorCount != 0)
                        {
                            result.Log =
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next)));
                            throw new ValidationException(
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next))));
                        }
                        break;
                    case Constants.Version32K:
                        loader.Validate2<v32K1.D.AccountRegisterD>(hFile);
                        if (loader.ErrorCount != 0)
                        {
                            result.Log =
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next)));
                            throw new ValidationException(
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next))));
                        }

                        loader.Validate2<v32K1.D.PersonalD>(lFile, "Medical.DataCore.v32K1.Xsd.D2.xsd");

                        if (loader.ErrorCount != 0)
                        {
                            result.Log =
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next)));
                            throw new ValidationException(
                                "При валидации реестра произошла ошибка<br>{0}".F(
                                    loader.Errors.Aggregate((p, next) => string.Format("{0}<br/>{1}", p, next))));
                        }
                        break;
                    default:
                        throw new ArgumentException(
                            "Версия файла не определена {0}<br>".F(((RegisterEInfo) loader.Info).Type));
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult UpdatePacketNumber(int accountId, RegisterInfo info)
        {
            var result = OperationResult.Create();
            try
            {
                var updateResult = _repository.UpdateTerritoryAccountPacketNumber(accountId, info.NumberXXXX);
                if (updateResult.HasError)
                {
                    result.AddError(updateResult.LastError);
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<Tuple<T1, RegisterEInfo, int>> WriteAnswer<T1, T2, T3, T4, T5, T6>(int accountId, int version, int type)
            where T1 : IRegisterAnswer, new()
            where T2 : IAccountAnswer, new()
            where T3 : IHeaderAnswer, new()
            where T4 : IRecordAnswer, new()
            where T5 : IPatientAnswer, new()
            where T6 : IMEventAnswer, new()
        {
            var result = new OperationResult<Tuple<T1, RegisterEInfo, int>>();
            try
            {
                var accountResult = _repository.GetTerritoryAccountById(accountId);
                if (accountResult.HasError)
                {
                    result.AddError(accountResult.LastError);
                    return result;
                }

                _repository.UpdateGuid(accountId);

                var register = new T1
                {
                    InnerAccount = Map.ObjectToObject<T2>(accountResult.Data, typeof(T2))
                };

                Expression<Func<FactPreparedReport, bool>> predicate =
                    i => i.ExternalId == accountId && (i.ReportId == 17 || i.ReportId == 18);

                var preparedReport = _repository.GetFactPreparedReport(predicate);
                if (preparedReport.Success)
                {

                }
                register.InnerAccount.Year = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0;
                register.InnerAccount.Month = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Month : 0;
                register.InnerHeader = new T3
                {
                    TargetOkato = accountResult.Data.Destination,
                    Version = version,
                    Date = preparedReport.Success ? preparedReport.Data.First()?.Date : default(DateTime?)
                };

                var patientsResult = _repository.GetPatientsByAccountId(accountId);
                if (patientsResult.Success)
                {
                    var records = new List<IRecordAnswer>();
                    foreach (FactPatient factPatient in patientsResult.Data)
                    {
                        FactPatient patient = factPatient;
                        FactPerson person = null;

                        if (patient.PersonalId.HasValue)
                        {
                            var personResult = _repository.GetPersonById(patient.PersonalId.Value);
                            if (personResult.Success)
                            {
                                person = personResult.Data;
                            }
                        }

                        var record = new T4();
                        record.InnerPatient = Map.ObjectToObject<T5>(person, typeof(T5));

                        record.ExternalId = factPatient.ExternalId ?? 0;

                        record.InnerPatient.InsuranceDocType = patient.InsuranceDocType;
                        record.InnerPatient.InsuranceDocSeries = patient.InsuranceDocSeries;

                        switch (version)
                        {
                            case Constants.Version10:
                                record.InnerPatient.PolicyNumber = patient.InsuranceDocType == 3 ? patient.INP : patient.InsuranceDocNumber;
                                break;
                            case Constants.Version21:
                                record.InnerPatient.InsuranceDocNumber = patient.InsuranceDocNumber;
                                //record.InnerPatient.INP = patient.INP; ; //раскомментировать эту надпись, если нужно все же в отказ выгружать ЕНП!
                                //и закомментировать эти 4 строки:

                                if (record.InnerPatient.InsuranceDocNumber.IsNullOrWhiteSpace() && patient.INP.IsNotNullOrWhiteSpace())
                                {
                                    record.InnerPatient.InsuranceDocNumber = patient.INP;
                                }

                                break;
                        }

                        var meventResult = _repository.GetMeventsByPatientId(patient.PatientId);
                        if (meventResult.Success)
                        {
                            var events = new List<IMEventAnswer>();
                            foreach (FactMedicalEvent factMedicalEvent in meventResult.Data)
                            {
                                factMedicalEvent.DoctorId = null;
                                var meventCopy = Map.ObjectToObject<T6>(factMedicalEvent, typeof(T6));

                                var refusals = new List<object>();
                                decimal? totalMec = 0.0m;
                                decimal? totalMee = 0.0m;
                                decimal? totalEqma = 0.0m;

                                var refusalsMecResult = _repository.GetMecByMedicalEventId(factMedicalEvent.MedicalEventId);
                                if (refusalsMecResult.Success &&
                                    refusalsMecResult.Data.Any(p => (p.IsLock == null || p.IsLock == false)))
                                {
                                    switch (version)
                                    {
                                        case Constants.Version10:
                                            refusals.AddRange(refusalsMecResult.Data
                                                .Where(p => (p.IsLock == null || p.IsLock == false) && p.ReasonId.HasValue)
                                                .Select(p => p.ReasonId.Value as object)
                                                .ToList());
                                            break;
                                        case Constants.Version21:
                                            foreach (var refuse in refusalsMecResult.Data.Where(p => (p.IsLock == null || p.IsLock == false) && p.ReasonId.HasValue))
                                            {
                                                var refuseCopy = refuse;
                                                var refusal = new v21.E.RefusalE
                                                {
                                                    RefusalCode = refuseCopy.ReasonId,
                                                    RefusalRate = refuseCopy.Amount,
                                                    RefusalSource = refuseCopy.Source,
                                                    RefusalType = (int)RefusalType.MEC,
                                                    ExternalGuid = refuseCopy.ExternalGuid,
                                                    Comments = refuseCopy.Comments
                                                };

                                                refusals.Add(refusal);
                                            }
                                            totalMec += refusalsMecResult.Data.Sum(p => p.Amount) ?? 0;
                                            break;
                                    }

                                }

                                var refusalsExternalResult = _repository.GetExternalRefuseByMedicalEventId(factMedicalEvent.MedicalEventId);
                                if (refusalsExternalResult.Success)
                                {
                                    switch (version)
                                    {
                                        case Constants.Version10:
                                            refusals.AddRange(refusalsExternalResult.Data
                                                .Select(p => p.ReasonId as object)
                                                .ToList());
                                            break;
                                        case Constants.Version21:
                                            foreach (var refuse in refusalsExternalResult.Data)
                                            {
                                                var refuseCopy = refuse;
                                                var refusal = new v21.E.RefusalE
                                                {
                                                    RefusalCode = refuseCopy.ReasonId,
                                                    RefusalRate = refuseCopy.Amount,
                                                    RefusalSource = refuseCopy.Source,
                                                    RefusalType = refuseCopy.Type,
                                                    ExternalGuid = refuseCopy.ExternalGuid,
                                                    Comments = refuseCopy.Comment
                                                };

                                                refusals.Add(refusal);
                                            }
                                            totalMec += refusalsExternalResult.Data.Where(p=>p.Type == (int)RefusalType.MEC).Sum(p => p.Amount) ?? 0;
                                            totalMee += refusalsExternalResult.Data.Where(p => p.Type ==(int)RefusalType.MEE).Sum(p => p.Amount) ?? 0;
                                            totalEqma += refusalsExternalResult.Data.Where(p => p.Type ==(int)RefusalType.EQMA).Sum(p => p.Amount) ?? 0;
                                            break;
                                    }
                                }

                                var refusalsMeeResult = _repository.GetMeeByMedicalEventId(factMedicalEvent.MedicalEventId);
                                if (refusalsMeeResult.Success)
                                {
                                    switch (version)
                                    {
                                        case Constants.Version10:
                                            refusals.AddRange(refusalsMeeResult.Data
                                                .Select(p => p.ReasonId as object)
                                                .ToList());
                                            break;
                                        case Constants.Version21:
                                            foreach (var refuse in refusalsMeeResult.Data)
                                            {
                                                var refuseCopy = refuse;
                                                var refusal = new v21.E.RefusalE
                                                {
                                                    RefusalCode = refuseCopy.ReasonId,
                                                    RefusalRate = refuseCopy.Amount,
                                                    RefusalSource = refuseCopy.Source,
                                                    RefusalType = (int)RefusalType.MEE,
                                                    ExternalGuid = refuseCopy.ExternalGuid,
                                                    Comments = refuseCopy.Comments
                                                };

                                                refusals.Add(refusal);
                                            }
                                            totalMee += refusalsMeeResult.Data.Sum(p => p.Amount) ?? 0;
                                            break;
                                    }
                                }

                                var refusalsEqmaResult = _repository.GetEqmaByMedicalEventId(factMedicalEvent.MedicalEventId);
                                if (refusalsEqmaResult.Success)
                                {
                                    switch (version)
                                    {
                                        case Constants.Version10:
                                            refusals.AddRange(refusalsEqmaResult.Data
                                                .Select(p => p.ReasonId as object)
                                                .ToList());
                                            break;
                                        case Constants.Version21:
                                            foreach (var refuse in refusalsEqmaResult.Data)
                                            {
                                                var refuseCopy = refuse;
                                                var refusal = new v21.E.RefusalE
                                                {
                                                    RefusalCode = refuseCopy.ReasonId,
                                                    RefusalRate = refuseCopy.Amount,
                                                    RefusalSource = refuseCopy.Source,
                                                    RefusalType = (int)RefusalType.EQMA,
                                                    ExternalGuid = refuseCopy.ExternalGuid,
                                                    Comments = refuseCopy.Comments
                                                };

                                                refusals.Add(refusal);
                                            }
                                            totalEqma += refusalsEqmaResult.Data.Sum(p => p.Amount) ?? 0;
                                            break;
                                    }
                                }


                                meventCopy.InnerRefusals = new List<object>(refusals);
                                meventCopy.RefusalPrice = Math.Min(totalMec.Value, meventCopy.Price??0) + totalMee + totalEqma;
                                events.Add(meventCopy);
                            }
                            record.InnerEventCollection = new List<IMEventAnswer>(events);
                        }

                        records.Add(record);
                    }
                    register.InnerRecordCollection = new List<IRecordAnswer>(records);
                    register.InnerRecordCollection = register.InnerRecordCollection.OrderBy(p => p.ExternalId).ToList();
                }


                if (!accountResult.Data.PacketNumber.HasValue)
                {
                    var packetNumberResult = _repository.GetFactExchangeLastPacketNumber(
                        exchange => exchange.Direction == accountResult.Data.Direction &&
                            exchange.Type == type &&
                            exchange.Date.Year == accountResult.Data.Date.Value.Year);
                    if (packetNumberResult.Success)
                    {
                        accountResult.Data.PacketNumber = packetNumberResult.Data;
                        accountResult.Data.PacketNumber = accountResult.Data.PacketNumber.HasValue ? accountResult.Data.PacketNumber + 1 : 1;
                    }
                }

                var f010Cache = _cache.Get(CacheRepository.F010Cache);
                var registerInfo = new RegisterEInfo
                {
                    Type = "A",
                    SourceTerritory = f010Cache.GetBackInt(accountResult.Data.Destination),
                    DestinationTerritory = f010Cache.GetBackInt(accountResult.Data.Source),
                    NumberXXXX = accountResult.Data.PacketNumber.Value,
                    YearXX = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0
                };

                result.Data = Tuple.Create(register, registerInfo, accountResult.Data.Direction ?? 0);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<Tuple<T1, RegisterEInfo, int>> WriteAnswerZ<T1, T2, T3, T4, T5, T6, T7>(int accountId, int version, int type)
            where T1 : IZRegisterAnswer, new()
            where T2 : IAccountAnswer, new()
            where T3 : IHeaderAnswer, new()
            where T4 : IZRecordAnswer, new()
            where T5 : IPatientAnswer, new()
            where T6 : IZslMeventAnswer, new()
            where T7 : IZMeventAnswer, new()
        {
            var result = new OperationResult<Tuple<T1, RegisterEInfo, int>>();
            try
            {
                var accountResult = _repository.GetTerritoryAccountById(accountId);
                if (accountResult.HasError)
                {
                    result.AddError(accountResult.LastError);
                    return result;
                }

                _repository.UpdateGuid(accountId);

                var register = new T1
                {
                    InnerAccount = Map.ObjectToObject<T2>(accountResult.Data, typeof(T2))
                };

                Expression<Func<FactPreparedReport, bool>> predicate =
                    i => i.ExternalId == accountId && (i.FactReport.Type == 1);

                var preparedReport = _repository.GetFactPreparedReport(predicate);
                if (preparedReport.HasError || !preparedReport.Data.Any())
                {
                    result.AddError("Нет актов мэк по данному счету");
                    return result;
                }
                register.InnerAccount.Year = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0;
                register.InnerAccount.Month = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Month : 0;
                register.InnerHeader = new T3
                {
                    TargetOkato = accountResult.Data.Destination,
                    Version = version,
                    Date = preparedReport.Success ? preparedReport.Data.First().Date : default(DateTime?)
                };

                var patientsResult = _repository.GetPatientsByAccountId(accountId);
                if (patientsResult.Success)
                {
                    var records = new List<IZRecordAnswer>();
                    foreach (FactPatient factPatient in patientsResult.Data)
                    {
                        FactPatient patient = factPatient;
                        FactPerson person = null;

                        if (patient.PersonalId.HasValue)
                        {
                            var personResult = _repository.GetPersonById(patient.PersonalId.Value);
                            if (personResult.Success)
                            {
                                person = personResult.Data;
                            }
                        }

                        var record = new T4();
                        record.InnerPatient = Map.ObjectToObject<T5>(person, typeof(T5));

                        record.ExternalId = factPatient.ExternalId ?? 0;

                        record.InnerPatient.InsuranceDocType = patient.InsuranceDocType;
                        record.InnerPatient.InsuranceDocSeries = patient.InsuranceDocSeries;
                        record.InnerPatient.InsuranceDocNumber = patient.InsuranceDocNumber;
                        record.InnerPatient.INP = patient.INP;

                        //switch (version)
                        //{
                        //    case Constants.Version30:
                        //        record.InnerPatient.InsuranceDocNumber = patient.InsuranceDocNumber;
                                
                        //        if (record.InnerPatient.InsuranceDocNumber.IsNullOrWhiteSpace() && patient.INP.IsNotNullOrWhiteSpace())
                        //        {
                        //            record.InnerPatient.InsuranceDocNumber = patient.INP;
                        //        }
                        //        break;
                        //}

                        var meventResult = _repository.GetZslMeventsByPatientId(patient.PatientId);
                        if (meventResult.Success)
                        {
                            var zslEvents = new List<IZslMeventAnswer>();
                            foreach (ZslFactMedicalEvent factMedicalEvent in meventResult.Data)
                            {
                                var zslMeventCopy = Map.ObjectToObject<T6>(factMedicalEvent, typeof (T6));
                                decimal? totalMec = 0.0m;
                                decimal? totalMee = 0.0m;
                                decimal? totalEqma = 0.0m;
                                int countMec = 0;

                                var zevents = new List<IZMeventAnswer>();
                                var zmeventsResult =
                                    _repository.GetZMeventsByZslMeventId(factMedicalEvent.ZslMedicalEventId);
                                if (zmeventsResult.Success)
                                {
                                    var refusals31 = new List<IRefusal>();
                                    foreach (var zFactMedicalEvent in zmeventsResult.Data)
                                    {
                                        var meventCopy = Map.ObjectToObject<T7>(zFactMedicalEvent, typeof(T7));

                                        var refusals = new List<IRefusal>();

                                        var refusalsResult =
                                            _repository.GetSankByZMedicalEventIdAndType(mec=>mec.ZmedicalEventId == zFactMedicalEvent.ZmedicalEventId);
                                        if (refusalsResult.Success &&
                                            refusalsResult.Data.Any(p => (p.IsLock == null || p.IsLock == false)))
                                        {
                                            foreach (var refuse in refusalsResult.Data.Where( p =>
                                                            (p.IsLock == null || p.IsLock == false) &&
                                                            p.ReasonId.HasValue))
                                            {
                                                var refuseCopy = refuse;
                                                if (version==Constants.Version30)
                                                {
                                                    var refusal = new v30.E.RefusalE
                                                    {
                                                        RefusalCode = refuseCopy.ReasonId,
                                                        RefusalRate = refuseCopy.Amount,
                                                        RefusalSource = refuseCopy.Source,
                                                        RefusalType = refuseCopy.Type,
                                                        ExternalGuid = refuseCopy.ExternalGuid,
                                                        Comments = refuseCopy.Comments
                                                    };

                                                    refusals.Add(refusal);
                                                }
                                                if (version == Constants.Version31)
                                                {
                                                    var guidmeventId = _repository.GetZMedicalEventByExternalId(zFactMedicalEvent.ZmedicalEventId);

                                                    var refusal = new v31.E.RefusalE
                                                    {
                                                        RefusalCode = refuseCopy.ReasonId,
                                                        RefusalRate = refuseCopy.Amount,
                                                        RefusalSource = refuseCopy.Source,
                                                        Date = refuseCopy.Date,
                                                        NumAct = preparedReport.Success ? preparedReport.Data.Single()?.Number.ToString() : null,
                                                        CodeExp = refuseCopy.CodeExp,
                                                        RefusalType = refuseCopy.Type,
                                                        ExternalGuid = refuseCopy.ExternalGuid,
                                                        SlidGuid = guidmeventId.Data,
                                                        Comments = refuseCopy.Comments
                                                    };

                                                    refusals31.Add(refusal);
                                                }
                                                if (version == Constants.Version32)
                                                {
                                                    var guidmeventId = _repository.GetZMedicalEventByExternalId(zFactMedicalEvent.ZmedicalEventId);

                                                    var refusal = new v32.E.RefusalE
                                                    {
                                                        RefusalCode = refuseCopy.ReasonId,
                                                        RefusalRate = refuseCopy.Amount,
                                                        RefusalSource = refuseCopy.Source,
                                                        Date = refuseCopy.Date,
                                                        NumAct = preparedReport.Success ? preparedReport.Data.Single()?.Number.ToString() : null,
                                                        CodeExp = refuseCopy.CodeExp,
                                                        RefusalType = refuseCopy.Type,
                                                        ExternalGuid = refuseCopy.ExternalGuid,
                                                        SlidGuid = guidmeventId.Data,
                                                        Comments = refuseCopy.Comments
                                                    };

                                                    refusals31.Add(refusal);
                                                }

                                                if (refuse.Type == 1)countMec = countMec + 1;
                                            }
                                            totalMec += refusalsResult.Data.Where(p=>p.Type == 1).Sum(p => p.Amount) ?? 0;
                                            totalMee += refusalsResult.Data.Where(p => Constants.Mee.Contains(p.Type)).Sum(p => p.Amount) ?? 0;
                                            totalEqma += refusalsResult.Data.Where(p => Constants.Eqma.Contains(p.Type)).Sum(p => p.Amount) ?? 0;
                                        }

                                        var refusalsExternalResult =
                                            _repository.GetZExternalRefuseByMedicalEventId(zFactMedicalEvent.ZmedicalEventId);
                                        if (refusalsExternalResult.Success)
                                        {
                                            foreach (var refuse in refusalsExternalResult.Data)
                                            {
                                                var refuseCopy = refuse;
                                                if (version == Constants.Version30)
                                                {
                                                    var refusal = new v30.E.RefusalE
                                                    {
                                                        RefusalCode = refuseCopy.ReasonId,
                                                        RefusalRate = refuseCopy.Amount,
                                                        RefusalSource = refuseCopy.Source,
                                                        RefusalType = refuseCopy.Type,
                                                        ExternalGuid = refuseCopy.ExternalGuid,
                                                        Comments = refuseCopy.Comment
                                                    };

                                                    refusals.Add(refusal);
                                                }
                                                if (version == Constants.Version31)
                                                {
                                                    var guidmeventId = _repository.GetZMedicalEventByExternalId(zFactMedicalEvent.ZmedicalEventId);

                                                    var refusal = new v31.E.RefusalE
                                                    {
                                                        RefusalCode = refuseCopy.ReasonId,
                                                        RefusalRate = refuseCopy.Amount,
                                                        RefusalSource = refuseCopy.Source,
                                                        RefusalType = refuseCopy.Type,
                                                        Date = refuseCopy.Date,
                                                        NumAct = preparedReport.Success ? preparedReport.Data.Single()?.Number.ToString() : null,
                                                        CodeExp = refuseCopy.CodeExp,
                                                        ExternalGuid = refuseCopy.ExternalGuid,
                                                        SlidGuid = guidmeventId.Data,
                                                        Comments = refuseCopy.Comment
                                                    };

                                                    refusals31.Add(refusal);
                                                }
                                                if (version == Constants.Version32)
                                                {
                                                    var guidmeventId = _repository.GetZMedicalEventByExternalId(zFactMedicalEvent.ZmedicalEventId);

                                                    var refusal = new v32.E.RefusalE
                                                    {
                                                        RefusalCode = refuseCopy.ReasonId,
                                                        RefusalRate = refuseCopy.Amount,
                                                        RefusalSource = refuseCopy.Source,
                                                        RefusalType = refuseCopy.Type,
                                                        Date = refuseCopy.Date,
                                                        NumAct = preparedReport.Success ? preparedReport.Data.Single()?.Number.ToString() : null,
                                                        CodeExp = refuseCopy.CodeExp,
                                                        ExternalGuid = refuseCopy.ExternalGuid,
                                                        SlidGuid = guidmeventId.Data,
                                                        Comments = refuseCopy.Comment
                                                    };

                                                    refusals31.Add(refusal);
                                                }
                                                if (refuse.Type == 1) countMec = countMec + 1;
                                            }
                                            totalMec += refusalsExternalResult.Data.Where(p => p.Type == (int) RefusalType.MEC).Sum(p => p.Amount) ?? 0;
                                            totalMee += refusalsExternalResult.Data.Where(p => Constants.Mee.Contains(p.Type)).Sum(p => p.Amount) ?? 0;
                                            totalEqma += refusalsExternalResult.Data.Where(p => Constants.Eqma.Contains(p.Type)).Sum(p => p.Amount) ?? 0;

                                        }

                                        if(version == Constants.Version30)meventCopy.InnerRefusals = new List<IRefusal>(refusals);
                                        zevents.Add(meventCopy);
                                    }
                                    if(version == Constants.Version31)zslMeventCopy.InnerRefusals = new List<IRefusal>(refusals31);
                                    if (version == Constants.Version32) zslMeventCopy.InnerRefusals = new List<IRefusal>(refusals31);

                                    
                                    zslMeventCopy.InnerEventCollection = new List<IZMeventAnswer>(zevents);
                                    zslMeventCopy.RefusalPrice = totalMec.Value >= 0 && countMec > 0 ? 
                                        zslMeventCopy.Price + totalMee + totalEqma : totalMee + totalEqma;
                                    if (zslMeventCopy.RefusalPrice == 0) zslMeventCopy.RefusalPrice = null;

                                    zslEvents.Add(zslMeventCopy);
                                }
                                record.InnerEventCollection = new List<IZslMeventAnswer>(zslEvents);
                            }
                        }
                        records.Add(record);
                    }
                    register.InnerRecordCollection = new List<IZRecordAnswer>(records);
                    register.InnerRecordCollection = register.InnerRecordCollection.OrderBy(p => p.ExternalId).ToList();
                }


                if (!accountResult.Data.PacketNumber.HasValue)
                {
                    var packetNumberResult = _repository.GetFactExchangeLastPacketNumber(
                        exchange => exchange.Direction == accountResult.Data.Direction &&
                            exchange.Type == type &&
                            exchange.Date.Year == accountResult.Data.Date.Value.Year);
                    if (packetNumberResult.Success)
                    {
                        accountResult.Data.PacketNumber = packetNumberResult.Data;
                        accountResult.Data.PacketNumber = accountResult.Data.PacketNumber.HasValue ? accountResult.Data.PacketNumber + 1 : 1;
                    }
                }

                var f010Cache = _cache.Get(CacheRepository.F010Cache);
                var registerInfo = new RegisterEInfo
                {
                    Type = "A",
                    SourceTerritory = f010Cache.GetBackInt(accountResult.Data.Destination),
                    DestinationTerritory = f010Cache.GetBackInt(accountResult.Data.Source),
                    NumberXXXX = accountResult.Data.PacketNumber.Value,
                    YearXX = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0
                };

                result.Data = Tuple.Create(register, registerInfo, accountResult.Data.Direction ?? 0);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }
        
        public OperationResult<Tuple<T1, RegisterEInfo, int>> WriteAnswer1<T1, T2, T3, T4, T5, T6>(int accountId, int economicAccountId, int version, int type)
            where T1 : IRegisterPlAnswer, new()
            where T2 : InformationPlAnswer, new()
            where T3 : IHeaderAnswer, new()
            where T4 : IRecordAnswer, new()
            where T5 : IPatientAnswer, new()
            where T6 : IMEventAnswer, new()
        {
            var result = new OperationResult<Tuple<T1, RegisterEInfo, int>>();
            try
            {
                var economicAccountResult = _repository.GetEconomicAccountById(economicAccountId);
                if (economicAccountResult.HasError)
                {
                    result.AddError(economicAccountResult.LastError);
                    return result;
                }

                var accountResult = _repository.GetTerritoryAccountById(accountId);
                if (accountResult.HasError)
                {
                    result.AddError(accountResult.LastError);
                    return result;
                }

                _repository.UpdateGuid(accountId);
                var register = new T1
                {
                    InnerAccount = Map.ObjectToObject<T2>(accountResult.Data, typeof(T2))
                };
                //Номер платежного поручения
                register.InnerAccount.PaymentOrderNumber = economicAccountResult.Data.PaymentOrder;
                //Дата платежного поручения
                register.InnerAccount.DatePaymentOrder = economicAccountResult.Data.PaymentOrderDate;

                var qq = _repository.GetEconomicAccountParametrById(economicAccountResult.Data.PaymentOrder,
                    economicAccountResult.Data.PaymentDate);
                //Количество счетов
                register.InnerAccount.NumberAccount = qq.Data.Count().ToString();
                register.InnerAccount.Account = new List<AccountPlAnswer>();

                decimal? summaTotal = 0;
                int[] eType = {1, 2};
                foreach (var factEconomicAccount in qq.Data)
                {
                    var factEx = _repository.GetFactExchange(x => x.AccountId == factEconomicAccount.AccountId && eType.Contains(x.Type) && x.Destination == Convert.ToString(TerritoryService.TfCode));

                    if (factEx.HasError)
                    {
                        result.AddError(factEx.LastError);
                        return result;
                    }

                    if (!factEx.Data.Any())
                    {
                        result.AddError("Название файла не найдены");
                        return result;
                    }
                    var accResult = _repository.GetTerritoryAccountById(factEconomicAccount.AccountId);
                    if (accResult.HasError)
                    {
                        result.AddError(accResult.LastError);
                        return result;
                    }

                    var ecPay = _repository.GetPaymentsByEconomicAccountId(factEconomicAccount.EconomicAccountId);
                    if (ecPay.HasError)
                    {
                        result.AddError(ecPay.LastError);
                        return result;
                    }
                    int?[] assistanceConditions = new int?[ecPay.Data.Count()];
                    int i = 0;
                    foreach (var factEconomicPayment in ecPay.Data)
                    {
                        assistanceConditions[i] = factEconomicPayment.AssistanceConditionsId;
                        i++;
                    }
                    var numberSlAccount = _repository.GetKolPaySluch(factEconomicAccount.AccountId, assistanceConditions);
                    var kol = (factEx.Data?.FirstOrDefault()?.FileName)?.IndexOf(".", 0);

                    register.InnerAccount.Account.Add(
                        new AccountPlAnswer
                        {
                            AccountNumber = accResult.Data.AccountNumber,
                            AccountDate = accResult.Data.AccountDate,
                            NameIshodReestr = (kol != -1)?factEx.Data?.FirstOrDefault()?.FileName?.Substring(0, (int)kol): factEx.Data?.FirstOrDefault()?.FileName,
                            SuumAccount = accResult.Data.TotalPaymentAmount,
                            NumberSlAccount = numberSlAccount.Data
                        });
                    summaTotal = summaTotal + accResult.Data.TotalPaymentAmount;
                    
                    register.InnerAccount.Total = accountResult.Data.Price;
                }
                register.InnerAccount.SubjectOfPayment = economicAccountResult.Data.Comments;
                register.InnerAccount.Total = summaTotal;


                var innerRecipient = _repository.GetEconomicPartner(accountResult.Data.Source);
                if (innerRecipient.HasError)
                {
                    result.AddError(innerRecipient.LastError);
                    return result;
                }
                var innerRec = innerRecipient.Data.FirstOrDefault();
                register.InnerAccount.InnerRecipient = new RecipientInformationPlAnswer
                {
                    NameRecipient   = innerRec?.Name,
                    AdressRecipient = innerRec?.Adr,
                    BankRecipient   = innerRec?.Bank,
                    RsRecipient     = innerRec?.Rs.ToString(),
                    BicRecipient    = innerRec?.Bic,
                    InnRecipient    = innerRec?.Inn,
                    KppRecipient    = innerRec?.Kpp,
                    KbkRecipient    = Convert.ToString(innerRec?.Kbk),
                    OktmoRecipient  = innerRec?.Oktmo
                };

                var innerPayer = _repository.GetEconomicPartner(accountResult.Data.Destination);
                if (innerPayer.HasError)
                {
                    result.AddError(innerPayer.LastError);
                    return result;
                }
                var innerPay = innerPayer.Data.FirstOrDefault();
                register.InnerAccount.InnerPayer = new PayerInformationPlAnswer
                {
                    NamePayer = innerPay?.Name,
                    AdressPayer = innerPay?.Adr,
                    BankPayer = innerPay?.Bank,
                    RsPayer = innerPay?.Rs.ToString(),
                    BicPayer = innerPay?.Bic,
                    InnPayer = innerPay?.Inn,
                    KppPayer = innerPay?.Kpp,
                    OktmoPayer = innerPay?.Oktmo
                };                
                        
                var f010Cache = _cache.Get(CacheRepository.F010Cache);
                var registerInfo = new RegisterEInfo
                {
                    Type = "PL",
                    SourceTerritory = f010Cache.GetBackInt(accountResult.Data.Source),
                    DestinationTerritory = f010Cache.GetBackInt(accountResult.Data.Destination),
                    NumberXXXX = accountResult.Data.PacketNumber.Value,
                    YearXX = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0
                };

                register.FileName = registerInfo.FileName.Replace(".oms", "") + ".oms";
                
                result.Data = Tuple.Create(register, registerInfo, accountResult.Data.Direction ?? 0);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<Tuple<T1, RegisterEInfo, int>> WritePaymentOrder<T1, T2, T3, T4, T5, T6>(int accountId, int economicAccountId, int version, int type)
            where T1 : IRegisterPlAnswer, new()
            where T2 : InformationPlAnswer, new()
            where T3 : IHeaderAnswer, new()
            where T4 : IRecordAnswer, new()
            where T5 : IPatientAnswer, new()
            where T6 : IMEventAnswer, new()
        {
            var result = new OperationResult<Tuple<T1, RegisterEInfo, int>>();
            try
            {
                var economicAccountResult = _repository.GetEconomicAccountById(economicAccountId);
                if (economicAccountResult.HasError)
                {
                    result.AddError(economicAccountResult.LastError);
                    return result;
                }
                dynamic terCode = _remoteSettings.Get(AppRemoteSettings.TerritoriCode);
                string dest = Convert.ToString(terCode.tf_code);
                var accountResult = _repository.GetTerritoryAccountById(accountId);
                if (accountResult.HasError)
                {
                    result.AddError(accountResult.LastError);
                    return result;
                }

                _repository.UpdateGuid(accountId);
                var register = new T1
                {
                    InnerAccount = Map.ObjectToObject<T2>(accountResult.Data, typeof(T2))
                };
                //Номер платежного поручения
                register.InnerAccount.PaymentOrderNumber = economicAccountResult.Data.PaymentOrder;
                //Дата платежного поручения
                register.InnerAccount.DatePaymentOrder = economicAccountResult.Data.PaymentOrderDate;

                var qq = _repository.GetEconomicAccountParametrById(economicAccountResult.Data.PaymentOrder,
                    economicAccountResult.Data.PaymentDate);
                //Количество счетов
                register.InnerAccount.NumberAccount = qq.Data.Count().ToString();
                register.InnerAccount.Account = new List<AccountPlAnswer>();

                decimal? summaTotal = 0;
                int[] eType = { 1, 2 };
                foreach (var factEconomicAccount in qq.Data)
                {
                    var factEx = _repository.GetFactExchange(x => x.AccountId == factEconomicAccount.AccountId && eType.Contains(x.Type) && x.Destination == dest);

                    if (factEx.HasError)
                    {
                        result.AddError(factEx.LastError);
                        return result;
                    }

                    if (!factEx.Data.Any())
                    {
                        result.AddError("Название файла не найдены");
                        return result;
                    }
                    var accResult = _repository.GetTerritoryAccountById(factEconomicAccount.AccountId);
                    if (accResult.HasError)
                    {
                        result.AddError(accResult.LastError);
                        return result;
                    }

                    var ecPay = _repository.GetPaymentsByEconomicAccountId(factEconomicAccount.EconomicAccountId);
                    if (ecPay.HasError)
                    {
                        result.AddError(ecPay.LastError);
                        return result;
                    }
                    int?[] assistanceConditions = new int?[ecPay.Data.Count()];
                    int i = 0;
                    foreach (var factEconomicPayment in ecPay.Data)
                    {
                        assistanceConditions[i] = factEconomicPayment.AssistanceConditionsId;
                        i++;
                    }
                    var numberSlAccount = _repository.GetKolPayZSluch(factEconomicAccount.AccountId, assistanceConditions);
                    var kol = (factEx.Data?.FirstOrDefault()?.FileName)?.IndexOf(".", 0);

                    register.InnerAccount.Account.Add(
                        new AccountPlAnswer
                        {
                            AccountNumber = accResult.Data.AccountNumber,
                            AccountDate = accResult.Data.AccountDate,
                            NameIshodReestr = (kol != -1) ? factEx.Data?.FirstOrDefault()?.FileName?.Substring(0, (int)kol) : factEx.Data?.FirstOrDefault()?.FileName,
                            SuumAccount = accResult.Data.TotalPaymentAmount,
                            NumberSlAccount = numberSlAccount.Data
                        });
                    summaTotal = summaTotal + accResult.Data.TotalPaymentAmount;

                    register.InnerAccount.Total = accountResult.Data.Price;
                }
                register.InnerAccount.SubjectOfPayment = economicAccountResult.Data.Comments;
                register.InnerAccount.Total = summaTotal;


                var innerRecipient = _repository.GetEconomicPartner(accountResult.Data.Source);
                if (innerRecipient.HasError)
                {
                    result.AddError(innerRecipient.LastError);
                    return result;
                }
                var innerRec = innerRecipient.Data.FirstOrDefault();
                register.InnerAccount.InnerRecipient = new RecipientInformationPlAnswer
                {
                    NameRecipient = innerRec?.Name,
                    AdressRecipient = innerRec?.Adr,
                    BankRecipient = innerRec?.Bank,
                    RsRecipient = innerRec?.Rs.ToString(),
                    BicRecipient = innerRec?.Bic,
                    InnRecipient = innerRec?.Inn,
                    KppRecipient = innerRec?.Kpp,
                    KbkRecipient = Convert.ToString(innerRec?.Kbk),
                    OktmoRecipient = innerRec?.Oktmo
                };

                var innerPayer = _repository.GetEconomicPartner(accountResult.Data.Destination);
                if (innerPayer.HasError)
                {
                    result.AddError(innerPayer.LastError);
                    return result;
                }
                var innerPay = innerPayer.Data.FirstOrDefault();
                register.InnerAccount.InnerPayer = new PayerInformationPlAnswer
                {
                    NamePayer = innerPay?.Name,
                    AdressPayer = innerPay?.Adr,
                    BankPayer = innerPay?.Bank,
                    RsPayer = innerPay?.Rs.ToString(),
                    BicPayer = innerPay?.Bic,
                    InnPayer = innerPay?.Inn,
                    KppPayer = innerPay?.Kpp,
                    OktmoPayer = innerPay?.Oktmo
                };

                var f010Cache = _cache.Get(CacheRepository.F010Cache);
                var registerInfo = new RegisterEInfo
                {
                    Type = "PL",
                    SourceTerritory = f010Cache.GetBackInt(accountResult.Data.Source),
                    DestinationTerritory = f010Cache.GetBackInt(accountResult.Data.Destination),
                    NumberXXXX = accountResult.Data.PacketNumber.Value,
                    YearXX = accountResult.Data.Date.HasValue ? accountResult.Data.Date.Value.Year : 0
                };

                register.FileName = registerInfo.FileName.Replace(".oms", "") + ".oms";

                result.Data = Tuple.Create(register, registerInfo, accountResult.Data.Direction ?? 0);
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

    }
}
