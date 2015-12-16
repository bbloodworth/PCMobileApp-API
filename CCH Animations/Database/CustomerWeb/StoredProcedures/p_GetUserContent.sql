
/****** Object:  StoredProcedure [dbo].[p_GetUserContent]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetUserContent]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetUserContent]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Gets the user's notification queue
      
Declarations:
            
Execute:
      exec p_GetUserContent
		@CCHID= 57020
		,@Pagesize = 2
		,@BaseContentID = 4

Objects Listing:

Tables- dbo.Campaign
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
2015-01-15	AS		 Updated to include PageSize, BaseContentID and RelatedContent (PC-38)
2015-01-15	AS		 Add CampaignID to output (PC-49)
2015-03-15  AS		 Add Employer Notifications (from Campaign 0) into the user's queue
2015-04-02  AS		 Add in ContentURL and ContentPhoneNum
2015-04-10  AS	     Updated for Localization
2015-07-24  AS       Updated to consider activation and expiration date for UserContent 
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetUserContent] (
	@CCHID int
	,@LocaleCode nvarchar(10) = NULL
	,@PageSize int = NULL
	,@BaseCampaignID int = NULL
	,@BaseContentID int = NULL
)
as

BEGIN
-----------------------------------------------------------
--Set the User's Preferred Locale (default to en-us)
-----------------------------------------------------------
IF @LocaleCode is NULL
	SET @LocaleCode = ISNULL((SELECT l.LocaleCode from UserContentPreference u inner join Locale l on u.DefaultLocaleID = l.LocaleID WHERE CCHID = @CCHID), 'en-us')
ELSE IF SUBSTRING(@LocaleCode,1,2) = 'en'
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
-----------------------------------------------------------
--Get Client Config value for Points label
-----------------------------------------------------------
	DECLARE @PointsLabel nvarchar(100)
	
	SELECT 
		@PointsLabel = ConfigValue
	FROM
		dbo.ClientConfig
	WHERE
		ConfigKey = 'AnimationsPointsLabel'
------------------------------------------------------------
--First Get All Possible Content and Insert into temp table
-------------------------------------------------------------
	SELECT
		uc.CampaignID 
		,c.ContentID
		,c.ContentTypeID
		,ct.ContentTypeDesc
		,uc.ContentStatusID
		,cs.ContentStatusDesc
		,coalesce(ctn.ContentTitle, cte.ContentTitle) as ContentTitle
		,c.ContentName
		,coalesce(ctn.ContentCaptionText, cte.ContentCaptionText) as ContentCaptionText
		,coalesce(ctn.ContentDesc, cte.ContentDesc) as ContentDesc
		,c.ContentDurationSecondsCount
		,c.ContentImageFileName
		,c.ContentFileLocationDesc
		,c.ContentPointsCount
		,c.ContentURL
		,c.ContentPhoneNum
		,uc.ContentSavingsAmt
		,uc.MemberContentDataText
		,c.ContentSourceDesc
		,CASE WHEN ct.ContentTypeDesc = 'Survey' 
			THEN (select COUNT(questionID) from SurveyQuestion where SurveyID = c.ContentID)
			END as NumQuestions
		,uc.CreateDate
		,ISNULL(rc.ParentContentID, c.ContentID) as ParentContentID
		,1 as DisplayFlag
	INTO
		#AllContent
	FROM
		dbo.UserContent uc
		INNER JOIN dbo.CampaignContent cc
			on uc.CampaignID = cc.CampaignID
			AND uc.ContentID = cc.ContentID
		INNER JOIN dbo.ContentStatus cs
			on uc.ContentStatusID = cs.ContentStatusID
		INNER JOIN dbo.Content c
			on uc.ContentID = c.ContentID
		LEFT OUTER JOIN dbo.ContentTranslation ctn
			on c.ContentID = ctn.ContentID
			AND ctn.LocaleID = @PreferredLocaleID
		LEFT OUTER JOIN dbo.ContentTranslation cte
			on c.ContentID = cte.ContentID
			AND cte.LocaleID = @DefaultLocaleID
		INNER JOIN dbo.ContentType ct
			on c.ContentTypeID = ct.ContentTypeID
		LEFT JOIN dbo.RelatedContent rc
			on c.ContentID = rc.RelatedContentID
	WHERE
		uc.CCHID = @CCHID
		AND ct.ContentTypeDesc != 'Action'
		AND c.IntroContentInd = 0
		AND GETDATE() >= ISNULL(cc.ActivationDate,DATEADD(day,-1,GETDATE()))
		AND GETDATE() < ISNULL(cc.ExpirationDate,DATEADD(day,+1,GETDATE()))

------------------------------------------------------------------
-- AS: 3/18/2015 -- Add in "Campaign 0" - Employer Anonymous notifications
--------------------------------------------------------------------------
	UNION ALL
		SELECT
		cc.CampaignID 
		,c.ContentID
		,c.ContentTypeID
		,ct.ContentTypeDesc
		,NULL
		,NULL
		,coalesce(ctn.ContentTitle, cte.ContentTitle) as ContentTitle
		,c.ContentName
		,coalesce(ctn.ContentCaptionText, cte.ContentCaptionText) as ContentCaptionText
		,coalesce(ctn.ContentDesc, cte.ContentDesc) as ContentDesc
		,c.ContentDurationSecondsCount
		,c.ContentImageFileName
		,c.ContentFileLocationDesc
		,c.ContentPointsCount
		,c.ContentURL
		,c.ContentPhoneNum
		,NULL
		,NULL
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
		LEFT OUTER JOIN ContentTranslation ctn
			on c.ContentID = ctn.ContentID
			AND ctn.LocaleID = @PreferredLocaleID
		LEFT OUTER JOIN ContentTranslation cte
			on c.ContentID = cte.ContentID
			AND cte.LocaleID = @DefaultLocaleID
		INNER JOIN ContentType ct
			on c.ContentTypeID = ct.ContentTypeID
		LEFT JOIN RelatedContent rc
			on c.ContentID = rc.RelatedContentID
	WHERE
		cc.CampaignID = 0
		AND cc.UserContentInd = 1
		AND GETDATE() >= ISNULL(cc.ActivationDate,DATEADD(day,-1,GETDATE()))
		AND GETDATE() < ISNULL(cc.ExpirationDate,DATEADD(day,+1,GETDATE()))
		
-------------------------------------------------------------------
--Remove Content which shouldn't be displayed yet
-----------------------------------------------------------------

	SELECT
		uc.CampaignID 
		,cc.ContentID as RelatedContentID
		,pc.ContentID as ParentContentID
		,pc.ContentTypeID as ParentContentTypeID
		,cdr.ContentDisplayRuleDesc as RelatedContentDisplayRule
		,cs.ContentStatusID as ParentContentStatusID
		,cs.ContentStatusDesc as ParentContentStatusDesc
	INTO #RelatedContent
	FROM		
		RelatedContent rc
		INNER JOIN Content pc
			on rc.ParentContentID = pc.ContentID
		INNER JOIN Content cc
			on rc.RelatedContentID = cc.ContentID
		INNER JOIN ContentDisplayRule cdr
			on rc.RelatedContentDisplayRuleID = cdr.ContentDisplayRuleID
		INNER JOIN UserContent uc
			on pc.ContentID = uc.ContentID
		INNER JOIN ContentStatus cs
			on uc.ContentStatusID = cs.ContentStatusID
	WHERE
		uc.CCHID = @CCHID
		
	UPDATE #AllContent
	SET DisplayFlag = 0
	FROM
		#AllContent ac
		INNER JOIN #RelatedContent rc
			on ac.ContentID = rc.RelatedContentID
			and ac.CampaignID = rc.CampaignID
		INNER JOIN ContentTypeState cts
			on rc.ParentContentTypeID = cts.ContentTypeID
			AND rc.ParentContentStatusID = cts.ContentStatusID
	WHERE
		rtrim(ltrim(rc.RelatedContentDisplayRule)) = 'Display on Parent End State'
		AND cts.EndStateInd = 0
--------------------------------------------------------------------------
--Return the notification queue
---------------------------------------------------------------------------
	SELECT 
		*, ROW_NUMBER() OVER(ORDER BY CreateDate desc) AS seq
	INTO #ACSort
	FROM
		#AllContent
	WHERE
		DisplayFlag = 1
		
	DECLARE @BaseSeq int
	
	IF @PageSize IS NULL 
		SELECT @PageSize = MAX(seq) from #ACSort

	IF @BaseContentID IS NOT NULL and @BaseCampaignID IS NOT NULL
		SELECT @BaseSeq = seq 
		FROM #ACSort
		WHERE
			CampaignID = @BaseCampaignID 
			AND ContentID = @BaseContentID
	ELSE
		SELECT @BaseSeq = 1
			
	SELECT
		CampaignID 
		,ContentID
		,ContentTypeID
		,ContentTypeDesc
		,ContentStatusID
		,ContentStatusDesc
		,ContentTitle
		,ContentName
		,ContentCaptionText
		,ContentDesc
		,LTRIM(RTRIM(CONVERT(CHAR,ROUND((ContentDurationSecondsCount)/60,0,1)))) + ' ' + 'Minutes' as Duration
		,ContentImageFileName
		,ContentFileLocationDesc
		,LTRIM(RTRIM(CONVERT(CHAR,ContentPointsCount))) + ' ' + @PointsLabel as Points
		,ContentURL
		,ContentPhoneNum
		,ContentSavingsAmt
		,MemberContentDataText
		,ContentSourceDesc
		,NumQuestions
		,ParentContentID
		,CreateDate
	FROM 
		#ACSort
	WHERE
		seq BETWEEN @BaseSeq and @BaseSeq + (@PageSize - 1)
	ORDER by seq
	
END

GO


 
