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
DECLARE @p1 Float;
SET @p1 = 18;
DECLARE @p2 Int;
SET @p2 = 1;
DECLARE @p3 Decimal(2,0);
--Профиль V002
SET @p3 = 17--детской кардиологии
DECLARE @p4 Decimal(2,0);
SET @p4 = 18--детской онкологии
DECLARE @p5 Decimal(2,0);
SET @p5 = 19--детской урологии-андрологии
DECLARE @p6 Decimal(2,0);
SET @p6 = 20--детской хирургии
DECLARE @p7 Decimal(2,0);
SET @p7 = 21--детской эндокринологии
DECLARE @p8 Decimal(2,0);
SET @p8 = 55--неонатология
DECLARE @p9 Decimal(2,0);
SET @p9 = 68--педиатрия
DECLARE @p10 Decimal(2,0);
SET @p10 = 86--стоматологии детской
--Специальность врача V004
DECLARE @p11 Decimal(2,0);
SET @p11 = 11--Лечебное дело. Педиатрия
DECLARE @p12 Decimal(4,0);
SET @p12 = 1134--Педиатрия
DECLARE @p13 Decimal(4,0);
SET @p13 = 2016--Сестринское дело в педиатрии
DECLARE @p14 Decimal(6,0);
SET @p14 = 112702--Детская эндокринология
DECLARE @p15 Decimal(6,0);
SET @p15 = 112801--Детская онкология
DECLARE @p16 Decimal(6,0);
SET @p16 = 113401--Детская онкология
DECLARE @p17 Decimal(6,0);
SET @p17 = 113402--Детская эндокринология
DECLARE @p18 Decimal(4,0);
SET @p18 = 1135--Детская хирургия
DECLARE @p19 Decimal(6,0);
SET @p19 = 113501--Детская онкология
DECLARE @p20 Decimal(6,0);
SET @p20 = 113502--Детская урология-андрология
DECLARE @p21 Decimal(6,0);
SET @p21 = 140102--Стоматология детская
DECLARE @AccountID Int;
SET @AccountID = 1;
DECLARE @PatientID Int;
SET @PatientID = 3;
-- EndRegion
SELECT [t0].[MedicalEventId]
FROM [FactMedicalEvent] AS [t0]
LEFT OUTER JOIN [FactPatient] AS [t1] ON [t1].[PatientId] = [t0].[PatientId]
LEFT OUTER JOIN [FactPerson] AS [t2] ON [t2].[PersonId] = [t1].[PersonalId]
LEFT OUTER JOIN [V002] AS [t3] ON [t3].[Id] = [t0].[ProfileCodeId]
LEFT OUTER JOIN [V004] AS [t4] ON [t4].[Id] = [t0].[SpecialityCode]
WHERE (FLOOR(DATEDIFF(day,[t2].[Birthday],[t0].[EventBegin])/365.242199) >= @p1) AND 
(([t0].[IsChildren] = @p2) OR 
([t3].[IDPR] = @p3) OR 
([t3].[IDPR] = @p4) OR 
([t3].[IDPR] = @p5) OR 
([t3].[IDPR] = @p6) OR 
([t3].[IDPR] = @p7) OR 
([t3].[IDPR] = @p8) OR 
([t3].[IDPR] = @p9) OR 
([t3].[IDPR] = @p10) OR 
([t4].[IDMSP] = @p11) OR 
([t4].[IDMSP] = @p12) OR 
([t4].[IDMSP] = @p13) OR 
([t4].[IDMSP] = @p14) OR 
([t4].[IDMSP] = @p15) OR 
([t4].[IDMSP] = @p16) OR 
([t4].[IDMSP] = @p17) OR 
([t4].[IDMSP] = @p18) OR 
([t4].[IDMSP] = @p19) OR 
([t4].[IDMSP] = @p20) OR 
([t4].[IDMSP] = @p21)) AND 
([t1].[AccountId] = @AccountID) AND
([t1].[PatientId] = @PatientID)
