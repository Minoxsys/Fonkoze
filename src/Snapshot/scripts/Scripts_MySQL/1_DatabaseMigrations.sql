go create table if not exists Permissions (
        Id VARCHAR(40) not null,
       Name NVARCHAR(255) null unique,
       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK VARCHAR(40) null,
       primary key (Id)
    )

 go create table if not exists PermissionRoles (
        PermissionId_FK VARCHAR(40) not null,
       RoleId_FK VARCHAR(40) not null
    )

  go  create table if not exists Roles (
        Id VARCHAR(40) not null,
       Name NVARCHAR(255) null unique,
       Description NVARCHAR(255) null,
       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK VARCHAR(40) null,
       primary key (Id)
    )

   go create table if not exists RoleUsers (
        RoleId_FK VARCHAR(40) not null,
       UserId_FK VARCHAR(40) not null
    )

    go create table if not exists Users (
        Id VARCHAR(40) not null,
       UserName NVARCHAR(255) null unique,
       ClientId VARCHAR(40) null,
       Password NVARCHAR(255) null,
       Email NVARCHAR(255) null,
       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK VARCHAR(40) null,
       primary key (Id)
    )
	go create table if not exists Clients (
        Id VARCHAR(40) not null,
       Name NVARCHAR(255) null,
       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK VARCHAR(40) null,
       primary key (Id)
    )
    go create table if not exists EmailRequests (
        Id VARCHAR(40) not null,
       Date DATETIME null,
       OutpostId VARCHAR(40) null,
       ProductGroupId VARCHAR(40) null,
       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK VARCHAR(40) null,
       primary key (Id)
    )   
    go alter table Clients 
        add constraint ByUser_FK 
        foreign key (ByUser_FK) 
        references Users (Id)
        
     go alter table Permissions 
        add constraint ByUser_PFK 
        foreign key (ByUser_FK) 
        references Users (Id)

    go alter table PermissionRoles 
        add constraint RoleId_PFK 
        foreign key (RoleId_FK) 
        references Roles (Id)

    go alter table PermissionRoles 
        add constraint PermissionId_FK 
        foreign key (PermissionId_FK) 
        references Permissions (Id)

    go alter table Roles 
        add constraint ByUser_RFK 
        foreign key (ByUser_FK) 
        references Users (Id)

   go alter table RoleUsers 
        add constraint UserId_FK 
        foreign key (UserId_FK) 
        references Users (Id)

    go alter table RoleUsers 
        add constraint RoleId_RFK 
        foreign key (RoleId_FK) 
        references Roles (Id)

    go alter table Users 
        add constraint ByUser_UFK 
        foreign key (ByUser_FK) 
        references Users (Id)