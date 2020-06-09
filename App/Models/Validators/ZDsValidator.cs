using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class ZDsValidator : AbstractValidator<ZDsModel>
    {
        public ZDsValidator()
        {
            //RuleFor(x => x.Ds).NotNull().WithMessage("Поле 'Дата взятия материала' обязательное");
            //RuleFor(x => x.DiagTip).NotNull().WithMessage("Поле 'Тип диагностического показателя");
            
        }
    }
}
