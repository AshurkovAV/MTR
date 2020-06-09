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
DECLARE @p0 Float;
SET @p0 = 365.25;
DECLARE @p1 Float;
SET @p1 = 18;
DECLARE @AccountID Int;
SET @AccountID = 1;
DECLARE @PatientID Int;
SET @PatientID = 72;

-- EndRegion
SELECT [t0].[MedicalEventId]
FROM [FactMedicalEvent] AS [t0]
LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
LEFT OUTER JOIN [FactPerson] AS [t2] ON [t2].[PersonId] = [t1].[PersonalId]
WHERE (FLOOR(DATEDIFF(day,[t2].[Birthday],[t0].[EventBegin])/365.242199) < @p1) AND (([t0].[IsChildren] IS NULL) OR (NOT ([t0].[IsChildren] = 1))) AND ([t1].[AccountId] = @AccountID) AND ([t1].[PatientId] = @PatientID) 