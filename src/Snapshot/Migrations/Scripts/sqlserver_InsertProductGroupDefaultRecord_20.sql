 GO
if not exists(select [Name] from [ProductGroups] where [Name]='All Products' )
begin
INSERT INTO ProductGroups
		   (Id
		   ,Created
		   ,Client_FK
		   ,Name
		   ,ReferenceCode
		   ,Description)
	 VALUES
		   (newid()
		   ,GETDATE()
		   ,'BEEC53CE-A73C-4F03-A354-C617F68BC813'
		   ,'All Products'
		   ,'ALL'
		   ,'Generic product group')
end
 GO