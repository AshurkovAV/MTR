using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using BLToolkit.Reflection;
using Core.Attributes;
using Core.Extensions;
using Core.Infrastructure;
using Core.Services;
using DataModel;
using Medical.AppLayer.Managers;
using Medical.DatabaseCore.Services.Database;
using MedicineNext.Managers;

namespace MedicineNext.Internal
{
    /// <summary>
    /// Загрузчик приложения
    /// </summary>
    public class Bootstrapper
    {
        //Имплементация паттерна одиночка (Singlton)
        #region Singlton
        private static volatile Bootstrapper _instance;
        private static readonly object SyncRoot = new Object();

        public static Bootstrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new Bootstrapper();
                    }
                }
                return _instance;
            }
        }
        #endregion

        /// <summary>
        /// Список общих сборок (third party)
        /// </summary>
        private readonly List<String> _commonAssemblies = new List<String>
                                                        {
                                                            "Autofac",
                                                            "BLToolkit.4",
                                                            "GalaSoft.MvvmLight",
                                                            "GalaSoft.MvvmLight.Extras",
                                                            "System.Windows.Interactivity",
                                                            "Microsoft.Expression.Interactions",
                                                            "WPFToolkit.Extended",
                                                            "ICSharpCode.SharpZipLib",
                                                            "Newtonsoft.Json",
                                                            "Microsoft.Practices.ServiceLocation",
                                                            
                                                        };
        /// <summary>
        /// Список сборок приложения
        /// </summary>
        private readonly List<String> _moduleAssemblies = new List<String>
                                                        {
                                                            "Core",
                                                            "CoreLayer",
                                                            "DatabaseCore",
                                                            "DataCore",
                                                            "AppLayer"
                                                        };
        
        public Exception LastError { get; protected set; }

        //Инициализация загрузчика
        public bool Init()
        {
            LoadCommonAssemblies();
            LoadModules();
            InitInjections();
            InitBltoolkit();

            //Предзагрузка сборок Devexpress
            //MedicineSplashScreen.Progress("Инициализация Devexpress");
            //Medical.CoreLayer.Helpers.OptimizerHelpers.PreloadDevexpress();

            MedicineSplashScreen.Progress("Инициализация завершена");

            return LastError == null;
        }

        private void InitBltoolkit()
        {
            TypeAccessor<localUser>.CreateInstance();
            TypeAccessor<localSettings>.CreateInstance();
            TypeAccessor<M001>.CreateInstance();
            TypeAccessor<F002>.CreateInstance();
            TypeAccessor<F003>.CreateInstance();
            TypeAccessor<F010>.CreateInstance();
            TypeAccessor<F005>.CreateInstance();
            TypeAccessor<F014>.CreateInstance();
            TypeAccessor<V015>.CreateInstance();
            TypeAccessor<V004>.CreateInstance();
            TypeAccessor<globalVersion>.CreateInstance();
            TypeAccessor<V009>.CreateInstance();
            TypeAccessor<globalParam>.CreateInstance();
            TypeAccessor<globalScope>.CreateInstance();
            TypeAccessor<V006>.CreateInstance();
            TypeAccessor<globalPaymentStatus>.CreateInstance();
            TypeAccessor<globalAccountStatus>.CreateInstance();
            TypeAccessor<globalAccountType>.CreateInstance();
            TypeAccessor<shareDoctor>.CreateInstance();
            TypeAccessor<localRole>.CreateInstance();
            TypeAccessor<globalRefusalPenalty>.CreateInstance();
            TypeAccessor<globalExaminationType>.CreateInstance();
            TypeAccessor<globalExaminationGroup>.CreateInstance();
            TypeAccessor<V002>.CreateInstance();
            TypeAccessor<V008>.CreateInstance();
            TypeAccessor<V012>.CreateInstance();
            TypeAccessor<FactTerritoryAccount>.CreateInstance();
            TypeAccessor<EventExtendedView>.CreateInstance();
            TypeAccessor<FactPatient>.CreateInstance();
            TypeAccessor<FactMedicalEvent>.CreateInstance();
            TypeAccessor<FactMedicalServices>.CreateInstance();
            TypeAccessor<FactSrzQuery>.CreateInstance();
            TypeAccessor<FactExchange>.CreateInstance();
            TypeAccessor<FactEconomicAccount>.CreateInstance();
            TypeAccessor<FactEconomicRefuse>.CreateInstance();
            TypeAccessor<FactEconomicSurcharge>.CreateInstance();
            TypeAccessor<FactMEE>.CreateInstance();
            TypeAccessor<FactEQMA>.CreateInstance();
            TypeAccessor<TerritoryAccountView>.CreateInstance();
            TypeAccessor<V005>.CreateInstance();
            TypeAccessor<F008>.CreateInstance();
            TypeAccessor<F011>.CreateInstance();
            TypeAccessor<V010>.CreateInstance();
            TypeAccessor<V014>.CreateInstance();
            TypeAccessor<globalRegionalAttribute>.CreateInstance();
            TypeAccessor<V017>.CreateInstance();
            TypeAccessor<F009>.CreateInstance();
            TypeAccessor<FactPerson>.CreateInstance();
            TypeAccessor<FactDocument>.CreateInstance();
            TypeAccessor<FactExternalRefuse>.CreateInstance();
            TypeAccessor<FactMEC>.CreateInstance();
            TypeAccessor<FactDocument>.CreateInstance();
            TypeAccessor<globalRefusalSource>.CreateInstance();

            /*TypeAccessor<PatientModel>.CreateInstance();
            TypeAccessor<DocumentModel>.CreateInstance();
            TypeAccessor<PersonModel>.CreateInstance();
            TypeAccessor<RefusalModel>.CreateInstance();
            TypeAccessor<MedicalEventModel>.CreateInstance();*/
            
        }

        /// <summary>
        /// Инициализация контейнера IoC
        /// </summary>
        private void InitInjections()
        {
            var builder = new ContainerBuilder();

            //регистрация менеджера окон
            builder.RegisterType<DockLayoutManager>().As<IDockLayoutManager>().SingleInstance();
            //регистрация службы нотификации
            builder.RegisterType<NotifyManager>().As<INotifyManager>().SingleInstance();
            builder.RegisterType<OverlayManager>().As<IOverlayManager>().SingleInstance();

            //builder.RegisterType<AppRemoteSettings>().As<IAppRemoteSettings>().SingleInstance();
            

            Di.Update(builder);
        }

        //Загрузка модулей приложения
        private void LoadModules()
        {
            try
            {
                //Загрузка сборок приложения
                _moduleAssemblies.ForEach(p =>
                {
                    MedicineSplashScreen.Progress("Загрузка {0}".F(p));
                    Assembly.Load(p);
                });
            }
            catch (Exception exception)
            {
                LastError = exception;
            }

            try
            {
                //Получение атрибутов загруженных модулей приложения
                var results = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(p => new { p.GetName().Name, Data = p })
                    .Where(p => _moduleAssemblies.Contains(p.Name))
                    .Select(p => p.Data)
                    .Select(p => p.GetTypes())
                    .SelectMany(p => p)
                    .Select(GetModuleAttribute)
                    .SelectMany(r => r)
                    .ToList();

                //Инициализация модулей с помощью метода с атрибутом ModuleAttribute
                foreach (var type in results)
                {
                    MedicineSplashScreen.Progress("Инициализация модуля '{0}'".F(type.Name));

                    var commandClass = (IModuleActivator)Activator.CreateInstance(type.ClassType);
                    if (commandClass != null)
                    {
                        commandClass.Run();
                    }
                }
            }
            catch (Exception exception)
            {
                LastError = exception;
            }         
        }

        internal IEnumerable<ModuleAttribute> GetModuleAttribute(Type type)
        {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            return attrs.OfType<ModuleAttribute>().ToList();
        }

        /// <summary>
        /// Загрузка общих сборок
        /// </summary>
        private void LoadCommonAssemblies()
        {
            try
            {
                _commonAssemblies.ForEach(p =>
                {
                    MedicineSplashScreen.Progress("Загрузка {0}".F(p));
                    Assembly.Load(p);
                });

            }catch (Exception exception)
            {
                LastError = exception;
            }
        }
       
    }
}
