CREATE TABLE [dbo].[EntryPoint] (
    [EntryPointId]     INT            IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (256) NOT NULL,
    [CreationBy]       NVARCHAR (256) NOT NULL,
    [CreationDate]     DATETIME       NOT NULL,
    [IsDeleted]        BIT            NOT NULL,
    [ModificationBy]   NVARCHAR (256) NULL,
    [ModificationDate] DATETIME       NULL,
    [IsLo]             BIT            NULL,
    CONSTRAINT [PK_EntryPoint] PRIMARY KEY CLUSTERED ([EntryPointId] ASC)
);



