use medicine_ins_new
--@MedicalAccountID только для тестов
DECLARE @MedicalAccountID Int = 821
-- EndRegion
UPDATE [FactPerson]
SET [RepresentativeName] = NULL,
		[RepresentativeSurname] = NULL,
		[RepresentativePatronymic] = NULL,
		[RepresentativeBirthday] = NULL,
		[RepresentativeSex] = NULL,
		[RepresentativeContacts] = NULL
FROM FactPerson p
INNER JOIN FactPatient t1
ON t1.PersonalId=p.PersonId
WHERE (Newborn is NULL OR RTRIM(LTRIM(Newborn)) = '' OR Newborn = '0') AND 
((p.RepresentativeName is NOT NULL AND RTRIM(LTRIM(p.RepresentativeName))<> '') OR 
(p.RepresentativeSurname is NOT NULL AND RTRIM(LTRIM(p.RepresentativeSurname))<> '') OR
(p.RepresentativePatronymic is NOT NULL AND RTRIM(LTRIM(p.RepresentativePatronymic))<> '') OR
(p.RepresentativeBirthday is NOT NULL AND RTRIM(LTRIM(p.RepresentativeBirthday))<> '') OR 
(p.RepresentativeSex is NOT NULL AND RTRIM(LTRIM(p.RepresentativeSex))<> '') OR 
(p.RepresentativeContacts is NOT NULL AND RTRIM(LTRIM(p.RepresentativeContacts))<> '')) AND
[t1].[MedicalAccountId] = @MedicalAccountID