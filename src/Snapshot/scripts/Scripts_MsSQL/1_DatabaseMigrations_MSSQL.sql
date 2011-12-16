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
if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Countries')
begin


	-- Countries
CREATE TABLE [Countries] (
       Id UNIQUEIDENTIFIER not null,
       Name NVARCHAR(255) null,
       ISOCode NVARCHAR(3) null,
       PhonePrefix NVARCHAR(3) null,
       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id))
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

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Districts')
begin
 create table Districts (
        Id UNIQUEIDENTIFIER not null,
       Name NVARCHAR(255) null,
       Created DATETIME null,
       Updated DATETIME null,
       Region_FK UNIQUEIDENTIFIER null,
       Client_FK UNIQUEIDENTIFIER null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Region_FK')
begin
 alter table Districts 
        add constraint Region_FK 
        foreign key (Region_FK) 
        references Regions
end
if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_DFK')
begin
    alter table Districts 
        add constraint ByUser_DFK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Client_DFK')
begin
alter table Districts 
        add constraint Client_DFK 
        foreign key (Client_FK) 
        references Clients
end
if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_REFK')
begin
	alter table Regions 
        add constraint ByUser_REFK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Client_RFK')
begin
alter table Regions 
        add constraint Client_RFK 
        foreign key (Client_FK) 
        references Clients
end
       
if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Country_RFK')
begin
  alter table Regions 
        add constraint Country_RFK 
        foreign key (Country_FK) 
        references Countries
end
if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_FK')
begin
    alter table Clients 
        add constraint ByUser_FK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_PFK')
begin      
    alter table Permissions 
        add constraint ByUser_PFK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='RoleId_PFK')
begin 
    alter table PermissionRoles 
        add constraint RoleId_PFK 
        foreign key (Role_FK) 
        references Roles        
    
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='PermissionId_FK')
begin 

	alter table PermissionRoles 
        add constraint PermissionId_FK 
        foreign key (Permission_FK) 
        references Permissions
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_RFK')
begin
    alter table Roles 
        add constraint ByUser_RFK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='UserId_FK')
begin
    alter table RoleUsers 
        add constraint UserId_FK 
        foreign key (User_FK) 
        references Users

    alter table RoleUsers 
        add constraint RoleId_RFK 
        foreign key (Role_FK) 
        references Roles
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='RoleId_RFK')
begin
	alter table RoleUsers 
        add constraint RoleId_RFK 
        foreign key (Role_FK) 
        references Roles
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_UFK')
begin
    alter table Users 
        add constraint ByUser_UFK 
        foreign key (ByUser_FK) 
        references Users
end



