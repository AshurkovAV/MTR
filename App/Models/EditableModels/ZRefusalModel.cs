using System;
using System.ComponentModel;
using System.Linq;
using Autofac;
using BLToolkit.EditableObjects;
using Core;
using Core.Services;
using FluentValidation;
using Medical.AppLayer.Models.Validators;
using Medical.DatabaseCore.Services.Cache;

namespace Medical.AppLayer.Models.EditableModels
{
    public abstract class ZRefusalModel : EditableObject<ZRefusalModel>, IDataErrorInfo 
    {
        private static readonly ICacheRepository Repository;

        private AbstractValidator<ZRefusalModel> _validator;
        public AbstractValidator<ZRefusalModel> Validator => _validator ?? (_validator = new ZRefusalValidator());

        static ZRefusalModel()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                if (scope.IsRegistered<ICacheRepository>())
                {
                    Repository = scope.Resolve<ICacheRepository>();
                }
            }
        }

        public abstract int? RefusalType { get; set; }
        public abstract int ZSankId { get; set; }
        public abstract decimal? Amount { get; set; }
        public abstract decimal? ZslAmount { get; set; }
        public abstract RefusalStatusFlag Flag { get; set; }
        public abstract bool? IsAgree { get; set; }
        public abstract RefusalSource? Source { get; set; }
        public abstract string Comments { get; set; }
        public abstract int? ReasonId { get; set; }
        public abstract RefusalDest Dest { get; set; }
        public abstract TypeSank TypeSank { get; set; }

        // This field is not editable.
        //
        public string ReasonCode => Repository.Get(CacheRepository.F014aCache).GetString(ReasonId);

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
