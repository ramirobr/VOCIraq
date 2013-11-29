
/***************************************/
/****		Alter EntryPoint table		****/
/***************************************/
 IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EntryPoint' AND COLUMN_NAME = 'IsLo')
	ALTER TABLE	dbo.EntryPoint
	ADD		IsLo bit
