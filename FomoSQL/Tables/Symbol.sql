﻿CREATE TABLE [dbo].[Symbol]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Ticker] VARCHAR(5) NOT NULL,
	[Exchange] VARCHAR(10) NOT NULL
)