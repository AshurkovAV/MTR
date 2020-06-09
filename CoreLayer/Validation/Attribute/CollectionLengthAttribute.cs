using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Medical.CoreLayer.Validation.Attribute
{
    public class LengthValidation
    {
        public static ValidationResult ValidateCollection(ICollection collection)
        {
            if(collection == null)
                return new ValidationResult("Выберете минимум 1 элемент!");

            bool isValid = collection.Count > 0;
            if (isValid)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Выберете минимум 1 элемент!");
        }
    }
}
