using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class ZСontraindicationsValidator : AbstractValidator<ZContraindicationsModel>
    {
        public ZСontraindicationsValidator()
        {
            RuleFor(x => x.Prot).NotNull().WithMessage("Поле 'Код противопоказания или отказа' обязательное");
            RuleFor(x => x.Dprot).NotNull().WithMessage("Поле 'Дата регистрации противопоказания или отказа' обязательное");
        }
    }
}
