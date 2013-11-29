USE IQVoc
GO

--Add DocumentType field in [dbo].[Document] table
IF ( NOT EXISTS (SELECT COLUMN_NAME
				FROM INFORMATION_SCHEMA.COLUMNS
				WHERE TABLE_NAME = 'Document' AND COLUMN_NAME = 'DocumentType'))
BEGIN

ALTER TABLE [dbo].[Document]
ADD DocumentType tinyint NULL

END

--Add ReleaseNoteId field in [dbo].[Document] table
IF ( NOT EXISTS (SELECT COLUMN_NAME
				FROM INFORMATION_SCHEMA.COLUMNS
				WHERE TABLE_NAME = 'Document' AND COLUMN_NAME = 'ReleaseNoteId'))
BEGIN

ALTER TABLE [dbo].[Document]
ADD ReleaseNoteId int NULL,
CONSTRAINT [FK_ReleaseNoteDocument] FOREIGN KEY ([ReleaseNoteId]) REFERENCES [dbo].[ReleaseNote] ([ReleaseNoteId])

END
go