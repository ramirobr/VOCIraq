
/***************************************/
/****	Alter ReleaseNote table		****/
/***************************************/

ALTER TABLE [dbo].[ReleaseNote]
ALTER COLUMN [PartialNumber] [int] NULL

ALTER TABLE [dbo].[ReleaseNote]
ALTER COLUMN [Goods] [nvarchar](512) NULL

ALTER TABLE [dbo].[ReleaseNote]
ADD [Containers] [nvarchar](512) NULL

ALTER TABLE [dbo].[ReleaseNote]
ALTER COLUMN [NetWeight] [nvarchar](512) NULL

ALTER TABLE [dbo].[ReleaseNote]
ALTER COLUMN [SecurityPaperNumber] [nvarchar](2048) NULL

ALTER TABLE [dbo].[ReleaseNote]
ALTER COLUMN [NetWeight] [nvarchar](512) NULL

ALTER TABLE [dbo].[ReleaseNote]
ALTER COLUMN [IssuanceDate] [datetime] NULL
