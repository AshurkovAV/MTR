using Core.Extensions;
using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class ZMedicalServiceValidator : AbstractValidator<ZMedicalServiceModel>
    {
        public ZMedicalServiceValidator()
        {
            RuleFor(x => x.ExternalGUID).NotEmpty().WithMessage("Поле 'Номер услуги' обязательное");
            RuleFor(x => x.IsChildren).NotEmpty().WithMessage("Поле 'Признак детского профиля' обязательное");
            RuleFor(x => x.Profile).NotEmpty().WithMessage("Поле 'Профиль' обязательное");
            RuleFor(x => x.MedicalOrganizationCode).NotEmpty().WithMessage("Поле 'Код МО' обязательное");
            RuleFor(x => x.ServiceBegin).NotEmpty().LessThanOrEqualTo(x => x.ServiceEnd).WithMessage("Поле 'Дата начала оказания услуги' обязательное");
            RuleFor(x => x.ServiceEnd).NotEmpty().GreaterThanOrEqualTo(x => x.ServiceBegin).WithMessage("Поле 'Дата окончания оказания услуги' обязательное и должно быть больше или равно дате начала лечения");
            RuleFor(x => x.Diagnosis).NotEmpty().WithMessage("Поле 'Диагноз' обязательное");
            RuleFor(x => x.ServiceName).NotEmpty().WithMessage("Поле 'Наименование услуги' обязательное");
            RuleFor(x => x.Quantity).NotEmpty().WithMessage("Поле 'Количество услуг (кратность услуги)' обязательное");
            RuleFor(x => x.Price).NotEmpty().WithMessage("Поле 'Стоимость медицинской услуги, выставленная к оплате' обязательное");
     
            RuleSet("Speciality", () =>
            {
                RuleFor(x => x.SpecialityCode).NotEmpty().When(x => !x.SpecialityCodeV021.HasValue).WithMessage("Должно быть заполнено поле 'Код специальности' или 'Код специальности V015'");
                RuleFor(x => x.SpecialityCodeV021).NotEmpty().When(x => !x.SpecialityCode.HasValue).WithMessage("Должно быть заполнено поле 'Код специальности' или 'Код специальности V015'");
            });
            
            
        }
    }
}
