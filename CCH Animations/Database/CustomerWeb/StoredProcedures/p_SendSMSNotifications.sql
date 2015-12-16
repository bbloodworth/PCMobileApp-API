/****** Object:  StoredProcedure [dbo].[p_SendSMSNotifications]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_SendSMSNotifications]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_SendSMSNotifications]
GO

/****** Object:  StoredProcedure [dbo].[p_SendSMSNotifications]    Script Date: 07/10/2015 13:27:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-09-03
Description:
     Checks the UserContent Queue and Identifies content requiring sms notification that has not yet been 
     successfully sent.  
      
Declarations:
      
Execute:
      exec p_SendSMSNotifications
	
Objects Listing:

Tables- 
	dbo.UserContent
	dbo.Content
	dbo.UserContentPreferences
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-08-19  AS       Created
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_SendSMSNotifications]
AS

BEGIN --proc
----------------------------------------------------------
--Create Results Table
-----------------------------------------------------------
	IF	object_id('tempdb..#NotificationResults') IS NOT NULL
	DROP TABLE #NotificationResults
	
	CREATE TABLE #NotificationResults (
	CampaignID int
	,ContentID int
	,CCHID int
	,MessageID nvarchar(50)
	,ErrorCode int
	,ErrorMessage nvarchar(500)
	,Status nvarchar(100)
	,MoreInfo nvarchar(200)
	,StatusDate DateTime
	)
----------------------------------------------------------
--Declarations
-----------------------------------------------------------
	DECLARE
		@IsLive nvarchar(10)
		,@AccountID nvarchar(50)
		,@AuthToken nvarchar(50)
		,@FromPhone nvarchar(20)
		,@CCHID int
		,@CampaignID int
		,@ContentID int
		,@MediaURL nvarchar(200) = NULL
		,@StatusCallback nvarchar(200) = NULL
		,@MemberData nvarchar(max)
		
--------------------------------------------------------
--Get Config Values
--------------------------------------------------------
	SELECT @AccountID = ConfigValue
	FROM CCH_FrontEnd2.dbo.InstanceConfig
	WHERE ConfigKey = 'SMSAccountID'
	
	SELECT @AuthToken = ConfigValue
	FROM CCH_FrontEnd2.dbo.InstanceConfig
	WHERE ConfigKey = 'SMSAuthToken'
	
	SELECT @FromPhone = ConfigValue
	FROM CCH_FrontEnd2.dbo.InstanceConfig
	WHERE ConfigKey = 'SMSFromPhone'
	
	SELECT @IsLive = ConfigValue 
	FROM CCH_FrontEnd2.dbo.InstanceConfig
	WHERE ConfigKey = 'IsLive'
---------------------------------------------------------
--Pull All the people / required notifications
---------------------------------------------------------
	IF	object_id('tempdb..#MessageQueue') IS NOT NULL
	DROP TABLE #MessageQueue
	
	CREATE TABLE #MessageQueue(
	CCHID int
	,CampaignID int
	,ContentID int
	,Name nvarchar(100)
	,PhoneNum nvarchar(50)
	,SMSNotificationText nvarchar(2000)
	,JSONString nvarchar(2000)
	,TextLocaleID int
	,UserDefaultLocaleID int
	)
		
--Logic if we're not in the Live environment
	IF ISNULL(@IsLive,'False') = 'False'
	BEGIN
		INSERT INTO #MessageQueue (CCHID,CampaignID,ContentID,Name,PhoneNum,SMSNotificationText,TextLocaleID,UserDefaultLocaleID)
		SELECT
			uc.CCHID
			,uc.CampaignID
			,uc.ContentID
			,LTRIM(RTRIM(CCH_ReferenceData.dbo.fn_GetProperCase(e.FirstName))) + ' ' + LTRIM(RTRIM(CCH_ReferenceData.dbo.fn_GetProperCase(e.LastName))) as Name
			,coalesce(NULLIF(e.MobilePhone,''),ucp.PreferredContactPhoneNum) as PhoneNum --we don't technically know this is mobile
			,ct.SMSNotificationText
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
				on uc.CCHID = e.CCHID
		WHERE
			cc.SMSNotificationInd = 1
			AND ucp.SMSInd = 1
			AND e.MemberMedicalID like '%TEST%' --If we're not in live, only pull test users
			AND ISNULL(coalesce(NULLIF(e.MobilePhone,''),ucp.PreferredContactPhoneNum),'') != ''
			AND ISNULL(ct.SMSNotificationText, '') != ''
			AND ISNULL(uc.SMSNotificationSentDate,'') = ''
			AND ISNULL(uc.SMSNotificationStatusDesc,'Not Sent') in ('Not Sent', 'Retry')
	END
	ELSE --Logic if we're in the Live environment
	BEGIN
		INSERT INTO #MessageQueue (CCHID,CampaignID,ContentID,Name,PhoneNum,SMSNotificationText,TextLocaleID,UserDefaultLocaleID)
		SELECT
			uc.CCHID
			,uc.CampaignID
			,uc.ContentID
			,LTRIM(RTRIM(CCH_ReferenceData.dbo.fn_GetProperCase(e.FirstName))) + ' ' + LTRIM(RTRIM(CCH_ReferenceData.dbo.fn_GetProperCase(e.LastName))) as Name
			,coalesce(NULLIF(e.MobilePhone,''),ucp.PreferredContactPhoneNum) as PhoneNum --we don't technically know this is mobile
			,ct.SMSNotificationText
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
				on c.ContentID = ct.ContentID
			INNER JOIN UserContentPreference ucp 
				ON uc.CCHID = ucp.CCHID
			INNER JOIN Enrollments e
				on uc.CCHID = e.CCHID
		WHERE
			cc.SMSNotificationInd = 1
			AND ucp.SMSInd = 1
			AND ISNULL(coalesce(NULLIF(e.MobilePhone,''),ucp.PreferredContactPhoneNum),'') != ''
			AND ISNULL(ct.SMSNotificationText, '') != ''
			AND ISNULL(uc.SMSNotificationSentDate,'') = ''
			AND ISNULL(uc.SMSNotificationStatusDesc,'Not Sent') in ('Not Sent', 'Retry')
	END
	--print @IsLive
	--select * from #MessageQueue
-----------------------------------------------------------
--Make sure we only send one translation of any given text
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
	
	print @IsLive
	select * from #MessageQueue
--------------------------------------------------
--Create the JSON strings
---------------------------------------------------
	UPDATE #MessageQueue
	SET JSONString = 
	'{' +
		'"CCHID": ' + ISNULL(LTRIM(RTRIM(CONVERT(CHAR,CCHID))),'null') + 
		',"CampaignID": ' + ISNULL(LTRIM(RTRIM(CONVERT(CHAR,CampaignID))),'null') + 
		',"ContentID": ' + ISNULL(LTRIM(RTRIM(CONVERT(CHAR,ContentID))),'null') + 
		',"ToPhone": "' + ISNULL(LTRIM(RTRIM(CONVERT(CHAR,PhoneNum))),'null') + '"' +
		',"MemberName": "' + ISNULL(LTRIM(RTRIM(Name)),'null') + '"' +
		',"MessageBody": "' + ISNULL(LTRIM(RTRIM(SMSNotificationText)),'null') + '"' +
	'}'
-----------------------------------------------
--STUFF all the member data
-----------------------------------------------
	;WITH cteSTUFF AS (SELECT DISTINCT JSONString FROM #MessageQueue)
		
	SELECT 
	@MemberData = '['+
		STUFF((SELECT ',' + JSONString
				FROM cteSTUFF
				FOR
				XML PATH ('')
				),1,1,'') + ']'
	
	select * from #MessageQueue				
	print @MemberData
------------------------------------------------
--Call the CLR
------------------------------------------------
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
	SELECT
		CampaignID
		,ContentID
		,CCHID
		,MessageID
		,ErrorCode
		,ErrorMessage
		,Status
		,MoreInfo
		,StatusDate
	FROM
		CCH_ReferenceData.dbo.SendSMSMessage(
		@AccountID
		,@AuthToken
		,@FromPhone
		,@MediaURL
		,@StatusCallback
		,@MemberData)

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
	
 -----------------------------------------------------------------------
 --Process the results / set the status
 -----------------------------------------------------------------------
	UPDATE 
		UserContent
	SET 
		SMSNotificationSentDate = CASE WHEN nr.Status = 'queued' AND ISNULL(nr.ErrorCode,'') = '' THEN GETDATE() END
		,SMSNotificationStatusDesc = 
			CASE WHEN nr.Status = 'queued' AND ISNULL(nr.ErrorCode,'') = '' THEN 'Success'
			WHEN nr.Status != 'queued' AND nr.ErrorCode in (30001, 30002, 30003, 30008) THEN 'Retry'
			WHEN nr.Status != 'queued' AND nr.ErrorCode in (30004, 30005, 30006, 30007, 30009) THEN 'Fail'
			ELSE 'Retry' END
	FROM
		UserContent uc
		INNER JOIN #NotificationResults nr
			ON uc.CampaignID = nr.CampaignID
			AND uc.ContentID = nr.ContentID
			AND uc.CCHID = nr.CCHID
  END

GO


