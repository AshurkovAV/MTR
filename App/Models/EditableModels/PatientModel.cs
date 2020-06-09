using System;
using System.ComponentModel;
using System.Linq;
using BLToolkit.EditableObjects;
using BLToolkit.Mapping;
using BLToolkit.Reflection;
using Core.Extensions;
using DataModel;
using FluentValidation;
using Medical.AppLayer.Models.Validators;

namespace Medical.AppLayer.Models.EditableModels
{
    /// <summary>
    /// Editable версия FactPatient
    /// </summary>
    public abstract class PatientModel : EditableObject<PatientModel>, IDataErrorInfo
    {
        private AbstractValidator<PatientModel> _validator;
        public AbstractValidator<PatientModel> Validator {
            get { return _validator ?? (_validator = new PatientValidator()); }
        }

        public FactPatient Patient { get; set; }
        // Any abstract property becomes editable.
        //
        public abstract int PatientId { get; set; }
        public abstract int? ExternalId { get; set; }
        public abstract int PersonalId { get; set; }
        public abstract int? InsuranceId { get; set; }
        public abstract int? InsuranceDocType { get; set; }
        public abstract string InsuranceDocSeries { get; set; }
        public abstract string InsuranceDocNumber { get; set; }
        public abstract string INP { get; set; }
        public abstract int? TerritoryOkato { get; set; }
        public abstract string InsuranceOgrn { get; set; }
        public abstract string InsuranceName { get; set; }
        public abstract string InsuranceRegion { get; set; }
        public abstract string Newborn { get; set; }
        public abstract int? NewbornWeight { get; set; }
        public abstract string Comments { get; set; }

        //Medical account specific
        public abstract int? MedicalAccountId { get; set; }
        public abstract int? MedicalExternalId { get; set; }
        
        // This field is not editable.
        //
        public string CommentFull
        {
            get { return string.Format("{0} {1}", Comments, " Full"); }
        }

        protected PatientModel(InitContext initContext)
        {
            Patient = (FactPatient)initContext.Parameters[0];
        }

        public FactPatient Update()
        {
            var tmp = Map.ObjectToObject<FactPatient>(Patient);
            var patientInfo = tmp.GetType().GetProperties();
            var dirtyMembers = GetDirtyMembers();
            foreach (var info in dirtyMembers)
            {
                var value = info.GetValue(this, null);
                var dest = patientInfo.FirstOrDefault(p => p.Name == info.Name);
                if (dest.IsNotNull())
                {
                    dest.SetValue(tmp, value, null);
                }
            }
            return tmp;
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
