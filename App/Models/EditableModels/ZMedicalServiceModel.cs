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
    public abstract class ZMedicalServiceModel : EditableObject<MedicalServiceModel>, IDataErrorInfo 
    {
        private AbstractValidator<ZMedicalServiceModel> _validator;
        public AbstractValidator<ZMedicalServiceModel> Validator
        {
            get { return _validator ?? (_validator = new ZMedicalServiceValidator()); }
        }
        public ZFactMedicalServices MedicalService { get; set; }
        // Any abstract property becomes editable.
        public abstract int ZmedicalServicesId { get; set; } // int(10)
        public abstract int ZmedicalEventId { get; set; } // int(10)
        public abstract int? MedicalOrganization { get; set; } // int(10)
        public abstract int? Departament { get; set; } // int(10)
        public abstract int? Unit { get; set; } // int(10)
        public abstract int? Profile { get; set; } // int(10)
        public abstract bool? IsChildren { get; set; } // bit
        public abstract DateTime? ServiceBegin { get; set; } // datetime(3)
        public abstract DateTime? ServiceEnd { get; set; } // datetime(3)
        public abstract int? Diagnosis { get; set; } // int(10)
        public abstract string ServiceCode { get; set; } // nvarchar(20)
        public abstract decimal? Quantity { get; set; } // money(19,4)
        public abstract decimal? Price { get; set; } // money(19,4)
        public abstract int? SpecialityCode { get; set; } // int(10)
        public abstract string DoctorId { get; set; } // nvarchar(14)
        public abstract string Comments { get; set; } // nvarchar(250)
        public abstract int? ExternalId { get; set; } // int(10)
        public abstract string ServiceName { get; set; } // nvarchar(255)
        public abstract decimal? Rate { get; set; } // money(19,4)
        public abstract int? MedicalExternalId { get; set; } // int(10)
        public abstract string ExternalGUID { get; set; } // nvarchar(36)
        public abstract string SurgeryType { get; set; } // nvarchar(15)
        public abstract string MedicalOrganizationCode { get; set; } // nvarchar(6)
        public abstract string MedicalDepartmentCode { get; set; } // nvarchar(14)
        public abstract string Subdivision { get; set; } // nvarchar(3)
        public abstract int? SpecialityCodeV021{ get; set; } // int(10)
        //public abstract int? SpecialityCodeV015 { get; set; } // int(10)

        //Medical
        public abstract long? Flag { get; set; }
        public abstract bool? IsServiceBeforeEvent { get; set; } 
        public abstract bool? IsServiceRefuse { get; set; }
       
        // This field is not editable.
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "IsChildren":
                        return IsChildren.IsNull() ? "Поле 'Признак детского профиля' обязательное" : string.Empty;
                    //TODO
                }
                return string.Empty;
            }
        }

        public string Error { get; private set; }

        protected ZMedicalServiceModel(InitContext initContext)
        {
            MedicalService = (ZFactMedicalServices)initContext.Parameters[0];
        }
        public ZFactMedicalServices Update()
        {
            var tmp = Map.ObjectToObject<ZFactMedicalServices>(MedicalService);
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
