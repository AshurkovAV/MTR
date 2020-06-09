using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class ZSlkoefValidator : AbstractValidator<ZSlkoefModel>
    {
        public ZSlkoefValidator()
        {
            RuleFor(x => x.NumberDifficultyTreatment).NotNull().WithMessage("Поле 'Номер коэффициента сложности лечения пациента' обязательное");
            RuleFor(x => x.ValueDifficultyTreatment).NotNull().WithMessage("Поле 'Значение коэффициента сложности лечения пациента' обязательное");
        }
    }
}
