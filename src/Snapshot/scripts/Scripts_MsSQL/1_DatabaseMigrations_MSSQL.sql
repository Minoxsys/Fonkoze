use [StockManager]

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
	   PhonePrefix NVARCHAR(3) null,
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
       Email NVARCHAR(50) null,
	   MainMobileNumber NVARCHAR(20) null,
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

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'MobilePhones')
begin
	-- MobilePhones
CREATE TABLE [MobilePhones] (
       Id UNIQUEIDENTIFIER not null,
       MobileNumber NVARCHAR(50) null,
       Created DATETIME null,
       Updated DATETIME null,
       Outpost_FK UNIQUEIDENTIFIER null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id))
end

go

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_RCoFK')
begin
    alter table Regions 
        add constraint ByUser_RCoFK 
        foreign key (Country_FK) 
        references Countries
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_RClFK')
begin
    alter table Regions 
        add constraint ByUser_RClFK 
        foreign key (Client_FK) 
        references Clients
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_RUFK')
begin
    alter table Regions 
        add constraint ByUser_RUFK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_RDFK')
begin
   alter table Districts 
        add constraint By_User_RDFK 
        foreign key (Client_FK) 
        references Clients
End

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Region_DRFK')
begin
    alter table Districts 
        add constraint Region_DRFK 
        foreign key (Region_FK) 
        references Regions
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_DUFK')
begin

    alter table Districts 
        add constraint ByUser_DUFK 
        foreign key (ByUser_FK) 
        references Users
end


if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_CUFK')
begin
    alter table Clients 
        add constraint ByUser_CUFK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_PUFK')
begin      
    alter table Permissions 
        add constraint ByUser_PUFK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Role_PRFK')
begin 
    alter table PermissionRoles 
        add constraint Role_PRFK 
        foreign key (Role_FK) 
        references Roles        
    
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Permission_PRFK')
begin 

	alter table PermissionRoles 
        add constraint Permission_FK 
        foreign key (Permission_FK) 
        references Permissions
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_RUFK')
begin
    alter table Roles 
        add constraint ByUser_RUFK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='User_URFK')
begin
    alter table RoleUsers 
        add constraint UserId_URFK 
        foreign key (User_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='User_RUFK')
begin
  alter table RoleUsers 
        add constraint Role_RUFK 
        foreign key (Role_FK) 
        references Roles
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Role_RUsFK')
begin
	alter table RoleUsers 
        add constraint Role_RUsFK 
        foreign key (Role_FK) 
        references Roles
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_UUFK')
begin
    alter table Users 
        add constraint ByUser_UUFK 
        foreign key (ByUser_FK) 
        references Users
end


if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Client_CClFK')
begin
    alter table Countries 
        add constraint Client_CClFK 
        foreign key (Client_FK) 
        references Clients
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='(ByUser_COUFK')
begin

    alter table Countries 
        add constraint ByUser_COUFK 
        foreign key (ByUser_FK) 
        references Users
End



if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Outpost_MPOFK')
begin
    alter table [MobilePhones] 
        add constraint Outpost_MPOFK 
        foreign key (Outpost_FK) 
        references Outposts
end


if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='(ByUser_MPUFK ')
begin

 alter table MobilePhones 
        add constraint ByUser_MPUFK 
        foreign key (ByUser_FK) 
        references Users
end


if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='(ByUser_OUFK ')
begin

    alter table Outposts 
        add constraint ByUser_OUFK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='(Country_OCFK ')
begin

    alter table Outposts 
        add constraint Country_OCFK 
        foreign key (ByUser_FK) 
        references Countries
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='(Region_OCFK ')
begin

    alter table Outposts 
        add constraint Region_OCFK 
        foreign key (ByUser_FK) 
        references Regions
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='(District_ODFK ')
begin

    alter table Outposts 
        add constraint ByUser_ODFK 
        foreign key (ByUser_FK) 
        references Districts
end
