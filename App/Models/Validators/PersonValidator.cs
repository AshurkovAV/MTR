using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class PersonValidator : AbstractValidator<PersonModel>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Birthday).NotNull().WithMessage("Поле 'Дата рождения' обязательное");
            RuleFor(x => x.Sex).NotNull().WithMessage("Поле 'Пол пациента' обязательное");
        }
    }
}
