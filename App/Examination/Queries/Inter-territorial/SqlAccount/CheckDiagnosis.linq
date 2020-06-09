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
DECLARE @p0 Int;
SET @p0 = 5;
DECLARE @p1 Int;
SET @p1 = 0;
DECLARE @p3 Int;
SET @p3 = 1;
DECLARE @AccountID Int;
SET @AccountID = 1;
-- EndRegion
SELECT [t0].[MedicalEventId]
FROM [FactMedicalEvent] AS [t0]
LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
LEFT OUTER JOIN [M001] AS [t2] ON [t2].[Id] = [t0].[DiagnosisGeneral]
WHERE (LEN([t2].[IDDS]) <> @p0) AND (((
	SELECT COUNT(*)
	FROM [M001] AS [t3]
	WHERE ((
		(CASE 
			WHEN (DATALENGTH([t2].[IDDS]) / 2) = 0 THEN 0
			ELSE CHARINDEX([t2].[IDDS], [t3].[IDDS]) - 1
		 END)) = @p1) AND ([t3].[Payable] = @p1)
	)) > @p3) AND ([t1].[AccountId] = @AccountID)