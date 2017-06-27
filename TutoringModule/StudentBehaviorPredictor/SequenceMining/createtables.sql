USE [LabBiotecnologia]
GO

/****** Object:  Table [dbo].[Actions]    Script Date: 24/09/2015 16:29:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

if object_id('dbo.Logs', 'U') is not null
DROP TABLE dbo.Logs
GO

CREATE TABLE dbo.Logs (
[studentid] nvarchar(max) NOT NULL,
[domain] nvarchar(max) NOT NULL,
[actionkey] nvarchar(max) NOT NULL,
[sequence] int NOT NULL,
[date] datetime NOT NULL
)

GO

if object_id('dbo.LogsPhase1', 'U') is not null
DROP TABLE dbo.Logs
GO

CREATE TABLE dbo.LogsPhase1 (
[studentid] nvarchar(max) NOT NULL,
[domain] nvarchar(max) NOT NULL,
[actionkey] nvarchar(max) NOT NULL,
[sequence] int NOT NULL,
[date] datetime NOT NULL
)

GO

if object_id('dbo.LogsPhase2', 'U') is not null
DROP TABLE dbo.Logs
GO

CREATE TABLE dbo.LogsPhase2 (
[studentid] nvarchar(max) NOT NULL,
[domain] nvarchar(max) NOT NULL,
[actionkey] nvarchar(max) NOT NULL,
[sequence] int NOT NULL,
[date] datetime NOT NULL
)

GO

if object_id('dbo.LogsPhase3', 'U') is not null
DROP TABLE dbo.Logs
GO

CREATE TABLE dbo.LogsPhase3 (
[studentid] nvarchar(max) NOT NULL,
[domain] nvarchar(max) NOT NULL,
[actionkey] nvarchar(max) NOT NULL,
[sequence] int NOT NULL,
[date] datetime NOT NULL
)

GO

if object_id('dbo.Students', 'U') is not null
DROP TABLE dbo.[Students]
GO

CREATE TABLE [dbo].[Students](
	[id] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

if object_id('dbo.LogsVal', 'U') is not null
DROP TABLE dbo.LogsVal
GO

CREATE TABLE dbo.LogsVal (
[studentid] nvarchar(max) NOT NULL,
[domain] nvarchar(max) NOT NULL,
[actionkey] nvarchar(max) NOT NULL,
[sequence] int NOT NULL,
[date] datetime NOT NULL
)

GO

if object_id('dbo.StudentsVal', 'U') is not null
DROP TABLE dbo.StudentsVal
GO

CREATE TABLE [dbo].StudentsVal(
	[id] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO