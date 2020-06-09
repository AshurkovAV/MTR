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
    public abstract class MedicalEventModel : EditableObject<MedicalEventModel>, IDataErrorInfo 
    {
        private AbstractValidator<MedicalEventModel> _validator;
        public AbstractValidator<MedicalEventModel> Validator
        {
            get { return _validator ?? (_validator = new MedicalEventValidator()); }
        }

        public FactMedicalEvent MedicalEvent { get; set; }

        // Any abstract property becomes editable.
        //
        public abstract int? PatientId { get; set; }
        public abstract int MedicalEventId { get; set; }
        public abstract int? ExternalId { get; set; }
        public abstract DateTime? EventBegin { get; set; }
        public abstract DateTime? EventEnd { get; set; }
        public abstract int? AssistanceConditions { get; set; }
        public abstract int? AssistanceType { get; set; }
        public abstract int? ProfileCodeId { get; set; }
        public abstract bool? IsChildren { get; set; }
        public abstract string DoctorId { get; set; }
        public abstract int? ReferralOrganization { get; set; }
        public abstract decimal? Hospitalization { get; set; }
        public abstract int? Department { get; set; }
        public abstract string History { get; set; }
        public abstract int? DiagnosisGeneral { get; set; }
        public abstract int? DiagnosisPrimary { get; set; }
        public abstract int? DiagnosisSecondary { get; set; }
        public abstract int? Result { get; set; }
        public abstract int? Outcome { get; set; }
        public abstract int? SpecialityCode { get; set; }
        public abstract int? PaymentMethod { get; set; }
        public abstract decimal? Quantity { get; set; }
        public abstract decimal? UetQuantity { get; set; }
        public abstract decimal? Rate { get; set; }
        public abstract decimal? Price { get; set; }
        public abstract decimal? AcceptPrice { get; set; }
        public abstract decimal? MEE { get; set; }
        public abstract decimal? MEC { get; set; }
        public abstract decimal? EQMA { get; set; }
        public abstract int? PaymentStatus { get; set; }
        public abstract int? Subdivision { get; set; }
        public abstract string ParticularCase { get; set; }
        public abstract string MES { get; set; } // nvarchar(20)
        public abstract string SecondaryMES { get; set; }
        public abstract string MedicalOrganizationCode { get; set; }
        public abstract string Comments { get; set; }
        
        public abstract int? SpecialityCodeV015 { get; set; }
        public abstract string NewbornsWeightAggregate { get; set; }
        public abstract string DiagnosisComplicationAggregate { get; set; }
        public abstract string DiagnosisSecondaryAggregate { get; set; }
        public abstract string HighTechAssistanceType { get; set; }
        public abstract int? HighTechAssistanceMethod { get; set; }
        public abstract int? AssistanceForm { get; set; }

        //Поля МО
        public abstract int? MedicalExternalId { get; set; }
        public abstract decimal? MoPrice { get; set; }
        public abstract int? MoPaymentStatus { get; set; }
        public abstract int? EventType { get; set; }
        public abstract bool? IsServiceBeforeEvent { get; set; }
        public abstract bool? IsServiceRefuse { get; set; }
        public abstract int? RegionalAttribute { get; set; }
        public abstract int? HealthGroup { get; set; }

        public abstract int? JobStatus_Id { get; set; }

        
        // This field is not editable.
        //
        
        public string FullName
        {
            get { return string.Format("{0} {1} {2}", 1,3,4); }
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

        protected MedicalEventModel(InitContext initContext)
        {
            MedicalEvent = (FactMedicalEvent)initContext.Parameters[0];
        }

        public FactMedicalEvent Update()
        {
            var tmp = Map.ObjectToObject<FactMedicalEvent>(MedicalEvent);
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
    }
}
