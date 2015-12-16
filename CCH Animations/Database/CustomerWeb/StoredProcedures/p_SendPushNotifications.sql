/****** Object:  StoredProcedure [dbo].[p_SendPushNotifications]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_SendPushNotifications]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_SendPushNotifications]
GO

/****** Object:  StoredProcedure [dbo].[p_SendPushNotifications]    Script Date: 07/10/2015 13:27:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-09-03
Description:
     Checks CampaignContent and Identifies content requiring push notifications that have not yet been 
     sent.  
      
Declarations:
      
Execute:
      exec p_SendPushNotifications
	
Objects Listing:

Tables- 
	dbo.CampaignContent
	dbo.Content
	dbo.ContentTranslation
	dbo.Device
	dbo.ExperienceDevice
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-08-19  AS       Created
2015-10-13  AS		 Updated to include different logic for Campaign 0 (anonymous notifications)
2015-10-29  AS		 Updated to make sure content is not expired.
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_SendPushNotifications]
AS

BEGIN --(1) sproc
----------------------------------------------------------
--Create Results Table / Devices table
-----------------------------------------------------------
	IF	object_id('tempdb..#NotificationResults') IS NOT NULL
	DROP TABLE #NotificationResults
	
	CREATE TABLE #NotificationResults (
	 StatusCode nvarchar(10)
	,StatusMessage nvarchar(100)
	)
	
	IF	object_id('tempdb..#Devices') IS NOT NULL
	DROP TABLE #Devices
	
	CREATE TABLE #Devices (
	 id int identity (1,1)
	,DeviceID nvarchar(100)
	)
----------------------------------------------------------
--Declarations
-----------------------------------------------------------
	DECLARE
		@IsLive nvarchar(10)
		,@ApplicationCode nvarchar(20)
		,@ApplicationGroup nvarchar(25) = ''
		,@AccessToken nvarchar(100)
		,@SendDate nvarchar(25) = 'Now' --Format should be 'YYYYMMDD HH:MM:SS' (24hr clock) or 'Now', default to 'Now' for now
		,@MessageText nvarchar(2000) = NULL
		,@TextLocaleID int
		,@URL nvarchar(200) = ''
		,@Devices nvarchar(max) = ''
		,@CampaignID int
		,@ContentID int
				
--------------------------------------------------------
--Get Config Values
--------------------------------------------------------
	SELECT @ApplicationCode = ConfigValue
	FROM dbo.ClientConfig
	WHERE ConfigKey = 'PushApplicationCode'
	
	SELECT @ApplicationGroup = ConfigValue
	FROM dbo.ClientConfig
	WHERE ConfigKey = 'PushApplicationGroup'
	
	SELECT @AccessToken = ConfigValue
	FROM CCH_FrontEnd2.dbo.InstanceConfig
	WHERE ConfigKey = 'PushAccessToken'
	
	SELECT @IsLive = ConfigValue
	FROM CCH_FrontEnd2.dbo.InstanceConfig
	WHERE ConfigKey = 'IsLive'
	
	
--------------------------------------------------------
--Pull All the people / devices /  required notifications
---------------------------------------------------------
	IF	object_id('tempdb..#MessageQueue') IS NOT NULL
	DROP TABLE #MessageQueue
	
	CREATE TABLE #MessageQueue(
	 CCHID int
	,CampaignID int
	,ContentID int
	,DeviceID nvarchar(100)
	,OSNotificationText nvarchar(2000)
	,TextLocaleID int
	,UserDefaultLocaleID int
	)
		
--Logic if we're not in the Live environment
	IF ISNULL(@IsLive,'False') = 'False'
	BEGIN --(2) env not live
		INSERT INTO #MessageQueue (CCHID,CampaignID,ContentID,DeviceID,OSNotificationText,TextLocaleID,UserDefaultLocaleID)
		SELECT DISTINCT
			uc.CCHID
			,uc.CampaignID
			,uc.ContentID
			,ed.DeviceID
			,ct.OSNotificationText
			,ct.LocaleID
			,ucp.DefaultLocaleID
		FROM
			UserContent uc
			INNER JOIN CampaignContent cc 
				ON uc.CampaignID = cc.CampaignID 
				AND uc.ContentID = cc.ContentID
			INNER JOIN Content c 
				ON cc.ContentID = c.ContentID
			INNER JOIN ContentTranslation ct
				ON c.ContentID = ct.ContentID
			INNER JOIN UserContentPreference ucp 
				ON uc.CCHID = ucp.CCHID
			INNER JOIN Enrollments e
				ON uc.CCHID = e.CCHID
			INNER JOIN ExperienceDevice ed
				ON uc.CCHID = ed.CCHID
		WHERE
			cc.OSNotificationInd = 1
			AND ucp.OSBasedAlertInd = 1
			AND e.MemberMedicalID like '%TEST%' --If we're not in live, only pull test users
			AND ISNULL(ct.OSNotificationText, '') != ''
			AND ISNULL(uc.OSNotificationSentDate,'') = ''
			AND ISNULL(uc.OSNotificationStatusDesc,'Retry') = 'Retry'
			AND cc.CampaignID != 0 -- campaign 0 notifications will be handled differently
			AND GETDATE() >= ISNULL(cc.ActivationDate, DATEADD(day,-1,GETDATE())) --make sure content is not expired
			AND GETDATE() < ISNULL(cc.ExpirationDate, DATEADD(day,1,GETDATE()))
	END --(2) env not live
	ELSE --Logic if we're in the Live environment
	BEGIN --(3) live
		INSERT INTO #MessageQueue (CCHID,CampaignID,ContentID,DeviceID,OSNotificationText,TextLocaleID,UserDefaultLocaleID)
		SELECT DISTINCT
			uc.CCHID
			,uc.CampaignID
			,uc.ContentID
			,ed.DeviceID
			,ct.OSNotificationText
			,ct.LocaleID
			,ucp.DefaultLocaleID
		FROM
			UserContent uc
			INNER JOIN CampaignContent cc 
				ON uc.CampaignID = cc.CampaignID 
				AND uc.ContentID = cc.ContentID
			INNER JOIN Content c 
				ON cc.ContentID = c.ContentID
			INNER JOIN ContentTranslation ct
				ON c.ContentID = ct.ContentID
			INNER JOIN UserContentPreference ucp 
				ON uc.CCHID = ucp.CCHID
			INNER JOIN Enrollments e
				ON uc.CCHID = e.CCHID
			INNER JOIN ExperienceDevice ed
				ON uc.CCHID = ed.CCHID
		WHERE
			cc.OSNotificationInd = 1
			AND ucp.OSBasedAlertInd = 1
			AND ISNULL(ct.OSNotificationText, '') != ''
			AND ISNULL(uc.OSNotificationSentDate,'') = ''
			AND ISNULL(uc.OSNotificationStatusDesc,'Retry') = 'Retry'
			AND cc.CampaignID != 0 -- campaign 0 notifications will be handled differently
			AND GETDATE() >= ISNULL(cc.ActivationDate, DATEADD(day,-1,GETDATE())) --make sure content is not expired
			AND GETDATE() < ISNULL(cc.ExpirationDate, DATEADD(day,1,GETDATE()))
	END --(3) live
	----------------------------------------------
	--debug:
	-----------------------------------------------
	print @IsLive
	print 'MessageQ'
	select * from #MessageQueue
	----------------------------------------------
-----------------------------------------------------------
--Make sure we only send one translation of any given notification
-----------------------------------------------------------
	;with cteMultiLocale as
	(SELECT	CampaignID, CCHID, ContentID, DeviceID, COUNT(*) as NumLocales
     FROM #MessageQueue
     GROUP BY CampaignID, CCHID, ContentID, DeviceID HAVING COUNT(*) >1
     )
     DELETE #MessageQueue
     FROM #MessageQueue mq INNER JOIN cteMultiLocale ml
		ON mq.CampaignID = ml.CampaignID
		AND mq.ContentID = ml.ContentID
		AND mq.CCHID = ml.CCHID
		AND mq.DeviceID = ml.DeviceID
     WHERE mq.TextLocaleID != mq.UserDefaultLocaleID
	
	----------------------------------------------
	--debug:
	-----------------------------------------------
	print 'MessageQ dedup'
	select * from #MessageQueue
	----------------------------------------------
	
-----------------------------------------------
--Create a table with just the CLR Calls (one CLR call per campaign content / translation / 1000 Device IDs)
-----------------------------------------------
	IF	object_id('tempdb..#CLRCalls') IS NOT NULL
	DROP TABLE #CLRCalls
	
	SELECT	DISTINCT	
		CampaignID
		,ContentID
		,OSNotificationText
		,TextLocaleID
	INTO
		#CLRCalls
	FROM
		#MessageQueue

-----------------------------------------------
--Add in Campaign 0 Calls
-----------------------------------------------
	INSERT INTO #CLRCalls
	SELECT
		cc.CampaignID
		,cc.ContentID
		,ISNULL(ct.OSNotificationText, '')
		,ct.LocaleID 
	FROM
		dbo.CampaignContent cc
		INNER JOIN Content c 
			ON cc.ContentID = c.ContentID
		INNER JOIN ContentTranslation ct
			on c.ContentID = ct.ContentID
	WHERE
		cc.CampaignID = 0
		AND cc.OSNotificationInd = 1
		AND ct.LocaleID = 1 --Campaign 0 Calls will only be sent in English for now
		AND ISNULL(cc.OSNotificationSentDate,'') = ''
		AND ISNULL(cc.OSNotificationStatusDesc,'Retry') = 'Retry'
		AND GETDATE() >= ISNULL(cc.ActivationDate, DATEADD(day,-1,GETDATE())) --make sure content is not expired
		AND GETDATE() < ISNULL(cc.ExpirationDate, DATEADD(day,1,GETDATE()))

	
	----------------------------------------------
	--debug:
	-----------------------------------------------
	print 'CLRCalls'
	select * from #CLRCalls
	----------------------------------------------
------------------------------------------------
--Call the CLR 
------------------------------------------------
	WHILE (SELECT COUNT(*) FROM #CLRCalls) > 0
	BEGIN --(4) while clr call
		SELECT TOP 1
			@CampaignID = CampaignID
			,@ContentID = ContentID
			,@MessageText = OSNotificationText
			,@TextLocaleID = TextLocaleID
		FROM
			#CLRCalls
	----------------------------------------------
	--debug:
	-----------------------------------------------
	print 'Variables for call'
	print @CampaignID
	print @ContentID
	print @MessageText
	print @TextLocaleID
	----------------------------------------------
-----------------------------------------------
--Load all the Devices for that campaign / content / Language into the #Devices table
-----------------------------------------------
		IF @CampaignID != 0
		BEGIN --(5) not campaign 0 DeviceIF
			INSERT INTO #Devices (DeviceID)
			SELECT DISTINCT DeviceID
			FROM #MessageQueue
			WHERE TextLocaleID = @TextLocaleID
			AND CampaignID = @CampaignID
			AND ContentID = @ContentID
	----------------------------------------------
	--debug:
	-----------------------------------------------
			print 'Devices if not Campaign 0'
			select * from #Devices
	----------------------------------------------	
-------------------------------------------------
--Batch up the Device IDs
-------------------------------------------------
			DECLARE @MaxID int = 0
		
			WHILE (SELECT COUNT(*) FROM #Devices) > 0
			BEGIN -- (6) while Devices
		
				SET @MaxID = @MaxID + 1000
-----------------------------------------------
--STUFF all the Devices for that campaign / content / language
-----------------------------------------------
				;WITH cteSTUFF AS (
					SELECT DISTINCT '''' + DeviceID + '''' as DeviceID
					FROM #Devices
					WHERE id < @MaxID
					)
		
				SELECT 
				@Devices = '['+
						STUFF((SELECT ',' + DeviceID
							FROM cteSTUFF
							FOR
							XML PATH ('')
							),1,1,'') + ']'
							
			----------------------------------------------
			--debug:
			-----------------------------------------------
				print '@Devices if Campaign is not 0'
				print @Devices
			----------------------------------------------		
				TRUNCATE TABLE #NotificationResults
				
				INSERT INTO #NotificationResults
					(StatusCode
					,StatusMessage)
				SELECT
					StatusCode
					,StatusMessage
				FROM
					CCH_ReferenceData.dbo.SendPushMessage(
					@ApplicationCode
					,@ApplicationGroup
					,@AccessToken
					,@SendDate
					,@MessageText
					,@URL
					,@Devices)

-----------------------------------------------------------------------
--Process the results / set the status
-----------------------------------------------------------------------
				IF (SELECT StatusCode from #NotificationResults) = '200'
				BEGIN--(7) success	
					;WITH cteDevices AS (
					SELECT DISTINCT DeviceID 
					FROM #Devices
					WHERE id < @MaxID
					)
					
					UPDATE 
						UserContent
					SET 
						OSNotificationSentDate = GETDATE()
						,OSNotificationStatusDesc = 'Success'
					FROM
						UserContent uc
						INNER JOIN #MessageQueue mq
							ON uc.CampaignID = mq.CampaignID
							AND uc.ContentID = mq.ContentID
							AND uc.CCHID = mq.CCHID 
						INNER JOIN cteDevices d
							ON mq.DeviceID = d.DeviceID
					WHERE
						mq.CampaignID = @CampaignID
						AND mq.ContentID = @ContentID
						AND mq.OSNotificationText = @MessageText
						AND mq.TextLocaleID = @TextLocaleID
				END--(7) success
				ELSE	
				BEGIN --(8) fail
					;WITH cteDevices2 AS (
					SELECT DISTINCT DeviceID 
					FROM #Devices
					WHERE id < @MaxID
					)
					
					UPDATE 
						UserContent
					SET 
						OSNotificationStatusDesc = 'Retry'
					FROM
						UserContent uc
						INNER JOIN #MessageQueue mq
							ON uc.CampaignID = mq.CampaignID
							AND uc.ContentID = mq.ContentID
							AND uc.CCHID = mq.CCHID
						INNER JOIN cteDevices2 d
							ON mq.DeviceID = d.DeviceID
					WHERE
						mq.CampaignID = @CampaignID
						AND mq.ContentID = @ContentID
						AND mq.OSNotificationText = @MessageText
						AND mq.TextLocaleID = @TextLocaleID
				END --(8) fail
				
				DELETE #Devices where id <= @MaxID	
			END --(6) while Devices
		END -- (5) not campaign 0 DeviceIf
		ELSE 
		BEGIN -- (9) Campaign 0 call
			SET @Devices = ''
	--------------------------
	--debug:
	-----------------------------
			print '@Devices if Campaign 0'
			print @Devices
				
			TRUNCATE TABLE #NotificationResults
				
			INSERT INTO #NotificationResults
				(StatusCode
				,StatusMessage)
			SELECT
				StatusCode
				,StatusMessage
			FROM
				CCH_ReferenceData.dbo.SendPushMessage(
				@ApplicationCode
				,@ApplicationGroup
				,@AccessToken
				,@SendDate
				,@MessageText
				,@URL
				,@Devices)
				
			IF (SELECT StatusCode from #NotificationResults) = '200'
				UPDATE 
					CampaignContent
				SET 
					OSNotificationSentDate = GETDATE()
					,OSNotificationStatusDesc = 'Success'
				FROM
					CampaignContent cc
					INNER JOIN Content c
						ON cc.ContentID = c.ContentID
					INNER JOIN ContentTranslation ct
						ON c.ContentID = ct.ContentID
				WHERE
					cc.CampaignID = @CampaignID
					AND cc.ContentID = @ContentID
					AND ISNULL(ct.OSNotificationText,'') = @MessageText
					AND ct.LocaleID = @TextLocaleID
			ELSE	
			
				UPDATE 
					CampaignContent
				SET 
					OSNotificationStatusDesc = 'Retry'
				FROM
				CampaignContent cc
					INNER JOIN Content c
						ON cc.ContentID = c.ContentID
					INNER JOIN ContentTranslation ct
						ON c.ContentID = ct.ContentID
				WHERE
					cc.CampaignID = @CampaignID
					AND cc.ContentID = @ContentID
					AND ISNULL(ct.OSNotificationText,'') = @MessageText
					AND ct.LocaleID = @TextLocaleID	
		END --(9) Campaign 0 call
		
		DELETE FROM #CLRCalls where 
		CampaignID = @CampaignID
		AND ContentID = @ContentID
		AND OSNotificationText = @MessageText
		AND TextLocaleID = @TextLocaleID
		
	END --(4) while
 
  END -- (1) sproc

GO
