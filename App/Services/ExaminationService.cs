using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Extensions;
using Core.Infrastructure;
using DataModel;
using Medical.AppLayer.Examination.ViewModels;
using Medical.AppLayer.Managers;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Services
{
    public class ExaminationService : IExaminationService
    {
        private readonly IMedicineRepository _repository;
        private readonly IDockLayoutManager _dockLayoutManager;
        private readonly IDataService _dataService;

        private readonly int _userId;

        private IEnumerable<int?> selectedExamsList;

        public ExaminationService(IMedicineRepository repository, 
            IUserService userService,
            IDataService dataService,
            IDockLayoutManager dockLayoutManager)
        {
            _userId = userService.UserId;
            _repository = repository;
            _dataService = dataService;
            _dockLayoutManager = dockLayoutManager;
        }

        public OperationResult<IEnumerable<int>> GetExamsIds(int scope, int version, IEnumerable<int?> groups = null)
        {
            var result = new OperationResult<IEnumerable<int>>();

            try
            {
                var examsResult = _repository.GetEnabledExpertCriterionByScopeAndVersion(scope, version);
                if (examsResult.Success)
                {
                    if (groups.IsNotNull())
                    {
                        result.Data = examsResult.Data
                            .Where(p=>groups.Contains(p.Group))
                            .Select(p=>p.FactExpertCriterionID);
                    }
                    else
                    {
                        result.Data = examsResult.Data
                            .Select(p=>p.FactExpertCriterionID);
                    }
                }
                else
                {
                    result.AddError("Доступные критерии для области {0}, версии {1} и групп {2} отсутствуют".F(scope, version, groups.AggregateToString()));
                    return result;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            return result;
        }

        public OperationResult<IEnumerable<ExamResultModel>> RunExams(int id, int scope, int version, IEnumerable<object> examsList = null)
        {
            var result = new OperationResult<IEnumerable<ExamResultModel>>();
            try
            {
                IEnumerable<FactExpertCriterion> examsTotalList;
                if (examsList == null)
                {
                    var examsResult = _repository.GetEnabledExpertCriterionByScopeAndVersion(scope, version);
                    if (examsResult.Success)
                    {
                        examsTotalList = examsResult.Data;
                    }
                    else
                    {
                        result.AddError("Доступные критерии для области {0} и версии {1} отсутствуют".F(scope, version));
                        return result;
                    }
                }
                else
                {
                    var examsResult = _repository.GetEnabledExpertCriterionByIds(examsList);
                    if (examsResult.Success)
                    {
                        examsTotalList = examsResult.Data;
                    }
                    else
                    {
                        result.AddError("Доступные критерии для области {0} и версии {1} отсутствуют".F(scope, version));
                        return result;
                    }
                }

                selectedExamsList = examsTotalList.Select(p=>p.Reason);
                var resultList = new List<ExamResultModel>();
                
                foreach (var exam in examsTotalList)
                {
                    _dockLayoutManager.SetOverlayMessage("Выполнение проверки:\n'{0}'".F(exam.Name));
                    var examExecuteResult = _repository.ExecuteExam(exam.FactExpertCriterionID, id, scope);
                    if (examExecuteResult.Success)
                    {
                        switch (scope)
                        {
                            case Constants.ScopeLocalAccount:
                                TransactionResult deleteResult = _repository.DeleteAllLocalAutoMec(id, selectedExamsList);
                                if (deleteResult.HasError)
                                {
                                    throw new InvalidOperationException("Произошла ошибка при удалении отказов счета. {0}".F(deleteResult.LastError));
                                }

                                TransactionResult updateResult = _repository.UpdateMedicalAccount(id);
                                if (updateResult.HasError)
                                {
                                    throw new InvalidOperationException("Произошла ошибка при обновлении счета. {0}".F(updateResult.LastError));
                                }
                                break;
                            case Constants.ScopeInterTerritorialAccount:
                                TransactionResult deleteTerritoryResult = Constants.ZterritoryVersion.Contains(version) ? _repository.DeleteAllTerritoryAutoSank(id,selectedExamsList) : _repository.DeleteAllTerritoryAutoMec(id, selectedExamsList);
                                if (deleteTerritoryResult.HasError)
                                {
                                    throw new InvalidOperationException("Произошла ошибка при удалении отказов счета. {0}".F(deleteTerritoryResult.LastError));
                                }

                                TransactionResult updateInterterritorialResult = Constants.ZterritoryVersion.Contains(version) ? _repository.UpdateZTerritoryAccount(id) : _repository.UpdateTerritoryAccount(id);
                                if (updateInterterritorialResult.HasError)
                                {
                                    throw new InvalidOperationException("Произошла ошибка при обновлении счета. {0}".F(updateInterterritorialResult.LastError));
                                }
                                break;
                        }

                        if (examExecuteResult.Data.Any())
                        {
                            FactExpertCriterion examLocalCopy = exam;
                            examExecuteResult.Data.ForEachAction(p =>
                            {
                                if (p.HasValue && examLocalCopy.ErrorScope.HasValue)
                                {
                                    var refusalAmount = _repository.GetAmountByScopeAndId(examLocalCopy.ErrorScope.Value, p.Value, version);
                                    if (refusalAmount.Success)
                                    {
                                        var examResult = new ExamResultModel
                                        {
                                            IsApply = true,
                                            Id = p,
                                            Reason = examLocalCopy.Reason,
                                            ErrorScope = examLocalCopy.ErrorScope,
                                            RefusalPercent = examLocalCopy.RefusalPercent * 100.0m,
                                            RefusalAmount = refusalAmount.Data,
                                            Comments = examLocalCopy.Comments,
                                            Scope = scope
                                        };
                                        resultList.Add(examResult);
                                    }
                                }
                            });
                        }
                    }
                    else
                    {
                        
                        result.AddError(examExecuteResult.LastError);
                        return result;
                    }
                }

                result.Data = new List<ExamResultModel>(resultList.OrderBy(p=>p.ErrorScope).ThenBy(p=>p.Id));
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }
            
            return result;
        }

        public OperationResult ApplyExams(int id, int scope, IEnumerable<ExamResultModel> examResultList)
        {
            var result = new OperationResult();
            var source = _dataService.GetRefusalSourceByScope(id, scope);
            if (source == 0)
            {
                //TODO log error
                throw new ArgumentException("Источник отказа неопределен ID счета {0}, область {1}".F(id,scope));
            }
            try
            {
                foreach (var model in examResultList)
                {
                    switch (model.Scope)
                    {
                        case Constants.ScopeLocalAccount:
                            var localMec = new FactMEC
                            {
                                ExternalGuid = Guid.NewGuid().ToString(),
                                ReasonId = model.Reason,
                                Date = DateTime.Today,
                                EmployeeId = Constants.SystemAccountId,//System
                                Source = source,
                                Type = (int)RefusalType.MEC,
                                Comments = model.Comments
                            };
                            TransactionResult addedResult = _repository.AddLocalMecToScope(localMec, model.Id, model.ErrorScope, model.RefusalPercent/100m);
                            if (addedResult.HasError)
                            {
                                throw new InvalidOperationException("Произошла ошибка при создании отказа. {0}".F(addedResult.LastError));
                            }
                            break;
                        case Constants.ScopeInterTerritorialAccount:
                            var interterritorialMec = new FactMEC
                            {
                                ExternalGuid = Guid.NewGuid().ToString(),
                                ReasonId = model.Reason,
                                Date = DateTime.Today,
                                EmployeeId = Constants.SystemAccountId,//System
                                Source = source,
                                Type = (int)RefusalType.MEC,
                                Comments = model.Comments
                            };
                            TransactionResult addedInterResult = _repository.AddTerritoryMecToScope(interterritorialMec, model.Id, model.ErrorScope, model.RefusalPercent/100m);
                            if (addedInterResult.HasError)
                            {
                                throw new InvalidOperationException("Произошла ошибка при создании отказа. {0}".F(addedInterResult.LastError));
                            }
                            break;
                        default:
                            throw new ArgumentException("Неверная область применения ошибок. {0}".F(scope));
                    }
                    
                }

                switch (scope)
                {
                    case Constants.ScopeLocalAccount:
                        TransactionResult updateResult = _repository.UpdateMedicalAccount(id);
                        if (updateResult.HasError)
                        {
                            throw new InvalidOperationException("Произошла ошибка при обновлении счета. {0}".F(updateResult.LastError));
                        }
                        break;
                    case Constants.ScopeInterTerritorialAccount:
                        TransactionResult updateInterterritorialResult = _repository.UpdateTerritoryAccount(id);
                        if (updateInterterritorialResult.HasError)
                        {
                            throw new InvalidOperationException("Произошла ошибка при обновлении счета. {0}".F(updateInterterritorialResult.LastError));
                        }
                        break;
                }
            }
            catch (Exception exception)
            {
                result.AddError(exception);
            }

            return result;
        }

        public OperationResult ApplyZslExams(int id, int scope, IEnumerable<ExamResultModel> examResultList)
        {
            var result = new OperationResult();
            var source = _dataService.GetRefusalSourceByScope(id, scope);
            if (source == 0)
            {
                //TODO log error
                throw new ArgumentException("Источник отказа неопределен ID счета {0}, область {1}".F(id, scope));
            }
            try
            {
                foreach (var model in examResultList)
                {
                    switch (model.Scope)
                    {
                        case Constants.ScopeLocalAccount:
                            var localMec = new ZFactSank
                            {
                                ExternalGuid = Guid.NewGuid().ToString(),
                                ReasonId = model.Reason,
                                Date = DateTime.Today,
                                EmployeeId = Constants.SystemAccountId,//System
                                Source = source,
                                Type = (int)RefusalType.MEC,
                                Comments = model.Comments
                            };
                            TransactionResult addedResult = _repository.AddLocalMecToScope(localMec, model.Id, model.ErrorScope, model.RefusalPercent / 100m);
                            if (addedResult.HasError)
                            {
                                throw new InvalidOperationException("Произошла ошибка при создании отказа. {0}".F(addedResult.LastError));
                            }
                            break;
                        case Constants.ScopeInterTerritorialAccount:
                            var interterritorialMec = new ZFactSank
                            {
                                ExternalGuid = Guid.NewGuid().ToString(),
                                ReasonId = model.Reason,
                                Date = DateTime.Today,
                                EmployeeId = Constants.SystemAccountId,//System
                                Source = source,
                                Type = (int)RefusalType.MEC,
                                Comments = model.Comments
                            };
                            TransactionResult addedInterResult = _repository.AddTerritoryMecToScope(interterritorialMec, model.Id, model.ErrorScope, model.RefusalPercent / 100m);
                            if (addedInterResult.HasError)
                            {
                                throw new InvalidOperationException("Произошла ошибка при создании отказа. {0}".F(addedInterResult.LastError));
                            }
                            break;
                        default:
                            throw new ArgumentException("Неверная область применения ошибок. {0}".F(scope));
                    }

                }

                switch (scope)
                {
                    case Constants.ScopeLocalAccount:
                        TransactionResult updateResult = _repository.UpdateZMedicalAccount(id);
                        if (updateResult.HasError)
                        {
                            throw new InvalidOperationException("Произошла ошибка при обновлении счета. {0}".F(updateResult.LastError));
                        }
                        break;
                    case Constants.ScopeInterTerritorialAccount:
                        TransactionResult updateInterterritorialResult = _repository.UpdateZTerritoryAccount(id);
                        if (updateInterterritorialResult.HasError)
                        {
                            throw new InvalidOperationException("Произошла ошибка при обновлении счета. {0}".F(updateInterterritorialResult.LastError));
                        }
                        break;
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