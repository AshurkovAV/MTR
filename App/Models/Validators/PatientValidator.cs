using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class PatientValidator : AbstractValidator<PatientModel>
    {
        public PatientValidator()
        {
            RuleFor(x => x.InsuranceDocType).NotNull().WithMessage("Поле 'Тип документа ОМС' обязательное");
            RuleFor(x => x.Newborn).NotEmpty().WithMessage("Поле 'Признак новорождённого' обязательное");
        }
    }
}
