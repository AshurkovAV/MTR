use medicine_ins
--@MedicalAccountID только для тестов
DECLARE @MedicalAccountID Int = 821
DECLARE @Zero Int = 0
-- EndRegion
UPDATE [FactMedicalEvent]
SET [AssistanceType] = 31,
	  [PaymentMethod] = 33	
FROM [FactMedicalEvent] AS [t0]
    LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		WHERE ([AssistanceConditions] = 1 ) AND [t1].[MedicalAccountId] = @MedicalAccountID

UPDATE [FactMedicalEvent]
SET [AssistanceType] = 31,
	  [PaymentMethod] = 34	
FROM [FactMedicalEvent] AS [t0]
    LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		WHERE ([AssistanceConditions] = 2 ) AND [t1].[MedicalAccountId] = @MedicalAccountID

UPDATE [FactMedicalEvent]
SET [AssistanceType] = 13
FROM [FactMedicalEvent] AS [t0]
    LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		WHERE ([AssistanceConditions] = 3 ) AND [t1].[MedicalAccountId] = @MedicalAccountID

UPDATE [FactMedicalEvent]
SET [AssistanceType] = 12
FROM [FactMedicalEvent] AS [t0]
    LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		WHERE ([AssistanceConditions] = 3 AND [ProfileCodeId] IN (97, 68, 58)) AND [t1].[MedicalAccountId] = @MedicalAccountID

UPDATE [FactMedicalEvent]
SET [AssistanceType] = 11
FROM [FactMedicalEvent] AS [t0]
    LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		WHERE ([AssistanceConditions] = 3 AND [ProfileCodeId] IN (3, 42)) AND [t1].[MedicalAccountId] = @MedicalAccountID

UPDATE [FactMedicalEvent]
SET [PaymentMethod] = 30
FROM [FactMedicalEvent] AS [t0]
    INNER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
    INNER JOIN V009 ON V009.Id = t0.Result
  WHERE ([AssistanceConditions] = 3 AND [IDRMP] BETWEEN 316 AND 351 AND [PaymentMethod] <> 12) AND [t1].[MedicalAccountId] = @MedicalAccountID

UPDATE [FactMedicalEvent]
SET [PaymentMethod] = 29
FROM [FactMedicalEvent] AS [t0]
    INNER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
    INNER JOIN V009 ON V009.Id = t0.Result
  WHERE ([AssistanceConditions] = 3 AND [IDRMP] NOT BETWEEN 316 AND 351 AND [PaymentMethod] <> 12) AND [t1].[MedicalAccountId] = @MedicalAccountID

UPDATE [FactMedicalEvent]
SET [AssistanceType] = 21,
		[PaymentMethod] = 24
FROM [FactMedicalEvent] AS [t0]
    LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		WHERE ([AssistanceConditions] = 4) AND [t1].[MedicalAccountId] = @MedicalAccountID

UPDATE [FactMedicalEvent]
SET [SpecialityCodeV015] = 25
FROM [FactMedicalEvent] AS [t0]
    LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		WHERE ([AssistanceConditions] = 4 AND [SpecialityCode] IS NULL) AND [t1].[MedicalAccountId] = @MedicalAccountID

UPDATE [FactMedicalEvent]
SET [ProfileCodeId] = 84
FROM [FactMedicalEvent] AS [t0]
    LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		WHERE ([AssistanceConditions] = 4 AND [ProfileCodeId] IS NULL) AND [t1].[MedicalAccountId] = @MedicalAccountID

UPDATE [FactMedicalEvent]
SET [ProfileCodeId] = 84
FROM [FactMedicalEvent] AS [t0]
    LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		WHERE ([AssistanceConditions] = 4 AND [ProfileCodeId] IS NULL) AND [t1].[MedicalAccountId] = @MedicalAccountID
