use medicine_ins
--@MedicalAccountID только для тестов
DECLARE @MedicalAccountID Int = 1535
DECLARE @EmptyString NVarChar(1) = ''
-- EndRegion
UPDATE [FactMedicalEvent]
SET [History] = [MedicalEventId]
FROM [FactMedicalEvent] AS [t0]
    LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
		WHERE ([History] IS NULL OR LTRIM(RTRIM([History])) = @EmptyString ) AND [t1].[MedicalAccountId] = @MedicalAccountID