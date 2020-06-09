DECLARE @MedicalAccountId INT
SET @MedicalAccountId = 1038;

SELECT t0.PatientId
FROM FactPatient AS t0
LEFT OUTER JOIN FactPerson AS t1 ON t1.PersonId = t0.PersonalId
WHERE (
	t1.Sex IS NULL OR
	t1.Birthday IS NULL OR
	t0.InsuranceDocType IS NULL OR
	(t0.InsuranceDocNumber IS NULL AND t0.INP IS NULL) OR
	t0.Newborn IS NULL
) 
AND (t0.MedicalAccountId = @MedicalAccountID)