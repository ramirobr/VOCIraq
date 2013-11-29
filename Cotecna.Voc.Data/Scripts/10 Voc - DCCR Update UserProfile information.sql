Use [IQVoc]
GO


IF EXISTS(SELECT 1 FROM [dbo].[UserProfile] WHERE  IsInternalUser is null)
BEGIN
	UPDATE [dbo].[UserProfile]
	SET IsInternalUser = 0
	where IsInternalUser is null

	print 'Users have been updated'
END