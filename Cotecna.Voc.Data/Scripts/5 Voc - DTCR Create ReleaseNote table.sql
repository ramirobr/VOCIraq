
/***************************************/
/****	Alter ReleaseNote table		****/
/***************************************/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReleaseNote]') AND type in (N'U'))
BEGIN
	ALTER TABLE [dbo].[ReleaseNote] DROP CONSTRAINT [FK_CertificateReleaseNote]
	DROP TABLE [dbo].[ReleaseNote]
END
GO

CREATE TABLE [dbo].[ReleaseNote] (
	[ReleaseNoteId] INT     IDENTITY (1, 1) NOT NULL,
    [CertificateId] INT     NOT NULL,
	[PartialNumber] [int] NULL,
	[Goods] [nvarchar](512) NULL,
	[NumberOfContainers] [int] NULL,
	[Containers] [nvarchar](512) NULL,
	[NetWeight] [nvarchar](512) NULL,
	[DocumentaryCheckResultId] [tinyint] NULL,
	[PhysicalCheckResultId] [tinyint] NULL,
	[VisualInspectionMade] [bit] NULL,
	[OverallResultId] [tinyint] NULL,
	[NoteIssuedId] [tinyint] NULL,
	[SecurityPaperNumber] [nvarchar](2048) NULL,
	[IssuanceDate] [datetime] NULL,
	[JointSamplingMade] [bit] NULL,
	[CreationBy] [nvarchar](256) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[ModificationBy] [nvarchar](256) NULL,
	[ModificationDate] [datetime] NULL,
    CONSTRAINT [PK_ReleaseNote] PRIMARY KEY CLUSTERED ([ReleaseNoteId] ASC),
    CONSTRAINT [FK_CertificateReleaseNote] FOREIGN KEY ([CertificateId]) REFERENCES [dbo].[Certificate] ([CertificateId])
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_CertificateReleaseNote]
    ON [dbo].[ReleaseNote]([CertificateId] ASC);


GO

