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
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_SendPushNotifications]
AS

BEGIN --proc
----------------------------------------------------------
--Create Results Table
-----------------------------------------------------------
	IF	object_id('tempdb..#NotificationResults') IS NOT NULL
	DROP TABLE #NotificationResults
	
	CREATE TABLE #NotificationResults (
	 StatusCode nvarchar(10)
	,StatusMessage nvarchar(100)
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
		,@URL nvarchar(200) = NULL
		,@EnglishDevices nvarchar(max)
		,@SpanishDevices nvarchar(max)
		,@CampaignID int
		,@ContentID int
				
--------------------------------------------------------
--Get Config Values
--------------------------------------------------------
	SELECT @ApplicationCode = ConfigValue
	FROM dbo.ClientConfig
	WHERE ConfigKey = 'PushApplicationCode'
	
	SELECT @ApplicationCode = ConfigValue
	FROM dbo.ClientConfig
	WHERE ConfigKey = 'PushApplicationGroup'
	
	SELECT @AccessToken = ConfigValue
	FROM CCH_FrontEnd2.dbo.InstanceConfig
	WHERE ConfigKey = 'PushAccessToken'
	
	SELECT @IsLive = ConfigValue
	FROM CCH_FrontEnd2.dbo.InstanceConfig
	WHERE ConfigKey = 'IsLive'
	
	
---------------------------------------------------------
--Pull All the people / devices /  required notifications
---------------------------------------------------------
	IF	object_id('tempdb..#MessageQueue') IS NOT NULL
	DROP TABLE #MessageQueue
	
	CREATE TABLE #MessageQueue(
	 CCHID int
	,CampaignID int
	,ContentID int
	,DeviceID int
	,OSNotificationText nvarchar(2000)
	,TextLocaleID int
	,UserDefaultLocaleID int
	)
		
--Logic if we're not in the Live environment
	IF ISNULL(@IsLive,'False') = 'False'
	BEGIN
		INSERT INTO #MessageQueue (CCHID,CampaignID,ContentID,DeviceID,OSNotificationText,TextLocaleID,UserDefaultLocaleID)
		SELECT
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
				ON uc.CCHID = ed.DeviceID
		WHERE
			cc.OSNotificationInd = 1
			AND ucp.OSBasedAlertInd = 1
			AND e.MemberMedicalID like '%TEST%' --If we're not in live, only pull test users
			AND ISNULL(ct.OSNotificationText, '') != ''
			AND ISNULL(uc.OSNotificationSentDate,'') = ''
	END
	ELSE --Logic if we're in the Live environment
	BEGIN
		INSERT INTO #MessageQueue (CCHID,CampaignID,ContentID,DeviceID,OSNotificationText,TextLocaleID,UserDefaultLocaleID)
		SELECT
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
				ON uc.CCHID = ed.DeviceID
		WHERE
			cc.OSNotificationInd = 1
			AND ucp.OSBasedAlertInd = 1
			AND ISNULL(ct.OSNotificationText, '') != ''
			AND ISNULL(uc.OSNotificationSentDate,'') = ''
	END
	--print @IsLive
	--select * from #MessageQueue
-----------------------------------------------------------
--Make sure we only send one translation of any given notification
-----------------------------------------------------------
	;with cteMultiLocale as
	(SELECT	CampaignID, CCHID, ContentID, COUNT(*) as NumLocales
     FROM #MessageQueue
     GROUP BY CampaignID, CCHID, ContentID HAVING COUNT(*) >1
     )
     DELETE #MessageQueue
     FROM #MessageQueue mq INNER JOIN cteMultiLocale ml
		ON mq.CampaignID = ml.CampaignID
		AND mq.ContentID = ml.ContentID
		AND mq.CCHID = ml.CCHID
     WHERE mq.TextLocaleID != mq.UserDefaultLocaleID
	
	--print @IsLive
	--select * from #MessageQueue
	
-----------------------------------------------
--Create a table with just the CLR Calls (one CLR call per campaign content / translation)
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
	
