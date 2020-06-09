using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using Autofac;
using BLToolkit.EditableObjects;
using BLToolkit.Mapping;
using BLToolkit.Reflection;
using Core;
using Core.Extensions;
using Core.Services;
using DataModel;
using FluentValidation;
using Medical.AppLayer.Models.Validators;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.AppLayer.Models.EditableModels
{
    public abstract class ZContraindicationsModel : EditableObject<ZContraindicationsModel>, IDataErrorInfo 
    {
        private static readonly ICacheRepository Repository;

        private AbstractValidator<ZContraindicationsModel> _validator;
        public AbstractValidator<ZContraindicationsModel> Validator => _validator ?? (_validator = new ZСontraindicationsValidator());

        public ZFactContraindications Contraindications { get; set; }

        static ZContraindicationsModel()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                if (scope.IsRegistered<ICacheRepository>())
                {
                    Repository = scope.Resolve<ICacheRepository>();
                }
            }
        }

        protected ZContraindicationsModel(InitContext initContext)
        {
            Contraindications = (ZFactContraindications)initContext.Parameters[0];
        }

        public ZFactContraindications Update()
        {
            var tmp = Map.ObjectToObject<ZFactContraindications>(Contraindications);
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

        public abstract int ZContraindicationsId { get; set; }
        public abstract int ZMedicalEventOnkId { get; set; }
        public abstract int? Prot { get; set; }
        public abstract DateTime Dprot { get; set; }

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
