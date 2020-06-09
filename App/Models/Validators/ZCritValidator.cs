using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class ZCritValidator : AbstractValidator<ZSlCritModel>
    {
        public ZCritValidator()
        {
            RuleFor(x => x.IdDkk).NotNull().WithMessage("Поле обязательное для заполнения");
        }
    }
}
