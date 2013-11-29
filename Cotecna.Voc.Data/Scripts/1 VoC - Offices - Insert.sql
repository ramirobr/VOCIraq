/************* Offices ****************/

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='GVA')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Geneva', 'GVA', 'CH', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='LON')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna London', 'LON', 'GB', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='SHA')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Sino-Swiss', 'SHA', 'CN', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='HOU')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Houston', 'HOU', 'US', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='NAN')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna France', 'NAN', 'FR', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='TYO')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Tokio', 'TYO', 'JP', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='MNL')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Manila', 'MNL', 'PH', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='OPO')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Porto', 'OPO', 'PT', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='BUE')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Buenos Aires', 'BUE', 'AR', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='CGH')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Sao Paolo', 'CGH', 'BR', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='BOG')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Bogota', 'BOG', 'CO', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='HKK')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Hong Kong', 'HKK', 'HK', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='DXB')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Dubai', 'DXB', 'AE', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='UIO')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Quito', 'UIO', 'EC', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='ALY')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Alexandria', 'ALY', 'EG', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='BOM')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Mumbai', 'BOM', 'IN', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='MIL')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Milano', 'MIL', 'IT', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='TSE')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Astana', 'TSE', 'KZ', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='MOW')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Moscow', 'MOW', 'RU', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='DMM')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Damman', 'DMM', 'SA', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='SIN')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Singapore', 'SIN', 'SG', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='SEL')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Seoul', 'SEL', 'KR', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='BCN')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Barcelona', 'BCN', 'ES', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='BKK')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Bangkok', 'BKK', 'TH', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='IST')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Istanbul', 'IST', 'TR', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='ODS')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Odesa', 'ODS', 'UA', 'AMERICA\ecuiocchauca', getdate(), 0)

If NOT EXISTS (SELECT * FROM dbo.Office Where OfficeCode='SGN')
insert into dbo.Office (OfficeName, OfficeCode, CountryCode, CreationBy, CreationDate, IsDeleted)
values ('Cotecna Ho Chi Minh', 'SGN', 'VN', 'AMERICA\ecuiocchauca', getdate(), 0)


