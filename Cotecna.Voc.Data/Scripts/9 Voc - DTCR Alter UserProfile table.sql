USE [IQVoc]
GO

--Add IsDisclaimerAccepted field in [dbo].[UserProfile] table
IF ( NOT EXISTS (SELECT COLUMN_NAME
				FROM INFORMATION_SCHEMA.COLUMNS
				WHERE TABLE_NAME = 'UserProfile' AND COLUMN_NAME = 'IsDisclaimerAccepted'))
BEGIN

ALTER TABLE [dbo].[UserProfile]
ADD IsDisclaimerAccepted BIT NULL

END

--Add TemporalPassword field in [dbo].[UserProfile] table
IF ( NOT EXISTS (SELECT COLUMN_NAME
				FROM INFORMATION_SCHEMA.COLUMNS
				WHERE TABLE_NAME = 'UserProfile' AND COLUMN_NAME = 'TemporalPassword'))
BEGIN

ALTER TABLE [dbo].[UserProfile]
ADD TemporalPassword NVARCHAR(MAX) NULL

END
