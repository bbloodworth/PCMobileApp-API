/*
-- =============================================
-- Author:		Rey D
-- Create date: 2016-04-06
-- Description:	Adds new Experience Event type for Network Latency in ExperienceEvent table
-- =============================================
*/

  IF (NOT EXISTS(SELECT * FROM [dbo].[ExperienceEvent] WHERE ExperienceEventDesc = 'NetworkLatency') )
	BEGIN TRY
	  INSERT INTO [dbo].[ExperienceEvent] (ExperienceEventDesc, CreateDate)
	  VALUES ('NetworkLatency', GETDATE())
	END TRY
	BEGIN CATCH
		IF (ERROR_NUMBER() = 515)
			-- Error Message Cannot insert the value NULL into column 'ExperienceEventID', 
			-- table 'dbo.ExperienceEvent'; column does not allow nulls. INSERT fails.
			BEGIN 
				INSERT INTO [dbo].[ExperienceEvent] (ExperienceEventID, ExperienceEventDesc, CreateDate)
				VALUES (32, 'NetworkLatency', GETDATE())
			END 
	END CATCH

  GO

  
