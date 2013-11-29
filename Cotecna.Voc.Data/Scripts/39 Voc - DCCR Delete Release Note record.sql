Use IQVOC
go

declare @certificateId int,
		@ReleaseNoteId int

Select @certificateId=CertificateId from Certificate
where Sequential = 'IRQ-CO13-00059' and IsDeleted = 0

select @ReleaseNoteId= ReleaseNoteId from ReleaseNote
where CertificateId = @certificateId and PartialNumber = 43 and IsDeleted = 0

update SecurityPaper
set ReleaseNoteId = NULL,
[Status] = 1,
ModificationBy = NULL,
ModificationDate = NUll
where ReleaseNoteId = @ReleaseNoteId and IsDeleted = 0

delete from Document
where ReleaseNoteId = @ReleaseNoteId

delete from ReleaseNote
where ReleaseNoteId = @ReleaseNoteId