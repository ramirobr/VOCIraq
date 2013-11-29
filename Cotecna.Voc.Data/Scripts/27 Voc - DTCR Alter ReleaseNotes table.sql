USE IQVOC
GO
ALTER TABLE [dbo].[ReleaseNote]
DROP COLUMN [ContainerSection]
GO
ALTER TABLE [dbo].[ReleaseNote]
ADD ShipmentType TINYINT NULL
GO
ALTER TABLE [dbo].[ReleaseNote]
ADD Unit NVARCHAR(64) NULL
GO
ALTER TABLE [dbo].[ReleaseNote]
ADD ReceivedQuantity INT NULL
GO
ALTER TABLE [dbo].[ReleaseNote]
ADD RemainingQuantity INT NULL
GO
--ALTER TABLE [dbo].[ReleaseNote]
--ADD ShipmentDate DATETIME NULL
--GO