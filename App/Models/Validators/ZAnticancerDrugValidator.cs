using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class ZAnticancerDrugValidator : AbstractValidator<ZAnticancerDrugModel>
    {
        public ZAnticancerDrugValidator()
        {
            RuleFor(x => x.RegNum).NotNull().WithMessage("Поле 'Идентификатор лекарственного препарата, применяемого при проведении лекарственной противоопухолевой терапии' обязательное");
            RuleFor(x => x.CodeSh).NotNull().WithMessage("Поле 'Код схемы лекарственной терапии' обязательное");

            //RuleFor(x => x.DataInj).NotNull().WithMessage("Поле 'Дата введения лекарственного препарата' обязательное");
        }
    }
}
