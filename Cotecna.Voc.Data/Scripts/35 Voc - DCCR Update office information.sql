USE [IQVOC]
GO

UPDATE [dbo].[Office] SET ServerName='GVASQLCLU1\MSSQLSERVER_1',DatabaseName ='COMDIV' WHERE OfficeName = 'Cotecna Geneva'
UPDATE [dbo].[Office] SET ServerName='LONSQL2',DatabaseName ='COMDIV_UK' WHERE OfficeName = 'Cotecna London'
UPDATE [dbo].[Office] SET ServerName='SHASQL1',DatabaseName ='COMDIV' WHERE OfficeName = 'Cotecna Sino-Swiss'
UPDATE [dbo].[Office] SET ServerName='HOUSQLCLU1\MSSQLSERVER_1',DatabaseName ='COMDIV' WHERE OfficeName = 'Cotecna Houston'
UPDATE [dbo].[Office] SET ServerName='NTESQL1',DatabaseName ='COMDIV' WHERE OfficeName = 'Cotecna France'
UPDATE [dbo].[Office] SET ServerName='SINSQL1',DatabaseName ='COMDIV_JP' WHERE OfficeName = 'Cotecna Tokio'
UPDATE [dbo].[Office] SET ServerName='BSASQL1',DatabaseName ='COMDIV' WHERE OfficeName = 'Cotecna Buenos Aires'
UPDATE [dbo].[Office] SET ServerName='CGHSQL1',DatabaseName ='COMDIV' WHERE OfficeName = 'Cotecna Sao Paolo'
UPDATE [dbo].[Office] SET ServerName='BOGSQL2,45777',DatabaseName ='COMDIV_CO' WHERE OfficeName = 'Cotecna Bogota'
UPDATE [dbo].[Office] SET ServerName='SHASQL1',DatabaseName ='COMDIV_HK' WHERE OfficeName = 'Cotecna Hong Kong'
UPDATE [dbo].[Office] SET ServerName='DXBSQL',DatabaseName ='COMDIV' WHERE OfficeName = 'Cotecna Dubai'
UPDATE [dbo].[Office] SET ServerName='GYESQL1,45777',DatabaseName ='COMDIV' WHERE OfficeName = 'Cotecna Quito'
UPDATE [dbo].[Office] SET ServerName='GVASQLCLU1\MSSQLSERVER_1',DatabaseName ='COMDIV_EG' WHERE OfficeName = 'Cotecna Alexandria'
UPDATE [dbo].[Office] SET ServerName='SINSQL1',DatabaseName ='COMDIV_IN' WHERE OfficeName = 'Cotecna Mumbai'
UPDATE [dbo].[Office] SET ServerName='GVASQLCLU1\MSSQLSERVER_1',DatabaseName ='COMDIV_IT' WHERE OfficeName = 'Cotecna Milano'
UPDATE [dbo].[Office] SET ServerName='GVASQLCLU1\MSSQLSERVER_1',DatabaseName ='COMDIV_KZ' WHERE OfficeName = 'Cotecna Astana'
UPDATE [dbo].[Office] SET ServerName='GVASQLCLU1\MSSQLSERVER_1',DatabaseName ='COMDIV_RU' WHERE OfficeName = 'Cotecna Moscow'
UPDATE [dbo].[Office] SET ServerName='DXBSQL',DatabaseName ='COMDIV_SA' WHERE OfficeName = 'Cotecna Damman'
UPDATE [dbo].[Office] SET ServerName='SINSQL1',DatabaseName ='COMDIV_SG' WHERE OfficeName = 'Cotecna Singapore'
UPDATE [dbo].[Office] SET ServerName='SINSQL1',DatabaseName ='COMDIV_KR' WHERE OfficeName = 'Cotecna Seoul'
UPDATE [dbo].[Office] SET ServerName='GVASQLCLU1\MSSQLSERVER_1',DatabaseName ='COMDIV_ES' WHERE OfficeName = 'Cotecna Barcelona'
UPDATE [dbo].[Office] SET ServerName='SINSQL1',DatabaseName ='COMDIV_TH' WHERE OfficeName = 'Cotecna Bangkok'
UPDATE [dbo].[Office] SET ServerName='ISTEXCHANGE',DatabaseName ='COMDIV' WHERE OfficeName = 'Cotecna Istanbul'
UPDATE [dbo].[Office] SET ServerName='GVASQLCLU1\MSSQLSERVER_1',DatabaseName ='COMDIV_UA' WHERE OfficeName = 'Cotecna Odesa'
UPDATE [dbo].[Office] SET ServerName='SINSQL1',DatabaseName ='COMDIV_VN' WHERE OfficeName = 'Cotecna Ho Chi Minh'

INSERT INTO [dbo].[Office] 
(
	[OfficeName],
	[OfficeCode],
	[CountryCode],
	[CreationBy],
	[CreationDate],
	[IsDeleted],
	[OfficeType],
	[ServerName],
	[DatabaseName]
)
VALUES (
'Cotecna Baghdad','BGW','IQ','AMERICA\ecuiocchauca',GETDATE(),0,1,'GVASQLCLU1\MSSQLSERVER_1','COMDIV_IQ'
)
