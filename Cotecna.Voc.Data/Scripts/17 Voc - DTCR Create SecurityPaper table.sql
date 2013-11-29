USE [IQVoc]
GO
CREATE TABLE [dbo].[SecurityPaper]
(
	[SecurityPaperId] INT NOT NULL IDENTITY (1,1), 
	[EntryPointId] INT NOT NULL,
	[ReleaseNoteId] INT NULL,
    [SecurityPaperNumber] NVARCHAR(50) NOT NULL, 
    [Status] TINYINT NOT NULL, 
    [Comment] NVARCHAR(MAX) NULL, 
    [CreationBy] NVARCHAR(256) NOT NULL, 
    [CreationDate] DATETIME NOT NULL, 
	[IsDeleted] BIT NOT NULL, 
    [ModificationBy] NVARCHAR(256) NULL, 
    [ModificationDate] DATETIME NULL,
	CONSTRAINT [SecurityPaperId] PRIMARY KEY CLUSTERED ([SecurityPaperId] ASC), 
    CONSTRAINT [FK_SecurityPaperEntryPoint] FOREIGN KEY ([EntryPointId]) REFERENCES [dbo].[EntryPoint]([EntryPointId]), 
    CONSTRAINT [FK_SecurityPaperReleaseNote] FOREIGN KEY ([ReleaseNoteId]) REFERENCES [dbo].[ReleaseNote]([ReleaseNoteId])
)


