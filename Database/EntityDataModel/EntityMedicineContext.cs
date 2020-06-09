namespace Medical.DatabaseCore.EntityDataModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class EntityMedicineContext : DbContext
    {
        public EntityMedicineContext()
            : base()
        {
        }

        public virtual DbSet<F001> F001 { get; set; }
        public virtual DbSet<F002> F002 { get; set; }
        public virtual DbSet<F003> F003 { get; set; }
        public virtual DbSet<F004> F004 { get; set; }
        public virtual DbSet<F005> F005 { get; set; }
        public virtual DbSet<F006> F006 { get; set; }
        public virtual DbSet<F007> F007 { get; set; }
        public virtual DbSet<F008> F008 { get; set; }
        public virtual DbSet<F009> F009 { get; set; }
        public virtual DbSet<F010> F010 { get; set; }
        public virtual DbSet<F011> F011 { get; set; }
        public virtual DbSet<F012> F012 { get; set; }
        public virtual DbSet<F013> F013 { get; set; }
        public virtual DbSet<F014> F014 { get; set; }
        public virtual DbSet<F015> F015 { get; set; }
        public virtual DbSet<FactActEqma> FactActEqmas { get; set; }
        public virtual DbSet<FactActMee> FactActMees { get; set; }
        public virtual DbSet<FactAddress> FactAddresses { get; set; }
        public virtual DbSet<FactClassifierVersion> FactClassifierVersions { get; set; }
        public virtual DbSet<FactDocument> FactDocuments { get; set; }
        public virtual DbSet<FactEconomicAccount> FactEconomicAccounts { get; set; }
        public virtual DbSet<FactEconomicDebt> FactEconomicDebts { get; set; }
        public virtual DbSet<FactEconomicPayment> FactEconomicPayments { get; set; }
        public virtual DbSet<FactEconomicPaymentDetail> FactEconomicPaymentDetails { get; set; }
        public virtual DbSet<FactEconomicRefuse> FactEconomicRefuses { get; set; }
        public virtual DbSet<FactEconomicRefuseDetail> FactEconomicRefuseDetails { get; set; }
        public virtual DbSet<FactEconomicSurcharge> FactEconomicSurcharges { get; set; }
        public virtual DbSet<FactEconomicSurchargeDetail> FactEconomicSurchargeDetails { get; set; }
        public virtual DbSet<FactEQMA> FactEQMAs { get; set; }
        public virtual DbSet<FactExchange> FactExchanges { get; set; }
        public virtual DbSet<FactExpertCriterion> FactExpertCriterions { get; set; }
        public virtual DbSet<FactExpertCriterion_copy> FactExpertCriterion_copy { get; set; }
        public virtual DbSet<FactExternalRefuse> FactExternalRefuses { get; set; }
        public virtual DbSet<FactInsuredRegister> FactInsuredRegisters { get; set; }
        public virtual DbSet<FactMEC> FactMECs { get; set; }
        public virtual DbSet<FactMedicalAccount> FactMedicalAccounts { get; set; }
        public virtual DbSet<FactMedicalEvent> FactMedicalEvents { get; set; }
        public virtual DbSet<FactMedicalService> FactMedicalServices { get; set; }
        public virtual DbSet<FactMEE> FactMEEs { get; set; }
        public virtual DbSet<FactPatient> FactPatients { get; set; }
        public virtual DbSet<FactPerson> FactPersons { get; set; }
        public virtual DbSet<FactPreparedReport> FactPreparedReports { get; set; }
        public virtual DbSet<FactReport> FactReports { get; set; }
        public virtual DbSet<FactSrzQuery> FactSrzQueries { get; set; }
        public virtual DbSet<FactTerritoryAccount> FactTerritoryAccounts { get; set; }
        public virtual DbSet<globalAccountStatu> globalAccountStatus { get; set; }
        public virtual DbSet<globalAccountType> globalAccountTypes { get; set; }
        public virtual DbSet<globalClassifierMap> globalClassifierMaps { get; set; }
        public virtual DbSet<globalClassifierType> globalClassifierTypes { get; set; }
        public virtual DbSet<globalCriterionReason> globalCriterionReasons { get; set; }
        public virtual DbSet<globalCurePeriod> globalCurePeriods { get; set; }
        public virtual DbSet<globalDirection> globalDirections { get; set; }
        public virtual DbSet<globalDoctorParticularSign> globalDoctorParticularSigns { get; set; }
        public virtual DbSet<globalEquivalentData> globalEquivalentDatas { get; set; }
        public virtual DbSet<globalExaminationGroup> globalExaminationGroups { get; set; }
        public virtual DbSet<globalExaminationType> globalExaminationTypes { get; set; }
        public virtual DbSet<globalFlag> globalFlags { get; set; }
        public virtual DbSet<globalIDC10Modernization> globalIDC10Modernization { get; set; }
        public virtual DbSet<globalMecType> globalMecTypes { get; set; }
        public virtual DbSet<globalMedicalAssistanceVolume> globalMedicalAssistanceVolumes { get; set; }
        public virtual DbSet<globalMedicalOrganizationIndicator> globalMedicalOrganizationIndicators { get; set; }
        public virtual DbSet<globalNonworkingDay> globalNonworkingDays { get; set; }
        public virtual DbSet<globalObsoleteData> globalObsoleteDatas { get; set; }
        public virtual DbSet<globalOldProfile> globalOldProfiles { get; set; }
        public virtual DbSet<globalParam> globalParams { get; set; }
        public virtual DbSet<globalParticularCase> globalParticularCases { get; set; }
        public virtual DbSet<globalParticularSign> globalParticularSigns { get; set; }
        public virtual DbSet<globalPaymentStatu> globalPaymentStatus { get; set; }
        public virtual DbSet<globalQuantityParticularSign> globalQuantityParticularSigns { get; set; }
        public virtual DbSet<globalRefusalSource> globalRefusalSources { get; set; }
        public virtual DbSet<globalReliability> globalReliabilities { get; set; }
        public virtual DbSet<globalSchoolStatu> globalSchoolStatus { get; set; }
        public virtual DbSet<globalScope> globalScopes { get; set; }
        public virtual DbSet<globalSrzStatu> globalSrzStatus { get; set; }
        public virtual DbSet<globalVersion> globalVersions { get; set; }
        public virtual DbSet<LocalChief> LocalChiefs { get; set; }
        public virtual DbSet<localDoctorSchedule> localDoctorSchedules { get; set; }
        public virtual DbSet<localEmployee> localEmployees { get; set; }
        public virtual DbSet<localF001> localF001 { get; set; }
        public virtual DbSet<localF003> localF003 { get; set; }
        public virtual DbSet<localIDC10Group> localIDC10Group { get; set; }
        public virtual DbSet<localIDC10GroupComposition> localIDC10GroupComposition { get; set; }
        public virtual DbSet<localLogin> localLogins { get; set; }
        public virtual DbSet<localOptionsDictionary> localOptionsDictionaries { get; set; }
        public virtual DbSet<localParticularSignComposition> localParticularSignCompositions { get; set; }
        public virtual DbSet<localPayment> localPayments { get; set; }
        public virtual DbSet<localPaymentSource> localPaymentSources { get; set; }
        public virtual DbSet<localUser> localUsers { get; set; }
        public virtual DbSet<M001> M001 { get; set; }
        public virtual DbSet<M001Classes> M001Classes { get; set; }
        public virtual DbSet<M001Sections> M001Sections { get; set; }
        public virtual DbSet<ME> MES { get; set; }
        public virtual DbSet<O001> O001 { get; set; }
        public virtual DbSet<O002> O002 { get; set; }
        public virtual DbSet<O003> O003 { get; set; }
        public virtual DbSet<O004> O004 { get; set; }
        public virtual DbSet<O005> O005 { get; set; }
        public virtual DbSet<R001> R001 { get; set; }
        public virtual DbSet<R002> R002 { get; set; }
        public virtual DbSet<R003> R003 { get; set; }
        public virtual DbSet<R004> R004 { get; set; }
        public virtual DbSet<SchemaChanx> SchemaChanges { get; set; }
        public virtual DbSet<shareDoctor> shareDoctors { get; set; }
        public virtual DbSet<shareKsg> shareKsgs { get; set; }
        public virtual DbSet<shareMedicalDepartment> shareMedicalDepartments { get; set; }
        public virtual DbSet<shareMedicalOrganizationLicense> shareMedicalOrganizationLicenses { get; set; }
        public virtual DbSet<shareMedicalOrganizationProfile> shareMedicalOrganizationProfiles { get; set; }
        public virtual DbSet<shareMedicalSubdivision> shareMedicalSubdivisions { get; set; }
        public virtual DbSet<shareOperatingSchedule> shareOperatingSchedules { get; set; }
        public virtual DbSet<shareRate> shareRates { get; set; }
        public virtual DbSet<subAcademicDegree> subAcademicDegrees { get; set; }
        public virtual DbSet<subDayOfWeek> subDayOfWeeks { get; set; }
        public virtual DbSet<subExpertAcademicDegree> subExpertAcademicDegrees { get; set; }
        public virtual DbSet<subExpertCertificate> subExpertCertificates { get; set; }
        public virtual DbSet<subQualificationCategory> subQualificationCategories { get; set; }
        public virtual DbSet<subScheduleDateTime> subScheduleDateTimes { get; set; }
        public virtual DbSet<V001> V001 { get; set; }
        public virtual DbSet<V002> V002 { get; set; }
        public virtual DbSet<V003> V003 { get; set; }
        public virtual DbSet<V004> V004 { get; set; }
        public virtual DbSet<V005> V005 { get; set; }
        public virtual DbSet<V006> V006 { get; set; }
        public virtual DbSet<V007> V007 { get; set; }
        public virtual DbSet<V008> V008 { get; set; }
        public virtual DbSet<V009> V009 { get; set; }
        public virtual DbSet<V010> V010 { get; set; }
        public virtual DbSet<V012> V012 { get; set; }
        public virtual DbSet<V013> V013 { get; set; }
        public virtual DbSet<V014> V014 { get; set; }
        public virtual DbSet<V015> V015 { get; set; }
        public virtual DbSet<V016> V016 { get; set; }
        public virtual DbSet<V017> V017 { get; set; }
        public virtual DbSet<V018> V018 { get; set; }
        public virtual DbSet<V019> V019 { get; set; }
        public virtual DbSet<EventShortView> EventShortViews { get; set; }
        public virtual DbSet<MedicalAccountView> MedicalAccountViews { get; set; }
        public virtual DbSet<PatientShortView> PatientShortViews { get; set; }
        public virtual DbSet<TerritoryAccountView> TerritoryAccountViews { get; set; }
        public virtual DbSet<localUserSettings> localUserSettings { get; set; }
        public virtual DbSet<localSettings> localSettings { get; set; }
        public virtual DbSet<FactProcessing> FactProcessing { get; set; }
        public virtual DbSet<globalProcessingType> globalProcessingType { get; set; }

        public virtual DbSet<localRole> localRole { get; set; }
        public virtual DbSet<globalRefusalPenalty> globalRefusalPenalty { get; set; }
        public virtual DbSet<globalMedicalEventType> globalMedicalEventType { get; set; }
        public virtual DbSet<globalRegionalAttribute> globalRegionalAttribute { get; set; }
        public virtual DbSet<globalClinicalExamination> globalClinicalExamination { get; set; }
        public virtual DbSet<globalV015EqV002> globalV015EqV002 { get; set; }
        public virtual DbSet<globalLicense> globalLicense { get; set; }
        public virtual DbSet<globalLicenseEntry> globalLicenseEntry { get; set; }
        public virtual DbSet<globalIDCToEventType> globalIDCToEventType { get; set; }
        public virtual DbSet<globalEqmaOutcome> globalEqmaOutcome { get; set; }
        
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<F001>()
                .Property(e => e.kf_tf)
                .HasPrecision(4, 0);

            modelBuilder.Entity<F002>()
                .Property(e => e.org)
                .HasPrecision(1, 0);

            modelBuilder.Entity<F002>()
                .Property(e => e.kol_zl)
                .HasPrecision(10, 0);

            modelBuilder.Entity<F002>()
                .HasMany(e => e.FactInsuredRegisters)
                .WithOptional(e => e.F002)
                .HasForeignKey(e => e.InsuranceNumber);

            modelBuilder.Entity<F002>()
                .HasMany(e => e.FactMedicalAccounts)
                .WithOptional(e => e.F002)
                .HasForeignKey(e => e.InsurancePayer);

            modelBuilder.Entity<F002>()
                .HasMany(e => e.FactPatients)
                .WithOptional(e => e.F002)
                .HasForeignKey(e => e.InsuranceId);

            modelBuilder.Entity<F003>()
                .Property(e => e.vedpri)
                .HasPrecision(3, 0);

            modelBuilder.Entity<F003>()
                .Property(e => e.org)
                .HasPrecision(1, 0);

            modelBuilder.Entity<F003>()
                .HasMany(e => e.shareDoctors)
                .WithOptional(e => e.F003)
                .HasForeignKey(e => e.MedicalOrganizationId);

            modelBuilder.Entity<F003>()
                .HasMany(e => e.shareMedicalDepartments)
                .WithOptional(e => e.F003)
                .HasForeignKey(e => e.MedicalOrganizationId);

            modelBuilder.Entity<F003>()
                .HasMany(e => e.FactInsuredRegisters)
                .WithOptional(e => e.F003)
                .HasForeignKey(e => e.Registration);

            modelBuilder.Entity<F003>()
                .HasMany(e => e.FactMedicalAccounts)
                .WithOptional(e => e.F003)
                .HasForeignKey(e => e.MedicalOrganization);

            modelBuilder.Entity<F003>()
                .HasMany(e => e.FactMedicalEvents)
                .WithOptional(e => e.F003)
                .HasForeignKey(e => e.ReferralOrganization);

            modelBuilder.Entity<F003>()
                .HasMany(e => e.globalMedicalAssistanceVolumes)
                .WithRequired(e => e.F003)
                .HasForeignKey(e => e.MedicalOrganization)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<F003>()
                .HasMany(e => e.shareMedicalDepartments1)
                .WithOptional(e => e.F0031)
                .HasForeignKey(e => e.MedicalOrganizationId);

            modelBuilder.Entity<F003>()
                .HasMany(e => e.shareOperatingSchedules)
                .WithRequired(e => e.F003)
                .HasForeignKey(e => e.MedicalOrganizationId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<F004>()
                .HasMany(e => e.subExpertAcademicDegrees)
                .WithRequired(e => e.F004)
                .HasForeignKey(e => e.ExpertId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<F004>()
                .HasMany(e => e.subExpertCertificates)
                .WithRequired(e => e.F004)
                .HasForeignKey(e => e.ExpertId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<F004>()
                .HasMany(e => e.subQualificationCategories)
                .WithRequired(e => e.F004)
                .HasForeignKey(e => e.ExpertId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<F005>()
                .Property(e => e.IDIDST)
                .HasPrecision(8, 0);

            modelBuilder.Entity<F005>()
                .HasMany(e => e.FactMedicalEvents)
                .WithOptional(e => e.F005)
                .HasForeignKey(e => e.PaymentStatus);

            modelBuilder.Entity<F006>()
                .Property(e => e.IDVID)
                .HasPrecision(1, 0);

            modelBuilder.Entity<F007>()
                .Property(e => e.IDVED)
                .HasPrecision(2, 0);

            modelBuilder.Entity<F008>()
                .Property(e => e.IDDOC)
                .HasPrecision(1, 0);

            modelBuilder.Entity<F008>()
                .HasMany(e => e.FactInsuredRegisters)
                .WithOptional(e => e.F008)
                .HasForeignKey(e => e.InsuranceDocType);

            modelBuilder.Entity<F008>()
                .HasMany(e => e.FactPatients)
                .WithOptional(e => e.F008)
                .HasForeignKey(e => e.InsuranceDocType);

            modelBuilder.Entity<F009>()
                .HasMany(e => e.FactInsuredRegisters)
                .WithOptional(e => e.F009)
                .HasForeignKey(e => e.Status);

            modelBuilder.Entity<F010>()
                .Property(e => e.OKRUG)
                .HasPrecision(1, 0);

            modelBuilder.Entity<F010>()
                .HasMany(e => e.FactAddresses)
                .WithOptional(e => e.F010)
                .HasForeignKey(e => e.RegionFederation);

            modelBuilder.Entity<F010>()
                .HasMany(e => e.FactPatients)
                .WithOptional(e => e.F010)
                .HasForeignKey(e => e.TerritoryOkato);

            modelBuilder.Entity<F011>()
                .Property(e => e.IDDoc)
                .HasPrecision(2, 0);

            modelBuilder.Entity<F011>()
                .HasMany(e => e.FactDocuments)
                .WithOptional(e => e.F011)
                .HasForeignKey(e => e.DocType);

            modelBuilder.Entity<F014>()
                .Property(e => e.IDVID)
                .HasPrecision(1, 0);

            modelBuilder.Entity<F014>()
                .HasMany(e => e.FactEQMAs)
                .WithOptional(e => e.F014)
                .HasForeignKey(e => e.ReasonId);

            modelBuilder.Entity<F014>()
                .HasMany(e => e.FactMEEs)
                .WithOptional(e => e.F014)
                .HasForeignKey(e => e.ReasonId);

            modelBuilder.Entity<F014>()
                .HasMany(e => e.FactExternalRefuses)
                .WithRequired(e => e.F014)
                .HasForeignKey(e => e.ReasonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<F014>()
                .HasMany(e => e.FactMECs)
                .WithOptional(e => e.F014)
                .HasForeignKey(e => e.ReasonId);

            modelBuilder.Entity<F015>()
                .Property(e => e.KOD_OK)
                .HasPrecision(1, 0);

            modelBuilder.Entity<FactActEqma>()
                .Property(e => e.Quantity)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactActEqma>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactActEqma>()
                .Property(e => e.AcceptPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactActEqma>()
                .Property(e => e.PenaltyPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactActEqma>()
                .Property(e => e.TotalDuration)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactActMee>()
                .Property(e => e.Quantity)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactActMee>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactActMee>()
                .Property(e => e.AcceptPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactActMee>()
                .Property(e => e.PenaltyPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactEconomicAccount>()
                .Property(e => e.TotalAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactEconomicAccount>()
                .HasMany(e => e.FactEconomicPayments)
                .WithRequired(e => e.FactEconomicAccount)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FactEconomicDebt>()
                .Property(e => e.TerritoryAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactEconomicDebt>()
                .Property(e => e.OwnAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactEconomicDebt>()
                .Property(e => e.TerritoryAmount25)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactEconomicDebt>()
                .Property(e => e.OwnAmount25)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactEconomicPayment>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactEconomicPaymentDetail>()
                .Property(e => e.AmountPayable)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactEconomicPaymentDetail>()
                .Property(e => e.AmountFact)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactEconomicPaymentDetail>()
                .Property(e => e.AmountDebt)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactEconomicRefuse>()
                .Property(e => e.RefuseTotalAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactEconomicRefuse>()
                .HasMany(e => e.FactEconomicRefuseDetails)
                .WithRequired(e => e.FactEconomicRefuse)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FactEconomicRefuseDetail>()
                .Property(e => e.AmountRefuse)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactEconomicSurcharge>()
                .Property(e => e.SurchargeTotalAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactEconomicSurcharge>()
                .HasMany(e => e.FactEconomicSurchargeDetails)
                .WithRequired(e => e.FactEconomicSurcharge)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FactEconomicSurchargeDetail>()
                .Property(e => e.AmountSurcharge)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactEQMA>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactEQMA>()
                .Property(e => e.Penalty)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactExpertCriterion>()
                .Property(e => e.PenaltyPercent)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactExpertCriterion>()
                .Property(e => e.RefusalPercent)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactExpertCriterion_copy>()
                .Property(e => e.PenaltyPercent)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactExpertCriterion_copy>()
                .Property(e => e.RefusalPercent)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactExternalRefuse>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactInsuredRegister>()
                .Property(e => e.INP)
                .HasPrecision(16, 0);

            modelBuilder.Entity<FactMEC>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalAccount>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalAccount>()
                .Property(e => e.AcceptPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalAccount>()
                .Property(e => e.MECPenalties)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalAccount>()
                .Property(e => e.MEEPenalties)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalAccount>()
                .Property(e => e.EQMAPenalties)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalEvent>()
                .Property(e => e.Hospitalization)
                .HasPrecision(2, 0);

            modelBuilder.Entity<FactMedicalEvent>()
                .Property(e => e.Quantity)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalEvent>()
                .Property(e => e.Rate)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalEvent>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalEvent>()
                .Property(e => e.AcceptPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalEvent>()
                .Property(e => e.MEC)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalEvent>()
                .Property(e => e.MEE)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalEvent>()
                .Property(e => e.EQMA)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalEvent>()
                .Property(e => e.UetQuantity)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalEvent>()
                .Property(e => e.RefusalPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalEvent>()
                .HasMany(e => e.FactExternalRefuses)
                .WithRequired(e => e.FactMedicalEvent)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FactMedicalEvent>()
                .HasMany(e => e.FactMECs)
                .WithOptional(e => e.FactMedicalEvent)
                .WillCascadeOnDelete();

            modelBuilder.Entity<FactMedicalEvent>()
                .HasMany(e => e.FactMedicalServices)
                .WithRequired(e => e.FactMedicalEvent)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FactMedicalService>()
                .Property(e => e.Quantity)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalService>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMedicalService>()
                .Property(e => e.Rate)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMEE>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactMEE>()
                .Property(e => e.Penalty)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactPatient>()
                .HasMany(e => e.FactActEqmas)
                .WithRequired(e => e.FactPatient)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FactPatient>()
                .HasMany(e => e.FactActMees)
                .WithRequired(e => e.FactPatient)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FactPatient>()
                .HasMany(e => e.FactExternalRefuses)
                .WithRequired(e => e.FactPatient)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FactPerson>()
                .HasMany(e => e.FactDocuments)
                .WithRequired(e => e.FactPerson)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FactPerson>()
                .HasMany(e => e.FactPatients)
                .WithOptional(e => e.FactPerson)
                .HasForeignKey(e => e.PersonalId);

            modelBuilder.Entity<FactTerritoryAccount>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactTerritoryAccount>()
                .Property(e => e.AcceptPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactTerritoryAccount>()
                .Property(e => e.MECPenalties)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactTerritoryAccount>()
                .Property(e => e.MEEPenalties)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactTerritoryAccount>()
                .Property(e => e.EQMAPenalties)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactTerritoryAccount>()
                .Property(e => e.TotalPaymentAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FactTerritoryAccount>()
                .HasMany(e => e.FactEconomicAccounts)
                .WithRequired(e => e.FactTerritoryAccount)
                .HasForeignKey(e => e.AccountId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FactTerritoryAccount>()
                .HasMany(e => e.FactEconomicPaymentDetails)
                .WithRequired(e => e.FactTerritoryAccount)
                .HasForeignKey(e => e.AccountId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FactTerritoryAccount>()
                .HasMany(e => e.FactEconomicRefuses)
                .WithRequired(e => e.FactTerritoryAccount)
                .HasForeignKey(e => e.AccountId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FactTerritoryAccount>()
                .HasMany(e => e.FactEconomicSurcharges)
                .WithRequired(e => e.FactTerritoryAccount)
                .HasForeignKey(e => e.AccountId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FactTerritoryAccount>()
                .HasMany(e => e.FactPatients)
                .WithOptional(e => e.FactTerritoryAccount)
                .HasForeignKey(e => e.AccountId);

            modelBuilder.Entity<globalCurePeriod>()
                .Property(e => e.Duration)
                .HasPrecision(19, 4);

            modelBuilder.Entity<globalExaminationGroup>()
                .HasMany(e => e.FactExpertCriterions)
                .WithOptional(e => e.globalExaminationGroup)
                .HasForeignKey(e => e.Group);

            modelBuilder.Entity<globalExaminationGroup>()
                .HasMany(e => e.FactExpertCriterion_copy)
                .WithOptional(e => e.globalExaminationGroup)
                .HasForeignKey(e => e.Group);

            modelBuilder.Entity<globalIDC10Modernization>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<globalIDC10Modernization>()
                .Property(e => e.LocalPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<globalIDC10Modernization>()
                .Property(e => e.FederalPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<globalMedicalAssistanceVolume>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<globalMedicalOrganizationIndicator>()
                .HasMany(e => e.globalMedicalAssistanceVolumes)
                .WithRequired(e => e.globalMedicalOrganizationIndicator)
                .HasForeignKey(e => e.Indicator)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<globalOldProfile>()
                .HasMany(e => e.globalEquivalentDatas)
                .WithRequired(e => e.globalOldProfile)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<globalParticularSign>()
                .HasMany(e => e.localParticularSignCompositions)
                .WithRequired(e => e.globalParticularSign)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<globalVersion>()
                .HasMany(e => e.FactExchanges)
                .WithOptional(e => e.globalVersion)
                .HasForeignKey(e => e.Version);

            modelBuilder.Entity<globalVersion>()
                .HasMany(e => e.FactExpertCriterions)
                .WithOptional(e => e.globalVersion)
                .HasForeignKey(e => e.Version);

            modelBuilder.Entity<globalVersion>()
                .HasMany(e => e.FactExpertCriterion_copy)
                .WithOptional(e => e.globalVersion)
                .HasForeignKey(e => e.Version);

            modelBuilder.Entity<globalVersion>()
                .HasMany(e => e.FactTerritoryAccounts)
                .WithOptional(e => e.globalVersion)
                .HasForeignKey(e => e.Version);

            modelBuilder.Entity<M001>()
                .HasMany(e => e.FactMedicalEvents)
                .WithOptional(e => e.M001)
                .HasForeignKey(e => e.DiagnosisGeneral);

            modelBuilder.Entity<M001>()
                .HasMany(e => e.FactMedicalEvents1)
                .WithOptional(e => e.M0011)
                .HasForeignKey(e => e.DiagnosisPrimary);

            modelBuilder.Entity<M001>()
                .HasMany(e => e.FactMedicalEvents2)
                .WithOptional(e => e.M0012)
                .HasForeignKey(e => e.DiagnosisSecondary);

            modelBuilder.Entity<M001>()
                .HasMany(e => e.globalIDC10Modernization)
                .WithOptional(e => e.M001)
                .HasForeignKey(e => e.IDC10Id);

            modelBuilder.Entity<M001>()
                .HasMany(e => e.localIDC10GroupComposition)
                .WithOptional(e => e.M001)
                .HasForeignKey(e => e.IDC10Id);

            modelBuilder.Entity<M001Classes>()
                .HasMany(e => e.M001)
                .WithOptional(e => e.M001Classes)
                .HasForeignKey(e => e.Class);

            modelBuilder.Entity<M001Sections>()
                .HasMany(e => e.M001)
                .WithOptional(e => e.M001Sections)
                .HasForeignKey(e => e.Section);

            modelBuilder.Entity<O001>()
                .Property(e => e.STATUS)
                .HasPrecision(5, 0);

            modelBuilder.Entity<O002>()
                .Property(e => e.STATUS)
                .HasPrecision(5, 0);

            modelBuilder.Entity<O002>()
                .HasMany(e => e.FactAddresses)
                .WithOptional(e => e.O002)
                .HasForeignKey(e => e.Okato);

            modelBuilder.Entity<O002>()
                .HasMany(e => e.FactInsuredRegisters)
                .WithOptional(e => e.O002)
                .HasForeignKey(e => e.Territory);

            modelBuilder.Entity<O003>()
                .Property(e => e.STATUS)
                .HasPrecision(5, 0);

            modelBuilder.Entity<O004>()
                .Property(e => e.STATUS)
                .HasPrecision(5, 0);

            modelBuilder.Entity<O005>()
                .Property(e => e.STATUS)
                .HasPrecision(5, 0);

            modelBuilder.Entity<R002>()
                .HasMany(e => e.FactInsuredRegisters)
                .WithOptional(e => e.R002)
                .HasForeignKey(e => e.InsuranceDocForm);

            modelBuilder.Entity<R003>()
                .HasMany(e => e.FactInsuredRegisters)
                .WithOptional(e => e.R003)
                .HasForeignKey(e => e.ApplicationMethod);

            modelBuilder.Entity<R003>()
                .HasMany(e => e.FactInsuredRegisters1)
                .WithOptional(e => e.R0031)
                .HasForeignKey(e => e.InsuranceDocMethod);

            modelBuilder.Entity<SchemaChanx>()
                .Property(e => e.MajorReleaseNumber)
                .IsUnicode(false);

            modelBuilder.Entity<SchemaChanx>()
                .Property(e => e.MinorReleaseNumber)
                .IsUnicode(false);

            modelBuilder.Entity<SchemaChanx>()
                .Property(e => e.PointReleaseNumber)
                .IsUnicode(false);

            modelBuilder.Entity<SchemaChanx>()
                .Property(e => e.ScriptName)
                .IsUnicode(false);

            modelBuilder.Entity<shareKsg>()
                .Property(e => e.Rate)
                .HasPrecision(19, 4);

            modelBuilder.Entity<shareKsg>()
                .Property(e => e.Rate2)
                .HasPrecision(19, 4);

            modelBuilder.Entity<shareKsg>()
                .Property(e => e.Rate3)
                .HasPrecision(19, 4);

            modelBuilder.Entity<shareKsg>()
                .Property(e => e.Rate4)
                .HasPrecision(19, 4);

            modelBuilder.Entity<shareKsg>()
                .Property(e => e.RateK)
                .HasPrecision(19, 4);

            modelBuilder.Entity<shareKsg>()
                .Property(e => e.RateD)
                .HasPrecision(19, 4);

            modelBuilder.Entity<shareMedicalDepartment>()
                .HasMany(e => e.shareOperatingSchedules)
                .WithOptional(e => e.shareMedicalDepartment)
                .HasForeignKey(e => e.DepartmentId);

            modelBuilder.Entity<shareMedicalSubdivision>()
                .HasMany(e => e.shareOperatingSchedules)
                .WithOptional(e => e.shareMedicalSubdivision)
                .HasForeignKey(e => e.SubdivisionId);

            modelBuilder.Entity<shareRate>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<shareRate>()
                .HasMany(e => e.localParticularSignCompositions)
                .WithRequired(e => e.shareRate)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<subAcademicDegree>()
                .HasMany(e => e.subExpertAcademicDegrees)
                .WithRequired(e => e.subAcademicDegree)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<subDayOfWeek>()
                .HasMany(e => e.globalNonworkingDays)
                .WithRequired(e => e.subDayOfWeek)
                .HasForeignKey(e => e.DayWeek)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<V001>()
                .Property(e => e.IDRB)
                .HasPrecision(12, 0);

            modelBuilder.Entity<V002>()
                .Property(e => e.IDPR)
                .HasPrecision(3, 0);

            modelBuilder.Entity<V002>()
                .HasMany(e => e.FactMedicalEvents)
                .WithOptional(e => e.V002)
                .HasForeignKey(e => e.ProfileCodeId);

            modelBuilder.Entity<V002>()
                .HasMany(e => e.globalEquivalentDatas)
                .WithRequired(e => e.V002)
                .HasForeignKey(e => e.ProfileId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<V002>()
                .HasMany(e => e.globalMedicalAssistanceVolumes)
                .WithOptional(e => e.V002)
                .HasForeignKey(e => e.Profile);

            modelBuilder.Entity<V002>()
                .HasMany(e => e.shareMedicalOrganizationProfiles)
                .WithOptional(e => e.V002)
                .HasForeignKey(e => e.V002ProfileId);

            modelBuilder.Entity<V002>()
                .HasMany(e => e.shareRates)
                .WithOptional(e => e.V002)
                .HasForeignKey(e => e.ProfileId);

            modelBuilder.Entity<V003>()
                .Property(e => e.IDLIC)
                .HasPrecision(3, 0);

            modelBuilder.Entity<V003>()
                .Property(e => e.IERARH)
                .HasPrecision(3, 0);

            modelBuilder.Entity<V003>()
                .Property(e => e.PRIM)
                .HasPrecision(1, 0);

            modelBuilder.Entity<V004>()
                .Property(e => e.IDMSP)
                .HasPrecision(9, 0);

            modelBuilder.Entity<V004>()
                .HasMany(e => e.FactMedicalEvents)
                .WithOptional(e => e.V004)
                .HasForeignKey(e => e.SpecialityCode);

            modelBuilder.Entity<V004>()
                .HasMany(e => e.globalEquivalentDatas)
                .WithRequired(e => e.V004)
                .HasForeignKey(e => e.SpecialityId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<V004>()
                .HasMany(e => e.shareDoctors)
                .WithOptional(e => e.V004)
                .HasForeignKey(e => e.SpecialityId);

            modelBuilder.Entity<V005>()
                .Property(e => e.IDPOL)
                .HasPrecision(1, 0);

            modelBuilder.Entity<V005>()
                .HasMany(e => e.FactPersons)
                .WithOptional(e => e.V005)
                .HasForeignKey(e => e.RepresentativeSex);

            modelBuilder.Entity<V005>()
                .HasMany(e => e.FactPersons1)
                .WithOptional(e => e.V0051)
                .HasForeignKey(e => e.Sex);

            modelBuilder.Entity<V006>()
                .Property(e => e.IDUMP)
                .HasPrecision(2, 0);

            modelBuilder.Entity<V006>()
                .HasMany(e => e.FactEconomicPayments)
                .WithRequired(e => e.V006)
                .HasForeignKey(e => e.AssistanceConditionsId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<V006>()
                .HasMany(e => e.FactEconomicPaymentDetails)
                .WithRequired(e => e.V006)
                .HasForeignKey(e => e.AssistanceConditionsId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<V006>()
                .HasMany(e => e.FactEconomicRefuseDetails)
                .WithRequired(e => e.V006)
                .HasForeignKey(e => e.AssistanceConditionsId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<V006>()
                .HasMany(e => e.FactEconomicSurchargeDetails)
                .WithRequired(e => e.V006)
                .HasForeignKey(e => e.AssistanceConditionsId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<V006>()
                .HasMany(e => e.FactMedicalEvents)
                .WithOptional(e => e.V006)
                .HasForeignKey(e => e.AssistanceConditions);

            modelBuilder.Entity<V006>()
                .HasMany(e => e.shareMedicalOrganizationProfiles)
                .WithOptional(e => e.V006)
                .HasForeignKey(e => e.AssistanceConditionsId);

            modelBuilder.Entity<V006>()
                .HasMany(e => e.shareMedicalSubdivisions)
                .WithRequired(e => e.V006)
                .HasForeignKey(e => e.AssistanceConditionsId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<V007>()
                .Property(e => e.IDNMO)
                .HasPrecision(6, 0);

            modelBuilder.Entity<V008>()
                .Property(e => e.IDVMP)
                .HasPrecision(2, 0);

            modelBuilder.Entity<V008>()
                .HasMany(e => e.FactMedicalEvents)
                .WithOptional(e => e.V008)
                .HasForeignKey(e => e.AssistanceType);

            modelBuilder.Entity<V009>()
                .Property(e => e.IDRMP)
                .HasPrecision(3, 0);

            modelBuilder.Entity<V009>()
                .Property(e => e.DL_USLOV)
                .HasPrecision(2, 0);

            modelBuilder.Entity<V009>()
                .HasMany(e => e.FactMedicalEvents)
                .WithOptional(e => e.V009)
                .HasForeignKey(e => e.Result);

            modelBuilder.Entity<V010>()
                .Property(e => e.IDSP)
                .HasPrecision(2, 0);

            modelBuilder.Entity<V010>()
                .HasMany(e => e.FactMedicalEvents)
                .WithOptional(e => e.V010)
                .HasForeignKey(e => e.PaymentMethod);

            modelBuilder.Entity<V010>()
                .HasMany(e => e.shareRates)
                .WithOptional(e => e.V010)
                .HasForeignKey(e => e.UnitId);

            modelBuilder.Entity<V012>()
                .Property(e => e.IDIZ)
                .HasPrecision(3, 0);

            modelBuilder.Entity<V012>()
                .Property(e => e.DL_USLOV)
                .HasPrecision(2, 0);

            modelBuilder.Entity<V012>()
                .HasMany(e => e.FactMedicalEvents)
                .WithOptional(e => e.V012)
                .HasForeignKey(e => e.Outcome);

            modelBuilder.Entity<EventShortView>()
                .Property(e => e.AssistanceConditions)
                .HasPrecision(2, 0);

            modelBuilder.Entity<EventShortView>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EventShortView>()
                .Property(e => e.Rate)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EventShortView>()
                .Property(e => e.Quantity)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EventShortView>()
                .Property(e => e.AcceptPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EventShortView>()
                .Property(e => e.Profile)
                .HasPrecision(3, 0);

            modelBuilder.Entity<EventShortView>()
                .Property(e => e.Speciality)
                .HasPrecision(9, 0);

            modelBuilder.Entity<EventShortView>()
                .Property(e => e.Result)
                .HasPrecision(3, 0);

            modelBuilder.Entity<EventShortView>()
                .Property(e => e.Outcome)
                .HasPrecision(3, 0);

            modelBuilder.Entity<EventShortView>()
                .Property(e => e.MEC)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EventShortView>()
                .Property(e => e.MEE)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EventShortView>()
                .Property(e => e.EQMA)
                .HasPrecision(19, 4);

            modelBuilder.Entity<EventShortView>()
                .Property(e => e.AssistanceType)
                .HasPrecision(2, 0);

            modelBuilder.Entity<MedicalAccountView>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<MedicalAccountView>()
                .Property(e => e.AcceptPrice)
                .HasPrecision(19, 4);

            modelBuilder.Entity<MedicalAccountView>()
                .Property(e => e.MECPenalties)
                .HasPrecision(19, 4);

            modelBuilder.Entity<MedicalAccountView>()
                .Property(e => e.MEEPenalties)
                .HasPrecision(19, 4);

            modelBuilder.Entity<MedicalAccountView>()
                .Property(e => e.EQMAPenalties)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TerritoryAccountView>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TerritoryAccountView>()
                .Property(e => e.MECPenalties)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TerritoryAccountView>()
                .Property(e => e.MEEPenalties)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TerritoryAccountView>()
                .Property(e => e.EQMAPenalties)
                .HasPrecision(19, 4);

            modelBuilder.Entity<TerritoryAccountView>()
                .Property(e => e.TotalPaymentAmount)
                .HasPrecision(19, 4);
        }
    }
}
