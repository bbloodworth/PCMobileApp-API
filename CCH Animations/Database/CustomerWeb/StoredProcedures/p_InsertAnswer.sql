
/****** Object:  StoredProcedure [dbo].[p_InsertAnswer]     ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_InsertAnswer]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_InsertAnswer]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
Author: AS
Create date: 2014-12-17
Description:
      Insert Answer
      
Declarations:
            
Execute:



Objects Listing:

Tables- dbo.Answer
    

UPDATES:
----------------------------------------------------------------------------------------------------
Date        Who      Description
----------  ---      -------------------------------------------------------------------------------
2014-12-18  AS       Created
2015-04-10  AS	     Updated for Localization
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

CREATE PROCEDURE [dbo].[p_InsertAnswer] (
	@AnswerText nvarchar(250)
	,@LocaleCode nvarchar(10) = NULL
)
as

DECLARE 
		@AnswerID int
		,@LocaleID int
-----------------------------------------------------------
--Set the Locale (default to en-us) and get the LocaleID
-----------------------------------------------------------
	IF @LocaleCode is NULL
		SET @LocaleCode = 'en-us'
	
	SELECT @LocaleID = LocaleID FROM dbo.Locale WHERE LocaleCode = @LocaleCode

--------------------------------------------------------------
--Insert the Answer Record
---------------------------------------------------------------

BEGIN

	INSERT INTO dbo.Answer (
		CreateDate)
	VALUES (
		getdate())
		
	SELECT @AnswerID = SCOPE_IDENTITY()
------------------------------------------------------------------
--Insert the translation fields if supplied
-------------------------------------------------------------------
	IF @AnswerText IS NOT NULL 
	BEGIN
	
		exec p_InsertAnswerTranslation
			@AnswerID = @AnswerID
			,@LocaleID = @LocaleID
			,@AnswerText = @AnswerText
	END --if		
END

GO


 
