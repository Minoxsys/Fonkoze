if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'PermissionRoles')
begin

    drop table PermissionRoles
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Permissions')
begin
    drop table Permissions
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'RoleUsers')
begin

    drop table RoleUsers
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Roles')
begin
    drop table Roles
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Products')
begin
	drop table Products
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'ProductGroups')
begin
	drop table ProductGroups
end


if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Contacts')
begin
    drop table Contacts
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Outposts')
begin

	drop table Outposts
end


if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Districts')
begin
	drop table Districts
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Regions')
begin
	drop table Regions
end


if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Countries')
begin
    drop table Countries
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'OutpostStockLevels')
begin
ALTER TABLE OutpostStockLevels
DROP INDEX UNIQUE_ProductIdOnProductGroupWithSMSRef
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Clients')
begin
    drop table Clients
end

if exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Users')
begin

    drop table Users
end

