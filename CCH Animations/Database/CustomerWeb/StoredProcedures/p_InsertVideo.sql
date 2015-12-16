
/****** Object:  StoredProcedure [dbo].[p_InsertVideo]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertVideo]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertVideo]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert Video
      
Declarations:
      
Execute:
      exec p_InsertVideo
		@Title = 'Healthplan Video',
		@Description = 'A Video designed to see how much you know about your healthplan.',
		@Source = 'Internal',
		@ImageFileName = 'n:\imagefilename',
		@FileLocation = 'n:\Videofilename',
		@PointsCount = 50,
		@DurationInSeconds = 120,
		@Caption = 'How much do you know about your healthplan?',
		@VideoFileName = 'n:\yetanotherfilename',
		@PersonalizedVideoInd = 1,
		@VideoDataProcName = 'p_GetVideoMemberData',
		@SMSNotificationText = 'New video available!'
	
Objects Listing:

Tables- dbo.Video
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
2015-01-20  AS		 Updated to include ContentName and IntroContentInd for dynamic animations website
2015-04-02  AS		 Updated to include new columns: ContentURL and ContentPhoneNum
2015-04-10  AS	     Updated for Localization
2015-09-04  AS		 Updated to include SMSNotificationText
2015-10-04  AS		 Updated to incude OSNotificationText
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertVideo] (
	@Title nvarchar(50),
	@ContentName nvarchar(100) = NULL,
	@Description nvarchar(2000) = NULL,
	@Source nvarchar(100) = NULL,
	@ImageFileName nvarchar(100) = NULL,
	@FileLocation nvarchar(100) = NULL,
	@PointsCount int = NULL,
	@DurationInSeconds int = NULL,
	@Caption nvarchar(250) = NULL,
	@VideoFileName nvarchar(50) = NULL,
	@PersonalizedVideoInd bit = 0,
	@VideoDataProcName nvarchar(50) = NULL,
	@IntroContentInd bit = NULL,
	@ContentURL nvarchar(100) = NULL,
	@ContentPhoneNum nvarchar(50) = NULL,
	@LocaleCode nvarchar(10) = NULL,
	@SMSNotificationText nvarchar(2000) = NULL,
	@OSNotificationText nvarchar(2000) = NULL
)
as

BEGIN

	DECLARE
		@ContentTypeID int,
		@VideoID int,
		@LocaleID int
		
-----------------------------------------------------------
--Set the Locale (default to en-us) and get the LocaleID
-----------------------------------------------------------
	IF @LocaleCode is NULL
		SET @LocaleCode = 'en-us'
	
	SELECT @LocaleID = LocaleID FROM dbo.Locale WHERE LocaleCode = @LocaleCode

--------------------------------------------------------------
--Insert the Contet Record
---------------------------------------------------------------

	SELECT 
		@ContentTypeID = ct.ContentTypeID
	FROM
		dbo.ContentType ct
	WHERE
		ct.ContentTypeDesc = 'Video'
		
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
		,getdate())
		
	SELECT @VideoID = SCOPE_IDENTITY()
	
	INSERT INTO dbo.Video (
		VideoID
		,VideoFileName
		,PersonalizedVideoInd
		,VideoDataProcName
		,CreateDate)
	VALUES (
		@VideoID
		,@VideoFileName
		,@PersonalizedVideoInd
		,@VideoDataProcName
		,GETDATE())
------------------------------------------------------------------
--Insert the translation fields if supplied
-------------------------------------------------------------------
	IF @Title IS NOT NULL OR @Description IS NOT NULL OR @Caption IS NOT NULL or @SMSNotificationText IS NOT NULL
	BEGIN
	
		exec p_InsertContentTranslation
			@ContentID = @VideoID
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

