USE IQVOC
go

IF EXISTS(select 1 from Office where OfficeType is null)
begin
	update Office
	set OfficeType = 1
	where OfficeType is null
end
