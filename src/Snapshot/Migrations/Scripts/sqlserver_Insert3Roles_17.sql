--Insert Seller,Manager, Fonkoze Sante Team in table Roles

GO
if not exists(select [Name] from Roles where [Name]=N'Manager')
begin
INSERT  INTO Roles
           (Id
           ,Name
           ,Description)
     VALUES
           ('618050EA-F20D-4B57-9B66-A196010A13BB'
           ,N'Manager'
           ,N'This role permits access to the entire application')
end	

GO
if not exists(select [Name] from Roles where [Name]=N'Seller')
begin
INSERT  INTO Roles
           (Id
           ,Name
           ,Description)
     VALUES
           ('E64459A9-BA8E-4E4D-8628-A19601093711'
           ,N'Seller'
           ,N'This role permits data view only')
end	

GO
if not exists(select [Name] from Roles where [Name]=N'Fonkoze Sante Team')
begin
INSERT  INTO Roles
           (Id
           ,Name
           ,Description)
     VALUES
           ('E97EA19E-364F-43A1-80BA-A196010A35EA'
           ,N'Fonkoze Sante Team'
           ,N'This role permits data view and edit stock levels')
end	


--------Insert Permissions for Role 'Manager'
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='80CDE125-6F44-477D-AAB7-171803030477' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('80CDE125-6F44-477D-AAB7-171803030477'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='369d6648-4e0f-453e-a2d6-5a54cc3b8aea' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('369d6648-4e0f-453e-a2d6-5a54cc3b8aea'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='2842ad00-170e-493e-b7ea-6c5f10d61d45' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('2842ad00-170e-493e-b7ea-6c5f10d61d45'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='744ba2ac-0a76-4c6e-8aa7-5d5cf8844ff1' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('744ba2ac-0a76-4c6e-8aa7-5d5cf8844ff1'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='5ce78ae3-32c8-4d93-8b93-6474ec13a1f2' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('5ce78ae3-32c8-4d93-8b93-6474ec13a1f2'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='45637f15-3bd3-4262-9abc-05cca6dc2e29' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('45637f15-3bd3-4262-9abc-05cca6dc2e29'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='6c199451-5501-4a04-839e-231d9a63e41d' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('6c199451-5501-4a04-839e-231d9a63e41d'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='810f37a9-424f-4362-9e06-7fbf41eff867' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('810f37a9-424f-4362-9e06-7fbf41eff867'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='a9890a76-cdb3-40fb-a7f5-507da335b186' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('a9890a76-cdb3-40fb-a7f5-507da335b186'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='985a8cab-14be-4262-a406-5a140a00203e' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('985a8cab-14be-4262-a406-5a140a00203e'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='e8501372-98cc-4fa8-8322-cc8629ed6dbc' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('e8501372-98cc-4fa8-8322-cc8629ed6dbc'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='0cad4331-fcac-43a3-91d0-80e3e3e8fa4d' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('0cad4331-fcac-43a3-91d0-80e3e3e8fa4d'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='0b1ca0af-7de5-4b3a-a937-6a9a21e4f2fd' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('0b1ca0af-7de5-4b3a-a937-6a9a21e4f2fd'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='51BC7941-7F69-4193-93B7-ACC35028890E' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('51BC7941-7F69-4193-93B7-ACC35028890E'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='397C3DC6-96A6-4DF7-ABA7-99260183184B' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('397C3DC6-96A6-4DF7-ABA7-99260183184B'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='A2B2EF2C-C6F4-4A34-894C-57B8B486AD54' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('A2B2EF2C-C6F4-4A34-894C-57B8B486AD54'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='6D389F28-5CDD-46D2-81F1-FED14D79A6A2' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('6D389F28-5CDD-46D2-81F1-FED14D79A6A2'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='E70E292A-6C11-45BF-A616-BEDA3AC0C26A' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('E70E292A-6C11-45BF-A616-BEDA3AC0C26A'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='07C27006-31EE-4FBF-BEF0-D8B776893B38' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('07C27006-31EE-4FBF-BEF0-D8B776893B38'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='9F34D436-E70C-4A9E-AB91-FB10AD1FA280' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('9F34D436-E70C-4A9E-AB91-FB10AD1FA280'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='C3949FDA-155D-4D35-B94E-FD2F270B423E' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('C3949FDA-155D-4D35-B94E-FD2F270B423E'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='A8F24F56-A9A2-4C1A-AADC-0BA266478C8C' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('A8F24F56-A9A2-4C1A-AADC-0BA266478C8C'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='082B2120-26F2-4A15-83DB-D01F351CCE11' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('082B2120-26F2-4A15-83DB-D01F351CCE11'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='10D23EA4-3A4F-4425-A6F1-4E561273ADBD' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('10D23EA4-3A4F-4425-A6F1-4E561273ADBD'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='98F72618-621B-477F-8C82-ED126D7EECEB' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('98F72618-621B-477F-8C82-ED126D7EECEB'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='2AFF765B-A1BA-4BBC-83B0-B9799A0E11BF' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('2AFF765B-A1BA-4BBC-83B0-B9799A0E11BF'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='470D3DA9-B579-4B82-A08C-8A4A399AE993' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('470D3DA9-B579-4B82-A08C-8A4A399AE993'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='1D987D4C-2183-47F0-A436-45273E015574' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('1D987D4C-2183-47F0-A436-45273E015574'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='7C379661-ACEE-4920-81BD-EFDDCD8720E2' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('7C379661-ACEE-4920-81BD-EFDDCD8720E2'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='C4D9A222-EBE0-43E3-B08E-EBF4F739410B' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('C4D9A222-EBE0-43E3-B08E-EBF4F739410B'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='2F86480E-3061-4753-AD70-6E58F380D3CD' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('2F86480E-3061-4753-AD70-6E58F380D3CD'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='3EAABA64-68C5-4F5E-854E-5319398DC72C' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('3EAABA64-68C5-4F5E-854E-5319398DC72C'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='EDEE5446-BEF6-4B19-A5B6-AFECBC8032ED' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('EDEE5446-BEF6-4B19-A5B6-AFECBC8032ED'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='802B1CE5-2A84-438F-8ACA-43EBE666705A' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('802B1CE5-2A84-438F-8ACA-43EBE666705A'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='64FD80C3-88BB-48A9-9B36-A86810F1D61A' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('64FD80C3-88BB-48A9-9B36-A86810F1D61A'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='ECC5B5A5-FF52-4C3A-8390-4C994AF980A8' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('ECC5B5A5-FF52-4C3A-8390-4C994AF980A8'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='796AE3E5-8FD9-4204-BA74-4D2A6B9152B4' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('796AE3E5-8FD9-4204-BA74-4D2A6B9152B4'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='F0A91CAF-82EC-4B33-ACD3-6F12A525A573' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('F0A91CAF-82EC-4B33-ACD3-6F12A525A573'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='5FFCEADE-F3FD-453B-8553-58E7D566ACF1' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('5FFCEADE-F3FD-453B-8553-58E7D566ACF1'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='222F64C2-B22C-4DE7-8934-E0B84806A1B5' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('222F64C2-B22C-4DE7-8934-E0B84806A1B5'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='7DAC2C99-05CD-49D2-9CC9-400C547D5F1B' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('7DAC2C99-05CD-49D2-9CC9-400C547D5F1B'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='189C44FD-8F02-429C-B4D7-32BFB6B8F724' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('189C44FD-8F02-429C-B4D7-32BFB6B8F724'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='903EC6AD-51C5-4128-AC42-ED2CD01D4A1B' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('903EC6AD-51C5-4128-AC42-ED2CD01D4A1B'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='7E508FF1-D448-4151-9C8B-906AF73505F5' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('7E508FF1-D448-4151-9C8B-906AF73505F5'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end
GO

if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='5521bcdc-bf90-417a-a419-b9879646fe0e' and [Role_FK] ='618050EA-F20D-4B57-9B66-A196010A13BB')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('5521bcdc-bf90-417a-a419-b9879646fe0e'
           ,'618050EA-F20D-4B57-9B66-A196010A13BB')
end

------------------------------------------------------------

-------Insert Permissions for Role 'Seller'

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='903EC6AD-51C5-4128-AC42-ED2CD01D4A1B' and [Role_FK] ='E64459A9-BA8E-4E4D-8628-A19601093711')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('903EC6AD-51C5-4128-AC42-ED2CD01D4A1B'
           ,'E64459A9-BA8E-4E4D-8628-A19601093711')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='80CDE125-6F44-477D-AAB7-171803030477' and [Role_FK] ='E64459A9-BA8E-4E4D-8628-A19601093711')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('80CDE125-6F44-477D-AAB7-171803030477'
           ,'E64459A9-BA8E-4E4D-8628-A19601093711')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='5FFCEADE-F3FD-453B-8553-58E7D566ACF1' and [Role_FK] ='E64459A9-BA8E-4E4D-8628-A19601093711')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('5FFCEADE-F3FD-453B-8553-58E7D566ACF1'
           ,'E64459A9-BA8E-4E4D-8628-A19601093711')
end


GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='7DAC2C99-05CD-49D2-9CC9-400C547D5F1B' and [Role_FK] ='E64459A9-BA8E-4E4D-8628-A19601093711')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('7DAC2C99-05CD-49D2-9CC9-400C547D5F1B'
           ,'E64459A9-BA8E-4E4D-8628-A19601093711')
end


GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='5521BCDC-BF90-417A-A419-B9879646FE0E' and [Role_FK] ='E64459A9-BA8E-4E4D-8628-A19601093711')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('5521BCDC-BF90-417A-A419-B9879646FE0E'
           ,'E64459A9-BA8E-4E4D-8628-A19601093711')
end


GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='6D389F28-5CDD-46D2-81F1-FED14D79A6A2' and [Role_FK] ='E64459A9-BA8E-4E4D-8628-A19601093711')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('6D389F28-5CDD-46D2-81F1-FED14D79A6A2'
           ,'E64459A9-BA8E-4E4D-8628-A19601093711')
end


GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='51BC7941-7F69-4193-93B7-ACC35028890E' and [Role_FK] ='E64459A9-BA8E-4E4D-8628-A19601093711')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('51BC7941-7F69-4193-93B7-ACC35028890E'
           ,'E64459A9-BA8E-4E4D-8628-A19601093711')
end


GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='2AFF765B-A1BA-4BBC-83B0-B9799A0E11BF' and [Role_FK] ='E64459A9-BA8E-4E4D-8628-A19601093711')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('2AFF765B-A1BA-4BBC-83B0-B9799A0E11BF'
           ,'E64459A9-BA8E-4E4D-8628-A19601093711')
end


GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='7E508FF1-D448-4151-9C8B-906AF73505F5' and [Role_FK] ='E64459A9-BA8E-4E4D-8628-A19601093711')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('7E508FF1-D448-4151-9C8B-906AF73505F5'
           ,'E64459A9-BA8E-4E4D-8628-A19601093711')
end


---------------------------Insert Permissions for Role 'Fonkoze Sante Team' ----
GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='903EC6AD-51C5-4128-AC42-ED2CD01D4A1B' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('903EC6AD-51C5-4128-AC42-ED2CD01D4A1B'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='80CDE125-6F44-477D-AAB7-171803030477' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('80CDE125-6F44-477D-AAB7-171803030477'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='082B2120-26F2-4A15-83DB-D01F351CCE11' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('082B2120-26F2-4A15-83DB-D01F351CCE11'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='9F34D436-E70C-4A9E-AB91-FB10AD1FA280' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('9F34D436-E70C-4A9E-AB91-FB10AD1FA280'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='C4D9A222-EBE0-43E3-B08E-EBF4F739410B' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('C4D9A222-EBE0-43E3-B08E-EBF4F739410B'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='369D6648-4E0F-453E-A2D6-5A54CC3B8AEA' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('369D6648-4E0F-453E-A2D6-5A54CC3B8AEA'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='5FFCEADE-F3FD-453B-8553-58E7D566ACF1' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('5FFCEADE-F3FD-453B-8553-58E7D566ACF1'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='810F37A9-424F-4362-9E06-7FBF41EFF867' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('810F37A9-424F-4362-9E06-7FBF41EFF867'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end


GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='7C379661-ACEE-4920-81BD-EFDDCD8720E2' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('7C379661-ACEE-4920-81BD-EFDDCD8720E2'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end


GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='7DAC2C99-05CD-49D2-9CC9-400C547D5F1B' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('7DAC2C99-05CD-49D2-9CC9-400C547D5F1B'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end


GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='5521BCDC-BF90-417A-A419-B9879646FE0E' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('5521BCDC-BF90-417A-A419-B9879646FE0E'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end


GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='6D389F28-5CDD-46D2-81F1-FED14D79A6A2' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('6D389F28-5CDD-46D2-81F1-FED14D79A6A2'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='51BC7941-7F69-4193-93B7-ACC35028890E' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('51BC7941-7F69-4193-93B7-ACC35028890E'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end


GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='2AFF765B-A1BA-4BBC-83B0-B9799A0E11BF' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('2AFF765B-A1BA-4BBC-83B0-B9799A0E11BF'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end


GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='5CE78AE3-32C8-4D93-8B93-6474EC13A1F2' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('5CE78AE3-32C8-4D93-8B93-6474EC13A1F2'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end


GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='7E508FF1-D448-4151-9C8B-906AF73505F5' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('7E508FF1-D448-4151-9C8B-906AF73505F5'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end


GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='ECC5B5A5-FF52-4C3A-8390-4C994AF980A8' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('ECC5B5A5-FF52-4C3A-8390-4C994AF980A8'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end


GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='EDEE5446-BEF6-4B19-A5B6-AFECBC8032ED' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('EDEE5446-BEF6-4B19-A5B6-AFECBC8032ED'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end

GO
if not exists(select [Permission_FK], [Role_FK] from [PermissionRoles] where [Permission_FK]='222F64C2-B22C-4DE7-8934-E0B84806A1B5' and [Role_FK] ='E97EA19E-364F-43A1-80BA-A196010A35EA')
begin
INSERT INTO PermissionRoles
           (Permission_FK
           ,Role_FK)
     VALUES
           ('222F64C2-B22C-4DE7-8934-E0B84806A1B5'
           ,'E97EA19E-364F-43A1-80BA-A196010A35EA')
end

