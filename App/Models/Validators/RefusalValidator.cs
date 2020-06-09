using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class RefusalValidator : AbstractValidator<RefusalModel>
    {
        public RefusalValidator()
        {
            RuleFor(x => x.Amount).NotNull().WithMessage("Поле 'Финансовая санкция' обязательное");
            RuleFor(x => x.RefusalType).NotNull().WithMessage("Поле 'Тип санкции' обязательное");
        }
    }
}
