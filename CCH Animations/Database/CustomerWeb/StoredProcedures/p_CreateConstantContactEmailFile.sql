/****** Object:  StoredProcedure [dbo].[p_CreateConstantContactEmailFile]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_CreateConstantContactEmailFile]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_CreateConstantContactEmailFile]
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
      exec p_CreateConstantContactEmailFile
		@CampaignID = 1

Objects Listing:

Tables- dbo.Campaign
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
2015-01-20	AS		 Updated to include employee savings
2015-01-30	AS		 Updated to flag instead of remove key clients
2015-02-05  AS		 Updated to pass Savings Month Start Date to fn_GetSavingsForCCHID
2015-07-01  AS       Updated to pull yourcost savings, employer phone and employer email
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_CreateConstantContactEmailFile] (
	@CampaignID int
)
as

BEGIN

	DECLARE
		@EmployerID int
		,@EmployerPhone nvarchar(50)
		,@EmployerEmail nvarchar(50)
		,@ContentID int = 0
		
	SELECT
		@EmployerID =EmployerID
	FROM 
		ControlData

	SELECT @EmployerPhone = ConfigValue
	FROM dbo.ClientConfig
	WHERE ConfigKey = 'EmployerPhone'
	
	SELECT @EmployerEmail = ConfigValue
	FROM dbo.ClientConfig
	WHERE ConfigKey = 'EmployerEmail'
	
		
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
--This pulls everyone in the campaign who wants email notifications
--------------------------------------------------------------------
	SELECT DISTINCT
		e.CCHID,
		e.FirstName,
		e.LastName,
		rm.Email,
		cm.Savings,
		cm.YourCostSavingsAmt,
		'N' as KeyClient,
		@EmployerPhone as EmployerPhone,
		@EmployerEmail as EmployerEmail,
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
	INTO #Emails
	FROM
		dbo.UserContent uc
		INNER JOIN CampaignMember cm
			ON uc.CCHID = cm.CCHID
			AND uc.CampaignID = cm.CampaignID
		INNER JOIN dbo.Enrollments e
			ON uc.CCHID = e.CCHID
		INNER JOIN dbo.UserContentPreference ucp
			ON uc.CCHID = ucp.CCHID
		INNER JOIN dbo.Campaign c
			on uc.CampaignID = c.CampaignID
		INNER JOIN dbo.fn_GetRegisteredMembers(GETDATE()) rm
			ON e.CCHID = rm.CCHID
	WHERE
		uc.CampaignID = @CampaignID
		AND ucp.EmailInd = 1
	
	UPDATE
		#Emails
	SET
		KeyClient = 'Y'
	FROM
		#Emails e
		INNER JOIN ExcludedMember em
			on e.CCHID = em.CCHID
	WHERE
		em.ExcludeReasonDesc = 'Key Client'
		
	SELECT
		CCHID,
		FirstName,
		LastName,
		Email,
		Savings,
		YourCostSavingsAmt,
		KeyClient,
		EmployerPhone,
		EmployerEmail,
		ProcessingURL,
		DevURL,
		AlphaURL,
		Alpha2URL,
		LiveURL
	FROM
		#Emails
		
--------------------------------------------------------------------
--This updates the notification sent date on the User Content table
---------------------------------------------------------------------
	
	UPDATE 
		dbo.UserContent 
	SET 
		NotificationSentDate = GETDATE()
	FROM 
		dbo.UserContent uc, #Emails e
	WHERE 
		uc.CampaignID = @CampaignID
		AND uc.CCHID = e.CCHID
	
END
 
GO
