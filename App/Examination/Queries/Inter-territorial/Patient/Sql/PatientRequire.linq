<Query Kind="SQL">
  <Connection>
    <ID>b1996b57-fd46-492c-bef8-8d58f766e8f6</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

-- Region Parameters
DECLARE @p0 NVarChar(1000);
SET @p0 = '';
DECLARE @p6 Int;
SET @p6 = 3;
DECLARE @p7 Int;
SET @p7 = 0;
DECLARE @AccountID Int;
SET @AccountID = 1;
DECLARE @PatientID Int;
SET @PatientID = 1;

-- EndRegion
SELECT [t0].[PatientId]
FROM [FactPatient] AS [t0]
LEFT OUTER JOIN [FactPerson] AS [t1] ON [t1].[PersonId] = [t0].[PersonalId]
WHERE (
((([t0].[InsuranceDocNumber] IS NULL) OR ([t0].[InsuranceDocNumber] = @p0)) AND (([t0].[INP] IS NULL) OR ([t0].[INP] = @p0))) OR 
([t0].[InsuranceDocType] IS NULL) OR 
([t1].[Surname] IS NULL) OR 
([t1].[Surname] = @p0) OR 
([t1].[PName] IS NULL) OR 
([t1].[PName] = @p0) OR 
([t1].[Patronymic] IS NULL) OR 
([t1].[Patronymic] = @p0) OR 
([t1].[Sex] IS NULL) OR 
([t1].[Birthday] IS NULL) OR 
([t0].[Newborn] IS NULL) OR 
([t0].[Newborn] = @p0) OR 
(([t0].[InsuranceDocType] <> @p6) AND (((
	SELECT COUNT(*)
	FROM [FactDocument] AS [t2]
	WHERE [t2].[PersonId] = [t1].[PersonId]
	)) = @p7))) AND 
	([t0].[AccountId] = @AccountID) AND 
	([t0].[PatientId] = @PatientID)
