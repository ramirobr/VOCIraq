Use IQVOC
go

if exists(select 1 from EntryPoint where Name = 'Al Qaem' and IsDeleted = 0)
begin 
	update EntryPoint
	set IsDeleted = 1
	where Name = 'Al Qaem'
end

if exists(select 1 from EntryPoint where Name = 'Arar' and IsDeleted = 0)
begin 
	update EntryPoint
	set IsDeleted = 1
	where Name = 'Arar'
end

if exists(select 1 from EntryPoint where Name = 'Rabea')
begin 
	update EntryPoint
	set Name = 'Rabeaa'
	where Name = 'Rabea'
end

if exists(select 1 from EntryPoint where Name = 'Trabil')
begin 
	update EntryPoint
	set Name = 'Trebil'
	where Name = 'Trabil'
end