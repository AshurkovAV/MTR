using Core.Extensions;
using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class ZMedicalConsultationsOnkValidator : AbstractValidator<ZMedicalConsultationsOnkModel>
    {
        public ZMedicalConsultationsOnkValidator()
        {
            RuleFor(x => x.PrCons).NotEmpty().WithMessage("Поле 'Цель проведения консилиума' обязательное");
        }
    }
}
