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
SET @p0 = 3;
DECLARE @p1 NVarChar(1000);
SET @p1 = '';
DECLARE @AccountID Int;
SET @AccountID = 1;
DECLARE @PatientID Int;
SET @PatientID = 103;
-- EndRegion
SELECT [t0].[PatientId]
FROM [FactPatient] AS [t0]
WHERE ([t0].[InsuranceDocType] <> @p0) AND ((((
	SELECT [t3].[DocType]
	FROM (
		SELECT TOP (1) [t2].[DocType]
		FROM [FactPerson] AS [t1], [FactDocument] AS [t2]
		WHERE ([t1].[PersonId] = [t0].[PersonalId]) AND ([t2].[PersonId] = [t1].[PersonId])
		) AS [t3]
	)) IS NULL) OR (((
	SELECT [t6].[DocNum]
	FROM (
		SELECT TOP (1) [t5].[DocNum]
		FROM [FactPerson] AS [t4], [FactDocument] AS [t5]
		WHERE ([t4].[PersonId] = [t0].[PersonalId]) AND ([t5].[PersonId] = [t4].[PersonId])
		) AS [t6]
	)) IS NULL) OR (((
	SELECT [t9].[DocSeries]
	FROM (
		SELECT TOP (1) [t8].[DocSeries]
		FROM [FactPerson] AS [t7], [FactDocument] AS [t8]
		WHERE ([t7].[PersonId] = [t0].[PersonalId]) AND ([t8].[PersonId] = [t7].[PersonId])
		) AS [t9]
	)) IS NULL) OR (((
	SELECT [t12].[DocNum]
	FROM (
		SELECT TOP (1) [t11].[DocNum]
		FROM [FactPerson] AS [t10], [FactDocument] AS [t11]
		WHERE ([t10].[PersonId] = [t0].[PersonalId]) AND ([t11].[PersonId] = [t10].[PersonId])
		) AS [t12]
	)) = @p1) OR (((
	SELECT [t15].[DocSeries]
	FROM (
		SELECT TOP (1) [t14].[DocSeries]
		FROM [FactPerson] AS [t13], [FactDocument] AS [t14]
		WHERE ([t13].[PersonId] = [t0].[PersonalId]) AND ([t14].[PersonId] = [t13].[PersonId])
		) AS [t15]
	)) = @p1)) AND ([t0].[AccountId] = @AccountID) AND  ([t0].[PatientId] = @PatientID)
