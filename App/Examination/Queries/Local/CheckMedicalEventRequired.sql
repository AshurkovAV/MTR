DECLARE @MedicalAccountID INT
SET @MedicalAccountID = 1038

SELECT t0.MedicalEventId
FROM FactMedicalEvent AS t0
LEFT OUTER JOIN FactPatient AS t1 ON t1.PatientId = t0.PatientId
WHERE (

t0.AssistanceConditions IS NULL OR
t0.AssistanceType IS NULL OR
t0.AssistanceForm IS NULL OR
t0.MedicalOrganizationCode IS NULL OR
t0.ProfileCodeId IS NULL OR
t0.IsChildren IS NULL OR
(t0.History IS NULL OR RTRIM(LTRIM(t0.History)) = '') OR
t0.EventBegin IS NULL OR
t0.EventEnd IS NULL OR
t0.DiagnosisGeneral IS NULL OR
t0.Result IS NULL OR
t0.Outcome IS NULL OR
(t0.SpecialityCode IS NULL AND t0.SpecialityCodeV015 IS NULL) OR 
(t0.DoctorId IS NULL OR RTRIM(LTRIM(t0.DoctorId)) = '') OR
t0.PaymentMethod IS NULL OR
t0.MoPrice IS NULL

) 
AND (t1.MedicalAccountId = @MedicalAccountID)