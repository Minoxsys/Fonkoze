if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'PermissionRoles')
begin

   -- alter table PermissionRoles  drop constraint RoleId_PFK
    
    alter table PermissionRoles  drop constraint Permission_FK

    drop table PermissionRoles
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Permissions')
begin
	alter table Permissions drop constraint ByUser_PFK
    drop table Permissions
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'RoleUsers')
begin

    alter table RoleUsers  drop constraint User_FK
    
    alter table RoleUsers  drop constraint Role_FK

    drop table RoleUsers
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Roles')
begin
	alter table Roles drop constraint ByUser_RFK
    drop table Roles
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Districts')
begin
	alter table Districts  drop constraint Region_FK
	alter table Districts  drop constraint ByUser_DFK
	drop table Districts
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Regions')
begin
	--alter table Regions drop constraint ByUser_REFK
	--alter table Regions drop constraint Client_RFK
	--alter table Regions drop constraint Country_RFK
	drop table Regions
	
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Countries')
begin
--	alter table Countries drop constraint Client_FK
    drop table Countries
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Clients')
begin
	alter table Clients drop constraint ByUser_FK
    drop table Clients
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Users')
begin
	alter table Users drop constraint ByUser_UFK
    drop table Users
end

