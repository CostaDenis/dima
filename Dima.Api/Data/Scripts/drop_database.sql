USE [master];
GO

ALTER DATABASE [Dima] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

DROP DATABASE [Dima];
GO




use[master]
DECLARE @kill varchar(8000) = '';
SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), session_id) + ';'
FROM sys.dm_exec_sessions
WHERE database_id  = db_id('Dima')

    EXEC(@kill);

DROP DATABASE [Dima]