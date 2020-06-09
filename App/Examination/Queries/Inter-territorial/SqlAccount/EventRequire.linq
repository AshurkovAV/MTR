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
DECLARE @p1 Decimal(5,4);
SET @p1 = 0;
DECLARE @AccountID Int;
SET @AccountID = 1;--Удалить при загрузке в БД
-- EndRegion
SELECT [t0].[MedicalEventId]
FROM [FactMedicalEvent] AS [t0]
LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
WHERE (([t0].[AssistanceConditions] IS NULL) OR 
([t0].[AssistanceType] IS NULL) OR 
([t0].[ProfileCodeId] IS NULL) OR 
([t0].[IsChildren] IS NULL) OR 
([t0].[History] IS NULL) OR 
([t0].[History] = @p0) OR 
([t0].[EventBegin] IS NULL) OR 
([t0].[EventEnd] IS NULL) OR 
([t0].[DiagnosisGeneral] IS NULL) OR 
([t0].[Result] IS NULL) OR 
([t0].[Outcome] IS NULL) OR 
([t0].[SpecialityCode] IS NULL) OR 
([t0].[PaymentStatus] IS NULL) OR 
([t0].[Price] IS NULL) OR 
([t0].[Price] = @p1)) AND 
([t1].[AccountId] = @AccountID)
