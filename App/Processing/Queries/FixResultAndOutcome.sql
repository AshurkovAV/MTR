use medicine_ins_new
--@MedicalAccountID только для тестов
DECLARE @MedicalAccountID Int = 1035
-- EndRegion
UPDATE [FactMedicalEvent]
SET [Result] = null
FROM [FactMedicalEvent] AS [t0]
    INNER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
    INNER JOIN v009 on V009.id = t0.Result and FLOOR(V009.idrmp / 100) <> t0.assistanceconditions
		WHERE [t1].[MedicalAccountId] = @MedicalAccountID



UPDATE [FactMedicalEvent]
SET [Outcome] = null
FROM [FactMedicalEvent] AS [t0]
    INNER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
    INNER JOIN v012 on V012.id = t0.Outcome and FLOOR(V012.idiz / 100) <> t0.assistanceconditions
		WHERE [t1].[MedicalAccountId] = @MedicalAccountID