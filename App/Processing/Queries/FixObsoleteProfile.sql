use medicine_ins_new
--@MedicalAccountID только для тестов
DECLARE @MedicalAccountID Int = 821
DECLARE @Zero Int = 0
-- EndRegion
UPDATE [FactMedicalEvent]
SET [ProfileCodeId] = [t2].[NewValue]
FROM [FactMedicalEvent] AS [t0]
    INNER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		INNER JOIN [globalObsoleteData] AS [t2] ON [t2].[OldValue] = [t0].[ProfileCodeId] AND [t2].[Classifier] = 'V002'
		WHERE [t1].[MedicalAccountId] = @MedicalAccountID

