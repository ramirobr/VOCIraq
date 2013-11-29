declare  @userName nvarchar(500)
declare @userId int

set @userName='america\ecuioososa'

select @userId=UserId from UserProfile where UserName=@userName --22

update webpages_UsersInRoles set RoleId=4  --role 4: coordinator
where UserId=@userId

--WorkflowStatus:
		--Created = 1,        
        --Requested = 2,        
        --Approved = 3,        
        --Rejected = 4,        
        --Ongoing = 5,        
        --Closed = 6,

--CertificateStatus
		--Conform = 1,        
        --NonConform = 2,        
        --Cancelled = 3,

--A certificate can be published  in workflowstatus: approved, ongoing, closed or on cancelled certificatestatus

delete Document
delete dbo.[Certificate]

set identity_insert dbo.[Certificate] on
insert into dbo.[Certificate] (CertificateId, Sequential, WorkflowStatusId, IsPublished, CertificateStatusId, IssuanceDate, IsInvoiced, EntryPointId, CreationBy, CreationDate, IsDeleted)
values (1,'CHGVA/VOCIQ00001', 1, 1, 1, getdate()-8, 0, 1, 'AMERICA\ecuiocchauca', getdate()-10, 0)

insert into dbo.[Certificate] (CertificateId, Sequential, WorkflowStatusId, IsPublished, CertificateStatusId, IssuanceDate, IsInvoiced, EntryPointId, CreationBy, CreationDate, IsDeleted)
values (2,'CHGVA/VOCIQ00002', 2, 1, 2, getdate()-8, 0, 2, 'AMERICA\ecuiocchauca', getdate()-10, 0)

insert into dbo.[Certificate] (CertificateId, Sequential, WorkflowStatusId, IsPublished, CertificateStatusId, IssuanceDate, IsInvoiced, EntryPointId, CreationBy, CreationDate, IsDeleted)
values (3,'CHGVA/VOCIQ00003', 3, 0, 3, getdate()-8, 0, 3, 'AMERICA\ecuiocchauca', getdate()-10, 0)

insert into dbo.[Certificate] (CertificateId, Sequential, WorkflowStatusId, IsPublished, CertificateStatusId, IssuanceDate, IsInvoiced, EntryPointId, CreationBy, CreationDate, IsDeleted)
values (4,'ECUIO/VOCIQ00004', 4, 1, 2, getdate()-8, 0, 4, 'AMERICA\ecuiocchauca', getdate()-10, 0)

insert into dbo.[Certificate] (CertificateId, Sequential, WorkflowStatusId, IsPublished, CertificateStatusId, IssuanceDate, IsInvoiced, EntryPointId, CreationBy, CreationDate, IsDeleted)
values (5,'USHOU/VOCIQ00005', 5, 0, 1, getdate()-8, 0, 5, 'AMERICA\ecuiocchauca', getdate()-10, 0)

insert into dbo.[Certificate] (CertificateId, Sequential, WorkflowStatusId, IsPublished, CertificateStatusId, IssuanceDate, IsInvoiced, EntryPointId, CreationBy, CreationDate, IsDeleted)
values (6,'CNSHA/VOCIQ00006', 6, 1, 2, getdate()-7, 0, 6, 'AMERICA\ecuiocchauca', getdate()-9, 0)

insert into dbo.[Certificate] (CertificateId, Sequential, WorkflowStatusId, IsPublished, CertificateStatusId, IssuanceDate, IsInvoiced, EntryPointId, CreationBy, CreationDate, IsDeleted)
values (7,'GBLON/VOCIQ00007', 1, 0, 3, getdate()-7, 0, 7, 'AMERICA\ecuiocchauca', getdate()-9, 0)

insert into dbo.[Certificate] (CertificateId, Sequential, WorkflowStatusId, IsPublished, CertificateStatusId, IssuanceDate, IsInvoiced, EntryPointId, CreationBy, CreationDate, IsDeleted)
values (8,'ECUIO/VOCIQ00008', 4, 0, 3, getdate()-7, 0, 8, 'AMERICA\ecuiocchauca', getdate()-9, 0)

insert into dbo.[Certificate] (CertificateId, Sequential, WorkflowStatusId, IsPublished, CertificateStatusId, IssuanceDate, IsInvoiced, EntryPointId, CreationBy, CreationDate, IsDeleted)
values (9,'ECUIO/VOCIQ00009', 1, 0, 1, getdate()-7, 0, 1, 'AMERICA\ecuiocchauca', getdate()-9, 0)

set identity_insert dbo.[Certificate] off

--Documents


INSERT INTO [dbo].[Document] ([CertificateId] ,[Filename],[Description],[FilePath],[IsSupporting],[CreationBy],[CreationDate],[IsDeleted])
VALUES (1,'Doc01.docx','Document for created', 'CHGVA/VOCIQ00001\',0, 'AMERICA\ecuiocchauca', getdate()-9, 0)

INSERT INTO [dbo].[Document] ([CertificateId] ,[Filename],[Description],[FilePath],[IsSupporting],[CreationBy],[CreationDate],[IsDeleted])
VALUES (4,'Doc02.docx','Document for rejected','ECUIO/VOCIQ00004\',0, 'AMERICA\ecuiocchauca', getdate()-9, 0)

INSERT INTO [dbo].[Document] ([CertificateId] ,[Filename],[Description],[FilePath],[IsSupporting],[CreationBy],[CreationDate],[IsDeleted])
VALUES (7,'Doc03.docx','Document for created','GBLON/VOCIQ00007\',0, 'AMERICA\ecuiocchauca', getdate()-9, 0)

INSERT INTO [dbo].[Document] ([CertificateId] ,[Filename],[Description],[FilePath],[IsSupporting],[CreationBy],[CreationDate],[IsDeleted])
VALUES (8,'Doc04.docx','Document for rejected','ECUIO/VOCIQ00008\',0, 'AMERICA\ecuiocchauca', getdate()-9, 0)



--result:
--certid 1  -> display request button because worflow status is created and working
--certid 4  -> display request button because worflow status is rejected and working
--certid 7  -> NOT display request button because certificate status is canceled although worflow status is created
--certid 8  -> NOT display request button because certificate status is canceled although worflow status is rejected
--certid 9  -> display request button because but not work because it does not have certificate document associated




