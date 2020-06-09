use medicine_ins_new
--@AccountID только для тестов
DECLARE @AccountID Int = 926
-- EndRegion
UPDATE FactMedicalEvent
SET PaymentStatus = 2
FROM FactMedicalEvent AS t0
    LEFT OUTER JOIN FactPatient AS t1 ON t1.PatientId = t0.PatientId
		WHERE (t0.PaymentStatus IS NULL AND t0.Price = t0.AcceptPrice) AND t1.AccountId = @AccountID

UPDATE FactMedicalEvent
SET PaymentStatus = 4
FROM FactMedicalEvent AS t0
    LEFT OUTER JOIN FactPatient AS t1 ON t1.PatientId = t0.PatientId
		WHERE (t0.PaymentStatus IS NULL AND t0.Price > t0.AcceptPrice AND t0.AcceptPrice > 0) AND t1.AccountId = @AccountID

UPDATE FactMedicalEvent
SET PaymentStatus = 3
FROM FactMedicalEvent AS t0
    LEFT OUTER JOIN FactPatient AS t1 ON t1.PatientId = t0.PatientId
		WHERE (t0.PaymentStatus IS NULL AND t0.AcceptPrice = 0) AND t1.AccountId = @AccountID