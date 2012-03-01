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

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Users')
begin
    create table Users (
       Id UNIQUEIDENTIFIER not null,
       UserName NVARCHAR(255) null unique,
       FirstName NVARCHAR(255) null,
       LastName NVARCHAR(255) null,
       ClientId UNIQUEIDENTIFIER null,
       RoleId UNIQUEIDENTIFIER null,
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
	   IsWarehouse BIT NOT NULL DEFAULT 'False',
       Created DATETIME null,
       Updated DATETIME null,
	   Warehouse_FK UNIQUEIDENTIFIER null,
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
       ReferenceCode NVARCHAR(255) null,
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
       Created DATETIME null,
       Updated DATETIME null,
       ProductGroup_FK UNIQUEIDENTIFIER null,
       ByUser_FK UNIQUEIDENTIFIER null,
       Outpost_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )
end


if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'OutpostStockLevels')
begin
	-- Stock Level
	CREATE TABLE OutpostStockLevels(
        Id UNIQUEIDENTIFIER not null,
		OutpostId UNIQUEIDENTIFIER not null,
		ProdGroupId UNIQUEIDENTIFIER NOT NULL,
		ProductId UNIQUEIDENTIFIER NOT NULL,
		ProductGroupName nvarchar(30) not null,
		ProductName nvarchar(30) not null,
		ProdSMSRef NVARCHAR(20) NOT NULL,
		StockLevel INTEGER NOT NULL,
		PrevStockLevel INTEGER NOT NULL,
		UpdatedMethod NCHAR(10) NULL DEFAULT 'SMS',
		Created DATETIME NULL,
		Updated DATETIME NULL,
        ByUser_FK UNIQUEIDENTIFIER NULL,
        Client_FK UNIQUEIDENTIFIER NOT NULL
        primary key (Id)
    )
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'OutpostHistoricalStockLevels')
begin
create table OutpostHistoricalStockLevels (
        Id UNIQUEIDENTIFIER not null,
       OutpostId UNIQUEIDENTIFIER null,
       ProdGroupId UNIQUEIDENTIFIER null,
       ProductId UNIQUEIDENTIFIER null,
       ProdSmsRef NVARCHAR(255) null,
       StockLevel INT null,
       PrevStockLevel INT null,
       UpdateMethod NVARCHAR(255) null,
       UpdateDate DATETIME null,
       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )
end


if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'EmailRequests')
begin
	create table EmailRequests (
        Id UNIQUEIDENTIFIER not null,
       [Date] DATETIME null,
       OutpostId UNIQUEIDENTIFIER null,
       ProductGroupId UNIQUEIDENTIFIER null,

       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'WorldCountryRecords')
begin
 create table WorldCountryRecords (
        Id UNIQUEIDENTIFIER not null,
       Name NVARCHAR(255) null,
       ISOCode NVARCHAR(255) null,
       PhonePrefix NVARCHAR(255) null,

       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Campaigns')
begin
 create table Campaigns (
        Id UNIQUEIDENTIFIER not null,
       Name varchar(30),
       StartDate DATETIME,
       EndDate DATETIME,
       CreationDate DATETIME,
       Opened INTEGER,
       Options binary,
       Created DATETIME,
       Updated DATETIME,
       Client_FK UNIQUEIDENTIFIER,
       ByUser_FK UNIQUEIDENTIFIER,
       primary key (Id)
    )
end
if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_Campaigns_FK')
begin
	alter table Campaigns 
        add constraint ByUser_Campaigns_FK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Client_Campaigns_FK')
begin
	alter table Campaigns 
        add constraint Client_Campaigns_FK 
        foreign key (Client_FK) 
        references Clients
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'SmsRequests')
begin
create table SmsRequests (
	Id UNIQUEIDENTIFIER not null,
	Message NVARCHAR(255) not null,
	Number NVARCHAR(255) not null,
	ProductGroupReferenceCode NVARCHAR(255) not null,
	OutpostId UNIQUEIDENTIFIER not null,
	ProductGroupId UNIQUEIDENTIFIER not null,
	Created DATETIME null,
    Updated DATETIME null,
    ByUser_FK UNIQUEIDENTIFIER null,
    Client_FK UNIQUEIDENTIFIER null,
	primary key (Id)
	)
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'RawSmsReceiveds')
begin
create table RawSmsReceiveds (
	Id UNIQUEIDENTIFIER not null,
	Sender NVARCHAR(255) not null,
	Content NVARCHAR(255) not null,
	Credits NVARCHAR(255) null,
	OutpostId UNIQUEIDENTIFIER null,
	ParseSucceeded bit null,
	ParseErrorMessage NVARCHAR(255) null,
	Created DATETIME null,
    Updated DATETIME null,
    ByUser_FK UNIQUEIDENTIFIER null,
	primary key (Id)
	)
