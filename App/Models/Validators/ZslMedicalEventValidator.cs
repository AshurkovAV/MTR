using Autofac;
using Core.Extensions;
using Core.Services;
using FluentValidation;
using Medical.AppLayer.Models.EditableModels;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.AppLayer.Models.Validators
{
    public class ZslMedicalEventValidator : AbstractValidator<ZslMedicalEventModel>
    {
        private static readonly ICacheRepository Repository;

        static ZslMedicalEventValidator()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                if (scope.IsRegistered<ICacheRepository>())
                {
                    Repository = scope.Resolve<ICacheRepository>();
                }

            }
        }

        public ZslMedicalEventValidator()
        {
            RuleFor(x => x.AssistanceConditions)
                .NotEmpty()
                .WithMessage("Поле 'Условия оказания медицинской помощи' обязательное");
            RuleFor(x => x.AssistanceType).NotEmpty().WithMessage("Поле 'Вид помощи' обязательное");
            RuleFor(x => x.AssistanceForm).NotEmpty().WithMessage("Поле 'Форма оказания МП' обязательное");
            RuleSet("Date", () =>
            {
                RuleFor(x => x.EventBeginZ1).NotEmpty().WithMessage("Поле 'Дата начала лечения' обязательное");
                RuleFor(x => x.EventEndZ2).NotEmpty().WithMessage("Поле 'Дата окончания лечения' обязательное");
                RuleFor(x => x.EventBeginZ1)
                    .LessThanOrEqualTo(x => x.EventEndZ2)
                    .WithMessage(
                        "Поле 'Дата начала лечения' обязательное и должно быть меньше или равно дате окончания лечения");
                RuleFor(x => x.EventEndZ2)
                    .GreaterThanOrEqualTo(x => x.EventBeginZ1)
                    .WithMessage(
                        "Поле 'Дата окончания лечения' обязательное и должно быть больше или равно дате начала лечения");
            });
            RuleFor(x => x.Result).NotEmpty().WithMessage("Поле 'Результат обращения/госпитализации' обязательное");
            RuleFor(x => x.Outcome).NotEmpty().WithMessage("Поле 'Исход заболевания' обязательное");
            RuleFor(x => x.PaymentMethod)
                .NotEmpty()
                .WithMessage("Поле 'Код способа оплаты медицинской помощи' обязательное");
            RuleFor(x => x.Price).NotEmpty().WithMessage("Поле 'Сумма, выставленная к оплате' обязательное");
            RuleFor(x => x.AcceptPrice).NotEmpty().WithMessage("Поле 'Сумма, принятая к оплате' обязательное");
            RuleFor(x => x.MedicalOrganizationCode).NotEmpty().WithMessage("Поле 'Код МО' обязательное");

        }

        private bool BeAValuable(int? code)
        {
            return Repository.Get(CacheRepository.IDC10PayableCache).Get(code).ToInt32() == 0;
        }
    }
}
