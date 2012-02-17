if exists(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME ='ClientName')
begin
    alter table Users 
        drop column ClientName
end

if exists(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME ='RoleName')
begin
    alter table Users 
        drop column RoleName
end
