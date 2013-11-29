CREATE TABLE [dbo].[UserProfile] (
    [UserId]        INT             IDENTITY (1, 1) NOT NULL,
    [UserName]      NVARCHAR (MAX)  NULL,
    [FirstName]     NVARCHAR (100)  NULL,
    [LastName]      NVARCHAR (100)  NULL,
    [IsActive]      BIT             NULL,
    [OfficeId]      INT             NULL,
	[EntryPointId]  INT             NULL,	
    [FilePath]      NVARCHAR (MAX)  NULL,
    [SignatureFile] VARBINARY (MAX) NULL,
    [IsInternalUser] BIT			NULL, 
    [IsDisclaimerAccepted] BIT NULL, 
    [TemporalPassword] NVARCHAR(MAX) NULL, 
    [Email] NVARCHAR(256) NULL, 
    CONSTRAINT [PK_UserProfile] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [FK_OfficeUserProfile] FOREIGN KEY ([OfficeId]) REFERENCES [dbo].[Office] ([OfficeId])
);



