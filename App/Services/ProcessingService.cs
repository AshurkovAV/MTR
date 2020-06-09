using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Core;
using Core.Extensions;
using Core.Infrastructure;
using DataModel;
using Medical.AppLayer.Managers;
using Medical.AppLayer.Models.PolicySearch;
using Medical.AppLayer.Processing.ViewModels;
using Medical.DatabaseCore.Services.Database;
using Remote.SrzServiceReference;

namespace Medical.AppLayer.Services
{
    public class ProcessingService : IProcessingService
    {
        private readonly IMedicineRepository _repository;
        private readonly IRemoteService _remoteService;
        private readonly ITortillaRepository _tortillaRepository;
        private readonly IDockLayoutManager _dockLayoutManager;
        private readonly IDataService _dataService;

        private readonly int _userId;

        public ProcessingService(IMedicineRepository repository, 
            IRemoteService remoteService, 
            ITortillaRepository tortillaRepository,
            IDataService dataService,
            IUserService userService,
            IDockLayoutManager dockLayoutManager)
        {
            _userId = userService.UserId;

            _repository = repository;
            _remoteService = remoteService;
            _tortillaRepository = tortillaRepository;
            _dataService = dataService;
            _dockLayoutManager = dockLayoutManager;
        }

        private OperationResult<Remote.SrzServiceReference.DataServiceClient> GetPolicyRemoteService()
        {
            var result = new OperationResult<Remote.SrzServiceReference.DataServiceClient>();
            try
            {
                var serviceResult = _remoteService.GetClient<Remote.SrzServiceReference.DataServiceClient>(AppRemoteSettings.SrzServiceConnectionString);
                if (!serviceResult.Success)
                {
                    result.AddError(serviceResult.LastError);
                }
                else
                {
                    result.Data = serviceResult.Data;
                }

            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        } 

        public OperationResult<IEnumerable<PolicyCheckPatientModel>>  CheckAccountPolicy(int accountId)
        {
            var result = new OperationResult<IEnumerable<PolicyCheckPatientModel>>();
            try
            {
                var meventResult = _repository.GetEventShortViewByAccountId(accountId);
                if (meventResult.Success)
                {
                    var checkPolicyResult = CheckPatientPolicy(meventResult.Data);
                    if (checkPolicyResult.Success)
                    {
                        result.Data = checkPolicyResult.Data;
                    }
                    else
                    {
                        result.AddError(checkPolicyResult.LastError);
                    }
                }
                
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult<IEnumerable<PolicyCheckPatientModel>> CheckZAccountPolicy(int accountId)
        {
            var result = new OperationResult<IEnumerable<PolicyCheckPatientModel>>();
            try
            {
                var meventResult = _repository.GetZslEventShortViewByAccountId(accountId);
                if (meventResult.Success)
                {
                    var checkPolicyResult = CheckPatientPolicy(meventResult.Data);
                    if (checkPolicyResult.Success)
                    {
                        result.Data = checkPolicyResult.Data;
                    }
                    else
                    {
                        result.AddError(checkPolicyResult.LastError);
                    }
                }

            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        private OperationResult<IEnumerable<PolicyCheckPatientModel>> CheckPatientPolicy(IEnumerable<ZslEventShortView> meventResult)
        {
            var result = new OperationResult<IEnumerable<PolicyCheckPatientModel>>();
            var errors = new List<PolicyCheckPatientModel>();
            _dockLayoutManager.ShowOverlay(Constants.RunCheckDataTitleMsg, Constants.PleaseWaitMsg);

            double total = meventResult.GroupBy(p => p.PatientId).Count();
            double count = 0;

            var policyRemoteServiceResult = GetPolicyRemoteService();
            if (policyRemoteServiceResult.HasError)
            {
                result.AddError(policyRemoteServiceResult.LastError);
                return result;
            }
            var service = policyRemoteServiceResult.Data;

            foreach (var mevents in meventResult.GroupBy(p => p.PatientId))
            {
                var mevent = mevents.First();
                var policyNumber = mevent.InsuranceDocType == (int)PolicyType.INP
                    ? mevent.INP
                    : mevent.InsuranceDocNumber;
                var policySeries = mevent.InsuranceDocSeries ?? string.Empty;

                var peopleResult = service.GetPeopleListByPolicy(mevent.InsuranceDocType ?? 1, policyNumber,
                    policySeries);
                if (peopleResult != null && peopleResult.Any())
                {
                    var peoples = new List<People>(peopleResult);
                    if (peoples.Count == 1)
                    {
                        var human = peoples.First();
                        if (!human.Surname.Equals(mevent.Surname, StringComparison.InvariantCultureIgnoreCase) ||
                            !human.FirstName.Equals(mevent.Name, StringComparison.InvariantCultureIgnoreCase) ||
                            !human.Patronymic.Equals(mevent.Patronymic, StringComparison.InvariantCultureIgnoreCase) ||
                            human.Sex != mevent.SexCode ||
                            human.Birthday != mevent.Birthday)
                        {
                            errors.Add(FillPolicyCheckModel(mevent.PatientId, mevent.PatientData, mevents, "Не совпадают персональные данные"));
                        }
                        else
                        {
                            //ЕНП тоже необходимо проверять
                            //if (mevent.InsuranceDocType != 3)
                            //{
                            var policyList = service.GetPolicyByPeopleId(peoples.First().PeopleId);

                            if (policyList != null && policyList.Any())
                            {

                                var dateCheck = policyList.Where(p => p.PolicyNumber == policyNumber &&
                                                                        ((p.DateStop >= mevent.EventBegin) ||
                                                                        p.DateStop == null) &&
                                                                        ((p.DateEnd >= mevent.EventBegin) ||
                                                                        p.DateEnd == null) &&
                                                                        p.DateBegin <= mevent.EventBegin).ToList();
                                if (dateCheck.Count == 0)
                                {
                                    errors.Add(FillPolicyCheckModel(mevent.PatientId, mevent.PatientData, mevents, "Не найден действующий полис"));
                                }
                                else if (dateCheck.Count > 1)
                                {
                                    //TODO add few policy check
                                }
                            }
                            //}

                        }
                    }
                    else
                    {
                        errors.Add(FillPolicyCheckModel(mevent.PatientId, mevent.PatientData, mevents, "Найден > чем 1 человек с данным полисом"));
                    }
                }
                else
                {
                    errors.Add(FillPolicyCheckModel(mevent.PatientId, mevent.PatientData, mevents, "Не найден номер полиса"));
                }
                _dockLayoutManager.SetOverlayProgress(total, count++);
            }

            result.Data = errors;
            return result;
        }

        public OperationResult<PolicyCheckPatientModel> CheckPatientPolicy(int patientId)
        {
            var result = new OperationResult<PolicyCheckPatientModel>();
            try
            {
                var meventResult = _repository.GetEventShortViewByPatientId(patientId);
                if (meventResult.Success)
                {
                    var checkPolicyResult = CheckPatientPolicy(meventResult.Data);
                    if (checkPolicyResult.Success)
                    {
                        //
                        result.Data = checkPolicyResult.Data.FirstOrDefault() ?? new PolicyCheckPatientModel { PatientId = patientId , Mevents = new ObservableCollection<PolicyCheckMedicalEventModel>()};
                        
                    }
                    else
                    {
                        result.AddError(checkPolicyResult.LastError);
                    }
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        public OperationResult<PolicyCheckPatientModel> CheckZslPatientPolicy(int patientId)
        {
            var result = new OperationResult<PolicyCheckPatientModel>();
            try
            {
                var meventResult = _repository.GetZslEventShortViewByPatientId(patientId);
                if (meventResult.Success)
                {
                    var checkPolicyResult = CheckPatientPolicy(meventResult.Data);
                    if (checkPolicyResult.Success)
                    {
                        //
                        result.Data = checkPolicyResult.Data.FirstOrDefault() ?? new PolicyCheckPatientModel { PatientId = patientId, Mevents = new ObservableCollection<PolicyCheckMedicalEventModel>() };

                    }
                    else
                    {
                        result.AddError(checkPolicyResult.LastError);
                    }
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            return result;
        }

        private OperationResult<IEnumerable<PolicyCheckPatientModel>> CheckPatientPolicy(IEnumerable<EventShortView> meventResult)
        {
            var result = new OperationResult<IEnumerable<PolicyCheckPatientModel>>();
            var errors = new List<PolicyCheckPatientModel>();
            _dockLayoutManager.ShowOverlay(Constants.RunCheckDataTitleMsg, Constants.PleaseWaitMsg);
            
            double total = meventResult.GroupBy(p => p.PatientId).Count();
            double count = 0;

            var policyRemoteServiceResult = GetPolicyRemoteService();
            if (policyRemoteServiceResult.HasError)
            {
                result.AddError(policyRemoteServiceResult.LastError);
                return result;
            }
            var service = policyRemoteServiceResult.Data;

            foreach (var mevents in meventResult.GroupBy(p => p.PatientId))
            {
                var mevent = mevents.First();
                var policyNumber = mevent.InsuranceDocType == (int) PolicyType.INP
                    ? mevent.INP
                    : mevent.InsuranceDocNumber;
                var policySeries = mevent.InsuranceDocSeries ?? string.Empty;

                var peopleResult = service.GetPeopleListByPolicy(mevent.InsuranceDocType ?? 1, policyNumber,
                    policySeries);
                if (peopleResult != null && peopleResult.Any())
                {
                    var peoples = new List<People>(peopleResult);
                    if (peoples.Count == 1)
                    {
                        var human = peoples.First();
                        if (!human.Surname.Equals(mevent.Surname, StringComparison.InvariantCultureIgnoreCase) ||
                            !human.FirstName.Equals(mevent.Name, StringComparison.InvariantCultureIgnoreCase) ||
                            !human.Patronymic.Equals(mevent.Patronymic, StringComparison.InvariantCultureIgnoreCase) ||
                            human.Sex != mevent.SexCode ||
                            human.Birthday != mevent.Birthday)
                        {
                            errors.Add(FillPolicyCheckModel(mevent.PatientId, mevent.PatientData, mevents, "Не совпадают персональные данные"));
                        }
                        else
                        {
                            //ЕНП тоже необходимо проверять
                            //if (mevent.InsuranceDocType != 3)
                            //{
                                var policyList = service.GetPolicyByPeopleId(peoples.First().PeopleId);

                                if (policyList != null && policyList.Any())
                                {

                                    var dateCheck = policyList.Where(p => p.PolicyNumber == policyNumber &&
                                                                            ((p.DateStop >= mevent.EventBegin) ||
                                                                            p.DateStop == null) &&
                                                                            ((p.DateEnd >= mevent.EventBegin) ||
                                                                            p.DateEnd == null) &&
                                                                            p.DateBegin <= mevent.EventBegin).ToList();
                                    if (dateCheck.Count == 0)
                                    {
                                        errors.Add(FillPolicyCheckModel(mevent.PatientId, mevent.PatientData, mevents, "Не найден действующий полис"));
                                    }
                                    else if (dateCheck.Count > 1)
                                    {
                                        //TODO add few policy check
                                    }
                                }
                            //}

                        }
                    }
                    else
                    {
                        errors.Add(FillPolicyCheckModel(mevent.PatientId, mevent.PatientData, mevents, "Найден > чем 1 человек с данным полисом"));
                    }
                }
                else
                {
                    errors.Add(FillPolicyCheckModel(mevent.PatientId, mevent.PatientData, mevents, "Не найден номер полиса"));
                }
                _dockLayoutManager.SetOverlayProgress(total, count++);
            }
            
            result.Data = errors;
            return result;
        }

        private PolicyCheckPatientModel FillPolicyCheckModel(int? patientId, string patientData, IGrouping<int?, EventShortView> mevents, string comment) {
            var meventsChecked = new List<PolicyCheckMedicalEventModel>();
            foreach (var m in mevents)
            {
                var mec = new PolicyCheckMedicalEventModel
                {
                    MedicalEventId = m.EventId,
                    Reason = Constants.PolicyRefusalId.Value,
                    Comments = comment,
                    Data = m.MeventData,
                    Amount = m.Price
                };
                meventsChecked.Add(mec);
            }
            var checkPatient = new PolicyCheckPatientModel
            {
                PatientId = patientId,
                Data = patientData,
                Mevents = new ObservableCollection<PolicyCheckMedicalEventModel>(meventsChecked)
            };
            return checkPatient;
        }

        private PolicyCheckPatientModel FillPolicyCheckModel(int? patientId, string patientData, IGrouping<int?, ZslEventShortView> mevents, string comment)
        {
            var meventsChecked = new List<PolicyCheckMedicalEventModel>();
            foreach (var m in mevents)
            {
                var mec = new PolicyCheckMedicalEventModel
                {
                    ZslMedicalEventId = m.ZslMedicalEventId,
                    MedicalEventId = m.EventId,
                    Reason = Constants.PolicyRefusalId.Value,
                    Comments = comment,
                    Data = m.MeventData,
                    Amount = m.Price,
                    ZslAmount = m.ZslPrice,
                };
                meventsChecked.Add(mec);
            }
            var checkPatient = new PolicyCheckPatientModel
            {
                PatientId = patientId,
                Data = patientData,
                Mevents = new ObservableCollection<PolicyCheckMedicalEventModel>(meventsChecked)
            };
            return checkPatient;
        }

        public OperationResult<IEnumerable<SearchPeopleByPolicyModel>> SearchPeopleByPolicy(int? insuranceDocType, string policyNumber, string policySeries)
        {
            var result = new OperationResult<IEnumerable<SearchPeopleByPolicyModel>>
            {
                Data = new List<SearchPeopleByPolicyModel>()
            };
            try
            {
                var policyRemoteServiceResult = GetPolicyRemoteService();
                if (policyRemoteServiceResult.HasError)
                {
                    result.AddError(policyRemoteServiceResult.LastError);
                    return result;
                }
                var service = policyRemoteServiceResult.Data;

                var peopleResult = service.GetPeopleListByPolicy(insuranceDocType ?? 1, policyNumber, policySeries);
                if (peopleResult != null && peopleResult.Any())
                {
                    result.Data = peopleResult.Select(p => new SearchPeopleByPolicyModel
                    {
                        Id = p.PeopleId,
                        FirstName = p.FirstName,
                        Surname = p.Surname,
                        Patronymic = p.Patronymic,
                        Sex = p.Sex,
                        Birthday = p.Birthday,
                        Inp = p.ENP
                    });
                }
            }
            catch (Exception ex)
            {
                result.AddError(ex.Message);
                return result;
            }
            

            return result;
        }

        public OperationResult<IEnumerable<ProcessingResultModel>> RunProcessing(int id, int scope, int version, List<object> selectedProcessingList)
        {
            var result = new OperationResult<IEnumerable<ProcessingResultModel>>();
            try
            {
                IEnumerable<FactProcessing> processingTotalList;
                if (selectedProcessingList == null)
                {
                    var processingResult = _repository.GetEnabledProcessingByScopeAndVersion(scope, version);
                    if (processingResult.Success)
                    {
                        processingTotalList = processingResult.Data;
                    }
                    else
                    {
                        result.AddError("Доступные фукции обработки данных для области {0} и версии {1} отсутствуют".F(scope, version));
                        return result;
                    }
                }
                else
                {
                    var processingResult = _repository.GetEnabledProcessingByIds(selectedProcessingList);
                    if (processingResult.Success)
                    {
                        processingTotalList = processingResult.Data;
                    }
                    else
                    {
                        result.AddError("Доступные критерии для области {0} и версии {1} отсутствуют".F(scope, version));
                        return result;
                    }
                }

                var resultList = new List<ProcessingResultModel>();

                foreach (var exam in processingTotalList.OrderByDescending(p=>p.Weight))
                {
                    _dockLayoutManager.SetOverlayMessage("Выполнение функции обработки данных:\n'{0}'".F(exam.Name));
                    TransactionResult<int> executeResult = null;
                    switch ((ProcessingType?)exam.ProcessingType_ProcessingTypeId)
                    {
                        case ProcessingType.Sql:
                            executeResult = _repository.ExecuteProcessing(exam.ProcessingId, id, scope);
                            break;
                        case ProcessingType.CSharp:
                            throw new NotImplementedException("ProcessingType.CSharp haven't implemented yet, Sorry dude");
                            break;
                        case ProcessingType.BuiltIn:
                            throw new NotImplementedException("ProcessingType.BuiltIn haven't implemented yet, Sorry dude");
                            break;
                    }

                    if (executeResult != null && executeResult.Success)
                    {
                        var examResult = new ProcessingResultModel
                        {
                            Id = exam.ProcessingId,
                            Affected = executeResult.Data,
                            Name = exam.Name
                        };
                        resultList.Add(examResult);
                    }
                    else
                    {
                        if (executeResult == null)
                        {
                            result.AddError(new ArgumentException("Processing type wrong"));
                        }
                        else
                        {
                            result.AddError(executeResult.LastError);
                        }
                        
                        return result;
                    }
                }

                result.Data = new List<ProcessingResultModel>(resultList.OrderBy(p => p.Id));
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult<IEnumerable<SearchPeopleByPolicyModel>> SearchPeopleByPersonal(string surname, string name, string patronymic, DateTime? birthday, int? sex)
        {
            var result = new OperationResult<IEnumerable<SearchPeopleByPolicyModel>>
            {
                Data = new List<SearchPeopleByPolicyModel>()
            };
            try
            {
                _dockLayoutManager.ShowOverlay(Constants.SearchDataTitleMsg, Constants.PleaseWaitMsg);

                var policyRemoteServiceResult = GetPolicyRemoteService();
                if (policyRemoteServiceResult.HasError)
                {
                    result.AddError(policyRemoteServiceResult.LastError);
                    return result;
                }
                var service = policyRemoteServiceResult.Data;

                var peopleResult = service.GetPeopleListByPersonal(surname, name, patronymic, birthday, sex);
                if (peopleResult != null && peopleResult.Any())
                {
                    result.Data = peopleResult.Select(p => new SearchPeopleByPolicyModel
                    {
                        Id = p.PeopleId,
                        FirstName = p.FirstName,
                        Surname = p.Surname,
                        Patronymic = p.Patronymic,
                        Sex = p.Sex,
                        Birthday = p.Birthday,
                        Inp = p.ENP
                    });
                }
            }
            catch (Exception ex)
            {
                result.AddError(ex.Message);
                return result;
            }
            return result;
        }

        public OperationResult<IEnumerable<PolicyByPeopleModel>> GetPolicyByPeopleId(int? peopleId)
        {
            var result = new OperationResult<IEnumerable<PolicyByPeopleModel>>
            {
                Data = new List<PolicyByPeopleModel>()
            };

            var policyRemoteServiceResult = GetPolicyRemoteService();
            if (policyRemoteServiceResult.HasError)
            {
                result.AddError(policyRemoteServiceResult.LastError);
                return result;
            }
            var service = policyRemoteServiceResult.Data;

            var policyResult = service.GetPolicyByPeopleId(peopleId);
            if (policyResult != null && policyResult.Any())
            {
                result.Data = policyResult.Select(p => new PolicyByPeopleModel
                {
                    Id = peopleId,
                    DateBegin = p.DateBegin,
                    DateStop = p.DateStop,
                    DateEnd = p.DateEnd,
                    PolicyNumber = p.PolicyNumber,
                    PolicySerial = p.PolicySerial,
                    PolicyType = p.PolicyType
                });
            }
            
            return result;
        }

        public OperationResult<IEnumerable<PolicyCheckTortillaResultModel>> CheckPolicyInTortilla(int medicalAccountId, bool isDebug)
        {
            var result = new OperationResult<IEnumerable<PolicyCheckTortillaResultModel>>();
            var errors = new List<PolicyCheckTortillaResultModel>();

            var log = new StringBuilder();
            log.AppendFormat("***Поиск ЕНП в 'Тортилле' id счета МО {0}***\r\n",medicalAccountId);
            
            var f010Result = _repository.GetF010();
            if (f010Result.HasError)
            {
                result.AddError(f010Result.LastError);
                return result;
            }

            var f010 = f010Result.Data.ToList();
            var patientsResult = _repository.GetEventShortViewByMedicalAccountId(medicalAccountId);
            if (patientsResult.HasError)
            {
                result.AddError(patientsResult.LastError);
                return result;
            }

            var patients = patientsResult.Data.ToList();
            var withoutInpPatients = patients.Where(p => p.INP == null || p.INP.Trim() == "").ToList();
            var patientCount = patients.Count();
            log.AppendFormat("Всего пациентов в счете: {0}\r\n", patientCount);
            log.AppendFormat("Найдено пациентов без ЕНП: {0}\r\n",withoutInpPatients.Count);
            var changedPolicy = 0;
            var changedTerritory = 0;
            int count = 0; 
            foreach (var patient in patients)
            {
                count++;
                _dockLayoutManager.SetOverlayProgress(patientCount,count);
                var srzQueriesResult = _tortillaRepository.GetSrzQueries(patient.Surname, patient.Name, patient.Patronymic, patient.Birthday, patient.SexCode, patient.DocType, patient.DocNum, patient.DocSeries);
                if (srzQueriesResult.HasError)
                {
                    result.AddError(srzQueriesResult.LastError);
                    return result;
                }

                var srzQueries = srzQueriesResult.Data.ToList();
                log.AppendLine("**********Пациент id {0}**********".F(patient.PatientId));
                if (isDebug)
                {
                    log.AppendFormat("Пациент id {0} - {1} {2} {3} дата рождения {4} пол {5}. начало случая {6} окончание случая {7}\r\n найдено запросов: {8}\r\n",
                        patient.PatientId,
                        patient.Surname,
                        patient.Name,
                        patient.Patronymic,
                        patient.Birthday.ToFormatString(),
                        patient.SexCode == 1 ? "М" : "Ж",
                        patient.EventBegin,
                        patient.EventEnd,
                        srzQueries.ToList().Count
                    );
                }


                var applyInp = false;
                foreach (var srzQuery in srzQueries)
                {
                    var answersResult = _tortillaRepository.GetSrzAnswer(srzQuery.guid);
                    if (answersResult.HasError)
                    {
                        result.AddError(answersResult.LastError);
                        return result;
                    }
                    if (answersResult.Data == null || !answersResult.Data.Any())
                    {
                        continue;
                    }

                    if (isDebug)
                    {
                        log.AppendFormat("srzQuery id {0} form_date{1}\r\n", srzQuery.id, srzQuery.form_date.ToFormatString());
                    }

                    var answers = answersResult.Data;

                    foreach (var srzAnswer in answers)
                    {
                        if (isDebug)
                        {
                            log.AppendFormat("srzAnswer main_enp {0} ter_code {1} dateBegin {2} dateEnd {3}\r\n", srzAnswer.main_enp, srzAnswer.terr_code, srzAnswer.date_beg, srzAnswer.date_end);
                        }
                        DateTime? dateBegin = default(DateTime?);
                        DateTime dateBeginTmp;
                        if (DateTime.TryParse(srzAnswer.date_beg, out dateBeginTmp))
                        {
                            dateBegin = dateBeginTmp;
                        }
                        DateTime? dateEnd = default(DateTime?);
                        DateTime dateEndTmp;
                        if (DateTime.TryParse(srzAnswer.date_end, out dateEndTmp))
                        {
                            dateEnd = dateBeginTmp;
                        }

                        var territory = f010.FirstOrDefault(p => p.KOD_OKATO == srzAnswer.terr_code);

                        if (srzAnswer.main_enp.IsNotNullOrWhiteSpace() && patient.INP == srzAnswer.main_enp &&
                            territory != null && srzAnswer.terr_code == patient.TerritoryOkato &&
                            patient.EventBegin >= dateBegin &&
                            (srzAnswer.date_end == "none" || patient.EventBegin <= dateEnd))
                        {
                            if (isDebug)
                            {
                                log.Append("Все хорошо! Не ищем дальше!!!\r\n");
                            }

                            applyInp = true;
                            changedPolicy++;
                            break;
                        }
                        
                        if (srzAnswer.main_enp.IsNotNullOrWhiteSpace() && patient.INP != srzAnswer.main_enp && patient.EventBegin >= dateBegin && (srzAnswer.date_end == "none" || patient.EventBegin <= dateEnd))
                        {
                            if (!isDebug)
                            {
                                log.AppendFormat("Пациент id {0} - {1} {2} {3} дата рождения {4} пол {5}.\r\n найдено запросов: {6}\r\n",
                                    patient.PatientId,
                                    patient.Surname,
                                    patient.Name,
                                    patient.Patronymic,
                                    patient.Birthday.ToFormatString(),
                                    patient.SexCode == 1 ? "М" : "Ж",
                                    srzQueries.ToList().Count
                                );
                            }

                            log.AppendFormat("Внимание! обнаружен главный ЕНП {0} отличающийся от ЕНП {1} пациента\r\n", srzAnswer.main_enp, patient.INP); //Ira
                            if (territory == null)
                            {
                                log.AppendFormat("Внимание! не удалось найти территорию {0} в справочнике F010\r\n", srzAnswer.terr_code);
                                break;
                            }

                            int j = 0;

                            if (errors.All(p => p.PatientId != patient.PatientId))
                            {
                                errors.Add(new PolicyCheckTortillaResultModel
                                {
                                    PatientId = patient.PatientId,
                                    INP = srzAnswer.main_enp,
                                    TerritoryOkato = territory.Id,
                                    Data = patient.PatientData,
                                    Comments = "Обнаружен главный ЕНП _{0}_ отличающийся от ЕНП _{1}_ пациента\r\n".F(srzAnswer.main_enp, patient.INP)
                                });

                            }
                            
                            
                            if (territory.KOD_TF == TerritoryService.TfCode.ToString())
                            {
                                log.AppendFormat("Внимание! Обнаружена территория {0}\r\n", territory.SUBNAME);
                            }
                            applyInp = true;
                            changedPolicy++;
                        }
                        else if (srzAnswer.main_enp.IsNotNullOrWhiteSpace() && srzAnswer.terr_code.IsNotNullOrWhiteSpace() && patient.INP == srzAnswer.main_enp && patient.EventBegin >= dateBegin && (srzAnswer.date_end == "none" || patient.EventBegin <= dateEnd))
                        {
                            if (territory == null)
                            {
                                log.AppendFormat("Внимание! Не удалось найти территорию {0} в справочнике F010\r\n", srzAnswer.terr_code);
                                break;
                            }
                            if (srzAnswer.terr_code != patient.TerritoryOkato)
                            {
                                if (!isDebug)
                                {
                                    log.AppendFormat("Пациент id {0} - {1} {2} {3} дата рождения {4} пол {5}.\r\n найдено запросов: {6}\r\n",
                                        patient.PatientId,
                                        patient.Surname,
                                        patient.Name,
                                        patient.Patronymic,
                                        patient.Birthday.ToFormatString(),
                                        patient.SexCode == 1 ? "М" : "Ж",
                                        srzQueries.Count
                                    );
                                }

                                log.AppendFormat("Внимание! ЕНП совпадают, территории отличаются запрос srz {0}  пациент {1}\r\n", srzAnswer.terr_code, patient.TerritoryOkato);
                                if (territory.KOD_TF == TerritoryService.TfCode.ToString())
                                {
                                    log.AppendFormat("Внимание! Обнаружена территория {0}\r\n", territory.SUBNAME);
                                }

                                if (errors.All(p => p.PatientId != patient.PatientId))
                                {
                                    errors.Add(new PolicyCheckTortillaResultModel
                                    {
                                        PatientId = patient.PatientId,
                                        INP = patient.INP,
                                        TerritoryOkato = territory.Id,
                                        Data = patient.PatientData,
                                        Comments = "ЕНП совпадают, территории отличаются, пациент _{1}_ \u21D2 запрос srz _{0}_ \r\n".F(srzAnswer.terr_code, patient.TerritoryOkato)
                                    });
                                }
                                
                                changedTerritory++;
                                applyInp = true;
                            }
                        }
                    }
                }


                if (string.IsNullOrWhiteSpace(patient.INP) && !applyInp)
                {
                    if (!isDebug)
                    {
                        log.AppendFormat("Пациент id {0} - {1} {2} {3} дата рождения {4} пол {5}.\r\n найдено запросов: {6}\r\n",
                            patient.PatientId,
                            patient.Surname,
                            patient.Name,
                            patient.Patronymic,
                            patient.Birthday.ToFormatString(),
                            patient.SexCode == 1 ? "М" : "Ж",
                            srzQueries.Count
                        );
                    }
                    log.Append("Внимание! Отсутствует ЕНП\r\n");
                }
            }

            log.AppendLine("********************");
            log.AppendFormat("Всего заменено полисов {0}\r\n", changedPolicy);
            log.AppendFormat("Всего заменено данных о территории {0}\r\n", changedTerritory);
            log.AppendFormat("Поиск успешно завершен\r\n");

            result.Data = errors;
            result.Log = log.ToString();

            return result;
        }

        public OperationResult<IEnumerable<PolicyCheckTortillaResultModel>> CheckZPolicyInTortilla(int medicalAccountId, bool isDebug)
        {
            var result = new OperationResult<IEnumerable<PolicyCheckTortillaResultModel>>();
            var errors = new List<PolicyCheckTortillaResultModel>();

            var log = new StringBuilder();
            log.AppendFormat("***Поиск ЕНП в 'Тортилле' id счета МО {0}***\r\n", medicalAccountId);

            var f010Result = _repository.GetF010();
            if (f010Result.HasError)
            {
                result.AddError(f010Result.LastError);
                return result;
            }

            var f010 = f010Result.Data.ToList();
            var patientsResult = _repository.GetZslEventViewByMedicalAccountId(medicalAccountId);
            if (patientsResult.HasError)
            {
                result.AddError(patientsResult.LastError);
                return result;
            }

            var patients = patientsResult.Data.ToList();
            var withoutInpPatients = patients.Where(p => p.INP == null || p.INP.Trim() == "").ToList();
            var patientCount = patients.Count();
            log.AppendFormat("Всего пациентов в счете: {0}\r\n", patientCount);
            log.AppendFormat("Найдено пациентов без ЕНП: {0}\r\n", withoutInpPatients.Count);
            var changedPolicy = 0;
            var changedTerritory = 0;
            int count = 0;
            foreach (var patient in patients)
            {
                count++;
                _dockLayoutManager.SetOverlayProgress(patientCount, count);
                var srzQueriesResult = _tortillaRepository.GetSrzQueries(patient.Surname, patient.Name, patient.Patronymic, patient.Birthday, patient.SexCode, patient.DocType, patient.DocNum, patient.DocSeries, patient.DateMedicalAccount);
                if (srzQueriesResult.HasError)
                {
                    result.AddError(srzQueriesResult.LastError);
                    return result;
                }

                var srzQueries = srzQueriesResult.Data.ToList();
                log.AppendLine("**********Пациент id {0}**********".F(patient.PatientId));
                if (isDebug)
                {
                    log.AppendFormat("Пациент id {0} - {1} {2} {3} дата рождения {4} пол {5}. начало случая {6} окончание случая {7}\r\n найдено запросов: {8}\r\n",
                        patient.PatientId,
                        patient.Surname,
                        patient.Name,
                        patient.Patronymic,
                        patient.Birthday.ToFormatString(),
                        patient.SexCode == 1 ? "М" : "Ж",
                        patient.EventBegin,
                        patient.EventEnd,
                        srzQueries.ToList().Count
                    );
                }


                var applyInp = false;
                foreach (var srzQuery in srzQueries)
                {
                    var answersResult = _tortillaRepository.GetSrzAnswer(srzQuery.guid);
                    if (answersResult.HasError)
                    {
                        result.AddError(answersResult.LastError);
                        return result;
                    }
                    if (answersResult.Data == null || !answersResult.Data.Any())
                    {
                        continue;
                    }

                    if (isDebug)
                    {
                        log.AppendFormat("srzQuery id {0} form_date{1}\r\n", srzQuery.id, srzQuery.form_date.ToFormatString());
                    }

                    var answers = answersResult.Data;

                    foreach (var srzAnswer in answers)
                    {
                        if (isDebug)
                        {
                            log.AppendFormat("srzAnswer main_enp {0} ter_code {1} dateBegin {2} dateEnd {3}\r\n", srzAnswer.main_enp, srzAnswer.terr_code, srzAnswer.date_beg, srzAnswer.date_end);
                        }
                        DateTime? dateBegin = default(DateTime?);
                        DateTime dateBeginTmp;
                        if (DateTime.TryParse(srzAnswer.date_beg, out dateBeginTmp))
                        {
                            dateBegin = dateBeginTmp;
                        }
                        DateTime? dateEnd = default(DateTime?);
                        DateTime dateEndTmp;
                        if (DateTime.TryParse(srzAnswer.date_end, out dateEndTmp))
                        {
                            dateEnd = dateBeginTmp;
                        }

                        var territory = f010.FirstOrDefault(p => p.KOD_OKATO == srzAnswer.terr_code);

                        if (srzAnswer.main_enp.IsNotNullOrWhiteSpace() && patient.INP == srzAnswer.main_enp &&
                            territory != null && srzAnswer.terr_code == patient.TerritoryOkato &&
                            patient.EventBegin >= dateBegin &&
                            (srzAnswer.date_end == "none" || patient.EventBegin <= dateEnd))
                        {
                            if (isDebug)
                            {
                                log.Append("Все хорошо! Не ищем дальше!!!\r\n");
                            }

                            applyInp = true;
                            changedPolicy++;
                            break;
                        }

                        if (srzAnswer.main_enp.IsNotNullOrWhiteSpace() && patient.INP != srzAnswer.main_enp && patient.EventBegin >= dateBegin && (srzAnswer.date_end == "none" || patient.EventBegin <= dateEnd))
                        {
                            if (!isDebug)
                            {
                                log.AppendFormat("Пациент id {0} - {1} {2} {3} дата рождения {4} пол {5}.\r\n найдено запросов: {6}\r\n",
                                    patient.PatientId,
                                    patient.Surname,
                                    patient.Name,
                                    patient.Patronymic,
                                    patient.Birthday.ToFormatString(),
                                    patient.SexCode == 1 ? "М" : "Ж",
                                    srzQueries.ToList().Count
                                );
                            }

                            log.AppendFormat("Внимание! обнаружен главный ЕНП {0} отличающийся от ЕНП {1} пациента\r\n", srzAnswer.main_enp, patient.INP); //Ira
                            if (territory == null)
                            {
                                log.AppendFormat("Внимание! не удалось найти территорию {0} в справочнике F010\r\n", srzAnswer.terr_code);
                                break;
                            }

                            int j = 0;

                            if (errors.All(p => p.PatientId != patient.PatientId))
                            {
                                errors.Add(new PolicyCheckTortillaResultModel
                                {
                                    PatientId = patient.PatientId,
                                    INP = srzAnswer.main_enp,
                                    TerritoryOkato = territory.Id,
                                    Data = patient.PatientData,
                                    Comments = "Обнаружен главный ЕНП _{0}_ отличающийся от ЕНП _{1}_ пациента\r\n".F(srzAnswer.main_enp, patient.INP)
                                });

                            }


                            if (territory.KOD_TF == TerritoryService.TfCode.ToString())
                            {
                                log.AppendFormat("Внимание! Обнаружена территория {0}\r\n", territory.SUBNAME);
                            }
                            applyInp = true;
                            changedPolicy++;
                        }
                        else if (srzAnswer.main_enp.IsNotNullOrWhiteSpace() && srzAnswer.terr_code.IsNotNullOrWhiteSpace() && patient.INP == srzAnswer.main_enp && patient.EventBegin >= dateBegin && (srzAnswer.date_end == "none" || patient.EventBegin <= dateEnd))
                        {
                            if (territory == null)
                            {
                                log.AppendFormat("Внимание! Не удалось найти территорию {0} в справочнике F010\r\n", srzAnswer.terr_code);
                                break;
                            }
                            if (srzAnswer.terr_code != patient.TerritoryOkato)
                            {
                                if (!isDebug)
                                {
                                    log.AppendFormat("Пациент id {0} - {1} {2} {3} дата рождения {4} пол {5}.\r\n найдено запросов: {6}\r\n",
                                        patient.PatientId,
                                        patient.Surname,
                                        patient.Name,
                                        patient.Patronymic,
                                        patient.Birthday.ToFormatString(),
                                        patient.SexCode == 1 ? "М" : "Ж",
                                        srzQueries.Count
                                    );
                                }

                                log.AppendFormat("Внимание! ЕНП совпадают, территории отличаются запрос srz {0}  пациент {1}\r\n", srzAnswer.terr_code, patient.TerritoryOkato);
                                if (territory.KOD_TF == TerritoryService.TfCode.ToString())
                                {
                                    log.AppendFormat("Внимание! Обнаружена территория {0}\r\n", territory.SUBNAME);
                                }

                                if (errors.All(p => p.PatientId != patient.PatientId))
                                {
                                    errors.Add(new PolicyCheckTortillaResultModel
                                    {
                                        PatientId = patient.PatientId,
                                        INP = patient.INP,
                                        TerritoryOkato = territory.Id,
                                        Data = patient.PatientData,
                                        Comments = "ЕНП совпадают, территории отличаются, пациент _{1}_ \u21D2 запрос srz _{0}_ \r\n".F(srzAnswer.terr_code, patient.TerritoryOkato)
                                    });
                                }

                                changedTerritory++;
                                applyInp = true;
                            }
                        }
                    }
                }


                if (string.IsNullOrWhiteSpace(patient.INP) && !applyInp)
                {
                    if (!isDebug)
                    {
                        log.AppendFormat("Пациент id {0} - {1} {2} {3} дата рождения {4} пол {5}.\r\n найдено запросов: {6}\r\n",
                            patient.PatientId,
                            patient.Surname,
                            patient.Name,
                            patient.Patronymic,
                            patient.Birthday.ToFormatString(),
                            patient.SexCode == 1 ? "М" : "Ж",
                            srzQueries.Count
                        );
                    }
                    log.Append("Внимание! Отсутствует ЕНП\r\n");
                }
            }

            log.AppendLine("********************");
            log.AppendFormat("Всего заменено полисов {0}\r\n", changedPolicy);
            log.AppendFormat("Всего заменено данных о территории {0}\r\n", changedTerritory);
            log.AppendFormat("Поиск успешно завершен\r\n");

            result.Data = errors;
            result.Log = log.ToString();

            return result;
        }


        public OperationResult ApplyPolicy(int accountId, IEnumerable<PolicyCheckPatientModel> data, int scope)
        {
            var result = new OperationResult();
            switch (scope)
            {
                case Constants.ScopeInterTerritorialAccount:
                    result = ApplyPolicyToInterTerritorialAccount(accountId, data);
                    break;
                case Constants.ScopePatient:
                    result = ApplyPolicyToPatientFromInterTerritorialAccount(accountId, data);
                    break;
                case Constants.ScopeLocalAccount:
                    result.AddError(new NotImplementedException("Область проверки полисов для локальных счетов еще не реализованна."));
                    break;
                default:
                    result.AddError(new ArgumentException("Неверная область применения данных проверки полисов - {0}.".F(scope)));
                    break;
            }
            return result;
        }

        private OperationResult ApplyPolicyToInterTerritorialAccount(int accountId, IEnumerable<PolicyCheckPatientModel> data)
        {
            var source = _dataService.GetRefusalSourceByScope(accountId, Constants.ScopeInterTerritorialAccount);
            var result = new OperationResult();
            try
            {
                var version = _repository.GetTerritoryAccountVersionById(accountId);
                if (!version.Success)
                {
                    result.AddError(new Exception("Отсутствует версия счета по которому идет проверка"));
                    return result;
                }
                if (Constants.ZterritoryVersion.Contains(version.Data))
                {
                    //Удаляем все отказы из счета с ID 57
                    var deleteResult = _repository.DeleteAllTerritorialSank(accountId, new List<int?> { Constants.PolicyRefusalId });
                    if (deleteResult.HasError)
                    {
                        result.AddError(deleteResult.LastError);
                        return result;
                    }

                    foreach (var patient in data)
                    {
                        foreach (var mevent in patient.Mevents)
                        {
                            var mec = new ZFactSank
                            {
                                Amount = mevent.Amount,
                                ZslAmount = mevent.ZslAmount,
                                PatientId = patient.PatientId,
                                ZslMedicalEventId = (int)mevent.ZslMedicalEventId,
                                ReasonId = mevent.Reason,
                                Date = DateTime.Today,
                                EmployeeId = _userId,
                                ZmedicalEventId = mevent.MedicalEventId,
                                Type = (int)RefusalType.MEC,
                                Source = source,
                                ExternalGuid = Guid.NewGuid().ToString(),
                                Comments = mevent.Comments
                            };
                            _repository.InsertOrUpdateTerritorialSank(mec, UpdateFlag.MedicalEvent|UpdateFlag.ZslMedicalEvent, (int)mevent.ZslMedicalEventId);
                        }
                    }

                    var updateResult = _repository.UpdateZTerritoryAccount(accountId);
                    if (updateResult.HasError)
                    {
                        throw new InvalidOperationException(Constants.DbErrorCommonMsg.F("обновлении", "счета"));
                    }
                }
                else
                {
                    //Удаляем все отказы из счета с ID 57
                    var deleteResult = _repository.DeleteAllTerritorialMec(accountId, new List<int?> { Constants.PolicyRefusalId });
                    if (deleteResult.HasError)
                    {
                        result.AddError(deleteResult.LastError);
                        return result;
                    }

                    foreach (var patient in data)
                    {
                        foreach (var mevent in patient.Mevents)
                        {
                            var mec = new FactMEC
                            {
                                Amount = mevent.Amount,
                                PatientId = patient.PatientId,
                                ReasonId = mevent.Reason,
                                Date = DateTime.Today,
                                EmployeeId = _userId,
                                MedicalEventId = mevent.MedicalEventId,
                                Type = (int)RefusalType.MEC,
                                Source = source,
                                ExternalGuid = Guid.NewGuid().ToString(),
                                Comments = mevent.Comments
                            };
                            _repository.InsertOrUpdateTerritorialMec(mec, UpdateFlag.MedicalEvent);
                        }
                    }

                    var updateResult = _repository.UpdateTerritoryAccount(accountId);
                    if (updateResult.HasError)
                    {
                        throw new InvalidOperationException(Constants.DbErrorCommonMsg.F("обновлении", "счета"));
                    }
                }
               
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
                
            return result;
        }

        private OperationResult ApplyPolicyToPatientFromInterTerritorialAccount(int accountId, IEnumerable<PolicyCheckPatientModel> data)
        {
            var source = _dataService.GetRefusalSourceByScope(accountId, Constants.ScopeInterTerritorialAccount);
            var result = new OperationResult();

            try
            {
                foreach (var patient in data)
                {
                    if (!patient.PatientId.HasValue)
                    {
                        result.AddError(new Exception("Отсутствует id пациента"));
                        return result;
                    }

                    var version = _repository.GetTerritoryAccountVersionById(accountId);
                    if (!version.Success)
                    {
                        result.AddError(new Exception("Отсутствует версия счета по которому идет проверка"));
                        return result;
                    }
                    if (version.Data == Constants.Version30)
                    {
                        //Удаляем отказы пациента из счета с ID 57
                        var deleteResult = _repository.DeletePatientTerritorialSank(accountId,
                            new List<int?> {Constants.PolicyRefusalId}, patient.PatientId.Value);
                        if (deleteResult.HasError)
                        {
                            result.AddError(deleteResult.LastError);
                            return result;
                        }

                        if (patient.Mevents.Any())
                        {
                            foreach (var mevent in patient.Mevents)
                            {
                                var mec = new ZFactSank
                                {
                                    Amount = mevent.Amount,
                                    ZslAmount = mevent.ZslAmount,
                                    PatientId = patient.PatientId,
                                    ZslMedicalEventId = (int)mevent.ZslMedicalEventId,
                                    ReasonId = mevent.Reason,
                                    Date = DateTime.Today,
                                    EmployeeId = _userId,
                                    ZmedicalEventId = mevent.MedicalEventId,
                                    Type = (int)RefusalType.MEC,
                                    Source = source,
                                    ExternalGuid = Guid.NewGuid().ToString(),
                                    Comments = mevent.Comments
                                };
                                _repository.InsertOrUpdateTerritorialSank(mec,
                                    UpdateFlag.MedicalEvent | UpdateFlag.ZslMedicalEvent,
                                    (int) mevent.ZslMedicalEventId);
                            }
                            var updateResult = _repository.UpdateZTerritoryAccount(accountId);
                            if (updateResult.HasError)
                            {
                                throw new InvalidOperationException(Constants.DbErrorCommonMsg.F("обновлении", "счета"));
                            }
                        }
                    }
                    else
                    {
                        //Удаляем отказы пациента из счета с ID 57
                        var deleteResult = _repository.DeletePatientTerritorialMec(accountId,
                            new List<int?> {Constants.PolicyRefusalId}, patient.PatientId.Value);
                        if (deleteResult.HasError)
                        {
                            result.AddError(deleteResult.LastError);
                            return result;
                        }

                        if (patient.Mevents.Any())
                        {
                            foreach (var mevent in patient.Mevents)
                            {
                                var mec = new FactMEC
                                {
                                    Amount = mevent.Amount,
                                    PatientId = patient.PatientId,
                                    ReasonId = mevent.Reason,
                                    Date = DateTime.Today,
                                    EmployeeId = _userId,
                                    MedicalEventId = mevent.MedicalEventId,
                                    Type = (int) RefusalType.MEC,
                                    Source = source,
                                    ExternalGuid = Guid.NewGuid().ToString(),
                                    Comments = mevent.Comments
                                };
                                _repository.InsertOrUpdateTerritorialMec(mec, UpdateFlag.MedicalEvent);
                            }
                            var updateResult = _repository.UpdateTerritoryAccount(accountId);
                            if (updateResult.HasError)
                            {
                                throw new InvalidOperationException(Constants.DbErrorCommonMsg.F("обновлении", "счета"));
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

        public OperationResult ApplyPolicyFromTortilla(IEnumerable<PolicyCheckTortillaResultModel> data)
        {
            var result = new OperationResult();
            try
            {
                foreach (var model in data)
                {
                    var updateResult = _repository.UpdatePatientPolicyById(model.PatientId, model.INP, model.TerritoryOkato);
                    if (updateResult.HasError)
                    {
                        result.AddError(updateResult.LastError);
                    }
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }
        
    }
}
