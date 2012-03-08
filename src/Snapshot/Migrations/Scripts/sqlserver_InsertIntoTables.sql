-- Roles
GO
if not exists(select [Name] from Roles where [Name]=N'AllAccess')
begin
INSERT  INTO Roles
           (Id
           ,Name
           ,Description)
     VALUES
           ('461e581b-e60b-4dfd-a5a8-88229f14379b'
           ,N'AllAccess'
           ,N'This role permits access to the entire application')
end	

-- Users
GO

if not exists(select * from Users where [UserName]=N'admin')
begin
INSERT INTO Users
           ([Id]
           ,[UserName]
           ,[ClientId]
		   ,[RoleId]
           ,[Password]
           ,[Email]
           ,[Created]
           ,[Updated]
           ,[ByUser_FK])
     VALUES
           ('E8346290-DE35-47FB-8FEC-D2562DED7F40'
           ,'admin'
           ,'BEEC53CE-A73C-4F03-A354-C617F68BC813'
		   ,'461e581b-e60b-4dfd-a5a8-88229f14379b'
           ,'1VEeTz7YcRY='
           ,'admin@evozon.com'
           ,GETDATE()
           ,GETDATE()
           ,null)
end
	GO	   
-- Clients
GO
if not exists(select [Name] from Clients where [Name]=N'Minoxsys')
begin
INSERT INTO [Clients]
           ([Id]
           ,[Name]
           ,[Created]
           ,[Updated]
           ,[ByUser_FK])
     VALUES
           ( 'BEEC53CE-A73C-4F03-A354-C617F68BC813'
           ,'Minoxsys'
           ,'2011-12-14 14:03:43.000'
           ,'2011-12-14 14:03:43.000'
           ,null)
end
GO
 -- Permissions
if not exists(select [Name] from Permissions where [Name]=N'UserManager.Overview')
begin            
 INSERT  INTO Permissions
           (Id
           ,Name)
VALUES
           ('95B06FCC-CCAC-4634-9908-AED2C6569BD5'
           ,N'UserManager.Overview')
end
 GO
if not exists(select [Name] from Permissions where [Name]=N'UserManager.CRUD')
begin          
 INSERT  INTO Permissions
           (Id
           ,Name)
VALUES
           ('F13223AD-7731-409B-BA96-7B0D01998085'
           ,N'UserManager.CRUD')
end 
GO
if not exists(select [Name] from Permissions where [Name]=N'RoleManager.Overview')
begin          
 INSERT  INTO Permissions
           (Id
           ,Name)
VALUES
           ('1A240DF2-9C2B-43A6-999D-DA2055A4533E'
           ,N'RoleManager.Overview')
end
GO 
if not exists(select [Name] from Permissions where [Name]=N'RoleManager.CRUD')
begin          
 INSERT  INTO Permissions
           (Id
           ,Name)
VALUES
           ('E37B5A01-B425-40B0-B613-34245393AD0D'
           ,N'RoleManager.CRUD')
end           
GO
if not exists(select [Name] from Permissions where [Name]=N'Home.Index')
begin           
 INSERT  INTO Permissions
           (Id
           ,Name)
VALUES
           ('80CDE125-6F44-477D-AAB7-171803030477'
           ,N'Home.Index')
