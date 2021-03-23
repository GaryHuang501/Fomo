﻿CREATE TABLE [dbo].[Vote]
(
	[Id] BIGINT IDENTITY(1,1) PRIMARY KEY,
	[SymbolId] INT NOT NULL,
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[Direction] SMALLINT NOT NULL,
	[LastUpdated] DateTime NOT NULL,
	UNIQUE(SymbolId, UserId),
	CONSTRAINT FK_Vote_SymbolId FOREIGN KEY (SymbolId) REFERENCES Symbol(Id),
	CONSTRAINT FK_Vote_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id),
	CONSTRAINT CHK_Vote_Direction CHECK (Direction IN(-1, 0, 1))
)

GO;

CREATE NONCLUSTERED INDEX IX_Vote_SymbolId ON Vote(SymbolId);
GO;

CREATE NONCLUSTERED INDEX IX_Vote_UserId ON Vote(UserId);
GO;