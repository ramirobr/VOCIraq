CREATE TABLE [dbo].[Office] (
    [OfficeId]         INT            IDENTITY (1, 1) NOT NULL,
    [OfficeName]       NVARCHAR (50)  NOT NULL,
    [OfficeCode]       NCHAR (3)      NOT NULL,
    [CountryCode]      NCHAR (3)      NOT NULL,
	[OfficeStamp]      VARBINARY(MAX) NULL,
    [CreationBy]       NVARCHAR (256) NOT NULL,
    [CreationDate]     DATETIME       NOT NULL,
    [IsDeleted]        BIT            DEFAULT ((0)) NOT NULL,
    [ModificationBy]   NVARCHAR (256) NULL,
    [ModificationDate] DATETIME       NULL,
    [OfficeType]       TINYINT        NULL, 
    [RegionalOfficeId] INT            NULL, 
    [ServerName]       NVARCHAR(256)  NULL, 
    [DatabaseName]     NVARCHAR(256)  NULL, 
    PRIMARY KEY CLUSTERED ([OfficeId] ASC),
	CONSTRAINT [RegionalOffice_Office] FOREIGN KEY ([RegionalOfficeId]) REFERENCES [dbo].[Office] ([OfficeId])
);


