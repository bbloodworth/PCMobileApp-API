
 /****** Object:  StoredProcedure [dbo].[p_InsertQuestionTranslation]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertQuestionTranslation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertQuestionTranslation]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2015-04-13
Description:
      Insert Question Translation
      
Declarations:
      
Execute:
      exec p_InsertQuestionTranslation
		@QuestionID = 1,
		@LocaleID = 2,
		@QuestionText = 'What is the deductible on your healthplan?'
	
Objects Listing:

Tables- dbo.Message
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-17  AS       Created
2015-01-20  AS		 Updated to include QuestionName and IntroQuestionInd for dynamic animations website
2015-04-02  AS		 Updated to include new columns: QuestionURL and QuestionPhoneNum
2015-04-10  AS	     Updated for Localization
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertQuestionTranslation] (
	@QuestionID int,
	@QuestionText nvarchar(250),
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
	INSERT INTO dbo.QuestionTranslation(
		QuestionID
		,LocaleID
		,QuestionText
		,CreateDate )
	VALUES (
		@QuestionID
		,@LocaleID
		,@QuestionText
		,GETDATE()
	)
		
END

GO

