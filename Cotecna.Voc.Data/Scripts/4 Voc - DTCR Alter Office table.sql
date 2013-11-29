
/***************************************/
/****		Alter Office table		****/
/***************************************/
 IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Office' AND COLUMN_NAME = 'OfficeStamp')
	ALTER TABLE	dbo.Office
	ADD		OfficeStamp varbinary(MAX)
