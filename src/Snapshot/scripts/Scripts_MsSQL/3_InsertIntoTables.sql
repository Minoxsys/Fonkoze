-- Users

INSERT INTO [StockManager].[dbo].[Users]
           ([Id]
           ,[UserName]
           ,[ClientId]
           ,[Password]
           ,[Email]
           ,[Created]
           ,[Updated]
           ,[ByUser_FK])
     VALUES
           ('E8346290-DE35-47FB-8FEC-D2562DED7F40'
           ,'admin'
           ,null
           ,'admin'
           ,'admin@evozon.com'
           ,GETDATE()
           ,GETDATE()
           ,null)
	GO	   
-- Clients

INSERT INTO [StockManager].[dbo].[Clients]
           ([Id]
           ,[Name]
           ,[Created]
           ,[Updated]
           ,[ByUser_FK])
     VALUES
           ( '00000000-0000-0000-0000-000000000000'
           ,'Minoxsys'
           ,GETDATE()
           ,GETDATE()
           ,null)
GO
 -- Permissions
            
 INSERT  INTO Permissions
           (Id
           ,Name)
VALUES
           ('95B06FCC-CCAC-4634-9908-AED2C6569BD5'
           ,N'UserManager.Overview')
           
 INSERT  INTO Permissions
           (Id
           ,Name)
VALUES
           ('F13223AD-7731-409B-BA96-7B0D01998085'
           ,N'UserManager.CRUD')
           
 INSERT  INTO Permissions
           (Id
           ,Name)
VALUES
           ('1A240DF2-9C2B-43A6-999D-DA2055A4533E'
           ,N'RoleManager.Overview')
           
 INSERT  INTO Permissions
           (Id
           ,Name)
VALUES
           ('E37B5A01-B425-40B0-B613-34245393AD0D'
           ,N'RoleManager.CRUD')
           
           
 INSERT  INTO Permissions
           (Id
           ,Name)
VALUES
           ('80CDE125-6F44-477D-AAB7-171803030477'
           ,N'Home.Index')
           
-- Roles

INSERT  INTO Roles
           (Id
           ,Name
           ,Description)
     VALUES
           ('461e581b-e60b-4dfd-a5a8-88229f14379b'
           ,N'AllAccess'
           ,N'This role permits access to the entire application')
		   
-- RoleUsers 

INSERT INTO RoleUsers
           (RoleId_FK
           ,UserId_FK)
     VALUES
           ('461e581b-e60b-4dfd-a5a8-88229f14379b'
            ,'E8346290-DE35-47FB-8FEC-D2562DED7F40' )
			
-- PermissionRoles

INSERT INTO PermissionRoles
           (PermissionId_FK
           ,RoleId_FK)
     VALUES
           ('80CDE125-6F44-477D-AAB7-171803030477'
           ,'461e581b-e60b-4dfd-a5a8-88229f14379b')