end           
	   
		
-- PermissionRoles
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='80CDE125-6F44-477D-AAB7-171803030477' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('80CDE125-6F44-477D-AAB7-171803030477'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='369d6648-4e0f-453e-a2d6-5a54cc3b8aea' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('369d6648-4e0f-453e-a2d6-5a54cc3b8aea'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='2842ad00-170e-493e-b7ea-6c5f10d61d45' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('2842ad00-170e-493e-b7ea-6c5f10d61d45'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='744ba2ac-0a76-4c6e-8aa7-5d5cf8844ff1' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('744ba2ac-0a76-4c6e-8aa7-5d5cf8844ff1'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='5ce78ae3-32c8-4d93-8b93-6474ec13a1f2' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('5ce78ae3-32c8-4d93-8b93-6474ec13a1f2'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='45637f15-3bd3-4262-9abc-05cca6dc2e29' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('45637f15-3bd3-4262-9abc-05cca6dc2e29'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='6c199451-5501-4a04-839e-231d9a63e41d' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('6c199451-5501-4a04-839e-231d9a63e41d'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='810f37a9-424f-4362-9e06-7fbf41eff867' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('810f37a9-424f-4362-9e06-7fbf41eff867'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='a9890a76-cdb3-40fb-a7f5-507da335b186' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('a9890a76-cdb3-40fb-a7f5-507da335b186'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='985a8cab-14be-4262-a406-5a140a00203e' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('985a8cab-14be-4262-a406-5a140a00203e'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='e8501372-98cc-4fa8-8322-cc8629ed6dbc' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('e8501372-98cc-4fa8-8322-cc8629ed6dbc'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='0cad4331-fcac-43a3-91d0-80e3e3e8fa4d' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('0cad4331-fcac-43a3-91d0-80e3e3e8fa4d'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='0b1ca0af-7de5-4b3a-a937-6a9a21e4f2fd' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('0b1ca0af-7de5-4b3a-a937-6a9a21e4f2fd'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='51BC7941-7F69-4193-93B7-ACC35028890E' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('51BC7941-7F69-4193-93B7-ACC35028890E'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='397C3DC6-96A6-4DF7-ABA7-99260183184B' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('397C3DC6-96A6-4DF7-ABA7-99260183184B'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='A2B2EF2C-C6F4-4A34-894C-57B8B486AD54' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('A2B2EF2C-C6F4-4A34-894C-57B8B486AD54'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='6D389F28-5CDD-46D2-81F1-FED14D79A6A2' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('6D389F28-5CDD-46D2-81F1-FED14D79A6A2'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='E70E292A-6C11-45BF-A616-BEDA3AC0C26A' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('E70E292A-6C11-45BF-A616-BEDA3AC0C26A'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='07C27006-31EE-4FBF-BEF0-D8B776893B38' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('07C27006-31EE-4FBF-BEF0-D8B776893B38'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='9F34D436-E70C-4A9E-AB91-FB10AD1FA280' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('9F34D436-E70C-4A9E-AB91-FB10AD1FA280'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='C3949FDA-155D-4D35-B94E-FD2F270B423E' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('C3949FDA-155D-4D35-B94E-FD2F270B423E'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='A8F24F56-A9A2-4C1A-AADC-0BA266478C8C' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('A8F24F56-A9A2-4C1A-AADC-0BA266478C8C'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='082B2120-26F2-4A15-83DB-D01F351CCE11' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('082B2120-26F2-4A15-83DB-D01F351CCE11'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='10D23EA4-3A4F-4425-A6F1-4E561273ADBD' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('10D23EA4-3A4F-4425-A6F1-4E561273ADBD'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='98F72618-621B-477F-8C82-ED126D7EECEB' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('98F72618-621B-477F-8C82-ED126D7EECEB'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='2AFF765B-A1BA-4BBC-83B0-B9799A0E11BF' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('2AFF765B-A1BA-4BBC-83B0-B9799A0E11BF'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='470D3DA9-B579-4B82-A08C-8A4A399AE993' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('470D3DA9-B579-4B82-A08C-8A4A399AE993'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='1D987D4C-2183-47F0-A436-45273E015574' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('1D987D4C-2183-47F0-A436-45273E015574'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='7C379661-ACEE-4920-81BD-EFDDCD8720E2' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('7C379661-ACEE-4920-81BD-EFDDCD8720E2'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='C4D9A222-EBE0-43E3-B08E-EBF4F739410B' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('C4D9A222-EBE0-43E3-B08E-EBF4F739410B'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='2F86480E-3061-4753-AD70-6E58F380D3CD' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('2F86480E-3061-4753-AD70-6E58F380D3CD'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='3EAABA64-68C5-4F5E-854E-5319398DC72C' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('3EAABA64-68C5-4F5E-854E-5319398DC72C'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='EDEE5446-BEF6-4B19-A5B6-AFECBC8032ED' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('EDEE5446-BEF6-4B19-A5B6-AFECBC8032ED'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='802B1CE5-2A84-438F-8ACA-43EBE666705A' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('802B1CE5-2A84-438F-8ACA-43EBE666705A'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='64FD80C3-88BB-48A9-9B36-A86810F1D61A' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('64FD80C3-88BB-48A9-9B36-A86810F1D61A'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='ECC5B5A5-FF52-4C3A-8390-4C994AF980A8' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('ECC5B5A5-FF52-4C3A-8390-4C994AF980A8'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='796AE3E5-8FD9-4204-BA74-4D2A6B9152B4' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('796AE3E5-8FD9-4204-BA74-4D2A6B9152B4'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='F0A91CAF-82EC-4B33-ACD3-6F12A525A573' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('F0A91CAF-82EC-4B33-ACD3-6F12A525A573'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='5FFCEADE-F3FD-453B-8553-58E7D566ACF1' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('5FFCEADE-F3FD-453B-8553-58E7D566ACF1'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='222F64C2-B22C-4DE7-8934-E0B84806A1B5' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('222F64C2-B22C-4DE7-8934-E0B84806A1B5'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='7DAC2C99-05CD-49D2-9CC9-400C547D5F1B' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('7DAC2C99-05CD-49D2-9CC9-400C547D5F1B'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='189C44FD-8F02-429C-B4D7-32BFB6B8F724' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('189C44FD-8F02-429C-B4D7-32BFB6B8F724'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='903EC6AD-51C5-4128-AC42-ED2CD01D4A1B' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('903EC6AD-51C5-4128-AC42-ED2CD01D4A1B'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='7E508FF1-D448-4151-9C8B-906AF73505F5' and [Role_FK] ='461e581b-e60b-4dfd-a5a8-88229f14379b')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('7E508FF1-D448-4151-9C8B-906AF73505F5'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')
end
GO


if not exists(select [Name] from Permissions where [Name]=N'Country.View')
begin
INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'369d6648-4e0f-453e-a2d6-5a54cc3b8aea'
			,'Country.View'
			,GETDATE()
           )
   end
GO
if not exists(select [Name] from Permissions where [Name]=N'Country.Edit')
begin
INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'2842ad00-170e-493e-b7ea-6c5f10d61d45'
			,'Country.Edit'
			,GETDATE()
           )
end
GO
if not exists(select [Name] from Permissions where [Name]=N'Country.Delete')
begin
INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'744ba2ac-0a76-4c6e-8aa7-5d5cf8844ff1'
			,'Country.Delete'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Region.View')
begin

INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'5ce78ae3-32c8-4d93-8b93-6474ec13a1f2'
			,'Region.View'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Region.Edit')
begin
INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'45637f15-3bd3-4262-9abc-05cca6dc2e29'
			,'Region.Edit'
			,GETDATE()
           )
end
GO


if not exists(select [Name] from Permissions where [Name]=N'Region.Delete')
begin

INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'6c199451-5501-4a04-839e-231d9a63e41d'
			,'Region.Delete'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'District.View')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'810f37a9-424f-4362-9e06-7fbf41eff867'
			,'District.View'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'District.Edit')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'a9890a76-cdb3-40fb-a7f5-507da335b186'
			,'District.Edit'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'District.Delete')
begin

INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'985a8cab-14be-4262-a406-5a140a00203e'
			,'District.Delete'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Outpost.View')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'e8501372-98cc-4fa8-8322-cc8629ed6dbc'
			,'Outpost.View'
			,GETDATE()
           )
           end
GO
if not exists(select [Name] from Permissions where [Name]=N'Outpost.Edit')
begin

INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'0cad4331-fcac-43a3-91d0-80e3e3e8fa4d'
			,'Outpost.Edit'
			,GETDATE()
           )
   end
GO
if not exists(select [Name] from Permissions where [Name]=N'Outpost.Delete')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'0b1ca0af-7de5-4b3a-a937-6a9a21e4f2fd'
			,'Outpost.Delete'
			,GETDATE()
           )
end
GO
if not exists(select [Name] from Permissions where [Name]=N'ProductGroup.View')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'51BC7941-7F69-4193-93B7-ACC35028890E'
			,'ProductGroup.View'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'ProductGroup.Edit')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'397C3DC6-96A6-4DF7-ABA7-99260183184B'
			,'ProductGroup.Edit'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'ProductGroup.Delete')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'A2B2EF2C-C6F4-4A34-894C-57B8B486AD54'
			,'ProductGroup.Delete'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Product.View')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'6D389F28-5CDD-46D2-81F1-FED14D79A6A2'
			,'Product.View'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Product.Edit')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'E70E292A-6C11-45BF-A616-BEDA3AC0C26A'
			,'Product.Edit'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Product.Delete')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'07C27006-31EE-4FBF-BEF0-D8B776893B38'
			,'Product.Delete'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Campaign.View')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'9F34D436-E70C-4A9E-AB91-FB10AD1FA280'
			,'Campaign.View'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Campaign.Edit')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'C3949FDA-155D-4D35-B94E-FD2F270B423E'
			,'Campaign.Edit'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Campaign.Duplicate')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'A8F24F56-A9A2-4C1A-AADC-0BA266478C8C'
			,'Campaign.Duplicate'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'AutomaticSchedule.View')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'082B2120-26F2-4A15-83DB-D01F351CCE11'
			,'AutomaticSchedule.View'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'AutomaticSchedule.Edit')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'10D23EA4-3A4F-4425-A6F1-4E561273ADBD'
			,'AutomaticSchedule.Edit'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'AutomaticSchedule.Delete')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'98F72618-621B-477F-8C82-ED126D7EECEB'
			,'AutomaticSchedule.Delete'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'ProductLevelRequest.View')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'2AFF765B-A1BA-4BBC-83B0-B9799A0E11BF'
			,'ProductLevelRequest.View'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'ProductLevelRequest.Edit')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'470D3DA9-B579-4B82-A08C-8A4A399AE993'
			,'ProductLevelRequest.Edit'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'ProductLevelRequest.Stop')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'1D987D4C-2183-47F0-A436-45273E015574'
			,'ProductLevelRequest.Stop'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'ExistingRequest.View')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'7C379661-ACEE-4920-81BD-EFDDCD8720E2'
			,'ExistingRequest.View'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Client.View')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'C4D9A222-EBE0-43E3-B08E-EBF4F739410B'
			,'Client.View'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Client.Edit')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'2F86480E-3061-4753-AD70-6E58F380D3CD'
			,'Client.Edit'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Client.Delete')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'3EAABA64-68C5-4F5E-854E-5319398DC72C'
			,'Client.Delete'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'User.View')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'EDEE5446-BEF6-4B19-A5B6-AFECBC8032ED'
			,'User.View'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'User.Edit')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'802B1CE5-2A84-438F-8ACA-43EBE666705A'
			,'User.Edit'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'User.Delete')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'64FD80C3-88BB-48A9-9B36-A86810F1D61A'
			,'User.Delete'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Role.View')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'ECC5B5A5-FF52-4C3A-8390-4C994AF980A8'
			,'Role.View'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Role.Edit')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'796AE3E5-8FD9-4204-BA74-4D2A6B9152B4'
			,'Role.Edit'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Role.Delete')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'F0A91CAF-82EC-4B33-ACD3-6F12A525A573'
			,'Role.Delete'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'CurrentOutpostStockLevel.View')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'5FFCEADE-F3FD-453B-8553-58E7D566ACF1'
			,'CurrentOutpostStockLevel.View'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'CurrentOutpostStockLevel.Edit')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'222F64C2-B22C-4DE7-8934-E0B84806A1B5'
			,'CurrentOutpostStockLevel.Edit'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'HistoricalOutpostStockLevel.View')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'7DAC2C99-05CD-49D2-9CC9-400C547D5F1B'
			,'HistoricalOutpostStockLevel.View'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'HistoricalOutpostStockLevel.Edit')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'189C44FD-8F02-429C-B4D7-32BFB6B8F724'
			,'HistoricalOutpostStockLevel.Edit'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Alert.View')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'903EC6AD-51C5-4128-AC42-ED2CD01D4A1B'
			,'Alert.View'
			,GETDATE()
           )
end
GO

if not exists(select [Name] from Permissions where [Name]=N'Report.View')
begin


INSERT INTO [Permissions]
           ([Id]
           ,[Name]
           ,[Created]
           )
     VALUES
           (
			'7E508FF1-D448-4151-9C8B-906AF73505F5'
			,'Report.View'
			,GETDATE()
           )
end
GO