USE IQVOC
GO
CREATE TABLE [dbo].[CertificateTracking]
(
	[CertificateTranckingId] INT NOT NULL IDENTITY(1,1), 
	[CertificateId] INT NOT NULL,
    [TrackingDate] DATETIME NOT NULL, 
    [TrackingBy] NVARCHAR(256) NOT NULL, 
    [TrackingStatus] TINYINT NOT NULL, 
    [CreationBy] NVARCHAR(256) NOT NULL, 
    [CreationDate] DATETIME NOT NULL, 
    [IsDeleted] BIT NOT NULL, 
    [ModficationBy] NVARCHAR(256) NULL, 
    [ModificationDate] DATETIME NULL, 
    CONSTRAINT [PK_CertificateTracking] PRIMARY KEY ([CertificateTranckingId]), 
	CONSTRAINT [FK_CertificateTracking_Certificate] FOREIGN KEY ([CertificateId]) REFERENCES [Certificate]([CertificateId])
)