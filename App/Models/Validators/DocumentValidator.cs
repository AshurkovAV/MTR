using FluentValidation;
using Medical.AppLayer.Models.EditableModels;

namespace Medical.AppLayer.Models.Validators
{
    public class DocumentValidator : AbstractValidator<DocumentModel>
    {
        public DocumentValidator()
        {
            //TODO load regex from database and convert
            RuleFor(x => x.DocSeries).NotEmpty().WithMessage("Поле 'Серия документа УДЛ' неверного формата");
           /* RuleSet("Document", () =>
            {
                When(x => x.DocType == 1, () =>
                {
                    RuleFor(x => x.DocSeries).Matches("R-ББ").WithMessage("Поле 'Серия документа УДЛ' неверного формата");
                    RuleFor(x => x.DocNum).Matches("999999").WithMessage("Поле 'Номер документа УДЛ' неверного формата");
                });

                When(x => x.DocType == 2, () =>
                {
                    RuleFor(x => x.DocSeries).Matches("S").WithMessage("Поле 'Серия документа УДЛ' неверного формата");
                    RuleFor(x => x.DocNum).Matches("00000009").WithMessage("Поле 'Номер документа УДЛ' неверного формата");
                });
            });*/
            
        }
    }
}
