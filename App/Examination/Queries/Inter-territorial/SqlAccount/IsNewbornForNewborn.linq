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
SET @p0 = N'нет';
DECLARE @p3 NVarChar(1000);
SET @p3 = '';
DECLARE @p4 NVarChar(1000);
SET @p4 = '0'
DECLARE @AccountID Int;
SET @AccountID = 1;
-- EndRegion
SELECT [t0].[PatientId]
FROM [FactPatient] AS [t0]
LEFT OUTER JOIN [FactPerson] AS [t1] ON [t1].[PersonId] = [t0].[PersonalId]
WHERE (LOWER([t1].[PName]) = @p0) AND (LOWER([t1].[Surname]) = @p0) AND (LOWER([t1].[Patronymic]) = @p0) AND 
(([t0].[Newborn] IS NULL) OR ([t0].[Newborn] = @p3) OR ([t0].[Newborn] = @p4)) AND ([t0].[AccountId] = @AccountID)
