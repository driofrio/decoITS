USE [LabBiotecnologia]


declare @studentid nvarchar(max);
declare @actionid nvarchar(max);
declare @date datetime;
declare @secuence int;

if object_id('tempdb..#temp') is not null
	DROP TABLE #temp

if object_id('tempdb..#tmpLogs') is not null
	DROP TABLE #tmpLogs

select [id] into #temp
from [dbo].[Students]
order by [id]

while exists (select * from #temp)
begin
	select top 1 @studentid=[id] from #temp;

	SELECT [studentid],actionkey, [date] into #tmpLogs
		FROM [dbo].[Logs]
		where [studentid]=@studentid
		order by [date]
	set @secuence=1;
	while exists(select * from #tmpLogs)
	begin
		select top 1 @actionid=actionkey, @date=[date] from #tmpLogs;
		update [dbo].[Logs] set [secuence]=@secuence
		where [studentid]=@studentid and [actionkey]=@actionid and [date]=@date
		set @secuence=@secuence+1;
		delete #tmpLogs where [actionkey]=@actionid and [date]=@date;
	end

	delete #temp where [id]=@studentid;
	set @actionid='';
	DROP TABLE #tmpLogs
end

if object_id('tempdb..#tempVal') is not null
	DROP TABLE #tempVal

if object_id('tempdb..#tmpValLogs') is not null
	DROP TABLE #tmpValLogs

select [id] into #tempVal
from [dbo].[StudentsVal]
order by [id]

while exists (select * from #tempVal)
begin
	select top 1 @studentid=[id] from #tempVal;

	SELECT [studentid],actionkey, [date] into #tmpValLogs
		FROM [dbo].[LogsVal]
		where [studentid]=@studentid
		order by [date]
	set @secuence=1;
	while exists(select * from #tmpValLogs)
	begin
		select top 1 @actionid=actionkey, @date=[date] from #tmpValLogs;
		update [dbo].[LogsVal] set [secuence]=@secuence
		where [studentid]=@studentid and [actionkey]=@actionid and [date]=@date
		set @secuence=@secuence+1;
		delete #tmpValLogs where [actionkey]=@actionid and [date]=@date;
	end

	delete #tempVal where [id]=@studentid;
	set @actionid='';
	DROP TABLE #tmpValLogs
end