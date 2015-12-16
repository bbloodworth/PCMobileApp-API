
/****** Object:  StoredProcedure [dbo].[p_InsertAnimation]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertAnimation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertAnimation]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert Animation
      
Declarations:
      
Execute:
      exec p_InsertAnimation
		@Title = 'Healthplan Explainer Video',
		@Description = 'This video will explain your healthplan',
		@Source = 'Internal',
		@ImageFileName = 'n:\imagefilename',
		@FileLocation = 'n:\animationfilename',
		@PointsCount = 15,
		@DurationInSeconds = 120,
		@Caption = 'How much do you know about your healthplan?',
		@AnimationScriptFileName = 'n:\script',
		@InteractiveAnimationInd = 1,
		@AnimationDataProcName = 'p_GetHealthplanExplainerMemberData',
		@AccumulatorsInd = 1,
		@SMSNotificationText = 'New Animation Available!'
	
Objects Listing:

Tables- dbo.Animation
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
2015-01-20  AS		 Updated to Include ContentName and Intro Content Ind for dynamic animations website
2015-04-02  AS		 Updated to include new columns: ContentURL and ContentPhoneNum
2015-04-10  AS	     Updated for Localization
2015-09-04  AS       Updated to include SMSNotificationText (PC-435) and AccumulatorsInd (PC-413)
2015-10-04  AS		 Updated to incude OSNotificationText
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertAnimation] (
	@Title nvarchar(50),
	@ContentName nvarchar(100) = NULL,
	@Description nvarchar(2000) = NULL,
	@Source nvarchar(100) = NULL,
	@ImageFileName nvarchar(100) = NULL,
	@FileLocation nvarchar(100) = NULL,
	@PointsCount int = NULL,
	@DurationInSeconds int = NULL,
	@Caption nvarchar(250) = NULL,
	@AnimationScriptFileName nvarchar(50) = NULL,
	@InteractiveAnimationInd bit = 0,
	@AnimationDataProcName nvarchar(50) = NULL,
	@IntroContentInd bit = NULL,
	@ContentURL nvarchar(100) = NULL,
	@ContentPhoneNum nvarchar(50) = NULL,
	@AccumulatorsInd bit = 0,
	@LocaleCode nvarchar(10) = NULL,
	@SMSNotificationText nvarchar(2000) = NULL,
	@OSNotificationText nvarchar(2000) = NULL
)
as

BEGIN

	DECLARE
		@ContentTypeID int,
		@AnimationID int,
		@LocaleID int
-----------------------------------------------------------
--Set the Locale (default to en-us) and get the LocaleID
-----------------------------------------------------------
	IF @LocaleCode is NULL
		SET @LocaleCode = 'en-us'
	
	SELECT @LocaleID = LocaleID FROM dbo.Locale WHERE LocaleCode = @LocaleCode

	SELECT 
		@ContentTypeID = ct.ContentTypeID
	FROM
		dbo.ContentType ct
	WHERE
		ct.ContentTypeDesc = 'Animation'
		
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
		,ISNULL(@IntroContentInd,0)
		,@ContentURL
		,@ContentPhoneNum
		,@AccumulatorsInd
		,getdate())
		
	SELECT @AnimationID = SCOPE_IDENTITY()
	
	INSERT INTO dbo.Animation (
		AnimationID
		,AnimationScriptFileName
		,InteractiveAnimationInd
		,AnimationDataProcName
		,CreateDate)
	VALUES (
		@AnimationID
		,@AnimationScriptFileName
		,@InteractiveAnimationInd
		,@AnimationDataProcName
		,GETDATE())
------------------------------------------------------------------
--Insert the translation fields if supplied
-------------------------------------------------------------------
	IF @Title IS NOT NULL OR @Description IS NOT NULL OR @Caption IS NOT NULL or @SMSNotificationText IS NOT NULL
	BEGIN
	
		exec p_InsertContentTranslation
			@ContentID = @AnimationID
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
 
