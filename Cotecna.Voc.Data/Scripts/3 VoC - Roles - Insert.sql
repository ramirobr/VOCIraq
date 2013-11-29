
/************* Roles ****************/


 SET IDENTITY_INSERT dbo.webpages_Roles ON

	IF(NOT EXISTS(SELECT * FROM dbo.webpages_Roles WHERE RoleId = 1))
	BEGIN
		INSERT INTO dbo.webpages_Roles (RoleId, RoleName)
		VALUES (1, 'Admin')
	END

	IF(NOT EXISTS(SELECT * FROM dbo.webpages_Roles WHERE RoleId = 2))
	BEGIN
		INSERT INTO dbo.webpages_Roles (RoleId, RoleName)
		VALUES (2, 'SuperAdmin')
	END

	IF(NOT EXISTS(SELECT * FROM dbo.webpages_Roles WHERE RoleId = 3))
	BEGIN
		INSERT INTO dbo.webpages_Roles (RoleId, RoleName)
		VALUES (3, 'Issuer')
	END

	IF(NOT EXISTS(SELECT * FROM dbo.webpages_Roles WHERE RoleId = 4))
	BEGIN
		INSERT INTO dbo.webpages_Roles (RoleId, RoleName)
		VALUES (4, 'Coordinator')
	END

	IF(NOT EXISTS(SELECT * FROM dbo.webpages_Roles WHERE RoleId = 5))
	BEGIN
		INSERT INTO dbo.webpages_Roles (RoleId, RoleName)
		VALUES (5, 'Client')
	END

	IF(NOT EXISTS(SELECT * FROM dbo.webpages_Roles WHERE RoleId = 6))
	BEGIN
		INSERT INTO dbo.webpages_Roles (RoleId, RoleName)
		VALUES (6, 'BorderAgent')
	END

	IF(NOT EXISTS(SELECT * FROM dbo.webpages_Roles WHERE RoleId = 7))
	BEGIN
		INSERT INTO dbo.webpages_Roles (RoleId, RoleName)
		VALUES (7, 'LOAdmin')
	END

 SET IDENTITY_INSERT dbo.webpages_Roles OFF

