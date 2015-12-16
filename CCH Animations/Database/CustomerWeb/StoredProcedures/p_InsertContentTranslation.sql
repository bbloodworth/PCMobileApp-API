
 /****** Object:  StoredProcedure [dbo].[p_InsertContentTranslation]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertContentTranslation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertContentTranslation]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-04-13
Description:
      Insert Content Translation
      
Declarations:
      
Execute:
      exec p_InsertContentTranslation
		@ContentID = 1,
		@LocaleID = 2,
		@Title = 'Watch your video',
		@Description = 'Some description',
		@Caption = 'You watched your video!'
		@SMSNotificationText = 'New video available',
	
Objects Listing:

Tables- dbo.Message
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
2015-01-20  AS		 Updated to include ContentName and IntroContentInd for dynamic animations website
2015-04-02  AS		 Updated to include new columns: ContentURL and ContentPhoneNum
2015-04-10  AS	     Updated for Localization
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertContentTranslation] (
	@ContentID int,
	@Title nvarchar(50),
	@Description nvarchar(2000) = NULL,
	@Caption nvarchar(250) = NULL,
	@SMSNotificationText nvarchar(2000) = NULL,
	@OSNotificationText nvarchar(2000) = NULL,
	@LocaleCode nvarchar(10) = NULL,
	@LocaleID int = NULL
)
as

BEGIN

-----------------------------------------------------------
--Set the Locale (default to en-us) and get the LocaleID
-----------------------------------------------------------
	IF @LocaleID is NULL
	BEGIN
		IF @LocaleCode is NULL
			SET @LocaleCode = 'en-us'
	
		SELECT @LocaleID = LocaleID FROM dbo.Locale WHERE LocaleCode = @LocaleCode
	END

------------------------------------------------------------------
--Insert the translation fields
-------------------------------------------------------------------
	INSERT INTO dbo.ContentTranslation(
		ContentID
		,LocaleID
		,ContentTitle
		,ContentCaptionText
		,SMSNotificationText
		,OSNotificationText
		,ContentDesc
		,CreateDate )
	VALUES (
		@ContentID
		,@LocaleID
		,@Title
		,@Caption
		,@SMSNotificationText
		,@OSNotificationText
		,@Description
		,GETDATE()
	)
		
END

GO

