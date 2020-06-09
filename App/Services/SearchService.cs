using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BLToolkit.Data.Linq;
using Core;
using Core.Extensions;
using Core.Linq;
using DataModel;
using Medical.DatabaseCore.Services.Database;

namespace Medical.AppLayer.Services
{

   

    public class SearchService : ISearchService
    {
        private readonly IMedicineRepository _repository;

        public SearchService(IMedicineRepository repository)
        {
            _repository = repository;
        }
        public IEnumerable<EventShortView> SearchByParameters(IDictionary<SearchParameters, object> parameters)
        {
            Expression<Func<EventShortView, bool>> predicate = PredicateBuilder.True<EventShortView>();

            if (parameters.ContainsKey(SearchParameters.InsuranceNumber))
            {
                var insuranceNumber = parameters[SearchParameters.InsuranceNumber] as string;
                predicate = predicate.And(p => Sql.Upper(p.InsuranceDocNumber) == Sql.Upper(insuranceNumber) || p.INP == insuranceNumber);
            }

            if (parameters.ContainsKey(SearchParameters.Id))
            {
                var id = parameters[SearchParameters.Id].ToInt32Nullable();
                predicate = predicate.And(p => p.PatientId == id);
            }

            if (parameters.ContainsKey(SearchParameters.Name))
            {
                var name = parameters[SearchParameters.Name] as string;
                predicate = predicate.And(p => Sql.Upper(p.Name) == Sql.Upper(name));
            }

            if (parameters.ContainsKey(SearchParameters.Surname))
            {
                var surname = parameters[SearchParameters.Surname] as string;
                predicate = predicate.And(p => Sql.Upper(p.Surname) == Sql.Upper(surname));
            }

            if (parameters.ContainsKey(SearchParameters.Patronymic))
            {
                var patronymic = parameters[SearchParameters.Patronymic] as string;
                predicate = predicate.And(p => Sql.Upper(p.Patronymic) == Sql.Upper(patronymic));
            }

            if (parameters.ContainsKey(SearchParameters.Sex))
            {
                var sex = parameters[SearchParameters.Sex].ToInt32Nullable();
                predicate = predicate.And(p => p.SexCode == sex);
            }

            if (parameters.ContainsKey(SearchParameters.BirthDate))
            {
                var birthDate = parameters[SearchParameters.BirthDate].ToDateTimeNullable();
                predicate = predicate.And(p => p.Birthday == birthDate);
            }

            if (parameters.ContainsKey(SearchParameters.EventBeginDate))
            {
                var eventBeginDate = parameters[SearchParameters.EventBeginDate].ToDateTimeNullable();
                predicate = predicate.And(p => p.EventBegin >= eventBeginDate);
            }

            if (parameters.ContainsKey(SearchParameters.EndEventBeginDate))
            {
                var endEventBeginDate = parameters[SearchParameters.EndEventBeginDate].ToDateTimeNullable();
                predicate = predicate.And(p => p.EventBegin <= endEventBeginDate);
            }

            if (parameters.ContainsKey(SearchParameters.EventEndDate))
            {
                var eventEndDate = parameters[SearchParameters.EventEndDate].ToDateTimeNullable();
                predicate = predicate.And(p => p.EventEnd >= eventEndDate);
            }

            if (parameters.ContainsKey(SearchParameters.EndEventEndDate))
            {
                var endEventEndDate = parameters[SearchParameters.EndEventEndDate].ToDateTimeNullable();
                predicate = predicate.And(p => p.EventEnd <= endEventEndDate);
            }

            if (parameters.ContainsKey(SearchParameters.IsUnderpayment))
            {
                var isUnderpayment = parameters[SearchParameters.IsUnderpayment].ToBool();
                if (isUnderpayment)
                {
                    predicate = predicate.And(p => p.PaymentStatusCode != 2);
                }
            }

            var result = _repository.GetEventShortView(predicate);
            if (result.Success)
            {
                return result.Data;
            }
            return null;
        }

        public IEnumerable<GeneralEventShortView> SearchGeneralByParameters(IDictionary<SearchParameters, object> parameters)
        {
            Expression<Func<GeneralEventShortView, bool>> predicate = PredicateBuilder.True<GeneralEventShortView>();

            if (parameters.ContainsKey(SearchParameters.InsuranceNumber))
            {
                var insuranceNumber = parameters[SearchParameters.InsuranceNumber] as string;
                predicate = predicate.And(p => Sql.Upper(p.InsuranceDocNumber) == Sql.Upper(insuranceNumber) || p.INP == insuranceNumber);
            }

            if (parameters.ContainsKey(SearchParameters.Id))
            {
                var id = parameters[SearchParameters.Id].ToInt32Nullable();
                predicate = predicate.And(p => p.PatientId == id);
            }

            if (parameters.ContainsKey(SearchParameters.Name))
            {
                var name = parameters[SearchParameters.Name] as string;
                predicate = predicate.And(p => Sql.Upper(p.Name) == Sql.Upper(name));
            }

            if (parameters.ContainsKey(SearchParameters.Surname))
            {
                var surname = parameters[SearchParameters.Surname] as string;
                predicate = predicate.And(p => Sql.Upper(p.Surname) == Sql.Upper(surname));
            }

            if (parameters.ContainsKey(SearchParameters.Patronymic))
            {
                var patronymic = parameters[SearchParameters.Patronymic] as string;
                predicate = predicate.And(p => Sql.Upper(p.Patronymic) == Sql.Upper(patronymic));
            }

            if (parameters.ContainsKey(SearchParameters.Sex))
            {
                var sex = parameters[SearchParameters.Sex].ToInt32Nullable();
                predicate = predicate.And(p => p.SexCode == sex);
            }

            if (parameters.ContainsKey(SearchParameters.BirthDate))
            {
                var birthDate = parameters[SearchParameters.BirthDate].ToDateTimeNullable();
                predicate = predicate.And(p => p.Birthday == birthDate);
            }

            if (parameters.ContainsKey(SearchParameters.EventBeginDate))
            {
                var eventBeginDate = parameters[SearchParameters.EventBeginDate].ToDateTimeNullable();
                predicate = predicate.And(p => p.EventBegin >= eventBeginDate);
            }

            if (parameters.ContainsKey(SearchParameters.EndEventBeginDate))
            {
                var endEventBeginDate = parameters[SearchParameters.EndEventBeginDate].ToDateTimeNullable();
                predicate = predicate.And(p => p.EventBegin <= endEventBeginDate);
            }

            if (parameters.ContainsKey(SearchParameters.EventEndDate))
            {
                var eventEndDate = parameters[SearchParameters.EventEndDate].ToDateTimeNullable();
                predicate = predicate.And(p => p.EventEnd >= eventEndDate);
            }

            if (parameters.ContainsKey(SearchParameters.EndEventEndDate))
            {
                var endEventEndDate = parameters[SearchParameters.EndEventEndDate].ToDateTimeNullable();
                predicate = predicate.And(p => p.EventEnd <= endEventEndDate);
            }

            if (parameters.ContainsKey(SearchParameters.IsUnderpayment))
            {
                var isUnderpayment = parameters[SearchParameters.IsUnderpayment].ToBool();
                if (isUnderpayment)
                {
                    predicate = predicate.And(p => p.PaymentStatusCode != 2);
                }
            }

            var result = _repository.GetGeneralEventShortView(predicate);
            if (result.Success)
            {
                return result.Data;
            }
            return null;
        }

        public IEnumerable<EventShortView> GetEvents(Expression<Func<EventShortView, bool>> predicate)
        {

            return null;
        }
    }
}
