USE [IQVOC]
GO

DECLARE @UserId int

select @UserId = UserId from [dbo].[UserProfile]
where UserName = 'products@shamrockoils.com'

delete from [dbo].[webpages_Membership]
where UserId = @UserId

delete from [dbo].[webpages_UsersInRoles]
where UserId = @UserId

delete from [dbo].[UserProfile]
where UserId = @UserId