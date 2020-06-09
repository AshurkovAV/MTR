using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class ZMedicalEventOnkValidator : AbstractValidator<ZMedicalEventOnkModel>
    {
        public ZMedicalEventOnkValidator()
        {
            RuleFor(x => x.StageDisease).NotNull().WithMessage("Поле 'Стадия заболевания' обязательное");
            RuleFor(x => x.OnkT).NotNull().WithMessage("Поле 'Значение Tumor' обязательное");
            RuleFor(x => x.OnkN).NotNull().WithMessage("Поле 'Значение Nodus' обязательное");
            RuleFor(x => x.OnkM).NotNull().WithMessage("Поле 'Значение Metastasis' обязательное");
            RuleFor(x => x.Mtstz).NotNull().WithMessage("Поле 'Признак выявления отдалённых метастазов' обязательное");
            RuleFor(x => x.Sod).NotNull().WithMessage("Поле 'Суммарная очаговая доза' обязательное");
        }
    }
}
