 GO
if not exists(select [Name] from [ProductGroups] where [Name]='Uncategorized' )
begin
INSERT INTO ProductGroups
		   (Id
		   ,Created
		   ,Client_FK
		   ,Name
		   ,ReferenceCode
		   ,Description)
	 VALUES
		   ('C1D9B38A-D2C6-4A95-BE0E-215F569B782F'
		   ,GETDATE()
		   ,'BEEC53CE-A73C-4F03-A354-C617F68BC813'
		   ,'Uncategorized'
		   ,'ALL'
		   ,'Generic product group')
end
 GO