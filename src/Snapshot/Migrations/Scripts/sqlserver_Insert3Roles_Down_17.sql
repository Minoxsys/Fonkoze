---Delete inserted roles and their permissions
GO
Delete From Roles Where Id ='618050EA-F20D-4B57-9B66-A196010A13BB'
GO
Delete From Roles Where Id ='E64459A9-BA8E-4E4D-8628-A19601093711'
GO
Delete From Roles Where Id ='E97EA19E-364F-43A1-80BA-A196010A35EA'
GO 
Delete From [PermissionRoles] Where  [Role_FK] in ('618050EA-F20D-4B57-9B66-A196010A13BB','E64459A9-BA8E-4E4D-8628-A19601093711','E97EA19E-364F-43A1-80BA-A196010A35EA')
GO