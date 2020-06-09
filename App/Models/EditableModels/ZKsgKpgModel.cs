
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
    public abstract class ZKsgKpgModel : EditableObject<ZKsgKpgModel>, IDataErrorInfo 
    {
        private AbstractValidator<ZKsgKpgModel> _validator;
        public AbstractValidator<ZKsgKpgModel> Validator
        {
            get { return _validator ?? (_validator = new ZKsgKpgValidator()); }
        }
        public ZFactKsgKpg KsgKpg { get; set; }

        // Any abstract property becomes editable.
        public abstract int ZksgKpgId { get; set; } // int(10)
        public abstract int ZmedicalEventId { get; set; } // int(10)
        public abstract string Kksg { get; set; } // int(10)
        public abstract int? VersionKsg { get; set; } // int(10)
        public abstract bool? IndicationPgKsg { get; set; } // int(10)
        public abstract string Kkpg { get; set; } // int(10)
        public abstract decimal? KoefZ { get; set; }
        public abstract decimal? KoefUp { get; set; }
        public abstract decimal? KoefD { get; set; }
        public abstract decimal? KoefU { get; set; }
        public abstract decimal? BaseRate { get; set; }

        public abstract int? AssistanceCondition { get; set; }

        public abstract int? AdditionСlassificatoryСriterion { get; set; } // int(10)
        public abstract int? AdditionСlassificatoryСriterion2 { get; set; } // int(10)
        public abstract bool? IndicationKslp { get; set; } // bit

        public string Error { get; private set; }

        protected ZKsgKpgModel(InitContext initContext)
        {
            KsgKpg = (ZFactKsgKpg)initContext.Parameters[0];
        }
        public ZFactKsgKpg Update()
        {
            var tmp = Map.ObjectToObject<ZFactKsgKpg>(KsgKpg);
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
