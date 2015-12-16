
/****** Object:  StoredProcedure [dbo].[p_GetCampaignContent]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetCampaignContent]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetCampaignContent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-01-20
Description:
      Gets all content except intro content for a campaign
      
Declarations:
            
Execute:
      exec p_GetCampaignContent
		@CampaignID= 5

Objects Listing:

Tables- dbo.Campaign
dbo.Content
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-01-20  AS       Created
2015-06-17  AS		 Updated to support multiple languages
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetCampaignContent] (
	@CampaignID int
	,@LocaleCode nvarchar(5) = NULL
)
as

BEGIN
	DECLARE @LocaleID int
------------------------------------------------------------
--Set the LocaleCode
-------------------------------------------------------------
	IF SUBSTRING(@LocaleCode,1,2) = 'en'
		SET @LocaleCode = 'en-us'
	ELSE IF SUBSTRING(@LocaleCode,1,2) = 'es'
		SET @LocaleCode = 'es-us'
	ELSE 
		SET @LocaleCode = 'en-us'
	
-----------------------------------------------------------
--Get PreferredLocaleID and DefaultLocaleID
-----------------------------------------------------------
	DECLARE
		@PreferredLocaleID int
		,@DefaultLocaleID int
	
	SELECT @PreferredLocaleID = LocaleID	
	FROM dbo.Locale
	WHERE LocaleCode = @LocaleCode

	SELECT @DefaultLocaleID = LocaleID
	FROM dbo.Locale
	WHERE LocaleCode = 'en-us'
------------------------------------------------------------
--Select all content for campaign
-------------------------------------------------------------
	SELECT
		cc.CampaignID 
		,c.ContentID
		,c.ContentTypeID
		,ct.ContentTypeDesc
		,coalesce(ctn.ContentTitle, cte.ContentTitle) as ContentTitle
		,c.ContentName
		,coalesce(ctn.ContentCaptionText, cte.ContentCaptionText) as ContentCaptionText
		,coalesce(ctn.ContentDesc, cte.ContentDesc) as ContentDesc
		,c.ContentDurationSecondsCount
		,c.ContentImageFileName
		,c.ContentFileLocationDesc
		,c.ContentPointsCount
		,c.ContentSourceDesc
		,CASE WHEN ct.ContentTypeDesc = 'Survey' 
			THEN (select COUNT(questionID) from SurveyQuestion where SurveyID = c.ContentID)
			END as NumQuestions
		,cc.CreateDate
		,ISNULL(rc.ParentContentID, c.ContentID) as ParentContentID
		,1 as DisplayFlag
	FROM
		dbo.CampaignContent cc
		INNER JOIN Content c
			on cc.ContentID = c.ContentID
		INNER JOIN ContentType ct
			on c.ContentTypeID = ct.ContentTypeID
		LEFT OUTER JOIN dbo.ContentTranslation ctn
			on c.ContentID = ctn.ContentID
			AND ctn.LocaleID = @PreferredLocaleID
		LEFT OUTER JOIN dbo.ContentTranslation cte
			on c.ContentID = cte.ContentID
			AND cte.LocaleID = @DefaultLocaleID
		LEFT JOIN RelatedContent rc
			on c.ContentID = rc.RelatedContentID
	WHERE
		cc.CampaignID = @CampaignID
		AND ct.ContentTypeDesc != 'Action'
		AND c.IntroContentInd = 0
	ORDER BY
		cc.CreateDate desc
END

GO


