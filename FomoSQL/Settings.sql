/*
	Table for storing generic Key Value Pairs
*/
CREATE TABLE [dbo].[Settings]
(
	[Id] VARCHAR(100) NOT NULL PRIMARY KEY,
	[Value] NVARCHAR(1000) NOT NULL
)
