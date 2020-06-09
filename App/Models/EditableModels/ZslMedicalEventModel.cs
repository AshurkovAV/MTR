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
    public abstract class ZslMedicalEventModel : EditableObject<ZslMedicalEventModel>, IDataErrorInfo 
    {
        private AbstractValidator<ZslMedicalEventModel> _validator;
        public AbstractValidator<ZslMedicalEventModel> Validator
        {
            get { return _validator ?? (_validator = new ZslMedicalEventValidator()); }
        }

        public ZslFactMedicalEvent MedicalEvent { get; set; }

        // Any abstract property becomes editable.
        //
        public abstract int ZslMedicalEventId { get; set; }
        public abstract int? PatientId { get; set; }        
        public abstract int? ExternalId { get; set; }

        public abstract int? AssistanceConditions { get; set; }
        public abstract int? AssistanceType { get; set; }
        public abstract int? AssistanceForm { get; set; }
        public abstract int? ReferralOrganization { get; set; }
        public abstract DateTime? ReferralDate { get; set; }
        public abstract int? Pdisp2 { get; set; }
        public abstract string MedicalOrganizationCode { get; set; }
        public abstract DateTime? EventBeginZ1 { get; set; }
        public abstract DateTime? EventEndZ2 { get; set; }
        public abstract int? Kdz { get; set; }
        public abstract string NewbornsWeightAggregate { get; set; }
        public abstract int? Result { get; set; }
        public abstract int? Outcome { get; set; }
        public abstract string ParticularCase { get; set; }
        public abstract int? RegionalAttribute { get; set; }
        public abstract int? Vbp { get; set; }
        public abstract int? PaymentMethod { get; set; }
        public abstract decimal? Price { get; set; }
        public abstract int? PaymentStatus { get; set; }
        public abstract decimal? AcceptPrice { get; set; }
        public abstract decimal? RefusalPrice { get; set; }

        //Поля МО
        public abstract int? MedicalExternalId { get; set; }
        public abstract decimal? MoPrice { get; set; }
        public abstract int? MoPaymentStatus { get; set; }
        
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

        protected ZslMedicalEventModel(InitContext initContext)
        {
            MedicalEvent = (ZslFactMedicalEvent)initContext.Parameters[0];
        }

        public ZslFactMedicalEvent Update()
        {
            var tmp = Map.ObjectToObject<ZslFactMedicalEvent>(MedicalEvent);
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
