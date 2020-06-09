using System;
using System.ComponentModel;
using System.Linq;
using BLToolkit.EditableObjects;
using BLToolkit.Mapping;
using Core.Extensions;
using DataModel;
using FluentValidation;
using Medical.AppLayer.Models.Validators;

namespace Medical.AppLayer.Models.EditableModels
{
    public abstract class PersonModel : EditableObject<PersonModel>, IDataErrorInfo 
    {
        private AbstractValidator<PersonModel> _validator;
        public AbstractValidator<PersonModel> Validator
        {
            get { return _validator ?? (_validator = new PersonValidator()); }
        }

        // Any abstract property becomes editable.
        //
        public abstract int PersonId { get; set; }
        public abstract string PName { get; set; }
        public abstract string Surname { get; set; }
        public abstract string Patronymic { get; set; }
        public abstract int? Sex { get; set; }
        public abstract string SNILS { get; set; }
        public abstract DateTime? Birthday { get; set; }
        public abstract string BirthPlace { get; set; }
        public abstract string RepresentativeName { get; set; }
        public abstract string RepresentativeSurname { get; set; }
        public abstract string RepresentativePatronymic { get; set; }
        public abstract int? RepresentativeSex { get; set; }
        public abstract DateTime? RepresentativeBirthday { get; set; }
        public abstract int? AddressLive { get; set; }
        public abstract int? AddressReg { get; set; }
        // This field is not editable.
        //
        public string FullName
        {
            get { return string.Format("{0} {1} {2}", Surname, PName, Patronymic); }
        }

        public FactPerson Update(FactPerson person)
        {
            var patientInfo = person.GetType().GetProperties();
            var dirtyMembers = GetDirtyMembers();
            foreach (var info in dirtyMembers)
            {
                var value = info.GetValue(this, null);
                var dest = patientInfo.FirstOrDefault(p => p.Name == info.Name);
                if (dest.IsNotNull())
                {
                    dest.SetValue(person, value, null);
                }
            }
            return person;
        }

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get
            {
                return Validator != null
                    ? string.Join(Environment.NewLine, Validator.Validate(this).Errors.Select(x => x.ErrorMessage).ToArray())
                    : string.Empty;
            }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                if (Validator != null)
                {
                    var results = Validator.Validate(this, propertyName);
                    if (results != null && results.Errors.Any())
                    {
                        var errors = string.Join(Environment.NewLine, results.Errors.Select(x => x.ErrorMessage).ToArray());
                        return errors;
                    }
                }
                return string.Empty;
            }
        }

        #endregion
    }
}
