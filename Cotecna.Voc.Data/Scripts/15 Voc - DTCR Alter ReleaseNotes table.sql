
/***************************************/
/****		Alter ReleaseNote table		****/
/***************************************/
 IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ReleaseNote' AND COLUMN_NAME = 'ImporterName')
	ALTER TABLE dbo.ReleaseNote ADD
		ImporterName nvarchar(512) NULL,
		VisuallyCheck bit NULL,
		ContainersDetails nvarchar(512) NULL,
		ImportDocumentDetails nvarchar(512) NULL,
		PartialComplete bit NULL,
		NumberLineItems nvarchar(512) NULL,
		ContainerSection nvarchar(512) NULL,
		Comments nvarchar(MAX) NULL
