CREATE TABLE [dbo].[Document] (
    [DocumentId]    INT            IDENTITY (1, 1) NOT NULL,
    [CertificateId] INT            NOT NULL,
    [Filename]      NVARCHAR (256) NOT NULL,
    [Description]   NVARCHAR (500) NULL,
    [FilePath]      NVARCHAR (512) NOT NULL,
    [IsSupporting]  BIT            NOT NULL,
    [CreationBy]    NVARCHAR (256) NOT NULL,
    [CreationDate]  DATETIME       NOT NULL,
    [IsDeleted]     BIT            NOT NULL,
    [ModificationBy] NVARCHAR(256) NULL, 
    [ModificationDate] DATETIME NULL, 
    [DocumentType] TINYINT NULL, 
    [ReleaseNoteId] INT NULL, 
    CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED ([DocumentId] ASC),
    CONSTRAINT [FK_CertificateDocument] FOREIGN KEY ([CertificateId]) REFERENCES [dbo].[Certificate] ([CertificateId]),
	CONSTRAINT [FK_ReleaseNoteDocument] FOREIGN KEY ([ReleaseNoteId]) REFERENCES [dbo].[ReleaseNote] ([ReleaseNoteId])
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_CertificateDocument]
    ON [dbo].[Document]([CertificateId] ASC);

