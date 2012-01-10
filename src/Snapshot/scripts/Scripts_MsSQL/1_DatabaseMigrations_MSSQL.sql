if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Permissions')
begin
create table Permissions (
       Id UNIQUEIDENTIFIER not null,
       Name NVARCHAR(255) null unique,
       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'PermissionRoles')
begin
    create table PermissionRoles (
       Permission_FK UNIQUEIDENTIFIER not null,
       Role_FK UNIQUEIDENTIFIER not null
    )
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Roles')
begin
    create table Roles (
       Id UNIQUEIDENTIFIER not null,
       Name NVARCHAR(255) null unique,
       Description NVARCHAR(255) null,
       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'RoleUsers')
begin
    create table RoleUsers (
       Role_FK UNIQUEIDENTIFIER not null,
       User_FK UNIQUEIDENTIFIER not null
    )
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Users')
begin
    create table Users (
       Id UNIQUEIDENTIFIER not null,
       UserName NVARCHAR(255) null unique,
       ClientId UNIQUEIDENTIFIER null,
       Password NVARCHAR(255) null,
       Email NVARCHAR(255) null,
       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Clients')
begin
	create table Clients (
       Id UNIQUEIDENTIFIER not null,
       Name NVARCHAR(255) null,
       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Regions')
begin
    create table Regions (
        Id UNIQUEIDENTIFIER not null,
       Name NVARCHAR(255) null,
       Coordinates NVARCHAR(255) null,
       Created DATETIME null,
       Updated DATETIME null,
       Country_FK UNIQUEIDENTIFIER null,
       Client_FK UNIQUEIDENTIFIER null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )
end
go
if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Districts')
begin

    create table Districts (
        Id UNIQUEIDENTIFIER not null,
       Name NVARCHAR(255) null,
       Created DATETIME null,
       Updated DATETIME null,
       Client_FK UNIQUEIDENTIFIER null,
       Region_FK UNIQUEIDENTIFIER null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Countries')
begin
	-- Countries
CREATE TABLE [Countries] (
       Id UNIQUEIDENTIFIER not null,
       Name NVARCHAR(50) null,
	   ISOCode NVARCHAR(3) null,
	   PhonePrefix NVARCHAR(5) null,
       Created DATETIME null,
       Updated DATETIME null,
	   Client_FK UNIQUEIDENTIFIER null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id))
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Outposts')
begin
	-- Outposts
CREATE TABLE [Outposts] (
       Id UNIQUEIDENTIFIER not null,
       Name NVARCHAR(40) null,
       OutpostType NVARCHAR(20) null,
	   DetailMethod NVARCHAR(100) null,
	   Latitude NVARCHAR(20) null,
	   Longitude NVARCHAR(20) null,
       Created DATETIME null,
       Updated DATETIME null,
	   Country_FK UNIQUEIDENTIFIER null,
	   Region_FK UNIQUEIDENTIFIER null,
	   District_FK UNIQUEIDENTIFIER null,
	   Client_FK UNIQUEIDENTIFIER null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id))
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Contacts')
begin
	-- Contacts
CREATE TABLE [Contacts] (
       Id UNIQUEIDENTIFIER not null,
       ContactType NVARCHAR(15) null,
       ContactDetail NVARCHAR(100) null,
       IsMainContact BIT NULL,
       Created DATETIME null,
       Updated DATETIME null,
       Outpost_FK UNIQUEIDENTIFIER null,
       ByUser_FK UNIQUEIDENTIFIER null,
       Client_FK UNIQUEIDENTIFIER not null
       primary key (Id))
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'ProductGroups')
begin
 create table ProductGroups (
       Id UNIQUEIDENTIFIER not null,
       Name NVARCHAR(255) null,
       Description NVARCHAR(255) null,
       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Products')
begin
 create table Products (
       Id UNIQUEIDENTIFIER not null,
       Name NVARCHAR(255) null,
       Description NVARCHAR(255) null,
       LowerLimit INT null,
       UpperLimit INT null,
       SMSReferenceCode NVARCHAR(255) null,
       UpdateMethod NVARCHAR(255) null,
       PreviousStockLevel INT null,
       StockLevel INT null,
       Created DATETIME null,
       Updated DATETIME null,
       ProductGroup_FK UNIQUEIDENTIFIER null,
       Outpost_FK UNIQUEIDENTIFIER null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )
end

go
if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Product_ProductGroups_FK')
begin
alter table Products 
        add constraint Product_ProductGroups_FK 
        foreign key (ProductGroup_FK) 
        references ProductGroups
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_User_Products_FK')
begin
	alter table Products 
        add constraint ByUser_User_Products_FK 
        foreign key (ByUser_FK) 
        references Users
end
if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_Region_Countries_FK')
begin
    alter table Regions 
        add constraint Region_Countries_FK 
        foreign key (Country_FK) 
        references Countries
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_User_ProductGroups_FK')
begin
	alter table ProductGroups 
        add constraint ByUser_User_ProductGroups_FK 
        foreign key (ByUser_FK) 
        references Users

end
if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_Client_Regions_FK')
begin
    alter table Regions 
        add constraint Client_Regions_FK 
        foreign key (Client_FK) 
        references Clients
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_User_Regions_FK')
begin
    alter table Regions 
        add constraint ByUser_User_Regions_FK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_Client_Districts_FK')
begin
   alter table Districts 
        add constraint ByUser_Client_Districts_FK 
        foreign key (Client_FK) 
        references Clients
End

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_User_Districts_FK')
begin

    alter table Districts 
        add constraint ByUser_User_Districts_FK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_User_Clients_FK')
begin
    alter table Clients 
        add constraint ByUser_User_Clients_FK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_User_Permissions_FK')
begin      
    alter table Permissions 
        add constraint ByUser_User_Permissions_FK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_User_Roles_FK')
begin
    alter table Roles 
        add constraint ByUser_User_Roles_FK 
        foreign key (ByUser_FK) 
        references Users
end

-- end ByUser Region


if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Region_Districts_FK')
begin
    alter table Districts 
        add constraint Region_Districts_FK 
        foreign key (Region_FK) 
        references Regions
end


if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Role_Permissions_FK')
begin 
    alter table PermissionRoles 
        add constraint Role_Permissions_FK 
        foreign key (Role_FK) 
        references Roles        
    
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Permission_PermissionRoles_FK')
begin 

	alter table PermissionRoles 
        add constraint Permission_PermissionRoles_FK 
        foreign key (Permission_FK) 
        references Permissions
end


if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_User_Users_FK')
begin
    alter table RoleUsers 
        add constraint ByUser_User_Users_FK 
        foreign key (User_FK) 
        references Users
end
/*
if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Role_RolesUsers_FK')
begin
  alter table RoleUsers 
        add constraint ByUser_User_Roles_FK 
        foreign key (Role_FK) 
        references Roles
end
*/
/*
if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_User_Users_UFK')
begin
    alter table Users 
        add constraint ByUser_User_Users_UFK 
        foreign key (ByUser_FK) 
        references Users
end
*/

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Client_Countries_FK')
begin
    alter table Countries 
        add constraint Client_Countries_FK 
        foreign key (Client_FK) 
        references Clients
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_User_Countries_FK')
begin

    alter table Countries 
        add constraint ByUser_User_Countries_FK 
        foreign key (ByUser_FK) 
        references Users
End



if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='User_Contacts_FK ')
begin

 alter table Contacts 
        add constraint User_Contacts_FK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Client_Contacts_FK')
begin

 alter table Contacts 
        add constraint Client_Contacts_FK
        foreign key (Client_FK) 
        references Clients
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Outpost_Contacts_FK')
begin

 alter table Contacts 
        add constraint Outpost_Contacts_FK 
        foreign key (Outpost_FK) 
        references Outposts
end


if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='User_Outposts_FK ')
begin

    alter table Outposts 
        add constraint User_Outposts_FK
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Country_Outposts_FK ')
begin

    alter table Outposts 
        add constraint Country_Outposts_FK 
        foreign key (Country_FK) 
        references Countries
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Region_Outposts_FK ')
begin

    alter table Outposts 
        add constraint Region_Outposts_FK 
        foreign key (Region_FK) 
        references Regions
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='District_Outposts_FK ')
begin

    alter table Outposts 
        add constraint District_Outposts_FK
        foreign key (District_FK) 
        references Districts
end


if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Outpost_Products_OFK ')
begin
alter table Products 
        add constraint Outpost_Products_OFK 
        foreign key (Outpost_FK) 
        references Outposts
end
