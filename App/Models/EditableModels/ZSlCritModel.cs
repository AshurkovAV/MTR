using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public abstract class ZSlCritModel : EditableObject<ZSlCritModel> 
    {
        private static readonly ICacheRepository Repository;
        private AbstractValidator<ZSlCritModel> _validator;
        //private ObservableCollection<ZSlCritModel> _slcrit;
        public ZFactCrit Crit { get; set; }

        //public AbstractValidator<ZSlCritModel> Validator => _validator ?? (_validator = new ZCritValidator());
        //public ObservableCollection<ZSlCritModel> MedicalSlcrit => _slcrit ?? (_slcrit = new ObservableCollection<ZSlCritModel>());
        static ZSlCritModel()
        {
            using (var scope = Di.I.BeginLifetimeScope())
            {
                if (scope.IsRegistered<ICacheRepository>())
                {
                    Repository = scope.Resolve<ICacheRepository>();
                }
            }
        }

        protected ZSlCritModel(InitContext initContext)
        {
            Crit = (ZFactCrit)initContext.Parameters[0];
        }

        public ZFactCrit Update()
        {
            var tmp = Map.ObjectToObject<ZFactCrit>(Crit);
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

        public abstract int ZCritId { get; set; }
        public abstract int ZksgKpgId { get; set; }
        public abstract int? Value { get; set; }
        public abstract string IdDkk { get; set; }

    }
}
