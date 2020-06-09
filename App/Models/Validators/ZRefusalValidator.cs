using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class ZRefusalValidator : AbstractValidator<ZRefusalModel>
    {
        public ZRefusalValidator()
        {
            RuleFor(x => x.Amount).NotNull().WithMessage("Поле 'Финансовая санкция' обязательное");
            RuleFor(x => x.RefusalType).NotNull().WithMessage("Поле 'Тип санкции' обязательное");
        }
    }
}
