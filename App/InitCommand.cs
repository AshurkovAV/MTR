using Autofac;
using Core.Attributes;
using Core.Infrastructure;
using Core.Services;
using Medical.AppLayer.Admin.ViewModels;
using Medical.AppLayer.Auth.ViewModels;
using Medical.AppLayer.Classifiers.ViewModels;
using Medical.AppLayer.Economic.ViewModels;
using Medical.AppLayer.Editors;
using Medical.AppLayer.Examination.ViewModels;
using Medical.AppLayer.Linq;
using Medical.AppLayer.Linq.Classifiers;
using Medical.AppLayer.Linq.Log;
using Medical.AppLayer.Migration.ViewModels;
using Medical.AppLayer.Models.EditableModels;
using Medical.AppLayer.Models.RegisterModels;
using Medical.AppLayer.Operator.ViewModel;
using Medical.AppLayer.Processing.ViewModels;
using Medical.AppLayer.Register.ViewModels;
using Medical.AppLayer.Report.ViewModels;
using Medical.AppLayer.Search.ViewModels;
using Medical.AppLayer.Services;
using Medical.AppLayer.Settings.ViewModels;
using Medical.AppLayer.StartPage.ViewModels;

namespace Medical.AppLayer
{
    [Module(Name = "Приложение", ClassType = typeof(InitCommand), Priority = ModulePriority.Normal, Order = 4)]
    public class InitCommand : IModuleActivator
    {
        public void Run()
        {
            var builder = new ContainerBuilder();

            #region Common services
            builder.RegisterType<TextService>().As<ITextService>();
            builder.RegisterType<DataService>().As<IDataService>();
            builder.RegisterType<FileService>().As<IFileService>();
            builder.RegisterType<CommonService>().As<ICommonService>();
            builder.RegisterType<ScriptCodeGenerator>().As<IScriptCodeGenerator>().SingleInstance();
            builder.RegisterType<RemoteService>().As<IRemoteService>().SingleInstance();
            #endregion

            #region Register
            builder.RegisterGeneric(typeof(PLinqListBase<>)).AsSelf();

            builder.RegisterType<ExportXmlViewModel>().AsSelf();
            builder.RegisterType<EcExportXmlViewModel>().AsSelf();
            builder.RegisterType<ImportXmlViewModel>().AsSelf();
            builder.RegisterType<ImportLocalXmlViewModel>().AsSelf();

            builder.RegisterType<PLinqExchangeList>().AsSelf();
            builder.RegisterType<ExchangeViewModel>().AsSelf();
            
            builder.RegisterType<PLinqAccountList>().AsSelf();
            builder.RegisterType<PLinqEventList>().AsSelf();
            builder.RegisterType<PLinqZEventList>().AsSelf();
            builder.RegisterType<TerritoryAccountViewModel>().AsSelf();
            builder.RegisterType<TerritoryAccountCollectionViewModel>().AsSelf(); 
            builder.RegisterType<TerritoryAccountSummaryViewModel>().AsSelf();

            builder.RegisterType<PLinqMedicalAccountList>().AsSelf();
            builder.RegisterType<MedicalAccountViewModel>().AsSelf();
            builder.RegisterType<MedicalAccountSummaryViewModel>().AsSelf();
            builder.RegisterType<MakeTerritoryAccountsViewModel>().AsSelf();

            builder.RegisterType<FilesViewModel>().AsSelf();
            builder.RegisterType<FileDataExchange>().AsSelf();

            builder.RegisterType<BreakUpByPeriodViewModel>().AsSelf();

            builder.RegisterType<MeeActViewModel>().AsSelf();
            builder.RegisterType<EqmaActViewModel>().AsSelf();

            builder.RegisterType<PLinqActsList>().AsSelf();
            builder.RegisterType<ActsViewModel>().AsSelf();

            builder.RegisterType<EconomicActExpertiseViewModel>().AsSelf();
            builder.RegisterType<PLinqEconomicActExpertiseList>().AsSelf();
            builder.RegisterType<EconomicSankCollectionViewModel>().AsSelf();
            


            builder.RegisterType<CreateAccountViewModel>().AsSelf();

            builder.RegisterType<PLinqEventExtendedList>().AsSelf();
            builder.RegisterType<PLinqZEventExtendedList>().AsSelf();

            #endregion

            #region Processing
            builder.RegisterType<CheckPolicyViewModel>().AsSelf();
            builder.RegisterType<CheckPolicyInTortillaViewModel>().AsSelf();
            builder.RegisterType<ProcessingViewModel>().AsSelf();
            builder.RegisterType<PLinqProcessingList>().AsSelf();
            builder.RegisterType<RunProcessingViewModel>().AsSelf();
            
            #endregion

            #region Operator
            builder.RegisterType<OperatorViewModel>().AsSelf();
            builder.RegisterType<ZslOperatorViewModel>().AsSelf();
            builder.RegisterType<MedicalEventContainer>().AsSelf();
            builder.RegisterType<ZslMedicalEventContainer>().AsSelf();
            builder.RegisterType<ZMedicalEventContainer>().AsSelf();
            builder.RegisterType<MedicalServiceContainer>().AsSelf();
            builder.RegisterType<ZMedicalServiceContainer>().AsSelf();
            builder.RegisterType<ZMedicalServiceOnkContainer>().AsSelf();
            builder.RegisterType<RefusalContainer>().AsSelf();
            builder.RegisterType<ZRefusalContainer>().AsSelf();
            builder.RegisterType<ZKsgKpgContainer>().AsSelf();
            builder.RegisterType<ZMedicalEventOnkContainer>().AsSelf();
            builder.RegisterType<ZMedicalConsultationsOnkContainer>().AsSelf();
            builder.RegisterType<ZDsContainer>().AsSelf();
            builder.RegisterType<ZDiagBlokContainer>().AsSelf();
            builder.RegisterType<ZDirectionContainer>().AsSelf(); 
            builder.RegisterType<ZContraindicationsContainer>().AsSelf();
            builder.RegisterType<ZAnticancerDrugContainer>().AsSelf();
            builder.RegisterType<ZSlKoefContainer>().AsSelf();
            builder.RegisterType<ZSlCritContainer>().AsSelf();
            builder.RegisterType<CopyPatientViewModel>().AsSelf();
            builder.RegisterType<CopyZPatientViewModel>().AsSelf();
            builder.RegisterType<MovePatientViewModel>().AsSelf();
            builder.RegisterType<CreateSrzQueryViewModel>().AsSelf();
            builder.RegisterType<SrzResultViewModel>().AsSelf();
            builder.RegisterType<SearchPolicyViewModel>().AsSelf();
            builder.RegisterType<OperatorService>().As<IOperatorService>().SingleInstance();
            builder.RegisterType<ZOperatorService>().As<IZOperatorService>().SingleInstance();
            builder.RegisterType<SearchDiagnosisViewModel>().AsSelf();
            builder.RegisterType<SearchKsgKpgViewModel>().AsSelf(); 
            builder.RegisterType<SearchInsuranceViewModel>().AsSelf();
            builder.RegisterType<SearchDoctorViewModel>().AsSelf();
            builder.RegisterType<SearchMedicalOrganizationViewModel>().AsSelf();
            builder.RegisterType<SearchReferralOrganizationViewModel>().AsSelf(); 

            #endregion

            #region Report service
            builder.RegisterType<PLinqReportList>().AsSelf();
            builder.RegisterType<ReportViewModel>().AsSelf();
            builder.RegisterType<PLinqPreparedReportList>().AsSelf();
            builder.RegisterType<PreparedReportViewModel>().AsSelf();

            builder.RegisterType<RunReportViewModel>().AsSelf();
            builder.RegisterType<PrintReportViewModel>().AsSelf();

            builder.RegisterType<ReportService>().As<IReportService>().SingleInstance();
            builder.RegisterType<ComplexReportRepository>().As<IComplexReportRepository>().SingleInstance();
            #endregion

            #region Examination
            builder.RegisterType<PLinqExaminationList>().AsSelf();
            builder.RegisterType<CriterionViewModel>().AsSelf();

            builder.RegisterType<RunExaminationViewModel>().AsSelf();
            builder.RegisterType<DeleteByPeriodViewModel>().AsSelf();
            
            builder.RegisterType<ExaminationService>().As<IExaminationService>().SingleInstance();

            builder.RegisterType<ProcessingService>().As<IProcessingService>().SingleInstance();
            #endregion

            #region User
            builder.RegisterType<UserService>().As<IUserService>().SingleInstance();
            builder.RegisterType<LoginViewModel>().AsSelf();
           
            #endregion

            #region Administration
            builder.RegisterType<AdminService>().As<IAdminService>().SingleInstance();
            builder.RegisterType<UsersManagementViewModel>().AsSelf();
            builder.RegisterType<EditUserViewModel>().AsSelf();
            builder.RegisterType<PLinqUserList>().AsSelf();
            builder.RegisterType<PLinqLogList>().AsSelf();
            
            
            #endregion

            #region Statistics
            builder.RegisterType<ExportMedicalAssistanceViewModel>().AsSelf();
            #endregion
            
            #region Economic
            builder.RegisterType<AddPaymentViewModel>().AsSelf();
            builder.RegisterType<AddRefuseViewModel>().AsSelf();
            builder.RegisterType<AddSurchargeViewModel>().AsSelf();

            builder.RegisterType<EcoReportForm2PrefilterViewModel>().AsSelf();
            builder.RegisterType<EcoReportRevisePrefilterViewModel>().AsSelf();

            builder.RegisterType<ReportNavigationViewModel>().AsSelf();

            builder.RegisterType<EconomicAccountViewModel>().AsSelf();
            builder.RegisterType<PLinqEconomicAccountList>().AsSelf();

            builder.RegisterType<EconomicRefuseViewModel>().AsSelf();
            builder.RegisterType<PLinqEconomicRefuseList>().AsSelf();

            builder.RegisterType<EconomicSurchargeViewModel>().AsSelf();
            builder.RegisterType<PLinqEconomicSurchargeList>().AsSelf();

            builder.RegisterType<EconomicJournalViewModel>().AsSelf();
            builder.RegisterType<PLinqEconomicJournalList>().AsSelf();
            #endregion

            #region Search
            builder.RegisterType<SearchViewModel>().AsSelf();
            builder.RegisterType<SearchService>().As<ISearchService>().SingleInstance();
            #endregion

            #region Classifiers
            builder.RegisterType<ClassifierViewModel>().AsSelf();
            builder.RegisterType<PLinqDoctorList>().AsSelf();
            builder.RegisterType<PLinqRefusalPenaltyList>().AsSelf();
            builder.RegisterType<PLinqF014List>().AsSelf();
            builder.RegisterType<PLinqV017List>().AsSelf();
            
            builder.RegisterType<PLinqLocalF001List>().AsSelf();
            builder.RegisterType<PLinqEconomicPartnerList>().AsSelf(); 
            builder.RegisterType<PLinqF015List>().AsSelf();
            builder.RegisterType<PLinqGlobalMedicalEventTypeList>().AsSelf();
            builder.RegisterType<PLinqGlobalRegionalAttributeList>().AsSelf();
            builder.RegisterType<PLinqObsoleteDataList>().AsSelf();
            builder.RegisterType<PLinqV015EqV002List>().AsSelf();
            builder.RegisterType<PLinqClinicalExaminationList>().AsSelf();
            builder.RegisterType<PLinqLicenseList>().AsSelf();
            builder.RegisterType<PLinqIDCToEventTypeList>().AsSelf();
            builder.RegisterType<PLinqLocalF003List>().AsSelf();
            builder.RegisterType<PLinqF004List>().AsSelf();
            #endregion

            #region Сommon
            builder.RegisterType<StartPageViewModel>().AsSelf();
            builder.RegisterType<XmlEditViewModel>().AsSelf();
            builder.RegisterType<SettingsViewModel>().AsSelf();
            builder.RegisterType<MigrationsViewModel>().AsSelf();
            #endregion



            Di.Update(builder);
        }
    }
}
