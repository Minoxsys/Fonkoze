﻿BEGIN TRANSACTION
GO
ALTER TABLE dbo.Users
 DROP COLUMN PhoneNumber
GO

GO
ALTER TABLE dbo.Districts
 DROP COLUMN DistrictManager_FK
GO

COMMIT