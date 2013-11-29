CREATE TABLE [dbo].[ReleaseNote] (
	[ReleaseNoteId] INT            IDENTITY (1, 1) NOT NULL,
    [CertificateId] INT            NOT NULL,
	[PartialNumber] [int] NULL,
	[Goods] [nvarchar](512) NULL,
	[NumberOfContainers] [int] NULL,
	[Containers] [nvarchar](512) NULL,
	[NetWeight] DECIMAL(5, 2) NULL,
	[DocumentaryCheckResultId] [tinyint] NULL,
	[PhysicalCheckResultId] [tinyint] NULL,
	[VisualInspectionMade] [bit] NULL,
	[OverallResultId] [tinyint] NULL,
	[NoteIssuedId] [tinyint] NULL,
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
