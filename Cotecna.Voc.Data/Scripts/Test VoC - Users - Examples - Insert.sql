/************* Users ****************/
declare @NewId as int, @RoleId as int

select @RoleId = RoleId from dbo.webpages_Roles where RoleName = 'SuperAdmin'

insert into dbo.UserProfile (UserName, FirstName, LastName, IsActive, OfficeId, IsInternalUser)
values ('AMERICA\ecuiocchauca', 'Carlos', 'Chauca', 1, 1, 1)
select @NewId = @@IDENTITY  
insert into dbo.webpages_UsersInRoles (UserId, RoleId) values (@NewId, @RoleId)

insert into dbo.UserProfile (UserName, FirstName, LastName, IsActive, OfficeId, IsInternalUser)
values ('AMERICA\ecuiocarauz', 'Carlos', 'Arauz', 1, 1, 1)
select @NewId = @@IDENTITY  
insert into dbo.webpages_UsersInRoles (UserId, RoleId) values (@NewId, @RoleId)

insert into dbo.UserProfile (UserName, FirstName, LastName, IsActive, OfficeId, IsInternalUser)
values ('AMERICA\ecuiopzambrano', 'Paul', 'Zambrano', 1, 1, 1)
select @NewId = @@IDENTITY  
insert into dbo.webpages_UsersInRoles (UserId, RoleId) values (@NewId, @RoleId)

insert into dbo.UserProfile (UserName, FirstName, LastName, IsActive, OfficeId, IsInternalUser)
values ('EUROPE\gvaferreira', 'Julie', 'Ferreira', 1, 1, 1)
select @NewId = @@IDENTITY  
insert into dbo.webpages_UsersInRoles (UserId, RoleId) values (@NewId, @RoleId)

insert into dbo.UserProfile (UserName, FirstName, LastName, IsActive, OfficeId, IsInternalUser)
values ('EUROPE\gvagirard', 'Christelle', 'Girard', 1, 1, 1)
select @NewId = @@IDENTITY  
insert into dbo.webpages_UsersInRoles (UserId, RoleId) values (@NewId, @RoleId)

