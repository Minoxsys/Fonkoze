-- Users
GO
if not exists(select * from Users where [UserName]=N'admin')
begin
INSERT INTO Users
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
INSERT INTO [StockManager].[dbo].[Clients]
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
-- RoleUsers 
GO
if not exists(select * from RoleUsers 
	where [Role_FK] = '461e581b-e60b-4dfd-a5a8-88229f14379b' and [User_FK] = 'E8346290-DE35-47FB-8FEC-D2562DED7F40')
begin
INSERT INTO RoleUsers
           (Role_FK
           ,User_FK)
     VALUES
           ('461e581b-e60b-4dfd-a5a8-88229f14379b'
            ,'E8346290-DE35-47FB-8FEC-D2562DED7F40' )
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

--[StockManager].[dbo].[



