/****** Object:  StoredProcedure [dbo].[p_CreateCampaignUserContent]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_CreateCampaignUserContent]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_CreateCampaignUserContent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Inserts Content for all users targeted by a specific campaign into UserContent. Makes
      sure all users in the Campaign have their notification preferences set, and if not
      inserts the members into UserContentPreference with default values for each type of 
      notification
      
Declarations:
            
Execute:
      exec p_CreateCampaignUserContent
		@CampaignID = 1

Objects Listing:

Tables- dbo.UserContent 
dbo.CampaignContent
dbo.CampaignMember
dbo.UserContentPreference

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
2015-08-18  AS       Updated to removed insert/update on UserContentPreference (which is now maintained via Enrollments processing)
2015-09-04  AS	     Updated to add notification fields (PC-435)
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_CreateCampaignUserContent] (
	@CampaignID int,
	@Comment nvarchar(250) = NULL
)
as

BEGIN
		
	INSERT UserContent(
		CCHID
		,CampaignID
		,ContentID
		,ContentStatusID
		,ContentStatusChangeDate
		,ContentSavingsAmt
		,NotificationSentDate
		,SMSNotificationSentDate
		,SMSNotificationStatusDesc
		,OSNotificationSentDate
		,MemberContentDataText
		,UserContentCommentText
		,CreateDate)
	SELECT
		e.CCHID
		,e.CampaignID
		,cc.ContentID
		,cts.ContentStatusID
		,GETDATE()
		,NULL
		,NULL
		,NULL
		,NULL
		,NULL
		, ''
		, @Comment
		,GETDATE()
	FROM	
		dbo.CampaignMember e
		INNER JOIN CampaignContent cc
			on e.CampaignID = CC.CampaignID
		INNER JOIN Content c
			on cc.ContentID = c.ContentID
		INNER JOIN ContentTypeState cts
			on c.ContentTypeID = cts.ContentTypeID
		WHERE
			cts.InitialStateInd = 1
			AND e.CampaignID = @CampaignID
			
----------------------------------------------------------------
--Make sure everyone has their notification preferences set
-----------------------------------------------------------------
--AS 2015-08-18 - this is no longer needed - table is maintained during Enrollments Processing
/*		SELECT 
			CampaignID
			,CCHID
			,0 as DeleteFlag
		INTO #Members
		FROM
			dbo.CampaignMember
		WHERE
			CampaignID = @CampaignID
			
		UPDATE #Members
		SET DeleteFlag = 1
		FROM
			#Members m
			INNER JOIN dbo.UserContentPreference ucp
				on m.CCHID = ucp.CCHID
				
		DELETE #Members 
		WHERE DeleteFlag = 1
			
		INSERT INTO dbo.UserContentPreference (
			CCHID
			,EmailInd
			,SMSInd
			,OSBasedAlertInd
			,DefaultLocaleID
			,PreferredContactPhoneNum
			,CreateDate
			,LastUpdateDate)
		SELECT
			m.CCHID
			,e.OptInEmailAlerts
			,e.OptInTextMsgAlerts
			,1
			,1
			,COALESCE(e.Phone, e.MobilePhone, e.Member_Phone_Home, e.Subscriber_Phone_Home)
			,GETDATE()
			,GETDATE()
		FROM
			#Members m
			INNER JOIN dbo.Enrollments e
				on m.CCHID = e.CCHID
*/
END
 

GO

 
