
/***************************************/
/****	Alter ReleaseNote table		****/
/***************************************/
SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FileRequest]') AND type in (N'U')

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FileRequest]') AND type in (N'U'))
BEGIN
	DROP TABLE [dbo].[FileRequest]
END
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileRequest](
	[FileRequestId] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](256) NULL,
	[DocumentId] [int] NULL,
	[IsRequested] [bit] NULL,
	[CreationBy] [nvarchar](256) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[ModificationBy] [nvarchar](256) NULL,
	[ModificationDate] [datetime] NULL,
 CONSTRAINT [PK_FileRequest] PRIMARY KEY CLUSTERED 
(
	[FileRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


GO

