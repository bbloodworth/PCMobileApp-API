
/****** Object:  StoredProcedure [dbo].[p_GetSurvey]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_GetSurvey]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_GetSurvey]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Gets information about a Survey
      
Declarations:
            
Execute:
      exec p_GetSurvey
		@SureveyID= 1

Objects Listing:

Tables: 
dbo.Survey
dbo.Content

    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
2015-04-10  AS	     Updated for Localization
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_GetSurvey] (
	@SurveyID int
	,@LocaleCode nvarchar(10) = NULL
)
as

BEGIN

-----------------------------------------------------------
--Set the User's Preferred Locale (default to en-us)
-----------------------------------------------------------
IF @LocaleCode is NULL
	SET @LocaleCode = 'en-us'
ELSE IF SUBSTRING(@LocaleCode,1,2) = 'en'
	SET @LocaleCode = 'en-us'
ELSE IF SUBSTRING(@LocaleCode,1,2) = 'es'
	SET @LocaleCode = 'es-us'
ELSE 
	SET @LocaleCode = 'en-us'
	
-----------------------------------------------------------
--Get LocaleID
-----------------------------------------------------------
DECLARE
	@LocaleID int

SELECT @LocaleID = LocaleID	
FROM dbo.Locale
WHERE LocaleCode = @LocaleCode

	SELECT
		c.ContentID as SurveyID
		,ct.ContentTitle as SurveyTitle
		,ct.ContentCaptionText as SurveyCaptionText
		,ct.ContentDesc as SurveyDesc
		,c.ContentDurationSecondsCount as SurveyDuration
		,c.ContentImageFileName as SurveyImageFileName
		,c.ContentFileLocationDesc as SurveyFileLocationDesc
		,c.ContentPointsCount as SurveyPointsCount
		,c.ContentSourceDesc as SurveySourceDesc
		,s.SurveyPassCount
		,s.EmbeddedSurveyInd
		,s.CreateDate
	FROM
		dbo.Content c
		INNER JOIN dbo.ContentTranslation ct
			on c.ContentID = ct.ContentID
		INNER JOIN dbo.Survey s
			ON c.ContentID = s.SurveyID
	WHERE
		s.SurveyID = @SurveyID
		AND ct.LocaleID = @LocaleID
END
 
 GO
 

 
