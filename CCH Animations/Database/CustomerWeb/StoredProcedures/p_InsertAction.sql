
/****** Object:  StoredProcedure [dbo].[p_InsertAction]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertAction]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertAction]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert Action
      
Declarations:
      
Execute:
      exec p_InsertAction
		@Title = 'Watch your video',
		@Description = 'Some description',
		@Source = 'Internal',
		@Caption = 'You watched your video!'
	
Objects Listing:

Tables- dbo.Action
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
2015-01-20	AS		 Updated to include ContentName and IntroContentInd for dynamic animation website
2015-04-02  AS		 Updated to include new columns: ContentURL and ContentPhoneNum
2015-04-10  AS	     Updated for Localization
2015-09-04  AS       Updated to add SMSNotificationText (PC-435) and AccumulatorsInd (PC-413)
2015-10-04  AS		 Updated to incude OSNotificationText
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertAction] (
	@Title nvarchar(50),
	@ContentName nvarchar(100) = NULL,
	@Description nvarchar(2000) = NULL,
	@Source nvarchar(100) = NULL,
	@ImageFileName nvarchar(100) = NULL,
	@FileLocation nvarchar(100) = NULL,
	@PointsCount int = NULL,
	@DurationInSeconds int = NULL,
	@IntroContentInd bit = NULL,
	@Caption nvarchar(250) = NULL,
	@ContentURL nvarchar(100) = NULL,
	@ContentPhoneNum nvarchar(50) = NULL,
	@LocaleCode nvarchar(10) = NULL,
	@AccumulatorsInd bit = 0,
	@SMSNotificationText nvarchar(2000) = NULL,
	@OSNotificationText nvarchar(2000) = NULL
)
as

BEGIN

	DECLARE
		@ContentTypeID int,
		@ContentID int,
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
		ct.ContentTypeDesc = 'Action'
		
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
		
	SELECT @ContentID = SCOPE_IDENTITY()
------------------------------------------------------------------
--Insert the translation fields if supplied
-------------------------------------------------------------------
	IF @Title IS NOT NULL OR @Description IS NOT NULL OR @Caption IS NOT NULL or @SMSNotificationText IS NOT NULL
	BEGIN
	
		exec p_InsertContentTranslation
			@ContentID = @ContentID
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
 