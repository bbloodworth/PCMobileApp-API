
 /****** Object:  StoredProcedure [dbo].[p_InsertAnswerTranslation]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertAnswerTranslation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertAnswerTranslation]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-04-13
Description:
      Insert Answer Translation
      
Declarations:
      
Execute:
      exec p_InsertAnswerTranslation
		@AnswerID = 1,
		@LocaleID = 2,
		@AnswerText = 'My healthplan'
	
Objects Listing:

Tables- dbo.Message
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
2015-01-20  AS		 Updated to include AnswerName and IntroAnswerInd for dynamic animations website
2015-04-02  AS		 Updated to include new columns: AnswerURL and AnswerPhoneNum
2015-04-10  AS	     Updated for Localization
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertAnswerTranslation] (
	@AnswerID int,
	@AnswerText nvarchar(250),
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
	INSERT INTO dbo.AnswerTranslation(
		AnswerID
		,LocaleID
		,AnswerText
		,CreateDate )
	VALUES (
		@AnswerID
		,@LocaleID
		,@AnswerText
		,GETDATE()
	)
		
END

GO

