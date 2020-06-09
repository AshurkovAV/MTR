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
SET @p0 = '0'
DECLARE @p1 NVarChar(1000);
SET @p1= N'нет'
DECLARE @AccountID Int;
SET @AccountID = 1;
DECLARE @PatientID Int;
SET @PatientID = 2;
-- EndRegion
SELECT [t0].[PatientId]
FROM [FactPatient] AS [t0]
LEFT OUTER JOIN [FactPerson] AS [t1] ON [t1].[PersonId] = [t0].[PersonalId]
WHERE ([t0].[Newborn] <> @p0) AND 
((LOWER([t1].[Surname]) <> @p1) OR (LOWER([t1].[Patronymic]) <> @p1) OR (LOWER([t1].[PName]) <> @p1)) AND 
([t0].[AccountId] = @AccountID) AND 
([t0].[PatientId] = @PatientID)