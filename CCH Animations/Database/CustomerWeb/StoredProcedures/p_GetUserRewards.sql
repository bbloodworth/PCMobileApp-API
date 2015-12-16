
/****** Object:  StoredProcedure [dbo].[p_GetUserRewards]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetUserRewards]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetUserRewards]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Gets the user's Rewards queue
      
Declarations:
            
Execute:
      exec p_GetUserRewards
		@CCHID= 57020

Objects Listing:

Tables- dbo.Campaign
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
2014-01-15  AS       Added CampaignID to results, and also added paging logic
2015-04-02  AS		 Add in ContentURL and ContentPhoneNum
2015-04-10  AS	     Updated for Localization
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetUserRewards] (
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
------------------------------------------------
--First Get Total Points & Total Savings for the User
------------------------------------------------
	SELECT
		SUM(c.ContentPointsCount) as TotalPoints,
		SUM(uc.ContentSavingsAmt) as TotalRewards
	FROM
		dbo.UserContent uc
		INNER JOIN ContentStatus cs
			on uc.ContentStatusID = cs.ContentStatusID
		INNER JOIN Content c
			on uc.ContentID = c.ContentID
		INNER JOIN ContentType ct
			on c.ContentTypeID = ct.ContentTypeID
		INNER JOIN ContentTypeState cts
			on ct.ContentTypeID = cts.ContentTypeID
			and cs.ContentStatusID = cts.ContentStatusID
	WHERE
		uc.CCHID = @CCHID
		AND cts.EndStateInd = 1
		AND cs.ContentStatusDesc != 'Failed'
		
------------------------------------------------
--Now pull all the detail for that
------------------------------------------------
		
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
		,ROW_NUMBER() OVER(ORDER BY uc.CreateDate desc) AS seq
	INTO
		#AllRewards
	FROM
		dbo.UserContent uc
		INNER JOIN ContentStatus cs
			on uc.ContentStatusID = cs.ContentStatusID
		INNER JOIN Content c
			on uc.ContentID = c.ContentID
		LEFT OUTER JOIN dbo.ContentTranslation ctn
			on c.ContentID = ctn.ContentID
			AND ctn.LocaleID = @PreferredLocaleID
		LEFT OUTER JOIN dbo.ContentTranslation cte
			on c.ContentID = cte.ContentID
			AND cte.LocaleID = @DefaultLocaleID	
		INNER JOIN ContentType ct
			on c.ContentTypeID = ct.ContentTypeID
		INNER JOIN ContentTypeState cts
			on ct.ContentTypeID = cts.ContentTypeID
			and cs.ContentStatusID = cts.ContentStatusID
	WHERE
		uc.CCHID = @CCHID
		AND cts.EndStateInd = 1
		AND cs.ContentStatusDesc != 'Failed'
	ORDER BY
		uc.CreateDate desc
--------------------------------------------------
--Paging logic here
---------------------------------------------------
		
	DECLARE @BaseSeq int
	
	IF @PageSize IS NULL 
		SELECT @PageSize = MAX(seq) from #AllRewards

	IF @BaseContentID IS NOT NULL and @BaseCampaignID IS NOT NULL
		SELECT @BaseSeq = seq 
		FROM #AllRewards
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
		,CreateDate
	FROM
		#AllRewards
	WHERE
		seq BETWEEN @BaseSeq and @BaseSeq + (@PageSize - 1)
	ORDER by seq
		
	
END
 
GO


