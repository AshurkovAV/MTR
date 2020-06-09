using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class ZKsgKpgValidator : AbstractValidator<ZKsgKpgModel>
    {
        public ZKsgKpgValidator()
        {
            RuleFor(x => x.Kksg).NotNull().WithMessage("Поле 'Номер ксг' обязательное");
        }
    }
}
