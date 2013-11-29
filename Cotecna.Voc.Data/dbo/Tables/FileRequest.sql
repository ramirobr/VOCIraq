CREATE TABLE [dbo].[FileRequest] (
    [FileRequestId]    INT            IDENTITY (1, 1) NOT NULL,
    [FullName]         NVARCHAR (256) NULL,
    [DocumentId]       INT            NULL,
    [IsRequested]      BIT            NULL,
    [CreationBy]       NVARCHAR (256) NOT NULL,
    [CreationDate]     DATETIME       NOT NULL,
    [IsDeleted]        BIT            NOT NULL,
    [ModificationBy]   NVARCHAR (256) NULL,
    [ModificationDate] DATETIME       NULL,
    CONSTRAINT [PK_FileRequest] PRIMARY KEY CLUSTERED ([FileRequestId] ASC)
);

