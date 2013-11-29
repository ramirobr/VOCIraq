USE [IQVoc]
GO

--Add Email field in [dbo].[UserProfile] table
IF ( NOT EXISTS (SELECT COLUMN_NAME
				FROM INFORMATION_SCHEMA.COLUMNS
				WHERE TABLE_NAME = 'UserProfile' AND COLUMN_NAME = 'Email'))
BEGIN

ALTER TABLE [dbo].[UserProfile]
ADD Email NVARCHAR(256) NULL

END
