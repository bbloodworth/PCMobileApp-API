
/****** Object:  StoredProcedure [dbo].[p_GetEmployerNotifications]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetEmployerNotifications]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetEmployerNotifications]
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
      exec p_GetEmployerNotifications
		,@Pagesize = 2
		,@BaseContentID = 4

Objects Listing:

Tables- dbo.Campaign
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2015-03-18  AS       Created
2015-04-10  AS	     Updated for Localization
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetEmployerNotifications] (
	@PageSize int = NULL
	,@BaseContentID int = NULL
	,@LocaleCode nvarchar(10) = NULL
)
as

BEGIN
-----------------------------------------------------------
--Set the User's Preferred Locale (default to en-us)
-----------------------------------------------------------
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
--First Get Employer Notification Content and insert into temp table
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
		,c.ContentURL
		,c.ContentPhoneNum
		,c.ContentSourceDesc
		,CASE WHEN ct.ContentTypeDesc = 'Survey' 
			THEN (select COUNT(questionID) from SurveyQuestion where SurveyID = c.ContentID)
			END as NumQuestions
		,cc.CreateDate
		,ISNULL(rc.ParentContentID, c.ContentID) as ParentContentID
		,1 as DisplayFlag
	INTO #AllContent
	FROM
		dbo.CampaignContent cc
		INNER JOIN Content c
			on cc.ContentID = c.ContentID
		LEFT OUTER JOIN dbo.ContentTranslation ctn
			on c.ContentID = ctn.ContentID
			AND ctn.LocaleID = @PreferredLocaleID
		LEFT OUTER JOIN dbo.ContentTranslation cte
			on c.ContentID = cte.ContentID
			AND cte.LocaleID = @DefaultLocaleID	
		INNER JOIN ContentType ct
			on c.ContentTypeID = ct.ContentTypeID
		LEFT JOIN RelatedContent rc
			on c.ContentID = rc.RelatedContentID
	WHERE
		cc.CampaignID = 0
		AND GETDATE() >= ISNULL(cc.ActivationDate, DATEADD(day,-1,GETDATE()))
		AND GETDATE() < ISNULL(cc.ExpirationDate, DATEADD(day,1,GETDATE()))

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

	IF @BaseContentID IS NOT NULL
		SELECT @BaseSeq = seq 
		FROM #ACSort
		WHERE
			ContentID = @BaseContentID
	ELSE
		SELECT @BaseSeq = 1
			
	SELECT
		CampaignID 
		,ContentID
		,ContentTypeID
		,ContentTypeDesc
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


 
