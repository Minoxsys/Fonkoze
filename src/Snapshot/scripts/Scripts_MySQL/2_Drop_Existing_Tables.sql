go alter table PermissionRoles  drop foreign key RoleId_PFK
   
go alter table PermissionRoles  drop foreign key PermissionId_FK

go alter table Permissions drop foreign key ByUser_PFK

go alter table RoleUsers  drop foreign key UserId_FK

go alter table RoleUsers  drop foreign key RoleId_RFK

go alter table Roles drop foreign key ByUser_RFK

go alter table Clients drop foreign key ByUser_FK

go alter table Users drop foreign key ByUser_UFK

go drop table if exists PermissionRoles
go drop table if exists Permissions
go drop table if exists RoleUsers
go drop table if exists Roles
go drop table if exists Clients
go drop table if exists Users
