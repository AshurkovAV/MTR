using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class ZDirectionValidator : AbstractValidator<ZDirectionModel>
    {
        public ZDirectionValidator()
        {
            RuleFor(x => x.DirectionDate).NotNull().WithMessage("Поле 'Дата направления' обязательное");
            RuleFor(x => x.DirectionViewId).NotNull().WithMessage("Поле 'Вид направления' обязательное");
        }
    }
}
