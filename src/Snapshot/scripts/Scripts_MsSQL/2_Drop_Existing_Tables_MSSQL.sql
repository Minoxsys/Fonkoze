if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'PermissionRoles')
begin

    alter table PermissionRoles  drop constraint Role_PRFK
    alter table PermissionRoles  drop constraint Permission_PRFK
    drop table PermissionRoles
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Permissions')
begin
	alter table Permissions drop constraint ByUser_PRFK
    drop table Permissions
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'RoleUsers')
begin
   
    alter table RoleUsers  drop constraint Role_RUsFK
    alter table RoleUsers  drop constraint User_RUFK
    alter table RoleUsers  drop constraint User_URFK
    drop table RoleUsers
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Outposts')
begin
    alter table Outposts  drop constraint Region_OCFK
    alter table Outposts  drop constraint District_ODFK
    alter table Outposts  drop constraint Country_OCFK
	alter table Outposts drop constraint ByUser_OUFK
	alter table Outposts drop constraint StockItem_OFK
	drop table Outposts
end


if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'StockItems')
begin
	alter table StockItems  drop constraint ByUser_StIFK
	alter table StockItems  drop constraint StockGroup_FK
	drop table StockItems
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'StockGroups')
begin
	alter table StockGroups  drop constraint ByUser_SGFK
	drop table StockGroups
end
if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Roles')
begin
	--alter table Roles drop constraint Role_PRFK
    drop table Roles
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'MobilePhones')
begin
	alter table MobilePhones drop constraint Outpost_MPOFK 
    drop table MobilePhones
end


if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Districts')
begin
	alter table Districts  drop constraint ByUser_DUFK
	alter table Districts  drop constraint ByUser_DClFK
	alter table Districts  drop constraint Region_DRFK
	drop table Districts
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Regions')
begin
	--alter table Regions drop constraint ByUser_RClFK
	alter table Regions drop constraint ByUser_RCoFK
	alter table Regions drop constraint ByUser_RUFK
	drop table Regions
end



if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Countries')
begin
	alter table Countries drop constraint ByUser_COUFK
	alter table Countries drop constraint Client_CClFK
    drop table Countries
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Clients')
begin
	alter table Clients drop constraint ByUser_CUFK
    drop table Clients
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Users')
begin
	alter table Users drop constraint ByUser_UUFK
    drop table Users
end

