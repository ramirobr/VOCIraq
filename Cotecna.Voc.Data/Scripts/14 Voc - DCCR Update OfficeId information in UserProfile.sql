USE  [IQVoc]
GO

IF EXISTS (SELECT 1 from [dbo].[UserProfile] WHERE OfficeId is null and IsInternalUser = 1)
BEGIN
	UPDATE [dbo].[UserProfile]
	SET OfficeId = 1
	WHERE OfficeId is null and IsInternalUser = 1
END