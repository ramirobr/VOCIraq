CREATE TABLE [dbo].[Certificate] (
    [CertificateId]       INT            IDENTITY (1, 1) NOT NULL,
    [Sequential]          NVARCHAR (20)  NOT NULL,
    [WorkflowStatusId]    TINYINT        NULL,
    [IsPublished]         BIT            NOT NULL,
    [CertificateStatusId] TINYINT        NOT NULL,
    [IssuanceDate]        DATETIME       NULL,
    [IsInvoiced]          BIT            NULL,
    [CreationBy]          NVARCHAR (256) NOT NULL,
    [CreationDate]        DATETIME       NOT NULL,
    [IsDeleted]           BIT            NOT NULL,
    [EntryPointId]        INT            NULL,
    [OfficeId]            INT            NULL,
    [WorkflowStatusDate]  DATETIME       NULL,
    [ModificationBy]      NVARCHAR (256) NULL,
    [ModificationDate]    DATETIME       NULL,
    [ApprovedBy]          NVARCHAR (256) NULL,
	[ComdivNumber]        NVARCHAR (256) NULL,
    [FOBValue]			  DECIMAL(14, 2) NULL, 
    [IsSynchronized]      BIT NULL, 
    CONSTRAINT [PK_Certificate] PRIMARY KEY CLUSTERED ([CertificateId] ASC),
    CONSTRAINT [FK_EntryPointCertificate] FOREIGN KEY ([EntryPointId]) REFERENCES [dbo].[EntryPoint] ([EntryPointId]),
    CONSTRAINT [FK_OfficeCertificate] FOREIGN KEY ([OfficeId]) REFERENCES [dbo].[Office] ([OfficeId])
);




GO
CREATE NONCLUSTERED INDEX [IX_FK_EntryPointCertificate]
    ON [dbo].[Certificate]([EntryPointId] ASC);


GO



GO




GO



GO


