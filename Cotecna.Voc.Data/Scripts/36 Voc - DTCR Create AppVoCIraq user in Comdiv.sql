USE [master]
GO
IF  EXISTS (SELECT * FROM sys.server_principals WHERE name = N'[AppVocIraq]')
 DROP LOGIN [AppVocIraq]
GO
USE [master]
GO
CREATE LOGIN [AppVocIraq] WITH PASSWORD=N'dXN1YXJpb2Z1ZXJ0ZWNvdGVjbmEyMDEz', CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO