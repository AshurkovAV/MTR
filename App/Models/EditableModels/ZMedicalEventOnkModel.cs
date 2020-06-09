
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
    public abstract class ZMedicalEventOnkModel : EditableObject<ZMedicalEventOnkModel>, IDataErrorInfo 
    {
        private AbstractValidator<ZMedicalEventOnkModel> _validator;
        public AbstractValidator<ZMedicalEventOnkModel> Validator
        {
            get { return _validator ?? (_validator = new ZMedicalEventOnkValidator()); }
        }
        public ZFactMedicalEventOnk MedicalEventOnk { get; set; }

        // Any abstract property becomes editable.
        public abstract int ZMedicalEventOnkId { get; set; } // int(10)
        public abstract int ZmedicalEventId { get; set; } // int(10)
        public abstract int? Ds1t { get; set; } // int(10)
        public abstract int? StageDisease { get; set; } // int(10)
        public abstract int? OnkT { get; set; } // int(10)
        public abstract int? OnkN { get; set; } // int(10)
        public abstract int? OnkM { get; set; } // int(10)
        public abstract int? Mtstz { get; set; } // int(10)
        public abstract decimal? Sod { get; set; } // int(10)
        public abstract int? Kfr { get; set; } // int(10)
        public abstract decimal? Wei { get; set; } // int(10)
        public abstract int? Hei { get; set; } // int(10)
        public abstract decimal? Bsa { get; set; } // int(10)

        public string Error { get; private set; }

        protected ZMedicalEventOnkModel(InitContext initContext)
        {
            MedicalEventOnk = (ZFactMedicalEventOnk)initContext.Parameters[0];
        }
        public ZFactMedicalEventOnk Update()
        {
            var tmp = Map.ObjectToObject<ZFactMedicalEventOnk>(MedicalEventOnk);
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
