USE IQVOC
GO
IF NOT EXISTS(SELECT 1 FROM [dbo].[EntryPoint] WHERE Name = 'Khour Abdullah')
BEGIN
	INSERT INTO [dbo].[EntryPoint] ([Name],[CreationBy],[CreationDate],[IsDeleted],[IsLo])
	VALUES ('Khour Abdullah','AMERICA\ecuiorbatallas',GETDATE(),0,0)
END

