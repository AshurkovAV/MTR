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
DECLARE @p0 Decimal(1,0);
SET @p0 = 1;--Стационар
DECLARE @p1 Decimal(6,1);
SET @p1 = 100.0;
DECLARE @p5 Decimal(1,0)
SET @p5 = 5--Койко-день в круглосуточном стационаре

DECLARE @p6 Decimal(1,0);
SET @p6 = 2--Дневной стационар
DECLARE @p11 Decimal(1,0);
SET @p11 = 6--Койко-день в дневном стационаре больничного учреждения
DECLARE @p12 Decimal(1,0);
SET @p12 = 7--День лечения в дневном стационаре АПУ
DECLARE @p13 Decimal(1,0);
SET @p13 = 8--День лечения в стационаре на дому

DECLARE @p14 Decimal(1,0);
SET @p14 = 3--Поликлиника
DECLARE @p19 Decimal(1,0) = 1--Посещение в поликлинике
DECLARE @p20 Decimal(1,0) = 9--УЕТ в стоматологии
DECLARE @AccountID Int;
SET @AccountID = 1
-- EndRegion
SELECT [t0].[MedicalEventId]
FROM [FactMedicalEvent] AS [t0]
LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
LEFT OUTER JOIN [V006] AS [t2] ON [t2].[id] = [t0].[AssistanceConditions]
LEFT OUTER JOIN [V009] AS [t3] ON [t3].[Id] = [t0].[Result]
LEFT OUTER JOIN [V012] AS [t4] ON [t4].[Id] = [t0].[Outcome]
LEFT OUTER JOIN [V010] AS [t5] ON [t5].[Id] = [t0].[PaymentMethod]
LEFT OUTER JOIN [V008] AS [t6] ON [t6].[Id] = [t0].[AssistanceType]
WHERE (
(([t2].[IDUMP] = @p0) AND 
((FLOOR(([t3].[IDRMP]) / @p1) <> @p0) OR (FLOOR(([t4].[IDIZ]) / @p1) <> @p0) OR ([t5].[IDSP] <> @p5))) OR 
(([t2].[IDUMP] = @p6) AND 
((FLOOR(([t3].[IDRMP]) / @p1) <> @p6) OR (FLOOR(([t4].[IDIZ]) / @p1) <> @p6) OR (([t5].[IDSP] <> @p11) AND ([t5].[IDSP] <> @p12) AND ([t5].[IDSP] <> @p13)))) OR 
(([t2].[IDUMP] = @p14) AND 
((FLOOR(([t3].[IDRMP]) / @p1) <> @p14) OR (FLOOR(([t4].[IDIZ]) / @p1) <> @p14) OR (([t5].[IDSP] <> @p19) AND ([t5].[IDSP] <> @p20)))))
AND ([t1].[AccountId] = @AccountID)