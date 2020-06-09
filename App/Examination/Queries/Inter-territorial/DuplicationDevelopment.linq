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
DECLARE @p0 Int = 65
DECLARE @p1 Int = 1
-- EndRegion
SELECT [t2].[EventId]
FROM (
	SELECT COUNT(*) AS [value], [t0].[Speciality], [t0].[EventBegin], [t0].[EventEnd], [t0].[Profile]
	FROM [EventShortView] AS [t0]
	WHERE [t0].[AccountId] = @p0
	GROUP BY [t0].[Speciality], [t0].[EventBegin], [t0].[EventEnd], [t0].[Profile]
	) AS [t1]
CROSS JOIN [EventShortView] AS [t2]
WHERE ([t1].[value] > @p1) AND 
(
	(([t1].[Speciality] IS NULL) AND ([t2].[Speciality] IS NULL)) OR 
	(
		([t1].[Speciality] IS NOT NULL) AND 
		([t2].[Speciality] IS NOT NULL) AND 
		(
			(([t1].[Speciality] IS NULL) AND ([t2].[Speciality] IS NULL)) OR 
			(([t1].[Speciality] IS NOT NULL) AND ([t2].[Speciality] IS NOT NULL) AND ([t1].[Speciality] = [t2].[Speciality]))
		)
	)
) 
AND 
((([t1].[EventBegin] IS NULL) AND ([t2].[EventBegin] IS NULL)) OR (([t1].[EventBegin] IS NOT NULL) AND ([t2].[EventBegin] IS NOT NULL) AND ((([t1].[EventBegin] IS NULL) AND ([t2].[EventBegin] IS NULL)) OR (([t1].[EventBegin] IS NOT NULL) AND ([t2].[EventBegin] IS NOT NULL) AND ([t1].[EventBegin] = [t2].[EventBegin]))))) 
AND 
((([t1].[EventEnd] IS NULL) AND ([t2].[EventEnd] IS NULL)) OR (([t1].[EventEnd] IS NOT NULL) AND ([t2].[EventEnd] IS NOT NULL) AND ((([t1].[EventEnd] IS NULL) AND ([t2].[EventEnd] IS NULL)) OR (([t1].[EventEnd] IS NOT NULL) AND ([t2].[EventEnd] IS NOT NULL) AND ([t1].[EventEnd] = [t2].[EventEnd]))))) 
AND 
((([t1].[Profile] IS NULL) AND ([t2].[Profile] IS NULL)) OR (([t1].[Profile] IS NOT NULL) AND ([t2].[Profile] IS NOT NULL) AND ((([t1].[Profile] IS NULL) AND ([t2].[Profile] IS NULL)) OR (([t1].[Profile] IS NOT NULL) AND ([t2].[Profile] IS NOT NULL) AND ([t1].[Profile] = [t2].[Profile]))))) 

AND ([t2].[AccountId] = @p0)
