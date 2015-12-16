/****** Object:  StoredProcedure [dbo].[p_CreateConstantContactSMSFile]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_CreateConstantContactSMSFile]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_CreateConstantContactSMSFile]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Create Constant Contact File
      
Declarations:
            
Execute:
      exec p_CreateConstantContactSMSFile
		@CampaignID = 1

Objects Listing:

Tables- dbo.Campaign
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
2015-01-20	AS		 Updated to include employee savings
2015-02-05  AS		 Updated to accept month start date to calculate savings.
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_CreateConstantContactSMSFile] (
	@CampaignID int
)
as

BEGIN

	DECLARE
		@EmployerID int
		,@ContentID int = 0
		
	SELECT
		@EmployerID =EmployerID
	FROM 
		ControlData
		
	SELECT
		@ContentID = uc.ContentID
	FROM
		UserContent uc
		INNER JOIN Content c
			on uc.ContentID = c.ContentID
		INNER JOIN ContentType ct
			on c.ContentTypeID = ct.ContentTypeID
	WHERE
		uc.CampaignID = @CampaignID
		AND c.IntroContentInd = 0
		AND ct.ContentTypeDesc in ('Animation', 'Video')
		
--------------------------------------------------------------------
--This pulls everyone in the campaign who wants SMS notifications
--------------------------------------------------------------------
	SELECT DISTINCT
		e.CCHID,
		e.FirstName,
		e.LastName,
		rm.MobilePhone,
		cm.Savings,
		'N' as KeyClient,
		CASE WHEN c.AuthRequiredInd = 1 THEN	
			'https://pmedia.clearcosthealth.com/?cid='+LTRIM(RTRIM(convert(char,@EmployerID)))+'|'+LTRIM(RTRIM(convert(char,@CampaignID)))+'|'+LTRIM(RTRIM(convert(char,@ContentID)))
		ELSE
			'https://pmedia.clearcosthealth.com/?cid='+LTRIM(RTRIM(convert(char,@EmployerID)))+'|'+LTRIM(RTRIM(convert(char,@CampaignID)))+'|'+LTRIM(RTRIM(convert(char,@ContentID)))+'|'+LTRIM(RTRIM(convert(char,e.CCHID)))
		END as ProcessingURL,
		CASE WHEN c.AuthRequiredInd = 1 THEN	
			'https://dmedia.clearcosthealth.com/?cid='+LTRIM(RTRIM(convert(char,@EmployerID)))+'|'+LTRIM(RTRIM(convert(char,@CampaignID)))+'|'+LTRIM(RTRIM(convert(char,@ContentID)))
		ELSE
			'https://dmedia.clearcosthealth.com/?cid='+LTRIM(RTRIM(convert(char,@EmployerID)))+'|'+LTRIM(RTRIM(convert(char,@CampaignID)))+'|'+LTRIM(RTRIM(convert(char,@ContentID)))+'|'+LTRIM(RTRIM(convert(char,e.CCHID)))
		END as DevURL,
		CASE WHEN c.AuthRequiredInd = 1 THEN	
			'https://amedia.clearcosthealth.com/?cid='+LTRIM(RTRIM(convert(char,@EmployerID)))+'|'+LTRIM(RTRIM(convert(char,@CampaignID)))+'|'+LTRIM(RTRIM(convert(char,@ContentID)))
		ELSE
			'https://amedia.clearcosthealth.com/?cid='+LTRIM(RTRIM(convert(char,@EmployerID)))+'|'+LTRIM(RTRIM(convert(char,@CampaignID)))+'|'+LTRIM(RTRIM(convert(char,@ContentID)))+'|'+LTRIM(RTRIM(convert(char,e.CCHID)))
		END as AlphaURL,
		CASE WHEN c.AuthRequiredInd = 1 THEN	
			'https://a2media.clearcosthealth.com/?cid='+LTRIM(RTRIM(convert(char,@EmployerID)))+'|'+LTRIM(RTRIM(convert(char,@CampaignID)))+'|'+LTRIM(RTRIM(convert(char,@ContentID)))
		ELSE
			'https://a2media.clearcosthealth.com/?cid='+LTRIM(RTRIM(convert(char,@EmployerID)))+'|'+LTRIM(RTRIM(convert(char,@CampaignID)))+'|'+LTRIM(RTRIM(convert(char,@ContentID)))+'|'+LTRIM(RTRIM(convert(char,e.CCHID)))
		END as Alpha2URL,
		CASE WHEN c.AuthRequiredInd = 1 THEN	
			'https://media.clearcosthealth.com/?cid='+LTRIM(RTRIM(convert(char,@EmployerID)))+'|'+LTRIM(RTRIM(convert(char,@CampaignID)))+'|'+LTRIM(RTRIM(convert(char,@ContentID)))
		ELSE
			'https://media.clearcosthealth.com/?cid='+LTRIM(RTRIM(convert(char,@EmployerID)))+'|'+LTRIM(RTRIM(convert(char,@CampaignID)))+'|'+LTRIM(RTRIM(convert(char,@ContentID)))+'|'+LTRIM(RTRIM(convert(char,e.CCHID)))
		END as LiveURL
	INTO #SMSs
	FROM
		dbo.UserContent uc
		INNER JOIN CampaignMember cm
			ON uc.CCHID = cm.CCHID
			AND uc.CampaignID = cm.CampaignID
		INNER JOIN dbo.Enrollments e
			ON uc.CCHID = e.CCHID
		INNER JOIN UserContentPreference ucp
			ON uc.CCHID = ucp.CCHID
		INNER JOIN dbo.Campaign c
			on uc.CampaignID = c.CampaignID
		INNER JOIN fn_GetRegisteredMembers(GETDATE()) rm
			ON e.CCHID = rm.CCHID
	WHERE
		uc.CampaignID = @CampaignID
		AND ucp.SMSInd = 1
		
	UPDATE
		#SMSs
	SET
		KeyClient = 'Y'
	FROM
		#SMSs s
		INNER JOIN dbo.ExcludedMember em
			on s.CCHID = em.CCHID
	WHERE
		em.ExcludeReasonDesc = 'Key Client'
		
--------------------------------------------------------------------
--This returns the file for Constant Contact
--------------------------------------------------------------------		
	SELECT * FROM #SMSs
--------------------------------------------------------------------
--This updates the notification sent date on the User Content table
---------------------------------------------------------------------
	
	UPDATE 
		dbo.UserContent 
	SET 
		NotificationSentDate = GETDATE()
	FROM 
		dbo.UserContent uc, #SMSs e
	WHERE 
		uc.CampaignID = @CampaignID
		AND uc.CCHID = e.CCHID
	
END
 
 GO
