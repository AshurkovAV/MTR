SELECT t0.MedicalServicesId
FROM FactMedicalServices AS t0
INNER JOIN FactMedicalEvent AS t1 ON t1.MedicalEventId = t0.MedicalEventId
LEFT OUTER JOIN FactPatient AS t2 ON t2.PatientId = t1.PatientId
WHERE (

t0.MedicalOrganization IS NULL OR
t0.Profile IS NULL OR 
t0.IsChildren IS NULL OR 
t0.ServiceBegin IS NULL OR 
t0.ServiceEnd IS NULL OR
t0.Diagnosis IS NULL OR 
(t0.ServiceCode IS NULL OR RTRIM(LTRIM(t0.ServiceCode)) = '') OR 
t0.Quantity IS NULL OR 
t0.Price IS NULL OR 
t0.SpecialityCode IS NULL OR 
(t0.DoctorId IS NULL OR RTRIM(LTRIM(t0.DoctorId)) = '') 

) 
AND (t2.MedicalAccountId = @MedicalAccountID)