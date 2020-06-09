using Autofac;
using Core.Extensions;
using Core.Services;
using FluentValidation;
using Medical.AppLayer.Models.EditableModels;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.AppLayer.Models.Validators
{
    public class MedicalEventValidator : AbstractValidator<MedicalEventModel>
    {
        private static readonly ICacheRepository Repository;

        static MedicalEventValidator()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                if (scope.IsRegistered<ICacheRepository>())
                {
                    Repository = scope.Resolve<ICacheRepository>();
                }

            }
        }

        public MedicalEventValidator()
        {
            RuleFor(x => x.AssistanceConditions).NotEmpty().WithMessage("Поле 'Условия оказания медицинской помощи' обязательное");
            RuleFor(x => x.AssistanceType).NotEmpty().WithMessage("Поле 'Вид помощи' обязательное");
            RuleFor(x => x.AssistanceForm).NotEmpty().WithMessage("Поле 'Форма оказания МП' обязательное");
            RuleSet("Date",()=>{
                RuleFor(x => x.EventBegin).NotEmpty().WithMessage("Поле 'Дата начала лечения' обязательное");
                RuleFor(x => x.EventEnd).NotEmpty().WithMessage("Поле 'Дата окончания лечения' обязательное");
                RuleFor(x => x.EventBegin).LessThanOrEqualTo(x => x.EventEnd).WithMessage("Поле 'Дата начала лечения' обязательное и должно быть меньше или равно дате окончания лечения");
                RuleFor(x => x.EventEnd).GreaterThanOrEqualTo(x => x.EventBegin).WithMessage("Поле 'Дата окончания лечения' обязательное и должно быть больше или равно дате начала лечения");
            });
            
            RuleFor(x => x.ProfileCodeId).NotEmpty().WithMessage("Поле 'Профиль' обязательное");
            RuleFor(x => x.IsChildren).NotEmpty().WithMessage("Поле 'Признак детского профиля' обязательное");
            RuleFor(x => x.History).NotEmpty().WithMessage("Поле 'Номер истории болезни/талона амбулаторного пациента' обязательное");
            RuleFor(x => x.DiagnosisGeneral).NotEmpty().WithMessage("Поле 'Основной диагноз' обязательное");
            RuleFor(x => x.Result).NotEmpty().WithMessage("Поле 'Результат обращения/госпитализации' обязательное");
            RuleFor(x => x.Outcome).NotEmpty().WithMessage("Поле 'Исход заболевания' обязательное");
            RuleFor(x => x.PaymentMethod).NotEmpty().WithMessage("Поле 'Код способа оплаты медицинской помощи' обязательное");
            RuleFor(x => x.Price).NotEmpty().WithMessage("Поле 'Сумма, выставленная к оплате' обязательное");
            RuleFor(x => x.AcceptPrice).NotEmpty().WithMessage("Поле 'Сумма, принятая к оплате' обязательное");
            RuleFor(x => x.MedicalOrganizationCode).NotEmpty().WithMessage("Поле 'Код МО' обязательное");

            RuleSet("Speciality", () =>
            {
                RuleFor(x => x.SpecialityCode).NotEmpty().When(x => !x.SpecialityCodeV015.HasValue).WithMessage("Должно быть заполнено поле 'Код специальности' или 'Код специальности V015'");
                RuleFor(x => x.SpecialityCodeV015).NotEmpty().When(x => !x.SpecialityCode.HasValue).WithMessage("Должно быть заполнено поле 'Код специальности' или 'Код специальности V015'");
            });

            RuleFor(x => x.DiagnosisGeneral).Must(BeAValuable).WithMessage("Диагноз неоплачиваемый");


            /*else if (propertyName == "SpecialityCode" && (SpecialityCode == null || SpecialityCode == 0) && (!SpecialityCodeV015.HasValue || SpecialityCodeV015 == 0))
            {
                info.ErrorText = String.Format("Поле 'Специальность лечащего врача/врача, закрывшего талон' обязательно для заполнения");
            }
            else if (propertyName == "SpecialityCodeV015" && (SpecialityCodeV015 == null || SpecialityCodeV015 == 0) && (!SpecialityCode.HasValue || SpecialityCode == 0))
            {
                info.ErrorText = String.Format("Поле 'Специальность лечащего врача/врача, закрывшего талон (v 2.1)' обязательно для заполнения");
            }
            
            else if (propertyName == "PaymentMethod" && (PaymentMethod != 0))
            {
                using (var db = new DatabaseIns())
                {
                    var result = db.V010.FirstOrDefault(p => p.Id == PaymentMethod);
                    if (result != null)
                    {
                        if (result.DATEBEG > EventBegin || (result.DATEEND.HasValue && result.DATEEND < EventEnd))
                        {
                            info.ErrorText = String.Format("'Код способа оплаты медицинской помощи' неверный, неактуальное значение справочника");
                            info.ErrorType = ErrorType.Warning;
                        }

                    }
                    else
                    {
                        info.ErrorText = String.Format("Поле 'Код способа оплаты медицинской помощи' отсутствует в справочнике");
                    }
                }
            }
            
            if (propertyName == "Result" && (Result.HasValue && Result != 0))
            {
                using (var db = new DatabaseIns())
                {
                    var result = db.V009.FirstOrDefault(p => p.Id == Result);
                    if (result != null)
                    {
                        if (result.DL_USLOV != AssistanceConditions)
                        {
                            info.ErrorText = String.Format("'Результат обращения/госпитализации' неверный");
                        }
                        if (result.DATEBEG > EventBegin || (result.DATEEND.HasValue && result.DATEEND < EventEnd))
                        {
                            info.ErrorText = String.Format("'Результат обращения/госпитализации' неверный, неактуальное значение справочника");
                            info.ErrorType = ErrorType.Warning;
                        }
                            
                    }
                    else
                    {
                        info.ErrorText = String.Format("Поле 'Результат обращения/госпитализации' отсутствует в справочнике");
                    }
                }
            }

            if (propertyName == "Outcome" && (Outcome.HasValue && Outcome != 0))
            {
                var result = UnityService.Instance.Resolve<ClassifierManager>().IdToObject<V012>(Outcome, "Id", "DL_USLOV");
                if (result != null)
                {
                    if (SafeConvert.ToInt32(result.ToString()) != AssistanceConditions)
                    {
                        info.ErrorText = String.Format("'Исход обращения/госпитализации' неверный");
                    }
                }
            }


            //Change it...very slow
            if (propertyName == "DiagnosisGeneral" && (DiagnosisGeneral != null && DiagnosisGeneral > 0))
            {
                var result = UnityService.Instance.Resolve<ClassifierManager>().IdToObject<M001>(DiagnosisGeneral, "Id", "Payable");
                if (result != null)
                {
                    if (SafeConvert.ToInt32(result.ToString()) == 1)
                    {
                        info.ErrorText = String.Format("Данный диагноз не оплачиваемый");
                        info.ErrorType = ErrorType.Warning;
                    }
                }
            }*/
        }

        private bool BeAValuable(int? code)
        {
            return Repository.Get(CacheRepository.IDC10PayableCache).Get(code).ToInt32() == 0;
        }
    }
}
