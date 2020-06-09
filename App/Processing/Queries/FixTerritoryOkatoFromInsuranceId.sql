use medicine_ins_new
--@MedicalAccountID только для тестов
DECLARE @MedicalAccountID Int = 832
-- EndRegion
UPDATE FactPatient
SET TerritoryOkato = t2.Id
FROM FactPatient AS t0
    INNER JOIN F002 AS t1 ON t1.Id = t0.InsuranceId
		INNER JOIN F010 AS t2 ON t1.tf_okato = t2.KOD_OKATO
		WHERE (TerritoryOkato IS NULL AND NOT InsuranceId IS NULL) AND t0.MedicalAccountId = @MedicalAccountID