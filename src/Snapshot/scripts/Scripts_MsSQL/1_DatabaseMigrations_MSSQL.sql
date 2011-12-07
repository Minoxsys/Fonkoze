create table Permissions (
        Id UNIQUEIDENTIFIER not null,
       Name NVARCHAR(255) null unique,
       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )

    create table PermissionRoles (
        PermissionId_FK UNIQUEIDENTIFIER not null,
       RoleId_FK UNIQUEIDENTIFIER not null
    )

    create table Roles (
        Id UNIQUEIDENTIFIER not null,
       Name NVARCHAR(255) null unique,
       Description NVARCHAR(255) null,
       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )

    create table RoleUsers (
        RoleId_FK UNIQUEIDENTIFIER not null,
       UserId_FK UNIQUEIDENTIFIER not null
    )

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
	create table Clients (
        Id UNIQUEIDENTIFIER not null,
       Name NVARCHAR(255) null,
       Created DATETIME null,
       Updated DATETIME null,
       ByUser_FK UNIQUEIDENTIFIER null,
       primary key (Id)
    )
    alter table Clients 
        add constraint ByUser_FK 
        foreign key (ByUser_FK) 
        references Users
        
    alter table Permissions 
        add constraint ByUser_PFK 
        foreign key (ByUser_FK) 
        references Users

    alter table PermissionRoles 
        add constraint RoleId_PFK 
        foreign key (RoleId_FK) 
        references Roles

    alter table PermissionRoles 
        add constraint PermissionId_FK 
        foreign key (PermissionId_FK) 
        references Permissions

    alter table Roles 
        add constraint ByUser_RFK 
        foreign key (ByUser_FK) 
        references Users

    alter table RoleUsers 
        add constraint UserId_FK 
        foreign key (UserId_FK) 
        references Users

    alter table RoleUsers 
        add constraint RoleId_RFK 
        foreign key (RoleId_FK) 
        references Roles

    alter table Users 
        add constraint ByUser_UFK 
        foreign key (ByUser_FK) 
        references Users