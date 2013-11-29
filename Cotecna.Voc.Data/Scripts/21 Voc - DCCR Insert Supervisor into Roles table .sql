Use IQVoc
GO

SET IDENTITY_INSERT dbo.webpages_Roles ON
IF(NOT EXISTS(SELECT * FROM dbo.webpages_Roles WHERE RoleId = 8))
	BEGIN
		INSERT INTO dbo.webpages_Roles (RoleId, RoleName)
		VALUES (8, 'Supervisor')
	END
SET IDENTITY_INSERT dbo.webpages_Roles OFF