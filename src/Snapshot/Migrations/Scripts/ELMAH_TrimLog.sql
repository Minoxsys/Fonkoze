CREATE PROCEDURE [dbo].[TrimLogTable] 

	@cutoffdays INT 

AS 
 
	SET NOCOUNT ON; 
	
	DELETE FROM ELMAH_Error WHERE [TimeUtc] < DATEADD(day, -@cutoffdays, GETDATE());
	
	SELECT NULL FROM [dbo].[Users] WHERE 2=3;

GO