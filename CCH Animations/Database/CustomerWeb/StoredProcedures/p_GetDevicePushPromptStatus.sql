/****** Object:  StoredProcedure [dbo].[p_GetDevicePushPromptStatus]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetDevicePushPromptStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetDevicePushPromptStatus]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-09/17
Description:
      Accepts a device ID, looks in the experience log to determine how many times the app has been
      launched on this device since they were last prompted to allow push notifications, checks the 
      PushNotificationPromptPeriod in ClientConfig and determines if the user needs to be prompted 
      again.
      
Declarations:
            
Execute:



Objects Listing:

Tables- dbo.Device
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-09-17 AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetDevicePushPromptStatus] (
	@DeviceID nvarchar(100)
)
as

BEGIN--proc
	DECLARE
		@PromptWaitPeriod int
		,@LastPromptDate datetime
		,@NumLogins int

-------------------------------------------------------
--If they allow push already, don't prompt them again
-------------------------------------------------------
	IF EXISTS (SELECT 1 from dbo.Device where DeviceID = @DeviceID and ClientAllowPushInd = 1)
	BEGIN --IF
		SELECT 0 AS PromptStatus
	END --IF
	ELSE
	BEGIN --ELSE
-------------------------------------------------------
--Otherwise Get the Wait Period & Last Prompt Date
-------------------------------------------------------
		SELECT
			@PromptWaitPeriod = ConfigValue
		FROM
			dbo.ClientConfig 
		WHERE 
			ConfigKey = 'PushNotificationPromptPeriod'
		
		SELECT
			@LastPromptDate = LastPushPromptDate -- date app was launched, more or less
		FROM
			dbo.Device
		WHERE
			DeviceID = @DeviceID
			
-----------------------------------------------------------
--Get Number of Logins since Last Prompt	
------------------------------------------------------------	
	IF @LastPromptDate IS NOT NULL	
		SELECT
			@NumLogins = COUNT(*)	
		FROM
			dbo.ExperienceLog el
			INNER JOIN ExperienceEvent ee
				ON el.ExperienceEventID = ee.ExperienceEventID
			INNER JOIN ExperienceDevice ed 
				ON el.ExperienceUserID = ed.ExperienceUserID
			INNER JOIN Device d 
				ON ed.DeviceID = d.DeviceID
		WHERE
			ed.DeviceID = @DeviceID
			AND el.CreateDate > @LastPromptDate
			AND ee.ExperienceEventDesc in ('StartAndroidApp','StartiOSApp')
-----------------------------------------------------------
--Return the status	
------------------------------------------------------------			
		IF @NumLogins >= @PromptWaitPeriod OR @LastPromptDate IS NULL
			SELECT 1 as PromptStatus
		ELSE
			SELECT 0 as PromptStatus			
		
	END --ELSE
		
	
END--proc

GO


 
