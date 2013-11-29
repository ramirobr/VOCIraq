USE IQVOC
go
Alter table [dbo].[Office]
Add OfficeType tinyint null
go
Alter table [dbo].[Office]
add RegionalOfficeId int null
go
Alter table [dbo].[Office]
add constraint RegionalOffice_Office
foreign key ([RegionalOfficeId])
references [dbo].[Office] ([OfficeId]) 