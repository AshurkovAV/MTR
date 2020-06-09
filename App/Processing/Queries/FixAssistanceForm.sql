use medicine_ins_new
--@MedicalAccountID только для тестов
DECLARE @MedicalAccountID Int = 821
DECLARE @Zero Int = 0
-- EndRegion
UPDATE [FactMedicalEvent]
SET [AssistanceForm] = 3
FROM [FactMedicalEvent] AS [t0]
    LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		WHERE ([AssistanceConditions] IN (1,2) AND ([AssistanceForm] IS NULL OR AssistanceForm = @Zero) ) AND [t1].[MedicalAccountId] = @MedicalAccountID

UPDATE [FactMedicalEvent]
SET [AssistanceForm] = 3
FROM [FactMedicalEvent] AS [t0]
    LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		WHERE ([AssistanceConditions] = 3 AND [Rate] IN (351.00, 356.48, 327.8, 330.29)) AND [t1].[MedicalAccountId] = @MedicalAccountID

UPDATE [FactMedicalEvent]
SET [AssistanceForm] = 2
FROM [FactMedicalEvent] AS [t0]
    LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		WHERE ([AssistanceConditions] = 3 AND [Rate] IN (449.3, 456.34)) AND [t1].[MedicalAccountId] = @MedicalAccountID

UPDATE [FactMedicalEvent]
SET [AssistanceForm] = 2
FROM [FactMedicalEvent] AS [t0]
    LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		WHERE ([AssistanceConditions] = 4) AND [t1].[MedicalAccountId] = @MedicalAccountID

UPDATE [FactMedicalEvent]
SET [AssistanceForm] = 1
FROM [FactMedicalEvent] AS [t0]
    LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		WHERE ([AssistanceConditions] = 4 AND [Rate] > 1000.0) AND [t1].[MedicalAccountId] = @MedicalAccountID