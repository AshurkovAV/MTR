using Core.Extensions;
using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class ZMedicalServiceOnkValidator : AbstractValidator<ZMedicalServiceOnkModel>
    {
        public ZMedicalServiceOnkValidator()
        {
            RuleFor(x => x.ServicesOnkTypeId).NotEmpty().WithMessage("Поле 'Тип услуги' обязательное");
        }
    }
}
