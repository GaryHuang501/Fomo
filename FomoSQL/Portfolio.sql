CREATE TABLE [dbo].[Portfolio]
(
	[Id] INT IDENTITY(1,1) PRIMARY KEY,
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateModified] DATETIME NOT NULL,
	CONSTRAINT FK_Portfolio_UserId FOREIGN KEY ([UserId]) REFERENCES AspNetUsers(Id)
);
GO;

CREATE NONCLUSTERED INDEX IX_Portfolio_FK_UserId ON Portfolio(UserId);
GO;

CREATE TRIGGER PortfolioLastModifiedTrigger 
ON Portfolio
FOR UPDATE
AS 
BEGIN
      UPDATE Portfolio 
      SET DateModified = GETUTCDATE()
      FROM INSERTED 
      WHERE INSERTED.Id = Portfolio.Id
END;