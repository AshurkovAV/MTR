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
DECLARE @p0 Decimal(5,4);
SET @p0 = 0;
DECLARE @p3 NVarChar(1000);
SET @p3 = '';
DECLARE @AccountID Int;
SET @AccountID = 1;
DECLARE @PatientID Int;
SET @PatientID = 1;

-- EndRegion
SELECT [t0].[MedicalServicesId]
FROM [FactMedicalServices] AS [t0]
INNER JOIN [FactMedicalEvent] AS [t1] ON [t1].[MedicalEventId] = [t0].[MedicalEventId]
LEFT OUTER JOIN [FactPatient] AS [t2] ON [t2].[PatientId] = [t1].[PatientId]
WHERE (
([t0].[Profile] IS NULL) OR 
([t0].[IsChildren] IS NULL) OR 
([t0].[ServiceBegin] IS NULL) OR 
([t0].[ServiceEnd] IS NULL) OR 
([t0].[Diagnosis] IS NULL) OR 
([t0].[Quantity] IS NULL) OR 
([t0].[Quantity] = @p0) OR 
([t0].[Rate] IS NULL) OR 
([t0].[Rate] = @p0) OR 
([t0].[Price] IS NULL) OR 
([t0].[Price] = @p0) OR 
([t0].[SpecialityCode] IS NULL) OR 
([t0].[ServiceName] IS NULL) OR 
([t0].[ServiceName] = @p3)) AND 
([t2].[AccountId] = @AccountID) AND 
([t2].[PatientId] = @PatientID)
