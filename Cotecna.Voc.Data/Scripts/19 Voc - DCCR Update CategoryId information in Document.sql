Use IQVoc
GO


IF EXISTS(SELECT 1 FROM [dbo].[Document] WHERE  DocumentType is null)
BEGIN
	UPDATE [dbo].[Document]
	SET DocumentType = 0
	where IsSupporting = 0 and DocumentType is null

	UPDATE [dbo].[Document]
	SET DocumentType = 1
	where IsSupporting = 1 and DocumentType is null

	print 'Documents have been updated'
END

