USE [IQVOC] 
GO
-- RELEASE NOTE TABLE
ALTER TABLE [dbo].[ReleaseNote]
ALTER COLUMN [ReceivedQuantity] DECIMAL(18,2) NULL
GO
ALTER TABLE [dbo].[ReleaseNote]
ALTER COLUMN [RemainingQuantity] DECIMAL(18,2) NULL
GO
ALTER TABLE [dbo].[ReleaseNote]
ADD [PaidFees] DECIMAL(14,2)