------------------------------------------------
--Call the CLR 
------------------------------------------------
	WHILE (SELECT * FROM #CLRCalls) > 0
	BEGIN --while
		SELECT TOP 1
			@CampaignID = CampaignID
			,@ContentID = ContentID
			,@MessageText = OSNotificationText
			,@TextLocaleID = TextLocaleID
		FROM
			#CLRCalls
			
		IF @TextLocaleID = 1
		BEGIN
-----------------------------------------------
--STUFF all the English Devices for that campaign / content
-----------------------------------------------
		;WITH cteSTUFF AS (
			SELECT DISTINCT DeviceID 
			FROM #MessageQueue 
			WHERE UserDefaultLocaleID = 1
				AND CampaignID = @CampaignID
				AND ContentID = @ContentID)
		
		SELECT 
			@EnglishDevices = '['+
				STUFF((SELECT ',' + DeviceID
					FROM cteSTUFF
					FOR
					XML PATH ('')
					),1,1,'') + ']'
		
			INSERT INTO #NotificationResults
				(StatusCode
				,StatusMessage)
			SELECT
				StatusCode
				,StatusMessaage
			FROM
				CCH_ReferenceData.dbo.SendPushNotifcations(
				@ApplicationCode
				,@ApplicationGroup
				,@AccessToken
				,@SendDate
				,@MessageText
				,@URL
				,@EnglishDevices)
		END
		
		IF @TextLocaleID = 2
-----------------------------------------------
--STUFF all the English Devices for that campaign / content
-----------------------------------------------
		BEGIN
		
			;WITH cteSTUFF2 AS (
			SELECT DISTINCT DeviceID 
			FROM #MessageQueue 
			WHERE UserDefaultLocaleID = 2
				AND CampaignID = @CampaignID
				AND ContentID = @ContentID)
			
			SELECT 
				@SpanishDevices = '['+
				STUFF((SELECT ',' + DeviceID
					FROM cteSTUFF2
					FOR
					XML PATH ('')
					),1,1,'') + ']'
				
			INSERT INTO #NotificationResults
				(StatusCode
				,StatusMessage)
			SELECT
				StatusCode
				,StatusMessaage
			FROM
				CCH_ReferenceData.dbo.SendPushNotifcations(
				@ApplicationCode
				,@ApplicationGroup
				,@AccessToken
				,@SendDate
				,@MessageText
				,@URL
				,@SpanishDevices)
		END
		
-----------------------------------------------------------------------
--Process the results / set the status
-----------------------------------------------------------------------
		UPDATE 
			UserContent
		SET 
			OSNotificationSentDate = CASE WHEN nr.StatusCode = '200' THEN GETDATE() END
			,OSNotificationStatusDesc = 
				CASE WHEN nr.StatusCode = '200' THEN 'Success'
				ELSE 'Retry' END
		FROM
			UserContent uc
			INNER JOIN #MessageQueue mq
				ON uc.CampaignID = mq.CampaignID
				AND uc.ContentID = mq.ContentID
				AND uc.CCHID = mq.CCHID
		WHERE
			mq.CampaignID = @CampaignID
			AND mq.ContentID = @ContentID
			AND mq.OSNotificationText = @MessageText
			AND mq.TextLocaleID = @TextLocaleID
		
		
		DELETE FROM #CLRCalls where 
			CampaignID = @CampaignID
			AND ContentID = @ContentID
			AND OSNotificationText = @MessageText
			AND TextLocaleID = @TextLocaleID
			
	END --while
 
  END

GO


/*---Testing----
TRUNCATE TABLE #NotificationResults 
INSERT INTO #NotificationResults
	(CampaignID
		,ContentID
		,CCHID
		,MessageID
		,ErrorCode
		,ErrorMessage
		,Status
		,MoreInfo
		,StatusDate)
VALUES(
	13,
	16,
	57020,
	1,
	NULL,
	NULL,
	'queued',
	NULL,
	GETDATE())
	
INSERT INTO #NotificationResults
	(CampaignID
		,ContentID
		,CCHID
		,MessageID
		,ErrorCode
		,ErrorMessage
		,Status
		,MoreInfo
		,StatusDate)
VALUES(
	13,
	16,
	63841,
	1,
	30007,
	NULL,
	'7',
	NULL,
	GETDATE())
	------------*/