end


if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'Schedules')
begin
create table Schedules (
	Id UNIQUEIDENTIFIER not null,
	Name NVARCHAR(255) not null,
	FrequencyType NVARCHAR(255) null,
	FrequencyValue INT null,
	StartOn INT null,
	ScheduleBasis NVARCHAR(255) not null,
	Created DATETIME null,
    Updated DATETIME null,
    ByUser_FK UNIQUEIDENTIFIER null,
	primary key (Id)
	)
end

if not exists(select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_NAME=N'RequestReminders')
begin
create table RequestReminders (
	Id UNIQUEIDENTIFIER not null,
	PeriodType NVARCHAR(255) not null,
	PeriodValue INT not null,
	Schedule_FK UNIQUEIDENTIFIER not null,
	Created DATETIME null,
    Updated DATETIME null,
    ByUser_FK UNIQUEIDENTIFIER null,
	primary key (Id)
	)
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='UniqueProductNameForProductGroup')
begin
	ALTER TABLE Products
	ADD CONSTRAINT UniqueProductNameForProductGroup UNIQUE (Name,ProductGroup_FK)
end
if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='UniqueRegionNameForCountry')
begin
	ALTER TABLE Regions
	ADD CONSTRAINT UniqueRegionNameForCountry UNIQUE (Name,Country_FK)
end
if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='UniqueRegionCoordinatesForCountry')
begin
	ALTER TABLE Regions
	ADD CONSTRAINT UniqueRegionCoordinatesForCountry UNIQUE (Coordinates,Country_FK)
end
if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_OutpostHistoricalStockLevels_FK')
begin
	alter table OutpostHistoricalStockLevels 
        add constraint ByUser_OutpostHistoricalStockLevels_FK 
        foreign key (ByUser_FK) 
        references Users
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


if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Role_RolesUsers_FK')
begin
  alter table RoleUsers 
        add constraint Role_RolesUsers_FK 
        foreign key (Role_FK) 
        references Roles
end

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


if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Warehouse_Outpost_FK ')
begin

    alter table Outposts 
        add constraint Warehouse_Outpost_FK
        foreign key (Warehouse_FK) 
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

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='WorldCountryRecord_User_FK ')
begin
 alter table WorldCountryRecords 
        add constraint WorldCountryRecord_User_FK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME = 'Outpost_SmsRequest_FK')
begin
	alter table SmsRequests
		add constraint Outpost_SmsRequest_FK
		foreign key (OutpostId)
		references Outposts
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME = 'ProductGroup_SmsRequest_FK')
begin
	alter table SmsRequests
		add constraint ProductGroup_SmsRequest_FK
		foreign key (ProductGroupId)
		references ProductGroups
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_SmsRequests_FK')
begin

    alter table SmsRequests 
        add constraint ByUser_SmsRequests_FK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_RawSmsReceiveds_FK')
begin

    alter table RawSmsReceiveds 
        add constraint ByUser_RawSmsReceiveds_FK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_RequestReminders_FK')
begin

    alter table RequestReminder 
        add constraint ByUser_RequestReminders_FK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_ScheduleFrequencys_FK')
begin

    alter table ScheduleFrequency 
        add constraint ByUser_ScheduleFrequencys_FK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_Schedules_FK')
begin

    alter table Schedules 
        add constraint ByUser_Schedules_FK 
        foreign key (ByUser_FK) 
        references Users
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='Schedule_RequestReminders_FK')
begin

    alter table RequestReminders 
        add constraint Schedule_RequestReminders_FK 
        foreign key (Schedule_FK) 
        references Schedules
end

if not exists(SELECT * FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS WHERE CONSTRAINT_NAME ='ByUser_RequestReminders_FK')
begin

    alter table RequestReminders 
        add constraint ByUser_RequestReminders_FK 
        foreign key (ByUser_FK) 
        references Users
end
