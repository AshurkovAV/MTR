using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class ZDiagBlokValidator : AbstractValidator<ZDiagBlokModel>
    {
        public ZDiagBlokValidator()
        {
            RuleFor(x => x.DiagDate).NotNull().WithMessage("Поле 'Дата взятия материала' обязательное");
            RuleFor(x => x.DiagTip).NotNull().WithMessage("Поле 'Тип диагностического показателя");
            
        }
    }
}
