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
    public abstract class ZMedicalServiceOnkModel : EditableObject<ZMedicalServiceOnkModel>, IDataErrorInfo 
    {
        private AbstractValidator<ZMedicalServiceOnkModel> _validator;
        public AbstractValidator<ZMedicalServiceOnkModel> Validator
        {
            get { return _validator ?? (_validator = new ZMedicalServiceOnkValidator()); }
        }
        public ZFactMedicalServicesOnk MedicalServiceOnk { get; set; }
        // Any abstract property becomes editable.
        public abstract int ZmedicalServicesOnkId { get; set; } // int(10)
        public abstract int ZmedicalServicesId { get; set; } // int(10)
        public abstract int? PrCons { get; set; } // int(10)
        public abstract int? ServicesOnkTypeId { get; set; } // int(10)
        public abstract int? HirTypeId { get; set; } // int(10)
        public abstract int? LekTypeLId { get; set; } // int(10)
        public abstract int? LekTypeVId { get; set; } // int(10)
        public abstract int? LuchTypeId { get; set; } // int(10)
     

        protected ZMedicalServiceOnkModel(InitContext initContext)
        {
            MedicalServiceOnk = (ZFactMedicalServicesOnk)initContext.Parameters[0];
        }
        public ZFactMedicalServicesOnk Update()
        {
            var tmp = Map.ObjectToObject<ZFactMedicalServicesOnk>(MedicalServiceOnk);
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
