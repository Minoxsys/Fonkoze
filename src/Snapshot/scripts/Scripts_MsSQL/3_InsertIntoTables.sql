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
           ,'BEEC53CE-A73C-4F03-A354-C617F68BC813'
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

-- WorldCountryRecord
GO

if not exists(select [Name] from [WorldCountryRecords] where [Name]='Romania' )
begin
INSERT INTO WorldCountryRecords
           (Name
           ,Id
           ,ISOCode
		   ,PhonePrefix)
     VALUES
           ('Romania'
           ,'9281D38F-25B8-493C-83AC-3F64E6FECB0C'
           ,'RO'
		   ,'0040')
end
GO
if not exists(select [Name] from [WorldCountryRecords] where [Name]='United Kingdom' )
begin
INSERT INTO WorldCountryRecords
           (Name
           ,Id
           ,ISOCode
		   ,PhonePrefix)
     VALUES
           ('United Kingdom'
           ,'87E504B5-6FB8-4F6C-9436-89C42EB5AEE1'
           ,'UK'
		   ,'0044')
end
GO
if not exists(select [Name] from [WorldCountryRecords] where [Name]='Germany' )
begin
INSERT INTO WorldCountryRecords
           (Name
           ,Id
           ,ISOCode
		   ,PhonePrefix)
     VALUES
           ('Germany'
           ,'1A842E3A-2606-4953-A41E-174D5A8F6D93'
           ,'GB'
		   ,'0049')
end

GO
if not exists(select [Name] from [WorldCountryRecords] where [Name]='Netherlands' )
begin
INSERT INTO WorldCountryRecords
           (Name
           ,Id
           ,ISOCode
		   ,PhonePrefix)
     VALUES
           ('Netherlands'
           ,'C3203A95-21A1-4601-A9CC-19C9A414D440'
           ,'NL'
		   ,'0031')
end

GO

INSERT INTO [StockManager].[dbo].[Permissions]
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
GO
INSERT INTO [StockManager].[dbo].[Permissions]
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
GO
INSERT INTO [StockManager].[dbo].[Permissions]
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
GO


INSERT INTO [StockManager].[dbo].[Permissions]
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
GO
INSERT INTO [StockManager].[dbo].[Permissions]
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
GO
INSERT INTO [StockManager].[dbo].[Permissions]
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
GO

INSERT INTO [StockManager].[dbo].[Permissions]
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
GO
INSERT INTO [StockManager].[dbo].[Permissions]
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
GO
INSERT INTO [StockManager].[dbo].[Permissions]
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
GO


INSERT INTO [StockManager].[dbo].[Permissions]
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
GO
INSERT INTO [StockManager].[dbo].[Permissions]
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
GO
INSERT INTO [StockManager].[dbo].[Permissions]
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
GO