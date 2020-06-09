using Autofac;
using Core.Extensions;
using Core.Services;
using FluentValidation;
using Medical.AppLayer.Models.EditableModels;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.AppLayer.Models.Validators
{
    public class ZMedicalEventValidator : AbstractValidator<ZMedicalEventModel>
    {
        private static readonly ICacheRepository Repository;

        static ZMedicalEventValidator()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                if (scope.IsRegistered<ICacheRepository>())
                {
                    Repository = scope.Resolve<ICacheRepository>();
                }

            }
        }

        public ZMedicalEventValidator()
        {
            RuleFor(x => x.AssistanceConditions).NotEmpty().WithMessage("Поле 'Условия оказания медицинской помощи' обязательное");
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
            RuleFor(x => x.EventPrice).NotEmpty().WithMessage("Поле 'Сумма, выставленная к оплате' обязательное");
            RuleFor(x => x.AcceptPrice).NotEmpty().WithMessage("Поле 'Сумма, принятая к оплате' обязательное");

            RuleSet("Speciality", () =>
            {
                RuleFor(x => x.SpecialityCode).NotEmpty().When(x => !x.SpecialityCodeV021.HasValue).WithMessage("Должно быть заполнено поле 'Код специальности' или 'Код специальности V015'");
                RuleFor(x => x.SpecialityCodeV021).NotEmpty().When(x => !x.SpecialityCode.HasValue).WithMessage("Должно быть заполнено поле 'Код специальности' или 'Код специальности V015'");
            });

            RuleFor(x => x.DiagnosisGeneral).Must(BeAValuable).WithMessage("Диагноз неоплачиваемый");
        
        }

        private bool BeAValuable(int? code)
        {
            return Repository.Get(CacheRepository.IDC10PayableCache).Get(code).ToInt32() == 0;
        }
    }
}
