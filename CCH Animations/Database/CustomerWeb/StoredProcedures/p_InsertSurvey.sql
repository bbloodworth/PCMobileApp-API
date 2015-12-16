
/****** Object:  StoredProcedure [dbo].[p_InsertSurvey]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertSurvey]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertSurvey]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert Survey
      
Declarations:
      
Execute:
      exec p_InsertSurvey
		@Title = 'Healthplan Explainer Video',
		@Description = 'This video will explain your healthplan',
		@Source = 'Internal',
		@ImageFileName = 'n:\imagefilename',
		@FileLocation = 'n:\Surveyfilename',
		@PointsCount = 15,
		@DurationInSeconds = 120,
		@Caption = 'How much do you know about your healthplan?',
		@SurveyPassCount = 30,
		@EmbeddedSurveyInd = 1,
		@SMSNotificationText = 'New survey available!'
	
Objects Listing:

Tables- dbo.Survey
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
2015-01-20	AS		 Updated to include ContentName and IntroContentInd for dynamic animations website
2015-04-02  AS		 Updated to include new columns: ContentURL and ContentPhoneNum
2015-04-10  AS	     Updated for Localization
2015-09-04  AS       Updated for SMSNotificationText (PC-435) & Accumulators Ind (PC-413)
2015-10-04  AS		 Updated to include OSNotificationText
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertSurvey] (
	@Title nvarchar(50),
	@ContentName nvarchar(100) = NULL,
	@Description nvarchar(2000) = NULL,
	@Source nvarchar(100) = NULL,
	@ImageFileName nvarchar(100) = NULL,
	@FileLocation nvarchar(100) = NULL,
	@PointsCount int = NULL,
	@DurationInSeconds int = NULL,
	@Caption nvarchar(250) = NULL,
	@SurveyPassCount int = 0,
	@IntroContentInd bit = 0,
	@EmbeddedSurveyInd bit = 0,
	@ContentURL nvarchar(100) = NULL,
	@ContentPhoneNum nvarchar(50) = NULL,
	@LocaleCode nvarchar(10) = NULL,
	@SMSNotificationText nvarchar(2000) = NULL,
	@OSNotificationText nvarchar(2000) = NULL,
	@AccumulatorsInd bit = 0
)
as

BEGIN

	DECLARE
		@ContentTypeID int,
		@SurveyID int,
		@LocaleID int
-----------------------------------------------------------
--Set the Locale (default to en-us) and get the LocaleID
-----------------------------------------------------------
	IF @LocaleCode is NULL
		SET @LocaleCode = 'en-us'
	
	SELECT @LocaleID = LocaleID FROM dbo.Locale WHERE LocaleCode = @LocaleCode
---------------------------------------------------------------
--Insert the content record
---------------------------------------------------------------
	SELECT 
		@ContentTypeID = ct.ContentTypeID
	FROM
		dbo.ContentType ct
	WHERE
		ct.ContentTypeDesc = 'Survey'
		
	INSERT INTO dbo.Content (
		ContentTypeID
		--,ContentTitle
		,ContentName
		--,ContentDesc
		,ContentSourceDesc
		,ContentImageFileName
		,ContentFileLocationDesc
		,ContentPointsCount
		,ContentDurationSecondsCount
		--,ContentCaptionText
		,IntroContentInd
		,ContentURL
		,ContentPhoneNum
		,AccumulatorsInd
		,CreateDate)
	VALUES (
		@ContentTypeID
		--,@Title
		,@ContentName
		--,@Description
		,@Source
		,@ImageFileName
		,@FileLocation
		,@PointsCount
		,@DurationInSeconds
		--,@Caption
		,@IntroContentInd
		,@ContentURL
		,@ContentPhoneNum
		,@AccumulatorsInd
		,getdate())
		
	SELECT @SurveyID = SCOPE_IDENTITY()
	
	INSERT INTO dbo.Survey (
		SurveyID
		,SurveyPassCount
		,EmbeddedSurveyInd
		,CreateDate)
	VALUES (
		@SurveyID
		,@SurveyPassCount
		,@EmbeddedSurveyInd
		,GETDATE())
		
------------------------------------------------------------------
--Insert the translation fields if supplied
-------------------------------------------------------------------
	IF @Title IS NOT NULL OR @Description IS NOT NULL OR @Caption IS NOT NULL or @SMSNotificationText IS NOT NULL
	BEGIN
	
		exec p_InsertContentTranslation
			@ContentID = @SurveyID
			,@Title = @Title
			,@Description = @Description
			,@Caption = @Caption
			,@SMSNotificationText = @SMSNotificationText
			,@OSNotificationText = @OSNotificationText
			,@LocaleCode = @LocaleCode
			,@LocaleID = @LocaleID
	END --if
END
 
GO

