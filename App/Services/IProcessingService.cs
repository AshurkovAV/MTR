using System;
using System.Collections.Generic;
using Core.Infrastructure;
using Medical.AppLayer.Models.PolicySearch;
using Medical.AppLayer.Processing.ViewModels;

namespace Medical.AppLayer.Services
{
    public interface IProcessingService
    {
        OperationResult<IEnumerable<PolicyCheckPatientModel>>  CheckAccountPolicy(int accountId);
        OperationResult<IEnumerable<PolicyCheckPatientModel>> CheckZAccountPolicy(int accountId);
        OperationResult<PolicyCheckPatientModel> CheckPatientPolicy(int patientId);
        OperationResult<PolicyCheckPatientModel> CheckZslPatientPolicy(int patientId);
        OperationResult<IEnumerable<PolicyCheckTortillaResultModel>> CheckPolicyInTortilla(int medicalAccountId, bool isDebug);
        OperationResult<IEnumerable<PolicyCheckTortillaResultModel>> CheckZPolicyInTortilla(int medicalAccountId, bool isDebug);
        OperationResult ApplyPolicyFromTortilla(IEnumerable<PolicyCheckTortillaResultModel> data);
        OperationResult ApplyPolicy(int accountId, IEnumerable<PolicyCheckPatientModel> data, int scope);
        OperationResult<IEnumerable<PolicyByPeopleModel>> GetPolicyByPeopleId(int? peopleId);
        OperationResult<IEnumerable<SearchPeopleByPolicyModel>> SearchPeopleByPersonal(string surname, string name, string patronymic, DateTime? birthday, int? sex);
        OperationResult<IEnumerable<SearchPeopleByPolicyModel>> SearchPeopleByPolicy(int? insuranceDocType, string policyNumber, string policySeries);
        OperationResult<IEnumerable<ProcessingResultModel>> RunProcessing(int id, int scope, int version, List<object> selectedProcessingList);
    }
}